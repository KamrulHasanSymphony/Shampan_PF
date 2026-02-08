using System;
using System.Collections.Generic;
using SymServices.WWF;
using SymViewModel.WWF;

namespace SymRepository.WWF
{
    public class LoanSattlementRepo
    {

        public List<LoanSattlementVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new LoanSattlementDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(LoanSattlementVM vm)
        {
            try
            {
                return new LoanSattlementDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(LoanSattlementVM vm)
        {
            try
            {
                return new LoanSattlementDAL().Update(vm);
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
                return new LoanSattlementDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}