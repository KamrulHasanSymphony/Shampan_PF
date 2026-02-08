using System;
using System.Collections.Generic;
using System.Data;
using SymServices.PF;
using SymViewModel.Common;
using SymViewModel.PF;

namespace SymRepository.PF
{
    public class ProfitDistributionNewRepo
    {
       
        public List<ProfitDistributionNewVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ProfitDistributionNewDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public ResultVM Process(ProfitDistributionNewVM vm)
        {
            try
            {
                ProfitDistributionNewDAL profitDistributionNewDal = new ProfitDistributionNewDAL();

                return profitDistributionNewDal.Process(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}