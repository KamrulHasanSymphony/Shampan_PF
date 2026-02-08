using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymWebUI.Areas.Config.Controllers
{
    public class EnumLoanTypeRepo
    {
        EnumLoanTypeDAL _enumDAL = new EnumLoanTypeDAL();

        public List<EnumLoanTypeVM> DropDown()
        {
            try
            {
                return _enumDAL.DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetLoanInterestPolicyList()
        {
            try
            {
                return _enumDAL.GetLoanInterestPolicyList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
    }
}
