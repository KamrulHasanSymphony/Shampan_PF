using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SymServices.BWWF;
using SymViewModel.BWWF;
using System.Data;

namespace SymRepository.BWWF
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
            catch(Exception ex)
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

        public List<GLJournalVM> SelectAll(int JournalType, string[] conditionFields = null, string[] conditionValues = null
    , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {

                return glJournalDal.SelectAll(JournalType, conditionFields, conditionValues);
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
        public DataTable Report(GLJournalVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return  glJournalDal.Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}