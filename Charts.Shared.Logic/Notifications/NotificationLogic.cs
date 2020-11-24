using System;
using System.Linq;
using System.Threading.Tasks;
using Charts.Shared.Data.Context;
using Charts.Shared.Logic.Models.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Charts.Shared.Logic.Notifications
{
    public class NotificationLogic : INotificationLogic
    {

        private readonly IBaseLogic _baseLogic;

        public NotificationLogic(IBaseLogic baseLogic)
        {
            _baseLogic = baseLogic;
        }

        public async Task<object> GetClientNotifications(Guid ClientId)
        {
            var NotificationClient = await _baseLogic.Of<Notification>().GetQueryable(x => !x.IsDeleted)
                .Include(x => x.LoanApplication).ThenInclude(x => x.User)
                .Where(x => x.LoanApplication.UserId == ClientId)
                .Select(x => new
                {
                    x.LoanApplication.User.FullName,
                    x.LoanApplication.Number,
                    x.TaskCode,
                    applicationId = x.ApplicationId,
                    x.SubjectKz,
                    x.SubjectRu,
                    x.BodyKz,
                    x.BodyRu,
                    createdDate = x.CreatedDate.ToString("G")
                })
                .ToListAsync();
            return NotificationClient;
        }

        public async Task<object> AddNotificationEvent(NotificationEventInDto model)
        {
            var notiTemplate =  _baseLogic.Of<NotificationTemplate>().GetQueryable(x => x.TaskCode == model.TaskCode).AsNoTracking().FirstOrDefault();

            if (notiTemplate != null)
            {
                var ClientNotification = new NotificationInDto
                {
                    ApplicationId = model.LoanApplicationId,
                    SubjectKz = notiTemplate.SubjectKz,
                    SubjectRu = notiTemplate.SubjectRu,
                    BodyKz = notiTemplate.BodyKz,
                    BodyRu = notiTemplate.BodyRu,
                    StatusCode = model.StatusCode,
                    TaskCode = model.TaskCode,
                    Error = model.Error
                };

                await AddClientNotification(ClientNotification);
            }
            return true;
        }

        public async Task<object> AddClientNotification(NotificationInDto model)
        {
            var clientNotification = new Notification
            {
                ApplicationId = model.ApplicationId,
                SubjectKz = model.SubjectKz,
                SubjectRu = model.SubjectRu,
                BodyKz = model.BodyKz,
                BodyRu = model.BodyRu,
                StatusCode = model.StatusCode,
                TaskCode = model.TaskCode,
                Error = model.Error
            };

            await _baseLogic.Of<Notification>().Add(clientNotification);
            return true;
        }

    }
}
