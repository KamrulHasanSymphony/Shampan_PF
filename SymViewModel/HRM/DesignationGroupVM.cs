using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.HRM
{
    public class DesignationGroupVM
    {

        public string Id { get; set; }
        public int BranchId { get; set; }
        public int Serial { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }

        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}
