using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
    public class SchedulePolicyVM
    {
        public int Id { get; set; }
        public string LineNumber { get; set; }
        public string ScheduleNo { get; set; }
        public string SalaryHead { get; set; }
        public bool FromBasic { get; set; }
        public bool IsFixed { get; set; }
        public decimal BasicPortion { get; set; }
        public decimal EqualMaxMinAmount { get; set; }
        public string Remarks { get; set; }
    }
}
