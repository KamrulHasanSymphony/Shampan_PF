using SymServices.WPPF;
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
    public class EmployeePFPaymentRepo
    {

        public List<EmployeePFPaymentVM> SelectAll(string empid = null)
        {
            try
            {
                return new EmployeePFPaymentDAL().SelectAll(empid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeePFPaymentVM SelectById(string Id, string empId="")
        {
            try
            {
                return new EmployeePFPaymentDAL().SelectById(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeePFPaymentVM SelectByIdAll(string Id, string empId = "")
        {
            try
            {
                return new EmployeePFPaymentDAL().SelectByIdAll(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeePFPaymentVM vm)
        {
            try
            {
                return new EmployeePFPaymentDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(EmployeePFPaymentVM vm)
        {
            try
            {
                return new EmployeePFPaymentDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(EmployeePFPaymentVM vm)
        {
            try
            {
                return new EmployeePFPaymentDAL().Post(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePost(EmployeePFPaymentVM vm, string[] Ids)
        {
            try
            {
                return new EmployeePFPaymentDAL().MultiplePost(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExportExcelFileFormEmployee(EmployeePFPaymentVM vm, string Filepath, string FileName)
        {
            return new EmployeePFPaymentDAL().ExportExcelFileFormEmployee(vm, Filepath, FileName);
        }

        public DataTable ExportExcelFileFormPFOpening(EmployeePFPaymentVM vm, string Filepath, string FileName)
        {
            return new EmployeePFPaymentDAL().ExportExcelFileFormPFOpening(vm, Filepath, FileName);
        }

        public string[] ImportExcelFile(string fullpath, string fileName, ShampanIdentityVM vm)
        {
            try
            {
                return new EmployeePFPaymentDAL().ImportExcelFile(fullpath, fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
