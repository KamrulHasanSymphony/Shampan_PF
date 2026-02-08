using SymServices.Enum;
using SymViewModel.Common;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
  public  class EnumYearRepo
    {
        EnumYearDAL _enumDAL = new EnumYearDAL();
        #region Method

        public List<EnumYearVM> DropDown()
        {
            try
            {
                return _enumDAL.DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> Autocomplete(string term)
        {
            try
            {
                return _enumDAL.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EnumYearVM> SelectAll()
        {
            try
            {
                return _enumDAL.SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EnumYearVM SelectById(int Id)
        {
            try
            {
                return _enumDAL.SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EnumYearVM vm)
        {
            try
            {
                return _enumDAL.Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(EnumYearVM vm)
        {
            try
            {
                return _enumDAL.Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(EnumYearVM vm, string[] ids)
        {
            try
            {
                return _enumDAL.Delete(vm,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
