using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SymViewModel.Common
{
    public class ProfessionalQualificationVM
    {
        public int Id { get; set; }
        public string Certification { get; set; }
        [Display(Name = "PQ Institute")]
        public string PQInstitute { get; set; }
        public string Location { get; set; }
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        public string ApplicantId { get; set; }

    }
}
