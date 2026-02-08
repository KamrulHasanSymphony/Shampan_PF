using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class ProjectRepo
    {
        ProjectDAL _dal = new ProjectDAL();
       #region Methods
       public List<ProjectVM> DropDown()
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
       public List<DropDownVM> DropDownByDepartment(string departmentId, string sectionId)
       {
           try
           {
               return _dal.DropDownByDepartment(departmentId, sectionId);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       public List<DropDownVM> DropDownByDepartment(string departmentId)
       {
           try
           {
               return _dal.DropDownByDepartment(departmentId);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================SelectAll=================
        public List<ProjectVM> SelectAll()
        {
            try
            {
                return new ProjectDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public ProjectVM SelectById(string Id)
        {
            try
            {
                return new ProjectDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(ProjectVM vm)
        {
            try
            {
                return new ProjectDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(ProjectVM vm)
        {
            try
            {
                return new ProjectDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public ProjectVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new ProjectDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(ProjectVM vm, string[] ids)
        {
            try
            {
                return new ProjectDAL().Delete(vm,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public string[] ImportExcelFile(ProjectVM vm)
        {
            try
            {
                ProjectDAL dal = new ProjectDAL();
                return dal.InsertExportData(vm, null, null);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
