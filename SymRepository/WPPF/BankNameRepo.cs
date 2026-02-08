using SymServices.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;

namespace SymRepository.WPPF
{
    public class BankNameRepo
    {

        public List<BankNameVM> DropDown(string TransType = "WPPF", string BranchId="")
        {
            try
            {
                return new BankNameDAL().DropDown(TransType, BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<BankNameVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new BankNameDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(BankNameVM vm)
        {
            try
            {
                return new BankNameDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(BankNameVM vm)
        {
            try
            {
                return new BankNameDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(BankNameVM vm, string[] ids)
        {
            try
            {
                return new BankNameDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
