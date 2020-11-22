using System;
using System.Threading.Tasks;
using Charts.Shared.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Charts.Api.Controllers
{
    public class ChartController : BaseController
    {
        [HttpGet("GetChartDatas")]
        public async Task<IActionResult> GetChartDatas()
        {
            try
            {
               
                return Ok();
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

    }
}