using System;
using System.Collections.Generic;
using SymServices.WWF;
using SymViewModel.WWF;

namespace SymRepository.WWF
{
    public class LoanRepaymentToBankRepo
    {

        public List<LoanRepaymentToBankVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new LoanRepaymentToBankDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(LoanRepaymentToBankVM vm)
        {
            try
            {
                return new LoanRepaymentToBankDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(LoanRepaymentToBankVM vm)
        {
            try
            {
                return new LoanRepaymentToBankDAL().Update(vm);
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
                return new LoanRepaymentToBankDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}