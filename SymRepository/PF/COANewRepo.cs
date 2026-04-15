using SymServices.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.PF
{
   public class COANewRepo
    {
        #region DropDown
        public List<COANewVM> DropDown(string TransType = "PF", string BranchId = "")
        {
            try { return new COANewDAL().DropDown(TransType, BranchId); }
            catch (Exception ex) { throw ex; }
        }

        public List<COANewVM> COATypeDropDown()
        {
            try { return new COANewDAL().COATypeDropDown(); }
            catch (Exception ex) { throw ex; }
        }

        // For ParentCOA selection dropdown (excludes self when editing)
        public List<COANewVM> ParentCOADropDown(string TransType = "PF", string BranchId = "", int excludeId = 0)
        {
            try { return new COANewDAL().ParentCOADropDown(TransType, BranchId, excludeId); }
            catch (Exception ex) { throw ex; }
        }
        #endregion

        #region SelectAll
        public List<COANewVM> SelectAll(string branchId, int Id = 0,
            string[] conditionFields = null, string[] conditionValues = null)
        {
            try { return new COANewDAL().SelectAll(branchId, Id, conditionFields, conditionValues); }
            catch (Exception ex) { throw ex; }
        }
        #endregion

        #region Tree
        // Returns flat list
        public List<COATreeVM> SelectAllForTree(string branchId, string transType = "PF")
        {
            try { return new COANewDAL().SelectAllForTree(branchId, transType); }
            catch (Exception ex) { throw ex; }
        }

        // Returns nested root nodes with Children populated
        public List<COATreeVM> SelectAllForTreeNested(string branchId, string transType = "PF")
        {
            try
            {
                var dal = new COANewDAL();
                var flatList = dal.SelectAllForTree(branchId, transType);
                return dal.BuildTree(flatList);
            }
            catch (Exception ex) { throw ex; }
        }
        #endregion

        #region CRUD
        public string[] Insert(COANewVM vm)
        {
            try { return new COANewDAL().Insert(vm); }
            catch (Exception ex) { throw ex; }
        }

        public string[] Update(COANewVM vm)
        {
            try { return new COANewDAL().Update(vm); }
            catch (Exception ex) { throw ex; }
        }

        public string[] Delete(COANewVM vm, string[] ids)
        {
            try { return new COANewDAL().Delete(vm, ids); }
            catch (Exception ex) { throw ex; }
        }
        #endregion
    }
}
