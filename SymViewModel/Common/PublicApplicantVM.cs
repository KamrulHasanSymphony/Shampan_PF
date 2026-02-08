using SymViewModel.Enum;
using System;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class PublicApplicantVM : BaseEntity
   {
       public string Id { get; set; }
       public int BranchId { get; set; }
       public string JobTitle	 { get; set; }
       public string DesignationName	 { get; set; }
       public string JobCircularId	 { get; set; }
       public string ApplicantName { get; set; }
       [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z")]  
       public string ApplicantEmail	 { get; set; }
       public string CVIdentificationNo	 { get; set; }
       public string CVPath	 { get; set; }
       public string Expriance { get; set; }
       public string WorkingExprianceDetail { get; set; }
       public string PersonalDetail { get; set; }
       public string Description { get; set; }
       public HttpPostedFileBase file { get; set; }

       public object CVName { get; set; }
   }
}