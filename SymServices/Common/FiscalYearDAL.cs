using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SymViewModel.Common;
using SymOrdinary;
using SymServices.Loan;

namespace SymServices.Common
{
    public class FiscalYearDAL
    {
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();


        //==================SelectByID=================
        public FiscalYearVM SelectByYear(int year)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            FiscalYearVM fiscalYearVM = new FiscalYearVM();
            List<FiscalYearDetailVM> fiscalYearDetailVMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM fiscalYearDetailVM;

            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
Id
,BranchId
,Year
,YearStart
,YearEnd
,YearLock
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

    From FiscalYear
where  Year=@Year
     
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Year", year);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    fiscalYearVM.Id = dr["Id"].ToString();
                    fiscalYearVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    fiscalYearVM.Year = Convert.ToInt32(dr["Year"]); ;
                    fiscalYearVM.YearStart = Ordinary.StringToDate(dr["YearStart"].ToString());
                    fiscalYearVM.YearEnd = Ordinary.StringToDate(dr["YearEnd"].ToString());
                    fiscalYearVM.YearLock = Convert.ToBoolean(dr["YearLock"]);
                    fiscalYearVM.Remarks = dr["Remarks"].ToString();
                    fiscalYearVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    fiscalYearVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    fiscalYearVM.CreatedBy = dr["CreatedBy"].ToString();
                    fiscalYearVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    fiscalYearVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    fiscalYearVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    fiscalYearVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                }
                dr.Close();

                sqlText = @"SELECT
 Id
,FiscalYearId
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,PayrollLock
,PFLock
,TAXLock
,LoanLock
,Remarks
    From FiscalYearDetail
where  FiscalYearId=@FiscalYearId
";
                SqlCommand cmdDetails = new SqlCommand();
                cmdDetails.Connection = currConn;
                cmdDetails.CommandText = sqlText;
                cmdDetails.CommandType = CommandType.Text;
                cmdDetails.Parameters.AddWithValue("@FiscalYearId", fiscalYearVM.Id);

                using (SqlDataReader ddr = cmdDetails.ExecuteReader())
                {
                    while (ddr.Read())
                    {
                        fiscalYearDetailVM = new FiscalYearDetailVM();
                        fiscalYearDetailVM.Id = Convert.ToInt32(ddr["Id"]);
                        fiscalYearDetailVM.FiscalYearId = ddr["FiscalYearId"].ToString();
                        fiscalYearDetailVM.PeriodName = ddr["PeriodName"].ToString();
                        fiscalYearDetailVM.PeriodStart = Ordinary.StringToDate(ddr["PeriodStart"].ToString());
                        fiscalYearDetailVM.PeriodEnd = Ordinary.StringToDate(ddr["PeriodEnd"].ToString());
                        fiscalYearDetailVM.PeriodLock = Convert.ToBoolean(ddr["PeriodLock"]);
                        fiscalYearDetailVM.PayrollLock = Convert.ToBoolean(ddr["PayrollLock"]);
                        fiscalYearDetailVM.PFLock = Convert.ToBoolean(ddr["PFLock"]);
                        fiscalYearDetailVM.TAXLock = Convert.ToBoolean(ddr["TAXLock"]);
                        fiscalYearDetailVM.LoanLock = Convert.ToBoolean(ddr["LoanLock"]);
                        fiscalYearDetailVM.Remarks = ddr["Remarks"].ToString();
                        fiscalYearDetailVMs.Add(fiscalYearDetailVM);

                    }
                    ddr.Close();
                }
                fiscalYearVM.FiscalYearDetailVM = fiscalYearDetailVMs;
                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return fiscalYearVM;
        }
        //==================SelectAll=================
        public List<FiscalYearVM> SelectAll(int BranchId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearVM> fiscalYearVMs = new List<FiscalYearVM>();
            FiscalYearVM fiscalYearVM;

            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
Id
,BranchId
,Year
,YearStart
,YearEnd
,YearLock
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

    From FiscalYear
where  BranchId=@BranchId
     
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@BranchId", BranchId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    fiscalYearVM = new FiscalYearVM();
                    fiscalYearVM.Id = dr["Id"].ToString();
                    fiscalYearVM.BranchId = Convert.ToInt32(dr["BranchId"]);

                    fiscalYearVM.Year = Convert.ToInt32(dr["Year"]);
                    fiscalYearVM.YearStart = Ordinary.StringToDate(dr["YearStart"].ToString());
                    fiscalYearVM.YearEnd = Ordinary.StringToDate(dr["YearEnd"].ToString());
                    fiscalYearVM.YearLock = Convert.ToBoolean(dr["YearLock"]);


                    fiscalYearVM.Remarks = dr["Remarks"].ToString();
                    fiscalYearVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    fiscalYearVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    fiscalYearVM.CreatedBy = dr["CreatedBy"].ToString();
                    fiscalYearVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    fiscalYearVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    fiscalYearVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    fiscalYearVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    fiscalYearVMs.Add(fiscalYearVM);
                }
                dr.Close();
                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return fiscalYearVMs;
        }
        //==================SelectByID=================
        public FiscalYearVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            FiscalYearVM fiscalYearVM = new FiscalYearVM();
            List<FiscalYearDetailVM> fiscalYearDetailVMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM fiscalYearDetailVM;

            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
Id
,BranchId
,Year
,YearStart
,YearEnd
,YearLock
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

    From FiscalYear
where  id=@Id
     
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    fiscalYearVM.Id = dr["Id"].ToString();
                    fiscalYearVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    fiscalYearVM.Year = Convert.ToInt32(dr["Year"]); ;
                    fiscalYearVM.YearStart = Ordinary.StringToDate(dr["YearStart"].ToString());
                    fiscalYearVM.YearEnd = Ordinary.StringToDate(dr["YearEnd"].ToString());
                    fiscalYearVM.YearLock = Convert.ToBoolean(dr["YearLock"]);
                    fiscalYearVM.Remarks = dr["Remarks"].ToString();
                    fiscalYearVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    fiscalYearVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    fiscalYearVM.CreatedBy = dr["CreatedBy"].ToString();
                    fiscalYearVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    fiscalYearVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    fiscalYearVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    fiscalYearVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

                }
                dr.Close();

                sqlText = @"SELECT
 Id
,FiscalYearId
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,PayrollLock
,PFLock
,TAXLock
,LoanLock
,Remarks
    From FiscalYearDetail
where  FiscalYearId=@FiscalYearId
";
                SqlCommand cmdDetails = new SqlCommand();
                cmdDetails.Connection = currConn;
                cmdDetails.CommandText = sqlText;
                cmdDetails.CommandType = CommandType.Text;
                cmdDetails.Parameters.AddWithValue("@FiscalYearId", fiscalYearVM.Id);

                using (SqlDataReader ddr = cmdDetails.ExecuteReader())
                {
                    while (ddr.Read())
                    {
                        fiscalYearDetailVM = new FiscalYearDetailVM();
                        fiscalYearDetailVM.Id = Convert.ToInt32(ddr["Id"]);
                        fiscalYearDetailVM.FiscalYearId = ddr["FiscalYearId"].ToString();
                        fiscalYearDetailVM.PeriodName = ddr["PeriodName"].ToString();
                        fiscalYearDetailVM.PeriodStart = Ordinary.StringToDate(ddr["PeriodStart"].ToString());
                        fiscalYearDetailVM.PeriodEnd = Ordinary.StringToDate(ddr["PeriodEnd"].ToString());
                        fiscalYearDetailVM.PeriodLock = Convert.ToBoolean(ddr["PeriodLock"]);
                        fiscalYearDetailVM.PayrollLock = Convert.ToBoolean(ddr["PayrollLock"]);
                        fiscalYearDetailVM.PFLock = Convert.ToBoolean(ddr["PFLock"]);
                        fiscalYearDetailVM.TAXLock = Convert.ToBoolean(ddr["TAXLock"]);
                        fiscalYearDetailVM.LoanLock = Convert.ToBoolean(ddr["LoanLock"]);
                        fiscalYearDetailVM.Remarks = ddr["Remarks"].ToString();
                        fiscalYearDetailVMs.Add(fiscalYearDetailVM);

                    }
                    ddr.Close();
                }
                fiscalYearVM.FiscalYearDetailVM = fiscalYearDetailVMs;
                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return fiscalYearVM;
        }

        public FiscalYearVM SelectByTransactionDate(string TransactionDate, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            FiscalYearVM fiscalYearVM = new FiscalYearVM();
            List<FiscalYearDetailVM> fiscalYearDetailVMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM fiscalYearDetailVM;

            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
Id
,BranchId
,Year
,YearStart
,YearEnd
,YearLock
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

    From FiscalYear
where @TransactionDate between YearStart and YearEnd
     
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@TransactionDate", TransactionDate);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    fiscalYearVM.Id = dr["Id"].ToString();
                    fiscalYearVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    fiscalYearVM.Year = Convert.ToInt32(dr["Year"]); ;
                    fiscalYearVM.YearStart = Ordinary.StringToDate(dr["YearStart"].ToString());
                    fiscalYearVM.YearEnd = Ordinary.StringToDate(dr["YearEnd"].ToString());
                    fiscalYearVM.YearLock = Convert.ToBoolean(dr["YearLock"]);
                    fiscalYearVM.Remarks = dr["Remarks"].ToString();
                    fiscalYearVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    fiscalYearVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    fiscalYearVM.CreatedBy = dr["CreatedBy"].ToString();
                    fiscalYearVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    fiscalYearVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    fiscalYearVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    fiscalYearVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

                }
                dr.Close();

                sqlText = @"SELECT
 Id
,FiscalYearId
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,PayrollLock
,PFLock
,TAXLock
,LoanLock
,Remarks
    From FiscalYearDetail
where  FiscalYearId=@FiscalYearId
";
                SqlCommand cmdDetails = new SqlCommand();
                cmdDetails.Connection = currConn;
                cmdDetails.CommandText = sqlText;
                cmdDetails.CommandType = CommandType.Text;
                cmdDetails.Parameters.AddWithValue("@FiscalYearId", fiscalYearVM.Id);

                using (SqlDataReader ddr = cmdDetails.ExecuteReader())
                {
                    while (ddr.Read())
                    {
                        fiscalYearDetailVM = new FiscalYearDetailVM();
                        fiscalYearDetailVM.Id = Convert.ToInt32(ddr["Id"]);
                        fiscalYearDetailVM.FiscalYearId = ddr["FiscalYearId"].ToString();
                        fiscalYearDetailVM.PeriodName = ddr["PeriodName"].ToString();
                        fiscalYearDetailVM.PeriodStart = Ordinary.StringToDate(ddr["PeriodStart"].ToString());
                        fiscalYearDetailVM.PeriodEnd = Ordinary.StringToDate(ddr["PeriodEnd"].ToString());
                        fiscalYearDetailVM.PeriodLock = Convert.ToBoolean(ddr["PeriodLock"]);
                        fiscalYearDetailVM.PayrollLock = Convert.ToBoolean(ddr["PayrollLock"]);
                        fiscalYearDetailVM.PFLock = Convert.ToBoolean(ddr["PFLock"]);
                        fiscalYearDetailVM.TAXLock = Convert.ToBoolean(ddr["TAXLock"]);
                        fiscalYearDetailVM.LoanLock = Convert.ToBoolean(ddr["LoanLock"]);
                        fiscalYearDetailVM.Remarks = ddr["Remarks"].ToString();
                        fiscalYearDetailVMs.Add(fiscalYearDetailVM);

                    }
                    ddr.Close();
                }
                fiscalYearVM.FiscalYearDetailVM = fiscalYearDetailVMs;
                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return fiscalYearVM;
        }

        public FiscalYearDetailVM PeriodLockByTransactionDate(string TransactionDate, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            FiscalYearDetailVM fiscalYearDetailVM = new FiscalYearDetailVM();

            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
 Id
,FiscalYearId
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,PayrollLock
,PFLock
,TAXLock
,LoanLock
,Remarks
    From FiscalYearDetail
where @TransactionDate between PeriodStart and PeriodEnd

";
                SqlCommand cmdDetails = new SqlCommand();
                cmdDetails.Connection = currConn;
                cmdDetails.Transaction = transaction;
                cmdDetails.CommandText = sqlText;
                cmdDetails.CommandType = CommandType.Text;
                cmdDetails.Parameters.AddWithValue("@TransactionDate", TransactionDate);

                using (SqlDataReader ddr = cmdDetails.ExecuteReader())
                {
                    while (ddr.Read())
                    {
                        fiscalYearDetailVM = new FiscalYearDetailVM();
                        fiscalYearDetailVM.Id = Convert.ToInt32(ddr["Id"]);
                        fiscalYearDetailVM.FiscalYearId = ddr["FiscalYearId"].ToString();
                        fiscalYearDetailVM.PeriodName = ddr["PeriodName"].ToString();
                        fiscalYearDetailVM.PeriodStart = Ordinary.StringToDate(ddr["PeriodStart"].ToString());
                        fiscalYearDetailVM.PeriodEnd = Ordinary.StringToDate(ddr["PeriodEnd"].ToString());
                        fiscalYearDetailVM.PeriodLock = Convert.ToBoolean(ddr["PeriodLock"]);
                        fiscalYearDetailVM.PayrollLock = Convert.ToBoolean(ddr["PayrollLock"]);
                        fiscalYearDetailVM.PFLock = Convert.ToBoolean(ddr["PFLock"]);
                        fiscalYearDetailVM.TAXLock = Convert.ToBoolean(ddr["TAXLock"]);
                        fiscalYearDetailVM.LoanLock = Convert.ToBoolean(ddr["LoanLock"]);
                        fiscalYearDetailVM.Remarks = ddr["Remarks"].ToString();

                    }
                    ddr.Close();
                }
                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return fiscalYearDetailVM;
        }


        public string[] FiscalYearInsert(FiscalYearVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction, bool callFromOutSide)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertFiscalYear"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) FROM FiscalYear ";
                sqlText += " WHERE BranchId=@BranchId And Year=@Year";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdExist.Parameters.AddWithValue("@Year", vm.Year);
                object objfoundId = cmdExist.ExecuteScalar();

                if ((int)objfoundId > 0)
                {
                    throw new ArgumentNullException("This fiscal year already used!", "");
                }

                #endregion Exist
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from FiscalYear where BranchId=@BranchId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                vm.Id = vm.BranchId.ToString() + "_" + (count + 1);
                #region Save
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO FiscalYear(

Id
,BranchId
,Year
,YearStart
,YearEnd
,YearLock
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
 @Id
,@BranchId
,@Year
,@YearStart
,@YearEnd
,@YearLock
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom)";
                    SqlCommand _cmdInsert = new SqlCommand(sqlText, currConn);
                    _cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    _cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    _cmdInsert.Parameters.AddWithValue("@Year", vm.Year);
                    _cmdInsert.Parameters.AddWithValue("@YearStart", Ordinary.DateToString(vm.YearStart));
                    _cmdInsert.Parameters.AddWithValue("@YearEnd", Ordinary.DateToString(vm.YearEnd));
                    _cmdInsert.Parameters.AddWithValue("@YearLock", vm.YearLock);
                    _cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    _cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    _cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    _cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    _cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    _cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    _cmdInsert.Transaction = transaction;
                    _cmdInsert.ExecuteNonQuery();

                    SqlCommand cmdDetails;
                    sqlText = @" INSERT INTO FiscalYearDetail(

 FiscalYearId
,Year
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,PayrollLock
,PFLock
,TAXLock
,LoanLock
,SagePostComplete
,Remarks
) VALUES (
 @FiscalYearId
,@Year
,@PeriodName
,@PeriodStart
,@PeriodEnd
,@PeriodLock
,@PayrollLock
,@PFLock
,@TAXLock
,@LoanLock
,0
,@Remarks
)";
                    foreach (FiscalYearDetailVM item in vm.FiscalYearDetailVM)
                    {
                        cmdDetails = new SqlCommand(sqlText, currConn);
                        cmdDetails.Parameters.AddWithValue("@FiscalYearId", vm.Id);
                        cmdDetails.Parameters.AddWithValue("@Year", vm.Year);
                        cmdDetails.Parameters.AddWithValue("@PeriodName", item.PeriodName);
                        cmdDetails.Parameters.AddWithValue("@PeriodStart", Ordinary.DateToString(item.PeriodStart));
                        cmdDetails.Parameters.AddWithValue("@PeriodEnd", Ordinary.DateToString(item.PeriodEnd));
                        cmdDetails.Parameters.AddWithValue("@PeriodLock", item.PeriodLock);
                        cmdDetails.Parameters.AddWithValue("@PayrollLock", item.PeriodLock);
                        cmdDetails.Parameters.AddWithValue("@PFLock", item.PeriodLock);
                        cmdDetails.Parameters.AddWithValue("@TAXLock", item.PeriodLock);
                        cmdDetails.Parameters.AddWithValue("@LoanLock", item.PeriodLock);
                        cmdDetails.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
                        cmdDetails.Transaction = transaction;
                        cmdDetails.ExecuteNonQuery();
                    }
                }

                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }

            #endregion try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }




            #endregion
            #region Results
            return retResults;
            #endregion


        }
        public string[] FiscalYearUpdate(FiscalYearVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = vm.Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "UpdateFiscalYear"; //Method Name
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Exist
                sqlText = "  ";
                sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) FROM FiscalYear ";
                sqlText += " WHERE BranchId=@BranchId And Year=@Year and Id<>@Id";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                cmdExist.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdExist.Parameters.AddWithValue("@Year", vm.Year);
                object objfoundId = cmdExist.ExecuteScalar();

                if ((int)objfoundId > 0)
                {
                    throw new ArgumentNullException("This fiscal year already used!", "");
                }

                #endregion Exist
                #region Save
                if (vm != null)
                {

                    sqlText = "  ";
                    sqlText = @" Update FiscalYear set


YearLock    =@YearLock
,Remarks     =@Remarks
,LastUpdateBy   =@LastUpdateBy
,LastUpdateAt   =@LastUpdateAt
,LastUpdateFrom =@LastUpdateFrom
where Id=@Id
";


                    SqlCommand _cmdUpdate = new SqlCommand(sqlText, currConn);
                    _cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                    _cmdUpdate.Parameters.AddWithValue("@YearLock", vm.YearLock);
                    _cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    _cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    _cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    _cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    _cmdUpdate.Transaction = transaction;
                    _cmdUpdate.ExecuteNonQuery();

                    SqlCommand cmdDetails;
                    foreach (FiscalYearDetailVM item in vm.FiscalYearDetailVM)
                    {
                        sqlText = @" Update FiscalYearDetail set


PeriodLock =@PeriodLock
,PayrollLock=@PayrollLock
,PFLock=@PFLock
,TAXLock=@TAXLock
,LoanLock=@LoanLock
,Remarks=@Remarks
where Id=@Id
";
                        cmdDetails = new SqlCommand(sqlText, currConn);
                        cmdDetails.Parameters.AddWithValue("@Id", item.Id);
                        cmdDetails.Parameters.AddWithValue("@PeriodLock", item.PeriodLock);
                        cmdDetails.Parameters.AddWithValue("@PayrollLock", item.PayrollLock);
                        cmdDetails.Parameters.AddWithValue("@PFLock", item.PFLock);
                        cmdDetails.Parameters.AddWithValue("@TAXLock", item.TAXLock);
                        cmdDetails.Parameters.AddWithValue("@LoanLock", item.LoanLock);
                        cmdDetails.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
                        cmdDetails.Transaction = transaction;
                        cmdDetails.ExecuteNonQuery();
                    }
                }
                #endregion Save
                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }


                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall



            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex

                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }


            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public DataTable LoadYear(string CurrentYear)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Year");
            #endregion

            #region try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement
                sqlText = @"
                            SELECT    
FiscalYear,
CurrentYear,
PeriodID,
PeriodName,
convert(varchar, PeriodStart,120)PeriodStart,
convert(varchar, PeriodEnd,120)PeriodEnd, 
PeriodLock,
isnull(GLLock,'N')GLLock 

FROM         FiscalYear
WHERE 	(CurrentYear  =  @CurrentYear ) 

ORDER BY PeriodStart";

                SqlCommand objCommYear = new SqlCommand();
                objCommYear.Connection = currConn;
                objCommYear.CommandText = sqlText;
                objCommYear.CommandType = CommandType.Text;

                if (!objCommYear.Parameters.Contains("@CurrentYear"))
                {
                    objCommYear.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                }
                else
                {
                    objCommYear.Parameters["@CurrentYear"].Value = CurrentYear;
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommYear);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //throw ex;
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return dataTable;
        }
        public List<FiscalYearVM> DropDownYear(int branch)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearVM> VMs = new List<FiscalYearVM>();
            FiscalYearVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
Id,
Year Name
   FROM FiscalYear
WHERE IsArchive=0 and IsActive=1 and BranchId=@branch
    ORDER BY Year desc
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@branch", branch);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();


                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return VMs;
        }


        #region FiscalYearDetail

        public List<FiscalYearDetailVM> FYPeriodDetail(int Id = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            

            #endregion
            try
            {
                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
*
   FROM FiscalYearDetail
 where id=@Id
    ORDER BY id
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@Id", Id);

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new FiscalYearDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.FiscalYearId = dr["FiscalYearId"].ToString();

                        vm.Year = Convert.ToInt32(dr["Year"]);
                        vm.PeriodName = dr["PeriodName"].ToString();
                        vm.Name = dr["PeriodName"].ToString();
                         vm.PeriodId = dr["PeriodId"].ToString();
                        vm.PeriodStart = dr["PeriodStart"].ToString();
                        vm.PeriodEnd = dr["PeriodEnd"].ToString();
                        vm.PeriodLock = Convert.ToBoolean(dr["PeriodLock"].ToString());
                        vm.PayrollLock = Convert.ToBoolean(dr["PayrollLock"]);
                        vm.PFLock = Convert.ToBoolean(dr["PFLock"]);
                        vm.TAXLock = Convert.ToBoolean(dr["TAXLock"]);
                        vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                        vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                        vm.SagePostComplete = Convert.ToBoolean(dr["SagePostComplete"]);
                        VMs.Add(vm);
                    }
                    dr.Close();

                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }


            #endregion

            return VMs;
        }

        public FiscalYearDetailVM FYPeriodDetailPeriodId(string PeriodId = "0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            FiscalYearDetailVM vm = new FiscalYearDetailVM();
            

            #endregion
            try
            {
                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
*
   FROM FiscalYearDetail
 where PeriodId=@PeriodId
    ORDER BY id
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@PeriodId", PeriodId);

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new FiscalYearDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.FiscalYearId = dr["FiscalYearId"].ToString();

                        vm.Year = Convert.ToInt32(dr["Year"]);
                        vm.PeriodName = dr["PeriodName"].ToString();
                        vm.Name = dr["PeriodName"].ToString();
                        vm.PeriodId = dr["PeriodId"].ToString();
                        
                        vm.PeriodStart = dr["PeriodStart"].ToString();
                        vm.PeriodEnd = dr["PeriodEnd"].ToString();
                        vm.PeriodLock = Convert.ToBoolean(dr["PeriodLock"].ToString());
                        vm.PayrollLock = Convert.ToBoolean(dr["PayrollLock"]);
                        vm.PFLock = Convert.ToBoolean(dr["PFLock"]);
                        vm.TAXLock = Convert.ToBoolean(dr["TAXLock"]);
                        vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                        vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                        vm.SagePostComplete = Convert.ToBoolean(dr["SagePostComplete"]);
                    }
                    dr.Close();

               
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }


            #endregion

            return vm;
        }
        public FiscalYearDetailVM FYPeriodDetail(int Id=0, string PeriodId ="0", string Date = "19000101", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            FiscalYearDetailVM vm = new FiscalYearDetailVM();
            

            #endregion
            try
            {
                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
*
   FROM FiscalYearDetail

   where 1=1 
";
                if (Id>0)
                {
                    sqlText += @"and  Id=@Id";
                    
                }
                else if (PeriodId!="")
                {
                    sqlText += @"and  PeriodId=@PeriodId";

                }
                else if (Date != "19000101")
                {
                    sqlText += @"and  @Date between PeriodStart and PeriodEnd";
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);

                }
                else if (PeriodId != "")
                {
                    objComm.Parameters.AddWithValue("@PeriodId", PeriodId);

                }
                else if (Date != "19000101")
                {
                    objComm.Parameters.AddWithValue("@Date", Date);
                }


                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new FiscalYearDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.FiscalYearId = dr["FiscalYearId"].ToString();
                        vm.Year = Convert.ToInt32(dr["Year"]);
                        vm.PeriodName = dr["PeriodName"].ToString();
                        vm.Name = dr["PeriodName"].ToString();
                        vm.PeriodId = dr["PeriodId"].ToString();

                        vm.PeriodStart = dr["PeriodStart"].ToString();
                        vm.PeriodEnd = dr["PeriodEnd"].ToString();
                        vm.PeriodLock = Convert.ToBoolean(dr["PeriodLock"].ToString());
                        vm.PayrollLock = Convert.ToBoolean(dr["PayrollLock"]);
                        vm.PFLock = Convert.ToBoolean(dr["PFLock"]);
                        vm.TAXLock = Convert.ToBoolean(dr["TAXLock"]);
                        vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                        vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                        vm.SagePostComplete = Convert.ToBoolean(dr["SagePostComplete"]);
                    }
                    dr.Close();


                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }


            #endregion

            return vm;
        }
        public DataTable FYPeriodDetailDt(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            

            #endregion
            try
            {
                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
*
   FROM FiscalYearDetail

   where 1=1 
";
                if (Id > 0)
                {
                    sqlText += @"and  Id=@Id";
                }
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                #endregion sql statement

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", Id);
                }

                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                da.Fill(dt);



                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

            }
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }


            #endregion

            return dt;
        }




        public List<FiscalYearDetailVM> FYPeriodDetailByPeriodName(string PeriodName = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
*
   FROM FiscalYearDetail
 where PeriodName=@PeriodName
    ORDER BY id
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@PeriodName", PeriodName);

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new FiscalYearDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.FiscalYearId = dr["FiscalYearId"].ToString();

                        vm.Year = Convert.ToInt32(dr["Year"]);
                        vm.PeriodName = dr["PeriodName"].ToString();
                        vm.Name = dr["PeriodName"].ToString();
                        vm.PeriodStart = dr["PeriodStart"].ToString();
                        vm.PeriodEnd = dr["PeriodEnd"].ToString();
                        vm.PeriodLock = Convert.ToBoolean(dr["PeriodLock"].ToString());
                        vm.PayrollLock = Convert.ToBoolean(dr["PayrollLock"]);
                        vm.PFLock = Convert.ToBoolean(dr["PFLock"]);
                        vm.TAXLock = Convert.ToBoolean(dr["TAXLock"]);
                        vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                        vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                        vm.SagePostComplete = Convert.ToBoolean(dr["SagePostComplete"]);
                        VMs.Add(vm);
                    }
                    dr.Close();

                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }


            #endregion

            return VMs;
        }


        public List<FiscalYearDetailVM> DropDownPeriod(int branch)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
Id,
PeriodName Name
   FROM FiscalYearDetail
 
    ORDER BY PeriodName
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();


                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return VMs;
        }
        public List<FiscalYearDetailVM> DropDownPeriod(int branch, int FiscalYearId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
Id,
PeriodName Name
   FROM FiscalYearDetail
WHERE IsArchive=0 and IsActive=1 and BranchId=@branch  and FiscalYearId=@FiscalYearId
    ORDER BY Year
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@branch", branch);
                _objComm.Parameters.AddWithValue("@FiscalYearId", FiscalYearId);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();


                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return VMs;
        }
        public List<FiscalYearDetailVM> DropDownPeriodByYear(int branch, int year)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
 fd.Id,fd.PeriodName
from FiscalYearDetail fd 
left join FiscalYear f on f.Id= fd.FiscalYearId where f.BranchId=@BranchId and fd.Year=@Year
    ORDER BY fd.Id
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", branch);
                _objComm.Parameters.AddWithValue("@Year", year);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["PeriodName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();


                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return VMs;
        }
        public List<FiscalYearDetailVM> DropDownPeriodByYearLockPayroll(int branch, int year)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
 fd.Id,fd.PeriodName
from FiscalYearDetail fd 
left join FiscalYear f on f.Id= fd.FiscalYearId WHERE fd.PayrollLock=0 and  f.BranchId=@BranchId and fd.Year=@Year
ORDER BY fd.Id";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", branch);
                _objComm.Parameters.AddWithValue("@Year", year);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["PeriodName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();


                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return VMs;
        }

        public List<FiscalYearDetailVM> DropDownPeriodByYearLockPayroll_All(int branch, int year)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
 fd.Id,fd.PeriodName
from FiscalYearDetail fd 
left join FiscalYear f on f.Id= fd.FiscalYearId WHERE fd.PayrollLock=0 and  f.BranchId=@BranchId and fd.Year in ("+ year + ","+ (year-1) +@") 
ORDER BY fd.Id";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", branch);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["PeriodName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();


                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return VMs;
        }


        public List<FiscalYearDetailVM> DropDownPeriodByYearLockTax(int branch, int year)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
 fd.Id,fd.PeriodName
from FiscalYearDetail fd 
left join FiscalYear f on f.Id= fd.FiscalYearId WHERE fd.TAXLock=0 and  f.BranchId=@BranchId and fd.Year=@Year
ORDER BY fd.Id";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", branch);
                _objComm.Parameters.AddWithValue("@Year", year);
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["PeriodName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }
        public List<FiscalYearDetailVM> DropDownPeriodByYearLockPF(int branch, int year)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
 fd.Id,fd.PeriodName
from FiscalYearDetail fd 
left join FiscalYear f on f.Id= fd.FiscalYearId WHERE fd.PFLock=0 and  f.BranchId=@BranchId and fd.Year=@Year
ORDER BY fd.Id";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", branch);
                _objComm.Parameters.AddWithValue("@Year", year);
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["PeriodName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }
        public List<FiscalYearDetailVM> DropDownPeriodByYearLockLoan(int branch, int year)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
 fd.Id,fd.PeriodName
from FiscalYearDetail fd 
left join FiscalYear f on f.Id= fd.FiscalYearId WHERE fd.LoanLock=0 and f.BranchId=@BranchId and fd.Year=@Year
ORDER BY fd.Id";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", branch);
                _objComm.Parameters.AddWithValue("@Year", year);
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["PeriodName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }
        public List<FiscalYearDetailVM> DropDownPeriodNext(int branch, int currentId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
 fd.Id,fd.PeriodName
from FiscalYearDetail fd 
left join FiscalYear f on f.Id= fd.FiscalYearId where  fd.Id>=@currentId  and f.BranchId=@BranchId  
    ORDER BY fd.Id
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", branch);
                _objComm.Parameters.AddWithValue("@currentId", currentId);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["PeriodName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();


                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return VMs;
        }

        public int FiscalPeriodIdByDate(string FiscalYearDetailId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            int FiscalPeriodId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                sqlText = @"select top 1 PeriodStart from FiscalYearDetail
                            where id=@FiscalPeriodId ";
                SqlCommand cmdfy = new SqlCommand(sqlText, currConn, transaction);
                cmdfy.Parameters.AddWithValue("@FiscalPeriodId", FiscalPeriodId);
                using (SqlDataReader dr = cmdfy.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        FiscalPeriodId = Convert.ToInt32(dr["Id"]);
                    }
                    dr.Close();
                }
            }
            #region Catch and Finall



            catch (Exception ex)
            {
                //retResults[4] = ex.Message.ToString(); //catch ex

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
                return FiscalPeriodId;
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }


            #endregion

            return FiscalPeriodId;
        }
        public string FiscalPeriodStartDate(int FiscalPeriodId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string FiscalPeriodStartDate = "19000101";
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            
            #endregion
            try
            {
                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                sqlText = @"select top 1 PeriodStart from  FiscalYearDetail
                            where id=@FiscalPeriodId ";
                SqlCommand cmdfy = new SqlCommand(sqlText, currConn, transaction);
                cmdfy.Parameters.AddWithValue("@FiscalPeriodId", FiscalPeriodId);
                using (SqlDataReader dr = cmdfy.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        FiscalPeriodStartDate = dr["PeriodStart"].ToString();
                    }
                    dr.Close();
                }
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
                return FiscalPeriodStartDate;
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }


            #endregion

            return FiscalPeriodStartDate;
        }
      
      
        public List<string> Autocomplete(string term)
        {
            #region Variables

            SqlConnection currConn = null;
            List<string> vms = new List<string>();

            string sqlText = "";

            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction
                #region sql statement
                sqlText = @"SELECT Id,Year Name    FROM FiscalYear ";
                sqlText += @" WHERE Year like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY Year";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["Name"].ToString());
                    i++;
                }
                dr.Close();
                vms.Sort();
                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion
            return vms;
        }
        public string[] UpateFiscalYearDetail(FiscalYearDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region Validation
                #endregion Validation
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save
                sqlText = @" Update FiscalYearDetail set
PeriodLock =@PeriodLock
,PayrollLock =@PayrollLock
,PFLock=@PFLock
,TAXLock=@TAXLock
,LoanLock=@LoanLock
,Remarks=@Remarks
where Id=@Id
";
                SqlCommand cmdDetails;
                cmdDetails = new SqlCommand(sqlText, currConn);
                cmdDetails.Parameters.AddWithValue("@Id", vm.Id);
                cmdDetails.Parameters.AddWithValue("@PeriodLock", vm.PeriodLock);
                cmdDetails.Parameters.AddWithValue("@PayrollLock", vm.PayrollLock);
                cmdDetails.Parameters.AddWithValue("@PFLock", vm.PFLock);
                cmdDetails.Parameters.AddWithValue("@TAXLock", vm.TAXLock);
                cmdDetails.Parameters.AddWithValue("@LoanLock", vm.LoanLock);
                cmdDetails.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                cmdDetails.Transaction = transaction;
                int sq = cmdDetails.ExecuteNonQuery();
                if (sq == 1)
                {
                    sqlText = @"SELECT
 Id
,FiscalYearId
,PeriodName
,PeriodStart
,PeriodEnd
,PayrollLock
,Remarks
    From FiscalYearDetail
where  Id=@Id
";

                    SqlCommand cmdLoanUpd = new SqlCommand();
                    cmdLoanUpd.Connection = currConn;
                    cmdLoanUpd.CommandText = sqlText;
                    cmdLoanUpd.CommandType = CommandType.Text;
                    cmdLoanUpd.Parameters.AddWithValue("@Id", vm.Id);
                    cmdLoanUpd.Transaction = transaction;

                    using (SqlDataReader ddr = cmdLoanUpd.ExecuteReader())
                    {
                        while (ddr.Read())
                        {
                            vm = new FiscalYearDetailVM();
                            vm.Id = Convert.ToInt32(ddr["Id"]);
                            vm.FiscalYearId = ddr["FiscalYearId"].ToString();
                            vm.PeriodStart = ddr["PeriodStart"].ToString();
                            vm.PayrollLock = Convert.ToBoolean(ddr["PayrollLock"]);
                            vm.PeriodEnd = ddr["PeriodEnd"].ToString();
                        }
                        ddr.Close();
                    }
                    EmployeeLoanDAL EDAL = new EmployeeLoanDAL();
                    retResults = EDAL.LoanUpdatePaid(vm.PeriodStart, vm.PeriodEnd, vm.PayrollLock, null, null);
                    //if (retResults[0]=="Success")
                    //{

                    //}

                }
                #endregion
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = "0";
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        //==================SelectDaysOfMonth=================
        public int SelectDaysOfMonth(int fydId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            int DaysOfMonth = 0;
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = "";
                sqlText = @"
declare @StartDate as varchar(20)  
declare @EndDate as varchar(20)  

--declare @FiscalYearDetailId as varchar(20)
--set @FiscalYearDetailId = 1009
declare @DOM as varchar(50)
select @DOM=settingValue from setting where SettingGroup='DOM' and SettingName='DOM'

select @StartDate=periodStart,@EndDate=PeriodEnd from FiscalYearDetail
where id=@FiscalYearDetailId

select @DOM= case when @DOM='DOM' then CONVERT(int,@EndDate)-CONVERT(int,@StartDate)+1 else 30 end

select @DOM DaysOfMonth

";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                objComm.Parameters.AddWithValue("@FiscalYearDetailId", fydId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    DaysOfMonth = Convert.ToInt32(dr["DaysOfMonth"]);
                }
                dr.Close();
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return DaysOfMonth;
        }

        public List<FiscalYearDetailVM> SelectAll_FiscalYearDetail(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                #region SqlText

                sqlText = @"
SELECT
 Id
,FiscalYearId
,Year
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,PayrollLock
,PFLock
,TAXLock
,LoanLock
,SagePostComplete
,Remarks
FROM FiscalYearDetail  
WHERE  1=1

";


                if (Id > 0)
                {
                    sqlText += @" and Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {

                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearId = Convert.ToString(dr["FiscalYearId"]);
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.PeriodName = Convert.ToString(dr["PeriodName"]);
                    vm.PeriodStart = Convert.ToString(dr["PeriodStart"]);
                    vm.PeriodEnd = Convert.ToString(dr["PeriodEnd"]);
                    vm.PeriodLock = Convert.ToBoolean(dr["PeriodLock"]);
                    vm.PayrollLock = Convert.ToBoolean(dr["PayrollLock"]);
                    vm.PFLock = Convert.ToBoolean(dr["PFLock"]);
                    vm.TAXLock = Convert.ToBoolean(dr["TAXLock"]);
                    vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                    vm.SagePostComplete = Convert.ToBoolean(dr["SagePostComplete"]);
                    vm.Remarks = Convert.ToString(dr["Remarks"]);

                    VMs.Add(vm);
                }
                dr.Close();

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }

        public FiscalYearDetailVM SelectAll_FiscalYearDetailByDate(string date, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            FiscalYearDetailVM vm = new FiscalYearDetailVM();
            

            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                #region SqlText
                                sqlText = @"
                select TOP 1 * from  FiscalYearDetail
                where  PeriodEnd>=@date";
                
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@date",Ordinary.DateToString(date));
 
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {

                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearId = Convert.ToString(dr["FiscalYearId"]);
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.PeriodName = Convert.ToString(dr["PeriodName"]);
                    vm.PeriodStart = Convert.ToString(dr["PeriodStart"]);
                    vm.PeriodEnd = Convert.ToString(dr["PeriodEnd"]);
                    vm.PeriodLock = Convert.ToBoolean(dr["PeriodLock"]);
                    vm.PayrollLock = Convert.ToBoolean(dr["PayrollLock"]);
                    vm.PFLock = Convert.ToBoolean(dr["PFLock"]);
                    vm.TAXLock = Convert.ToBoolean(dr["TAXLock"]);
                    vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                    vm.SagePostComplete = Convert.ToBoolean(dr["SagePostComplete"]);
                    vm.Remarks = Convert.ToString(dr["Remarks"]);

                }
                dr.Close();

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return vm;
        }

        public List<FiscalYearDetailVM> SelectAll_PreviousFiscalPeriod(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion

            #region try
            
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region sql statement

                #region SqlText

                sqlText = @"
SELECT TOP 1 
 Id
,FiscalYearId
,Year
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,PayrollLock
,PFLock
,TAXLock
,LoanLock
,SagePostComplete
,Remarks
FROM FiscalYearDetail  
WHERE  1=1
AND PeriodLock = 0
AND Id<@Id
ORDER BY Id DESC
";

                #endregion

                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                    objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearId = Convert.ToString(dr["FiscalYearId"]);
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.PeriodName = Convert.ToString(dr["PeriodName"]);
                    vm.PeriodStart = Convert.ToString(dr["PeriodStart"]);
                    vm.PeriodEnd = Convert.ToString(dr["PeriodEnd"]);
                    vm.PeriodLock = Convert.ToBoolean(dr["PeriodLock"]);
                    vm.PayrollLock = Convert.ToBoolean(dr["PayrollLock"]);
                    vm.PFLock = Convert.ToBoolean(dr["PFLock"]);
                    vm.TAXLock = Convert.ToBoolean(dr["TAXLock"]);
                    vm.LoanLock = Convert.ToBoolean(dr["LoanLock"]);
                    vm.SagePostComplete = Convert.ToBoolean(dr["SagePostComplete"]);
                    vm.Remarks = Convert.ToString(dr["Remarks"]);
                    VMs.Add(vm);
                }
                dr.Close();

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion

            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return VMs;
        }


        public DataTable SelectFromToId(string year, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<FiscalYearDetailVM> VMs = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM vm;
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region sql statement

                #region SqlText

                sqlText = @"
SELECT min(id)FiscalYearDetailId, max(Id)FiscalYearDetailIdTo
FROM FiscalYearDetail  
WHERE  1=1
and Year = @year
";

                #endregion

                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@year", year);
                SqlDataAdapter adapter = new SqlDataAdapter(objComm);
                DataTable dtResult = new DataTable();
                adapter.Fill(dtResult);

                #endregion SqlExecution

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }
                #endregion

                return dtResult;
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion

            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
        }



        public bool FiscalPeriodLockCheck(int FiscalYearDetailId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            bool PeriodLock;
            
            #endregion

            try
            {
                FiscalYearDetailVM vm = new FiscalYearDetailVM();
                
                vm = SelectAll_FiscalYearDetail(FiscalYearDetailId).FirstOrDefault();

                PeriodLock = vm.PeriodLock;
               
            }

            #region Catch and Finall

            catch (Exception ex)
            {
                throw ex;
            }

            
            #endregion

            return PeriodLock;
        }


        #endregion


        public string FiscalPeriodEndDate(int FiscalPeriodId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string FiscalPeriodEndDate = "19000101";
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            
            #endregion
            try
            {
                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                sqlText = @"select top 1 PeriodEnd from  FiscalYearDetail
                            where id=@FiscalPeriodId ";
                SqlCommand cmdfy = new SqlCommand(sqlText, currConn, transaction);
                cmdfy.Parameters.AddWithValue("@FiscalPeriodId", FiscalPeriodId);
                using (SqlDataReader dr = cmdfy.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        FiscalPeriodEndDate = dr["PeriodEnd"].ToString();
                    }
                    dr.Close();
                }
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
                return FiscalPeriodEndDate;
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }


            #endregion

            return FiscalPeriodEndDate;
        }
    }
}
