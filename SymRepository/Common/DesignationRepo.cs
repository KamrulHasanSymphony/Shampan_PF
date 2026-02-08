using SymServices.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class DesignationRepo
    {
        DesignationDAL _dal = new DesignationDAL();
        #region Methods
        public List<DesignationVM> DropDown()
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
        public List<DesignationVM> SelectAll()
        {
            try
            {
                return new DesignationDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public DesignationVM SelectById(string Id)
        {
            try
            {
                return new DesignationDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(DesignationVM vm)
        {
            try
            {
                return new DesignationDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(DesignationVM vm)
        {
            try
            {
                return new DesignationDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        //public DesignationVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        //{

        //    try
        //    {
        //        return new DesignationDAL().Select(query, Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //==================Delete =================
        public string[] Delete(DesignationVM vm,string[] ids)
        {
            try
            {
                return new DesignationDAL().Delete(vm,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DesignationGroupVM> DesignationGroupDropDown()
        {
            try
            {
                return new DesignationDAL().DesignationGroupDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
