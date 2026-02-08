using SymServices.Common;
using SymServices.PF;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.PF
{
  public  class EmployeeLeftInformationRepo
    {
        #region Methods
        //==================SelectAll=================
      public List<EmployeeLeftInformationVM> SelectAll(string branchId, int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeLeftInformationDAL().SelectAll(branchId, Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeLeftInformationVM employeeLeftInformationVM)
        {
            try
            {
                return new EmployeeLeftInformationDAL().Insert(employeeLeftInformationVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeLeftInformationVM employeeLeftInformationVM)
        {
            try
            {
                return new EmployeeLeftInformationDAL().Update(employeeLeftInformationVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeLeftInformationVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeLeftInformationDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeLeftInformationVM vm, string[] ids)
        {
            try
            {
                return new EmployeeLeftInformationDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeeLeftInformationVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeLeftInformationDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        #endregion
    }
}
