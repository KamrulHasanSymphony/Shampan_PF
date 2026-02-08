using SymServices.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;

namespace SymRepository.PF
{
    public class AccountTypeRepo
    {
        
        public List<AccountTypeVM> DropDown()
        {
            try
            {
                return new AccountTypeDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AccountTypeVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new AccountTypeDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
