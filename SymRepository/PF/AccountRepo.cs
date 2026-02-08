using SymServices.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;

namespace SymRepository.PF
{
    public class AccountRepo
    {
        public List<AccountVM> DropDown_Account_WithdrawDebitHead(string WithdrawTypeId = "0", string TransType="PF")
        {
            try
            {
                return new AccountDAL().DropDown_Account_WithdrawDebitHead(WithdrawTypeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AccountVM> DropDown(string AccountType = "", string TransType = "PF")
        {
            try
            {
                return new AccountDAL().DropDown(AccountType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AccountVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new AccountDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(AccountVM vm)
        {
            try
            {
                return new AccountDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(AccountVM vm)
        {
            try
            {
                return new AccountDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
