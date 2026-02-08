using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
   public class RolesVM
    {
       public string Id { get; set; }
       public string Category { get; set; }
       public string Description { get; set; }
       public bool IsArchived { get; set; }

    }
}
