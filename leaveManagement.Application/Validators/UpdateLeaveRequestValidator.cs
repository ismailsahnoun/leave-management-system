using FluentValidation;
using leaveManagement.Application.DTOs;
using leaveManagement.Domain.Enums;
using leaveManagement.Domain.Repositories;

namespace leaveManagement.Application.Validators;

public class UpdateLeaveRequestValidator : AbstractValidator<UpdateLeaveRequestDto>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
        
    public UpdateLeaveRequestValidator(ILeaveRequestRepository leaveRequestRepository)
    {
        _leaveRequestRepository = leaveRequestRepository;
            
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Leave request ID must be specified.");
                
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .LessThanOrEqualTo(x => x.EndDate).WithMessage("Start date must be before or equal to end date.");
                
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.");
                
        RuleFor(x => x.LeaveType)
            .IsInEnum().WithMessage("Invalid leave type.");
                
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status.");
                
        // Sick leave requires a reason
        RuleFor(x => x.Reason)
            .NotEmpty().When(x => x.LeaveType == LeaveType.Sick)
            .WithMessage("Reason is required for sick leave.");
                
        // Check for overlapping leave dates
        RuleFor(x => x)
            .MustAsync(HaveNoOverlappingLeaves)
            .WithMessage("Employee already has approved or pending leave during this period.");
    }
        
    private async Task<bool> HaveNoOverlappingLeaves(UpdateLeaveRequestDto dto, CancellationToken cancellationToken)
    {
        return !await _leaveRequestRepository.HasOverlappingLeavesAsync(
            dto.Id, dto.StartDate, dto.EndDate, dto.Id);
    }
}