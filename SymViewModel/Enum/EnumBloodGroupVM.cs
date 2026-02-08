using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Enum
{
    public class EnumBloodGroupVM : BaseEntity
   {
       public int Id { get; set; }
       [Required(ErrorMessage = "Name is required")]
       public string Name { get; set; }
      
    }
}
