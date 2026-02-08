using SymOrdinary;
using SymRepository;
using SymRepository.Common;
using SymViewModel.Common;
using SymWebUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace SymWebUI.Controllers
{


    [OutputCache(NoStore = true, Duration = 150)]
    [Authorize]
    public class HomeController : Controller
    {

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            //return RedirectToAction("Login", "Home", new { area = "Acc" });
            //if (project.ToLower() == "acc")
            //{
            //    return RedirectToAction("Login", "Home", new { area = "Acc" });
            //}
            //else if (project.ToLower()=="sage")
            //{
            //    return RedirectToAction("LoginIndex", "Home", new { area = "Sage" });
            //}
            //return RedirectToAction("Index", "Home", new { area = "Sage" });
            //return RedirectToAction("LoginIndex", "Home", new { area = "Acc" });
            //var ReturnUrl = "";
            //try
            //{
            //    ReturnUrl = HttpContext.Request.UrlReferrer.ToString();
            //}
            //catch (Exception)
            //{

            //    ReturnUrl = "/";
            //}

            UserLogsVM vm = new UserLogsVM();
            Session["User"] = "";
            Session["FullName"] = "";
            Session["UserType"] = "";
            Session["EmployeeId"] = "";
            Session["SessionDate"] = "";
            Session["SessionYear"] = "";
            ViewBag.ReturnUrl = returnUrl;
            vm.ReturnUrl = returnUrl;
            vm.SessionDate = DateTime.Now.ToString("dd-MMM-yyyy");
            return View(vm);
        }
        [AllowAnonymous]
        public ActionResult Client(string returnUrl)
        {

            return View();
        }
        [AllowAnonymous]
        public ActionResult ContactUs(string returnUrl)
        {
            EmailSettings ems = new EmailSettings();

            return View(ems);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ContactUs(EmailSettings ems)
        {
            //var result=   EmpEmailProcess(ems);

            return View();
        }
        [AllowAnonymous]
        public ActionResult AboutUs(string returnUrl)
        {

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserLogsVM vm, string returnUrl)
        {
            //checkThread(Work);
            try
            {

                WebClient client = new WebClient();
                var realip = client.DownloadString("http://ipinfo.io");
                var workStationIP = realip.Replace("\n", "").ToString();
            }
            catch (Exception)
            {

                //throw;
            }
            try
            {
                //LabelOther1
                Session["LabelOther1"] = new AppSettingsReader().GetValue("LabelOther1", typeof(string)).ToString();
                Session["LabelOther2"] = new AppSettingsReader().GetValue("LabelOther2", typeof(string)).ToString();
                Session["LabelOther3"] = new AppSettingsReader().GetValue("LabelOther3", typeof(string)).ToString();
                Session["LabelOther4"] = new AppSettingsReader().GetValue("LabelOther4", typeof(string)).ToString();
                Session["LabelOther5"] = new AppSettingsReader().GetValue("LabelOther5", typeof(string)).ToString();


                Ordinary.CompanyLogoPath = new AppSettingsReader().GetValue("CompanyLogoPath", typeof(string)).ToString();
                //ManualRoster
                string AttendanceSystem = new AppSettingsReader().GetValue("AttendanceSystem", typeof(string)).ToString();
                Session["AttendanceSystem"] = AttendanceSystem;

                string[] retResults = new string[2];
                UserInformationRepo userRepo = new UserInformationRepo();
                vm.Password = Ordinary.Encrypt(vm.Password, true);
                vm.BranchId = 1;
                Tuple<bool, UserLogsVM> result = userRepo.UserLogIn(vm);
                CompanyRepo compRepo = new CompanyRepo();
                CompanyVM company = compRepo.SelectAll().FirstOrDefault();
                if (result.Item1)
                {
                    Session["User"] = result.Item2.LogID.ToString();
                    Session["FullName"] = result.Item2.FullName.ToString();
                    Session["UserType"] = result.Item2.IsAdmin.ToString();
                    Session["EmployeeId"] = result.Item2.EmployeeId.ToString();
                    string directory = Server.MapPath(@"~/Files/EmployeeInfo\") + result.Item2.PhotoName;
                    if (!System.IO.File.Exists(directory))
                    {
                        Session["PhotoName"] = "0-mini.jpg";
                    }
                    else
                    {
                        Session["PhotoName"] = result.Item2.PhotoName.ToString();
                    }

                    Session["mgs"] = "";
                    retResults[0] = "Success"; retResults[1] = "Login Successed";
                    List<UserRolesVM> roles = new UserRoleRepo().SelectAll(result.Item2.Id.ToString(), result.Item2.BranchId);
                    string[] rol = new string[roles.Count];
                    for (int i = 0; i < rol.Length; i++)
                    {
                        rol[i] = roles[i].RoleInfoId;
                    }
                    vm.SessionDate = DateTime.Now.ToString("dd-MMM-yyyy");
                    Session["SessionDate"] = DateTime.Now.AddDays(-5).ToString("dd-MMM-yyyy");
                    Session["SessionYear"] = Convert.ToDateTime(vm.SessionDate).ToString("yyyy");
                    string roleTicket = ShampanIdentity.CreateRoleTicket(rol);

                    string basicTicket = ShampanIdentity.CreateBasicTicket(result.Item2.LogID,
                                                                            result.Item2.FullName.Trim(),
                                                                            company.Id.ToString().Trim(),
                                                                            company.Name.ToString().Trim(),
                                                                            result.Item2.BranchId.ToString(),
                                                                            result.Item2.BranchId.ToString(),
                                                                            vm.ComputerIPAddress,
                                                                            "symphony.com",
                                                                            "BDT",
                                                                            Convert.ToDateTime(vm.SessionDate).ToString("yyyyMMdd"),
                                                                            "HRM",
                                                                            result.Item2.EmployeeId.ToString().Trim(),
                                                                            result.Item2.EmployeeCode.ToString().Trim(),
                                                                            result.Item2.Id.ToString().Trim(),
                                                                            result.Item2.IsESS,
                                                                            null

                                                                            );
                    int timeOut = Convert.ToInt32(new AppSettingsReader().GetValue("COOKIE_TIMEOUT", typeof(string)));
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName, DateTime.Now, DateTime.Now.AddMinutes(30), true, basicTicket);
                    FormsAuthentication.SetAuthCookie(vm.LogID, true);
                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
                    HttpContext.Response.Cookies.Add(new HttpCookie(CompanyName, encTicket));
                    //HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    HttpContext.Application["BasicTicket" + result.Item2.LogID] = basicTicket;
                    HttpContext.Application["RoleTicket" + result.Item2.LogID] = roleTicket;
                    DataTable sessiondt = new DataTable();

                    SymUserRoleRepo _repo = new SymUserRoleRepo();
                    sessiondt = _repo.RollByUserId(result.Item2.Id.ToString().Trim());
                    if (!string.IsNullOrEmpty(Session[result.Item2.Id.ToString().Trim() + "-SymRoll"] as string))
                    {
                        Session.Remove(result.Item2.Id.ToString().Trim() + "-SymRoll");
                    }

                    Session.Add(result.Item2.Id.ToString().Trim() + "-SymRoll", sessiondt);

                    var appPath = HttpContext.Request.ApplicationPath.ToString();
                    if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && vm.ReturnUrl != "/")
                    {
                        return Redirect(vm.ReturnUrl);
                    }
                    else if (Session["UserType"].ToString() == "True")
                    {
                        return Redirect("/hrm/Home");
                    }
                    else
                    {
                        return Redirect("/hrm/employeeinfo/Edit/" + result.Item2.EmployeeId);
                    }
                }
                else
                {
                    retResults[0] = "Fail"; retResults[1] = "User Name / Password is invalid!";
                    return RedirectToAction("Index");
                }

                Session["result"] = retResults[0] + "~" + retResults[1];
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult LogOut()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && authCookie.Value != "")
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                authCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Response.Cookies.Add(authCookie);
            }
            Session["User"] = "";
            Session["FullName"] = "";
            Session["UserType"] = "";
            Session["EmployeeId"] = "";
            Session["SessionDate"] = "";
            Session["SessionYear"] = "";
            Session["mgs"] = "";
            return RedirectToAction("Index");
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/hrm/employeeinfo");
            }
        }

    }
}