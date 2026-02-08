using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymWebUI.Areas.Config.Controllers
{
   public class EnumLeftTypeRepo
    {
       EnumLoanTypeDAL _enumDAL = new EnumLoanTypeDAL();

       public List<EnumLeftTypeVM> DropDown()
       {
           try
           {
               return _enumDAL.DropDownLeftType();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
    }
}
