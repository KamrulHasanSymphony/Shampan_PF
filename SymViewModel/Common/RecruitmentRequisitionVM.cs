using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class RecruitmentRequisitionVM
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Experience { get; set; }
        public string Deadline { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedDate { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string CreatedBy { get; set; }
         [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }
        [Display(Name = "Designation Name")]
         public string DesignationName { get; set; }
        public bool IsArchive { get; set; }
    }
}
