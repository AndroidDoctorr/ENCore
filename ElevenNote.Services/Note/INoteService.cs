using ElevenNote.Models.Note;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services.Note
{
    public interface INoteService
    {
        void SetUserId(int userId);

        Task<IEnumerable<NoteListItem>> GetAllNotesAsync();
        Task<bool> CreateNoteAsync(NoteCreate model);
        Task<NoteDetail> GetNoteByIdAsync(int noteId);
        Task<bool> UpdateNoteAsync(NoteUpdate model);
        Task<bool> DeleteNoteAsync(int noteId);
    }
}
