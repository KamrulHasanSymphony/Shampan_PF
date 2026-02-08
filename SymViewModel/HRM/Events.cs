using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.HRM
{
    public class Events
    {
        public int EventId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }


    }
}
