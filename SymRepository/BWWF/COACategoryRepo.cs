using SymServices.BWWF;
using SymViewModel.BWWF;
using System;
using System.Collections.Generic;

namespace SymRepository.BWWF
{
    public class COACategoryRepo
    {
        public List<COAGroupVM> DropDown()
        {
            try
            {
                return new COACategoryDAL().DropDown();
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
                return new COACategoryDAL().SelectAll(Id, conditionFields, conditionValues);
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
                return new COACategoryDAL().Insert(vm);
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
                return new COACategoryDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
