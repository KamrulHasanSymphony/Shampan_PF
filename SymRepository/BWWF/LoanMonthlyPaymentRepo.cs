using System;
using System.Collections.Generic;
using SymServices.BWWF;
using SymViewModel.BWWF;

namespace SymRepository.BWWF
{
    public class LoanMonthlyPaymentRepo
    {

        public List<LoanMonthlyPaymentVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new LoanMonthlyPaymentDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(LoanMonthlyPaymentVM vm)
        {
            try
            {
                return new LoanMonthlyPaymentDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(LoanMonthlyPaymentVM vm)
        {
            try
            {
                return new LoanMonthlyPaymentDAL().Update(vm);
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
                return new LoanMonthlyPaymentDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}