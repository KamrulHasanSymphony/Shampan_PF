using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.HRM
{
    public class DesignationVM
    {
        public string Id { get; set; }
        public int BranchId { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
        public string Name { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        [Display(Name = "Attendance Bonus")]
        public decimal AttendenceBonus { get; set; }
        [Display(Name = "Export Processing Zones")]
        public decimal EPZ { get; set; }
        public decimal Other { get; set; }
        [Display(Name = "Dinner Amount")]
        public decimal DinnerAmount { get; set; }
        [Display(Name = "Iftar Amount")]
        public decimal IfterAmount { get; set; }
        [Display(Name = "Tiffin Amount")]
        public decimal TiffinAmount { get; set; }
        [Display(Name = "Extra Tiffin Amount")]
        public decimal ETiffinAmount { get; set; }
        [Display(Name = "OT allowance")]
        public bool OTAlloawance { get; set; }
        [Display(Name = "OT Orginal")]
        public bool OTOrginal { get; set; }
        [Display(Name = "OT Bayer")]
        public bool OTBayer { get; set; }
        [Display(Name = "Extra OT")]
        public bool ExtraOT { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        [Display(Name = "Designation Group")]
        public string DesignationGroupId { get; set; }

        [Display(Name = "Grade")]
        public string GradeId { get; set; }
         [Display(Name = "Priority Level")]
        public int PriorityLevel { get; set; }

        [Display(Name = "Order No")]
        public int OrderNo { get; set; }

       

        //=====InsurancePlane========

        [Display(Name = "Hospital Benefit insured Per Incident")]
        public string HospitalPlanC1 { get; set; }
        [Display(Name = "Hospital Room & Board Limit")]
        public string HospitalPlanC2 { get; set; }
        [Display(Name = "Daily Room Limit")]
        public string HospitalPlanC3 { get; set; }
        [Display(Name = "Intensive Care Unit")]
        public string HospitalPlanC4 { get; set; }
        [Display(Name = "Hospital Service/Surgical Anesthesia Charge")]
        public string HospitalPlanC5 { get; set; }
        [Display(Name = "Death Coverage")]
        public string DeathCoveragePlanC6 { get; set; }
        [Display(Name = "In Patient Maternity Plan")]
        public string MaternityPlanC7 { get; set; }
        [Display(Name = "In Patient Maternity Plan")]
        public string MaternityPlanC8 { get; set; }
        [Display(Name = "In Patient Maternity Plan")]
        public string MaternityPlanC9 { get; set; }

        //==========Entitlement============
        [Display(Name = "Company Car")]
        public string EntitlementC1 { get; set; }
        [Display(Name = "Car Allowance(Incl.EMI,Maintenance)")]
        public string EntitlementC2 { get; set; }
        [Display(Name = "Fuel")]
        public string EntitlementC3 { get; set; }
        [Display(Name = "Maintenance")]
        public string EntitlementC4 { get; set; }
        [Display(Name = "Driver")]
        public string EntitlementC5 { get; set; }

        //=====Mobile Expense====
        [Display(Name = "Title")]
        public string MobileExpenseC1 { get; set; }

        [Display(Name = "Hand SET")]
        public string MobileExpenseC2 { get; set; }

        [Display(Name = "Monthly Limit(Operations)")]
        public string MobileExpenseC3 { get; set; }

        [Display(Name = "Monthly Limit(Others)")]
        public string MobileExpenseC4 { get; set; }

        //============International Travel===========
        [Display(Name = "Air Fare")]
        public string InternationalTravelC1 { get; set; }
        [Display(Name = "Hotel")]
        public string InternationalTravelC2 { get; set; }
        [Display(Name = "Daily Expenses Limit(Food, laundry,etc)")]
        public string InternationalTravelC3 { get; set; }
        [Display(Name = "Daily Expenses Limit(Food, laundry,etc)")]
        public string InternationalTravelC4 { get; set; }

        //============Domestic Travel===========
        [Display(Name = "Transportation")]
        public string DomesticlTravelC1 { get; set; }
        [Display(Name = "Accommodation")]
        public string DomesticTravelC2 { get; set; }
        [Display(Name = "Daily Expenses limit for Day Tip")]
        public string DomesticTravelC3 { get; set; }
        [Display(Name = "Daily Expenses Overnight for Day Tip")]
        public string DomesticTravelC4 { get; set; }
        [Display(Name = "Domestic Travel C5")]
        public string DomesticTravelC5 { get; set; }

        ///////////////        Plz delete after new 
        ////////////public int Id { get; set; }
        ////////////public string EmployeeId { get; set; }
        //////////////public string Code { get; set; }
        //////////////public string Name { get; set; }
        ////////////public decimal Salary { get; set; }




    }
}
