using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

public static class ConnectionFunc
{
    [FunctionName("ConnectionFunc")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "documents")] HttpRequest req,
        ILogger log)
    {
        var connectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
        var cosmosClient = new CosmosClient(connectionString);
        var container = cosmosClient.GetContainer("todo", "tasks");

        var query = new QueryDefinition("SELECT * FROM c");
        var documents = new List<dynamic>();

        var iterator = container.GetItemQueryIterator<dynamic>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            documents.AddRange(response.ToList());
        }

        return new OkObjectResult(documents);
    }
}
