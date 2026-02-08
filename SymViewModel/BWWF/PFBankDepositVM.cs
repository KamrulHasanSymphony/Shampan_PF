using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.BWWF
{
    public class PFBankDepositVM
    {
        public int Id { get; set; }
        public string Code { get; set; }

        
        public int FiscalYearDetailId { get; set; }
        [Display(Name = "Total Employee PF Value")]
        public decimal TotalEmployeePFValue { get; set; }
        [Display(Name = "Total Employeer PF Value")]
        public decimal TotalEmployeerPFValue { get; set; }
        [Display(Name = "Deposit Amount")]
        public decimal DepositAmount { get; set; }
        [Display(Name = "Deposit Date")]
        public string DepositDate { get; set; }
        [Display(Name = "Bank Branch")]
        public int BankBranchId { get; set; }
        [Display(Name = "Bank Branch Name")]
        public string BankBranchName { get; set; }

        public bool Post { get; set; }
        public string TransactionType { get; set; }

        public string ReferenceNo  { get; set; }
        [Display(Name = "Transaction Media")]
        public string TransactionMedia { get; set; }


        public int ReferenceId { get; set; }
        

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

        [Display(Name = "Period Name")]
        public string PeriodName { get; set; }
        [Display(Name = "Period Start")]
        public string PeriodStart { get; set; }
        [Display(Name = "Period End")]
        public string PeriodEnd { get; set; }

        public string Operation { get; set; }

        [Display(Name = "Date From")]
        public string DateFrom { get; set; }
        [Display(Name = "Date To")]
        public string DateTo { get; set; }
        public string TransType { get; set; }





        public int TransactionMediaId { get; set; }



    }
}
