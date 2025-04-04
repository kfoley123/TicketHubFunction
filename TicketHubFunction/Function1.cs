using System;
using System.Net.Sockets;
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

            //create options to ignore case when deserializing json

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

                //TODO : insert the purchase into the actual correct table in the database !!! 
                //A new comment for GIT

                var query = "INSERT INTO TicketPurchase (ConcertId, Email, Name, Phone, Quantity, CreditCard, Expiry, SecurityCode, Address, City, Province, PostalCode, Country) VALUES (@ConcertId, @Email, @Name, @Phone, @Quantity, @CreditCard, @Expiry, @SecurityCode, @Address, @City, @Province, @PostalCode, @Country);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ConcertId", purchase.ConcertId);
                    cmd.Parameters.AddWithValue("@Email", purchase.Email);
                    cmd.Parameters.AddWithValue("@Name", purchase.Name);
                    cmd.Parameters.AddWithValue("@Phone", purchase.Phone);
                    cmd.Parameters.AddWithValue("@Quantity", purchase.Quantity);
                    cmd.Parameters.AddWithValue("@CreditCard", purchase.CreditCard); 
                    cmd.Parameters.AddWithValue("@Expiry", purchase.Expiry);
                    cmd.Parameters.AddWithValue("@SecurityCode", purchase.SecurityCode);
                    cmd.Parameters.AddWithValue("@Address", purchase.Address);
                    cmd.Parameters.AddWithValue("@City", purchase.City);
                    cmd.Parameters.AddWithValue("@Province", purchase.Province);
                    cmd.Parameters.AddWithValue("@PostalCode", purchase.PostalCode);
                    cmd.Parameters.AddWithValue("@Country", purchase.Country);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
