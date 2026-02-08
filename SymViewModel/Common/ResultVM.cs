using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SymViewModel.Common
{
    public class ResultVM
    {

        public string Status { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }

        public ExcelPackage excel;

        public string ReportName { get; set; }




    }
}
