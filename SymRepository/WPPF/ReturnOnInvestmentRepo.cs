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
    public class ReturnOnInvestmentRepo
    {
        public List<ReturnOnInvestmentVM> SelectAllNotTransferPDF(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ReturnOnInvestmentDAL().SelectAllNotTransferPDF(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ReturnOnInvestmentVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ReturnOnInvestmentDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReturnOnInvestmentVM PreInsert(ReturnOnInvestmentVM vm)
        {
            try
            {
                return new ReturnOnInvestmentDAL().PreInsert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Insert(ReturnOnInvestmentVM vm)
        {
            try
            {
                return new ReturnOnInvestmentDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(ReturnOnInvestmentVM vm)
        {
            try
            {
                return new ReturnOnInvestmentDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(ReturnOnInvestmentVM vm, string[] ids)
        {
            try
            {
                return new ReturnOnInvestmentDAL().Delete(vm, ids);
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
                return new ReturnOnInvestmentDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(ReturnOnInvestmentVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ReturnOnInvestmentDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
