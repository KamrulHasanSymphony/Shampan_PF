using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class BranchRepo
    {
        #region Methods
       public List<BranchVM> DropDown()
       {
           try
           {
               return new BranchDAL().DropDown();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

        //==================SelectAll=================
        public List<BranchVM> SelectAll()
        {
            try
            {
                return new BranchDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public BranchVM SelectById(int Id)
        {
            try
            {
                return new BranchDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(BranchVM branchVM)
        {
            try
            {
                return new BranchDAL().Insert(branchVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(BranchVM branchVM)
        {
            try
            {
                return new BranchDAL().Update(branchVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public BranchVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new BranchDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(BranchVM branchVM, string[] ids)
        {
            try
            {
                return new BranchDAL().Delete(branchVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        public string[] ImportExcelFile(BranchVM vm)
        {

            try
            {
                BranchDAL dal = new BranchDAL();
                return dal.InsertExportData(vm, null, null);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
