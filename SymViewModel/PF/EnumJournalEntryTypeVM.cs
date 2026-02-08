using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
  public  class EnumJournalTypeVM
    {
        public int Id { get; set; }
public string Name      { get; set; }
public string Remarks { get; set; }
public string TransType { get; set; }

    }

  public class EnumJournalTransactionTypeVM
  {
      public int Id { get; set; }
      public string Name { get; set; }
      public string NameTrim { get; set; }
      public string Remarks { get; set; }
      public bool IsActive { get; set; }
      public string TransType { get; set; }

  }
}
