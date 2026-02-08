using SymServices.Common;
using SymServices.WWF;
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
    public class EmployeeInfoForPFRepo
    {
        public string[] InsertEmployeeInfoForPF(EmployeeInfoForPFVM vm)
        {
            try
            {
                return new EmployeeInfoForPFDAL().InsertEmployeeInfoForPF(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeInfoForPFVM> SelectAllEmployeeInfoForPF(string[] conditionFields, string[] conditionValues)
        {
            try
            {
                return new EmployeeInfoForPFDAL().SelectAllEmployeeInfoForPF(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeInfoForPFVM> SelectRegnationEmployeeInfoForPF(string[] conditionFields, string[] conditionValues)
        {
            try
            {
                return new EmployeeInfoForPFDAL().SelectRegnationEmployeeInfoForPF(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public EmployeeInfoForPFVM SelectById(int id)
        {
            try
            {
                return new EmployeeInfoForPFDAL().SelectById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public string[] DeleteEmployeeInfoForPF(int Id)
        {
            try
            {
                return new EmployeeInfoForPFDAL().DeleteEmployeeInfoForPF(Id, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] ImportExcelFile(EmployeeInfoForPFVM vm)
        {
            try
            {
                EmployeeInfoForPFDAL dal = new EmployeeInfoForPFDAL(); // Create an instance
                return dal.InsertExportData(vm, null, null); // Call the method on the instance
            }
            catch (Exception)
            {
                throw;
            }
        }


        public string[] SelectFromAPIData()
        {
            try
            {
                EmployeeInfoForPFDAL dal = new EmployeeInfoForPFDAL(); // Create an instance
                return dal.SelectFromAPIData(null, null); // Call the method on the instance
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string[] ReActiveEmployeeInfoForPF(string Id)
        {
            try
            {
                return new EmployeeInfoForPFDAL().ReActiveEmployeeInfoForPF(Id, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] UpdatePhoto(int EmployeeId, string PhotoName)
        {
            try
            {
                return new EmployeeInfoForPFDAL().UpdatePhoto(EmployeeId, PhotoName, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
