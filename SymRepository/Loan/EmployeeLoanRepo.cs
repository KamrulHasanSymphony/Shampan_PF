using SymServices.Loan;
using SymViewModel.Common;
using SymViewModel.Loan;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Loan
{
    public class EmployeeLoanRepo
    {
        public List<EmployeeLoanVM> SelectAll(int BranchId, string employee = null)
        {
            try
            {
                return new EmployeeLoanDAL().SelectAll(BranchId, employee);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================Get All Distinct FiscalPeriodName =================
        public EmployeeLoanVM SelectLoan(string loanID)
        {
            try
            {
                return new EmployeeLoanDAL().SelectLoan(loanID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public EmployeeLoanVM SelectLoanForSettelment(string loanID)
        {
            try
            {
                return new EmployeeLoanDAL().SelectLoanForSettelment(loanID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public EmployeeLoanDetailVM SelectEmployeeLoan(string Id, string emploanId)
        {
            try
            {
                return new EmployeeLoanDAL().SelectEmployeeLoan(Id, emploanId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] EmployeeLoanInsert(EmployeeLoanVM vm)
        {
            try
            {
                return new EmployeeLoanDAL().EmployeeLoanInsert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EmployeeLoanUpdate(EmployeeLoanVM vm)
        {
            try
            {
                return new EmployeeLoanDAL().EmployeeLoanUpdate(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EmployeeLoanUpdate2(EmployeeLoanDetailVM vm)
        {
            try
            {
                return new EmployeeLoanDAL().EmployeeLoanUpdate2(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EmployeeLoanHold(EmployeeLoanVM vm, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeLoanDAL().EmployeeLoanHold(vm, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] PaidEdit(EmployeeLoanDetailVM vm)
        {
            try
            {
                return new EmployeeLoanDAL().PaidEdit(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeLoanDetailHold(EmployeeLoanDetailVM dVM, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeLoanDAL().EmployeeLoanDetailHold(dVM, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EmployeeLoanDetailPayment(EmployeeLoanDetailVM dVM, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeLoanDAL().EmployeeLoanDetailPayment(dVM, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeLoanDetailVM> SelectAllForSLMPeport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string FromDate, string ToDate, string BranchId)
        {
            try
            {
                return new EmployeeLoanDAL().SelectAllForSLMPeport(ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, FromDate, ToDate, BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeLoanDetailVM> SelectAllForReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string BranchId)
        {
            try
            {
                return new EmployeeLoanDAL().SelectAllForReport(ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeLoanDetailVM> SelectLoanStatementForReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT)
        {
            try
            {
                return new EmployeeLoanDAL().SelectLoanStatementForReport(ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public string[] Delete(EmployeeLoanVM vm, string[] ids)
        {
            try
            {
                return new EmployeeLoanDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable GetData(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeLoanDAL().GetData(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetIndividualLoanData(string[] conditionFields, string[] conditionValues)
        {
            try
            {
                return new EmployeeLoanDAL().GetIndividualLoanData(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetSummeryLoanData(string[] conditionFields, string[] conditionValues)
        {
            try
            {
                return new EmployeeLoanDAL().GetSummeryLoanData(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public DataTable getBalance(string date, string emploanId)
        {
            try
            {
                return new EmployeeLoanDAL().getBalance(date, emploanId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] UpdateSettelment(string loanId, decimal TotalDuePrincipalAmount, decimal TotalDueInterestAmount, string EarlySellteDate)
        {
            try
            {
                return new EmployeeLoanDAL().UpdateSettelment(loanId, TotalDuePrincipalAmount, TotalDueInterestAmount, EarlySellteDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ApprovedSettelment(string loanId, string EarlySellteDate)
        {
            try
            {
                return new EmployeeLoanDAL().ApprovedSettelment(loanId, EarlySellteDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
