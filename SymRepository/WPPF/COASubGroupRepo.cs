using SymServices.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;

namespace SymRepository.WPPF
{
    public class COASubGroupRepo
    {

        public List<COAGroupVM> DropDown()
        {
            try
            {
                return new COASubGroupDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public List<COAGroupVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new COASubGroupDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(COAGroupVM vm)
        {
            try
            {
                return new COASubGroupDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(COAGroupVM vm)
        {
            try
            {
                return new COASubGroupDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
