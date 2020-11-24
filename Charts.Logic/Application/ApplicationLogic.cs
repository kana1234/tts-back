using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Camunda.Api.Client;
using Camunda.Api.Client.UserTask;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic;
using Charts.Shared.Logic.Application;
using Charts.Shared.Logic.Dictionary;
using Charts.Shared.Logic.User;

namespace Charts.Logic.Application
{
    public class ArmLoanApplicationLogic : ApplicationLogic, Charts.Logic.Application.IApplicationLogic
    {
        private readonly IUserLogic _userLogic;
        public ArmLoanApplicationLogic(IDictionaryLogic dictionarylogic, IUserLogic userLogic, IBaseLogic baseLogic, Shared.Logic.Application.IApplicationLogic loanApplicationLogicImplementation) : base(dictionarylogic, userLogic, baseLogic, loanApplicationLogicImplementation)
        {
            _userLogic = userLogic;
        }
        public Task ManagerSend(Guid userId, Guid applicationTaskId, string decision, string comment)
        {
            throw new NotImplementedException();
        }
    }
}
