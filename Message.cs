using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChGPTcmd
{
    public class Message
    {
        public ChatRole Role { get; set; }
        public string? Content { get; set; }
    }
}
