using SymServices.WWF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.WWF
{
    public class PFBankDepositRepo
    {
        public List<PFBankDepositVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFBankDepositDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       


        public string[] Insert(PFBankDepositVM vm)
        {
            try
            {
                return new PFBankDepositDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(PFBankDepositVM vm)
        {
            try
            {
                return new PFBankDepositDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(PFBankDepositVM vm, string[] ids)
        {
            try
            {
                return new PFBankDepositDAL().Delete(vm, ids);
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
                return new PFBankDepositDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable Report(PFBankDepositVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFBankDepositDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable PFBankStatementReport(PFBankDepositVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFBankDepositDAL().PFBankStatementReport(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
