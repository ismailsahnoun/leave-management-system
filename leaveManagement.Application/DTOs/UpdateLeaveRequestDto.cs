using leaveManagement.Domain.Enums;

namespace leaveManagement.Application.DTOs;

public class UpdateLeaveRequestDto
{
    public int Id { get; set; }
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveStatus Status { get; set; }
    public string Reason { get; set; }
}