using SymServices.WPPF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.WPPF
{
    public class PFDetailRepo
    {
       
        public List<PFDetailVM> SelectEmployeeList(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().SelectEmployeeList(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PFDetailVM> SelectFiscalPeriod(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().SelectFiscalPeriod(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PFHeaderVM> SelectFiscalPeriodHeader(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().SelectFiscalPeriodHeader(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<PFDetailVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] PFProcess(string FiscalYearDetailId, string ProjectId ,string chkAll, ShampanIdentityVM auditvm)
        {
            try
            {
                return new PFDetailDAL().PFProcess(FiscalYearDetailId, ProjectId, chkAll, auditvm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(PFDetailVM vm)
        {
            try
            {
                return new PFDetailDAL().Post(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] PostHeader(PFHeaderVM vm)
        {
            try
            {
                return new PFDetailDAL().PostHeader(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(PFDetailVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable PFEmployersProvisionsReport(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().PFEmployersProvisionsReport(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExportExcelFilePF(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, string Orderby = null, string BranchId = null)
        {
            return new PFDetailDAL().ExportExcelFilePF(Filepath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, Orderby, BranchId);
        }

        public string[] ImportExcelFile(string fullPath, string fileName, ShampanIdentityVM vm, int FYDId = 0, string PId="")
        {
            try
            {
                return new PFDetailDAL().ImportExcelFile(fullPath, fileName, vm, null, null, FYDId, PId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertAutoJournal(string TransactionMonth, string TransactionForm, string TransactionCode, string BranchId, ShampanIdentityVM vm)
        {
            try
            {
                return new PFDetailDAL().AutoJournalSave(TransactionMonth, TransactionForm, TransactionCode, BranchId, null, null, vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
