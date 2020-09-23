using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.Note;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevenNote.Services.Note
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;
        private int _userId;

        public NoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NoteListItem>> GetAllNotesAsync()
        {
            var noteQuery =
                _context
                    .Notes
                    .Where(entity => entity.OwnerId == _userId)
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
                OwnerId = _userId,
                CreatedUtc = DateTimeOffset.Now
            };

            _context.Notes.Add(noteEntity);

            var numberOfChanges = await _context.SaveChangesAsync();
            return numberOfChanges == 1;
        }

        public async Task<NoteDetail> GetNoteByIdAsync(int noteId)
        {

            var noteEntity = await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteId && n.OwnerId == _userId);

            if (noteEntity is null)
                return null;

            var detail = new NoteDetail
            {
                Id = noteEntity.Id,
                Title = noteEntity.Title,
                Content = noteEntity.Content,
                CreatedUtc = noteEntity.CreatedUtc,
                ModifiedUtc = noteEntity.ModifiedUtc
            };

            return detail;
        }

        public async Task<bool> UpdateNoteAsync(NoteUpdate model)
        {
            var entity = await _context.Notes.FindAsync(model.Id);

            if (entity?.OwnerId != _userId)
                return false;

            entity.Title = model.Title;
            entity.Content = model.Title;
            entity.ModifiedUtc = DateTimeOffset.Now;

            return await _context.SaveChangesAsync() == 1;
        }

        public async Task<bool> DeleteNoteAsync(int noteId)
        {
            var entity = await _context.Notes.FindAsync(noteId);
            if (entity?.OwnerId != _userId)
                return false;

            _context.Notes.Remove(entity);
            return await _context.SaveChangesAsync() == 1;
        }

        public void SetUserId(int userId) => _userId = userId;
    }
}
