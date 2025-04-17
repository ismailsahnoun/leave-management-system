namespace leaveManagement.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Department { get; set; }
    public DateTime JoiningDate { get; set; }
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; }
}   