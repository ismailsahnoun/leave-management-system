namespace leaveManagement.Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Department { get; set; }
    public DateTime JoiningDate { get; set; }
}