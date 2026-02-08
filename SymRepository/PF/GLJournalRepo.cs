using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SymServices.PF;
using SymViewModel.PF;
using System.Data;

namespace SymRepository.PF
{
    public class GLJournalRepo
    {
        private GLJournalDAL glJournalDal = new GLJournalDAL();

        public string[] Insert(GLJournalVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {

                return glJournalDal.Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(GLJournalVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {

                return glJournalDal.Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLJournalVM> SelectAll(string branchId, int JournalType, string[] conditionFields = null, string[] conditionValues = null
    , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {

                return glJournalDal.SelectAll(branchId, JournalType, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLJournalVM> SelectById(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {

                return glJournalDal.SelectById(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLJournalDetailVM> SelectAllDetails(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {

                return glJournalDal.SelectAllDetails(Id, conditionFields, conditionValues);
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
                return new GLJournalDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Approve(string[] ids)
        {
            try
            {
                return new GLJournalDAL().Approve(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Reject(string[] ids)
        {
            try
            {
                return new GLJournalDAL().Reject(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(GLJournalVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return glJournalDal.Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}