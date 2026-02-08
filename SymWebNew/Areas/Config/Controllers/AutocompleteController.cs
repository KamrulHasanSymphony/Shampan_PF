using SymOrdinary;
using SymRepository.Common;
using SymRepository.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Config.Controllers
{
    public class AutocompleteController : Controller
    {
   
        // GET: /Enum/DropDown/
        
        #region HRPayroll / Others
       
        //Acc
      
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Employee(string term)
        {
            return Json(new EmployeeInfoRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeCode(string term)
        {
            return Json(new EmployeeInfoRepo().AutocompleteCode(term), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmployeeCodeAll(string term)
        {
            return Json(new EmployeeInfoRepo().AutocompleteCodeAll(term), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult EmployeeMarge(string term)
        {
            return Json(new EmployeeInfoRepo().AutocompleteMarge(term), JsonRequestBehavior.AllowGet);
        }
     
        public JsonResult Department(string term)
        {
            return Json(new DepartmentRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult Section(string term)
        {
            return Json(new SectionRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Designation(string term)
        {
            return Json(new DesignationRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
      
        public JsonResult Gender(string term)
        {
            return Json(new EnumGenderRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Year(string term)
        {
            return Json(new EnumYearRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
    
        public JsonResult DepermentsByProject(string term)
        {
            return Json(new SymRepository.Common.DepartmentRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }

       
        #endregion
    }
}
