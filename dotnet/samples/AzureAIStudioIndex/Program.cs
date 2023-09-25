using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.AzureCognitiveSearch;
using Microsoft.SemanticKernel.Memory;
using static System.Environment;

string AZURE_OPENAI_ENDPOINT = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT", EnvironmentVariableTarget.User);
string AZURE_OPENAI_API_KEY = GetEnvironmentVariable("AZURE_OPENAI_KEY", EnvironmentVariableTarget.User);

string AZURE_SEARCH_ADMIN_KEY = GetEnvironmentVariable("AZURE_SEARCH_ADMIN_KEY", EnvironmentVariableTarget.User);
string AZURE_SEARCH_ENDPOINT = GetEnvironmentVariable("AZURE_SEARCH_ENDPOINT", EnvironmentVariableTarget.User);

var kernel = new KernelBuilder()
    .WithAzureTextEmbeddingGenerationService(
        "text-embedding-ada-002",
        AZURE_OPENAI_ENDPOINT,
        AZURE_OPENAI_API_KEY)
    .WithMemoryStorage(new AzureCognitiveSearchMemoryStore(
        AZURE_SEARCH_ENDPOINT,
        AZURE_SEARCH_ADMIN_KEY))
    .Build();


Console.WriteLine("Searching index ..");
var idxs = await kernel.Memory.GetCollectionsAsync();
foreach (string idx in idxs)
{
    Console.WriteLine(idx.ToString());
}


IAsyncEnumerable<MemoryQueryResult> memories = kernel.Memory.SearchAsync("resumes-0910-3", "Find resumes of people with Frontend software development experience", limit: 10);
await foreach (MemoryQueryResult memory in memories)
{
    //Console.WriteLine(memory.Embedding.HasValue ? memory.Embedding.Value : "No Embedding" );
    Console.WriteLine("Id:     : " + memory.Metadata.Id);
    Console.WriteLine("Title    : " + memory.Metadata.Description);
    Console.WriteLine("Url    : " + memory.Metadata.ExternalSourceName);
    Console.WriteLine("Text     : " + memory.Metadata.Text.Substring(0, 100));
    Console.WriteLine("Relevance: " + memory.Relevance);
}