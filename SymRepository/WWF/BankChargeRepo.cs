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
    public class BankChargeRepo
    {
        public List<BankChargeVM> SelectAllNotTransferPDF(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new BankChargeDAL().SelectAllNotTransferPDF(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BankChargeVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new BankChargeDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(BankChargeVM vm)
        {
            try
            {
                return new BankChargeDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(BankChargeVM vm)
        {
            try
            {
                return new BankChargeDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(BankChargeVM vm, string[] ids)
        {
            try
            {
                return new BankChargeDAL().Delete(vm, ids);
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
                return new BankChargeDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(BankChargeVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new BankChargeDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
