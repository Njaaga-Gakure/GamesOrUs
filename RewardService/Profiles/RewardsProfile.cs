using AutoMapper;
using RewardService.Models;
using RewardService.Models.DTOs;

namespace RewardService.Profiles
{
    public class RewardsProfile : Profile
    {
        public RewardsProfile()
        {
            CreateMap<RewardDTO, Reward>().ReverseMap();    
        }
    }
}
