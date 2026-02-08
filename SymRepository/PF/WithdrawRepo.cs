using SymServices.PF;
using SymServices.Common;
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
    public class WithdrawRepo
    {
        public List<WithdrawVM> DropDown(string tType = null, int branchId = 0)
        {
            try
            {
                return new WithdrawDAL().DropDownX(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public WithdrawVM SelectAvailableBalance(WithdrawVM vm)
        {
            try
            {
                return new WithdrawDAL().SelectAvailableBalanceX(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        


        public List<WithdrawVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new WithdrawDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(WithdrawVM vm)
        {
            try
            {
                return new WithdrawDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(WithdrawVM vm)
        {
            try
            {
                return new WithdrawDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Delete(WithdrawVM vm, string[] ids)
        {
            try
            {
                return new WithdrawDAL().Delete(vm, ids);
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
                return new WithdrawDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable Report(WithdrawVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new WithdrawDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
