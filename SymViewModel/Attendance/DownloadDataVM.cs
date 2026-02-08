using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymViewModel.Attendance
{
    public class DownloadDataVM
    {
        [Required]
        public int Id { get; set; }
        //[Required]
        //public int BranchId { get; set; }

        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        [Required]        
		[Display(Name = "Punch Data")]
        public string PunchData { get; set; }
        [Required]        
		[Display(Name = "Proxy ID")]
        public string ProxyID { get; set; }
        [Required]
		[Display(Name = "Punch Date")]
        public string PunchDate { get; set; }
        [Required]
		[Display(Name = "Punch Time")]
        public string PunchTime { get; set; }
        [Required]
		[Display(Name = "Node ID")]
        public string NodeID { get; set; }
        [Required]
		[Display(Name = "Migrate")]
        public bool IsMigrate { get; set; }


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


        public string EmployeeId { get; set; }
        public string EmployeeGroupId { get; set; }
        public string AttendanceStructureId { get; set; }




        public string ProxyID1 { get; set; }

        public string AttendanceSystem { get; set; }
    }
}



