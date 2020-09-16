using ElevenNote.Models.Note;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services.Note
{
    public interface INoteService
    {
        Task<IEnumerable<NoteListItem>> GetAllNotesAsync(int ownerId);
    }
}
