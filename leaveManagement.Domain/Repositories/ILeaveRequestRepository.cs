using leaveManagement.Domain.Entities;

namespace leaveManagement.Domain.Repositories;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest> GetByIdAsync(int id);
    Task<IEnumerable<LeaveRequest>> GetAllAsync();
    Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest);
    Task UpdateAsync(LeaveRequest leaveRequest);
    Task DeleteAsync(int id);
    IQueryable<LeaveRequest> GetQueryable();
    
    // Rules validation
    Task<bool> HasOverlappingLeavesAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeLeaveRequestId = null);
    Task<int> GetEmployeeAnnualLeaveDaysInYearAsync(int employeeId, int year);
}