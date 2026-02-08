using SymServices.WWF;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;

namespace SymRepository.WWF
{
    public class SettlementPolicyRepo
    {
        
        
        public List<SettlementPolicyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new SettlementPolicyDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(SettlementPolicyVM vm)
        {
            try
            {
                return new SettlementPolicyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(SettlementPolicyVM vm)
        {
            try
            {
                return new SettlementPolicyDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Delete(SettlementPolicyVM vm, string[] ids)
        {
            try
            {
                return new SettlementPolicyDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
