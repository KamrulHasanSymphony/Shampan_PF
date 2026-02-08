using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.Common
{
  public  class ExportImportVM
    {
      public List<string> IDs { get; set; }
      public string Type { get; set; }
      public HttpPostedFileBase file { get; set; }
      public string BranchId { get; set; }
    }
}
