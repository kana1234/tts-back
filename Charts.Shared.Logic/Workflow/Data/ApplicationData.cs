using System;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Application;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Charts.Shared.Logic.Workflow.Data
{
    public class ApplicationData 
    {
        public Guid? ApplicationId { get; set; }
        public Guid? CurrentUserId { get; set; }
        public string WorkflowId { get; set; }
        public Guid? UserId { get; set; }
        public RoleEnum? Role { get; set; }
        public ApplicationStatusEnum? Status { get; set; }
        public string Comment { get; set; }
        public ApplicationTaskStatusEnum? TaksStatus { get; set; }
    }
}