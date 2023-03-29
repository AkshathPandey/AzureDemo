using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;
using System;

public static class PostDocuments
{
    [FunctionName("PostDocuments")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "postdocuments")] HttpRequest req,
        ILogger log)
    {
        var connectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
        var cosmosClient = new CosmosClient(connectionString);
        var container = cosmosClient.GetContainer("todo", "tasks");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);

        var document = new
        {
            id = Guid.NewGuid().ToString(),
            data.name,
            data.PhNo,
            timestamp = DateTime.UtcNow
        };

        await container.CreateItemAsync(document, new PartitionKey(document.id));

        return new OkObjectResult(document);
    }
}
