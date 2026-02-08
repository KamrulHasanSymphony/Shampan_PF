using SymServices.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.WPPF
{
    public class PFReportRepo
    {
        public DataSet Report(PFReportVM vm)
        {
            try
            {
                return new PFReportDAL().Report(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable InvestmentDetailsReport(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().InvestmentDetailsReport(vm, conditionFields,conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable InvestmentNameDetailsReport(string InvestmentNameId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().InvestmentNameDetailsReport(InvestmentNameId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public DataTable InvestmentHeaderReport(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().InvestmentHeaderReport(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable PFContributionReport(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().PFContributionReport(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable InvestmentEncashment(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().InvestmentEncashment(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region WPPF Report (25 Dec 2021)

        public DataTable BankCharge_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().BankCharge_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable EmployeeBreakMonthPF_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().EmployeeBreakMonthPF_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable EmployeePaymentPF_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().EmployeePaymentPF_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable EmployeePaymentGF_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().EmployeePaymentGF_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable EmployeeBreakMonthGF_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().EmployeeBreakMonthGF_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable EmployeeForfeiture_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().EmployeeForfeiture_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GFEmployeeForfeiture_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().GFEmployeeForfeiture_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable EmployeePFOpeinig_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().EmployeePFOpeinig_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable EmployeeGFOpeinig_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().EmployeeGFOpeinig_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable EmployeeStatement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().EmployeeStatement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable PFEmployeeLedger(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().PFEmployeeLedger(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GFEmployeeLedger(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().GFEmployeeLedger(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GFEmployeeStatement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().GFEmployeeStatement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Loan_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().Loan_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable LoanMonthlyPayment_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().LoanMonthlyPayment_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable LoanRepaymentToBank_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().LoanRepaymentToBank_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable LoanSattlement_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().LoanSattlement_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable PFBankDeposits_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().PFBankDeposits_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable PFContribution_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().PFContribution_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GFContribution_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().GFContribution_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ProfitDistributionNew_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().ProfitDistributionNew_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ReturnOnBankInterests_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().ReturnOnBankInterests_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Withdraws_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().Withdraws_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Voucher_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().Voucher_Statement(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] YearClosing(PFReportVM vm)
        {
            try
            {
                return new PFReportDAL().Year_Closing(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet View_TrialBalance(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().View_TrialBalance(vm,conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable View_IncomeStatement(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().View_IncomeStatement(vm,conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable View_BalanceSheet(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().View_BalanceSheet(vm,conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable View_NetChange(PFReportVM vm,string[] conditionFields = null, string[] conditionValues = null , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().View_NetChange(vm,conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet InvestmentNameReport( int Id,string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PFReportDAL().InvestmentNameReport(Id,conditionFields, conditionValues);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet FRReports(PFReportVM vm)
        {
            try
            {
                return new PFReportDAL().FRReports(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet IFRSReports(PFReportVM vm)
        {
            try
            {
                return new PFReportDAL().IFRSReports(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable InvestmentAccruedSummery()
        {
            try
            {
                return new PFReportDAL().InvestmentAccruedSummery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

          public DataTable InvestmentSummery()
        {
            try
            {
                return new PFReportDAL().InvestmentSummery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

          public DataTable DataExportForSunTemplatePF(PFReportVM vm)
          {
              try
              {
                  return new PFReportDAL().DataExportForSunTemplatePF(vm);
              }
              catch (Exception ex)
              {
                  throw ex;
              }
          }

          public DataTable DataExportForSunTemplatePFProfit(PFReportVM vm)
          {
              try
              {
                  return new PFReportDAL().DataExportForSunTemplatePFProfit(vm);
              }
              catch (Exception ex)
              {
                  throw ex;
              }
          }


          public DataTable PFAllEmployee(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
          {
              try
              {
                  return new PFReportDAL().PFAllEmployee(vm, conditionFields, conditionValues);
              }
              catch (Exception ex)
              {
                  throw ex;
              }
          }


          public DataTable ProfitDistributionSummery()
          {
              try
              {
                  return new PFReportDAL().ProfitDistributionSummery();
              }
              catch (Exception ex)
              {
                  throw ex;
              }
          }
        
        #endregion        
        
    }
}
