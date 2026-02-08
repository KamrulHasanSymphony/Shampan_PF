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
    public class ProfitDistributionRepo
    {
        public List<ProfitDistributionVM> DropDown(string tType = null, int branchId = 0)
        {
            try
            {
                return new ProfitDistributionDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProfitDistributionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ProfitDistributionDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(ProfitDistributionVM vm)
        {
            try
            {
                return new ProfitDistributionDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(ProfitDistributionVM vm)
        {
            try
            {
                return new ProfitDistributionDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Delete(ProfitDistributionVM vm, string[] ids)
        {
            try
            {
                return new ProfitDistributionDAL().Delete(vm, ids);
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
                return new ProfitDistributionDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Payment(string[] ids)
        {
            try
            {
                return new ProfitDistributionDAL().Payment(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ProfitDistributionVM PreSelect(ProfitDistributionVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ProfitDistributionDAL().PreSelect(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable Report(ProfitDistributionVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ProfitDistributionDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report_Detail(ProfitDistributionVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ProfitDistributionDAL().Report_Detail(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
