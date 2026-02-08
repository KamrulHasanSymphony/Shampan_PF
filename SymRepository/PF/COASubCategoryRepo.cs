using SymServices.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;

namespace SymRepository.PF
{
    public class COASubCategoryRepo
    {
        public List<COAGroupVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new COASubCategoryDAL().SelectAll(Id, conditionFields, conditionValues);
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
                return new COASubCategoryDAL().Insert(vm);
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
                return new COASubCategoryDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
