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
    public class WithdrawDetailRepo
    {
        public List<WithdrawDetailVM> SelectById(int Id = 0)
        {
            try
            {
                return new WithdrawDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<WithdrawDetailVM> SelectByMasterId(int Id = 0)
        {
            try
            {
                return new WithdrawDetailDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<WithdrawDetailVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new WithdrawDetailDAL().SelectAll(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
