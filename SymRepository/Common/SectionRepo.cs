using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class SectionRepo
    {
       SectionDAL _dal = new SectionDAL();
       #region Methods
       public List<SectionVM> DropDown()
        {
            try
            {
                return _dal.DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       public List<DropDownVM> DropDownByDepartment( string departmentId)
       {
           try
           {
               return _dal.DropDownByDepartment(departmentId);
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
                return _dal.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       //==================SelectAll=================
        public List<SectionVM> SelectAll()
        {
            try
            {
                return new SectionDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public SectionVM SelectById(string Id)
        {
            try
            {
                return new SectionDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(SectionVM dVM)
        {
            try
            {
                return new SectionDAL().Insert(dVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(SectionVM vm)
        {
            try
            {
                return new SectionDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public SectionVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new SectionDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(SectionVM vm, string[] ids)
        {
            try
            {
                return new SectionDAL().Delete(vm,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public string[] ImportExcelFile(SectionVM vm)
        {
            try
            {
                SectionDAL dal = new SectionDAL();
                return dal.InsertExportData(vm, null, null);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
