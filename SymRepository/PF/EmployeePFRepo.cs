using SymServices.Common;
using SymServices.PF;
using SymViewModel.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.PF
{
    public class EmployeePFRepo
    {
        EmployeePFDAL _dal = new EmployeePFDAL();
        #region Methods
        public List<EmployeePFVM> DropDown()
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
        public List<EmployeePFVM> SelectAll()
        {
            try
            {
                return new EmployeePFDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeePFVM SelectById(string Id)
        {
            try
            {
                return new EmployeePFDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeePFVM vm)
        {
            try
            {
                return new EmployeePFDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeePFVM vm)
        {
            try
            {
                return new EmployeePFDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeePFVM vm, string[] Ids)
        {
            try
            {
                return new EmployeePFDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] ImportExcelFile(string fullPath, string fileName, ShampanIdentityVM auditvm)
        {
            try
            {
                return _dal.ImportExcelFile(fullPath, fileName, auditvm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable ExportExcelFile(EmployeePFVM vm, string[] conFields = null, string[] conValues = null)
        {
            try
            {
                return _dal.ExportExcelFile(vm, conFields, conValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion
    }
}
