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
    public class InvestmentDetailRepo
    {
        public List<InvestmentDetailVM> SelectById(int Id = 0)
        {
            try
            {
                return new InvestmentDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<InvestmentDetailVM> SelectByMasterId(int Id = 0)
        {
            try
            {
                return new InvestmentDetailDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<InvestmentDetailVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new InvestmentDetailDAL().SelectAll(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
