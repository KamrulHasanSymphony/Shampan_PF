using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.GF
{
    public class GFEmployeeProvisionVM
    {



        public int Id { get; set; }
        public int FiscalYearDetailId { get; set; }
        public string EmployeeId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string JoinDate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal BasicAmount { get; set; }
        public decimal ProvisionAmount { get; set; }
        public int GFPolicyId { get; set; }
        public decimal MultipicationFactor { get; set; }
        public decimal JobMonth { get; set; }

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

////////Id
////////FiscalYearDetailId
////////EmployeeId
////////ProjectId
////////DepartmentId
////////SectionId
////////DesignationId
////////JoinDate
////////GrossAmount
////////BasicAmount
////////ProvisionAmount
////////GFPolicyId
////////MultipicationFactor
////////JobMonth



        public string EmpName { get; set; }

        public string Code { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Project { get; set; }


        public string GFHeaderId { get; set; }
        public bool IsBreakMonth { get; set; }


        public decimal IncrementArrear { get; set; }

        public string GFStartFrom { get; set; }

        public decimal TotalProvisionAmount { get; set; }
    }
}
