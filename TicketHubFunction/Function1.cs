using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace TicketHubFunction
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run([QueueTrigger("purchases", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

            string json = message.MessageText;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            //deserialize the json into a TicketPurchase object
            TicketPurchase? purchase = JsonSerializer.Deserialize<TicketPurchase>(json, options);

            if(purchase == null)
            {
                _logger.LogError("Failed to deserialize the purchase");
                return;
            }

            _logger.LogInformation($"hello {purchase.Name}");

            //add contact to the database
            // get connection string from app settings
            string? connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("SQL connection string is not set in the environment variables.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync(); // Note the ASYNC

                var query = "INSERT INTO dbo.SomeTable (Column1, Column2) VALUES (@value1, @value2)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@value1", "hello");
                    cmd.Parameters.AddWithValue("@value2", "value");

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
