using System;
using System.Threading.Tasks;
using Nest;

class Program
{
    private static ElasticClient? _client;
    private static readonly string IndexName = "products";

    static async Task Main(string[] args)
    {
        var settings = new ConnectionSettings(new Uri("http://localhost:9201")) // or 9200
            .DefaultIndex(IndexName);
        _client = new ElasticClient(settings);

        if (_client == null)
        {
            Console.WriteLine("Elasticsearch client is not initialized.");
            return;
        }

        await CreateIndexIfNotExists();
        await AddProduct(new Product { Id = 1, Name = "Laptop", Description = "Asus Gaming laptop" });
        await AddProduct(new Product { Id = 2, Name = "Phone", Description = "Samsung Smartphone" });
        await AddProduct(new Product { Id = 3, Name = "Tablet", Description = "Samsung Tablet" });

        await SearchProducts("gaming");
        await UpdateProduct(1, "Asus updated Gaming Laptop");
        await GetProduct(1);
        await DeleteProduct(2);
        await SearchProducts("phone");
        await GetProduct(3); 
    }

    static async Task CreateIndexIfNotExists()
    {
        if (_client == null) return;

        var existsResponse = await _client.Indices.ExistsAsync(IndexName);
        if (!existsResponse.Exists)
        {
            var createIndexResponse = await _client.Indices.CreateAsync(IndexName, c => c
                .Map<Product>(m => m.AutoMap())
            );
            Console.WriteLine($"Index created: {createIndexResponse.IsValid}");
        }
    }

    static async Task AddProduct(Product product)
    {
        if (_client == null) return;

        var response = await _client.IndexDocumentAsync(product);
        Console.WriteLine($"Product added: {response.IsValid}");
    }

    static async Task GetProduct(int id)
    {
        if (_client == null) return;

        var response = await _client.GetAsync<Product>(id);
        if (response.Found)
        {
            Console.WriteLine($"Product found: {response.Source.Name}");
        }
        else
        {
            Console.WriteLine("Product not found");
        }
    }

    static async Task SearchProducts(string query)
    {
        if (_client == null) return;

        var response = await _client.SearchAsync<Product>(s => s
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Description)
                    .Query(query)
                )
            )
        );

        Console.WriteLine($"Search results for '{query}':");
        foreach (var hit in response.Hits)
        {
            Console.WriteLine($"- {hit.Source.Name}: {hit.Source.Description}");
        }
    }

    static async Task UpdateProduct(int id, string newName)
    {
        if (_client == null) return;

        var response = await _client.UpdateAsync<Product>(id, u => u
            .Doc(new Product { Name = newName })
        );

        Console.WriteLine($"Product updated: {response.IsValid}");
    }

    static async Task DeleteProduct(int id)
    {
        if (_client == null) return;

        var response = await _client.DeleteAsync<Product>(id);
        Console.WriteLine($"Product deleted: {response.IsValid}");
    }
}
