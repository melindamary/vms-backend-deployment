﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using VMS.AVHubs;
using VMS.Models;
using VMS.Models.DTO;
using VMS.Repository.IRepository;
using VMS.Services.IServices;

namespace VMS.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IHubContext<VisitorHub> _hubContext;

        public LocationController(ILocationService  locationService, IHubContext<VisitorHub> hubContext)
        {
            _locationService = locationService;
            _hubContext = hubContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(APIResponse))]
        public async Task<IEnumerable<LocationIdAndNameDTO>> GetLocationIdAndName()
        {
            return await _locationService.GetLocationIdAndNameAsync();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(APIResponse))]
        public async Task<ActionResult<APIResponse>> LocationList()
        {
            var response = new APIResponse();
            try
            {
                var locationDetails = await _locationService.GetAllLocationDetailsAsync();

                response.IsSuccess = true;
                response.Result = locationDetails;
                response.StatusCode = HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<APIResponse>> PostLocation([FromBody] AddOfficeLocationDTO locationdDTO)
        {
            var response = new APIResponse();
            if (!ModelState.IsValid)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Invalid input data.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    response.ErrorMessages.Add(error.ErrorMessage);
                }
                return BadRequest(response);
            }

            try
            {
                var success = await _locationService.AddLocationAsync(locationdDTO);
                if (success)
                {
                    await _hubContext.Clients.All.SendAsync("LocationAdded");
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.Created;
                    response.Result = "Location added successfully.";
                    return CreatedAtAction(nameof(PostLocation), new {  }, response);
                }
                else
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Failed to add location.");
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<APIResponse>> Location(int id, [FromBody] UpdateLocationDTO dto)
        {
            var response = new APIResponse();
            if (!ModelState.IsValid)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Invalid input data.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    response.ErrorMessages.Add(error.ErrorMessage);
                }
                return BadRequest(response);
            }

            try
            {
                var success = await _locationService.UpdateLocationAsync(id, dto);
                if (success)
                {
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Result = "Location updated successfully.";
                    return Ok(response);
                }
                else
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Location not found.");
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }
    }
}

