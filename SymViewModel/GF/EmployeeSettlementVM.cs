using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.GF
{
    public class EmployeeSettlementVM
    {
        public int Id { get; set; }
        [Display(Name = "GF Policy")]
        public string GFPolicyName { get; set; }
        [Display(Name = "GF Policy")]
        public int GFPolicyId { get; set; }
        [Display(Name = "P. Last Basic Multipication")]
        public decimal PolicyLastBasicMultipication { get; set; }
        public string EmployeeId { get; set; }
        public string DesignationId { get; set; }

        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string ProjectId { get; set; }

        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        [Display(Name = "Left Date")]
        public string LeftDate { get; set; }
        [Display(Name = "Last Gross")]
        public decimal LastGross { get; set; }
        [Display(Name = "Last Basic")]
        public decimal LastBasic { get; set; }
        [Display(Name = "Settlement Date")]
        public string SettlementDate { get; set; }
        [Display(Name = "GF Value")]
        public decimal GFValue { get; set; }
        [Display(Name = "Service Charge")]
        public decimal ServiceCharge { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }

        [Display(Name = "Employee Code")]
        public string EmpCode { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Project { get; set; }


        public string Operation { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        [Display(Name = "Total Job Year")]
        public int TotalJobDurationYear { get; set; }
        [Display(Name = "Policy Job Year From")]
        public int PolicyJobDurationYearFrom { get; set; }
        [Display(Name = "Policy Job Year To")]
        public int PolicyJobDurationYearTo { get; set; }

        [Display(Name = "P. Multiplication Factor")]
        public decimal PolicyMultipicationFactor { get; set; }
        [Display(Name = "Policy Fixed")]
        public bool PolicyIsFixed { get; set; }

        public List<GFPolicyVM> gfPolicyVMs {get; set;}


    }
}
