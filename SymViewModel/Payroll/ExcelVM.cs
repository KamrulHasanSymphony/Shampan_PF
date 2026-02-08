using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Payroll
{
    public class ExcelVM
    {
        public ExcelPackage varExcelPackage { get; set; }

        public FileStream varFileStream { get; set; }
        public string SheetName { get; set; }
        public string FileName { get; set; }
    }
}
