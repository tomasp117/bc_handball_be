using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services.Models
{
    public class GeneratedMatchInfo
    {
        public string Home { get; set; } = string.Empty;     // např. "1.A"
        public string Away { get; set; } = string.Empty;     // např. "2.C"
        public string Label { get; set; } = string.Empty;    // např. "SF1"
    }
}
