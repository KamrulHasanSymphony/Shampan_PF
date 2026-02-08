using SymServices.WPPF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.WPPF
{
    public class GLTransactionDetailRepo
    {

         public DataTable Report(PFParameterVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLTransactionDetailDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
