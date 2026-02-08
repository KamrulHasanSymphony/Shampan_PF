using SymServices.PF;
using SymViewModel.PF;
using SymViewModel.Attendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymRepository.PF
{
    public class EEHeadRepo
    {
        #region Methods
        public List<EEHeadVM> DropDown()
        {
            try
            {
                return new EEHeadDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EEHeadVM> SelectAll(int Id=0)
        {
            try
            {
                return new EEHeadDAL().SelectAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EEHeadVM vm)
        {
            try
            {
                return new EEHeadDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(EEHeadVM vm)
        {
            try
            {
                return new EEHeadDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(EEHeadVM vm, string[] ids)
        {
            try
            {
                return new EEHeadDAL().Delete(vm,ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
