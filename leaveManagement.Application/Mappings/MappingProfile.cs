using AutoMapper;
using leaveManagement.Application.DTOs;
using leaveManagement.Domain.Entities;

namespace leaveManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeDto>();
        CreateMap<EmployeeDto, Employee>();

        CreateMap<LeaveRequest, LeaveRequestDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : string.Empty));
            
        CreateMap<CreateLeaveRequestDto, LeaveRequest>();
        CreateMap<UpdateLeaveRequestDto, LeaveRequest>();
    }
}