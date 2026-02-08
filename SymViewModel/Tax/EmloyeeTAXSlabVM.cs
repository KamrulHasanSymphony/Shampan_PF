using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
    public class EmloyeeTAXSlabVM
    {
        public int Id { get; set; }
        public string TaxSlabId { get; set; }
        public string EmployeeId { get; set; }
        public string FiscalYearDetailToId { get; set; }
        public string EffectFrom { get; set; }

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


        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }

        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Project { get; set; }
        public string Gender { get; set; }

        public string TaxSlabName { get; set; }


        public bool IsEmployeeChecked { get; set; }

        public string DojFrom { get; set; }
        public string DojTo { get; set; }
    }
}
