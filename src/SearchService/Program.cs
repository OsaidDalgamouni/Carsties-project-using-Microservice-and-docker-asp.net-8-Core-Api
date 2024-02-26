using System.Net;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
var app = builder.Build();




app.UseAuthorization();

app.MapControllers();
app.Lifetime.ApplicationStarted.Register(async () =>
{
  try
  {

    await DbInitializer.InItDb(app);
  }
  catch (Exception e)
  {

    Console.WriteLine(e);

  }
});

app.Run();
//IF Auction service Down we handle the error and keep try every 3 seconde
static IAsyncPolicy<HttpResponseMessage> GetPolicy() =>
HttpPolicyExtensions.HandleTransientHttpError().OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound).WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));