using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Charts.Shared.Api.Controllers;
using Charts.Shared.Data.Context.Dictionary;
using Charts.Shared.Logic.Dictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Charts.Api.Controllers
{
    public class DictionaryController : BaseController
    {
        private readonly IDictionaryLogic _logic;

        public DictionaryController(IDictionaryLogic logic)
        {
            _logic = logic;
        }

        
        [HttpGet("GetDictionaryItems/{dictionaryName}")]
        public async Task<IActionResult> GetDictionaryItems([Required] string dictionaryName)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                return Ok(GetDictionaryData(dictionaryName));
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        

        private object GetDictionaryData(string dictionaryName)
        {
            var type = Assembly.GetAssembly(typeof(BaseDictionary)).GetTypes()
                .FirstOrDefault(myType => myType.Name == dictionaryName && myType.IsSubclassOf(typeof(BaseDictionary)));

            return _logic.GetType().GetMethod("DictionaryRepoGetDtoList")?.MakeGenericMethod(type).Invoke(_logic, new object[] { }); ;
        }
    }
}
