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
    public class EmployeeForfeitureRepo
    {

        public List<EmployeePFForfeitureVM> SelectAll(string empid = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeForfeitureDAL().SelectAll(empid,conditionFields,conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeePFForfeitureVM> SelectAllList(string empid = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeForfeitureDAL().SelectAllList(empid, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public EmployeePFForfeitureVM SelectByIdAll(string Id, string empId = "")
        {
            try
            {
                return new EmployeeForfeitureDAL().SelectByIdAll(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeePFForfeitureVM vm)
        {
            try
            {
                return new EmployeeForfeitureDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(EmployeePFForfeitureVM vm)
        {
            try
            {
                return new EmployeeForfeitureDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(EmployeePFForfeitureVM vm)
        {
            try
            {
                return new EmployeeForfeitureDAL().Post(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePost(EmployeePFForfeitureVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeForfeitureDAL().MultiplePost(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExportExcelFileFormEmployee(EmployeePFForfeitureVM vm, string Filepath, string FileName)
        {
            return new EmployeeForfeitureDAL().ExportExcelFileFormEmployee(vm, Filepath, FileName);
        }

        public DataTable ExportExcelFileFormPFOpening(EmployeePFForfeitureVM vm, string Filepath, string FileName)
        {
            return new EmployeeForfeitureDAL().ExportExcelFileFormPFOpening(vm, Filepath, FileName);
        }

        public string[] ImportExcelFile(string fullpath, string fileName, ShampanIdentityVM vm)
        {
            try
            {
                return new EmployeeForfeitureDAL().ImportExcelFile(fullpath, fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///////// ForFeiture /////////////

        public List<EmployeePFOpeinigVM> SelectAllForFeiture(string empid = null)
        {
            try
            {
                return new EmployeePFOpeinigDAL().SelectAllForFeiture(empid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeePFOpeinigVM SelectByIdForFeiture(string Id, string empId = "")
        {
            try
            {
                return new EmployeePFOpeinigDAL().SelectByIdForFeiture(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeePFOpeinigVM SelectForFeitureById(string Id, string empId = "")
        {
            try
            {
                return new EmployeePFOpeinigDAL().SelectForFeitureById(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeePFOpeinigVM SelectById(string Id, string empId = "")
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

        public string[] InsertForfeiture(EmployeePFOpeinigVM vm)
        {
            try
            {
                return new EmployeePFOpeinigDAL().InsertForfeiture(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] UpdateForFeiture(EmployeePFOpeinigVM vm)
        {
            try
            {
                return new EmployeePFOpeinigDAL().UpdateForFeiture(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] PostForFeiture(EmployeePFOpeinigVM vm)
        {
            try
            {
                return new EmployeePFOpeinigDAL().PostForFeiture(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePostForFeiture(EmployeePFOpeinigVM vm, string[] Ids)
        {
            try
            {
                return new EmployeePFOpeinigDAL().MultiplePostForFeiture(vm, Ids, null, null);
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



        public EmployeePFForfeitureVM GetCodeById(string id, string empId="")
        {
            try
            {
                return new EmployeeForfeitureDAL().GetCodeById(id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
