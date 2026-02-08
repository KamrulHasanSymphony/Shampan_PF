using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;


namespace SymRepository.Common
{
    public class UserGroupRepo
    {
        UserGroupDAL _dal = new UserGroupDAL();
        #region Methods
        public List<UserGroupVM> DropDown()
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
        public List<UserGroupVM> SelectAll()
        {
            try
            {
                return new UserGroupDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public UserGroupVM SelectById(string Id)
        {
            try
            {
                return new UserGroupDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(UserGroupVM vm)
        {
            try
            {
                return new UserGroupDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(UserGroupVM vm)
        {
            try
            {
                return new UserGroupDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(UserGroupVM vm, string[] ids)
        {
            try
            {
                return new UserGroupDAL().Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
