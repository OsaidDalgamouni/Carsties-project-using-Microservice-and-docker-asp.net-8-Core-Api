using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;

namespace SearchService;

public class DbInitializer
{
    public static async Task InItDb(WebApplication webApplication)
    {
        await DB.InitAsync("SearchDB", MongoClientSettings.FromConnectionString
        (webApplication.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
        .Key(x => x.Make, KeyType.Text)
        .Key(x => x.Model, KeyType.Text)
         .Key(x => x.Color, KeyType.Text).CreateAsync();

         var count = await DB.CountAsync<Item>();

        //  
        var scope= webApplication.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

        var items= await httpClient.GetItemsForSearchDB();

        Console.WriteLine(items +"Returned From Auction Service");

        if(items.Count >0) await DB.SaveAsync(items);


    }
}
