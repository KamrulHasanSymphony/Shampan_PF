namespace SymViewModel.HRM
{
    public class JournalDetailsModel
    {


        public int Id { get; set; }
        public string JournalHeaderId { get; set; }
        public string LineReference { get; set; }
        public string LineComment { get; set; }
        public string LineDescription { get; set; }
        public string AccountCode { get; set; }
        public string CostCenterCode { get; set; }

        public decimal DrAmount { get; set; }
        public decimal CrAmount { get; set; }

        public string AccountNo { get; set; }

        public decimal Amount { get; set; }



    }
}