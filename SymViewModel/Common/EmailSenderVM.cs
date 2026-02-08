using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class EmailSenderVM
    {
        public int Id { get; set; }
        public string SenderEmail { get; set; }
        public string RecepentEmail { get; set; }
        public string EmailSubject { get; set; }
        public bool IsAttachmentCV { get; set; }
        public string RecipientName { get; set; }
        public int PresentDistrictId { get; set; }
        public int JobCategoryId { get; set; }
        public string Website { get; set; }
    }
}
