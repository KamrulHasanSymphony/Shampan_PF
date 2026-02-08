using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class ExportImportRepo
    {

        //==================SelectAll=================

        #region

        public DataTable SelectEmpInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmpInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectEmployeeInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectPersonalDetail(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectPersonalDetail(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectEmployeeJob(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeJob(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeEducation(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeEducation(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeProfessionalDegree(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeProfessionalDegree(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeLanguage(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeLanguage(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeExtraCurriculumActivities(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeExtraCurriculumActivities(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeImmigration(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeImmigration(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeTraining(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeTraining(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeTravel(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeTravel(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeNominee(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeNominee(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeDependent(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeDependent(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeLeftInformation(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeLeftInformation(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectEmployeeEmergencyContact(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeEmergencyContact(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectDepartment(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectDepartment(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectDesignationGroup(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectDesignationGroup(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectDesignation(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectDesignation(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectAsset(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectAsset(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectBank(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectBank(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectBranch(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectBranch(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectEmployeePF(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeePF(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectEmployeeGroup(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectEmployeeGroup(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #region Import Section
        public string[] InsertEmpInfo(EmployeeInfoVM vm)
        {
            try
            {
                return new ExportImportDAL().InsertEmpInfo(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertEmployeeLeaveStructure(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeLeaveStructure(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertDepartment(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertDepartment(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertDesignation(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertDesignation(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertBank(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertBank(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertDesignationGroup(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertDesignationGroup(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertEmployeePF(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeePF(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertPersonalDetail(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertPersonalDetail(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertEmployeeEmergencyContact(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeEmergencyContact(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertEmployeeJob(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeJob(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertEmployeeEducation(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeEducation(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertEmployeeProfessionalDegree(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeProfessionalDegree(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertEmployeeExtraCurriculumActivities(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeExtraCurriculumActivities(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertEmployeeImmigration(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeImmigration(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertEmployeeTraining(DataTable dt, EmployeeInfoVM vm)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeTraining(dt, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertEmployeeTravels(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeTravels(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertEmployeeNominee(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeNominee(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertEmployeeDependent(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeDependent(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertEmployeeAssets(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeAssets(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertEmployeeLeftInformation(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeLeftInformation(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public string[] InsertEmployeeLeaveStructure(DataTable dt)
        //{
        //    try
        //    {
        //        return new ExportImportDAL().InsertEmployeeLeaveStructure(dt, null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public string[] InsertEmployeeLanguage(DataTable dt)
        {
            try
            {
                return new ExportImportDAL().InsertEmployeeLanguage(dt, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        public string[] InsertPFJournalTemp(GLJournalDetailVM vm)
        {
            try
            {
                return new ExportImportDAL().InsertPFJournalTemp(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetAccName(string AccName)
        {
            try
            {
                return new ExportImportDAL().GetAccName(AccName, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetAccHeader()
        {
            try
            {
                return new ExportImportDAL().GetAccHeader(null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertHeader(GLJournalVM vm)
        {
            try
            {
                return new ExportImportDAL().InsertHeader(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetAccDetailsTemp(int GLJournalId)
        {
            try
            {
                return new ExportImportDAL().GetAccDetailsTemp(GLJournalId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertPFJournalDetails(GLJournalDetailVM vm)
        {
            try
            {
                return new ExportImportDAL().InsertPFJournalDetails(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SelectDepartmentInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectDepartmentInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectAssetInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectAssetInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable SelectDesignationGroupInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectDesignationGroupInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectBranchInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectBranchInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectSectionInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectSectionInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectGradeInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectGradeInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SelectLeaveTypeInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectLeaveTypeInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable SelectBankInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectBankInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SelectProjectInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectProjectInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SelectDesignationInfo(ExportImportVM VM)
        {
            try
            {
                return new ExportImportDAL().SelectDesignationInfo(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
