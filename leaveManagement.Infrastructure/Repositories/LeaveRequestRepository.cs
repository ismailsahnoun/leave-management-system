using leaveManagement.Domain.Entities;
using leaveManagement.Domain.Enums;
using leaveManagement.Domain.Repositories;
using leaveManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace leaveManagement.Infrastructure.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly ApplicationDbContext _context;

    public LeaveRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LeaveRequest> GetByIdAsync(int id)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == id);
    }

    public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .ToListAsync();
    }

    public async Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest)
    {
        await _context.LeaveRequests.AddAsync(leaveRequest);
        await _context.SaveChangesAsync();
        return leaveRequest;
    }

    public async Task UpdateAsync(LeaveRequest leaveRequest)
    {
        _context.LeaveRequests.Update(leaveRequest);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(id);
        if (leaveRequest != null)
        {
            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();
        }
    }
    
    public IQueryable<LeaveRequest> GetQueryable()
    {
        return _context.LeaveRequests.Include(lr => lr.Employee).AsQueryable();
    }
    
    public async Task<bool> HasOverlappingLeavesAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeLeaveRequestId = null)
    {
        var query = _context.LeaveRequests
            .Where(lr => lr.EmployeeId == employeeId &&
                         lr.Status != LeaveStatus.Rejected &&
                         ((lr.StartDate <= endDate && lr.EndDate >= startDate)));

        // Exclude the current leave request if it's being updated
        if (excludeLeaveRequestId.HasValue)
        {
            query = query.Where(lr => lr.Id != excludeLeaveRequestId.Value);
        }

        return await query.AnyAsync();
    }
    
    public async Task<int> GetEmployeeAnnualLeaveDaysInYearAsync(int employeeId, int year)
    {
        var startOfYear = new DateTime(year, 1, 1);
        var endOfYear = new DateTime(year, 12, 31);
            
        var approvedLeaves = await _context.LeaveRequests
            .Where(lr => lr.EmployeeId == employeeId &&
                         lr.LeaveType == LeaveType.Annual &&
                         lr.Status == LeaveStatus.Approved &&
                         ((lr.StartDate >= startOfYear && lr.StartDate <= endOfYear) ||
                          (lr.EndDate >= startOfYear && lr.EndDate <= endOfYear)))
            .ToListAsync();
                
        // Calculate total days for each leave request within the specified year
        int totalDays = 0;
        foreach (var leave in approvedLeaves)
        {
            var effectiveStartDate = leave.StartDate < startOfYear ? startOfYear : leave.StartDate;
            var effectiveEndDate = leave.EndDate > endOfYear ? endOfYear : leave.EndDate;
                
            // Calculate business days between the dates (excluding weekends)
            totalDays += CountBusinessDays(effectiveStartDate, effectiveEndDate);
        }
            
        return totalDays;
    }
    private int CountBusinessDays(DateTime startDate, DateTime endDate)
    {
        // Count days excluding weekends (Saturday and Sunday)
        int days = 0;
        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                days++;
            }
        }
        return days;
    }
}