using SymOrdinary;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using JQueryDataTables.Models;

using SymRepository.PF;
using SymViewModel.HRM;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;
using System.IO;
using SymRepository.Common;
using SymWebUI.Areas.PF.Models;
using SymViewModel.Common;
using SymServices.Common;

namespace SymWebUI.Areas.PF.Controllers
{
    public class JournalController : Controller
    {
        public JournalController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /PF/Journal/
        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;


        private GLJournalRepo _glJournalRepo = new GLJournalRepo();
        public ActionResult Index(string JournalType = "1")
        {
            GLJournalVM vm = new GLJournalVM
            {
                JournalType = Convert.ToInt32(JournalType)

            };
            vm.TransType = AreaTypePFVM.TransType;

            return View("~/Areas/PF/Views/Journal/Index.cshtml", vm);

        }


        public ActionResult Create(string JournalType, string TransactionForm, string TransactionId)
        {
            int TransactionType = 0;
            EnumJournalTypeRepo _JournalTypeRepo = new EnumJournalTypeRepo();

            SettingDAL _settingDal = new SettingDAL();
            string CashCOAId = _settingDal.settingValue("PF", "CashCOAId").Trim();


            var getAllData = _JournalTypeRepo.SelectAllJournalTransactionType(0, new[] { "NameTrim", "TransType" }, new[] { TransactionForm, AreaTypePFVM.TransType });
            if (getAllData.Count > 0)
            {
                TransactionType = getAllData.FirstOrDefault().Id;
            }


            GLJournalVM vm = new GLJournalVM
            {
                Operation = "add",
                JournalType = Convert.ToInt32(JournalType),
                TransactionType = TransactionType,
                TransType = AreaTypePFVM.TransType,
                CashCOAId = CashCOAId
            };

            return View("~/Areas/PF/Views/Journal/Create.cshtml", vm);

        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Investment Name information.
        /// </summary>      
        /// <returns>View containing PF Journal</returns>
        public ActionResult _index(JQueryDataTableParamModel param, int JournalType = 1)
        {

            string branchId = Session["BranchId"].ToString();
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            //List<GLJournalVM> getAllData = new List<GLJournalVM>();
            var getAllData = _glJournalRepo.SelectAll(branchId, JournalType);
            IEnumerable<GLJournalVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredData = getAllData.Where(c =>
                    c.Code.ToLower().Contains(param.sSearch.ToLower()) ||
                    c.TransactionDate.ToLower().Contains(param.sSearch.ToLower()) ||
                        //c.TransactionTypeName.ToLower().Contains(param.sSearch.ToLower()) ||
                    c.TransactionValue.ToString().ToLower().Contains(param.sSearch.ToLower()) ||
                    (c.Post ? "Yes" : "No").ToLower().Contains(param.sSearch.ToLower()) ||
                    (c.IsApprove ? "Approve" : "Not Approve").ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }

            var displayedCompanies = filteredData
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[]
                 {
                     Convert.ToString(c.Id),
                     c.Code,
                     c.TransactionDate,
                     //c.TransactionTypeName,
                     c.TransactionValue.ToString(),
                     c.Post ? "Posted" : "Not Posted",
                     c.IsApprove ? "Approve" : "Not Approve"
                 };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Adds or updates a GL Journal entry based on the operation type provided in the model. 
        /// The method performs either an insert or an update operation on the GL Journal data and redirects accordingly.
        /// </summary>
        /// <param name="vm">The GLJournalVM object containing the journal entry details and the operation to perform.</param>
        /// <returns>A redirection to the "Edit" action with the journal entry ID if the operation is successful, or stays on the current view with failure information if an error occurs.</returns>
        /// <remarks>
        /// - If the operation is "add", the method will insert a new GL Journal entry and log the result.
        /// - If the operation is "update", the method will update an existing GL Journal entry and log the result.
        /// - If an error occurs, it will log the error details and show a failure message in the session.
        /// </remarks>
        [HttpPost]
        public ActionResult CreateEdit(GLJournalVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {

                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddhhmm");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;
                    vm.TransactionType = 31;
                    vm.BranchId = Session["BranchId"].ToString();
                    result = _glJournalRepo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = vm.Id });
                    }

                    return RedirectToAction("Create", new { JournalType = vm.JournalType });

                }

                if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddhhmm");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;
                    result = _glJournalRepo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = vm.Id });

                }

                return PartialView("~/Areas/PF/Views/Journal/Create.cshtml", vm);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return PartialView("~/Areas/PF/Views/Journal/Create.cshtml", vm);
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves and displays the details of a GL Journal entry for editing based on the provided ID. 
        /// If the journal entry is found, it loads the details into a view for updating. If the journal entry is not found, an error is logged, and a new view is returned.
        /// </summary>
        /// <param name="id">The ID of the GL Journal entry to retrieve and edit.</param>
        /// <returns>A view with the details of the GL Journal entry for editing. If an error occurs, a failure message is displayed, and an empty view is returned.</returns>
        /// <remarks>
        /// - If the GL Journal entry is found, the method loads its details along with its associated journal details.
        /// - If the entry is not found, an exception is thrown, logged, and an empty form is returned for editing.
        /// - Any errors encountered during the process are logged, and a failure message is shown in the session.
        /// </remarks>
        public ActionResult Edit(string id)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                GLJournalVM vm = _glJournalRepo.SelectById(Convert.ToInt32(id)).FirstOrDefault();

                if (vm == null)
                    throw new Exception("null");

                vm.GLJournalDetails =
                    _glJournalRepo.SelectAllDetails(0, new[] { "gd.GLJournalId" }, new[] { vm.Id.ToString() });
                vm.Operation = "update";
                vm.TransType = AreaTypePFVM.TransType;
                return View("~/Areas/PF/Views/Journal/Create.cshtml", vm);

            }
            catch (Exception e)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());

                return View("~/Areas/PF/Views/Journal/Create.cshtml", new GLJournalVM());

            }
        }

        public ActionResult BlankItem(GLJournalDetailVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {

                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/PF/Views/Journal/_details.cshtml", vm);


            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return PartialView("~/Areas/PF/Views/Journal/_details.cshtml", vm);
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves and generates a report based on the provided GL Journal ID. The report type is determined by the "JournalType" field in the data. It supports generating different types of vouchers (Journal, Payment, and Receive). 
        /// The report is loaded from a specified path, and a company logo and details are injected into the report's formula fields.
        /// </summary>
        /// <param name="id">The ID of the GL Journal entry for which the report is to be generated.</param>
        /// <returns>A PDF report based on the specified GL Journal entry. If an error occurs, an exception is thrown.</returns>
        /// <remarks>
        /// - The report header is dynamically set based on the "JournalType" field (1 = Journal Voucher, 2 = Payment Voucher, 3 = Receive Voucher).
        /// - The report location is specified relative to the application's base directory, and the company's address and name are included in the report.
        /// - The method handles the report's setup and data binding, including logo injection and company-specific details.
        /// </remarks>
        public ActionResult ReportView(string id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                GLJournalVM vm = new GLJournalVM();

                string[] conditionFields = { "h.Id" };
                string[] conditionValues = { id };
                table = _glJournalRepo.Report(vm, conditionFields, conditionValues);
                ReportHead = "There are no data to Preview for Journal Voucher";
                if (table.Rows.Count > 0)
                {
                    if (table.Rows[0]["JournalType"].ToString() == "1")
                    {
                        ReportHead = "Journal Voucher";
                    }
                    if (table.Rows[0]["JournalType"].ToString() == "2")
                    {
                        ReportHead = "Payment Voucher";
                    }
                    if (table.Rows[0]["JournalType"].ToString() == "3")
                    {
                        ReportHead = "Receive Voucher";
                    }

                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtGLTransaction";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptTIB_GLTransaction.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"].ToString() + "'";
                //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Posts one or more investment records based on the provided IDs.  
        /// Updates the session permission and invokes the repository post method.
        /// </summary>
        /// <param name="ids">Tilde (~) separated string of Journal IDs to post</param>
        /// <returns>
        /// A JSON result containing the post status message from the repository
        /// </returns>
        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');

            GLJournalVM vm = _glJournalRepo.SelectById(Convert.ToInt32(a[0])).FirstOrDefault();
            if (vm.Post)
            {
                return Json("Already Posted", JsonRequestBehavior.AllowGet);

            }

            string[] result = new string[6];
            result = _glJournalRepo.Post(a);

            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Approve(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');

            GLJournalVM vm = _glJournalRepo.SelectById(Convert.ToInt32(a[0])).FirstOrDefault();
            if (vm.IsApprove)
            {
                return Json("Already Approved", JsonRequestBehavior.AllowGet);

            }

            string[] result = new string[6];
            result = _glJournalRepo.Approve(a);

            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Reject(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');

            string[] result = new string[6];
            result = _glJournalRepo.Reject(a);

            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
