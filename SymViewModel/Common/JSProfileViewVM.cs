using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class JSProfileViewVM
    {
        public int Id { get; set; }
        public string ViewType { get; set; }
        public string ViewDate { get; set; }
        public string FullName { get; set; }
        public int PresentDistrictId { get; set; }
        public int JobCategoryId { get; set; }
        public string Website { get; set; }
    }
}
