using Charts.Shared.Api.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace TTS.File.Api.Controllers
{
    [AllowAnonymous]
    public class PrintController : BaseController
    {
        //private readonly IPrintLogic _printLogic;

        //public PrintController(IPrintLogic printLogic)
        //{
        //    _printLogic = printLogic;
        //}

        //[HttpPost("GenerateFile")]
        //public IActionResult GenerateFile([FromBody] PrintInDto model)
        //{
        //    var result = _printLogic.GenerateFromXml(model);
        //    if (result == null)
        //    {
        //        return NoContent();
        //    }

        //    return result;
        //}

    }
}