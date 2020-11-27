using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charts.Shared.Data.Extensions;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Dictionary;
using Charts.Shared.Logic.Helper;
using Charts.Shared.Logic.Models.Applciation;
using Charts.Shared.Logic.User;
using Microsoft.EntityFrameworkCore;

namespace Charts.Shared.Logic.Workflow
{
    public class WorkflowLogic : IWorkflowLogic
    {

        private readonly IDictionaryLogic _dictionarylogic;
        private readonly IUserLogic _userLogic;

        private readonly IBaseLogic _baseLogic;


        public WorkflowLogic(IDictionaryLogic dictionarylogic, IUserLogic userLogic, IBaseLogic baseLogic)
        {
            _dictionarylogic = dictionarylogic;
            _userLogic = userLogic;
            _baseLogic = baseLogic;
        }

    }
}