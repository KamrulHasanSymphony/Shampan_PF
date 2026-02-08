using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
  public  class RoleRepo
    {
      RoleDAL roleApp = new RoleDAL();
      public List<RolesVM> SelectAll()
      {
          try
          {
              return roleApp.SelectAll();
          }
          catch (Exception)
          {
              
              throw;
          }
      }
    }
}
