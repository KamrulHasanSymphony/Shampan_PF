using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Enum
{
    public class EnumReportVM
    {
        public int Id { get; set; }
        public string ReportId { get; set; }
        
        public int ReportSL { get; set; }
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public string ReportFileName { get; set; }

        public string Name { get; set; }

    }
}
