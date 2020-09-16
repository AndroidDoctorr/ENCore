using ElevenNote.Data;
using ElevenNote.Models.Note;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<NoteListItem>> GetAllNotesAsync(int ownerId)
        {
            var noteQuery =
                _context
                    .Notes
                    .Where(entity => entity.OwnerId == ownerId)
                    .Select(entity =>
                        new NoteListItem
                        {
                            Id = entity.Id,
                            Title = entity.Title,
                            CreatedUtc = entity.CreatedUtc
                        });

            return await noteQuery.ToListAsync();
        }
    }
}
