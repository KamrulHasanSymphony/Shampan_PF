using SymServices.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;

namespace SymRepository.WPPF
{
    public class COARepo
    {

        public List<COAVM> DropDown(string TransType = "WPPF")
        {
            try
            {
                return new COADAL().DropDown(TransType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<COAVM> COATypeDropDown()

        {
            try
            {
                return new COADAL().COATypeDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<COAVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
               return new COADAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(COAVM vm)
        {
            try
            {
                return new COADAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(COAVM vm)
        {
            try
            {
                return new COADAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(COAVM vm, string[] ids)
        {
            try
            {
                return new COADAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

    }
}
