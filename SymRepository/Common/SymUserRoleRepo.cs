using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class SymUserRoleRepo
   {
       #region Methods
       //==================DropDownModule=================
       public List<SymUserRollVM> DropDownModule()
        {
            try
            {
                return new SymUserRollDAL().DropDownsymArea();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       //==================DropDownMenu=================
        public List<SymUserRollVM> DropDownMenu()
        {
            try
            {
                return new SymUserRollDAL().DropDownsymController();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAll=================
        public List<SymUserRollVM> SelectSymUserRollAll()
        {
            try
            {
                return new SymUserRollDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SymUserRollVM> SelectAllSymUserRollByGroupId(string GroupId, string SymArea = null)
        {
            try
            {
                return new SymUserRollDAL().SelectAllByGroupId(GroupId, SymArea);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAll=================
        public List<UserLogsVM> SelectAllUser()
        {
            try
            {
                return new SymUserRollDAL().SelectAllUser();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<SymUserDefaultRollVM> SelectSymUserById(string Id)
        {
            try
            {
                return new SymUserRollDAL().SelectSymUserById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserLogsVM SelectUserById(string Id)
        {
            try
            {
                return new SymUserRollDAL().SelectGroupId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public SymUserRollVM SelectById(string Id)
        {
            try
            {
                return new SymUserRollDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================InsertWithSelectAll =================
         public string[] SelectAllSymRollwithInsert(SymUserRollVM vm)
        {
            try
            {
                return new SymUserRollDAL().SelectAllSymRollwithInsert(vm,null,null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(SymUserRollVM SymUserRollVM)
        {
            try
            {
                return new SymUserRollDAL().Insert(SymUserRollVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(UserGroupVM VM)
        {
            try
            {
                return new SymUserRollDAL().Update(VM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(SymUserRollVM SymUserRollVM, string[] ids)
        {
            try
            {
                return new SymUserRollDAL().Delete(SymUserRollVM, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SymUserDefaultRollVM> UserRollDetail(string empId, string Module)
        {
            try
            {
                return new SymUserRollDAL().UserRollDetail(empId, Module);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool SymRollSessionBackup(string UserId, string symArea, string symController, string IndexAddEditDeleteProcessReport)
        {
            try
            {
                return new SymUserRollDAL().SymRollSessionBackup(UserId, symArea, symController, IndexAddEditDeleteProcessReport);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool  SymRoleSession(string UserId, string DefaultRollId, string IndexAddEditDeleteProcessReport)
        {
            try
            {
                return new SymUserRollDAL().SymRoleSession(UserId, DefaultRollId, IndexAddEditDeleteProcessReport);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable RollByUserId(string userId)
        {
            try
            {
                return new SymUserRollDAL().RollByGroupId( userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        
   }
}
