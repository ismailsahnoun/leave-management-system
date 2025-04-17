using leaveManagement.Application.DTOs;

namespace leaveManagement.Application.Interfaces;

public interface ILeaveRequestService
{
    Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync();
    Task<LeaveRequestDto> GetLeaveRequestByIdAsync(int id);
    Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto leaveRequestDto);
    Task<LeaveRequestDto> UpdateLeaveRequestAsync(UpdateLeaveRequestDto leaveRequestDto);
    Task DeleteLeaveRequestAsync(int id);
    Task<PagedResult<LeaveRequestDto>> FilterLeaveRequestsAsync(LeaveRequestFilterDto filterDto);
    Task<IEnumerable<LeaveReportDto>> GetLeaveReportAsync(int year);
    Task<bool> ApproveLeaveRequestAsync(int id);
}