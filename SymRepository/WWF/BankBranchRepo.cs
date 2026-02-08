using SymServices.WWF;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;

namespace SymRepository.WWF
{
    public class BankBranchRepo
    {
        public List<BankBranchVM> DropDown(string TransType = "WWF")
        {
            try
            {
                return new BankBranchDAL().DropDown(TransType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<BankBranchVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new BankBranchDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(BankBranchVM vm)
        {
            try
            {
                return new BankBranchDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(BankBranchVM vm)
        {
            try
            {
                return new BankBranchDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(BankBranchVM vm, string[] ids)
        {
            try
            {
                return new BankBranchDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
