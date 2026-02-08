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
    public class ProfitDistributionDetailRepo
    {
         public List<ProfitDistributionDetailVM> SelectByMasterId(int Id)
        {
            try
            {
                return new ProfitDistributionDetailDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ProfitDistributionDetailVM> SelectById(int Id)
        {
            try
            {
                return new ProfitDistributionDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ProfitDistributionDetailVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new ProfitDistributionDetailDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(ProfitDistributionDetailVM vm)
        {
            try
            {
                return new ProfitDistributionDetailDAL().InsertBackup(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
