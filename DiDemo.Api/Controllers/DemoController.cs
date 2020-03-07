using DiDemo.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly MyCoolService _myCoolService;

        public DemoController(MyCoolService myCoolService)
        {   
            _myCoolService = myCoolService;
            
        }

        [HttpGet]
        public string[] GetDescriptions(){
            return _myCoolService.GetDescriptions();
        }
    }
}