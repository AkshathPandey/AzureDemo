using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System;
using System.Threading.Tasks;

public static class GetDocById
{
    [FunctionName("GetDocById")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "documentsbyid/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            // Get the Cosmos DB endpoint and key from environment variables
            string connectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");

            // Create a new Cosmos DB client
            CosmosClient cosmosClient = new CosmosClient(connectionString);

            // Get a reference to the database
            Database cosmosDatabase = await cosmosClient.CreateDatabaseIfNotExistsAsync("todo");

            // Get a reference to the container
            Container cosmosContainer = cosmosDatabase.GetContainer("tasks");

            // Get the document by ID
            ItemResponse<dynamic> response = await cosmosContainer.ReadItemAsync<dynamic>(id, new PartitionKey(id));

            return new OkObjectResult(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new NotFoundResult();
        }
        catch (Exception ex)
        {
            return new StatusCodeResult(500);
        }
    }
}
