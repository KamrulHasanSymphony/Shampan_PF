using SymServices.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.WPPF
{
    public class EETransactionRepo
    {
        public List<EETransactionVM> DropDown(string tType=null, int branchId = 0)
        {
            try
            {
                return new EETransactionDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EETransactionVM> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new EETransactionDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EETransactionVM vm)
        {
            try
            {
                return new EETransactionDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(EETransactionVM vm)
        {
            try
            {
                return new EETransactionDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(EETransactionVM vm, string[] ids)
        {
            try
            {
                return new EETransactionDAL().Delete(vm,ids);
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
                return new EETransactionDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(EETransactionVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EETransactionDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
