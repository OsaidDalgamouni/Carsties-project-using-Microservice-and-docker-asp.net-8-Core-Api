using System.Net;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(x =>
{
  x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
  x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("Search",false));
  x.UsingRabbitMq((context, cfg) =>
  { //if SearchService DB is Down 
    cfg.ReceiveEndpoint("search-auction-created",e=>{
      //try 5 time between each try 5 seconde 
      e.UseMessageRetry(r=>r.Interval(5,5));
      e.ConfigureConsumer<AuctionCreatedConsumer>(context);
    });
    cfg.ConfigureEndpoints(context);
  });
}
);
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
HttpPolicyExtensions.HandleTransientHttpError()
.OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
.WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));