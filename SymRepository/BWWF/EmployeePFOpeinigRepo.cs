using SymServices.BWWF;
using SymViewModel.Common;
using SymViewModel.BWWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.BWWF
{
    public class EmployeePFOpeinigRepo
    {

        public List<EmployeePFOpeinigVM> SelectAll(string empid = null)
        {
            try
            {
                return new EmployeePFOpeinigDAL().SelectAll(empid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        public EmployeePFOpeinigVM SelectById(string Id, string empId="")
        {
            try
            {
                return new EmployeePFOpeinigDAL().SelectById(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        
        public EmployeePFOpeinigVM SelectByIdAll(string Id, string empId = "")
        {
            try
            {
                return new EmployeePFOpeinigDAL().SelectByIdAll(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeePFOpeinigVM vm)
        {
            try
            {
                return new EmployeePFOpeinigDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        
        public string[] Update(EmployeePFOpeinigVM vm)
        {
            try
            {
                return new EmployeePFOpeinigDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(EmployeePFOpeinigVM vm)
        {
            try
            {
                return new EmployeePFOpeinigDAL().Post(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePost(EmployeePFOpeinigVM vm, string[] Ids)
        {
            try
            {
                return new EmployeePFOpeinigDAL().MultiplePost(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExportExcelFileFormEmployee(EmployeePFOpeinigVM vm, string Filepath, string FileName)
        {
            return new EmployeePFOpeinigDAL().ExportExcelFileFormEmployee(vm, Filepath, FileName);
        }

        public DataTable ExportExcelFileFormPFOpening(EmployeePFOpeinigVM vm, string Filepath, string FileName)
        {
            return new EmployeePFOpeinigDAL().ExportExcelFileFormPFOpening(vm, Filepath, FileName);
        }

        public string[] ImportExcelFile(string fullpath, string fileName, ShampanIdentityVM vm)
        {
            try
            {
                return new EmployeePFOpeinigDAL().ImportExcelFile(fullpath, fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
