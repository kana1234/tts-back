using Charts.Shared.Logic.Workflow.Data;
using Charts.Shared.Logic.Workflow.Steps;
using WorkflowCore.Interface;

namespace Charts.Shared.Logic.Workflow
{
    public class Workflows : IWorkflow<ApplicationData>
    {
        public string Id => "TTSWorkflow";

        public int Version => 1;

        public void Build(IWorkflowBuilder<ApplicationData> builder)
        {
            builder
                .StartWith<CreateAppStep>()
                    .Input(step => step.ApplicationId, data => data.ApplicationId)
                    .Input(step => step.Status, data => data.Status)
                    .Input(step => step.UserId, data => data.UserId)
                    .Input(step => step.Role, data => data.Role)
                    .Output(step => step.WorkflowId, data => data.WorkflowId)
                .Then<PauseStep>()
                   .Input(step => step.WorkflowId, data => data.WorkflowId)
                .Then<EndStep>();
            builder
                .StartWith<EndStep>();

        }
    }
}