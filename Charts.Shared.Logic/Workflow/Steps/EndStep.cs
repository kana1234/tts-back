using System;
using Charts.Shared.Data;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Application;
using Charts.Shared.Logic.ApplicationTask;
using Microsoft.Extensions.Options;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Charts.Shared.Logic.Workflow.Steps
{
    public class EndStep : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("End world");
            return ExecutionResult.Next();
        }
    }
}