using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.HRM;
using SymViewModel.Leave;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class EmployeeInfoRepo
    {
        #region Methods

        public List<BranchVM> Branch()
        {
            try
            {
                return new EmployeeDAL().Branch();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> DropDown()
        {
            try
            {
                return new EmployeeDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeInfoVM> DropDown(string currentCode)
        {
            try
            {
                return new EmployeeDAL().DropDown(currentCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeInfoVM> EmployeeBySuppervisor(string EmployeeId, string EmpCode)
        {
            try
            {
                return new EmployeeDAL().EmployeeBySuppervisor(EmployeeId, EmpCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> EmployeeByDepartment(string Department)
        {
            try
            {
                return new EmployeeDAL().EmployeeByDepartment(Department);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> DropDownCodeName(string BranchId)
        {
            try
            {
                return new EmployeeDAL().DropDownCodeName(BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeInfoVM> EmploymentStatus()
        {
            try
            {
                return new EmployeeDAL().EmploymentStatus();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> GetAccountNatureList()
        {
            try
            {
                return new EmployeeDAL().GetAccountNatureList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public List<EmployeeInfoVM> DropDownCodeNameAll(string BranchId)
        {
            try
            {
                return new EmployeeDAL().DropDownCodeNameAll(BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeInfoVM> DropDownCodeNameConfirmed()
        {
            try
            {
                return new EmployeeDAL().DropDownCodeNameConfirmed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public List<EmployeeInfoVM> DropDownExCodeName()
        {
            try
            {
                return new EmployeeDAL().DropDownExCodeName();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> DropDownAllEmployee(string[] conFields = null, string[] conValues = null)
        {
            try
            {
                return new EmployeeDAL().DropDownAllEmployee(conFields, conValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //==================ViewSelectAll=================
        public List<ViewEmployeeInfoVM> ViewSelectAllEmployee(string Code = null, string Id = null, string BranchId = null, string ProjectId = null, string DepartmentId = null, string SectionId = null,
               string DesignationId = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string EmployeeId = "")
        {
            try
            {
                return new EmployeeInfoDAL().ViewSelectAllEmployee(Code, Id, null, ProjectId, DepartmentId, SectionId, DesignationId, null, null, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
         
        //==================SelectAll=================
        public List<EmployeeInfoVM> SelectAll(string BranchId, string DOJFrom = "", string DOJTo = "")
        {
            try
            {
                return new EmployeeInfoDAL().SelectAll(BranchId, DOJFrom, DOJTo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SelectEmpByCode(string Code)
        {
            try
            {
                return new EmployeeInfoDAL().SelectEmpByCode(Code);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAll=================
        public List<EmployeeInfoVM> SelectAllActiveEmp(string branchId, string DOJFrom = "", string DOJTo = "")
        {
            try
            {
                return new EmployeeInfoDAL().SelectAllActiveEmp(branchId, DOJFrom, DOJTo);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public EmployeeInfoVM SelectEmpForSearch(string empcode, string btn)
        {
            try
            {

                return new EmployeeInfoDAL().SelectEmpForSearch(empcode, btn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeInfoVM SelectEmpForSearchAll(string empcode, string btn)
        {
            try
            {

                return new EmployeeInfoDAL().SelectEmpForSearchAll(empcode, btn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeInfoVM SelectEmpStructure(string empcode, string btn)
        {
            try
            {
                return new EmployeeInfoDAL().SelectEmpStructure(empcode, btn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeInfoVM SelectExEmpStructure(string empcode, string btn)
        {
            try
            {
                return new EmployeeInfoDAL().SelectExEmpStructure(empcode, btn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public EmployeeInfoVM SelectEmpStructureAll(string empcode, string btn)
        {
            try
            {
                return new EmployeeInfoDAL().SelectEmpStructureAll(empcode, btn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public EmployeeInfoVM SelectById(string Id)
        {
            try
            {
                return new EmployeeInfoDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeInfoVM SelectLeaveScheduleById(string Id)
        {
            try
            {
                return new EmployeeInfoDAL().SelectLeaveScheduleById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public EmployeeInfoVM AllSelectById(string Id)
        {
            try
            {
                return new EmployeeInfoDAL().AllSelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllUser=================

        //==================SelectByID=================
        public EmployeeInfoVM SelectByIdAll(string Id)
        {
            try
            {
                return new EmployeeInfoDAL().SelectByIdAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeVM EmployeeInfo(string Id)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeInfo(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeInfoVM empEducationVM)
        {
            try
            {
                return new EmployeeInfoDAL().Insert(empEducationVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeInfoVM educationVM)
        {
            try
            {
                //throw new ArgumentNullException("EmployeeInfo Update", "Could not found any item.");
                return new EmployeeInfoDAL().Update(educationVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeInfoVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeInfoDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeInfoVM EmployeeInfoVM, string[] ids)
        {
            try
            {
                return new EmployeeInfoDAL().Delete(EmployeeInfoVM, ids, null, null);
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
                return new EmployeeInfoDAL().Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> AutocompleteCode(string term)
        {
            try
            {
                return new EmployeeInfoDAL().AutocompleteCode(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> AutocompleteCodeAll(string term)
        {
            try
            {
                return new EmployeeInfoDAL().AutocompleteCodeAll(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> AutocompleteMarge(string term)
        {
            try
            {
                return new EmployeeInfoDAL().AutocompleteMarge(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string EmployeeExist(string term)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeExist(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeNameByCode(string empCode)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeNameByCode(empCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int NextOtherId(string EmpCategory, string EmpJobType)
        {
            try
            {
                return new CommonDAL().NextOtherId(EmpCategory, EmpJobType, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        //===========================Reports==================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CodeF"></param>
        /// <param name="CodeT"></param>
        /// <param name="Name"></param>
        /// <param name="Department"></param>
        /// <param name="dtpFrom"></param>
        /// <param name="dtpTo"></param>
        /// <param name="leaveyear"></param>
        /// <param name="LeaveType"></param>
        /// <param name="Project"></param>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public List<EmployeeLeaveStatementVM> EmployeeLeaveStatement(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType, string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeLeaveStatement(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeLeaveBalanceVM> EmployeeLeaveStatementNew(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
        , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType, string EmployeeId, string LId = "0")
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeLeaveStatementNew(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId, LId);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeLeaveVM> EmployeeLeaveRegister(string CodeF, string CodeT, string DepartmentId, string SectionId
           , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType
           , string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeLeaveRegister(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeLeaveBalanceVM> EmployeeLeaveList(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType
            , string EmployeeId, string Gender_E, string Religion, string GradeId)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeLeaveList(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId, Gender_E, Religion, GradeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AttLogsVM> EA(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
                , string DesignationId, string dtpFrom, string dtpTo, string dtAtnFrom, string dtAtnTo, string PunchMissIn, string PunchMissOut, string ReportNo, string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().EA(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, dtAtnFrom, dtAtnTo, PunchMissIn, PunchMissOut, ReportNo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> EmployeeProfilesFull(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
               , string DesignationId, string dtpFrom, string dtpTo, string BloodGroup, string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeProfilesFull(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, BloodGroup, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<EmployeeInfoVM> EmployeeList(string CodeF = null, string CodeT = null, string DepartmentId = null, string SectionId = null, string ProjectId = null
                , string DesignationId = null, string dtpFrom = null, string dtpTo = null, string BloodGroup = null, string EmployeeId = null, string Gender_E = null
            , string Religion = null, string GradeId = null
            , string other1 = "", string other2 = "", string other3 = "", string OrderBy = "", string EmpJobType = null, string EmpCategory = null
            )
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeList(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, BloodGroup, EmployeeId, Gender_E, Religion, GradeId, other1, other2, other3, OrderBy, EmpJobType, EmpCategory);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ViewEmployeeInfoAllVM> EmployeeInformationAll(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
                , string DesignationId, string dtpFrom, string dtpTo, string Gender, string Religion, string GradeId, string OrderByCode = "")
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeInformationAll(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, Gender, Religion, GradeId, OrderByCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public DataTable ExportExcelFile(string Filepath, string FileName, string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
         , string DesignationId, string dtpFrom, string dtpTo, string Gender, string Religion, string GradeId, string OrderByCode = "")
        {
            return new EmployeeInfoDAL().ExportExcelEmpInforAll(Filepath, FileName, CodeF, CodeT, DepartmentId, SectionId, ProjectId
           , DesignationId, dtpFrom, dtpTo, Gender, Religion, GradeId, OrderByCode);
        }


        public DataTable ExportExcelFileemplist(string Filepath, string FileName, string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
         , string DesignationId, string dtpFrom, string dtpTo, string Gender, string Religion, string GradeId, string OrderByCode = "")
        {
            return new EmployeeInfoDAL().ExportExcelEmpInforAll(Filepath, FileName, CodeF, CodeT, DepartmentId, SectionId, ProjectId
           , DesignationId, dtpFrom, dtpTo, Gender, Religion, GradeId, OrderByCode);
        }


        public List<EmployeeInfoVM> ExEmployeeList(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
                , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().ExEmployeeList(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> UnConfirmedList(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
              , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().UnConfirmedList(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> EmpServiceLength(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
                , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId, string Gender, string Religion, string GradeId)
        {
            try
            {
                return new EmployeeInfoDAL().EmpServiceLength(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, EmployeeId, Gender, Religion, GradeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> EmpTransfer(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
                , string DesignationId, string dtjFrom, string dtjTo, string EmployeeId, string dttFrom, string dttTo)
        {
            try
            {
                return new EmployeeInfoDAL().EmpTransfer(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtjFrom, dtjTo, EmployeeId, dttFrom, dttTo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> EmpTransferLetter(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
        , string DesignationId, string dtjFrom, string dtjTo, string EmployeeId, string dttFrom, string dttTo, string LetterName = null)
        {
            try
            {
                return new EmployeeInfoDAL().EmpTransferLetter(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtjFrom, dtjTo, EmployeeId, dttFrom, dttTo, LetterName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> EmpTraining(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
                , string DesignationId, string dtpFrom, string dtpTo, string topics, string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().EmpTraining(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, topics, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> EmpPromotion(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
                , string DesignationId, string dtjFrom, string dtjTo, string EmployeeId, string dtpFrom, string dtpTo, string PL = null)
        {
            try
            {
                return new EmployeeInfoDAL().EmpPromotion(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtjFrom, dtjTo, EmployeeId, dtpFrom, dtpTo, PL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] UpdatePhoto(string EmployeeId, string PhotoName)
        {
            try
            {
                return new EmployeeInfoDAL().UpdatePhoto(EmployeeId, PhotoName, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //===========================Letters==================================

        public DataTable EmployeeListLetter(EmployeeInfoVM vm, string[] conditionFields = null, string[] conditionValues = null, string OrderByCode = "")
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeListLetter(vm, conditionFields, conditionValues, OrderByCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable EmployeePromotionValue(EmployeeInfoVM vm)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeePromotionValue(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable EmployeeIncrementValue(EmployeeInfoVM vm)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeIncrementValue(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        public DataTable EmployeeListTravelLetter(EmployeeInfoVM vm, string[] conditionFields = null, string[] conditionValues = null, string OrderByCode = "")
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeListTravelLetter(vm, conditionFields, conditionValues, OrderByCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public DataTable EmployeeNewReport(EmployeeInfoVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeNewReport(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable EmployeeSummaryReport(EmployeeInfoVM vm)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeSummaryReport(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable CombinedEmployeeSummaryReport(EmployeeInfoVM vm)
        {
            try
            {
                return new EmployeeInfoDAL().CombinedEmployeeSummaryReport(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable EmployeeSummaryLeaveRegisterReport(string CodeF, string CodeT, string DepartmentId, string SectionId
           , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType
           , string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeSummaryLeaveRegisterReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> SelectActiveEmp(string DOJFrom = "", string DOJTo = "")
        {
            try
            {
                return new EmployeeInfoDAL().SelectActiveEmp(DOJFrom, DOJTo);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public DataSet EmployeeLeaveRegisterShampan(string CodeF, string CodeT, string DepartmentId, string SectionId
        , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType
        , string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeLeaveRegisterShampan(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataSet EmployeeLeaveEncashmentG4S(string CodeF, string CodeT, string DepartmentId, string SectionId
         , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType
         , string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeLeaveEncashmentG4S(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet EmployeeLeaveRegisterG4S(string CodeF, string CodeT, string DepartmentId, string SectionId
         , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType
         , string EmployeeId)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeLeaveRegisterG4S(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable EmployeeLeaveRegisterDownload(string CodeF, string CodeT, string DepartmentId, string SectionId
        , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType
        , string EmployeeId, string EmpCategory, string EmpJobType)
        {
            try
            {
                return new EmployeeInfoDAL().EmployeeLeaveRegisterDownload(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId, EmpCategory, EmpJobType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public DataTable ExportExcelFile(EmployeeInfoVM vm)
        //{
        //    try
        //    {
        //        return new EmployeeInfoDAL().EmployeeNomineeDownload(vm);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public string[] InsertApplicant(string[] data)
        {
            try
            {
                EmployeeInfoVM emp = new EmployeeInfoVM();
                emp.MiddleName = data[4];
                emp.CreatedBy = data[1];
                emp.CreatedAt = data[2];
                emp.CreatedFrom = data[3];
                emp.BranchId = 1;

                return new EmployeeInfoDAL().Insert(emp, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
    }
}
