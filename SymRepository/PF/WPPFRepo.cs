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
    public class WPPFRepo
    {
       
        public List<PFHeaderVM> SelectFiscalPeriodHeader(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new WPPFDAL().SelectFiscalPeriodHeader(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PFHeaderVM> SelectProfitDistribution(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new WPPFDAL().SelectProfitDistribution(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(decimal? TotalProfit, string FiscalYearDetailId, int? FiscalYear, ShampanIdentityVM auditvm)
        {
            try
            {
                return new WPPFDAL().Insert(TotalProfit, FiscalYearDetailId, FiscalYear, auditvm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] PostHeader(PFHeaderVM vm)
        {
            try
            {
                return new WPPFDAL().PostHeader(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PFHeaderVM> SelectAll()
        {
            try
            {
                return new WPPFDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
