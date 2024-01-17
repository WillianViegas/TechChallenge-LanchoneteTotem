using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.ExtendedClient;
using Amazon.SQS.Model;
using Application.UseCases.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PagamentoPedidoNotificationConsumer.Model;
using System.Net;

namespace PagamentoPedidoNotificationConsumer.Service
{
    public class PagamentoPedidoConsumer : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IPedidoUseCase _pedidoUseCase;
        private readonly string _queueUrl;
        private readonly IAmazonSQS _amazonSQS;


        public PagamentoPedidoConsumer(ILogger<PagamentoPedidoConsumer> logger, IConfiguration configuration, IPedidoUseCase pedidoUseCase, IAmazonSQS sqs, IAmazonS3 s3)
        {
            _logger = logger;
            _configuration = configuration;
            _pedidoUseCase = pedidoUseCase;
            _queueUrl = configuration.GetSection("QueueUrl").Value;

            var bucketName = configuration.GetSection("SQSExtendedClient").GetSection("S3Bucket").Value;
            _amazonSQS = new AmazonSQSExtendedClient(sqs, new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3, bucketName));

            LastActivity = DateTime.Now;
        }

        public DateTime LastActivity { get; set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(200, stoppingToken);

            var createTestQueue = _configuration.GetSection("Test").GetSection("CreateTestQueue").Get<bool>();
            var sendTestMessage = _configuration.GetSection("Test").GetSection("SendTestMessage").Get<bool>();

            if (createTestQueue)
            {
                await CreateMessageInQueueWithStatusASync();

                if (sendTestMessage)
                {
                    await SendTestMessageAsync();
                }
            }


            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Reading queue...");

                    var response = await _amazonSQS.ReceiveMessageAsync(new ReceiveMessageRequest
                    {
                        QueueUrl = _queueUrl,
                        WaitTimeSeconds = 10,
                        AttributeNames = new List<string> { "ApproximateReceiveCount" },
                        MessageAttributeNames = new List<string> { "All" },
                        MaxNumberOfMessages = 10
                    });

                    if (response.HttpStatusCode != HttpStatusCode.OK)
                    {
                        _logger.LogError($"Error creating the queue: {_queueUrl}!");
                        throw new AmazonSQSException($"Failed to GetMessages for queue {_queueUrl}. Response: {response.HttpStatusCode}");
                    }

                    foreach (var message in response.Messages)
                    {

                        var obj = TratarMessage(message.Body);

                        if (obj == null)
                            continue;


                        //chamar confirmar pedido
                        var pedido = await _pedidoUseCase.ConfirmarPedido(obj.idPedido);
                        Console.WriteLine(message);

                        await _amazonSQS.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
                        _logger.LogInformation($"Message deleted");

                        LastActivity = DateTime.Now;
                    }
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Stopping queue consumer service...");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing messages for queue {_queueUrl}!");
                }
            }
        }

        private PedidoNotification? TratarMessage(string body)
        {
            var obj = new PedidoNotification();
            try
            {
                obj = Newtonsoft.Json.JsonConvert.DeserializeObject<PedidoNotification>(body);

                if (string.IsNullOrEmpty(obj.idPedido))
                {
                    //logar
                    return null;
                }

            }
            catch
            {
                //logar
                return null;
            }

            return obj;
        }

        private async Task SendTestMessageAsync()
        {
            var messageBody = new PedidoNotification();
            messageBody.IdTransacao = Guid.NewGuid();
            messageBody.idPedido = "65a315fadb1f522d916d9361";
            messageBody.Status = "OK";
            messageBody.DataTransacao = DateTime.Now;

            var jsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(messageBody);

            await _amazonSQS.SendMessageAsync(_queueUrl, jsonObj);
        }

        private async Task CreateMessageInQueueWithStatusASync()
        {
            var name = _configuration.GetSection("Test").GetSection("TestQueueName").Value;
            var responseQueue = await _amazonSQS.CreateQueueAsync(new CreateQueueRequest(name));

            if (responseQueue.HttpStatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Error creating the queue: {name}!");
                throw new AmazonSQSException($"Failed to CreateQueue for queue {name}. Response: {responseQueue.HttpStatusCode}");
            }
        }
    }
}
