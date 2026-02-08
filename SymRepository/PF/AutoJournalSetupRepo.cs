using SymServices.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.PF
{
    public class AutoJournalSetupRepo
    {
        public List<AutoJournalSetupVM> JournalForDropDown()
        {
            try
            {
                return new AutoJournalSetupDAL().JournalForDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<AutoJournalSetupVM> SelectAll(string BranchId, int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new AutoJournalSetupDAL().SelectAll(BranchId, Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(AutoJournalSetupVM vm)
        {
            try
            {
                return new AutoJournalSetupDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(AutoJournalSetupVM vm)
        {
            try
            {
                return new AutoJournalSetupDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Delete(string[] ids)
        {
            try
            {
                return new AutoJournalSetupDAL().Delete(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AutoJournalSetupVM SelectById(string Id)
        {
            try
            {
                return new AutoJournalSetupDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
