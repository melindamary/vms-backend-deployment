﻿using Microsoft.AspNetCore.Mvc;
using VMS.Repository.IRepository;
using VMS.Models;
using VMS.Models.DTO;
using VMS.Services;
using Microsoft.AspNetCore.SignalR;
using VMS.AVHubs;

namespace VMS.Controllers
{

    [ApiController]
    [Route("[controller]/[action]")]
    public class VisitorController : ControllerBase
    {
        private readonly IVisitorFormService _visitorService;
        private readonly IHubContext<VisitorHub> _hubContext;

        public VisitorController(IVisitorFormService visitorService, IHubContext<VisitorHub> hubContext)
        {
            _visitorService = visitorService;
            _hubContext = hubContext;
        }

        [HttpGet("details")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(APIResponse))]
        public async Task<ActionResult<IEnumerable<Visitor>>> GetVisitorDetails()
        {
            var visitors = await _visitorService.GetVisitorDetailsAsync();
            return Ok(visitors);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(APIResponse))]
        public async Task<ActionResult<IEnumerable<string>>> GetPersonInContact()
        {
            var contacts = await _visitorService.GetPersonInContactAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(APIResponse))]
        public async Task<ActionResult<Visitor>> GetVisitorById(int id)
        {
            var visitor = await _visitorService.GetVisitorByIdAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }
            return Ok(visitor);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(APIResponse))]
        public async Task<ActionResult<Visitor>> CreateVisitor(VisitorCreationDTO visitorDto)
        {
            var visitor = await _visitorService.CreateVisitorAsync(visitorDto);
            if (visitor != null)
            {
                // Notify all connected clients to reload the visitor log
                await _hubContext.Clients.All.SendAsync("ReloadVisitorLog");
            }
            return CreatedAtAction(nameof(GetVisitorById), new { id = visitor.Id }, visitor);
        }


        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(APIResponse))]
        [HttpPost]
        public async Task<ActionResult<VisitorDevice>> AddVisitorDevice(AddVisitorDeviceDTO addDeviceDto)
        {
            var device = await _visitorService.AddVisitorDeviceAsync(addDeviceDto);
            return CreatedAtAction(nameof(GetVisitorById), new { id = device.VisitorId }, device);
        }
    }
}
