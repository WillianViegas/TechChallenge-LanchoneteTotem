using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Infra.Configurations.SQS
{
    public class SQSConfiguration : ISQSConfiguration
    {
        public async Task<AmazonSQSClient> ConfigurarSQS()
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

        public async Task EnviarParaSQS(string jsonMessage, AmazonSQSClient sqsClient)
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
            catch (Exception ex)
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
    }
}
