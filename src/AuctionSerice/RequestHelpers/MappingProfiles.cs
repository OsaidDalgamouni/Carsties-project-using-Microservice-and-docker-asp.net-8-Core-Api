using AuctionSerice.DTOs;
using AuctionSerice.Entities;
using AutoMapper;
using Contract;

namespace AuctionSerice.RequetHelpers;


public class MappingProfiles: Profile{

public  MappingProfiles(){
    CreateMap<Auction,AuctionDto>().IncludeMembers(x=>x.Item);
    CreateMap<Item,AuctionDto>();
    
    CreateMap<CreateAuctionDto,Auction>().ForMember(d => d.Item ,o=>o.MapFrom(s=>s));
    CreateMap<CreateAuctionDto,Item>();
    CreateMap<AuctionDto,AuctionCreated>();
    CreateMap<Auction,AuctionUpdated>().IncludeMembers(x=>x.Item);
    CreateMap<Item,AuctionUpdated>();


}
    
}