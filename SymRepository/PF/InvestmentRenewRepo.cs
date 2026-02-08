using SymServices.PF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.PF
{
    public class InvestmentRenewRenewRepo
    { 

        public List<InvestmentRenewVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new InvestmentRenewDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      


        public string[] Insert(InvestmentRenewVM vm)
        {
            try
            {
                return new InvestmentRenewDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
           
        public string[] Update(InvestmentRenewVM vm)
        {
            try
            {
                return new InvestmentRenewDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] UpdateEncash(string[] ids)
        {
            try
            {
                return new InvestmentRenewDAL().UpdateEncash(ids);
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
                return new InvestmentRenewDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
