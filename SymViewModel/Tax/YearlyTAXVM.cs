namespace SymViewModel.Tax
{
    public class YearlyTAXVM
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string FiscalYearId { get; set; }
        public string FiscalYearDetailId { get; set; }
        public decimal Value { get; set; }
        public decimal AdvanceTax { get; set; }
        public string FiscalPeriodName { get; set; }
        public string FiscalYearName { get; set; }
        public string EmployeeName { get; set; }

        public string LastUpdateAt { get; set; }

        public string LastUpdateBy { get; set; }

        public string LastUpdateFrom { get; set; }
    }
}