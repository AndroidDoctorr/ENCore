﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ElevenNote.Models.Token
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime Expires { get; set; }
    }
}
