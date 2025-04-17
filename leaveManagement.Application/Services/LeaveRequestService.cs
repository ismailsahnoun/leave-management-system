using AutoMapper;
using FluentValidation;
using leaveManagement.Application.DTOs;
using leaveManagement.Application.Interfaces;
using leaveManagement.Application.Validators;
using leaveManagement.Domain.Entities;
using leaveManagement.Domain.Enums;
using leaveManagement.Domain.Repositories;

namespace leaveManagement.Application.Services;

public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly CreateLeaveRequestValidator _createValidator;
        private readonly UpdateLeaveRequestValidator _updateValidator;


        public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository, 
                                   IEmployeeRepository employeeRepository, 
                                   IMapper mapper,CreateLeaveRequestValidator createValidator,
                                   UpdateLeaveRequestValidator updateValidator)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync()
        {
            var leaveRequests = await _leaveRequestRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests);
        }

        public async Task<LeaveRequestDto> GetLeaveRequestByIdAsync(int id)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
            if (leaveRequest == null)
                return null;

            return _mapper.Map<LeaveRequestDto>(leaveRequest);
        }

        public async Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createLeaveRequestDto)
        {
            var validationResult = await _createValidator.ValidateAsync(createLeaveRequestDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            LeaveRequest leaveRequest = _mapper.Map<LeaveRequest>(createLeaveRequestDto);
            leaveRequest.Status = Domain.Enums.LeaveStatus.Pending;
            leaveRequest.CreatedAt = DateTime.UtcNow;

            LeaveRequest result = await _leaveRequestRepository.AddAsync(leaveRequest);
            return _mapper.Map<LeaveRequestDto>(result);
        }

        public async Task<LeaveRequestDto> UpdateLeaveRequestAsync(UpdateLeaveRequestDto updateLeaveRequestDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateLeaveRequestDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            
            var existingLeaveRequest = await _leaveRequestRepository.GetByIdAsync(updateLeaveRequestDto.Id);
            if (existingLeaveRequest == null)
                return null;

            _mapper.Map(updateLeaveRequestDto, existingLeaveRequest);
            await _leaveRequestRepository.UpdateAsync(existingLeaveRequest);
            return _mapper.Map<LeaveRequestDto>(existingLeaveRequest);
        }

        public async Task DeleteLeaveRequestAsync(int id)
        {
            await _leaveRequestRepository.DeleteAsync(id);
        }
        
        public async Task<PagedResult<LeaveRequestDto>> FilterLeaveRequestsAsync(LeaveRequestFilterDto filterDto)
        {
            // Get the base query from the repository
            IQueryable<LeaveRequest> query = _leaveRequestRepository.GetQueryable();

            // Apply filtering
            query = ApplyFilters(query, filterDto);

            // Count total results before pagination
            int totalCount = await Task.FromResult(query.Count());

            // Apply sorting
            query = ApplySorting(query, filterDto.SortBy, filterDto.SortOrder);

            // Apply pagination
            query = query.Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize);

            // Execute query and map to DTOs
            var leaveRequests = await Task.FromResult(query.ToList());
            var leaveRequestDtos = _mapper.Map<List<LeaveRequestDto>>(leaveRequests);

            // Return paged result
            return new PagedResult<LeaveRequestDto>
            {
                Items = leaveRequestDtos,
                TotalCount = totalCount,
                PageNumber = filterDto.PageNumber,
                PageSize = filterDto.PageSize
            };
        }
        
        private IQueryable<LeaveRequest> ApplyFilters(IQueryable<LeaveRequest> query, LeaveRequestFilterDto filterDto)
        {
            if (filterDto.EmployeeId.HasValue)
            {
                query = query.Where(lr => lr.EmployeeId == filterDto.EmployeeId.Value);
            }

            if (filterDto.LeaveType.HasValue)
            {
                query = query.Where(lr => lr.LeaveType == filterDto.LeaveType.Value);
            }

            if (filterDto.Status.HasValue)
            {
                query = query.Where(lr => lr.Status == filterDto.Status.Value);
            }

            if (filterDto.StartDate.HasValue)
            {
                query = query.Where(lr => lr.StartDate >= filterDto.StartDate.Value);
            }

            if (filterDto.EndDate.HasValue)
            {
                query = query.Where(lr => lr.EndDate <= filterDto.EndDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(filterDto.Keyword))
            {
                query = query.Where(lr => lr.Reason.Contains(filterDto.Keyword));
            }

            return query;
        }
        
        private IQueryable<LeaveRequest> ApplySorting(IQueryable<LeaveRequest> query, string sortBy, string sortOrder)
        {
            // Default sorting by Id if invalid property name is provided
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                sortBy = "Id";
            }

            bool isAscending = string.IsNullOrWhiteSpace(sortOrder) || sortOrder.ToLower() == "asc";

            // Apply sorting based on property name
            query = sortBy.ToLower() switch
            {
                "id" => isAscending ? query.OrderBy(lr => lr.Id) : query.OrderByDescending(lr => lr.Id),
                "employeeid" => isAscending ? query.OrderBy(lr => lr.EmployeeId) : query.OrderByDescending(lr => lr.EmployeeId),
                "leavetype" => isAscending ? query.OrderBy(lr => lr.LeaveType) : query.OrderByDescending(lr => lr.LeaveType),
                "startdate" => isAscending ? query.OrderBy(lr => lr.StartDate) : query.OrderByDescending(lr => lr.StartDate),
                "enddate" => isAscending ? query.OrderBy(lr => lr.EndDate) : query.OrderByDescending(lr => lr.EndDate),
                "status" => isAscending ? query.OrderBy(lr => lr.Status) : query.OrderByDescending(lr => lr.Status),
                "createdat" => isAscending ? query.OrderBy(lr => lr.CreatedAt) : query.OrderByDescending(lr => lr.CreatedAt),
                _ => isAscending ? query.OrderBy(lr => lr.Id) : query.OrderByDescending(lr => lr.Id)
            };

            return query;
        }
        
        public async Task<IEnumerable<LeaveReportDto>> GetLeaveReportAsync(int year)
        {
            var employees = await _employeeRepository.GetAllAsync();
            var leaveRequests = await _leaveRequestRepository.GetAllAsync();
            
            var approvedLeaves = leaveRequests
                .Where(lr => lr.StartDate.Year == year && lr.Status == LeaveStatus.Approved);
            
            var report = employees.Select(emp => {
                var employeeLeaves = approvedLeaves.Where(lr => lr.EmployeeId == emp.Id);
                
                return new LeaveReportDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.FullName,
                    Department = emp.Department,
                    TotalLeaves = employeeLeaves.Count(),
                    AnnualLeaves = employeeLeaves.Count(lr => lr.LeaveType == LeaveType.Annual),
                    SickLeaves = employeeLeaves.Count(lr => lr.LeaveType == LeaveType.Sick)
                };
            });
            
            return report;
        }
        
        public async Task<bool> ApproveLeaveRequestAsync(int id)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
            
            if (leaveRequest == null || leaveRequest.Status != LeaveStatus.Pending)
            {
                return false;
            }
            
            leaveRequest.Status = LeaveStatus.Approved;
            await _leaveRequestRepository.UpdateAsync(leaveRequest);
            return true;
        }
    }