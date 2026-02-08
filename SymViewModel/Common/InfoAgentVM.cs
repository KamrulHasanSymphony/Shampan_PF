using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class InfoAgentVM
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhotoName { get; set; }
        public int PresentDistrictId { get; set; }
        public string MobileBankType { get; set; }
        public string MobileBankNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
    }
}
