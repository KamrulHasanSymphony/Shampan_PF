using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class DesignationGroupRepo
    {

        DesignationGroupDAL _dal = new DesignationGroupDAL();

        #region Methods
        public List<DesignationGroupVM> DropDown()
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
        public List<DesignationGroupVM> SelectAll()
        {
            try
            {
                return new DesignationGroupDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public DesignationGroupVM SelectById(string Id)
        {
            try
            {
                return new DesignationGroupDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(DesignationGroupVM vm)
        {
            try
            {
                return new DesignationGroupDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(DesignationGroupVM vm)
        {
            try
            {
                return new DesignationGroupDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public DesignationGroupVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new DesignationGroupDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(DesignationGroupVM vm, string[] ids)
        {
            try
            {
                return new DesignationGroupDAL().Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        public string[] ImportExcelFile(DesignationGroupVM vm)
        {

            try
            {
                DesignationGroupDAL dal = new DesignationGroupDAL();
                return dal.InsertExportData(vm, null, null);
            }
            catch (Exception)
            {
                throw;
            }
        }
    
    }
}
