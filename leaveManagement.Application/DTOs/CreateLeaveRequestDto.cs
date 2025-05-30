using leaveManagement.Domain.Enums;

namespace leaveManagement.Application.DTOs;

public class CreateLeaveRequestDto
{
    public int EmployeeId { get; set; }
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; }
}