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
    public class CreateAppStep : StepBody
    {
        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public RoleEnum Role { get; set; }
        public string WorkflowId { get; set; }
        public ApplicationStatusEnum Status { get; set; }

        //private readonly IApplicationTaskLogic _applicationWorkflow;

        //public CreateAppStep(IApplicationTaskLogic applicationWorkflow)
        //{
        //    _applicationWorkflow = applicationWorkflow;
        //}

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            //_applicationWorkflow.InsertLoanApplicationTaskUser(ApplicationId, UserId, Role, Status).Wait();
           // WorkflowId = context.Workflow.Id;
            return ExecutionResult.Next();
        }
    }
}