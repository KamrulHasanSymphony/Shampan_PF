using SymServices.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.PF
{
    public class EETransactionDetailRepo
    {
        public List<EETransactionDetailVM> SelectAll(int Id = 0, int EETransactionId = 0, string[] conditionField = null, string[] conditionValue = null, bool IsPS=false)
        {
            try
            {
                return new EETransactionDetailDAL().SelectAll(Id, EETransactionId,  conditionField, conditionValue, IsPS);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EETransactionDetailVM vm)
        {
            try
            {
                return new EETransactionDetailDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(EETransactionDetailVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EETransactionDetailDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
