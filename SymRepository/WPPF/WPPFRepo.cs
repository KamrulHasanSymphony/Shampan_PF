using SymServices.WPPF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.WPPF
{
    public class WPPFRepo
    {

        public List<PFHeaderVM> SelectFiscalPeriodHeader(string TransType, string[] conditionFields = null, string[] conditionValues = null )
        {
            try
            {
                return new WPPFDAL().SelectFiscalPeriodHeader(TransType, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PFHeaderVM> SelectProfitDistribution(string TransType, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new WPPFDAL().SelectProfitDistribution(TransType, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(decimal? TotalProfit, string FiscalYearDetailId, int? FiscalYear,string TransType, ShampanIdentityVM auditvm)
        {
            try
            {
                return new WPPFDAL().Insert(TotalProfit, FiscalYearDetailId, FiscalYear,TransType, auditvm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] PostHeader(string TransType, PFHeaderVM vm)
        {
            try
            {
                return new WPPFDAL().PostHeader(TransType, vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PFHeaderVM> SelectAll(string TransType)
        {
            try
            {
                return new WPPFDAL().SelectAll(TransType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
