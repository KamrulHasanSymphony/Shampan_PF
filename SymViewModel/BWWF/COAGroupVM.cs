using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.BWWF
{
    public class COAGroupVM
    {
        public int Id { get; set; }
        [Display(Name = "COA Group")]
        public string COAGroupId { get; set; }
        public string GroupName { get; set; }
        [Display(Name = "COA Sub Group")]
        public string COASubGroupId { get; set; }
        public string SubGroupName { get; set; }
        [Display(Name = "COA Category")]
        public string COACategoryId { get; set; }
        public string CategoryName { get; set; }


public string Name   { get; set; }
public string GroupType   { get; set; }
         [Display(Name = "COA Type of Report")]
public string COATypeOfReportId { get; set; }
           [Display(Name = "COA Group Type")]
public string COAGroupTypeId { get; set; }
 [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Serial")]

        public int GroupSL { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        public string Operation { get; set; }
        public string TransType { get; set; }
        public string COATypeOfReport { get; set; }
        public string COAGroupType { get; set; }
    }

     
    }
