using leaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace leaveManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<LeaveRequest>()
            .HasOne(l => l.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(l => l.EmployeeId);

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed employees
        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, FullName = "Ismail Sahnoun", Department = "IT", JoiningDate = new DateTime(2022, 1, 15) },
            new Employee { Id = 2, FullName = "Foulen Ben Foulen", Department = "HR", JoiningDate = new DateTime(2021, 5, 10) },
            new Employee { Id = 3, FullName = "Test User", Department = "Finance", JoiningDate = new DateTime(2023, 2, 20) }
        );

        // Seed leave requests
        modelBuilder.Entity<LeaveRequest>().HasData(
            new LeaveRequest
            {
                Id = 1,
                EmployeeId = 1,
                LeaveType = Domain.Enums.LeaveType.Annual,
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                Status = Domain.Enums.LeaveStatus.Pending,
                Reason = "Family vacation",
                CreatedAt = DateTime.Now.AddDays(-2)
            },
            new LeaveRequest
            {
                Id = 2,
                EmployeeId = 2,
                LeaveType = Domain.Enums.LeaveType.Sick,
                StartDate = DateTime.Now.AddDays(-3),
                EndDate = DateTime.Now.AddDays(-1),
                Status = Domain.Enums.LeaveStatus.Approved,
                Reason = "Cold and fever",
                CreatedAt = DateTime.Now.AddDays(-5)
            }
        );
    }
}
