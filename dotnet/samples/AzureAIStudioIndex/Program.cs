using System;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.AzureCognitiveSearch;
using Microsoft.SemanticKernel.Memory;
using static System.Environment;

namespace AzureAIStudioIndexExample;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        string indexName = "index-created-from-ai-studio";
        string systemPrompt = "You are a helpful research assistant.";

        if (args.Length > 0)
        {
            indexName = args[0];
        }

        if (args.Length > 1)
        {
            systemPrompt = args[1];
        }



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


        Console.WriteLine($"Searching index : {indexName}..");
        var idxs = await kernel.Memory.GetCollectionsAsync();
        foreach (string idx in idxs)
        {
            Console.WriteLine($"Found index: {idx}");
        }

        while (true)
        {
            Console.Write("\n\nAsk me: ");
            string? ask = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ask))
            {
                continue;
            }

            IAsyncEnumerable<MemoryQueryResult> memories = kernel.Memory.SearchAsync(indexName, ask, limit: 5);
            await foreach (MemoryQueryResult memory in memories)
            {
                Console.WriteLine($">> Id: {memory.Metadata.Id}");
                Console.WriteLine($"\nTitle: {memory.Metadata.Description}");
                Console.WriteLine($"Url: {memory.Metadata.ExternalSourceName}");
                Console.WriteLine($"Relevance: {memory.Relevance}");
                Console.WriteLine($"Has Embedding: {memory.Embedding.HasValue}");
                Console.WriteLine($"Text: {memory.Metadata.Text.Substring(0, 100)}");
            }
        }

    }
}
