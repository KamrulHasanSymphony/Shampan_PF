using SymServices.PF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.PF
{
    public class ReturnOnBankInterestRepo
    {
        public List<ReturnOnBankInterestVM> SelectAllNotTransferPDF(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ReturnOnBankInterestDAL().SelectAllNotTransferPDF(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ReturnOnBankInterestVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ReturnOnBankInterestDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(ReturnOnBankInterestVM vm)
        {
            try
            {
                return new ReturnOnBankInterestDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(ReturnOnBankInterestVM vm)
        {
            try
            {
                return new ReturnOnBankInterestDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(ReturnOnBankInterestVM vm, string[] ids)
        {
            try
            {
                return new ReturnOnBankInterestDAL().Delete(vm, ids);
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
                return new ReturnOnBankInterestDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(ReturnOnBankInterestVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ReturnOnBankInterestDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
