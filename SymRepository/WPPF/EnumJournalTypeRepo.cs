
using SymServices.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;

namespace SymRepository.WPPF
{
    public class EnumJournalTypeRepo
    {

        public List<EnumJournalTypeVM> JournalTypeDropDown()
        {
            try
            {
                return new EnumJournalTypeDAL().JournalTypeDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EnumJournalTransactionTypeVM> JournalTransactionTypeDropDown()
        {
            try
            {
                return new EnumJournalTypeDAL().JournalTransactionTypeDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EnumJournalTransactionTypeVM> SelectAllJournalTransactionType(int Id = 0,string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EnumJournalTypeDAL().SelectAllJournalTransactionType(Id, conditionFields , conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
