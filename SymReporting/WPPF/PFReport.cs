using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using SymRepository.Common;
using SymServices.WPPF;
using SymViewModel.Common;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymReporting.WPPF
{
    public class PFReport
    {
        //public PFReportVM ROI_GLTransactionReport(string id)
        //{
        //    PFReportVM vm = new PFReportVM();
        //    #region try

        //    try
        //    {
        //        #region Objects and Variables

        //        string ReportHead = "";
        //        string rptLocation = "";
        //        ReportDocument doc = new ReportDocument();
        //        DataTable dt = new DataTable();

        //        ReturnOnInvestmentVM varROIVM = new ReturnOnInvestmentVM();
        //        ReturnOnInvestmentDAL _ROIDAL = new ReturnOnInvestmentDAL();
        //        GLTransactionDetailDAL _detilDAL = new GLTransactionDetailDAL();
        //        #endregion

        //        #region Data Call

        //        var ROIVM = _ROIDAL.SelectAll(Convert.ToInt32(id));
        //        dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(ROIVM));



        //        //string[] cFields = { "td.TransactionCode" };
        //        //string[] cValues = { varROIVM.TransactionCode};
        //        //dt = _detilDAL.Report(null, cFields, cValues);
        //        #endregion

        //        #region Report Populating

        //        ReportHead = "There are no data to Preview for GL Transaction for Return on Investment";
        //        if (dt.Rows.Count > 0)
        //        {
        //            ReportHead = "Return on Investment GL Transactions";
        //        }
        //        dt.TableName = "dtReturnOnInvestment";
        //        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptReturnOnInvestment.rpt";

        //        doc.Load(rptLocation);
        //        doc.SetDataSource(dt);
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


        //        doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
        //        doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //        #endregion

        //        #region MemoryStream


        //        Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        vm.MemStream = new MemoryStream();
        //        stream.CopyTo(vm.MemStream);

        //        byte[] byteInfo = vm.MemStream.ToArray();
        //        vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
        //        vm.MemStream.Position = 0;

        //        vm.FileName = "GL Transaction for Return on Investment";
        //        #endregion

        //    }
        //    #endregion

        //    #region catch and finally

        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {


        //    }
        //    #endregion

        //    return vm;
        //}

        public PFReportVM ROB_GLTransactionReport(string id)
        {
            PFReportVM vm = new PFReportVM();
            #region try

            try
            {
                #region Objects and Variables

                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                ReturnOnBankInterestVM varROIVM = new ReturnOnBankInterestVM();
                ReturnOnBankInterestDAL _ROBDAL = new ReturnOnBankInterestDAL();
                //GLTransactionDetailDAL _detilDAL = new GLTransactionDetailDAL();
                #endregion

                #region Data Call

                var Result = _ROBDAL.SelectAll(Convert.ToInt32(id));

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                //string[] cFields = { "td.TransactionCode" };
                //string[] cValues = { varROIVM.TransactionCode };
                //dt = _detilDAL.Report(null, cFields, cValues);
                #endregion

                #region Report Populating

                ReportHead = "There are no data to Preview for GL Transaction for Return on Investment";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Return on Bank Investment GL Transactions";
                }
                dt.TableName = "dtWithdraw";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptReturnOnBankInterest.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                #endregion

                #region MemoryStream


                Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                vm.MemStream = new MemoryStream();
                stream.CopyTo(vm.MemStream);

                byte[] byteInfo = vm.MemStream.ToArray();
                vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
                vm.MemStream.Position = 0;

                vm.FileName = "GL Transaction for Return on Bank Investment";
                #endregion

            }
            #endregion

            #region catch and finally

            catch (Exception)
            {

            }
            finally
            {


            }
            #endregion

            return vm;
        }


        //public PFReportVM Investment_GLTransactionReport(string id)
        //{
        //    PFReportVM vm = new PFReportVM();
        //    #region try

        //    try
        //    {
        //        #region Objects and Variables

        //        string ReportHead = "";
        //        string rptLocation = "";
        //        ReportDocument doc = new ReportDocument();
        //        DataTable dt = new DataTable();

        //        InvestmentVM varInvestmentVM = new InvestmentVM();
        //        InvestmentDAL _dal = new InvestmentDAL();
        //        GLTransactionDetailDAL _detilDAL = new GLTransactionDetailDAL();
        //        #endregion

        //        #region Data Call

        //        var InvestmentVM = _dal.SelectAll(Convert.ToInt32(id));

        //        dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(InvestmentVM));


        //        //string[] cFields = { "td.TransactionCode" };
        //        //string[] cValues = { varInvestmentVM.TransactionCode};
        //        //dt = _detilDAL.Report(null, cFields, cValues);
        //        #endregion

        //        #region Report Populating

        //        ReportHead = "There are no data to Preview for GL Transaction for Investment";
        //        if (dt.Rows.Count > 0)
        //        {
        //            ReportHead = "Investment GL Transactions";
        //        }
        //        dt.TableName = "dtInvestment";
        //        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptInvestment.rpt";

        //        doc.Load(rptLocation);
        //        doc.SetDataSource(dt);
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


        //        doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
        //        doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //        #endregion

        //        #region MemoryStream


        //        Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        vm.MemStream = new MemoryStream();
        //        stream.CopyTo(vm.MemStream);

        //        byte[] byteInfo = vm.MemStream.ToArray();
        //        vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
        //        vm.MemStream.Position = 0;

        //        vm.FileName = "GL Transaction for Investment";
        //        #endregion

        //    }
        //    #endregion

        //    #region catch and finally

        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {


        //    }
        //    #endregion

        //    return vm;
        //}


        //public PFReportVM Withdraw_GLTransactionReport(string id)
        //{
        //    PFReportVM vm = new PFReportVM();
        //    #region try

        //    try
        //    {
        //        #region Objects and Variables

        //        string ReportHead = "";
        //        string rptLocation = "";
        //        ReportDocument doc = new ReportDocument();
        //        DataTable dt = new DataTable();

        //        WithdrawVM varWithdrawVM = new WithdrawVM();
        //        WithdrawDAL _dal = new WithdrawDAL();
        //        GLTransactionDetailDAL _detilDAL = new GLTransactionDetailDAL();
        //        #endregion

        //        #region Data Call

        //        var Result = _dal.SelectAll(Convert.ToInt32(id));

        //        dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));

 

        //        //string[] cFields = { "td.TransactionMasterId", "td.TransactionType" };
        //        //string[] cValues = { varWithdrawVM.Id.ToString(), varWithdrawVM.TransactionType };
        //        //dt = _dal.Report(null, cFields, cValues);
        //        #endregion

        //        #region Report Populating

        //        ReportHead = "There are no data to Preview for GL Transaction for Withdraw";
        //        if (dt.Rows.Count > 0)
        //        {
        //            ReportHead = "Withdraw GL Transactions";
        //        }
        //        dt.TableName = "dtWithdraw";
        //        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptWithdraw.rpt";

        //        doc.Load(rptLocation);
        //        doc.SetDataSource(dt);
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


        //        doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
        //        doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //        #endregion

        //        #region MemoryStream


        //        Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        vm.MemStream = new MemoryStream();
        //        stream.CopyTo(vm.MemStream);

        //        byte[] byteInfo = vm.MemStream.ToArray();
        //        vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
        //        vm.MemStream.Position = 0;

        //        vm.FileName = "GL Transaction for Withdraw";
        //        #endregion

        //    }
        //    #endregion

        //    #region catch and finally

        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {


        //    }
        //    #endregion

        //    return vm;
        //}


        //public PFReportVM PFSettlement_GLTransactionReport(string id)
        //{
        //    PFReportVM vm = new PFReportVM();
        //    #region try

        //    try
        //    {
        //        #region Objects and Variables

        //        string ReportHead = "";
        //        string rptLocation = "";
        //        ReportDocument doc = new ReportDocument();
        //        DataTable dt = new DataTable();

        //        PFSettlementVM varPFSettlementVM = new PFSettlementVM();
        //        PFSettlementDAL _dal = new PFSettlementDAL();
        //        GLTransactionDetailDAL _detilDAL = new GLTransactionDetailDAL();
        //        #endregion

        //        #region Data Call

        //        varPFSettlementVM = _dal.SelectAll(Convert.ToInt32(id)).FirstOrDefault();



        //        string[] cFields = { "td.TransactionCode" };
        //        string[] cValues = { varPFSettlementVM.TransactionCode };
        //        dt = _detilDAL.Report(null, cFields, cValues);
        //        #endregion

        //        #region Report Populating

        //        ReportHead = "There are no data to Preview for GL Transaction for PFSettlement";
        //        if (dt.Rows.Count > 0)
        //        {
        //            ReportHead = "PFSettlement GL Transactions";
        //        }
        //        dt.TableName = "dtGLTransaction";
        //        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptGLTransaction.rpt";

        //        doc.Load(rptLocation);
        //        doc.SetDataSource(dt);
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


        //        doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
        //        doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //        #endregion

        //        #region MemoryStream


        //        Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        vm.MemStream = new MemoryStream();
        //        stream.CopyTo(vm.MemStream);

        //        byte[] byteInfo = vm.MemStream.ToArray();
        //        vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
        //        vm.MemStream.Position = 0;

        //        vm.FileName = "GL Transaction for PFSettlement";
        //        #endregion

        //    }
        //    #endregion

        //    #region catch and finally

        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {


        //    }
        //    #endregion

        //    return vm;
        //}


        //public PFReportVM AccountStatement(PFParameterVM paramVM)
        //{
        //    PFReportVM vm = new PFReportVM();
        //    #region try

        //    try
        //    {
        //        #region Objects and Variables

        //        string ReportHead = "";
        //        string rptLocation = "";
        //        ReportDocument doc = new ReportDocument();
        //        DataTable dt = new DataTable();
        //        string ReportName ="";

        //        PFReportVM rptVM = new PFReportVM();
        //        PFReportDAL _rptDAL = new PFReportDAL();

        //        AccountVM accVM = new AccountVM();
        //        AccountDAL _accDAL = new AccountDAL();

        //        #endregion

        //        #region Data Call

        //        accVM = _accDAL.SelectAll(Convert.ToInt32(paramVM.AccountId)).FirstOrDefault();

        //        dt = _rptDAL.AccountStatement(paramVM);
        //        #endregion

        //        #region Report Populating
        //        ReportName = accVM.Name + " Statement";

        //        ReportHead = "There are no data to Preview for " + ReportName;
        //        if (dt.Rows.Count > 0)
        //        {
        //            ReportHead = ReportName;
        //        }
        //        dt.TableName = "dtGLTransaction";
        //        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptAccountStatement.rpt";

        //        doc.Load(rptLocation);
        //        doc.SetDataSource(dt);
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


        //        doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
        //        doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //        #endregion

        //        #region MemoryStream


        //        Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        vm.MemStream = new MemoryStream();
        //        stream.CopyTo(vm.MemStream);

        //        byte[] byteInfo = vm.MemStream.ToArray();
        //        vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
        //        vm.MemStream.Position = 0;

        //        vm.FileName = ReportName;
        //        #endregion

        //    }
        //    #endregion

        //    #region catch and finally

        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {


        //    }
        //    #endregion

        //    return vm;
        //}

        public PFReportVM PFReportSummaryDetail(string fydid, string rType, string ProjectId)
        {

            PFReportVM vm = new PFReportVM();
            #region try

            try
            {
                #region Objects and Variables

                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                PFDetailDAL _detilDAL = new PFDetailDAL();
                #endregion

                #region Data Call

                dt = _detilDAL.PFReportSummaryDetail(fydid, rType, ProjectId);
                #endregion

                #region Report Populating

                ReportHead = "There are no data to Preview for Contribution Journal";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Contribution Journal";
                }
                dt.TableName = "dtGLTransaction";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptContributionJournal.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                #endregion

                #region MemoryStream


                Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                vm.MemStream = new MemoryStream();
                stream.CopyTo(vm.MemStream);

                byte[] byteInfo = vm.MemStream.ToArray();
                vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
                vm.MemStream.Position = 0;
                vm.DataTable = dt;
                vm.FileName = "Contribution Journal";
                #endregion

            }
            #endregion

            #region catch and finally

            catch (Exception)
            {

            }
            finally
            {


            }
            #endregion

            return vm;


        }


        //public PFReportVM AccountLedgerReport(PFParameterVM paramVM)
        //{
        //    PFReportVM vm = new PFReportVM();
        //    #region try

        //    try
        //    {
        //        #region Objects and Variables

        //        string ReportHead = "";
        //        string rptLocation = "";
        //        ReportDocument doc = new ReportDocument();
        //        DataTable dt = new DataTable();
        //        string ReportName = "";

        //        PFReportVM rptVM = new PFReportVM();
        //        PFReportDAL _rptDAL = new PFReportDAL();

        //        #endregion

        //        #region Data Call

        //        dt = _rptDAL.AccountLedgerReport(paramVM);
        //        #endregion

        //        #region Report Populating
        //                        AccountVM accVM = new AccountVM();

        //        accVM = new AccountDAL().SelectAll(Convert.ToInt32(paramVM.AccountId)).FirstOrDefault();
        //        ReportName = accVM.Name + " Account Ledger";


        //        ReportHead = "There are no data to Preview for " + ReportName;
        //        if (dt.Rows.Count > 0)
        //        {
        //            ReportHead = ReportName;
        //        }
        //        dt.TableName = "dtGLTransaction";
        //        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptAccountLedger.rpt";

        //        doc.Load(rptLocation);
        //        doc.SetDataSource(dt);
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


        //        doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
        //        doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //        #endregion

        //        #region MemoryStream


        //        Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        vm.MemStream = new MemoryStream();
        //        stream.CopyTo(vm.MemStream);

        //        byte[] byteInfo = vm.MemStream.ToArray();
        //        vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
        //        vm.MemStream.Position = 0;

        //        vm.FileName = ReportName;
        //        #endregion

        //    }
        //    #endregion

        //    #region catch and finally

        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {


        //    }
        //    #endregion

        //    return vm;
        //}


        public PFReportVM EmployeePFStatements(string rType, string EmployeeId, string ToDate,string FromDate)
        {
            PFReportVM vm = new PFReportVM();
            #region try

            try
            {
                #region Objects and Variables

                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                string ReportName = " Employee PF Statements ";

                PFReportVM rptVM = new PFReportVM();
                PFReportDAL _rptDAL = new PFReportDAL();

                #endregion

                #region Data Call

                dt = _rptDAL.EmployeePFStatements(rType,  EmployeeId, ToDate,FromDate);
                #endregion

                #region Report Populating

                ReportHead = "There are no data to Preview for Employee PF Statements ";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                dt.TableName = "dtEmployeePFStatement";

                if (rType == "Summary")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptEmployeePFStatementSummary.rpt";

                }
                else
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptEmployeePFStatement.rpt";

                }


                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                #endregion

                #region MemoryStream


                Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                vm.MemStream = new MemoryStream();
                stream.CopyTo(vm.MemStream);

                byte[] byteInfo = vm.MemStream.ToArray();
                vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
                vm.MemStream.Position = 0;

                vm.FileName = ReportName;
                #endregion

            }
            #endregion

            #region catch and finally

            catch (Exception)
            {

            }
            finally
            {


            }
            #endregion

            return vm;
        }
        public PFReportVM EmployeePFSettlementReport(string EmployeeId, string ToDate, string FromDate, string BranchName)
        {
            PFReportVM vm = new PFReportVM();
            #region try

            try
            {
                #region Objects and Variables

                decimal Payment = 0;
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                DataTable dtpay = new DataTable();
                string ReportName = " Employee PF Final Settlement ";

                PFReportVM rptVM = new PFReportVM();
                PFReportDAL _rptDAL = new PFReportDAL();

                #endregion

                #region Data Call

                dt = _rptDAL.EmployeePFSettlementReport(EmployeeId, ToDate, FromDate);
                dtpay = _rptDAL.EmployeePFSettlementPayment(EmployeeId, ToDate, FromDate);
                if(dtpay.Rows.Count>0)
                {
                    Payment =Convert.ToDecimal(dtpay.Rows[0][0].ToString());
                }

                #endregion

                #region Report Populating

                ReportHead = "There are no data to Preview for Employee PF Settlement ";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                dt.TableName = "dtEmployeePFSettlement";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptEmployeePFSettlement.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["Payment"].Text = "'" + Payment + "'";
                doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";
                #endregion

                #region MemoryStream


                Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                vm.MemStream = new MemoryStream();
                stream.CopyTo(vm.MemStream);

                byte[] byteInfo = vm.MemStream.ToArray();
                vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
                vm.MemStream.Position = 0;

                vm.FileName = ReportName;
                #endregion

            }
            #endregion

            #region catch and finally

            catch (Exception)
            {

            }
            finally
            {


            }
            #endregion

            return vm;
        }
        public PFReportVM PFInvestmentRepor(string Year, string BranchName)
        {
            PFReportVM vm = new PFReportVM();
            #region try

            try
            {
                #region Objects and Variables

                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                string ReportName = "Interest Income from FDR";

                PFReportVM rptVM = new PFReportVM();
                PFReportDAL _rptDAL = new PFReportDAL();

                #endregion

                #region Data Call

                dt = _rptDAL.PFInvestmentReport(Year);
                #endregion

                #region Report Populating

                ReportHead = "There is no data to Preview for FDR";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                dt.TableName = "dtInvestmentFDR";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\PFInvestmentReportFDR.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";
                #endregion

                #region MemoryStream


                Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                vm.MemStream = new MemoryStream();
                stream.CopyTo(vm.MemStream);

                byte[] byteInfo = vm.MemStream.ToArray();
                vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
                vm.MemStream.Position = 0;

                vm.FileName = ReportName;
                #endregion

            }
            #endregion

            #region catch and finally

            catch (Exception)
            {

            }
            finally
            {


            }
            #endregion

            return vm;
        }
        public PFReportVM PFInvestmentReporSyn(string Year, string BranchName)
        {
            PFReportVM vm = new PFReportVM();
            #region try

            try
            {
                #region Objects and Variables

                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                string ReportName = "Interest Income from Sanchayapatra";
                PFReportVM rptVM = new PFReportVM();
                PFReportDAL _rptDAL = new PFReportDAL();

                #endregion

                #region Data Call

                dt = _rptDAL.PFInvestmentReportSyn(Year);

                #endregion

                #region Report Populating

                ReportHead = "There is no data to Preview for FDR";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
            
                dt.TableName = "dtInvestmentSyn";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\PFInvestmentReportSyn.rpt";
                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";
                #endregion

                #region MemoryStream


                Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                vm.MemStream = new MemoryStream();
                stream.CopyTo(vm.MemStream);

                byte[] byteInfo = vm.MemStream.ToArray();
                vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
                vm.MemStream.Position = 0;

                vm.FileName = ReportName;
                #endregion

            }
            #endregion

            #region catch and finally

            catch (Exception)
            {

            }
            finally
            {


            }
            #endregion

            return vm;
        }

        public PFReportVM EmployeeLoanSettelment(string rType, string ToDate, string FromDate, string BranchId, string BranchName)
        {
            PFReportVM vm = new PFReportVM();
            #region try

            try
            {
                #region Objects and Variables
                DateTime startdate = DateTime.Now;
                DateTime enddate = DateTime.Now;
                string ReportHead = "";
                string rptLocation = "";
                if(ToDate!="" && FromDate!="")
                {
                    startdate = Convert.ToDateTime(FromDate);
                    enddate = Convert.ToDateTime(ToDate);
                }

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                string ReportName = " Advance Loan Settlement Report " + startdate.ToString("yyyy") + " - " + enddate.ToString("yy") + " ";

                PFReportVM rptVM = new PFReportVM();
                PFReportDAL _rptDAL = new PFReportDAL();

                #endregion

                #region Data Call

                dt = _rptDAL.EmployeeLoanSettelment(rType, ToDate, FromDate, BranchId);
                #endregion

                #region Report Populating

                ReportHead = "There are no data to Preview for Employee Loan Settelment ";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                dt.TableName = "dtEmployeeLoanSettelment";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptEmployeeLoanSettelment.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                #endregion

                #region MemoryStream


                Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                vm.MemStream = new MemoryStream();
                stream.CopyTo(vm.MemStream);

                byte[] byteInfo = vm.MemStream.ToArray();
                vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
                vm.MemStream.Position = 0;

                vm.FileName = ReportName;
                #endregion

            }
            #endregion

            #region catch and finally

            catch (Exception)
            {

            }
            finally
            {


            }
            #endregion

            return vm;
        }

        public PFReportVM EmployeeLoanClosed(string rType, string ToDate, string FromDate, string BranchName)
        {
            PFReportVM vm = new PFReportVM();
            #region try

            try
            {
                #region Objects and Variables

                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                string ReportName = " Employee Loan Closed report of " + FromDate + " to " + ToDate + " ";

                PFReportVM rptVM = new PFReportVM();
                PFReportDAL _rptDAL = new PFReportDAL();

                #endregion

                #region Data Call

                dt = _rptDAL.EmployeeLoanClosed(rType, ToDate, FromDate);
                #endregion

                #region Report Populating

                ReportHead = "There are no data to Preview for Employee Loan Settelment ";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                dt.TableName = "dtEmployeeLoanSettelment";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptEmployeeLoanClosed.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";
                #endregion

                #region MemoryStream


                Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                vm.MemStream = new MemoryStream();
                stream.CopyTo(vm.MemStream);

                byte[] byteInfo = vm.MemStream.ToArray();
                vm.MemStream.Write(byteInfo, 0, byteInfo.Length);
                vm.MemStream.Position = 0;

                vm.FileName = ReportName;
                #endregion

            }
            #endregion

            #region catch and finally

            catch (Exception)
            {

            }
            finally
            {


            }
            #endregion

            return vm;
        }
    }
}
