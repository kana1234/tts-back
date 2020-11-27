using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charts.Shared.Data.Extensions;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Dictionary;
using Charts.Shared.Logic.Helper;
using Charts.Shared.Logic.Models.Applciation;
using Charts.Shared.Logic.Steps;
using Charts.Shared.Logic.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WorkflowCore.Interface;

namespace Charts.Shared.Logic.Workflow
{
    public class ApplicationWorkflow : IWorkflow
    {
        public string Id => "TTSWorkflow";

        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder
                .StartWith<FirstStep>()
                .Then<FinishStep>();
        }

    }
}