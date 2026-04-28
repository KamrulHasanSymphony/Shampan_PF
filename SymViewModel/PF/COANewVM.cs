using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
    public class COANewVM
    {
          public int Id { get; set; }

        [Display(Name = "COA Group")]
        public string COAGroupId         { get; set; }
        public string Code               { get; set; }
        public string Name               { get; set; }
        public string Nature             { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks            { get; set; }

        [Display(Name = "Active")]
        public bool IsActive             { get; set; }
        public bool IsArchive            { get; set; }
        public string CreatedBy          { get; set; }
        public string CreatedAt          { get; set; }
        public string CreatedFrom        { get; set; }
        public string LastUpdateBy       { get; set; }
        public string LastUpdateAt       { get; set; }
        public string LastUpdateFrom     { get; set; }
        public string Operation          { get; set; }

        public string GroupName          { get; set; }
        public string TransType          { get; set; }

        [Display(Name = "Is Retained Earning")]
        public bool IsRetainedEarning    { get; set; }

        [Display(Name = "Serial")]
        public int COASL                 { get; set; }

        [Display(Name = "Is Net Profit")]
        public bool IsNetProfit          { get; set; }

        [Display(Name = "Is Depreciation")]
        public bool IsDepreciation       { get; set; }

        [Display(Name = "Type")]
        public string COAType            { get; set; }

        public string BranchId           { get; set; }

        // Self-referencing parent for tree structure
        [Display(Name = "Parent COA")]
        public int? ParentCOAId          { get; set; }
        public string ParentCOAName      { get; set; }   // display only
    }

    // ── Tree node VM (used by _tree JSON endpoint) ───────────────────────────
    public class COATreeVM
    {
        public int    Id            { get; set; }
        public string Code          { get; set; }
        public string Name          { get; set; }
        public string Nature        { get; set; }
        public string COAType       { get; set; }
        public string COAGroupId    { get; set; }
        public string GroupName     { get; set; }
        public int?   ParentCOAId   { get; set; }
        public bool   IsActive      { get; set; }
        public int    COASL         { get; set; }

        // Populated in C# after flat fetch — NOT from DB
        public List<COATreeVM> Children { get; set; }  
    }
}
