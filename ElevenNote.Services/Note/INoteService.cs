using ElevenNote.Models.Note;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services.Note
{
    public interface INoteService
    {
        int UserId { get; set; }
        Task<IEnumerable<NoteListItem>> GetAllNotesAsync();
        Task<bool> CreateNoteAsync(NoteCreate model);
    }
}
