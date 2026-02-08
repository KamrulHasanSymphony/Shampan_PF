using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository
{
   public class UserRoleRepo
    {
       UserRoleDAL userRole = new UserRoleDAL();
       public List<UserRolesVM> SelectAll(string userId, int branchId)
       {
           try
           {
               return userRole.SelectAll(userId,branchId); 
           }
           catch (Exception ex)
           {
               
               throw ex;
           }
       }
    }
}
