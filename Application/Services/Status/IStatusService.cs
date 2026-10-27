using Application.Response;
using Application.Services.Status.Dtos;
using Application.Services.Status.Viewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Status
{
    public interface IStatusService
    {
        Task<ApiResponse<string>> CreateStatuesAsync(Guid userId, CreateStatuesDto model);
        Task<ApiResponse<List<UserStatusDto>>> GetContactsStatusesAsync(Guid userId);
        Task<ApiResponse<List<MyStatusDto>>> GetMyStatusesAsync(Guid userId);
        Task<ApiResponse<bool>> ViewStatusAsync(Guid userId, Guid statusId);
        Task<ApiResponse<bool>> DeleteStatusAsync(Guid userId, Guid statusId);
        Task<ApiResponse<List<StatusViewDto>>> GetViewersAsync(Guid userId, Guid statusId);
    }
}
