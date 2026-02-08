using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WWF
{
    public class BankBranchVM
    {
        public int Id { get; set; }
        [Display(Name = "Bank")]
        public int BankId { get; set; }
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }
        [Display(Name = "Branch Address")]
        public string BranchAddress { get; set; }
        [Display(Name = "Bank Account Type")]
        public string BankAccountType { get; set; }
        [Display(Name = "Bank Account No")]
        public string BankAccountNo { get; set; }
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        public string Name { get; set; }


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

        public string Operation { get; set; }
        public string TransType { get; set; }


        public string BranchId { get; set; }
    }
}
