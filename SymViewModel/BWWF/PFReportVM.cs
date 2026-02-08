using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using OfficeOpenXml;
using SymViewModel.Enum;
using System.ComponentModel.DataAnnotations;

namespace SymViewModel.BWWF
{
    public class PFReportVM
    {
        public MemoryStream MemStream { get; set; }
        public string FileName { get; set; }
        public DataTable DataTable { get; set; }
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public int PFHeaderId { get; set; }

        public string Code { get; set; }
        public string ReportType { get; set; }
         [Display(Name = "Date From")]
        public string DateFrom { get; set; }
        [Display(Name = "Date To")]
        public string DateTo { get; set; }

        public string YearFrom { get; set; }
        public string YearTo { get; set; }


        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }

        public string FiscalYear { get; set; }

        public string JournalType { get; set; }

        public string SheetName { get; set; }
        public ExcelPackage varExcelPackage { get; set; }
        public string varFileDirectory { get; set; }
        public FileStream varFileStream { get; set; }
        public string TransType { get; set; }
        public bool OptionA { get; set; }
        public string IdFrom { get; set; }
        public PFReport1VM PFReport1VM = new PFReport1VM();
        public List<PFReport1VM> PFReport1VMs = new List<PFReport1VM>();
        public BaseEntity BaseEntity { get; set; }

        public PFReportVM()
        {
            BaseEntity = new BaseEntity();
        }
        public string HRMDB { get; set; }

        public object BranchId { get; set; }
       
    }

    public class PFReport1VM
    {
        public string FirstStart { get; set; }
        public string FirstEnd { get; set; }
        public string FirstYear { get; set; }
        public string LastStart { get; set; }
        public string LastEnd { get; set; }
        public string LastYear { get; set; }

        public decimal FirstRetainedEarning { get; set; }
        public decimal FirstNetProfit { get; set; }
        public decimal LastRetainedEarning { get; set; }
        public decimal LastNetProfit { get; set; }
    }

}
