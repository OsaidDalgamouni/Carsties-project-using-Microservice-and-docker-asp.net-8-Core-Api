using Contract;
using MassTransit;

namespace AuctionSerice;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine("-->Consuming Faulty Creation");
        var exception =context.Message.Exceptions.First();

        if(exception.ExceptionType=="System.ArgumentException"){

            context.Message.Message.Model="FooBar";
            await context.Publish(context.Message.Message);
        }

        else{

            Console.WriteLine("Not an ArgumentException -update error dashboard somewhere");
        }
    }
}
