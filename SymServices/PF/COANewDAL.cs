using SymOrdinary;
using SymServices.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.PF
{
    public class COANewDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region DropDown
        public List<COANewVM> DropDown(string TransType = "PF", string BranchId = "")
        {
            SqlConnection currConn = null;
            string sqlText = "";
            List<COANewVM> VMs = new List<COANewVM>();
            COANewVM vm;
            try
            {
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open) currConn.Open();

                sqlText = @"
SELECT Id, '[ '+Code+' ] '+ Name Name
FROM COAs
WHERE 1=1 AND TransType=@TransType AND BranchId=@BranchId
ORDER BY Name";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@TransType", TransType);
                objComm.Parameters.AddWithValue("@BranchId", BranchId);

                SqlDataReader dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new COANewVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
            }
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open) currConn.Close();
            }
            return VMs;
        }

        // Dropdown for ParentCOA selection (excludes self when editing)
        public List<COANewVM> ParentCOADropDown(string TransType = "PF", string BranchId = "", int excludeId = 0)
        {
            SqlConnection currConn = null;
            string sqlText = "";
            List<COANewVM> VMs = new List<COANewVM>();
            COANewVM vm;
            try
            {
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open) currConn.Open();

                sqlText = @"
SELECT Id, '[ '+Code+' ] '+ Name Name
FROM COAs
WHERE IsArchive=0 AND TransType=@TransType AND BranchId=@BranchId";

                if (excludeId > 0)
                    sqlText += " AND Id <> @ExcludeId";

                sqlText += " ORDER BY Code";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@TransType", TransType);
                objComm.Parameters.AddWithValue("@BranchId", BranchId);
                if (excludeId > 0)
                    objComm.Parameters.AddWithValue("@ExcludeId", excludeId);

                SqlDataReader dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new COANewVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
            }
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open) currConn.Close();
            }
            return VMs;
        }

        public List<COANewVM> COATypeDropDown()
        {
            SqlConnection currConn = null;
            string sqlText = "";
            List<COANewVM> VMs = new List<COANewVM>();
            COANewVM vm;
            try
            {
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open) currConn.Open();

                sqlText = @"SELECT Id, Name FROM COAType WHERE 1=1 ORDER BY Id";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                SqlDataReader dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new COANewVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
            }
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open) currConn.Close();
            }
            return VMs;
        }
        #endregion

        #region SelectAll
        public List<COANewVM> SelectAll(string branchId, int Id = 0,
            string[] conditionFields = null, string[] conditionValues = null,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<COANewVM> VMs = new List<COANewVM>();
            COANewVM vm;
            try
            {
                if (VcurrConn != null) currConn = VcurrConn;
                if (Vtransaction != null) transaction = Vtransaction;
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open) currConn.Open();
                }
                if (transaction == null) transaction = currConn.BeginTransaction("");

                sqlText = @"
SELECT
 COAs.Id
,COAs.COAGroupId
,COAs.Code
,COAs.Name
,g.Name GroupName
,COAs.Nature
,COAs.Remarks
,COAs.IsActive
,COAs.IsArchive
,COAs.CreatedBy
,COAs.CreatedAt
,COAs.CreatedFrom
,COAs.LastUpdateBy
,COAs.LastUpdateAt
,COAs.LastUpdateFrom
,isnull(COAs.IsRetainedEarning,0)  IsRetainedEarning
,isnull(COAs.COASL,0)              COASL
,isnull(COAs.IsNetProfit,0)        IsNetProfit
,isnull(COAs.IsDepreciation,0)     IsDepreciation
,isnull(COAs.COAType,'-')          COAType
,COAs.ParentCOAId
,isnull(p.Name,'')                 ParentCOAName
FROM COAs
LEFT OUTER JOIN COAGroups g  ON COAs.COAGroupId  = g.Id
LEFT OUTER JOIN COAs p       ON COAs.ParentCOAId  = p.Id
WHERE COAs.IsArchive=0 AND COAs.BranchId=@BranchId";

                if (Id > 0) sqlText += " AND COAs.Id=@Id";

                string cField = "";
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) ||
                            string.IsNullOrWhiteSpace(conditionValues[i])) continue;
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) ||
                            string.IsNullOrWhiteSpace(conditionValues[j])) continue;
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (Id > 0) objComm.Parameters.AddWithValue("@Id", Id);
                objComm.Parameters.AddWithValue("@BranchId", branchId);

                SqlDataReader dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new COANewVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.COASL = Convert.ToInt32(dr["COASL"]);
                    vm.Code = dr["Code"].ToString();
                    vm.COAGroupId = dr["COAGroupId"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.GroupName = dr["GroupName"].ToString();
                    vm.Nature = dr["Nature"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.IsRetainedEarning = Convert.ToBoolean(dr["IsRetainedEarning"]);
                    vm.IsNetProfit = Convert.ToBoolean(dr["IsNetProfit"]);
                    vm.IsDepreciation = Convert.ToBoolean(dr["IsDepreciation"]);
                    vm.COAType = dr["COAType"].ToString();
                    vm.BranchId = branchId;
                    vm.ParentCOAName = dr["ParentCOAName"].ToString();

                    var rawPid = dr["ParentCOAId"];
                    vm.ParentCOAId = (rawPid != DBNull.Value && Convert.ToInt32(rawPid) != 0)
                        ? (int?)Convert.ToInt32(rawPid)
                        : null;

                    VMs.Add(vm);
                }
                dr.Close();

                if (Vtransaction == null && transaction != null)
                    transaction.Commit();
            }
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (VcurrConn == null && currConn != null &&
                    currConn.State == ConnectionState.Open)
                    currConn.Close();
            }
            return VMs;
        }
        #endregion

        #region SelectAllForTree
        public List<COATreeVM> SelectAllForTree(string branchId, string transType = "PF",
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<COATreeVM> VMs = new List<COATreeVM>();
            COATreeVM vm;
            try
            {
                if (VcurrConn != null) currConn = VcurrConn;
                if (Vtransaction != null) transaction = Vtransaction;
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open) currConn.Open();
                }
                if (transaction == null) transaction = currConn.BeginTransaction("");

                sqlText = @"
SELECT
 c.Id
,c.Code
,c.Name
,c.Nature
,isnull(c.COAType,'-')  AS COAType
,c.COAGroupId
,isnull(g.Name,'')      AS GroupName
,c.ParentCOAId
,isnull(c.IsActive,0)   AS IsActive
,isnull(c.COASL,0)      AS COASL
FROM COAs c
LEFT OUTER JOIN COAGroups g ON c.COAGroupId = g.Id
WHERE c.IsArchive=0
  AND c.BranchId =@BranchId
  AND c.TransType=@TransType
ORDER BY isnull(c.ParentCOAId,0), c.COASL, c.Code";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@BranchId", branchId);
                objComm.Parameters.AddWithValue("@TransType", transType);

                SqlDataReader dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new COATreeVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Nature = dr["Nature"].ToString();
                    vm.COAType = dr["COAType"].ToString();
                    vm.COAGroupId = dr["COAGroupId"].ToString();
                    vm.GroupName = dr["GroupName"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.COASL = Convert.ToInt32(dr["COASL"]);

                    var rawPid = dr["ParentCOAId"];
                    if (rawPid != DBNull.Value)
                    {
                        int pid = Convert.ToInt32(rawPid);
                        vm.ParentCOAId = pid == 0 ? (int?)null : pid;
                    }
                    else
                    {
                        vm.ParentCOAId = null;
                    }

                    VMs.Add(vm);
                }
                dr.Close();

                if (Vtransaction == null && transaction != null)
                    transaction.Commit();
            }
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (VcurrConn == null && currConn != null &&
                    currConn.State == ConnectionState.Open)
                    currConn.Close();
            }
            return VMs;
        }

        // Converts flat list → nested tree in memory
        public List<COATreeVM> BuildTree(List<COATreeVM> flatList)
        {
            var lookup = flatList.ToDictionary(x => x.Id);
            var roots = new List<COATreeVM>();

            foreach (var node in flatList)
            {
                if (node.ParentCOAId == null || !lookup.ContainsKey(node.ParentCOAId.Value))
                    roots.Add(node);
                else
                    lookup[node.ParentCOAId.Value].Children.Add(node);
            }
            return roots;
        }
        #endregion

        #region Insert
        public string[] Insert(COANewVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = Id.ToString();
            retResults[3] = sqlText;
            retResults[4] = "ex";
            retResults[5] = "InsertCOAs";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            try
            {
                if (VcurrConn != null) currConn = VcurrConn;
                if (Vtransaction != null) transaction = Vtransaction;
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open) currConn.Open();
                }
                if (transaction == null) transaction = currConn.BeginTransaction("");

                vm.Id = _cDal.NextId("COAs", currConn, transaction);
                if (vm != null)
                {
                    sqlText = @"
INSERT INTO COAs(
 Id, COAGroupId, Code, Name, Nature, TransType, Remarks
,IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom
,IsRetainedEarning, COASL, IsNetProfit, IsDepreciation, COAType
,BranchId, ParentCOAId
) VALUES (
 @Id, @COAGroupId, @Code, @Name, @Nature, @TransType, @Remarks
,@IsActive, @IsArchive, @CreatedBy, @CreatedAt, @CreatedFrom
,@IsRetainedEarning, @COASL, @IsNetProfit, @IsDepreciation, @COAType
,@BranchId, @ParentCOAId
)";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@COAGroupId", vm.COAGroupId);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@Nature", vm.Nature);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@IsRetainedEarning", vm.IsRetainedEarning);
                    cmdInsert.Parameters.AddWithValue("@COASL", vm.COASL);
                    cmdInsert.Parameters.AddWithValue("@IsNetProfit", vm.IsNetProfit);
                    cmdInsert.Parameters.AddWithValue("@IsDepreciation", vm.IsDepreciation);
                    cmdInsert.Parameters.AddWithValue("@COAType", vm.COAType);
                    cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType ?? "PF");
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@ParentCOAId",
                        vm.ParentCOAId.HasValue ? (object)vm.ParentCOAId.Value : DBNull.Value);

                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This COAs already used!";
                    throw new ArgumentNullException("Please Input COAs Value", "");
                }

                if (Vtransaction == null && transaction != null)
                    transaction.Commit();

                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message.ToString();
                if (Vtransaction == null) transaction.Rollback();
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null &&
                    currConn.State == ConnectionState.Open)
                    currConn.Close();
            }
            return retResults;
        }
        #endregion

        #region Update
        public string[] Update(COANewVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "0";
            retResults[3] = "sqlText";
            retResults[4] = "ex";
            retResults[5] = "Employee COAs Update";
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
            try
            {
                if (VcurrConn != null) currConn = VcurrConn;
                if (Vtransaction != null) transaction = Vtransaction;
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open) currConn.Open();
                }
                if (transaction == null) transaction = currConn.BeginTransaction("UpdateToCOAs");

                if (vm != null)
                {
                    sqlText = @"
UPDATE COAs SET
 COAGroupId       = @COAGroupId
,Name             = @Name
,Code             = @Code
,Nature           = @Nature
,Remarks          = @Remarks
,IsActive         = @IsActive
,LastUpdateBy     = @LastUpdateBy
,LastUpdateAt     = @LastUpdateAt
,LastUpdateFrom   = @LastUpdateFrom
,IsRetainedEarning= @IsRetainedEarning
,COASL            = @COASL
,IsNetProfit      = @IsNetProfit
,IsDepreciation   = @IsDepreciation
,COAType          = @COAType
,TransType        = @TransType
,ParentCOAId      = @ParentCOAId
WHERE Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@COAGroupId", vm.COAGroupId);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code);
                    cmdUpdate.Parameters.AddWithValue("@Nature", vm.Nature);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@IsRetainedEarning", vm.IsRetainedEarning);
                    cmdUpdate.Parameters.AddWithValue("@COASL", vm.COASL);
                    cmdUpdate.Parameters.AddWithValue("@IsNetProfit", vm.IsNetProfit);
                    cmdUpdate.Parameters.AddWithValue("@IsDepreciation", vm.IsDepreciation);
                    cmdUpdate.Parameters.AddWithValue("@COAType", vm.COAType);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@TransType", vm.TransType ?? "PF");
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@ParentCOAId",
                        vm.ParentCOAId.HasValue ? (object)vm.ParentCOAId.Value : DBNull.Value);

                    cmdUpdate.ExecuteNonQuery();
                    retResults[2] = vm.Id.ToString();
                    retResults[3] = sqlText;
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("COAs Update", "Could not found any item.");
                }

                if (iSTransSuccess)
                {
                    if (Vtransaction == null && transaction != null)
                        transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                }
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message;
                if (Vtransaction == null) transaction.Rollback();
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null &&
                    currConn.State == ConnectionState.Open)
                    currConn.Close();
            }
            return retResults;
        }
        #endregion

        #region Delete
        public string[] Delete(COANewVM vm, string[] ids,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "0";
            retResults[3] = "sqlText";
            retResults[4] = "ex";
            retResults[5] = "DeleteCOAs";
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
            try
            {
                if (VcurrConn != null) currConn = VcurrConn;
                if (Vtransaction != null) transaction = Vtransaction;
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open) currConn.Open();
                }
                if (transaction == null) transaction = currConn.BeginTransaction("DeleteToEEHead");

                if (ids.Length >= 1)
                {
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = @"
UPDATE COAs SET
 IsActive         = @IsActive
,IsArchive        = @IsArchive
,LastUpdateBy     = @LastUpdateBy
,LastUpdateAt     = @LastUpdateAt
,LastUpdateFrom   = @LastUpdateFrom
,IsRetainedEarning= @IsRetainedEarning
WHERE Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        cmdUpdate.Parameters.AddWithValue("@IsRetainedEarning", false);

                        transResult = Convert.ToInt32(cmdUpdate.ExecuteNonQuery());
                    }
                    retResults[2] = "";
                    retResults[3] = sqlText;

                    if (transResult <= 0)
                        throw new ArgumentNullException("COAs Delete", vm.Id + " could not Delete.");

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("COAs Information Delete", "Could not found any item.");
                }

                if (iSTransSuccess)
                {
                    if (Vtransaction == null && transaction != null)
                        transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message;
                if (Vtransaction == null) transaction.Rollback();
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null &&
                    currConn.State == ConnectionState.Open)
                    currConn.Close();
            }
            return retResults;
        }
        #endregion
    }
}
