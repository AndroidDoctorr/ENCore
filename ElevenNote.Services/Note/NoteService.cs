using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.Note;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevenNote.Services.Note
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;
        public NoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int UserId { get; set; }

        public async Task<IEnumerable<NoteListItem>> GetAllNotesAsync()
        {
            var noteQuery =
                _context
                    .Notes
                    .Where(entity => entity.OwnerId == UserId)
                    .Select(entity =>
                        new NoteListItem
                        {
                            Id = entity.Id,
                            Title = entity.Title,
                            CreatedUtc = entity.CreatedUtc
                        });

            return await noteQuery.ToListAsync();
        }

        public async Task<bool> CreateNoteAsync(NoteCreate model)
        {
            var noteEntity = new NoteEntity
            {
                Title = model.Title,
                Content = model.Content,
                OwnerId = UserId,
                CreatedUtc = DateTimeOffset.Now
            };

            _context.Notes.Add(noteEntity);
            return await _context.SaveChangesAsync() == 1;
        }
    }
}
