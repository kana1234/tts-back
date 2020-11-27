using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Charts.Shared.Logic.Steps
{
    public class FinishStep : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Goodbye world");
            return ExecutionResult.Next();
        }
    }
}