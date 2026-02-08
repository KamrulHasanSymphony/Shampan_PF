using SymServices.BWWF;
using SymViewModel.BWWF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.BWWF
{
  public  class InvestmentAccruedRepo
    {

      public List<InvestmentAccruedVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
      {
          try
          {
              return new InvestmentAccruedDAL().SelectAll(Id, conditionFields, conditionValues);
          }
          catch (Exception ex)
          {
              throw ex;
          }
      }

      public string[] Process(InvestmentNameVM vm)
      {
          try
          {
              return new InvestmentAccruedDAL().Process(vm);
          }
          catch (Exception ex)
          {
              throw ex;
          }
      }

      public string[] Post(string[] ids)
      {
          try
          {
              return new InvestmentAccruedDAL().Post(ids);
          }
          catch (Exception ex)
          {
              throw ex;
          }
      }
      public string[] Delete(string[] ids)
      {
          try
          {
              return new InvestmentAccruedDAL().Delete(ids);
          }
          catch (Exception ex)
          {
              throw ex;
          }
      }
    }
}
