using AuctionSerice;
using AuctionSerice.Data;
using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AuctionDbContext>(options =>
{

    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{     x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
  x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction",false));
     x.AddEntityFrameworkOutbox<AuctionDbContext>(o=>{
     //if the Bus works the message will send but if it down will try every 10 seconde to send the message 
    o.QueryDelay=TimeSpan.FromSeconds(10);
    o.UsePostgres();
    o.UseBusOutbox();
});
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
}
);
var app = builder.Build();



app.UseAuthorization();

app.MapControllers();

try
{


    DbInitializer.InitDb(app);
}
catch (Exception e)
{

    Console.WriteLine(e);
}
app.Run();
