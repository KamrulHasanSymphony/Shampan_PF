using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
    public class PFParameterVM
    {
        public int Id { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionDate { get; set; }
        public int AccountId { get; set; }


        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string TransType { get; set; }





    }
}
