using SymServices.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;

namespace SymRepository.WPPF
{
    public class WithdrawTypeRepo
    {
        
        public List<WithdrawTypeVM> DropDown()
        {
            try
            {
                return new WithdrawTypeDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<WithdrawTypeVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new WithdrawTypeDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
