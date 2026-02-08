using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
 public   class GroupRepo
    {
        GroupDAL _dal = new GroupDAL();
        #region Methods
        public List<GroupVM> DropDown(int branch)
        {
            try
            {
                return _dal.DropDown(branch);
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
        public List<GroupVM> SelectAll(int BranchId)
        {
            try
            {
                return new GroupDAL().SelectAll(BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public GroupVM SelectById(string Id)
        {
            try
            {
                return new GroupDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(GroupVM vm)
        {
            try
            {
                return new GroupDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(GroupVM vm)
        {
            try
            {
                return new GroupDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(GroupVM vm, string[] Ids)
        {
            try
            {
                return new GroupDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

}
