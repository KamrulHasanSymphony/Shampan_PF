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
    public class EmployeeTransferRepo
    {
        //================== Select All =================
        public List<EmployeeTransferVM> SelectAll(string empid = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeTransferDAL().SelectAll(empid, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeTransferVM> SelectAllList(string empid = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeTransferDAL().SelectAllList(empid, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //================== Select By Id =================
        public EmployeeTransferVM SelectById(string Id, string empId = "")
        {
            try
            {
                return new EmployeeTransferDAL().SelectById(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeTransferVM SelectByIdAll(string Id, string empId = "")
        {
            try
            {
                return new EmployeeTransferDAL().SelectByIdAll(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //================== Insert =================
        public string[] Insert(EmployeeTransferVM vm)
        {
            try
            {
                return new EmployeeTransferDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //================== Update =================
        public string[] Update(EmployeeTransferVM vm)
        {
            try
            {
                return new EmployeeTransferDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //================== Get Code By Id =================
        public EmployeeTransferVM GetCodeById(string id, string empId = "")
        {
            try
            {
                return new EmployeeTransferDAL().GetCodeById(id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
