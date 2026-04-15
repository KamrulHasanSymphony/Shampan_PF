using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices;

namespace SymRepository
{
    public class DbCreateRepo
    {
        public string[] Insert(DbCreateVM VM)
        {
            try
            {
                return new DbCreateDAL().Insert(VM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] TestConnection(DbCreateVM VM)
        {
            try
            {
                return new DbCreateDAL().TestConnection(VM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
