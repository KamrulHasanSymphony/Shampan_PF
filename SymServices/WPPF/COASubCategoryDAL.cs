using SymOrdinary;
using SymServices.Common;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.WPPF
{
    public class COASubCategoryDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods

        public List<COAGroupVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<COAGroupVM> VMs = new List<COAGroupVM>();
            COAGroupVM vm;
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
                sqlText = @"
SELECT 
 a.Id
,a.GroupId
,a.SubGroupId
,a.CategoryId
,g.Name AS GroupName
,s.SubGroupName AS SubGroupName
,c.CategoryName AS CategoryName
,a.SubCategoryName
,isnull(a.Remarks,'') Remarks
,a.IsActive
,a.IsArchive
,a.CreatedBy
,a.CreatedAt
,a.CreatedFrom
,a.LastUpdateBy
,a.LastUpdateAt
,a.LastUpdateFrom
FROM COASubCategories a
left outer join COAGroups g on a.GroupId = g.Id
left outer join COASubGroups s on a.SubGroupId = s.Id
left outer join COACategories c on a.CategoryId = c.Id
WHERE a.IsArchive = 0
";

                if (Id > 0)
                {
                    sqlText += " AND a.Id = @Id";
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
                        sqlText += " AND " + conditionFields[i] + " = @" + cField;
                    }
                }

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

                SqlDataReader dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new COAGroupVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.COAGroupId = dr["GroupId"].ToString();
                    vm.COASubGroupId = dr["SubGroupId"].ToString();
                    vm.COACategoryId = dr["CategoryId"].ToString();
                    vm.GroupName = dr["GroupName"].ToString();
                    vm.SubGroupName = dr["SubGroupName"].ToString();
                    vm.CategoryName = dr["CategoryName"].ToString();
                    vm.Name = dr["SubCategoryName"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

        public string[] Insert(COAGroupVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertCOASubCategories"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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

                #region Save
                vm.Id = _cDal.NextId("COASubCategories", currConn, transaction);
                if (vm != null)
                {
                    sqlText = @"INSERT INTO COASubCategories
                                (GroupId, SubGroupId, CategoryId, SubCategoryName, Remarks, IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom)
                                VALUES
                                (@GroupId, @SubGroupId, @CategoryId, @SubCategoryName, @Remarks, @IsActive, @IsArchive, @CreatedBy, @CreatedAt, @CreatedFrom)";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                    cmdInsert.Parameters.AddWithValue("@GroupId", vm.COAGroupId);
                    cmdInsert.Parameters.AddWithValue("@SubGroupId", vm.COASubGroupId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CategoryId", vm.COACategoryId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@SubCategoryName", vm.Name ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdInsert.ExecuteNonQuery();
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
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message.ToString();
                if (Vtransaction == null && transaction != null) { transaction.Rollback(); }
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

            return retResults;
        }

        public string[] Update(COAGroupVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "COASubCategories Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("COASubCategories"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE COASubCategories SET ";
                    sqlText += " GroupId = @GroupId";
                    sqlText += " , SubGroupId = @SubGroupId";
                    sqlText += " , CategoryId = @CategoryId";
                    sqlText += " , SubCategoryName = @SubCategoryName";
                    sqlText += " , Remarks = @Remarks";
                    sqlText += " , IsActive = @IsActive";
                    sqlText += " , LastUpdateBy = @LastUpdateBy";
                    sqlText += " , LastUpdateAt = @LastUpdateAt";
                    sqlText += " , LastUpdateFrom = @LastUpdateFrom";
                    sqlText += " WHERE Id = @Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@GroupId", vm.COAGroupId);
                    cmdUpdate.Parameters.AddWithValue("@SubGroupId", vm.COASubGroupId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@CategoryId", vm.COACategoryId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@SubCategoryName", vm.Name ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    if (transResult <= 0)
                    {
                        // handle no rows affected if needed
                    }
                    #endregion Update Settings

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("COASubCategories Update", "Could not found any item.");
                }

                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to update COASubCategories.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message;
                if (Vtransaction == null && transaction != null) { transaction.Rollback(); }
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

            return retResults;
        }

        #endregion
    }
}
