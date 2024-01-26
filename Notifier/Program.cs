using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.ExtendedClient;
using Amazon.SQS.Model;
using Infra.Configurations.SQS;
using LocalStack.Client.Extensions;
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
builder.Services.AddTransient<ISQSConfiguration, SQSConfiguration>();


var queueUrl = builder.Configuration.GetSection("QueueUrl").Value;
var criarFila = builder.Configuration.GetSection("SQSConfig").GetSection("CreateTestQueue").Get<bool>();
var enviarMensagem = builder.Configuration.GetSection("SQSConfig").GetSection("SendTestMessage").Get<bool>();
var useLocalStack = builder.Configuration.GetSection("SQSConfig").GetSection("useLocalStack").Get<bool>();


//config SQS AWS
var sqsClient = new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(
        string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MY_SECRET")) || Environment.GetEnvironmentVariable("MY_SECRET").Equals("{MY_SECRET}")
        ? "us-east-1"
        : Environment.GetEnvironmentVariable("MY_SECRET")));


if (!useLocalStack)
{
    var sqsConfiguration = new SQSConfiguration();
    sqsClient = await sqsConfiguration.ConfigurarSQS();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var pedido = app.MapGroup("/pedido").WithTags("Pedido");

pedido.MapPost("/", EnviarConfirmacaoPedido).WithName("EnviarConfirmacaoPedido").WithMetadata(new SwaggerOperationAttribute(summary: "Criar pedido", description: "Cria um novo pedido")).Produces(201).Produces(400).Produces(404).Produces(500);

app.Run();

async Task<IResult> EnviarConfirmacaoPedido(IConfiguration configuration, ISQSConfiguration sqsConfiguration, IAmazonSQS sqs, IAmazonS3 s3, MessageBody message)
{
    try
    {
        if (useLocalStack)
        {
            var bucketName = builder.Configuration.GetSection("SQSExtendedClient").GetSection("S3Bucket").Value;
            var configSQS = new AmazonSQSExtendedClient(sqs, new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3, bucketName));

            if (criarFila)
                await CreateMessageInQueueWithStatusASyncLocalStack(builder.Configuration, configSQS);

            if (enviarMensagem)
                await SendTestMessageAsyncLocalStack(queueUrl, configSQS);


            var jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            await configSQS.SendMessageAsync(queueUrl, jsonMessage);
        }
        else
        {
            var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            await sqsConfiguration.EnviarParaSQS(messageJson, sqsClient);
        }


        return TypedResults.Created($"/pedido/{message.idPedido}", message);
    }
    catch (Exception ex)
    {
        return TypedResults.Problem($"Erro ao criar o pedido.");
    }
}

async Task SendTestMessageAsyncLocalStack(string queue, AmazonSQSExtendedClient sqs)
{
    var messageBody = new MessageBody();
    messageBody.IdTransacao = Guid.NewGuid().ToString();
    messageBody.idPedido = "65a315fadb1f522d916d9361";
    messageBody.Status = "OK";
    messageBody.DataTransacao = DateTime.Now;

    var jsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(messageBody);

    await sqs.SendMessageAsync(queue, jsonObj);
}

async Task CreateMessageInQueueWithStatusASyncLocalStack(IConfiguration configuration, AmazonSQSExtendedClient sqs)
{
    var name = configuration.GetSection("SQSConfig").GetSection("TestQueueName").Value;
    var responseQueue = await sqs.CreateQueueAsync(new CreateQueueRequest(name));

    if (responseQueue.HttpStatusCode != HttpStatusCode.OK)
    {
        //_logger.LogError($"Error creating the queue: {name}!");
        throw new AmazonSQSException($"Failed to CreateQueue for queue {name}. Response: {responseQueue.HttpStatusCode}");
    }
}



