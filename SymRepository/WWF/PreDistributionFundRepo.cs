using SymServices.WWF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.WWF
{
    public class PreDistributionFundRepo
    {
        
        public List<PreDistributionFundVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PreDistributionFundDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        public string[] Insert(PreDistributionFundVM vm)
        {
            try
            {
                return new PreDistributionFundDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(PreDistributionFundVM vm)
        {
            try
            {
                return new PreDistributionFundDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Delete(PreDistributionFundVM vm, string[] ids)
        {
            try
            {
                return new PreDistributionFundDAL().Delete(vm, ids);
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
                return new PreDistributionFundDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(PreDistributionFundVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PreDistributionFundDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] InsertAutoJournal(string JournalType, string TransactionForm, string TransactionCode, string BranchId, ShampanIdentityVM vm)
        {
            try
            {
                return new PreDistributionFundDAL().AutoJournalSave(JournalType, TransactionForm, TransactionCode, BranchId, null, null, vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
