using SymServices.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;

namespace SymRepository.PF
{
    public class TransactionMediaRepo
    {
        
        public List<TransactionMediaVM> DropDown()
        {
            try
            {
                return new TransactionMediaDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TransactionMediaVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new TransactionMediaDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(TransactionMediaVM vm)
        {
            try
            {
                return new TransactionMediaDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(TransactionMediaVM vm)
        {
            try
            {
                return new TransactionMediaDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(TransactionMediaVM vm, string[] ids)
        {
            try
            {
                return new TransactionMediaDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
