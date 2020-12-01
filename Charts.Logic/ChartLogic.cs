using System;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Charts.Logic
{
    public class ChartLogic : IChartLogic
    {
        private readonly IBaseLogic _baseLogic;

        public ChartLogic(IBaseLogic baseLogic)
        {
            _baseLogic = baseLogic;
        }

        public Task<object> GetExpiredContractors()
        {
            throw new NotImplementedException();
        }
    }
}
