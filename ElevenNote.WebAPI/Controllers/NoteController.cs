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

        // GET: api/Note
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            _service.SetUserId(userId.Value);

            return Ok(await _service.GetAllNotesAsync());
        }

        // POST: api/Note
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NoteCreate model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            _service.SetUserId(userId.Value);

            var createResult = await _service.CreateNoteAsync(model);
            if (createResult)
                return Ok("Note was created.");

            return BadRequest("Note could not be created.");
        }

        // GET: api/Note/5
        [HttpGet]
        [Route("{noteId:int}")]
        public async Task<IActionResult> GetById([FromRoute] int noteId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            _service.SetUserId(userId.Value);

            var detail = await _service.GetNoteByIdAsync(noteId);

            if (detail != null)
                return Ok(detail);

            return NotFound(detail);
        }

        // PUT: api/Note
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] NoteUpdate model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            _service.SetUserId(userId.Value);

            if (await _service.UpdateNoteAsync(model))
                return Ok($"Note {model.Id} was updated.");

            return BadRequest($"Note {model.Id} could not be updated.");
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
