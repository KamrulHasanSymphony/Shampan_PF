using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class SMSSenderVM
    {
        public int Id { get; set; }
        public string SenderMobileNo { get; set; }
        public string RecepentMobileNo { get; set; }
        public string SMSBody { get; set; }
        public string FullName  { get; set; }
        public int PresentDistrictId { get; set; }
        public int JobCategoryId { get; set; }
        public string Website { get; set; }
    }
}
