using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class BankRepo
    {
        #region Methods
        //==================SelectAll=================
       public List<BankVM> DropDown()
        {
            try
            {
                return new BankDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAll=================
        public List<BankVM> SelectAll()
        {
            try
            {
                return new BankDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public BankVM SelectById(string Id)
        {
            try
            {
                return new BankDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(BankVM bankVM)
        {
            try
            {
                return new BankDAL().Insert(bankVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(BankVM bankVM)
        {
            try
            {
                return new BankDAL().Update(bankVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public BankVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new BankDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(BankVM bankVM, string[] ids)
        {
            try
            {
                return new BankDAL().Delete(bankVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public string[] ImportExcelFile(BankVM vm)
        {
            try
            {
                BankDAL dal = new BankDAL();
                return dal.InsertExportData(vm, null, null);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
