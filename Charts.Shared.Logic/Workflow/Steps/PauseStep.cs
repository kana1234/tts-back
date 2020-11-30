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
    public class PauseStep : StepBody
    {
        //private readonly IOptions<AppSettings> _conf;
        //private readonly IBaseLogic _baseLogic;
        //private readonly IWorkflowController _workflowService;
        //private readonly IWorkflowRegistry _registry;
        //private readonly IPersistenceProvider _workflowStore;

        //public PauseStep(IOptions<AppSettings> conf, IBaseLogic baseLogic, IWorkflowController workflowService, IWorkflowRegistry registry, IPersistenceProvider workflowStore)
        //{
        //    _conf = conf;
        //    _baseLogic = baseLogic;
        //    _workflowService = workflowService;
        //    _registry = registry;
        //    _workflowStore = workflowStore;
        //}
        public string WorkflowId { get; set; }


        public override ExecutionResult Run(IStepExecutionContext context)
        {
            //_workflowService.SuspendWorkflow(WorkflowId);
            Console.WriteLine(WorkflowId);
            return ExecutionResult.Next();
        }
    }
}