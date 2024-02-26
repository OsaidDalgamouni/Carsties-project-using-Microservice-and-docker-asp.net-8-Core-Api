using AuctionSerice.Data;
using AuctionSerice.DTOs;
using AuctionSerice.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionSerice.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _db;
    private readonly IMapper _mapper;
    public AuctionsController(AuctionDbContext auctionDbContext, IMapper mapper)
    {

        _db = auctionDbContext;
        _mapper = mapper;

    }
    [HttpGet]

    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string Date)
    {   //AsQueryable To Keep Query Type  Iqeryable because  OrderBy change the type of query    
        var query =_db.Auctions.OrderBy(x=>x.Item.Make).AsQueryable();

        if(! string.IsNullOrEmpty(Date)){

            query=query.Where(x=>x.UpdatedAt.CompareTo(DateTime.Parse(Date).ToUniversalTime())>0);
        }

        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();

    }
    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var Auction = await _db.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
        if (Auction == null) return NotFound();
        return _mapper.Map<AuctionDto>(Auction);
    }

    [HttpPost]

    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {

        var Auction = _mapper.Map<Auction>(auctionDto);


        Auction.Seller = "test";
        _db.Auctions.Add(Auction);

        var result = await _db.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not save auction");

        return CreatedAtAction(nameof(GetAuctionById), new { Auction.Id }, _mapper.Map<AuctionDto>(Auction));

    }

    [HttpPut("{id}")]

    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {

        var FindAuction = _db.Auctions.Include(x => x.Item).FirstOrDefault(x => x.Id == id);
        if (FindAuction == null) return NotFound();
        //only on nullabel property
        FindAuction.Item.Make = updateAuctionDto.Make ?? FindAuction.Item.Make;
        FindAuction.Item.Model = updateAuctionDto.Model ?? FindAuction.Item.Model;
        FindAuction.Item.Color = updateAuctionDto.Color ?? FindAuction.Item.Color;
        FindAuction.Item.Mileage = updateAuctionDto.Mileage ?? FindAuction.Item.Mileage;
        FindAuction.Item.Year = updateAuctionDto.Year ?? FindAuction.Item.Year;

        var result = _db.SaveChanges() > 0;
        if (result) return Ok();

        return BadRequest("Error Saving Changes");

    }
    [HttpDelete("{id}")]

    public async Task<ActionResult>DeleteAuction(Guid id){
       
       var Auction =await _db.Auctions.FindAsync(id);

       if(Auction ==null)return NotFound();


       _db.Auctions.Remove(Auction);
       var Result =_db.SaveChanges() >0;

       if(!Result)return BadRequest("Error While SaveChanges");

       return Ok();
       

    }



}