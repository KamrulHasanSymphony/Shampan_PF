using SymRepository.Common;
using SymRepository.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SymRepository.Payroll;
using SymOrdinary;
using System.Threading;
using SymRepository.PF;
using SymServices.Enum;

namespace SymWebUI.Areas.Config.Controllers
{
    public class DropDownController : Controller
    {

        
        #region PF - Module
        //public JsonResult PF_WithdrawDebitHead(string WithdrawTypeId = "0")
        //{
        //    return Json(new SelectList(new AccountRepo().DropDown_Account_WithdrawDebitHead(WithdrawTypeId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult PF_Account(string AccountType = "")
        //{
        //    return Json(new SelectList(new AccountRepo().DropDown(AccountType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult PF_JournalFor()
        {
            return Json(new SelectList(new AutoJournalSetupRepo().JournalForDropDown(), "Id", "JournalFor"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoanInterestPolicyList()
        {
            var data = new EnumLoanTypeRepo().GetLoanInterestPolicyList();
            return Json(new SelectList(data, "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult LeftType()
        {
            return Json(new SelectList(new EnumLeftTypeRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoanType()
        {
            return Json(new SelectList(new EnumLoanTypeRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_COA(string TransType = "PF")
        {
            string BranchId = Session["BranchId"].ToString();
            return Json(new SelectList(new COARepo().DropDown(TransType, BranchId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PF_COAGroup(string TransType = "PF")
        {
            return Json(new SelectList(new COAGroupRepo().DropDown(TransType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_COASubGroup()
        {
            return Json(new SelectList(new COASubGroupRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_COACategory()
        {
            return Json(new SelectList(new COACategoryRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_COATypeOfReport(string TransType = "PF")
        {
            return Json(new SelectList(new COAGroupRepo().COATypeOfReportDropDown(TransType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_COAGroupType(string TransType = "PF")
        {
            return Json(new SelectList(new COAGroupRepo().COAGroupTypeDropDown(TransType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_JournalTypeDropDown()
        {
            return Json(new SelectList(new EnumJournalTypeRepo().JournalTypeDropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult PF_EnumJournalEntryType()

        public JsonResult PF_JournalTransactionTypeDropDown()
        {
            return Json(new SelectList(new EnumJournalTypeRepo().JournalTransactionTypeDropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult PF_COAType()
        {
            return Json(new SelectList(new COARepo().COATypeDropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PF_EnumJournalEntryType()
        {
            return Json(new SelectList(new EnumJournalTypeRepo().JournalTransactionTypeDropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PF_InvestmentName(string TransType = "PF")
        {
            string branchId = Session["BranchId"].ToString();
            return Json(new SelectList(new InvestmentNameRepo().DropDown(TransType, branchId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_TransactionMedia()
        {
            return Json(new SelectList(new TransactionMediaRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_TransactionMediaName()
        {
            return Json(new SelectList(new TransactionMediaRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }


        //public JsonResult PF_WithdrawType()
        //{
        //    return Json(new SelectList(new WithdrawTypeRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}


        //public JsonResult PF_Investment(string InvestmentTypeId)
        //{
        //    return Json(new SelectList(new InvestmentRepo().DropDown(InvestmentTypeId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult PF_EnumInvestmentType()
        {
            return Json(new SelectList(new EnumInvestmentTypeRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_BankBranchName(string TransType = "PF")
        {
            string branchId = Session["BranchId"].ToString();
            return Json(new SelectList(new BankBranchRepo().DropDown(TransType, branchId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_BankName(string TransType = "PF")
        {
            return Json(new SelectList(new BankNameRepo().DropDown(TransType, Session["BranchId"].ToString()), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult PF_COA()
        //{
        //    return Json(new SelectList(new SymRepository.PF.COARepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}

        #endregion PF

        #region HRM - Payroll

             
        public JsonResult DropDownYear()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownYear(Convert.ToInt32(identity.BranchId)), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownReportType()
        {
            // Define static report types
            List<SelectListItem> staticReportTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "BS", Text = "Balance Sheet" },
                new SelectListItem { Value = "IS", Text = "Income Statement" },
                new SelectListItem { Value = " ", Text = "Trial Balance" }
                // Add more items as needed
            };

            // Create a SelectList from the static list
            SelectList reportTypes = new SelectList(staticReportTypes, "Value", "Text");

            // Return the SelectList as JSON result with support for HTTP GET requests
            return Json(reportTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Branch()
        {
            return Json(new SelectList(new EmployeeInfoRepo().Branch(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult DropDownPeriodByYear()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriodByYear(Convert.ToInt32(identity.BranchId), DateTime.Now.Year), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriod()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriod(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriod(int FiscalYearId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriod(Convert.ToInt32(identity.BranchId), FiscalYearId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriodByFYear(int year)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriodByYear(Convert.ToInt32(identity.BranchId), year), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriodByYearLockPayroll(int year)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriodByYearLockPayroll(Convert.ToInt32(identity.BranchId), year), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriodByYearLockPayroll_All(int year)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriodByYearLockPayroll_All(Convert.ToInt32(identity.BranchId), year), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriodNext(int currentId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriodNext(Convert.ToInt32(identity.BranchId), currentId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FiscalPeriod()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriod(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
      
        public JsonResult GroupList()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new GroupRepo().DropDown(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
      
        public JsonResult PFStructureList()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new PFStructureRepo().DropDown(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
       
      
        public JsonResult EmployeeCode()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDown(), "Code", "Code"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MultiEmployeeCodeName()
        {
            return Json(new MultiSelectList(new EmployeeInfoRepo().DropDownCodeName(Session["BranchId"].ToString()), "Code", "EmpName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmployeeCodeName()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeName(Session["BranchId"].ToString()), "Code", "EmpName"), JsonRequestBehavior.AllowGet);

        }
        public JsonResult EmployeeCodeNameAll()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeNameAll(Session["BranchId"].ToString()), "Code", "EmpName"), JsonRequestBehavior.AllowGet);

        }
        public JsonResult EmployeeAll()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeNameAll(Session["BranchId"].ToString()), "Code", "EmpName"), JsonRequestBehavior.AllowGet);

        }
        public JsonResult EmployeeCodeNameId()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeName(Session["BranchId"].ToString()), "Id", "EmpName"), JsonRequestBehavior.AllowGet);

        }
        public JsonResult EmployeeIdName()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeName(Session["BranchId"].ToString()), "Id", "EmpName"), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult EmployeeCodeNameConfirmed()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeNameConfirmed(), "Code", "EmpName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExEmployeeCodeName()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownExCodeName(), "Code", "EmpName"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeCodeNext(string currentCode)
        {
            string vcurrentCode = "0_0";

            if (currentCode != "0_0" && currentCode != "0" && currentCode != "" && currentCode != "null" && currentCode != null)
                vcurrentCode = currentCode;
            return Json(new SelectList(new EmployeeInfoRepo().DropDown(vcurrentCode), "Code", "EmpName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DropDownAllEmployee()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownAllEmployee(), "Id", "EmpInfo"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownEmployeeId()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeName(Session["BranchId"].ToString()), "Id", "EmpName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmployeeBySuppervisor()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string EmployeeId = identity.EmployeeId;
            string EmpCode = identity.EmployeeCode;
            return Json(new SelectList(new EmployeeInfoRepo().EmployeeBySuppervisor(EmployeeId, EmpCode), "Code", "EmpName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmployeeByDepartment(string departmentId)
        {
            return Json(new SelectList(new EmployeeInfoRepo().EmployeeByDepartment(departmentId), "Code", "EmpName"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Grade()
        {
            return Json(new SelectList(new GradeRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult DesignationGroup()
        {
            return Json(new SelectList(new DesignationRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Company()
        {
            return Json(new SelectList(new SymRepository.Common.CompanyRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DepartmentName()
        {
            return Json(new SelectList(new DepartmentRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Department()
        {
            return Json(new SelectList(new DepartmentRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AppraisalCategory()
        {
            return Json(new SelectList(new DepartmentRepo().AppraisalCategory(), "Id", "CategoryName"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AppraisalAssignTo()
        {
            return Json(new SelectList(new DepartmentRepo().AppraisalAssignTo(), "Id", "AssignToName"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AppraisalQuestion()
        {
            return Json(new SelectList(new DepartmentRepo().AppraisalQuestion(), "Id", "Question"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EvaluationFor()
        {
            return Json(new SelectList(new DepartmentRepo().EvaluationFor(), "Id", "EvaluationFor"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AppraisalQuestionSet()
        {
            return Json(new SelectList(new DepartmentRepo().AppraisalQuestionSet(), "Id", "QuestionSetName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AssignTo()
        {
            return Json(new SelectList(new DepartmentRepo().AssignTo(), "Id", "AssignToName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Project()
        {
            return Json(new SelectList(new ProjectRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProjectByDepartment(string departmentId, string sectionId)
        {
            string vDepartmentId = "0_0";
            string vSectionId = "0_0";

            if (departmentId != "0_0" && departmentId != "0" && departmentId != "" && departmentId != "null" && departmentId != null)
                vDepartmentId = departmentId;


            if (sectionId != "0_0" && sectionId != "0" && sectionId != "" && sectionId != "null" && sectionId != null)
                vSectionId = sectionId;
            return Json(new SelectList(new ProjectRepo().DropDownByDepartment(vDepartmentId, vSectionId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ProjectByDepartmentId(string departmentId)
        {
            string vDepartmentId = "0_0";
            if (departmentId != "0_0" && departmentId != "0" && departmentId != "" && departmentId != "null" && departmentId != null)
                vDepartmentId = departmentId;
            return Json(new SelectList(new ProjectRepo().DropDownByDepartment(vDepartmentId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DerparmentByProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(new SelectList(new DepartmentRepo().DropDownByProject(projectId), "value", "text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Section()
        {
            return Json(new SelectList(new SectionRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SectionByDepartment(string departmentId)
        {
            string vDepartmentId = "0_0";
            if (departmentId != "0_0" && departmentId != "0" && departmentId != "" && departmentId != "null" && departmentId != null)
                vDepartmentId = departmentId;

            return Json(new SelectList(new SectionRepo().DropDownByDepartment(vDepartmentId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DesignationName()
        {
            return Json(new SelectList(new DesignationRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Designation()
        {
            return Json(new SelectList(new DesignationRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
             
        public ActionResult Index()
        {
            return View();
        }
           
        public JsonResult Gender()
        {
            return Json(new SelectList(new EnumGenderRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult Year()
        {
            return Json(new SelectList(new EnumYearRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Bank()
        {
            return Json(new SelectList(new BankRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult UserGroup()
        {
            return Json(new SelectList(new SymRepository.Common.UserGroupRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


     
        public JsonResult ModuleDropDown()
        {
            return Json(new SelectList(new SymRepository.Common.SymUserRoleRepo().DropDownModule(), "symArea", "symArea"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MenuDropDown()
        {
            return Json(new SelectList(new SymRepository.Common.SymUserRoleRepo().DropDownMenu(), "symController", "symController"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DesignationGroupDropDown()
        {
            return Json(new SelectList(new SymRepository.Common.DesignationRepo().DesignationGroupDropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult FileDelete(string filepath, int table, string field, string id)
        {
            string tableName = "";
            if (table == 1)
            {
                tableName = "EmployeePersonalDetail";
            }
            else if (table == 2)
            {
                tableName = "EmployeeEmergencyContact";
            }
            else if (table == 3)
            {
                tableName = "EmployeeJobHistory";
            }
            else if (table == 4)
            {
                tableName = "EmployeeLeftInformation";
            }
            else if (table == 5)
            {
                tableName = "EmployeePromotion";
            }
            else if (table == 6)
            {
                tableName = "EmployeeTransfer";

            }
            else if (table == 7)
            {
                tableName = "EmployeeEducation";

            }
            else if (table == 8)
            {
                tableName = "EmployeeExtraCurriculumActivities";

            }
            else if (table == 9)
            {
                tableName = "EmployeeLanguage";

            }
            else if (table == 10)
            {
                tableName = "EmployeeImmigration";

            }
            else if (table == 11)
            {
                tableName = "EmployeeTraining";

            }
            else if (table == 12)
            {
                tableName = "EmployeeTravel";

            }
            else if (table == 13)
            {
                tableName = "EmployeeDependent";

            }
            else if (table == 14)
            {
                tableName = "EmployeeNominee";

            }
            else if (table == 15)
            {
                tableName = "EmployeeFile";

            }
            else if (table == 501)
            {
                tableName = "Clients";
            }
            else if (table == 601)
            {
                tableName = "GLUsers";
            }

            bool returnval = new CommonRepo().FileDelete(tableName, field, id);
            if (returnval)
            {
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "" + filepath;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            return Json(returnval, JsonRequestBehavior.AllowGet);
        }


        public JsonResult EnumOrderBy(string Module)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new EnumOderByRepo().DropDown(Module), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        //---------- User Dropdown---------
        public JsonResult User()
        {
            return Json(new SelectList(new UserInformationRepo().DropDown(), "Id", "FullName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUser(string deptId)
        {
            return Json(new SelectList(new UserInformationRepo().GetUser(deptId), "Id", "FullName"), JsonRequestBehavior.AllowGet);
        }
            
        //====== User By Department ===

        public JsonResult UserByDepartmentId(string departmentId)
        {
            string vDepartmentId = "0_0";
            if (departmentId != "0_0" && departmentId != "0" && departmentId != "" && departmentId != "null" && departmentId != null)
                vDepartmentId = departmentId;
            return Json(new SelectList(new ProjectRepo().DropDownByDepartment(vDepartmentId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReqEmploymentStatus()
        {
            return Json(new SelectList(new EmployeeInfoRepo().EmploymentStatus(), "Id", "EmpName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AccountNature()
        {
            return Json(new SelectList(new EmployeeInfoRepo().GetAccountNatureList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CountryEN(bool? isContact = null)
        {
            return Json(new SelectList(new EnumCountryRepo().DropDown(isContact), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Division(string country)
        {
            return Json(new SelectList(new EmployeeInfoForPFRepo().DivisionDropDown(country), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult District(string division)
        {
            return Json(new SelectList(new EmployeeInfoForPFRepo().DistrictDropDown(division), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
    }
}
