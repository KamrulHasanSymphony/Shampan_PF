using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class SymUserRollVM
    {
        public string Id { get; set; }
        public string DefaultRollId { get; set; }
        public int BranchId { get; set; }
        public int GroupId { get; set; }
        public string EmpName { get; set; }
        public string symArea { get; set; }
        public string symController { get; set; }
        public bool IsIndex { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsReport { get; set; }
        public bool IsProcess { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        public IEnumerable<SymUserDefaultRollVM> SymUserDefaultRollVMs { get; set; }
        public UserLogsVM UserlogVM { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class SymUserDefaultRollVM
    {
        public string Id { get; set; }
        public string DefaultRollId { get; set; }
        public int BranchId { get; set; }
        [Display(Name = "Group")]
        public int GroupId { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        [Display(Name = "Sym Area")]
        public string symArea { get; set; }
        [Display(Name = "Sym Controller")]
        public string symController { get; set; }
        [Display(Name = "Index")]
        public bool IsIndex { get; set; }
        [Display(Name = "Add")]
        public bool IsAdd { get; set; }
        [Display(Name = "Edit")]
        public bool IsEdit { get; set; }
        [Display(Name = "Delete")]
        public bool IsDelete { get; set; }
        [Display(Name = "Report")]
        public bool IsReport { get; set; }
        [Display(Name = "Process")]
        public bool IsProcess { get; set; }
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
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
