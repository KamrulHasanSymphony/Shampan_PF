using SymServices.WWF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.WWF
{
    public class PFSettlementDetailRepo
    {
        public List<PFSettlementDetailVM> SelectById(int Id = 0)
        {
            try
            {
                return new PFSettlementDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PFSettlementDetailVM> SelectByMasterId(int Id = 0)
        {
            try
            {
                return new PFSettlementDetailDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PFSettlementDetailVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFSettlementDetailDAL().SelectAll(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
