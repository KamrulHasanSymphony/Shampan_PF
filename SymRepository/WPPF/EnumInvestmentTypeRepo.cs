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
    public class EnumInvestmentTypeRepo
    {
        
        public List<EnumInvestmentTypeVM> DropDown(string tType=null, int branchId = 0)
        {
            try
            {
                return new EnumInvestmentTypeDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EnumInvestmentTypeVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EnumInvestmentTypeDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EnumInvestmentTypeVM vm)
        {
            try
            {
                return new EnumInvestmentTypeDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(EnumInvestmentTypeVM vm)
        {
            try
            {
                return new EnumInvestmentTypeDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Delete(EnumInvestmentTypeVM vm, string[] ids)
        {
            try
            {
                return new EnumInvestmentTypeDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
