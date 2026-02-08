using SymServices.Common;
using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class GradeRepo
    {
        GradeDAL _dal = new GradeDAL();
       #region Methods
        public List<GradeVM> DropDown()
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
        public List<GradeVM> SelectAll()
        {
            try
            {
                return new GradeDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public GradeVM SelectById(string Id)
        {
            try
            {
                return new GradeDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(GradeVM vm)
        {
            try
            {
                return new GradeDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(GradeVM vm)
        {
            try
            {
                return new GradeDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(GradeVM vm,string[] Ids)
        {
            try
            {
                return new GradeDAL().Delete(vm,Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
      
    }
}
