using Application.Services.Status.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IStatusNotificationService
    {
        Task SendNewStatusAsync(Guid userId, IEnumerable<Guid> contactIds);
    }
}
