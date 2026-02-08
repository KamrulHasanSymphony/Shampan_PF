using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class FiscalYearRepo
    {
        FiscalYearDAL _fiscalYeaDAL = new FiscalYearDAL();

        public FiscalYearVM SelectById(string Id)
        {
            try
            {
                return _fiscalYeaDAL.SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FiscalYearVM SelectByYear(int year)
        {
            try
            {
                return _fiscalYeaDAL.SelectByYear(year);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FiscalYearVM> SelectAll(int branchId)
        {
            try
            {
                return _fiscalYeaDAL.SelectAll(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] FiscalYearInsert(FiscalYearVM vm)
        {
            try
            {
                return _fiscalYeaDAL.FiscalYearInsert(vm, null, null, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] FiscalYearUpdate(FiscalYearVM vm)
        {
            try
            {
                return _fiscalYeaDAL.FiscalYearUpdate(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FiscalYearDetailVM> FYPeriodDetail(int Id = 0)
        {
            try
            {
                return _fiscalYeaDAL.FYPeriodDetail(Id, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable LoadYear(string CurrentYear)
        {
            DataTable dataTable = new DataTable("Year");
            try
            {
                dataTable = _fiscalYeaDAL.LoadYear(CurrentYear);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataTable;
        }

        public List<FiscalYearVM> DropDownYear(int branchId)
        {
            try
            {
                return _fiscalYeaDAL.DropDownYear(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> Autocomplete(string term)
        {
            try
            {
                return _fiscalYeaDAL.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FiscalYearDetailVM> DropDownPeriod(int branchId)
        {
            try
            {
                return _fiscalYeaDAL.DropDownPeriod(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FiscalYearDetailVM> DropDownPeriod(int branchId, int FiscalYearId)
        {
            try
            {
                return _fiscalYeaDAL.DropDownPeriod(branchId, FiscalYearId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FiscalYearDetailVM> DropDownPeriodByYear(int branchId, int year)
        {
            try
            {
                return _fiscalYeaDAL.DropDownPeriodByYear(branchId, year);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FiscalYearDetailVM> DropDownPeriodNext(int branchId, int currentId)
        {
            try
            {
                return _fiscalYeaDAL.DropDownPeriodNext(branchId, currentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int FiscalPeriodIdByDate(string FiscalYearDetailId)
        {
            int FiscalPeriodId = 0;

            try
            {
                FiscalPeriodId = _fiscalYeaDAL.FiscalPeriodIdByDate(FiscalYearDetailId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return FiscalPeriodId;
        }
        public string FiscalPeriodStartDate(int FiscalPeriodId)
        {
            string FiscalPeriodStartDate = "19000101";

            try
            {
                FiscalPeriodStartDate = _fiscalYeaDAL.FiscalPeriodStartDate(FiscalPeriodId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return FiscalPeriodStartDate;
        }
        public string[] UpateFiscalYearDetail(FiscalYearDetailVM vm)
        {
            try
            {
                return _fiscalYeaDAL.UpateFiscalYearDetail(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FiscalYearDetailVM> DropDownPeriodByYearLockPayroll(int branchId, int year)
        {
            try
            {
                return _fiscalYeaDAL.DropDownPeriodByYearLockPayroll(branchId, year);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FiscalYearDetailVM> DropDownPeriodByYearLockPayroll_All(int branchId, int year)
        {
            try
            {
                return _fiscalYeaDAL.DropDownPeriodByYearLockPayroll_All(branchId, year);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int SelectDaysOfMonth(int fydId)
        {
            try
            {
                return _fiscalYeaDAL.SelectDaysOfMonth(fydId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FiscalYearDetailVM FiscalYearDetailByDate(string date)
        {
            try
            {
                return _fiscalYeaDAL.SelectAll_FiscalYearDetailByDate(date);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<FiscalYearDetailVM> SelectAll_FiscalYearDetail(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return _fiscalYeaDAL.SelectAll_FiscalYearDetail(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool FiscalPeriodLockCheck(int FiscalYearDetailId)
        {
            try
            {
                return _fiscalYeaDAL.FiscalPeriodLockCheck(FiscalYearDetailId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
