using SymServices.PF;
using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.PF
{
    public class PFSettlementRepo
    {
        public List<PFSettlementVM> SelectAll_ResignEmployee(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFSettlementDAL().SelectAll_ResignEmployee(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<PFSettlementVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFSettlementDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(PFSettlementVM vm)
        {
            try
            {
                return new PFSettlementDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PFSettlementVM PreInsert(PFSettlementVM vm)
        {
            try
            {
                return new PFSettlementDAL().PreInsert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(string ids)
        {
            try
            {
                return new PFSettlementDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(PFSettlementVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFSettlementDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(PFSettlementVM vm)
        {
            try
            {
                return new PFSettlementDAL().Update(vm);
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
                return new PFSettlementDAL().AutoJournalSave(JournalType, TransactionForm, TransactionCode, BranchId, null, null, vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
