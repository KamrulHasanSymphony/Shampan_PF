using SymServices.Common;
using SymServices.PF;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.PF
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

        public List<EnumDivisionVM> DivisionDropDown(string country)
        {
            try
            {
                EmployeeInfoForPFDAL dal = new EmployeeInfoForPFDAL();
                return dal.DivisionDropDown(country);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EnumDistrictVM> DistrictDropDown(string division)
        {
            try
            {
                EmployeeInfoForPFDAL dal = new EmployeeInfoForPFDAL();
                return dal.DistrictDropDown(division);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<EmployeeNomineeVM> SelectAllByEmployeeNominee(string employeeId)
        {
            try
            {
                return new EmployeeInfoForPFDAL().SelectAllByEmployeeNominee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] InsertNominee(EmployeeNomineeVM employeeNomineeVM)
        {
            try
            {
                return new EmployeeInfoForPFDAL().InsertNominee(employeeNomineeVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] UpdateNominee(EmployeeNomineeVM employeeNomineeVM)
        {
            try
            {
                return new EmployeeInfoForPFDAL().UpdateNominee(employeeNomineeVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] DeleteNominee(EmployeeNomineeVM employeeNomineeVM, string[] ids)
        {
            try
            {
                return new EmployeeInfoForPFDAL().DeleteNominee(employeeNomineeVM, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeNomineeVM SelectByIdNominee(int Id)
        {
            try
            {
                return new EmployeeInfoForPFDAL().SelectByIdNominee(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
