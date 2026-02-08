using SymOrdinary;
using SymServices.Common;
using SymViewModel.HRM;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Common
{
    public class EmployeeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods

        public List<BranchVM> Branch()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<BranchVM> VMs = new List<BranchVM>();
            BranchVM vm;
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
                    SELECT Id
                          ,Name    
                      FROM Branch
                    ";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BranchVM();
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

        public List<EmployeeInfoVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
Code
   FROM EmployeeInfo
WHERE IsArchive=0 and IsActive=1
    ORDER BY Code
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
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
        public List<EmployeeInfoVM> DropDownBack(string currentCode)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
Code
   FROM EmployeeInfo
WHERE IsArchive=0 and IsActive=1
";
                if (currentCode != "0_0")
                    sqlText += @" and Code>=@Code ";
                sqlText += @"   ORDER BY Code";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                if (currentCode != "0_0")
                    _objComm.Parameters.AddWithValue("@Code", currentCode);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
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
        public List<EmployeeInfoVM> DropDown(string currentCode)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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

                sqlText = @"SELECT *
   FROM EmployeeInfo
WHERE IsArchive=0 
";
                if (currentCode != "0_0")
                    sqlText += @" AND Code>=@Code ";
                sqlText += @"   ORDER BY Code";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                if (currentCode != "0_0")
                    _objComm.Parameters.AddWithValue("@Code", currentCode);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["Code"].ToString() + "~" + dr["Name"].ToString();
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
        public List<EmployeeInfoVM> DropDownCodeName(string BranchId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
EmployeeId
,Code,EMpName as Name
   FROM ViewEmployeeInformation
WHERE IsArchive=0 and IsActive=1 and BranchId=@BranchId
    ORDER BY Code
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", BranchId);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["EmployeeId"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["Code"].ToString() + "~" + dr["Name"].ToString();
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

        static string[] AccountNature = new string[] { "Dr", "Cr" };

        public IEnumerable<object> GetAccountNatureList()
        {
            IEnumerable<object> enumList = from e in AccountNature
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }
        public List<EmployeeInfoVM> EmploymentStatus()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
                    SELECT [Id]
                          ,[Name]    
                      FROM EnumReqEmpStatus
                    ";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();                  
                    vm.EmpName = dr["Name"].ToString();
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

        
        public List<EmployeeInfoVM> EmployeeBySuppervisor(string EmployeeId, string EmpCode)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
            #endregion
            try
            {
                EmployeeInfoDAL eDal = new EmployeeInfoDAL();

                vm = eDal.SelectById(EmployeeId);
           
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"
                    SELECT Id
                    ,Code,(MiddleName+' '+ ISNULL(LastName,'')) Name
                       FROM ViewEmployeeInformation 
                    WHERE IsArchive=0 and IsActive=1 and LEFT(Supervisor, CHARINDEX('~', Supervisor + '~') - 1) = @EmpCode
                        ORDER BY Code
                    ";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                _objComm.Parameters.AddWithValue("@EmpCode", EmpCode);
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["Code"].ToString() + "~" + dr["Name"].ToString();
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

        public List<EmployeeInfoVM> EmployeeByDepartment(string Department)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
            #endregion
            try
            {
                EmployeeInfoDAL eDal = new EmployeeInfoDAL();

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"
                    SELECT Id
                    ,Code,(MiddleName+' '+ ISNULL(LastName,'')) Name
                       FROM ViewEmployeeInformation 
                    WHERE IsArchive=0 and IsActive=1 and DepartmentId = @DepartmentId
                        ORDER BY Code
                    ";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@DepartmentId", Department);             
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["Code"].ToString() + "~" + dr["Name"].ToString();
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

        public List<EmployeeInfoVM> DropDownCodeNameAll(string BranchId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
,Code,Name
   FROM EmployeeInfo
WHERE 1=1 and BranchId=@BranchId
    ORDER BY Code
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("BranchId",BranchId);
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["Code"].ToString() + "~" + dr["Name"].ToString();
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

        public List<EmployeeInfoVM> DropDownCodeNameConfirmed()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
e.Code,(e.MiddleName+' '+ ISNULL(e.LastName,'')) Name
   FROM EmployeeInfo e
   Left OUTER JOIN EmployeeJob ejob ON e.Id = ejob.EmployeeId
WHERE e.IsArchive=0 and e.IsActive=1 AND ISNULL(ejob.IsPermanent, 0) = 1
    ORDER BY e.Code
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    //vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["Code"].ToString() + "~" + dr["Name"].ToString();
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

        public List<EmployeeInfoVM> DropDownExCodeName()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
                            Code,Name
                            FROM EmployeeInfo
                            WHERE  1=1
                            AND (IsArchive=1 OR IsActive = 0)
                            ORDER BY Code ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    //vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["Code"].ToString() + "~" + dr["Name"].ToString();
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

        public List<EmployeeInfoVM> DropDownAllEmployee(string[] conFields = null, string[] conValues = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
e.Id
,e.Code
,(e.MiddleName+' '+ ISNULL(e.LastName,'')) Name
,ISNULL(ej.JoinDate,'19000101') JoinDate
,ISNULL(ej.LeftDate,'19000101') LeftDate
,case when e.IsActive = 0 then 'Inactive' when e.IsActive = 1 then 'Active' end as Status
   FROM EmployeeInfo e
   LEFT OUTER JOIN EmployeeJob ej ON e.Id = ej.EmployeeId

WHERE 1=1
and e.Id != '1_0'
";
                string cField = "";
                if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                {
                    for (int i = 0; i < conFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]))
                        {
                            continue;
                        }
                        cField = conFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conFields[i] + "=@" + cField;
                    }
                }

                sqlText += " ORDER BY e.Code";


                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                {
                    for (int j = 0; j < conFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[j]) || string.IsNullOrWhiteSpace(conValues[j]))
                        {
                            continue;
                        }
                        cField = conFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        _objComm.Parameters.AddWithValue("@" + cField, conValues[j]);
                    }
                }

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["Name"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.LeftDate = Ordinary.StringToDate(dr["LeftDate"].ToString());
                    vm.Status =   dr["Status"].ToString();
                    vm.EmpInfo = vm.Code + "~" + vm.EmpName + "~" + vm.JoinDate+ "~" + vm.LeftDate+ "~" + vm.Status;
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

        //==================SelectAll Employees=================
        public List<EmployeeVM> SelectAllEmployee()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeVM> employees = new List<EmployeeVM>();
            EmployeeVM employee;
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
Name,
Code,
FatherName
    FROM Employees
    ORDER BY Name
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employee = new EmployeeVM();
                    employee.Id = dr["Id"].ToString();
                    employee.Name = dr["Name"].ToString();
                    employee.Code = dr["Code"].ToString();
                    employee.FatherName = dr["FatherName"].ToString();
                    employees.Add(employee);
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

            return employees;
        }
        public EmployeeVM SelectSingleEmployee(int Id) 
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeVM employee = new EmployeeVM(); ;
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
Name,
Code,
 FatherName
    FROM Employees
where id=@Id 
 
     
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
                    employee = new EmployeeVM();
                    employee.Id = dr["Id"].ToString();
                    employee.Name = dr["Name"].ToString();
                    employee.Code = dr["Code"].ToString();
                    employee.FatherName = dr["FatherName"].ToString();

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

            return employee;
        }
        //==================Insert Employeees=================
        public string[] EmployeeDataInsert(EmployeeVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "EmployeeDataInsert"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.Code))
                {
                    retResults[1] = "Please Input Employee Code";
                    return retResults;
                }
                else if (string.IsNullOrEmpty(vm.Name))
                {
                    retResults[1] = "Please Input Employee Name";
                    return retResults;
                }

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

                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Employees";
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist
                #region Save

                //int foundId = (int)objfoundId;
                //if (foundId <= 0)
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += " INSERT INTO Employees(	Code,Name,FatherName)";
                    sqlText += " VALUES (@Code,@Name,@FatherName ) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name.Trim());
                    cmdInsert.Parameters.AddWithValue("@FatherName", vm.FatherName);

                    cmdInsert.Transaction = transaction;
                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee already used";
                    throw new ArgumentNullException("Please Input Employee Value", "");
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
                retResults[1] = "Data Save Successfully";
                retResults[2] = Id.ToString();

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
        //==================Update Employee=================
        public string[] EmployeeUpdate(EmployeeVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeeUpdate"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployee"); }

                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Code)Code FROM Employees ";
                //sqlText += " WHERE Code=@Code AND Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", employeeVM.Id);
                //cmdExist.Parameters.AddWithValue("@Code", employeeVM.Code.Trim());

                //var exeRes = cmdExist.ExecuteScalar();
                //int exists = Convert.ToInt32(exeRes);
                //if (exists > 0)
                //{
                //    retResults[1] = "This Employee already used";
                //    throw new ArgumentNullException("Please Input Employee Value", "");
                //}
                //#endregion Exist

                //#region Exist

                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "Employees";
                //string[] fieldName = { "Code", "Name" };
                //string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };

                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInUpdate(vm.Id.ToString(), tableName, fieldName[i], fieldValue[i], currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                //#endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update Employees set";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";
                    sqlText += " FatherName=@FatherName";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name.Trim());
                    cmdUpdate.Parameters.AddWithValue("@FatherName", vm.FatherName);

                    cmdUpdate.Transaction = transaction;
                    try
                    {
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    catch (Exception ex)
                    {
                        retResults[1] = " Could not updated.";// Return Id
                        throw ex;
                    }

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        retResults[1] = vm.Name + " could not updated.";// Return Id

                        throw new ArgumentNullException("EmployeeUpdate", vm.Name + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    retResults[1] = " Could not found any item.";
                    throw new ArgumentNullException("EmployeeUpdate", "Could not found any item.");
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
                    retResults[1] = "Requested Employee Information Successfully Updated.";

                }
                else
                {
                    retResults[1] = "Unexpected error to update Employee.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
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

            return retResults;
        }
        //==================Select Employee=================
        public EmployeeVM SelectEmployee(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeeVM employeeVM = new EmployeeVM();

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
                sqlText = @"SELECT Top 1 Id,Code, Name, FatherName FROM Employees  ";
                if (query == null)
                {
                    if (Id != 0)
                    {
                        sqlText += " AND Id=@Id";
                    }
                    else
                    {
                        sqlText += " ORDER BY Id ";
                    }
                }
                else
                {
                    if (query == "FIRST")
                    {
                        sqlText += " ORDER BY Id ";
                    }
                    else if (query == "LAST")
                    {
                        sqlText += " ORDER BY Id DESC";
                    }
                    else if (query == "NEXT")
                    {
                        sqlText += " and  Id > @Id   ORDER BY Id";
                    }
                    else if (query == "PREVIOUS")
                    {
                        sqlText += "  and  Id < @Id   ORDER BY Id DESC";
                    }
                }


                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (Id != null)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                try
                {
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        employeeVM.Id = dr["Id"].ToString();
                        employeeVM.Name = dr["Name"].ToString();
                        employeeVM.Code = dr["Code"].ToString();
                        employeeVM.FatherName = dr["FatherName"].ToString();
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return employeeVM;
        }
        //==================Delete Employee=================
        public string[] EmployeeDelete(int buyerId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeeDelete"; //Method Name

            int transResult = 0;
            int countId = 0;
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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployee"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (buyerId > 0)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update Employees set";
                    sqlText += " IsArchive=@IsArchive";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", buyerId);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", UserInfoVM.UserId);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", DateTime.Now);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", UserInfoVM.UserLogingPcIP);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = buyerId.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EmployeeDelete", buyerId + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeDelete", "Could not found any item.");
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
                    retResults[1] = "Requested Employee Information Successfully Deleted.";

                }
                else
                {
                    retResults[1] = "Unexpected error to delete Employee.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
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

            return retResults;
        }



        public EmployeeVM SelectSingleEmployeeName(string LogId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeVM employee = new EmployeeVM(); ;
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

                //CONCAT(UPPER(TRIM([User].LogId)),'~', TRIM(fullname)) empFullName
                sqlText = @"SELECT TRIM(fullname) empFullName FROM ViewEmployeeInformation ve 
INNER JOIN [dbo].[User] ON 
[user].EmployeeId = ve.EmployeeId WHERE [user].LogId=@LogId AND [User].IsActive=1
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@LogId", LogId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employee = new EmployeeVM();
                    employee.Name = dr["empFullName"].ToString();
                    
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

            return employee;
        }


        public DataTable EmployeeNomeniee()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();          
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
                sqlText = @"Select ei.Code,ei.EmpName,en.Name as NomineeName,en.Relation,en.Mobile,en.DateOfBirth,en.BirthCertificateNo,en.Address, 
en.PostalCode,en.District,en.Division,en.Country from  ViewEmployeeInformation ei
Left Join EmployeeNominee en on en.EmployeeId=ei.Id
Left Join EmployeePersonalDetail epd on epd.EmployeeId=ei.Id";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);             
                da.Fill(dt);

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

            return dt;
        }


        #endregion


        public DataTable EmployeeDependent()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                sqlText = @"Select ei.Branch,ei.Code,ei.EmpName,epd.DateOfBirth,ei.Gender,ei.SpouseName,ed.Name as ChildName,epd.FatherName,epd.MotherName from ViewEmployeeInformation ei 
Left Join EmployeeDependent ed on ed.EmployeeId=ei.Id
Left Join EmployeePersonalDetail epd on epd.EmployeeId=ei.Id";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.Fill(dt);

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

            return dt;
        }
    }
}
