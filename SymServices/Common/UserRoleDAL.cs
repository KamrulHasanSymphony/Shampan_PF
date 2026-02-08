using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Common
{
   public class UserRoleDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<UserRolesVM> SelectAll(string userId, int branchId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<UserRolesVM> urolesVMs = new List<UserRolesVM>();
            if (userId ==null || branchId <=0)
            {
                return urolesVMs;
            }
            UserRolesVM urolesVM;
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
,UserInfoId
,RoleInfoId
    From UserRoles
";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = currConn;
                cmd.CommandText = sqlText;
                cmd.CommandType = CommandType.Text;
                //cmd.Parameters.AddWithValue("@BranchId", branchId);
                //cmd.Parameters.AddWithValue("@UserInfoId",userId);
                SqlDataReader dr;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    urolesVM = new UserRolesVM();
                    urolesVM.Id = Convert.ToInt32(dr["Id"]);
                    urolesVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    urolesVM.UserInfoId = dr["UserInfoId"].ToString();
                    urolesVM.RoleInfoId = dr["RoleInfoId"].ToString();
                    urolesVMs.Add(urolesVM);
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

            return urolesVMs;
        }
        #endregion Methods
    }
}
