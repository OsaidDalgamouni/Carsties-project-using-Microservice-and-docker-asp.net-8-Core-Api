using System.Data;
using AuctionSerice.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionSerice.Data;


public class AuctionDbContext :DbContext{

public AuctionDbContext(DbContextOptions options): base(options) {


}
public DbSet<Auction>Auctions {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }

}