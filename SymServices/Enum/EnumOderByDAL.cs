using SymOrdinary;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.Enum
{
   public class EnumOderByDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        public List<EnumOderByVM> DropDown(string Module)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumOderByVM> VMs = new List<EnumOderByVM>();
            EnumOderByVM vm;
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

                sqlText = @"
SELECT
Id,
Name
   FROM EnumOderBy
WHERE IsArchive=0 and IsActive=1
";
                if (!string.IsNullOrWhiteSpace(Module))
                {
                    sqlText += @" and   Module=@Module";
                }
                sqlText += @" ORDER BY Name";


                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(Module))
                {
                        _objComm.Parameters.AddWithValue("@Module",Module);
                }
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EnumOderByVM();
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
    }
}
