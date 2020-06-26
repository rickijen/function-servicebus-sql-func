using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AZBlue.Function
{
    public static class ServiceBusQueue2AzureSQL
    {
        [FunctionName("ServiceBusQueue2AzureSQL")]                    
        public static void Run(
            [ServiceBusTrigger("blue-queue", Connection = "ServiceBusConnection")] 
            string myQueueItem,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
            log.LogInformation($"DeliveryCount={deliveryCount}");
            log.LogInformation($"MessageId={messageId}");

            var ShipMethod = myQueueItem;

            var str = Environment.GetEnvironmentVariable("SQLConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = "UPDATE SalesLT.SalesOrderHeader " +
                        //"SET [ShipMethod] = 'UPS' WHERE ShipDate < GetDate();";
                        "SET [ShipMethod] = " + "'" + ShipMethod + "'" + " WHERE ShipDate < GetDate();";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    // Execute the command and log the # rows affected.
                    var rows = cmd.ExecuteNonQueryAsync();
                    log.LogInformation($"{rows} rows were updated");
                }
            }
        }    
    }
}
