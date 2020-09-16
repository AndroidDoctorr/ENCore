using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ElevenNote.Models.Note;
using ElevenNote.Services.Note;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace ElevenNote.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _service;
        public NoteController(INoteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            _service.UserId = userId.Value;

            return Ok(await _service.GetAllNotesAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post(NoteCreate model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            _service.UserId = userId.Value;

            if (await _service.CreateNoteAsync(model))
                return Ok("Note was created.");

            return BadRequest("Note could not be created.");
        }

        private int? GetUserId()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst("Id")?.Value;

            if (userId != null)
                return int.Parse(userId);

            return null;
        }
    }
}
