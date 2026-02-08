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
    public class ForfeitureAccountRepo
    {
        public List<ForfeitureAccountVM> SelectAllNotTransferPDF(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ForfeitureAccountDAL().SelectAllNotTransferPDF(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        public List<ForfeitureAccountVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,int branchId=0,  string postedOnly = "", bool IsPS=false)
        {
            try
            {
                return new ForfeitureAccountDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ForfeitureAccountVM> SelectAll_ResignEmployee(string[] conditionFields = null, string[] conditionValues = null,int branchId=0,  string postedOnly = "", bool IsPS=false)
        {
            try
            {
                return new ForfeitureAccountDAL().SelectAll_ResignEmployee(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ForfeitureAccountVM PreInsert(ForfeitureAccountVM vm)
        {
            try
            {
                return new ForfeitureAccountDAL().PreInsert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Insert(ForfeitureAccountVM vm)
        {
            try
            {
                return new ForfeitureAccountDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(ForfeitureAccountVM vm)
        {
            try
            {
                return new ForfeitureAccountDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(ForfeitureAccountVM vm, string[] ids)
        {
            try
            {
                return new ForfeitureAccountDAL().Delete(vm, ids);
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
                return new ForfeitureAccountDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(ForfeitureAccountVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ForfeitureAccountDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
