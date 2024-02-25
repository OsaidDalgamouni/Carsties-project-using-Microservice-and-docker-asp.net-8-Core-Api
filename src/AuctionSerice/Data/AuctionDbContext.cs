using System.Data;
using AuctionSerice.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionSerice.Data;


public class AuctionDbContext :DbContext{

public AuctionDbContext(DbContextOptions options): base(options) {


}
public DbSet<Auction>Auctions {get;set;}

}