namespace SymViewModel.BWWF
{
    public class GFHeaderVM
    {


        public int Id { get; set; }
        
        public string  Code{ get; set; }
        public string  FiscalYearDetailId{ get; set; }
        public string   ProjectId{ get; set; }
        public decimal   ProvisionAmount{ get; set; }
        public bool      Post{ get; set; }
        public string   Remarks{ get; set; }
        public string      IsActive{ get; set; }
        public string  IsArchive{ get; set; }
        public string      CreatedBy{ get; set; }
        public string  CreatedAt{ get; set; }
        public string      CreatedFrom{ get; set; }
        public string  LastUpdateBy{ get; set; }
        public string      LastUpdateAt{ get; set; }
        public string LastUpdateFrom { get; set; }
        public string PeriodStart { get; set; }

        public string ProjectName { get; set; }

        public string FiscalPeriod { get; set; }
        public string TransType { get; set; }

        public decimal IncrementArrear { get; set; }

        public decimal TotalProvisionAmount { get; set; }
    }
}