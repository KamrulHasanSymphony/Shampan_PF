using System.Collections.Generic;

namespace SymViewModel.HRM
{
    public class JournalHeaders
    {
        public JournalHeaders()
        {
            GlDetails = new List<JournalDetailsModel>();
        }
        public string BatchNo { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Currency { get; set; }
        public string FiscalYear { get; set; }
        public string FiscalPeriod { get; set; }
        public string SourceType { get; set; }
        public string BatchDescription { get; set; }

        public List<JournalDetailsModel> GlDetails { get; set; }


    }
}