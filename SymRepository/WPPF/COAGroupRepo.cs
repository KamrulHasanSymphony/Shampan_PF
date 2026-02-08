using SymServices.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;

namespace SymRepository.WPPF
{
    public class COAGroupRepo
    {

        public List<COAGroupVM> DropDown(string TransType = "WPPF")
        {
            try
            {
                return new COAGroupDAL().DropDown(TransType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<COAGroupVM> COATypeOfReportDropDown(string TransType = "WPPF")
        {
            try
            {
                return new COAGroupDAL().COATypeOfReportDropDown(TransType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<COAGroupVM> COAGroupTypeDropDown(string TransType = "WPPF")
        {
            try
            {
                return new COAGroupDAL().COAGroupTypeDropDown(TransType);
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
                return new COAGroupDAL().SelectAll(Id, conditionFields, conditionValues);
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
                return new COAGroupDAL().Insert(vm);
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
                return new COAGroupDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(COAGroupVM vm, string[] ids)
        {
            try
            {
                return new COAGroupDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

    }
}
