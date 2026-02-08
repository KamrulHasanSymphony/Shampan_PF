using SymServices.PF;
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
    public class EmployeeBreakMonthPFRepo
    {

        public List<EmployeeBreakMonthPFVM> SelectAll(string empid = null)
        {
            try
            {
                return new EmployeeBreakMonthPFDAL().SelectAll(empid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeBreakMonthPFVM SelectById(string Id, string empId="")
        {
            try
            {
                return new EmployeeBreakMonthPFDAL().SelectById(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeBreakMonthPFVM SelectByIdAll(string Id, string empId = "")
        {
            try
            {
                return new EmployeeBreakMonthPFDAL().SelectByIdAll(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeBreakMonthPFVM> SelectAllList(string empid = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeBreakMonthPFDAL().SelectAllList(empid, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeBreakMonthPFVM vm)
        {
            try
            {
                return new EmployeeBreakMonthPFDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(EmployeeBreakMonthPFVM vm)
        {
            try
            {
                return new EmployeeBreakMonthPFDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(EmployeeBreakMonthPFVM vm)
        {
            try
            {
                return new EmployeeBreakMonthPFDAL().Post(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePost(EmployeeBreakMonthPFVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeBreakMonthPFDAL().MultiplePost(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExportExcelFileFormEmployee(EmployeeBreakMonthPFVM vm, string Filepath, string FileName)
        {
            return new EmployeeBreakMonthPFDAL().ExportExcelFileFormEmployee(vm, Filepath, FileName);
        }

        public DataTable ExportExcelFileFormPFOpening(EmployeeBreakMonthPFVM vm, string Filepath, string FileName)
        {
            return new EmployeeBreakMonthPFDAL().ExportExcelFileFormPFOpening(vm, Filepath, FileName);
        }

        public string[] ImportExcelFile(string fullpath, string fileName, ShampanIdentityVM vm)
        {
            try
            {
                return new EmployeeBreakMonthPFDAL().ImportExcelFile(fullpath, fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
