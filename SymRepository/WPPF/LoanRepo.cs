using SymServices.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;

namespace SymRepository.WPPF
{
    public class LoanRepo
    {
       
        public List<LoanVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new LoanDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(LoanVM vm)
        {
            try
            {
                return new LoanDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(LoanVM vm)
        {
            try
            {
                return new LoanDAL().Update(vm);
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
                return new LoanDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
