using SymServices.WPPF;
using SymServices.Common;
using SymViewModel.WPPF;
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
    public class ReservedFundRepo
    {
        
        
        public List<ReservedFundVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ReservedFundDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(ReservedFundVM vm)
        {
            try
            {
                return new ReservedFundDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(ReservedFundVM vm)
        {
            try
            {
                return new ReservedFundDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(ReservedFundVM vm, string[] ids)
        {
            try
            {
                return new ReservedFundDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Post(string[] ids)
        {
            try
            {
                return new ReservedFundDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(ReservedFundVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ReservedFundDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
