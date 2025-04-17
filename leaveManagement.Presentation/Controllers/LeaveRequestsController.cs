using FluentValidation;
using leaveManagement.Application.DTOs;
using leaveManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace leaveManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestsController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetAll()
    {
        var leaveRequests = await _leaveRequestService.GetAllLeaveRequestsAsync();
        return Ok(leaveRequests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveRequestDto>> GetById(int id)
    {
        var leaveRequest = await _leaveRequestService.GetLeaveRequestByIdAsync(id);
        if (leaveRequest == null)
            return NotFound();

        return Ok(leaveRequest);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> Create(CreateLeaveRequestDto createLeaveRequestDto)
    {
        try
        {
            var leaveRequest = await _leaveRequestService.CreateLeaveRequestAsync(createLeaveRequestDto);
            return CreatedAtAction(nameof(GetById), new { id = leaveRequest.Id }, leaveRequest);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateLeaveRequestDto updateLeaveRequestDto)
    {
        if (id != updateLeaveRequestDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var leaveRequest = await _leaveRequestService.UpdateLeaveRequestAsync(updateLeaveRequestDto);
            if (leaveRequest == null)
                return NotFound();

            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _leaveRequestService.DeleteLeaveRequestAsync(id);
        return NoContent();
    }
    
    [HttpGet("filter")]
    public async Task<ActionResult<PagedResult<LeaveRequestDto>>> Filter([FromQuery] LeaveRequestFilterDto filterDto)
    {
        var result = await _leaveRequestService.FilterLeaveRequestsAsync(filterDto);
        return Ok(result);
    }
    
    [HttpGet("report")]
    public async Task<ActionResult<IEnumerable<LeaveReportDto>>> GetLeaveReport([FromQuery] int year)
    {
        if (year <= 0)
        {
            return BadRequest("Invalid year parameter");
        }
            
        var report = await _leaveRequestService.GetLeaveReportAsync(year);
        return Ok(report);
    }

    // Admin Approval Endpoint
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveLeaveRequest(int id)
    {
        var result = await _leaveRequestService.ApproveLeaveRequestAsync(id);
            
        if (!result)
        {
            return BadRequest("Unable to approve the leave request. It may not exist or is not in pending status.");
        }
            
        return Ok(new { message = "Leave request approved successfully" });
    }
}