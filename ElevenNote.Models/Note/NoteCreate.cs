using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElevenNote.Models.Note
{
    public class NoteCreate
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
