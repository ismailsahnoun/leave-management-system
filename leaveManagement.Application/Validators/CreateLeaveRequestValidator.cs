using FluentValidation;
using leaveManagement.Application.DTOs;
using leaveManagement.Domain.Enums;
using leaveManagement.Domain.Repositories;

namespace leaveManagement.Application.Validators;

public class CreateLeaveRequestValidator : AbstractValidator<CreateLeaveRequestDto>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        
        public CreateLeaveRequestValidator(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("Employee ID must be specified.");
                
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .LessThanOrEqualTo(x => x.EndDate).WithMessage("Start date must be before or equal to end date.");
                
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.");
                
            RuleFor(x => x.LeaveType)
                .IsInEnum().WithMessage("Invalid leave type.");
                
            // Sick leave requires a reason
            RuleFor(x => x.Reason)
                .NotEmpty().When(x => x.LeaveType == LeaveType.Sick)
                .WithMessage("Reason is required for sick leave.");
                
            // Check for overlapping leave dates
            RuleFor(x => x)
                .MustAsync(HaveNoOverlappingLeaves)
                .WithMessage("Employee already has approved or pending leave during this period.");
                
            // Check annual leave days limit (20 days per year)
            RuleFor(x => x)
                .MustAsync(NotExceedAnnualLeaveLimit)
                .When(x => x.LeaveType == LeaveType.Annual)
                .WithMessage("Employee has exceeded the maximum 20 annual leave days for the year.");
        }
        
        private async Task<bool> HaveNoOverlappingLeaves(CreateLeaveRequestDto dto, CancellationToken cancellationToken)
        {
            return !await _leaveRequestRepository.HasOverlappingLeavesAsync(
                dto.EmployeeId, dto.StartDate, dto.EndDate);
        }
        
        private async Task<bool> NotExceedAnnualLeaveLimit(CreateLeaveRequestDto dto, CancellationToken cancellationToken)
        {
            int requestedDays = CountBusinessDays(dto.StartDate, dto.EndDate);
            
            int currentUsedDays = await _leaveRequestRepository.GetEmployeeAnnualLeaveDaysInYearAsync(
                dto.EmployeeId, dto.StartDate.Year);
            
            // Check if the total would exceed 20 days
            return (currentUsedDays + requestedDays) <= 20;
        }
        
        private int CountBusinessDays(DateTime startDate, DateTime endDate)
        {
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
    