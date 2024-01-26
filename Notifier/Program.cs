using Amazon.S3;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Amazon.SQS;
using Amazon.SQS.ExtendedClient;
using Amazon.SQS.Model;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.IO;
using Notifier.Model;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts => opts.EnableAnnotations());
builder.Services.AddSwaggerGen();
builder.Services.AddLocalStack(builder.Configuration);
builder.Services.AddAWSServiceLocalStack<IAmazonSQS>();
builder.Services.AddAWSServiceLocalStack<IAmazonS3>();

var useLocalStack = builder.Configuration.GetSection("SQSConfig").GetSection("useLocalStack").Get<bool>();
var sqsClient = new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(
        string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MY_SECRET")) 
        ? "us-east-1"
        : Environment.GetEnvironmentVariable("MY_SECRET")));

if (!useLocalStack)
    sqsClient = await ConfigurarSQS();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var pedido = app.MapGroup("/pedido").WithTags("Pedido");

pedido.MapPost("/", EnviarConfirmacaoPedido).WithName("EnviarConfirmacaoPedido").WithMetadata(new SwaggerOperationAttribute(summary: "Criar pedido", description: "Cria um novo pedido")).Produces(201).Produces(400).Produces(404).Produces(500);

app.Run();

async Task<IResult> EnviarConfirmacaoPedido(IConfiguration configuration, IAmazonSQS sqs, IAmazonS3 s3, MessageBody message)
{
    try
    {
        var queueUrl = configuration.GetSection("QueueUrl").Value;

        if (useLocalStack)
        {
            var criarFila = configuration.GetSection("SQSConfig").GetSection("CreateTestQueue").Get<bool>();
            var enviarMensagem = configuration.GetSection("SQSConfig").GetSection("SendTestMessage").Get<bool>();

            var configSQS = ConfigurarSQSBasic(configuration, sqs, s3);

            if (criarFila)
                await CreateMessageInQueueWithStatusASync(configuration, configSQS);

            if (enviarMensagem)
            {
                await SendTestMessageAsync(queueUrl, configSQS);
            }
            else
            {
                var jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                await sqs.SendMessageAsync(queueUrl, jsonMessage);
            }
        }
        else
        {
            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            await EnviarParaSQS(messageJson, sqsClient);
        }


        return TypedResults.Created($"/pedido/{message.idPedido}", message);
    }
    catch (Exception ex)
    {
        return TypedResults.Problem($"Erro ao criar o pedido.");
    }
}

AmazonSQSExtendedClient ConfigurarSQSBasic(IConfiguration configuration, IAmazonSQS sqs, IAmazonS3 s3)
{
    var bucketName = configuration.GetSection("SQSExtendedClient").GetSection("S3Bucket").Value;
    var amazonSQS = new AmazonSQSExtendedClient(sqs, new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3, bucketName));

    return amazonSQS;
}

async Task SendTestMessageAsync(string queue, AmazonSQSExtendedClient sqs)
{
    var messageBody = new MessageBody();
    messageBody.IdTransacao = Guid.NewGuid().ToString();
    messageBody.idPedido = "65a315fadb1f522d916d9361";
    messageBody.Status = "OK";
    messageBody.DataTransacao = DateTime.Now;

    var jsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(messageBody);

    await sqs.SendMessageAsync(queue, jsonObj);
}

async Task CreateMessageInQueueWithStatusASync(IConfiguration configuration, AmazonSQSExtendedClient sqs)
{
    var name = configuration.GetSection("SQSConfig").GetSection("TestQueueName").Value;
    var responseQueue = await sqs.CreateQueueAsync(new CreateQueueRequest(name));

    if (responseQueue.HttpStatusCode != HttpStatusCode.OK)
    {
        //_logger.LogError($"Error creating the queue: {name}!");
        throw new AmazonSQSException($"Failed to CreateQueue for queue {name}. Response: {responseQueue.HttpStatusCode}");
    }
}

async Task<AmazonSQSClient> ConfigurarSQS()
{
    using (var secretsManagerClient = new AmazonSecretsManagerClient())
    {
        var secretName = Environment.GetEnvironmentVariable("MY_SECRET");
        var getSecretValueRequest = new GetSecretValueRequest
        {
            SecretId = secretName
        };

        var getSecretValueResponse = await secretsManagerClient.GetSecretValueAsync(getSecretValueRequest);
        var secretString = getSecretValueResponse.SecretString;

        // Parse the secretString to get SQS connection details
        var sqsConnectionDetails = ParseSecretString(secretString);

        // Initialize the AmazonSQS client with the retrieved credentials
        var sqsConfig = new AmazonSQSConfig
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(sqsConnectionDetails.Region)
        };

        var sqsClient = new AmazonSQSClient(sqsConnectionDetails.AccessKeyId, sqsConnectionDetails.SecretAccessKey, sqsConfig);

        return sqsClient;
    }
}


async Task EnviarParaSQS(string jsonMessage, AmazonSQSClient sqsClient)
{
    try
    {
        // Use sqsClient to perform SQS operations
        var listQueuesResponse = await sqsClient.ListQueuesAsync(new ListQueuesRequest());
        foreach (var queueUrl in listQueuesResponse.QueueUrls)
        {
            Console.WriteLine($"SQS Queue URL: {queueUrl}");
            await sqsClient.SendMessageAsync(queueUrl, jsonMessage);
        }
    }
    catch(Exception ex)
    {
        //logar;
    }
  
}

static SqsConnectionDetails ParseSecretString(string secretString)
{
    return Newtonsoft.Json.JsonConvert.DeserializeObject<SqsConnectionDetails>(secretString);
}

class SqsConnectionDetails
{
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }
    public string Region { get; set; }
}

