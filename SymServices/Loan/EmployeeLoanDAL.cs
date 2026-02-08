using SymOrdinary;
using SymServices.Common;

using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Loan;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymServices.Loan
{
    public class EmployeeLoanDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        /// <summary>
        /// Only Master data 
        /// </summary>
        /// <param name="BranchId">Branch Id</param>
        /// <param name="employeeId"> employee Id</param>
        /// <returns></returns>
        public List<EmployeeLoanVM> SelectAll(int BranchId, string employeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLoanVM> VMs = new List<EmployeeLoanVM>();
            EmployeeLoanVM vm;
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
ve.Code,
ve.EmpName,ve.Department ,ve.Designation
,l.PrincipalAmount
,l.InterestRate
,l.InterestAmount
,l.TotalAmount
,l.InterestPolicy
,l.NumberOfInstallment
,l.StartDate
,l.EndDate
,l.IsHold
,l.Remarks
,t.Name
,t.Name LoanType
 ,l.Id
 from EmployeeLoan l
left outer join ViewEmployeeInformation ve on l.EmployeeId=ve.EmployeeId
left outer join EnumLoanType t on t.Id=l.LoanType_E
WHERE l.IsArchive=0  and l.BranchId=@BranchId
";
                if (!string.IsNullOrWhiteSpace(employeeId))
                {
                    sqlText += "  and ve.Id=@employeeId";
                }
                sqlText += "  ORDER BY ve.Department, ve.EmpName desc";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", BranchId);
                if (!string.IsNullOrWhiteSpace(employeeId))
                {
                    _objComm.Parameters.AddWithValue("@employeeId", employeeId);
                }
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLoanVM();
                    vm.Id = dr["Id"].ToString();
                    vm.InterestPolicy = dr["InterestPolicy"].ToString();
                    vm.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"]);
                    vm.InterestRate = Convert.ToDecimal(dr["InterestRate"]);
                    vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                    vm.NumberOfInstallment = Convert.ToInt32(dr["NumberOfInstallment"]);
                    vm.StartDate = Ordinary.StringToDate(dr["StartDate"].ToString());
                    vm.EndDate = Ordinary.StringToDate(dr["EndDate"].ToString());
                    vm.IsHold = Convert.ToBoolean(dr["IsHold"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    //vm.Employee = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    vm.LoanType = dr["Name"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    VMs.Add(vm);
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
            return VMs;
        }
        /// <summary>
        /// Individual loan
        /// </summary>
        /// <param name="loanID">loanId</param>
        /// <returns>Loan header and details</returns>
        public EmployeeLoanVM SelectLoan(string loanID)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeLoanVM vm = new EmployeeLoanVM();
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
 l.Id
,l.BranchId
,l.LoanType_E
,l.EmployeeId
,ve.EmpName, ve.Code, ve.Designation, ve.Department, ve.section
,l.PrincipalAmount
,l.IsFixed
,l.InterestPolicy
,l.InterestRate
,l.InterestAmount
,l.TotalAmount
,l.NumberOfInstallment
,l.StartDate
,l.EndDate
,l.IsHold
,l.IsApproved
,l.ApplicationDate
,l.ApprovedDate
,l.RefundAmount
,isnull(l.RefundDate, '') RefundDate
,l.Remarks
,t.Name LoanType
 from EmployeeLoan l
left outer join ViewEmployeeInformation ve on l.EmployeeId=ve.EmployeeId
left outer join EnumLoanType t on t.Id=l.LoanType_E
WHERE l.IsArchive=0 and l.Id=@loanID
";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@loanID", loanID);
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = dr["BranchId"].ToString();
                    vm.LoanType_E = dr["LoanType_E"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.InterestPolicy = dr["InterestPolicy"].ToString();
                    vm.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"]);
                    vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    vm.InterestRate = Convert.ToDecimal(dr["InterestRate"]);
                    vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                    vm.NumberOfInstallment = Convert.ToInt32(dr["NumberOfInstallment"]);
                    vm.StartDate = Ordinary.StringToDate(dr["StartDate"].ToString());
                    vm.EndDate = Ordinary.StringToDate(dr["EndDate"].ToString());
                    vm.IsHold = Convert.ToBoolean(dr["IsHold"]);
                    vm.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.ApplicationDate = Ordinary.StringToDate(dr["ApplicationDate"].ToString());
                    vm.ApprovedDate = Ordinary.StringToDate(dr["ApprovedDate"].ToString());
                    vm.LoanType = dr["LoanType"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    //vm.Employee = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() +" " +dr["LastName"].ToString();
                }
                dr.Close();
                vm.employeeLoanDetails = LoanDetails(loanID, currConn);
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
            return vm;
        }
        private List<EmployeeLoanDetailVM> LoanDetails(string loanID, SqlConnection con)
        {
            List<EmployeeLoanDetailVM> loanDetails = new List<EmployeeLoanDetailVM>();
            EmployeeLoanDetailVM loanDetail;
            string sql = @"SELECT 
    Id,
    EmployeeLoanId,
    EmployeeId,
    InstallmentAmount,
    InstallmentPaidAmount,
    PaymentScheduleDate,
    PaymentDate,
    IsHold,
    IsPaid,
    HaveDuplicate,
    DuplicateID,
    Remarks,
    PrincipalAmount,
    InterestAmount,
     (SELECT ISNULL(SUM(PrincipalAmount),0) PrincipalAmount
     FROM EmployeeLoanDetail 
     WHERE EmployeeLoanId = @LoanId 
     AND IsPaid = 0 
     AND IsArchive = 0) AS TotalDuePrincipalAmount,
    (SELECT ISNULL(SUM(InterestAmount),0) InterestAmount
     FROM EmployeeLoanDetail 
     WHERE EmployeeLoanId = @LoanId 
     AND IsPaid = 0 
     AND IsArchive = 0) AS TotalDueInterestAmount,
    (SELECT ISNULL(SUM(InterestAmount),0)
     FROM EmployeeLoanDetail 
     WHERE EmployeeLoanId = @LoanId 
     AND IsPaid = 0 
     AND IsArchive = 0) AS SettlementAmount,
  (SELECT ISNULL(Count(IsPaid),0)
     FROM EmployeeLoanDetail 
     WHERE EmployeeLoanId = @LoanId  and IsPaid=0 
     AND IsPaid = 0 
     AND IsArchive = 0) AS NoofInstallment
    FROM 
        EmployeeLoanDetail 
    WHERE 
        EmployeeLoanId = @LoanId";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@LoanId", loanID);
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    loanDetail = new EmployeeLoanDetailVM();
                    loanDetail.Id = Convert.ToInt32(dr["Id"]);
                    loanDetail.EmployeeLoanId = dr["EmployeeLoanId"].ToString();
                    loanDetail.EmployeeId = dr["EmployeeId"].ToString();
                    loanDetail.InstallmentAmount = Convert.ToDecimal(dr["InstallmentAmount"]);
                    loanDetail.InstallmentPaidAmount = Convert.ToDecimal(dr["InstallmentPaidAmount"]);
                    loanDetail.PaymentScheduleDate = Ordinary.StringToDate(dr["PaymentScheduleDate"].ToString());
                    loanDetail.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());
                    loanDetail.IsHold = Convert.ToBoolean(dr["IsHold"]);
                    loanDetail.HaveDuplicate = Convert.ToBoolean(dr["HaveDuplicate"]);
                    loanDetail.DuplicateID = Convert.ToInt32(dr["DuplicateID"]);
                    loanDetail.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                    loanDetail.Remarks = dr["Remarks"].ToString();
                    loanDetail.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"]);
                    loanDetail.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                    loanDetail.TotalDuePrincipalAmount = Convert.ToDecimal(dr["TotalDuePrincipalAmount"]);
                    loanDetail.TotalDueInterestAmount = Convert.ToDecimal(dr["TotalDueInterestAmount"]);
                    loanDetail.SettlementAmount = Convert.ToDecimal(dr["SettlementAmount"]);
                    loanDetail.NoofInstallment = Convert.ToDecimal(dr["NoofInstallment"]);
                    loanDetails.Add(loanDetail);
                }
            }
            return loanDetails;
        }

        public EmployeeLoanVM SelectLoanForSettelment(string loanID)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeLoanVM vm = new EmployeeLoanVM();
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
 l.Id
,l.BranchId
,l.LoanType_E
,l.EmployeeId
,ve.EmpName, ve.Code, ve.Designation, ve.Department, ve.section
,l.PrincipalAmount
,l.IsFixed
,l.InterestPolicy
,l.InterestRate
,l.InterestAmount
,l.TotalAmount
,l.NumberOfInstallment
,l.StartDate
,l.EndDate
,l.IsHold
,l.IsApproved
,l.ApplicationDate
,l.ApprovedDate
,isnull(l.RefundAmount,0) RefundAmount
,isnull(l.RefundDate, '') RefundDate
,l.Remarks
,t.Name LoanType
,isnull(l.EarlySellteInterestAmount,0) EarlySellteInterestAmount
 from EmployeeLoan l
left outer join ViewEmployeeInformation ve on l.EmployeeId=ve.EmployeeId
left outer join EnumLoanType t on t.Id=l.LoanType_E
WHERE l.IsArchive=0 and l.Id=@loanID
";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@loanID", loanID);
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = dr["BranchId"].ToString();
                    vm.LoanType_E = dr["LoanType_E"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.InterestPolicy = dr["InterestPolicy"].ToString();
                    vm.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"]);
                    vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    vm.InterestRate = Convert.ToDecimal(dr["InterestRate"]);
                    vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                    vm.NumberOfInstallment = Convert.ToInt32(dr["NumberOfInstallment"]);
                    vm.StartDate = Ordinary.StringToDate(dr["StartDate"].ToString());
                    vm.EndDate = Ordinary.StringToDate(dr["EndDate"].ToString());
                    vm.IsHold = Convert.ToBoolean(dr["IsHold"]);
                    vm.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    vm.RefundAmount = Convert.ToDecimal(dr["RefundAmount"]);
                    vm.RefundDate = Ordinary.StringToDate(dr["RefundDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.ApplicationDate = Ordinary.StringToDate(dr["ApplicationDate"].ToString());
                    vm.ApprovedDate = Ordinary.StringToDate(dr["ApprovedDate"].ToString());
                    vm.LoanType = dr["LoanType"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.TotalDueInterestAmount =  Convert.ToDecimal(dr["EarlySellteInterestAmount"].ToString());
                    //vm.Employee = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() +" " +dr["LastName"].ToString();
                }
                dr.Close();
                vm.employeeLoanDetails = LoanDetailsForSettelment(loanID, currConn);
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
            return vm;
        }
        private List<EmployeeLoanDetailVM> LoanDetailsForSettelment(string loanID, SqlConnection con)
        {
            List<EmployeeLoanDetailVM> loanDetails = new List<EmployeeLoanDetailVM>();
            EmployeeLoanDetailVM loanDetail;
            string sql = @"SELECT 
    Id,
    EmployeeLoanId,
    EmployeeId,
    InstallmentAmount,
    InstallmentPaidAmount,
    PaymentScheduleDate,
    PaymentDate,
    IsHold,
    IsPaid,
    HaveDuplicate,
    DuplicateID,
    Remarks,
    PrincipalAmount,
    InterestAmount,
    (SELECT SUM(PrincipalAmount) 
     FROM EmployeeLoanDetail 
     WHERE EmployeeLoanId = @LoanId 
     AND IsPaid = 0 
     AND IsArchive = 0) AS TotalDuePrincipalAmount,
    (SELECT TOP 1  InterestAmount 
     FROM EmployeeLoanDetail 
     WHERE EmployeeLoanId = @LoanId 
     AND IsPaid = 0 
     AND IsArchive = 0) AS TotalDueInterestAmount,
    (SELECT SUM(PrincipalAmount) 
     FROM EmployeeLoanDetail 
     WHERE EmployeeLoanId = @LoanId 
     AND IsPaid = 0 
     AND IsArchive = 0)
    +
    (SELECT TOP 1  InterestAmount 
     FROM EmployeeLoanDetail 
     WHERE EmployeeLoanId = @LoanId 
     AND IsPaid = 0 
     AND IsArchive = 0) AS SettlementAmount,
     (SELECT 
        COUNT(*) AS UnpaidRowCount
    FROM 
        EmployeeLoanDetail 
    WHERE 
        EmployeeLoanId = '1_129' 
        AND IsPaid = 0 
        AND IsArchive = 0) as NoofInstallment
    FROM 
        EmployeeLoanDetail 
    WHERE 
        EmployeeLoanId = @LoanId ";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@LoanId", loanID);
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    loanDetail = new EmployeeLoanDetailVM();
                    loanDetail.Id = Convert.ToInt32(dr["Id"]);
                    loanDetail.EmployeeLoanId = dr["EmployeeLoanId"].ToString();
                    loanDetail.EmployeeId = dr["EmployeeId"].ToString();
                    loanDetail.InstallmentAmount = Convert.ToDecimal(dr["InstallmentAmount"]);
                    loanDetail.InstallmentPaidAmount = Convert.ToDecimal(dr["InstallmentPaidAmount"]);
                    loanDetail.PaymentScheduleDate = Ordinary.StringToDate(dr["PaymentScheduleDate"].ToString());
                    loanDetail.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());
                    loanDetail.IsHold = Convert.ToBoolean(dr["IsHold"]);
                    loanDetail.HaveDuplicate = Convert.ToBoolean(dr["HaveDuplicate"]);
                    loanDetail.DuplicateID = Convert.ToInt32(dr["DuplicateID"]);
                    loanDetail.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                    loanDetail.Remarks = dr["Remarks"].ToString();
                    loanDetail.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"]);
                    loanDetail.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                    loanDetail.TotalDuePrincipalAmount = Convert.ToDecimal(dr["TotalDuePrincipalAmount"]);
                    loanDetail.TotalDueInterestAmount = Convert.ToDecimal(dr["TotalDueInterestAmount"]);
                    loanDetail.SettlementAmount = Convert.ToDecimal(dr["SettlementAmount"]);
                    loanDetail.NoofInstallment = Convert.ToDecimal(dr["NoofInstallment"]);
                    loanDetails.Add(loanDetail);
                }
            }
            return loanDetails;
        }

        public EmployeeLoanDetailVM SelectEmployeeLoan(string Id, string emploanId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeLoanDetailVM vm = new EmployeeLoanDetailVM();
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
                sqlText = @"select
 Id
,EmployeeLoanId
,EmployeeId
,InstallmentAmount
,InstallmentPaidAmount
,PaymentScheduleDate
,PaymentDate
,IsHold
,IsPaid
,HaveDuplicate
,DuplicateID
,Remarks
,PrincipalAmount
,InterestAmount
from EmployeeLoanDetail where Id=@Id and EmployeeLoanId=@emploanId and IsArchive=0 ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@Id", Id);
                _objComm.Parameters.AddWithValue("@emploanId", emploanId);
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLoanDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeLoanId = dr["EmployeeLoanId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.InstallmentAmount = Convert.ToDecimal(dr["InstallmentAmount"]);
                    vm.InstallmentPaidAmount = Convert.ToDecimal(dr["InstallmentPaidAmount"]);
                    vm.PaymentScheduleDate = Ordinary.StringToDate(dr["PaymentScheduleDate"].ToString());
                    vm.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());
                    vm.IsHold = Convert.ToBoolean(dr["IsHold"]);
                    vm.HaveDuplicate = Convert.ToBoolean(dr["HaveDuplicate"]);
                    vm.DuplicateID = Convert.ToInt32(dr["DuplicateID"]);
                    vm.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"]);
                    vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                    //vm.Employee = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() +" " +dr["LastName"].ToString();
                }
                dr.Close();
                #endregion
            }
            #region catch
            //catch (SqlException sqlex)
            //{
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //}
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
            return vm;
        }
        public string[] EmployeeLoanInsertBackup(EmployeeLoanVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (vm.IsApproved != true)
                {
                    //throw new ArgumentNullException("This Loan already Approved" ,"");
                    if (vm.Id != null)
                    {
                        sqlText = "  ";
                        sqlText += "   delete from EmployeeLoan where Id=@Id";
                        SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                        cmdExistD.Transaction = transaction;
                        cmdExistD.Parameters.AddWithValue("@Id", vm.Id);
                        var exeResD = cmdExistD.ExecuteScalar();
                    }
                    #region Save
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EmployeeLoan where BranchId=@BranchId";
                    SqlCommand cmd111 = new SqlCommand(sqlText, currConn);
                    cmd111.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd111.Transaction = transaction;
                    var exeRes = cmd111.ExecuteScalar();
                    int count = Convert.ToInt32(exeRes);
                    vm.Id = vm.BranchId.ToString() + "_" + (count + 1);
                    sqlText = "  ";
                    sqlText += @"   INSERT INTO EmployeeLoan(
Id
,BranchId
,LoanType_E
,EmployeeId
,PrincipalAmount
,IsFixed
,InterestPolicy
,InterestRate
,InterestAmount
,ApplicationDate
,IsApproved
,TotalAmount
,NumberOfInstallment
,ApprovedDate
,StartDate
,EndDate
,IsHold
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
                                    ) VALUES (
 @Id
,@BranchId
,@LoanType_E
,@EmployeeId
,@PrincipalAmount
,@IsFixed
,@InterestPolicy
,@InterestRate
,@InterestAmount
,@ApplicationDate
,@IsApproved
,@TotalAmount
,@NumberOfInstallment
,@ApprovedDate
,@StartDate
,@EndDate
,@IsHold
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom) ";
                    SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                    cmdIns.Parameters.AddWithValue("@Id", vm.Id);
                    cmdIns.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdIns.Parameters.AddWithValue("@LoanType_E", vm.LoanType_E);
                    cmdIns.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdIns.Parameters.AddWithValue("@PrincipalAmount", vm.PrincipalAmount);
                    cmdIns.Parameters.AddWithValue("@IsFixed", true);
                    cmdIns.Parameters.AddWithValue("@InterestPolicy", vm.InterestPolicy);
                    cmdIns.Parameters.AddWithValue("@InterestRate", vm.InterestRate);
                    cmdIns.Parameters.AddWithValue("@InterestAmount", vm.InterestAmount);
                    cmdIns.Parameters.AddWithValue("@TotalAmount", vm.TotalAmount);
                    cmdIns.Parameters.AddWithValue("@NumberOfInstallment", vm.NumberOfInstallment);
                    cmdIns.Parameters.AddWithValue("@ApprovedDate", Ordinary.DateToString(vm.ApprovedDate));
                    cmdIns.Parameters.AddWithValue("@ApplicationDate", Ordinary.DateToString(vm.ApplicationDate));
                    cmdIns.Parameters.AddWithValue("@StartDate", Ordinary.DateToString(vm.StartDate));
                    cmdIns.Parameters.AddWithValue("@EndDate", Ordinary.DateToString(vm.EndDate));
                    cmdIns.Parameters.AddWithValue("@IsHold", vm.IsHold);
                    cmdIns.Parameters.AddWithValue("@IsApproved", vm.IsApproved);
                    cmdIns.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdIns.Parameters.AddWithValue("@IsActive", true);
                    cmdIns.Parameters.AddWithValue("@IsArchive", false);
                    cmdIns.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdIns.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdIns.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdIns.Transaction = transaction;
                    exeRes = cmdIns.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);
                    #region Save Detail
                    sqlText = "  ";
                    sqlText += @"   INSERT INTO EmployeeLoanDetail(
                        EmployeeLoanId,EmployeeId,InstallmentAmount,InstallmentPaidAmount
                        ,PaymentScheduleDate,PaymentDate,IsHold,IsPaid,Remarks
                        ,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                        ,PrincipalAmount,InterestAmount,HaveDuplicate,DuplicateID
                        ) VALUES (                     
                        @EmployeeLoanId,@EmployeeId,@InstallmentAmount,@InstallmentPaidAmount
                        ,@PaymentScheduleDate,@PaymentDate,@IsHold,@IsPaid,@Remarks
                        ,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                        ,@PrincipalAmount,@InterestAmount,@HaveDuplicate,@DuplicateID
                        )  ";
                    if (vm.IsApproved == true)
                    {
                        foreach (var item in vm.employeeLoanDetails)
                        {
                            SqlCommand cmdInsD = new SqlCommand(sqlText, currConn);
                            cmdInsD.Parameters.AddWithValue("@EmployeeLoanId", vm.Id);
                            cmdInsD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                            cmdInsD.Parameters.AddWithValue("@InstallmentAmount", item.InstallmentAmount);
                            cmdInsD.Parameters.AddWithValue("@InstallmentPaidAmount", item.InstallmentPaidAmount);
                            cmdInsD.Parameters.AddWithValue("@PaymentScheduleDate", Ordinary.DateToString(item.PaymentScheduleDate));
                            cmdInsD.Parameters.AddWithValue("@PaymentDate", Ordinary.DateToString(item.PaymentDate));
                            cmdInsD.Parameters.AddWithValue("@IsHold", false);
                            cmdInsD.Parameters.AddWithValue("@IsPaid", false);
                            cmdInsD.Parameters.AddWithValue("@PrincipalAmount", item.PrincipalAmount);
                            cmdInsD.Parameters.AddWithValue("@InterestAmount", item.InterestAmount);
                            cmdInsD.Parameters.AddWithValue("@HaveDuplicate", false);
                            cmdInsD.Parameters.AddWithValue("@DuplicateID", 0);
                            cmdInsD.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                            cmdInsD.Parameters.AddWithValue("@IsActive", true);
                            cmdInsD.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsD.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                            cmdInsD.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                            cmdInsD.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                            cmdInsD.Transaction = transaction;
                            cmdInsD.ExecuteNonQuery();
                        }
                    }
                    #endregion Save Detail
                    #endregion Save
                }
                else
                {
                    sqlText = "  ";
                    sqlText += @"  update  EmployeeLoanDetail set
InstallmentAmount=@InstallmentAmount
,InstallmentPaidAmount=@InstallmentPaidAmount
,PaymentScheduleDate=@PaymentScheduleDate
,InstallmentPaidAmount=@InstallmentPaidAmount
where id=@id
";
                    foreach (var item in vm.employeeLoanDetails)
                    {
                        SqlCommand cmdInsD = new SqlCommand(sqlText, currConn);
                        cmdInsD.Parameters.AddWithValue("@id", item.Id);
                        cmdInsD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                        cmdInsD.Parameters.AddWithValue("@InstallmentAmount", item.InstallmentAmount);
                        cmdInsD.Parameters.AddWithValue("@PrincipalAmount", item.PrincipalAmount);
                        cmdInsD.Parameters.AddWithValue("@InterestAmount", item.InterestAmount);
                        cmdInsD.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                        cmdInsD.Parameters.AddWithValue("@IsActive", true);
                        cmdInsD.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsD.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdInsD.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdInsD.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                        cmdInsD.Transaction = transaction;
                        cmdInsD.ExecuteNonQuery();
                    }
                }
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
        public string[] EmployeeLoanInsert(EmployeeLoanVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                //throw new ArgumentNullException("This Loan already Approved" ,"");
                if (vm.Id != null)
                {
                    sqlText = "  ";
                    sqlText += "   delete from EmployeeLoan where Id=@Id";
                    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                    cmdExistD.Transaction = transaction;
                    cmdExistD.Parameters.AddWithValue("@Id", vm.Id);
                    var exeResD = cmdExistD.ExecuteScalar();
                }
                #region Save
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EmployeeLoan where BranchId=@BranchId";
                SqlCommand cmd111 = new SqlCommand(sqlText, currConn);
                cmd111.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd111.Transaction = transaction;
                var exeRes = cmd111.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                vm.Id = vm.BranchId.ToString() + "_" + (count + 1);
                sqlText = "  ";
                sqlText += @"   INSERT INTO EmployeeLoan(
                    Id,BranchId,LoanType_E,EmployeeId,PrincipalAmount,IsFixed,InterestPolicy,InterestRate,InterestAmount,ApplicationDate
                    ,IsApproved,TotalAmount,NumberOfInstallment,ApprovedDate,StartDate,EndDate,IsHold,Remarks
                    ,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LoanNo
                    ) VALUES (
                    @Id,@BranchId,@LoanType_E,@EmployeeId,@PrincipalAmount
                    ,@IsFixed,@InterestPolicy,@InterestRate,@InterestAmount,@ApplicationDate,@IsApproved,@TotalAmount
                    ,@NumberOfInstallment,@ApprovedDate,@StartDate,@EndDate,@IsHold,@Remarks
                    ,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@LoanNo
                    ) ";
                SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                cmdIns.Parameters.AddWithValue("@Id", vm.Id);
                cmdIns.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdIns.Parameters.AddWithValue("@LoanType_E", vm.LoanType_E);
                cmdIns.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdIns.Parameters.AddWithValue("@PrincipalAmount", vm.PrincipalAmount);
                cmdIns.Parameters.AddWithValue("@IsFixed", true);
                cmdIns.Parameters.AddWithValue("@InterestPolicy", vm.InterestPolicy);
                cmdIns.Parameters.AddWithValue("@InterestRate", vm.InterestRate);
                cmdIns.Parameters.AddWithValue("@InterestAmount", vm.InterestAmount);
                cmdIns.Parameters.AddWithValue("@TotalAmount", vm.TotalAmount);
                cmdIns.Parameters.AddWithValue("@NumberOfInstallment", vm.NumberOfInstallment);
                cmdIns.Parameters.AddWithValue("@ApprovedDate", Ordinary.DateToString(vm.ApprovedDate));
                cmdIns.Parameters.AddWithValue("@ApplicationDate", Ordinary.DateToString(vm.ApplicationDate));
                cmdIns.Parameters.AddWithValue("@StartDate", Ordinary.DateToString(vm.StartDate));
                cmdIns.Parameters.AddWithValue("@EndDate", Ordinary.DateToString(vm.EndDate));
                cmdIns.Parameters.AddWithValue("@IsHold", vm.IsHold);
                cmdIns.Parameters.AddWithValue("@IsApproved", vm.IsApproved);
                cmdIns.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                cmdIns.Parameters.AddWithValue("@IsActive", true);
                cmdIns.Parameters.AddWithValue("@IsArchive", false);
                cmdIns.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdIns.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                cmdIns.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                cmdIns.Parameters.AddWithValue("@LoanNo", vm.LoanNo);
                cmdIns.Transaction = transaction;
                exeRes = cmdIns.ExecuteScalar();
                Id = Convert.ToInt32(exeRes);
                #region Save Detail
                sqlText = "  ";
                sqlText += @"   INSERT INTO EmployeeLoanDetail(
                        EmployeeLoanId,EmployeeId,InstallmentAmount,InstallmentPaidAmount
                        ,PaymentScheduleDate,PaymentDate,IsHold,IsPaid,Remarks
                        ,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                        ,PrincipalAmount,InterestAmount,HaveDuplicate,DuplicateID,InstallmentSLNo
                        ) VALUES (                     
                        @EmployeeLoanId,@EmployeeId,@InstallmentAmount,@InstallmentPaidAmount
                        ,@PaymentScheduleDate,@PaymentDate,@IsHold,@IsPaid,@Remarks
                        ,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                        ,@PrincipalAmount,@InterestAmount,@HaveDuplicate,@DuplicateID,@InstallmentSLNo
                        )  ";
                if (vm.IsApproved == true)
                {
                    foreach (var item in vm.employeeLoanDetails)
                    {
                        SqlCommand cmdInsD = new SqlCommand(sqlText, currConn);
                        cmdInsD.Parameters.AddWithValue("@EmployeeLoanId", vm.Id);
                        cmdInsD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                        cmdInsD.Parameters.AddWithValue("@InstallmentAmount", item.InstallmentAmount);
                        cmdInsD.Parameters.AddWithValue("@InstallmentPaidAmount", item.InstallmentPaidAmount);
                        cmdInsD.Parameters.AddWithValue("@PaymentScheduleDate", Ordinary.DateToString(item.PaymentScheduleDate));
                        cmdInsD.Parameters.AddWithValue("@PaymentDate", Ordinary.DateToString(item.PaymentDate));
                        cmdInsD.Parameters.AddWithValue("@IsHold", false);
                        cmdInsD.Parameters.AddWithValue("@IsPaid", false);
                        cmdInsD.Parameters.AddWithValue("@PrincipalAmount", item.PrincipalAmount);
                        cmdInsD.Parameters.AddWithValue("@InterestAmount", item.InterestAmount);
                        cmdInsD.Parameters.AddWithValue("@HaveDuplicate", false);
                        cmdInsD.Parameters.AddWithValue("@DuplicateID", 0);
                        cmdInsD.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                        cmdInsD.Parameters.AddWithValue("@IsActive", true);
                        cmdInsD.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsD.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdInsD.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdInsD.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                        cmdInsD.Parameters.AddWithValue("@InstallmentSLNo", item.InstallmentSLNo);
                        cmdInsD.Transaction = transaction;
                        cmdInsD.ExecuteNonQuery();
                    }
                }
                #endregion Save Detail
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
        public string[] EmployeeLoanUpdate(EmployeeLoanVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Update"; //Method Name
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
                if (vm.IsApproved == true)
                {
                    //throw new ArgumentNullException("This Loan already Approved" ,"");
                }
                #region Save
                sqlText = "  ";
                sqlText = @"Update EmployeeLoan set 
                                RefundAmount=@RefundAmount   
                                ,RefundDate=@RefundDate   
                                ,Remarks=@Remarks   
                                ,LastUpdateBy=@LastUpdateBy   
                                ,LastUpdateAt=@LastUpdateAt   
                                ,LastUpdateFrom=@LastUpdateFrom 
                                where Id=@id";
                SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                cmdIns.Parameters.AddWithValue("@Id", vm.Id);
                cmdIns.Parameters.AddWithValue("@RefundAmount", vm.RefundAmount);
                cmdIns.Parameters.AddWithValue("@RefundDate", Ordinary.DateToString(vm.RefundDate));
                cmdIns.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                cmdIns.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                cmdIns.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                cmdIns.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                cmdIns.Transaction = transaction;
                var exeRes = cmdIns.ExecuteScalar();
                Id = Convert.ToInt32(exeRes);
                #region Save Detail
                sqlText = "  ";
                sqlText = @"Update EmployeeLoanDetail set 
                                PrincipalAmount=@PrincipalAmount
                                ,InterestAmount=@InterestAmount
                                ,InstallmentAmount=@InstallmentAmount
                                ,Remarks=@Remarks   
                                ,LastUpdateBy=@LastUpdateBy   
                                ,LastUpdateAt=@LastUpdateAt   
                                ,LastUpdateFrom=@LastUpdateFrom 
                                where Id=@id";
                foreach (var item in vm.employeeLoanDetails)
                {
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    //cmdInsD.Parameters.AddWithValue("@EmployeeLoanId", vm.Id);
                    //cmdInsD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@InstallmentAmount", item.InstallmentAmount);
                    cmdUpdate.Parameters.AddWithValue("@PrincipalAmount", item.PrincipalAmount);
                    cmdUpdate.Parameters.AddWithValue("@InterestAmount", item.InterestAmount);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@id", item.Id);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.ExecuteNonQuery();
                }
                #endregion Save Detail
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
                retResults[1] = "Data Updated Successfully.";
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
        public string[] EmployeeLoanUpdate2(EmployeeLoanDetailVM loanDetail, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                //if ishold false then it must be duplicate or hold
                if (loanDetail.triger == "hold")
                {
                    sqlText = @"Update EmployeeLoanDetail set 
                                IsHold=@IsHold
                                ,Remarks=@Remarks   
                                ,LastUpdateBy=@LastUpdateBy   
                                ,LastUpdateAt=@LastUpdateAt   
                                ,LastUpdateFrom=@LastUpdateFrom 
                                where Id=@id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@IsHold", loanDetail.IsHold);
                    cmdUpdate.Parameters.AddWithValue("@id", loanDetail.Id);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", loanDetail.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", loanDetail.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", loanDetail.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", loanDetail.LastUpdateFrom);
                    cmdUpdate.ExecuteNonQuery();
                }
                else if (loanDetail.triger == "update")
                {
                    sqlText = @"Update EmployeeLoanDetail set 
                                InterestAmount=@InterestAmount
                                ,PrincipalAmount=@PrincipalAmount
                                ,InstallmentAmount=@InstallmentAmount
                                ,Remarks=@Remarks   
                                ,LastUpdateBy=@LastUpdateBy   
                                ,LastUpdateAt=@LastUpdateAt   
                                ,LastUpdateFrom=@LastUpdateFrom 
                                where Id=@id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@InterestAmount", loanDetail.InterestAmount);
                    cmdUpdate.Parameters.AddWithValue("@PrincipalAmount", loanDetail.PrincipalAmount);
                    cmdUpdate.Parameters.AddWithValue("@InstallmentAmount", loanDetail.InstallmentAmount);
                    cmdUpdate.Parameters.AddWithValue("@IsHold", loanDetail.IsHold);
                    cmdUpdate.Parameters.AddWithValue("@id", loanDetail.Id);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", loanDetail.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", loanDetail.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", loanDetail.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", loanDetail.LastUpdateFrom);
                    cmdUpdate.ExecuteNonQuery();
                }
                else if (loanDetail.triger == "duplicate")
                //else if (loanDetail.HaveDuplicate)
                {
                    EmployeeLoanDetailVM loanDetailOld = new EmployeeLoanDetailVM();
                    sqlText = "select  * from EmployeeLoanDetail where id=@id ";
                    SqlCommand detailSelect = new SqlCommand(sqlText, currConn, transaction);
                    detailSelect.Parameters.AddWithValue("@id", loanDetail.Id);
                    using (SqlDataReader dr = detailSelect.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            loanDetailOld.Id = Convert.ToInt32(dr["Id"]);
                            loanDetailOld.EmployeeLoanId = dr["EmployeeLoanId"].ToString();
                            loanDetailOld.EmployeeId = dr["EmployeeId"].ToString();
                            loanDetailOld.InstallmentAmount = Convert.ToDecimal(dr["InstallmentAmount"]);
                            loanDetailOld.InstallmentPaidAmount = Convert.ToDecimal(dr["InstallmentPaidAmount"]);
                            loanDetailOld.PaymentScheduleDate = dr["PaymentScheduleDate"].ToString();
                            loanDetailOld.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());
                            loanDetailOld.IsHold = Convert.ToBoolean(dr["IsHold"]);
                            loanDetailOld.HaveDuplicate = Convert.ToBoolean(dr["HaveDuplicate"]);
                            loanDetailOld.DuplicateID = Convert.ToInt32(dr["DuplicateID"]);
                            loanDetailOld.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                            loanDetailOld.Remarks = dr["Remarks"].ToString();
                            loanDetailOld.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"]);
                            loanDetailOld.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                        }
                        dr.Close();
                    }
                    string PaymentScheduleDate = new DateTime(Convert.ToInt32(loanDetailOld.PaymentScheduleDate.Substring(0, 4)), Convert.ToInt32(loanDetailOld.PaymentScheduleDate.Substring(4, 2)), Convert.ToInt32(loanDetailOld.PaymentScheduleDate.Substring(6, 2))).AddMonths(1).ToString("yyyyMMdd");
                    sqlText = @"   INSERT INTO EmployeeLoanDetail(
                        EmployeeLoanId,EmployeeId,InstallmentAmount,InstallmentPaidAmount,PaymentScheduleDate
                        ,PaymentDate,IsHold,IsPaid,Remarks,IsActive,IsArchive
                        ,CreatedBy,CreatedAt,CreatedFrom,PrincipalAmount,InterestAmount,HaveDuplicate,DuplicateID
                        ) VALUES (                     
                        @EmployeeLoanId,@EmployeeId,@InstallmentAmount,@InstallmentPaidAmount,@PaymentScheduleDate
                        ,@PaymentDate,@IsHold,@IsPaid,@Remarks,@IsActive,@IsArchive
                        ,@CreatedBy,@CreatedAt,@CreatedFrom,@PrincipalAmount,@InterestAmount,@HaveDuplicate,@DuplicateID
                        ) SELECT SCOPE_IDENTITY()";
                    SqlCommand cmdInsD = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsD.Parameters.AddWithValue("@EmployeeLoanId", loanDetailOld.EmployeeLoanId);
                    cmdInsD.Parameters.AddWithValue("@EmployeeId", loanDetailOld.EmployeeId);
                    cmdInsD.Parameters.AddWithValue("@InstallmentAmount", loanDetailOld.InstallmentAmount);
                    cmdInsD.Parameters.AddWithValue("@InstallmentPaidAmount", loanDetailOld.InstallmentPaidAmount);
                    cmdInsD.Parameters.AddWithValue("@PaymentScheduleDate", PaymentScheduleDate);
                    cmdInsD.Parameters.AddWithValue("@PaymentDate", Ordinary.DateToString(loanDetailOld.PaymentDate));
                    cmdInsD.Parameters.AddWithValue("@IsHold", false);
                    cmdInsD.Parameters.AddWithValue("@IsPaid", false);
                    cmdInsD.Parameters.AddWithValue("@PrincipalAmount", loanDetailOld.PrincipalAmount);
                    cmdInsD.Parameters.AddWithValue("@InterestAmount", loanDetailOld.InterestAmount);
                    cmdInsD.Parameters.AddWithValue("@HaveDuplicate", false);
                    cmdInsD.Parameters.AddWithValue("@DuplicateID", 0);
                    cmdInsD.Parameters.AddWithValue("@Remarks", loanDetailOld.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdInsD.Parameters.AddWithValue("@IsActive", true);
                    cmdInsD.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsD.Parameters.AddWithValue("@CreatedBy", loanDetail.LastUpdateBy);
                    cmdInsD.Parameters.AddWithValue("@CreatedAt", loanDetail.LastUpdateAt);
                    cmdInsD.Parameters.AddWithValue("@CreatedFrom", loanDetail.LastUpdateFrom);
                    var exeRes = cmdInsD.ExecuteScalar();
                    int newLoanDetailsId = Convert.ToInt32(exeRes);
                    sqlText = @"Update EmployeeLoanDetail set 
                            HaveDuplicate=@HaveDuplicate
                            ,DuplicateID=@newLoanDetailsId
                            ,InstallmentAmount=0
                            ,PrincipalAmount=0
                            ,InterestAmount=0
                            ,Remarks=@Remarks   
                            ,LastUpdateBy=@LastUpdateBy   
                            ,LastUpdateAt=@LastUpdateAt   
                            ,LastUpdateFrom=@LastUpdateFrom 
                            where Id=@id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@HaveDuplicate", true);
                    cmdUpdate.Parameters.AddWithValue("@newLoanDetailsId", newLoanDetailsId);
                    cmdUpdate.Parameters.AddWithValue("@id", loanDetail.Id);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", loanDetail.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", loanDetail.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", loanDetail.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", loanDetail.LastUpdateFrom);
                    cmdUpdate.ExecuteNonQuery();
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
        public string[] LoanUpdatePaid(string startDate, string endDate, bool IsPaid, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                //if ishold false then it must be duplicate or hold
                sqlText = @"Update EmployeeLoanDetail set 
                                IsPaid=@IsPaid
                                ,PaymentDate=@PaymentDate   
                               where 1=1 and  ishold=0 
 and isnull(nullif(IsManual,''),'0')=0
and HaveDuplicate=0
                                and PaymentScheduleDate between @startDate and  @endDate ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@IsPaid", IsPaid);
                cmdUpdate.Parameters.AddWithValue("@startDate", startDate);
                cmdUpdate.Parameters.AddWithValue("@endDate", endDate);
                cmdUpdate.Parameters.AddWithValue("@PaymentDate", IsPaid == true ? endDate : "");
                cmdUpdate.ExecuteNonQuery();

                if (IsPaid == false)
                {
                    sqlText = @"Delete from [PFLoanDetail]
                               where 1=1 
                                and PaymentScheduleDate between @startDate and  @endDate ";
                    SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                    cmdDelete.Parameters.AddWithValue("@startDate", startDate);
                    cmdDelete.Parameters.AddWithValue("@endDate", endDate);
                    cmdDelete.ExecuteNonQuery();
                }
                else
                {
                    sqlText = @"Delete from [PFLoanDetail]  where 1=1 
                                and PaymentScheduleDate between @startDate and  @endDate ";
                    SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                    cmdDelete.Parameters.AddWithValue("@startDate", startDate);
                    cmdDelete.Parameters.AddWithValue("@endDate", endDate);
                    cmdDelete.ExecuteNonQuery();

                    sqlText = @"
                Insert into [PFLoanDetail]([EmployeeLoanId]
                      ,[EmployeeId]
                      ,[InstallmentAmount]
                      ,[InstallmentPaidAmount]
                      ,[PaymentScheduleDate]
                      ,[PaymentDate]
                      ,[IsHold]
                      ,[IsManual]
                      ,[IsPaid]
                      ,[Remarks]
                      ,[IsActive]
                      ,[IsArchive]
                      ,[CreatedBy]
                      ,[CreatedAt]
                      ,[CreatedFrom]
                      ,[LastUpdateBy]
                      ,[LastUpdateAt]
                      ,[LastUpdateFrom]
                      ,[PrincipalAmount]
                      ,[InterestAmount]
                      ,[HaveDuplicate]
                      ,[DuplicateID])  
                SELECT 
                      [EmployeeLoanId]
                      ,[EmployeeId]
                      ,[InstallmentAmount]
                      ,[InstallmentPaidAmount]
                      ,[PaymentScheduleDate]
                      ,[PaymentDate]
                      ,[IsHold]
                      ,[IsManual]
                      ,[IsPaid]
                      ,[Remarks]
                      ,[IsActive]
                      ,[IsArchive]
                      ,[CreatedBy]
                      ,[CreatedAt]
                      ,[CreatedFrom]
                      ,[LastUpdateBy]
                      ,[LastUpdateAt]
                      ,[LastUpdateFrom]
                      ,[PrincipalAmount]
                      ,[InterestAmount]
                      ,[HaveDuplicate]
                      ,[DuplicateID]
                  FROM EmployeeLoanDetail where PaymentScheduleDate between @startDate and  @endDate ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@startDate", startDate);
                    cmdInsert.Parameters.AddWithValue("@endDate", endDate);
                    cmdInsert.ExecuteNonQuery();
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
        public string[] EmployeeLoanHold(EmployeeLoanVM vm, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                #region Save
                sqlText = @"update  EmployeeLoan set
                        IsHold=@IsHold      
                        ,Remarks=@Remarks      
                        ,LastUpdateBy=@LastUpdateBy      
                        ,LastUpdateAt=@LastUpdateAt      
                        ,LastUpdateFrom=@LastUpdateFrom      
                   where Id=@Id 
                        update  EmployeeLoanDetail set
                        IsHold=@IsHold      
                        ,Remarks=@Remarks      
                        ,LastUpdateBy=@LastUpdateBy      
                        ,LastUpdateAt=@LastUpdateAt      
                        ,LastUpdateFrom=@LastUpdateFrom      
                   where EmployeeLoadId=@Id 
";
                SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                cmdIns.Parameters.AddWithValue("@Id", vm.Id);
                cmdIns.Parameters.AddWithValue("@IsHold", vm.IsHold);
                cmdIns.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                cmdIns.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                cmdIns.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                cmdIns.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                cmdIns.Transaction = transaction;
                cmdIns.ExecuteNonQuery();
                #endregion Save
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
        public string[] PaidEdit(EmployeeLoanDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                //if ishold false then it must be duplicate or hold
                sqlText = @"Update EmployeeLoanDetail set IsManual=1, IsPaid=@IsPaid, PaymentDate=@PaymentDate, Remarks=@Remarks, LastUpdateAt=@LastUpdateAt,LastUpdateBy=@LastUpdateBy,LastUpdateFrom=@LastUpdateFrom
                               where 1=1 and  Id=@Id";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                //cmdUpdate.Parameters.AddWithValue("@EmployeeLoanId", vm.EmployeeLoanId);
                cmdUpdate.Parameters.AddWithValue("@IsPaid", true);
                cmdUpdate.Parameters.AddWithValue("@PaymentDate", Ordinary.DateToString(vm.PaymentDate));
                cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                cmdUpdate.ExecuteNonQuery();
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
        public string[] EmployeeLoanDetailHold(EmployeeLoanDetailVM dVM, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                sqlText = @" 
                        update  EmployeeLoanDetail set
                        IsHold=@IsHold      
                        ,Remarks=@Remarks      
                        ,LastUpdateBy=@LastUpdateBy      
                        ,LastUpdateAt=@LastUpdateAt      
                        ,LastUpdateFrom=@LastUpdateFrom      
                   where EmployeeLoadId=@Id 
";
                SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                cmdIns.Parameters.AddWithValue("@Id", dVM.Id);
                cmdIns.Parameters.AddWithValue("@IsHold", dVM.IsHold);
                cmdIns.Parameters.AddWithValue("@Remarks", dVM.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                cmdIns.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                cmdIns.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                cmdIns.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                cmdIns.Transaction = transaction;
                cmdIns.ExecuteNonQuery();
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
        public string[] EmployeeLoanDetailPayment(EmployeeLoanDetailVM dVM, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                sqlText = @" 
                        update  EmployeeLoanDetail set
                        InstallmentPaidAmount=@InstallmentPaidAmount      
                        ,PaymentDate=@PaymentDate      
                        ,Remarks=@Remarks      
                        ,LastUpdateBy=@LastUpdateBy      
                        ,LastUpdateAt=@LastUpdateAt      
                        ,LastUpdateFrom=@LastUpdateFrom      
                   where EmployeeLoadId=@Id 
";
                SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                cmdIns.Parameters.AddWithValue("@Id", dVM.Id);
                cmdIns.Parameters.AddWithValue("@InstallmentPaidAmount", dVM.InstallmentPaidAmount);
                cmdIns.Parameters.AddWithValue("@PaymentDate", Ordinary.DateToString(dVM.PaymentDate.ToString()));
                cmdIns.Parameters.AddWithValue("@Remarks", dVM.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                cmdIns.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                cmdIns.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                cmdIns.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                cmdIns.Transaction = transaction;
                cmdIns.ExecuteNonQuery();
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
        public List<EmployeeLoanDetailVM> SelectAllForReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string BranchId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLoanDetailVM> VMs = new List<EmployeeLoanDetailVM>();
            EmployeeLoanDetailVM vm;
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
select
 el.BranchId
,lt.Name LoanType_E
,el.EmployeeId
,el.PrincipalAmount
,el.InterestPolicy
,el.InterestRate
,el.InterestAmount
,el.TotalAmount
,el.NumberOfInstallment
,el.ApprovedDate
,el.StartDate
,el.EndDate
,el.ApplicationDate
,el.RefundAmount
,el.RefundDate 
,el.IsApproved
,el.Remarks
,fyd.PeriodName
,eld.PrincipalAmount [InstallmentPrincipalAmount]
,eld.InterestAmount [InstallmentInterestAmount]
,eld.InstallmentAmount
,eld.InstallmentPaidAmount
,eld.PaymentScheduleDate
,eld.PaymentDate
,eld.IsHold
,eld.IsPaid
,eld.HaveDuplicate
,emp.Code
,emp.EmpName
,emp.JoinDate
,emp.Project
,emp.Branch
,emp.Department
,emp.Section
,emp.Designation
,emp.Grade
,eld.EmployeeLoanId
,emp.ProjectId
,emp.SectionId
,emp.DepartmentId
,emp.DesignationId
,emp.GrossSalary
,emp.BasicSalary
 from EmployeeLoanDetail eld
    left outer join FiscalYearDetail fyd on eld.PaymentScheduleDate between  fyd.periodstart and fyd.PeriodEnd
    left outer join EmployeeLoan el on eld.EmployeeLoanId=el.id
    left outer join ViewEmployeeInformation emp on emp.employeeid=el.EmployeeId
    left outer join EnumLoanType lt on lt.Id = el.LoanType_E
     where 1=1 
";
                //if (fid != null && fid != 0)
                //{
                //    sqlText += @" and slnd.FiscalYearDetailId='" + fid + "'";
                //}
                if (ProjectId != "0_0")
                    sqlText += " and emp.ProjectId=@ProjectId";
                if (DepartmentId != "0_0")
                    sqlText += " and emp.DepartmentId=@DepartmentId ";
                if (SectionId != "0_0")
                    sqlText += " and emp.SectionId=@SectionId ";
                if (DesignationId != "0_0")
                    sqlText += " and emp.DesignationId=@DesignationId ";
                if (CodeF != "0_0")
                    sqlText += " and emp.Code>= @CodeF";
                if (CodeT != "0_0")
                    sqlText += " and emp.Code<= @CodeT";
                if (BranchId != "")
                    sqlText += " and el.BranchId= @BranchId";
                sqlText += " ORDER BY fyd.Id, emp.Department, emp.Section, emp.Code ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (ProjectId != "0_0")
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                if (DepartmentId != "0_0")
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                if (SectionId != "0_0")
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                if (DesignationId != "0_0")
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                if (CodeF != "0_0")
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                if (CodeT != "0_0")
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                if (BranchId != "")
                    objComm.Parameters.AddWithValue("@BranchId", BranchId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLoanDetailVM();
                    vm.LoanType_E = dr["LoanType_E"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"]);
                    vm.InterestPolicy = dr["InterestPolicy"].ToString();
                    vm.InterestRate = Convert.ToDecimal(dr["InterestRate"]);
                    vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                    vm.NumberOfInstallment = Convert.ToInt32(dr["NumberOfInstallment"]);
                    vm.ApprovedDate = Ordinary.StringToDate(dr["ApprovedDate"].ToString());
                    vm.StartDate = Ordinary.StringToDate(dr["StartDate"].ToString());
                    vm.EndDate = Ordinary.StringToDate(dr["EndDate"].ToString());
                    vm.ApplicationDate = Ordinary.StringToDate(dr["ApplicationDate"].ToString());
                    vm.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.InstallmentPrincipalAmount = Convert.ToDecimal(dr["InstallmentPrincipalAmount"]);
                    vm.InstallmentInterestAmount = Convert.ToDecimal(dr["InstallmentInterestAmount"]);
                    vm.InstallmentAmount = Convert.ToDecimal(dr["InstallmentAmount"]);
                    vm.InstallmentPaidAmount = Convert.ToDecimal(dr["InstallmentPaidAmount"]);
                    vm.PaymentScheduleDate = Ordinary.StringToDate(dr["PaymentScheduleDate"].ToString());
                    vm.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());
                    vm.IsHold = Convert.ToBoolean(dr["IsHold"]);
                    vm.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                    vm.HaveDuplicate = Convert.ToBoolean(dr["HaveDuplicate"]);
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Project = dr["Project"].ToString();
                    vm.Branch = dr["Branch"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.EmployeeLoanId = dr["EmployeeLoanId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    VMs.Add(vm);
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
            return VMs;
        }
        public List<EmployeeLoanDetailVM> SelectLoanStatementForReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLoanDetailVM> VMs = new List<EmployeeLoanDetailVM>();
            EmployeeLoanDetailVM vm;
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
       EmpName
      ,Code
      ,Designation
      ,NumberOfInstallment
      ,LoanNo
      ,FORMAT(CAST(ApprovedDate AS DATE), 'dd-MMM-yyyy') ApprovedDate
      ,SUM(ROUND(PrincipalAmount, 0)) InstallmentAmount
      ,SUM(ROUND(InterestAmount, 0)) InterestAmount
      ,SUM(ROUND(PrincipalAmountPaid, 0)) PrincipalAmountPaid
      ,SUM(ROUND(InterestAmountPaid, 0)) InterestAmountPaid
  FROM View_LoanDetails
     where 1=1 
";
                //if (fid != null && fid != 0)
                //{
                //    sqlText += @" and slnd.FiscalYearDetailId='" + fid + "'";
                //}
                if (ProjectId != "0_0")
                    sqlText += " and emp.ProjectId=@ProjectId";
                if (DepartmentId != "0_0")
                    sqlText += " and emp.DepartmentId=@DepartmentId ";
                if (SectionId != "0_0")
                    sqlText += " and emp.SectionId=@SectionId ";
                if (DesignationId != "0_0")
                    sqlText += " and emp.DesignationId=@DesignationId ";
                if (CodeF != "0_0")
                    sqlText += " and emp.Code>= @CodeF";
                if (CodeT != "0_0")
                    sqlText += " and emp.Code<= @CodeT";
                //if (BranchId != "")
                //{
                //    sqlText += " and BranchId=@BranchId";
                //}
                sqlText += " group by EmpName,Code,Designation,EmployeeLoanId,NumberOfInstallment,ApprovedDate,LoanNo Order by Code ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (ProjectId != "0_0")
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                if (DepartmentId != "0_0")
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                if (SectionId != "0_0")
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                if (DesignationId != "0_0")
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                if (CodeF != "0_0")
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                if (CodeT != "0_0")
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                //if (BranchId != "")
                //    objComm.Parameters.AddWithValue("@BranchId", BranchId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLoanDetailVM();

                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();                   
                    vm.Designation = dr["Designation"].ToString();
                    vm.NumberOfInstallment =Convert.ToInt32(dr["NumberOfInstallment"].ToString());
                    vm.InstallmentAmount = Convert.ToDecimal(dr["InstallmentAmount"].ToString());
                    vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"].ToString());
                    vm.PrincipalAmountPaid = Convert.ToDecimal(dr["PrincipalAmountPaid"].ToString());
                    vm.InterestAmountPaid = Convert.ToDecimal(dr["InterestAmountPaid"].ToString());
                    vm.ApprovedDate = dr["ApprovedDate"].ToString();
                    vm.LoanNo = dr["LoanNo"].ToString();                  
                
                    VMs.Add(vm);
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
            return VMs;
        }
        public List<EmployeeLoanDetailVM> SelectAllForSLMPeport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string FromDate, string ToDate, string BranchId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLoanDetailVM> VMs = new List<EmployeeLoanDetailVM>();
            EmployeeLoanDetailVM vm;
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
with cat as (
select
emp.Code
,emp.EmpName
,emp.Designation
,el.PrincipalAmount
,el.InterestAmount
,case when IsPaid=1 then  eld.PrincipalAmount else 0 end as InstallmentPrincipalAmount
,case when IsPaid=1 then eld.InterestAmount else 0  end as InstallmentInterestAmount
,eld.IsPaid
,eld.EmployeeLoanId
,eld.PaymentScheduleDate
 from EmployeeLoanDetail eld
    left outer join FiscalYearDetail fyd on eld.PaymentScheduleDate between  fyd.periodstart and fyd.PeriodEnd
    left outer join EmployeeLoan el on eld.EmployeeLoanId=el.id
    left outer join ViewEmployeeInformation emp on emp.employeeid=el.EmployeeId
    left outer join EnumLoanType lt on lt.Id = el.LoanType_E
     where 1=1 

";
                //if (fid != null && fid != 0)
                //{
                //    sqlText += @" and slnd.FiscalYearDetailId='" + fid + "'";
                //}
                if (ProjectId != "0_0")
                    sqlText += " and emp.ProjectId=@ProjectId";
                if (DepartmentId != "0_0")
                    sqlText += " and emp.DepartmentId=@DepartmentId ";
                if (SectionId != "0_0")
                    sqlText += " and emp.SectionId=@SectionId ";
                if (DesignationId != "0_0")
                    sqlText += " and emp.DesignationId=@DesignationId ";
                if (CodeF != "0_0")
                    sqlText += " and emp.Code>= @CodeF";
                if (CodeT != "0_0")
                    sqlText += " and emp.Code<= @CodeT";
                if (BranchId != "0_0")
                    sqlText += " and el.BranchId<= @BranchId";
                sqlText += @" ) Select Code,EmpName,Designation, SUM(distinct PrincipalAmount) as PrincipalAmount,SUM(distinct InterestAmount) as InterestAmount,
                SUM(InstallmentPrincipalAmount) as InstallmentPrincipalAmount, SUM(InstallmentInterestAmount) as InstallmentInterestAmount
                from cat ";
                  if (FromDate != "")
                    sqlText += " where PaymentScheduleDate between @FromDate and @ToDate";
                sqlText += @" group by Code,EmpName,Designation,EmployeeLoanId  ORDER BY Code";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (ProjectId != "0_0")
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                if (DepartmentId != "0_0")
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                if (SectionId != "0_0")
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                if (DesignationId != "0_0")
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                if (CodeF != "0_0")
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                if (CodeT != "0_0")
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                if (FromDate != "")
                    objComm.Parameters.AddWithValue("@FromDate", FromDate);
                if (ToDate != "")
                    objComm.Parameters.AddWithValue("@ToDate", ToDate);
                if (BranchId != "")
                    objComm.Parameters.AddWithValue("@BranchId", BranchId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLoanDetailVM();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"]);
                    vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                    vm.InstallmentPrincipalAmount = Convert.ToDecimal(dr["InstallmentPrincipalAmount"]);
                    vm.InstallmentInterestAmount = Convert.ToDecimal(dr["InstallmentInterestAmount"]);

                    VMs.Add(vm);
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
            return VMs;
        }

        ////==================Delete =================
        public string[] Delete(EmployeeLoanVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeLoan"; //Method Name
            int transResult = 0;
            string sqlText = "";
            string retVal = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeLoan"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length >= 1)
                {
                    #region Check Already Used

                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "  ";
                        sqlText += @" SELECT COUNT(DISTINCT Id)Id FROM EmployeeLoanDetail 
                                      WHERE 1=1 and EmployeeLoanId=@EmployeeLoanId
									  and (IsPaid = 1 or IsHold = 1 or HaveDuplicate = 1)";
                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn, transaction);
                        cmdExist.Parameters.AddWithValue("@EmployeeLoanId", ids[i]);
                        var exec = cmdExist.ExecuteScalar();
                        int objfoundId = Convert.ToInt32(exec);

                        if (objfoundId > 0)
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Loan Already Used! Cannot be Deleted.";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                        #endregion Check Already Used
                    #region Delete Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {

                        sqlText = "";
                        sqlText += " ";
                        sqlText += "DELETE EmployeeLoanDetail";
                        sqlText += " WHERE EmployeeLoanId=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        //////////if (transResult <= 0)
                        //////////{
                        //////////    retResults[3] = sqlText;
                        //////////    throw new ArgumentNullException("Unexpected error to update EmployeeLoanDetail.", "");
                        //////////}
                        sqlText = " ";
                        sqlText = "DELETE EmployeeLoan";
                        sqlText += " WHERE Id=@Id";
                        cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update EmployeeLoan.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EmployeeLoan Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("EmployeeLoan Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
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
      
        public DataTable getBalance(string date, string emploanId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            string HRMHRMDB = "";
            EmployeeInfoVM varEmployeeInfoVM = new EmployeeInfoVM();
            EmployeeInfoDAL _EmployeeInfoDAL = new EmployeeInfoDAL();
            #endregion
            try
            {
                HRMHRMDB = new AppSettingsReader().GetValue("PFDB", typeof(string)).ToString();
                string pfDb = new AppSettingsReader().GetValue("PFDB", typeof(string)).ToString();

                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                SettingDAL _settingDal = new SettingDAL();
                var tt = _settingDal.settingValue("PFLoan", "BothContributionJobAge");

                int bothContributionJobAge = Convert.ToInt32(tt);

                varEmployeeInfoVM = new EmployeeInfoVM();

                sqlText = @"SELECT JoinDate From ViewEmployeeInformation
                      where  EmployeeId=@emploanId";
              
                SqlCommand cmd = new SqlCommand(sqlText, currConn,transaction);
                cmd.Parameters.AddWithValue("@emploanId", emploanId);
                string JoinDate = cmd.ExecuteScalar().ToString();

                DateTime doj = Convert.ToDateTime(Ordinary.StringToDate(JoinDate));
                double JobDay = ((Convert.ToDateTime(date) - doj).TotalDays)/365;// Ordinary.CalculateDayBetween(doj, Convert.ToDateTime(date));
                JobDay = Math.Round(JobDay, MidpointRounding.AwayFromZero);

             sqlText = @"
----declare @EmployeeId varchar(100) = '1_1'
----declare @ToDate varchar(14) = 20200318
";
             if (JobDay >= bothContributionJobAge)
             {
                 sqlText += @" select EmployeeId, ((EmployeeContribution*2)-LoanAmount+PaymentAmount) Balance";

             }
             else
             {
                 sqlText += @" select EmployeeId, (EmployeeContribution-LoanAmount+PaymentAmount) Balance";

             }


  sqlText += @" 

from
(
select EmployeeId, Sum(EmployeeContribution)EmployeeContribution, Sum(LoanAmount)LoanAmount, Sum(PaymentAmount)PaymentAmount
from
(
select fyd.PeriodEnd TransactionDate, pfd.EmployeeId, pfd.EmployeePFValue EmployeeContribution
,0 LoanAmount, 0 PaymentAmount, 'Contribution' TransactionType
from PFDetails pfd
left outer join FiscalYearDetail fyd on pfd.FiscalYearDetailId=fyd.Id
where 1=1 and EmployeeId=@EmployeeId and fyd.PeriodEnd <=@ToDate



union all
select StartDate, EmployeeId, 0 EmployeeContribution
,TotalAmount LoanAmount, 0 PaymentAmount, 'Loan' TransactionType 
from EmployeeLoan loan
left outer join EnumLoanType elt on elt.id=loan.LoanType_E
where 1=1 and loan.EmployeeId=@EmployeeId and loan.IsApproved=1
and elt.Name = 'PF Loan' and loan.StartDate <= @ToDate

) as a
group by EmployeeId
) as bal



";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                da.SelectCommand.Parameters.AddWithValue("@EmployeeId", emploanId);
                da.SelectCommand.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(date));

                da.Fill(dt);

                //dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");


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
            return dt;



        }

        public DataTable GetData(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 I.Id
,I.BranchId
,I.LoanType_E
,I.EmployeeId
,ve.EmpName, ve.Code, ve.Designation, ve.Department, ve.section
,I.IsFixed
,I.InterestPolicy
,I.InterestRate
,I.TotalAmount
,I.NumberOfInstallment
,FORMAT(CAST(I.StartDate AS DATE), 'MMM-yyyy') As StartDate
,FORMAT(CAST(I.EndDate AS DATE), 'MMM-yyyy') As EndDate
,FORMAT(CAST(I.StartDate AS DATE), 'yyyy') +'-'+ FORMAT(CAST(I.EndDate AS DATE), 'yy') AS  FyscalYear
,I.IsHold
,I.IsApproved
,I.ApplicationDate
,I.ApprovedDate
,I.RefundAmount
,I.LoanNo
,isnull(I.RefundDate, '') RefundDate
,t.Name LoanType
 ,ELD.Id
,ELD.EmployeeLoanId
,ELD.EmployeeId
,ELD.InstallmentAmount
,ELD.InstallmentPaidAmount
,ELD.PaymentScheduleDate
,ELD.PaymentDate
,case when ELD.IsHold=0  then 'N' else 'Y'
end IsHold
,case when ELD.IsPaid=0  then 'N' else 'Y'
end IsPaid
,case when ELD.HaveDuplicate=0  then 'N' else 'Y'
end HaveDuplicate
,ELD.DuplicateID
,ELD.Remarks
,ELD.PrincipalAmount
,ELD.InterestAmount
, SUM(CASE WHEN ELD.IsPaid = '1' THEN ELD.PrincipalAmount ELSE 0 END) OVER () AS PrincipalPaid
, SUM(CASE WHEN ELD.IsPaid = '1' THEN ELD.InterestAmount ELSE 0 END) OVER () AS InterestPaid
,I.PrincipalAmount as TotalPrincipalAmount
from EmployeeLoanDetail ELD 
left outer join dbo.EmployeeLoan I on ELD.EmployeeLoanId=I.Id
left outer join ViewEmployeeInformation ve on I.EmployeeId=ve.EmployeeId
left outer join EnumLoanType t on t.Id=I.LoanType_E
where ELD.IsArchive=0 
 
";

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


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
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

                dt = Ordinary.DtColumnStringToDate(dt, "ApplicationDate");
                dt = Ordinary.DtColumnStringToDate(dt, "ApprovedDate");
                dt = Ordinary.DtColumnStringToDate(dt, "PaymentScheduleDate");

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }

            #region catch

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

            return dt;
        }

        public DataTable GetIndividualLoanData(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
    0 AS RowNum,
    I.Id,
    I.BranchId,
    I.LoanType_E,
    I.EmployeeId,
    ve.EmpName, 
    ve.Code AS Code, 
    ve.Designation,  
    ve.Department, 
    ve.section,  
    0 AS InterestRate,
    I.PrincipalAmount TotalPrincipalAmount, 
    I.InterestAmount TotalInterestAmount,
    I.TotalAmount,
    0 AS NumberOfInstallment,
    0 AS InstallmentSLNo,
	'' PaymentScheduleDate,
	I.StartDate,
	I.ApprovedDate,
	I.InterestPolicy,
	I.LoanNo,
    SUM(CASE WHEN ELD.IsPaid = '1' THEN ELD.PrincipalAmount ELSE 0 END) AS PrincipalAmount,
    SUM(CASE WHEN ELD.IsPaid = '1' THEN ELD.InterestAmount ELSE 0 END) AS InterestAmount,
    SUM(CASE WHEN ELD.IsPaid = '1' THEN ELD.PrincipalAmount ELSE 0 END) AS PrincipalPaid,
    SUM(CASE WHEN ELD.IsPaid = '1' THEN ELD.InterestAmount ELSE 0 END) AS InterestPaid
FROM 
    EmployeeLoanDetail ELD
LEFT OUTER JOIN 
    EmployeeLoan I ON ELD.EmployeeLoanId = I.Id
LEFT OUTER JOIN 
    ViewEmployeeInformation ve ON I.EmployeeId = ve.EmployeeId
LEFT OUTER JOIN 
    EnumLoanType t ON t.Id = I.LoanType_E
WHERE 
    ELD.IsArchive = 0 
    AND ELD.PaymentScheduleDate <@StartDate
GROUP BY 
    I.Id, I.BranchId, I.LoanType_E, I.EmployeeId,ve.EmpName,ve.Code
,ve.Designation, 
    ve.Department, 
    ve.section, 
    I.InterestRate,
    I.PrincipalAmount, 
    I.InterestAmount,
    I.TotalAmount,
    I.NumberOfInstallment,
	I.StartDate,
	I.ApprovedDate,
	I.InterestPolicy,
	I.LoanNo

UNION ALL

-- Original Query
SELECT
    ROW_NUMBER() OVER (PARTITION BY I.Id ORDER BY ELD.PaymentScheduleDate) AS RowNum,
    I.Id,
    I.BranchId,
    I.LoanType_E,
    I.EmployeeId,
    ve.EmpName, 
    ve.Code, 
    ve.Designation, 
    ve.Department, 
    ve.section,
    I.InterestRate,
    I.PrincipalAmount TotalPrincipalAmount,
    I.InterestAmount TotalInterestAmount,
    I.TotalAmount,
    I.NumberOfInstallment,
    ELD.InstallmentSLNo,
	ELD.PaymentScheduleDate,
	I.StartDate,
	I.ApprovedDate,
	I.InterestPolicy,
	I.LoanNo,
    CASE 
        WHEN ELD.IsPaid = '1' THEN ELD.PrincipalAmount 
        ELSE 0 
    END AS PrincipalAmount,
    CASE 
        WHEN ELD.IsPaid = '1' THEN ELD.InterestAmount 
        ELSE 0 
    END AS InterestAmount,
    CASE 
        WHEN ELD.IsPaid = '1' THEN ELD.PrincipalAmount 
        ELSE 0 
    END AS PrincipalPaid,
    CASE 
        WHEN ELD.IsPaid = '1' THEN ELD.InterestAmount 
        ELSE 0 
    END AS InterestPaid
FROM 
    EmployeeLoanDetail ELD 
LEFT OUTER JOIN 
    EmployeeLoan I ON ELD.EmployeeLoanId = I.Id
LEFT OUTER JOIN 
    ViewEmployeeInformation ve ON I.EmployeeId = ve.EmployeeId
LEFT OUTER JOIN 
    EnumLoanType t ON t.Id = I.LoanType_E
WHERE 
    ELD.IsArchive = 0 
    AND ELD.PaymentScheduleDate >= @StartDate 
    AND ELD.PaymentScheduleDate <= @EndDate
    and I.BranchId= @BranchId
 ";              
                #endregion SqlText

                #region SqlExecution

                if (conditionValues[2].ToString() != "[All]")
                    sqlText += " and Code>= @CodeF";
                if (conditionValues[3].ToString()!="[All]")
                    sqlText += " and Code<= @CodeT";
               

                using (SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction))
                {
                    cmd.Parameters.AddWithValue("@StartDate", conditionValues[0]);
                    cmd.Parameters.AddWithValue("@EndDate", conditionValues[1]);
                    if (conditionValues[2].ToString() != "[All]")
                    {
                        cmd.Parameters.AddWithValue("@CodeF", conditionValues[2]);
                    }
                    if (conditionValues[3].ToString() != "[All]")
                    {
                        cmd.Parameters.AddWithValue("@CodeT", conditionValues[3]);
                    }
                    if (conditionValues[4].ToString() != "")
                    {
                        cmd.Parameters.AddWithValue("@BranchId", conditionValues[4]);
                    }       

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }

            #region catch

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

            return dt;
        }

        public DataTable GetSummeryLoanData(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
DECLARE @cols AS NVARCHAR(MAX),
        @query AS NVARCHAR(MAX);

-- Generate columns for PrincipalAmount and InterestAmount for each month with month names
SELECT @cols = STUFF((SELECT DISTINCT ',' + QUOTENAME(DATENAME(MONTH, PaymentScheduleDate) + '_Principle') 
                      + ',' + QUOTENAME(DATENAME(MONTH, PaymentScheduleDate) + '_Interest')
                      FROM (
                            SELECT DATEADD(MONTH, DATEDIFF(MONTH, 0, PaymentScheduleDate), 0) AS PaymentScheduleDate
                            FROM EmployeeLoanDetail
                            WHERE PaymentScheduleDate >= @StartDate AND PaymentScheduleDate <= @EndDate
                            GROUP BY DATEADD(MONTH, DATEDIFF(MONTH, 0, PaymentScheduleDate), 0)
                           ) AS MonthlyDates
                      FOR XML PATH(''), TYPE
                      ).value('.', 'NVARCHAR(MAX)') 
                     ,1,1,'');

-- Construct the dynamic SQL query
SET @query = '
    WITH PrincipalPivot AS (
        SELECT 
            I.LoanNo,
            I.BranchId,
            I.LoanType_E,
            I.EmployeeId,
            ve.EmpName,
			ve.Code,
            DATENAME(MONTH, ELD.PaymentScheduleDate) AS PaymentScheduleMonth,
            SUM(ELD.PrincipalAmount) AS PrincipalAmount
        FROM 
           EmployeeLoan I
        LEFT OUTER JOIN 
            EmployeeLoanDetail ELD ON ELD.EmployeeLoanId = I.Id
        LEFT OUTER JOIN 
            ViewEmployeeInformation ve ON I.EmployeeId = ve.EmployeeId
        WHERE 
            1 = 1 
            AND ELD.PaymentScheduleDate >= @StartDate
            AND ELD.PaymentScheduleDate <=@EndDate
           and ELD.IsPaid=1 and I.BranchId=@BranchId
        GROUP BY 
            I.LoanNo,
            I.BranchId,
            I.LoanType_E,
            I.EmployeeId,
            ve.EmpName,
			ve.Code,
            DATENAME(MONTH, ELD.PaymentScheduleDate)
    ),
    InterestPivot AS (
        SELECT 
            I.LoanNo,
            I.BranchId,
            I.LoanType_E,
            I.EmployeeId,
            ve.EmpName,
			ve.Code,
            DATENAME(MONTH, ELD.PaymentScheduleDate) AS PaymentScheduleMonth,
            SUM(ELD.InterestAmount) AS InterestAmount
        FROM 
            EmployeeLoan I
        LEFT OUTER JOIN 
            EmployeeLoanDetail ELD ON ELD.EmployeeLoanId = I.Id
        LEFT OUTER JOIN 
            ViewEmployeeInformation ve ON I.EmployeeId = ve.EmployeeId
        WHERE 
            1 = 1 
            AND ELD.PaymentScheduleDate >= @StartDate
            AND ELD.PaymentScheduleDate <=@EndDate
            and ELD.IsPaid=1 and I.BranchId=@BranchId
        GROUP BY 
            I.LoanNo,
            I.BranchId,
            I.LoanType_E,
            I.EmployeeId,
            ve.EmpName,
			ve.Code,
            DATENAME(MONTH, ELD.PaymentScheduleDate)
    ),
    PivotData AS (
        SELECT 
            LoanNo, 
            BranchId,
            LoanType_E,
            EmployeeId,
            EmpName,
			Code,
            PaymentScheduleMonth + ''_Principle'' AS ColumnName,
            PrincipalAmount AS Amount
        FROM PrincipalPivot
        UNION ALL
        SELECT 
            LoanNo,
            BranchId,
            LoanType_E,
            EmployeeId,
            EmpName,
			Code,
            PaymentScheduleMonth + ''_Interest'' AS ColumnName,
            InterestAmount AS Amount
        FROM InterestPivot
    ),
    PivotTable AS (
        SELECT 
            LoanNo, 
            BranchId,
            LoanType_E,
            EmployeeId,
            EmpName,
			Code,
            ' + @cols + '
        FROM
        (
            SELECT LoanNo, BranchId, LoanType_E, EmployeeId, EmpName,Code, ColumnName, Amount
            FROM PivotData
        ) AS SourceTable
        PIVOT
        (
            MAX(Amount) FOR ColumnName IN (' + @cols + ')
        ) AS PivotTable
    )
    SELECT 
        LoanNo, 
        BranchId,
        LoanType_E,
        EmployeeId,
        EmpName,
		Code,
        ' + @cols + ',
        ISNULL(
            (SELECT SUM(Amount) 
             FROM PivotData
             WHERE PivotData.LoanNo = PT.LoanNo
               AND PivotData.BranchId = PT.BranchId
               AND PivotData.LoanType_E = PT.LoanType_E
               AND PivotData.EmployeeId = PT.EmployeeId
               AND PivotData.ColumnName LIKE ''%_Principle%''), 0) AS TotalPriciple,
        ISNULL(
            (SELECT SUM(Amount) 
             FROM PivotData
             WHERE PivotData.LoanNo = PT.LoanNo
               AND PivotData.BranchId = PT.BranchId
               AND PivotData.LoanType_E = PT.LoanType_E
               AND PivotData.EmployeeId = PT.EmployeeId
               AND PivotData.ColumnName LIKE ''%_Interest%''), 0) AS TotalInterest
    FROM PivotTable PT
    ORDER BY LoanNo, BranchId, LoanType_E, EmployeeId'
 
-- Execute the dynamic SQL
EXEC sp_executesql @query, N'@StartDate DATE, @EndDate DATE @BranchId INT', @StartDate, @EndDate,@BranchId;

 ";
                #endregion SqlText

                #region SqlExecution

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


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
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

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }

            #region catch

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

            return dt;
        }

        public string[] UpdateSettelment(string loanId, decimal TotalDuePrincipalAmount, decimal TotalDueInterestAmount, string EarlySellteDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "UpdateLoanSettelment"; //Method Name
            int transResult = 0;
            string sqlText = "";
            string retVal = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            EmployeeLoanVM vm = new EmployeeLoanVM();

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeLoan"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (loanId.Length >= 1)
                {
                    sqlText = " ";
                    sqlText += "Update EmployeeLoan set EarlySelltePrincipleAmount=@EarlySelltePrincipleAmount,EarlySellteInterestAmount=@EarlySellteInterestAmount,EarlySellteDate=@EarlySellteDate";
                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                    cmdDelete.Parameters.AddWithValue("@Id", loanId);
                    cmdDelete.Parameters.AddWithValue("@EarlySelltePrincipleAmount", TotalDuePrincipalAmount);
                    cmdDelete.Parameters.AddWithValue("@EarlySellteInterestAmount", TotalDueInterestAmount);
                    cmdDelete.Parameters.AddWithValue("@EarlySellteDate", EarlySellteDate);
                    var exeRes = cmdDelete.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);                  
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update EmployeeLoan.", "");
                    }
                }
                else
                {
                    throw new ArgumentNullException("Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Updated Successfully.";
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

        public string[] ApprovedSettelment(string loanId,string EarlySellteDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ApproveLoanSettelment"; //Method Name
            int transResult = 0;
            string sqlText = "";
            string retVal = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            EmployeeLoanVM vm = new EmployeeLoanVM();

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeLoan"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (loanId.Length >= 1)
                {
                    sqlText = "";
                    sqlText += "Update EmployeeLoan set IsEarlySellte=@IsEarlySellte,IsActive=@IsActive,IsArchive=@IsArchive ";
                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValue("@Id", loanId);
                    cmd.Parameters.AddWithValue("@IsEarlySellte", true);
                    cmd.Parameters.AddWithValue("@IsActive", false);
                    cmd.Parameters.AddWithValue("@IsArchive", true);
                    var exeRes = cmd.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Unexpected error to Approved EmployeeLoan.", "");
                    }
                    sqlText = "";
                    sqlText = "Update EmployeeLoanDetail set IsPaid=@IsPaid,PaymentDate=@PaymentDate";
                    sqlText += " WHERE EmployeeLoanId=@Id and IsPaid=0";
                    SqlCommand cmdupdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdupdate.Parameters.AddWithValue("@Id", loanId);
                    cmdupdate.Parameters.AddWithValue("@IsPaid", true);               
                    cmdupdate.Parameters.AddWithValue("@PaymentDate", EarlySellteDate);
                    var exeRed = cmdupdate.ExecuteNonQuery();

                    transResult = Convert.ToInt32(exeRed);
                    if (transResult <= 0)
                    {                       
                        throw new ArgumentNullException("Unexpected error to update Paid EmployeeLoan.", "");
                    }
                }
                else
                {
                    throw new ArgumentNullException("Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Loan settelment approved successfully.";
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
    }
}
