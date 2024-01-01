using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RewardService.Models;
using RewardService.Models.DTOs;
using RewardService.Service.IService;

namespace RewardService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly IReward _rewardService;
        private readonly IMapper _mapper;
        private readonly ResponseDTO _response;  

        public RewardsController(IReward rewardService, IMapper mapper)
        {
            _rewardService = rewardService;
            _mapper = mapper;   
            _response = new ResponseDTO();  
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> AddReward(RewardDTO reward)
        {
            try
            {
                var newReward = _mapper.Map<Reward>(reward);
                var response = await _rewardService.AddReward(newReward);
                _response.Result = response;

                return Ok(_response);
            }
            catch (Exception ex) 
            { 
                _response.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, _response);
            }
        }
    }
}
