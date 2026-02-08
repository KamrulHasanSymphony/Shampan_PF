using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class CompanyRepo
    {
        CompanyDAL _dal = new CompanyDAL();
        #region Methods
        public List<CompanyVM> DropDown()
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
        //==================SelectAll=================
        public List<CompanyVM> SelectAll()
        {
            try
            {
                return new CompanyDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public CompanyVM SelectById(int Id)
        {
            try
            {
                return new CompanyDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(CompanyVM companyVM)
        {
            try
            {
                return new CompanyDAL().Insert(companyVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(CompanyVM companyVM)
        {
            try
            {
                return new CompanyDAL().Update(companyVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public CompanyVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {
            try
            {
                return new CompanyDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(CompanyVM companyVM)
        {
            try
            {
                return new CompanyDAL().Delete(companyVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
