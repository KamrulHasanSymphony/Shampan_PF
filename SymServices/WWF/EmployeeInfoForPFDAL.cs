using Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.WWF
{
    public class EmployeeInfoForPFDAL
    {
        #region Global Variables
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        public string FieldDelimeter { get; set; }

        /// <summary>
        /// Inserts a new Employee Break Month record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="EmployeeBreakMonthVM"/> containing the Employee Break Month data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any)    
        /// </returns>
        public string[] InsertEmployeeInfoForPF(EmployeeInfoForPFVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeInfoForPF"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(EmployeeInfoForPFVM.Id))
                //{
                //    retResults[1] = "Please Input InsertEmployeeInfoForPF";
                //    return retResults;
                //}
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

                if (vm != null)
                {
                    if (vm.Id > 0)
                    {
                        sqlText = "  ";
                        sqlText += @" Update EmployeeInfo set
                                     Code=@Code
                                     ,Name=@Name
                                     ,Department=@Department
                                     ,Designation =@Designation
                                     ,DateOfBirth=@DateOfBirth   
                                     ,JoinDate=@JoinDate                                                       
                                     ,IsActive=@IsActive 
                                     ,IsArchive=@IsArchive 
                                     ,CreatedBy=@CreatedBy 
                                     ,CreatedAt=@CreatedAt 
                                     ,CreatedFrom=@CreatedFrom 
                                     ,LastUpdateBy=@LastUpdateBy 
                                     ,LastUpdateFrom=@LastUpdateFrom 
                                     ,PhotoName=@PhotoName  
                                     ,Remarks=@Remarks 
                                     ,ResignReason=@ResignReason        
                                     ,ResignDate=@ResignDate                                 
                                     ,NomineeName=@NomineeName
                                     ,NomineeRelation=@NomineeRelation
                                     ,NomineeAddress=@NomineeAddress
                                     ,NomineeDistrict=@NomineeDistrict
                                     ,NomineeDivision=@NomineeDivision
                                     ,NomineeCountry=@NomineeCountry
                                     ,NomineeCity=@NomineeCity
                                     ,NomineePostalCode=@NomineePostalCode
                                     ,NomineePhone=@NomineePhone
                                     ,NomineeMobile=@NomineeMobile
                                     ,NomineeBirthCertificateNo=@NomineeBirthCertificateNo
                                     ,NomineeFax=@NomineeFax
                                     ,NomineeFileName=@NomineeFileName
                                     ,NomineeRemarks=@NomineeRemarks
                                     ,NomineeNID=@NomineeNID
                                     ,GrossSalary=@GrossSalary
                                     ,BasicSalary=@BasicSalary 
                                     ,Email=@Email
                                     ,ContactNo=@ContactNo
                                        ,OfficialContactNo = @OfficialContactNo,
                                        EmployeeNID = @EmployeeNID,
                                        EmployeeTIN = @EmployeeTIN,
                                        FathersName = @FathersName,
                                        MothersName = @MothersName,
                                        SpouseName = @SpouseName,
                                        EmployeeBankAccountNumber = @EmployeeBankAccountNumber,
                                        PresentAddress = @PresentAddress,
                                        ParmanentAdderss = @ParmanentAdderss,
                                        NomineeBankAccountNumber = @NomineeBankAccountNumber,
                                        NomineeShare = @NomineeShare,
                                        EmployeeBankNameId = @EmployeeBankNameId,
                                        NomineeBankNameId = @NomineeBankNameId
                                     where Id=@Id   
                                 ";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                        cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                        cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                        cmdInsert.Parameters.AddWithValue("@Department", vm.Department);
                        cmdInsert.Parameters.AddWithValue("@Designation", vm.Designation);
                        cmdInsert.Parameters.AddWithValue("@DateOfBirth", vm.DateOfBirth);
                        cmdInsert.Parameters.AddWithValue("@JoinDate", vm.JoinDate);

                        cmdInsert.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? "");
                        cmdInsert.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy ?? "");
                        cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? "");
                        cmdInsert.Parameters.AddWithValue("@PhotoName", vm.PhotoName ?? "");
                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? "");
                        cmdInsert.Parameters.AddWithValue("@ResignReason", vm.ResignReason ?? "");
                        cmdInsert.Parameters.AddWithValue("@ResignDate", vm.ResignDate ?? "");
                        if (vm.ResignReason != null)
                        {
                            cmdInsert.Parameters.AddWithValue("@IsActive", false);
                        }
                        else
                        {
                            cmdInsert.Parameters.AddWithValue("@IsActive", vm.IsActive);
                        }
                        cmdInsert.Parameters.AddWithValue("@NomineeName", vm.NomineeName ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeRelation", vm.NomineeRelation ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeAddress", vm.NomineeAddress ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeDistrict", vm.NomineeDistrict ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeDivision", vm.NomineeDivision ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeCountry", vm.NomineeCountry ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeCity", vm.NomineeCity ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineePostalCode", vm.NomineePostalCode ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineePhone", vm.NomineePhone ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeMobile", vm.NomineeMobile ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeBirthCertificateNo", vm.NomineeBirthCertificateNo ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeFax", vm.NomineeFax ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeFileName", vm.NomineeFileName ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeRemarks", vm.NomineeRemarks ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeNID", vm.NomineeNID ?? "");
                        cmdInsert.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                        cmdInsert.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                        cmdInsert.Parameters.AddWithValue("@ContactNo", vm.ContactNo ?? "");
                        cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? "");

                        cmdInsert.Parameters.AddWithValue("@OfficialContactNo", vm.OfficialContactNo ?? "");
                        cmdInsert.Parameters.AddWithValue("@EmployeeNID", vm.EmployeeNID ?? "");
                        cmdInsert.Parameters.AddWithValue("@EmployeeTIN", vm.EmployeeTIN ?? "");
                        cmdInsert.Parameters.AddWithValue("@FathersName", vm.FathersName ?? "");
                        cmdInsert.Parameters.AddWithValue("@MothersName", vm.MothersName ?? "");
                        cmdInsert.Parameters.AddWithValue("@SpouseName", vm.SpouseName ?? "");
                        cmdInsert.Parameters.AddWithValue("@EmployeeBankAccountNumber", vm.EmployeeBankAccountNumber ?? "");
                        cmdInsert.Parameters.AddWithValue("@PresentAddress", vm.PresentAddress ?? "");
                        cmdInsert.Parameters.AddWithValue("@ParmanentAdderss", vm.ParmanentAdderss ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeBankAccountNumber", vm.NomineeBankAccountNumber ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeShare", vm.NomineeShare ?? "");
                        cmdInsert.Parameters.AddWithValue("@EmployeeBankNameId", vm.EmployeeBankNameId);
                        cmdInsert.Parameters.AddWithValue("@NomineeBankNameId", vm.NomineeBankNameId);

                        cmdInsert.ExecuteNonQuery();
                    }
                    else
                    {

                        sqlText = "  ";
                        sqlText += @" INSERT INTO EmployeeInfo
                                (
                                      [Code]
                                    , [Name]
                                    , [Department]
                                    , [Designation]
                                    , [DateOfBirth]
                                    , [JoinDate]
                                    , [ResignDate]
                                    , [Remarks]
                                    , [IsActive]
                                    , [IsArchive]
                                    , [CreatedBy]
                                    , [CreatedAt]
                                    , [CreatedFrom]
                                    , [LastUpdateAt]
                                    , [LastUpdateFrom]
                                    , [PhotoName]
                                    , [NomineeDateofBirth]
                                    , [NomineeName]
                                    , [NomineeRelation]
                                    , [NomineeAddress]
                                    , [NomineeDistrict]
                                    , [NomineeDivision]
                                    , [NomineeCountry]
                                    , [NomineeCity]
                                    , [NomineePostalCode]
                                    , [NomineePhone]
                                    , [NomineeMobile]
                                    , [NomineeBirthCertificateNo]
                                    , [NomineeFax]
                                    , [NomineeFileName]
                                    , [NomineeRemarks]
                                    , [NomineeNID]
                                    , [GrossSalary]
                                    , [BasicSalary]
                                    , [Email]
                                    , [ContactNo]
                                    , [BranchId]

                                    , [OfficialContactNo]
                                    , [EmployeeNID]
                                    , [EmployeeTIN]
                                    , [FathersName]
                                    , [MothersName]
                                    , [SpouseName]
                                    , [EmployeeBankAccountNumber]
                                    , [PresentAddress]
                                    , [ParmanentAdderss]
                                    , [NomineeBankAccountNumber]
                                    , [NomineeShare]
                                    , [EmployeeBankNameId]
                                    , [NomineeBankNameId]
                                )
                                VALUES
                                (
                                      @Code
                                    , @Name
                                    , @Department
                                    , @Designation
                                    , @DateOfBirth
                                    , @JoinDate
                                    , @ResignDate
                                    , @Remarks
                                    , @IsActive
                                    , @IsArchive
                                    , @CreatedBy
                                    , @CreatedAt
                                    , @CreatedFrom
                                    , @LastUpdateAt
                                    , @LastUpdateFrom
                                    , @PhotoName
                                    , @NomineeDateofBirth
                                    , @NomineeName
                                    , @NomineeRelation
                                    , @NomineeAddress
                                    , @NomineeDistrict
                                    , @NomineeDivision
                                    , @NomineeCountry
                                    , @NomineeCity
                                    , @NomineePostalCode
                                    , @NomineePhone
                                    , @NomineeMobile
                                    , @NomineeBirthCertificateNo
                                    , @NomineeFax
                                    , @NomineeFileName
                                    , @NomineeRemarks
                                    , @NomineeNID
                                    , @GrossSalary
                                    , @BasicSalary
                                    , @Email
                                    , @ContactNo
                                    , @BranchId

                                    , @OfficialContactNo
                                    , @EmployeeNID
                                    , @EmployeeTIN
                                    , @FathersName
                                    , @MothersName
                                    , @SpouseName
                                    , @EmployeeBankAccountNumber
                                    , @PresentAddress
                                    , @ParmanentAdderss
                                    , @NomineeBankAccountNumber
                                    , @NomineeShare
                                    , @EmployeeBankNameId
                                    , @NomineeBankNameId
                                )";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                        cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                        cmdInsert.Parameters.AddWithValue("@Department", vm.Department ?? "1_18");
                        cmdInsert.Parameters.AddWithValue("@Designation", vm.Designation);
                        cmdInsert.Parameters.AddWithValue("@DateOfBirth", vm.DateOfBirth);
                        cmdInsert.Parameters.AddWithValue("@JoinDate", vm.JoinDate);
                        cmdInsert.Parameters.AddWithValue("@ResignDate", vm.ResignDate ?? "1990101");
                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? "");
                        cmdInsert.Parameters.AddWithValue("@IsActive", vm.IsActive);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                        cmdInsert.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt ?? "");
                        cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? "");
                        cmdInsert.Parameters.AddWithValue("@PhotoName", vm.PhotoName ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeDateofBirth", vm.NomineeDateofBirth ?? "1990101");
                        cmdInsert.Parameters.AddWithValue("@NomineeName", vm.NomineeName ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeRelation", vm.NomineeRelation ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeAddress", vm.NomineeAddress ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeDistrict", vm.NomineeDistrict ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeDivision", vm.NomineeDivision ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeCountry", vm.NomineeCountry ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeCity", vm.NomineeCity ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineePostalCode", vm.NomineePostalCode ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineePhone", vm.NomineePhone ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeMobile", vm.NomineeMobile ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeBirthCertificateNo", vm.NomineeBirthCertificateNo ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeFax", vm.NomineeFax ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeFileName", vm.NomineeFileName ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeRemarks", vm.NomineeRemarks ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeNID", vm.NomineeNID ?? "");
                        cmdInsert.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                        cmdInsert.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                        cmdInsert.Parameters.AddWithValue("@ContactNo", vm.ContactNo ?? "");
                        cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? "");
                        cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId ?? "");

                        cmdInsert.Parameters.AddWithValue("@OfficialContactNo", vm.OfficialContactNo ?? "");
                        cmdInsert.Parameters.AddWithValue("@EmployeeNID", vm.EmployeeNID ?? "");
                        cmdInsert.Parameters.AddWithValue("@EmployeeTIN", vm.EmployeeTIN ?? "");
                        cmdInsert.Parameters.AddWithValue("@FathersName", vm.FathersName ?? "");
                        cmdInsert.Parameters.AddWithValue("@MothersName", vm.MothersName ?? "");
                        cmdInsert.Parameters.AddWithValue("@SpouseName", vm.SpouseName ?? "");
                        cmdInsert.Parameters.AddWithValue("@EmployeeBankAccountNumber", vm.EmployeeBankAccountNumber ?? "");
                        cmdInsert.Parameters.AddWithValue("@PresentAddress", vm.PresentAddress ?? "");
                        cmdInsert.Parameters.AddWithValue("@ParmanentAdderss", vm.ParmanentAdderss ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeBankAccountNumber", vm.NomineeBankAccountNumber ?? "");
                        cmdInsert.Parameters.AddWithValue("@NomineeShare", vm.NomineeShare ?? "");
                        cmdInsert.Parameters.AddWithValue("@EmployeeBankNameId", vm.EmployeeBankNameId);
                        cmdInsert.Parameters.AddWithValue("@NomineeBankNameId", vm.NomineeBankNameId);

                        cmdInsert.ExecuteNonQuery();
                    }
                }
                else
                {
                    retResults[1] = "This EmployeeInfoForPF already used";
                    throw new ArgumentNullException("Please Input EmployeeInfoForPF Value", "");
                }


                #endregion User Create

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
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {
                        retResults[1] = "Unexpected error to update InsertEmployeeInfoForPF";
                        return retResults;
                        // throw new ArgumentNullException("Unexpected error to update EmployeeInfoForPF.", "EmployeeInfoForPF");
                    }
                }
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

        /// <summary>
        /// Retrieves a list of all employee information related to Provident Fund (WWF) from the database.
        /// </summary>
        /// <returns>
        /// A list of <see cref="EmployeeInfoForPFVM"/> objects containing employee details such as EmployeeId, Code, Name, Department, Designation, Project, Section, Date of Birth, and Join Date.
        /// </returns>
        public List<EmployeeInfoForPFVM> SelectAllEmployeeInfoForPF(string[] conditionFields, string[] conditionValues)
        {
            #region Variables
            SqlTransaction transaction = null;
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoForPFVM> employeeInfoForPFVMs = new List<EmployeeInfoForPFVM>();
            EmployeeInfoForPFVM EmployeeInfoForPFVM;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement
                sqlText = @"SELECT ve.EmployeeId, ve.Code, ve.EmpName, ve.DateOfBirth, ve.JoinDate, ve.ResignDate LeftDate, ve.Branch, ve.Grade, ISNULL(ve.GrossSalary, 0) AS GrossSalary, ISNULL(ve.BasicSalary, 0) AS BasicSalary, ve.PhotoName, 
                         ve.IsActive, ve.IsArchive, ve.LastUpdateAt, ve.LastUpdateBy, ve.LastUpdateFrom, ve.Other1, ve.Remarks, ve.Department, ve.Designation, ve.Section, ve.Project
                         FROM ViewEmployeeInformation AS ve where 1=1 and IsActive=1";

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
                sqlText += @" ORDER BY ve.EmpName";

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

                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    EmployeeInfoForPFVM = new EmployeeInfoForPFVM();
                    EmployeeInfoForPFVM.Id = Convert.ToInt32(dr["EmployeeId"]);
                    EmployeeInfoForPFVM.Code = dr["Code"].ToString();
                    EmployeeInfoForPFVM.Name = dr["EmpName"].ToString();
                    EmployeeInfoForPFVM.Department = dr["Department"].ToString();
                    EmployeeInfoForPFVM.Designation = dr["Designation"].ToString();
                    EmployeeInfoForPFVM.Project = dr["Project"].ToString();
                    EmployeeInfoForPFVM.Section = dr["Section"].ToString();
                    EmployeeInfoForPFVM.DateOfBirth = dr["DateOfBirth"].ToString();
                    EmployeeInfoForPFVM.JoinDate = dr["JoinDate"].ToString();
                    employeeInfoForPFVMs.Add(EmployeeInfoForPFVM);

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

            return employeeInfoForPFVMs;
        }

        public List<EmployeeInfoForPFVM> SelectRegnationEmployeeInfoForPF(string[] conditionFields, string[] conditionValues)
        {
            #region Variables
            SqlTransaction transaction = null;
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoForPFVM> employeeInfoForPFVMs = new List<EmployeeInfoForPFVM>();
            EmployeeInfoForPFVM EmployeeInfoForPFVM;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement
                sqlText = @"SELECT ve.EmployeeId, ve.Code, ve.EmpName, ve.DateOfBirth, ve.JoinDate, ve.ResignDate LeftDate, ve.Branch, ve.Grade, ISNULL(ve.GrossSalary, 0) AS GrossSalary, ISNULL(ve.BasicSalary, 0) AS BasicSalary, ve.PhotoName, 
                         ve.IsActive, ve.IsArchive, ve.LastUpdateAt, ve.LastUpdateBy, ve.LastUpdateFrom, ve.Other1, ve.Remarks, ve.Department, ve.Designation, ve.Section, ve.Project
                         FROM ViewEmployeeInformation AS ve where 1=1 and IsActive=0";

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
                sqlText += @" ORDER BY ve.EmpName";

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

                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    EmployeeInfoForPFVM = new EmployeeInfoForPFVM();
                    EmployeeInfoForPFVM.Id = Convert.ToInt32(dr["EmployeeId"]);
                    EmployeeInfoForPFVM.Code = dr["Code"].ToString();
                    EmployeeInfoForPFVM.Name = dr["EmpName"].ToString();
                    EmployeeInfoForPFVM.Department = dr["Department"].ToString();
                    EmployeeInfoForPFVM.Designation = dr["Designation"].ToString();
                    EmployeeInfoForPFVM.Project = dr["Project"].ToString();
                    EmployeeInfoForPFVM.Section = dr["Section"].ToString();
                    EmployeeInfoForPFVM.DateOfBirth = dr["DateOfBirth"].ToString();
                    EmployeeInfoForPFVM.JoinDate = dr["JoinDate"].ToString();
                    employeeInfoForPFVMs.Add(EmployeeInfoForPFVM);

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

            return employeeInfoForPFVMs;
        }

        /// <summary>
        /// Retrieves an EmployeeInfoForPFVM object from the database by the specified employee Id.
        /// </summary>
        /// <param name="Id">The unique identifier of the employee to retrieve.</param>
        /// <returns>
        /// An instance of EmployeeInfoForPFVM populated with data from the database if found; 
        /// otherwise, an instance with default values.
        /// </returns>
        public EmployeeInfoForPFVM SelectById(int Id)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeInfoForPFVM EmployeeInfoForPFVM = new EmployeeInfoForPFVM();
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

                sqlText = @"SELECT * FROM EmployeeInfo EF               
                WHERE 1=1";
                sqlText += @" and EF.Id=@Id";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.Parameters.AddWithValue("@Id", Id);
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    EmployeeInfoForPFVM = new EmployeeInfoForPFVM();
                    EmployeeInfoForPFVM.Id = Convert.ToInt32(dr["Id"]);
                    EmployeeInfoForPFVM.Code = dr["Code"].ToString();
                    EmployeeInfoForPFVM.Name = dr["Name"].ToString();
                    EmployeeInfoForPFVM.Department = dr["Department"].ToString();
                    EmployeeInfoForPFVM.Designation = dr["Designation"].ToString();
                    //EmployeeInfoForPFVM.Project = dr["Project"].ToString();
                    //EmployeeInfoForPFVM.Section = dr["Section"].ToString();
                    EmployeeInfoForPFVM.DateOfBirth = dr["DateOfBirth"].ToString();
                    EmployeeInfoForPFVM.JoinDate = dr["JoinDate"].ToString();
                    EmployeeInfoForPFVM.ResignDate = dr["ResignDate"].ToString();
                    EmployeeInfoForPFVM.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    EmployeeInfoForPFVM.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    EmployeeInfoForPFVM.NomineeName = dr["NomineeName"].ToString();
                    EmployeeInfoForPFVM.NomineeDateofBirth = dr["NomineeDateofBirth"].ToString();
                    EmployeeInfoForPFVM.NomineeRelation = dr["NomineeRelation"].ToString();
                    EmployeeInfoForPFVM.NomineeAddress = dr["NomineeAddress"].ToString();
                    EmployeeInfoForPFVM.NomineeCountry = dr["NomineeCountry"].ToString();
                    EmployeeInfoForPFVM.NomineeDivision = dr["NomineeDivision"].ToString();
                    EmployeeInfoForPFVM.NomineeDistrict = dr["NomineeDistrict"].ToString();
                    EmployeeInfoForPFVM.NomineeCity = dr["NomineeCity"].ToString();
                    EmployeeInfoForPFVM.NomineePostalCode = dr["NomineePostalCode"].ToString();
                    EmployeeInfoForPFVM.NomineePhone = dr["NomineePhone"].ToString();
                    EmployeeInfoForPFVM.NomineeMobile = dr["NomineeMobile"].ToString();
                    EmployeeInfoForPFVM.NomineeBirthCertificateNo = dr["NomineeBirthCertificateNo"].ToString();
                    EmployeeInfoForPFVM.NomineeFax = dr["NomineeFax"].ToString();
                    EmployeeInfoForPFVM.NomineeFileName = dr["NomineeFileName"].ToString();
                    EmployeeInfoForPFVM.NomineeNID = dr["NomineeNID"].ToString();
                    EmployeeInfoForPFVM.NomineeRemarks = dr["NomineeRemarks"].ToString();
                    EmployeeInfoForPFVM.Remarks = dr["Remarks"].ToString();
                    EmployeeInfoForPFVM.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    EmployeeInfoForPFVM.ContactNo = dr["ContactNo"].ToString();
                    EmployeeInfoForPFVM.Email = dr["Email"].ToString();
                    EmployeeInfoForPFVM.PhotoName = dr["PhotoName"].ToString();

                    EmployeeInfoForPFVM.OfficialContactNo = dr["OfficialContactNo"].ToString();
                    EmployeeInfoForPFVM.EmployeeNID = dr["EmployeeNID"].ToString();
                    EmployeeInfoForPFVM.EmployeeTIN = dr["EmployeeTIN"].ToString();
                    EmployeeInfoForPFVM.FathersName = dr["FathersName"].ToString();
                    EmployeeInfoForPFVM.MothersName = dr["MothersName"].ToString();
                    EmployeeInfoForPFVM.SpouseName = dr["SpouseName"].ToString();
                    EmployeeInfoForPFVM.EmployeeBankAccountNumber = dr["EmployeeBankAccountNumber"].ToString();
                    EmployeeInfoForPFVM.PresentAddress = dr["PresentAddress"].ToString();
                    EmployeeInfoForPFVM.ParmanentAdderss = dr["ParmanentAdderss"].ToString();
                    EmployeeInfoForPFVM.NomineeBankAccountNumber = dr["NomineeBankAccountNumber"].ToString();
                    EmployeeInfoForPFVM.NomineeShare = dr["NomineeShare"].ToString();
                    EmployeeInfoForPFVM.EmployeeBankNameId = Convert.ToInt32(dr["EmployeeBankNameId"] == DBNull.Value ? 0 : dr["EmployeeBankNameId"]);
                    EmployeeInfoForPFVM.NomineeBankNameId = Convert.ToInt32(dr["NomineeBankNameId"] == DBNull.Value ? 0 : dr["NomineeBankNameId"]);
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

            return EmployeeInfoForPFVM;
        }

        /// <summary>
        /// Marks an employee record as inactive and archived in the database by updating the EmployeeInfo table,
        /// using the provided employee Id. Supports optional external SQL connection and transaction handling.
        /// </summary>
        /// <param name="Id">The unique identifier of the employee to be updated.</param>
        /// <param name="VcurrConn">An optional existing SQL connection to use; if null, a new connection is created and managed 
        public string[] DeleteEmployeeInfoForPF(int Id, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {

            #region Initializ
            string sqlText = "";
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeInfoForPF"; //Method Name
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

                if (Id != null)
                {
                    sqlText = "  ";
                    sqlText += @"Update EmployeeInfo  set                        
                          IsActive =@IsActive,
                          IsArchive =@IsArchive
                          ";
                    sqlText += " where Id=@Id ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", Id);
                    cmdInsert.Parameters.AddWithValue("@IsActive", false);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", true);
                    cmdInsert.ExecuteNonQuery();
                }
                #endregion User Create

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
                retResults[1] = "DeleteEmployeeInfoForPF has been Deleted Successfully";
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {
                        retResults[1] = "Unexpected error to update DeleteEmployeeInfoForPF.";
                        return retResults;
                    }
                }
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
        public string[] ReActiveEmployeeInfoForPF(string Id, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {

            #region Initializ
            string sqlText = "";
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ReActiveEmployeeInfoForPF"; //Method Name
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

                if (Id != null)
                {
                    sqlText = "  ";
                    sqlText += @"Update EmployeeInfo  set                        
                          ResignReason =null,
                          ResignDate =null,
                          IsActive =1
                          ";
                    sqlText += " where Id=@Id ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", Id);
                    cmdInsert.ExecuteNonQuery();
                }
                #endregion User Create

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
                retResults[1] = "EmployeeInfo has been ReActive Successfully";
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {
                        retResults[1] = "Unexpected error to update ReActiveEmployeeInfoForPF.";
                        return retResults;
                    }
                }
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

        public string[] InsertExportData(EmployeeInfoForPFVM paramVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "ImportExcelFile"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                File.Delete(Fullpath);
                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    paramVM.File.SaveAs(Fullpath);
                }


                FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();


                dt = ds.Tables[0];
                reader.Close();

                File.Delete(Fullpath);
                #endregion

                #region open connection and transaction
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
                string Code = "";

                EmployeeInfoForPFVM vEmployeeInfoVM = new EmployeeInfoForPFVM();

                #region Assign Data
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    DataTable returnDt = new DataTable();
                    string Department = dr["Department"].ToString().Trim();
                    string Designation = dr["Designation"].ToString().Trim();
                    //string Project = dr["Project"].ToString().Trim();
                    //string Section = dr["Section"].ToString().Trim();


                    #region Finding DepartmentId Using Department
                    returnDt = _cDal.SelectByCondition("Department", "Name", Department, currConn, transaction);
                    if (returnDt != null && returnDt.Rows.Count > 0)
                    {
                        vEmployeeInfoVM.Department = returnDt.Rows[0]["Id"].ToString();
                    }
                    else
                    {
                        retResults[1] = "Department Not Found for " + Department;
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion Finding DepartmentId Using Department
                    #region Finding DesignationId Using Designation
                    returnDt = _cDal.SelectByCondition("Designation", "Name", Designation, currConn, transaction);
                    if (returnDt != null && returnDt.Rows.Count > 0)
                    {
                        vEmployeeInfoVM.Designation = returnDt.Rows[0]["Id"].ToString();
                    }
                    else
                    {
                        retResults[1] = "Designation Not Found for " + Designation;
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion Finding DesignationId Using Designation
                    //#region Finding ProjectId Using Project
                    //returnDt = _cDal.SelectByCondition("Project", "Name", Project, currConn, transaction);
                    //if (returnDt != null && returnDt.Rows.Count > 0)
                    //{
                    //    vEmployeeInfoVM.Project = returnDt.Rows[0]["Id"].ToString();
                    //}
                    //else
                    //{
                    //    retResults[1] = "Project Not Found for " + Project;
                    //    throw new ArgumentNullException(retResults[1], "");
                    //}
                    //#endregion Finding ProjectId Using Project
                    //#region Finding SectionId Using Section
                    //returnDt = _cDal.SelectByCondition("Section", "Name", Section, currConn, transaction);
                    //if (returnDt != null && returnDt.Rows.Count > 0)
                    //{
                    //    vEmployeeInfoVM.Section = returnDt.Rows[0]["Id"].ToString();
                    //}
                    //else
                    //{
                    //    retResults[1] = "Section Not Found for " + Section;
                    //    throw new ArgumentNullException(retResults[1], "");
                    //}
                    //#endregion Finding SectionId Using Section

                    vEmployeeInfoVM.Code = dr["Code"].ToString();
                    vEmployeeInfoVM.Name = dr["Name"].ToString();
                    vEmployeeInfoVM.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vEmployeeInfoVM.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vEmployeeInfoVM.Remarks = dr["Remarks"].ToString();
                    vEmployeeInfoVM.DateOfBirth = dr["DateOfBirth"].ToString();
                    vEmployeeInfoVM.JoinDate = dr["JoinDate"].ToString();
                    vEmployeeInfoVM.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    vEmployeeInfoVM.ContactNo = dr["ContactNo"].ToString();
                    vEmployeeInfoVM.Email = dr["Email"].ToString();

                    vEmployeeInfoVM.NomineeName = dr["NomineeName"].ToString();
                    vEmployeeInfoVM.NomineeDateofBirth = dr["NomineeDateofBirth"].ToString();
                    vEmployeeInfoVM.NomineeRelation = dr["NomineeRelation"].ToString();
                    vEmployeeInfoVM.NomineeAddress = dr["NomineeAddress"].ToString();
                    vEmployeeInfoVM.NomineeDistrict = dr["NomineeDistrict"].ToString();
                    vEmployeeInfoVM.NomineeDivision = dr["NomineeDivision"].ToString();
                    vEmployeeInfoVM.NomineeCountry = dr["NomineeCountry"].ToString();
                    vEmployeeInfoVM.NomineeCity = dr["NomineeCity"].ToString();
                    vEmployeeInfoVM.NomineePostalCode = dr["NomineePostalCode"].ToString();
                    vEmployeeInfoVM.NomineePhone = dr["NomineePhone"].ToString();
                    vEmployeeInfoVM.NomineeMobile = dr["NomineeMobile"].ToString();
                    vEmployeeInfoVM.NomineeBirthCertificateNo = dr["NomineeBirthCertificateNo"].ToString();
                    vEmployeeInfoVM.NomineeFax = dr["NomineeFax"].ToString();
                    vEmployeeInfoVM.NomineeFileName = dr["NomineeFileName"].ToString();
                    vEmployeeInfoVM.NomineeRemarks = dr["NomineeRemarks"].ToString();
                    vEmployeeInfoVM.NomineeNID = dr["NomineeNID"].ToString();

                    vEmployeeInfoVM.BranchId = paramVM.BranchId;

                    retResults = Insert(vEmployeeInfoVM, currConn, transaction);
                }
                #endregion

                #region Data Insert


                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }
                #endregion
                #endregion
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                transaction.Rollback();
                return retResults;
            }
            finally
            {
            }
            #endregion
            #region Results
            return retResults;
            #endregion

        }


        /// <summary>
        /// Retrieves employee data from an external API, deserializes it, and saves the data into the database within a transaction.
        /// </summary>
        /// <param name="VcurrConn">An existing SQL connection; if null, a new connection will be created.</param>
        /// <param name="Vtransaction">An existing SQL transaction; if null, a new transaction will be started.</param>
        /// <returns>
        /// An array of strings containing the operation result status, message, returned ID, executed SQL query (if any), exception message (if any), and method name.
        /// </returns>
        public string[] SelectFromAPIData(SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail"; // Success or Fail
            retResults[1] = "Something went wrong."; // Message
            retResults[2] = "0"; // Return ID
            retResults[3] = "";  // SQL Query if needed
            retResults[4] = "";  // Exception message
            retResults[5] = "SelectFromAPIData"; // Method name

            SqlConnection currConn = VcurrConn;
            SqlTransaction transaction = Vtransaction;

            try
            {
                #region Ensure Connection & Transaction
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
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                SettingDAL _settingDal = new SettingDAL();
                string URL = _settingDal.settingValue("WWF", "API_URL").Trim();
                string Response = _settingDal.settingValue("WWF", "Response").Trim();

                #region Call API and Deserialize
                List<EmployeeInfoForPFVM> categories = new List<EmployeeInfoForPFVM>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var response = client.GetAsync(Response).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        categories = JsonConvert.DeserializeObject<List<EmployeeInfoForPFVM>>(result);
                    }
                    else
                    {
                        retResults[1] = "API call failed: " + response.ReasonPhrase;
                        return retResults;
                    }
                }
                #endregion

                #region Loop and Save Data
                foreach (var item in categories)
                {
                    var emp = new EmployeeInfoForPFVM
                    {
                        Code = item.Code,
                        Name = item.Name,
                        Department = item.Department,
                        Designation = item.Designation,
                        Section = item.Section,
                        Project = item.Project,
                        BasicSalary = item.BasicSalary,
                        GrossSalary = item.GrossSalary,
                        Remarks = item.Remarks,
                        DateOfBirth = item.DateOfBirth,
                        JoinDate = item.JoinDate,
                        IsActive = item.IsActive,
                        ContactNo = item.ContactNo,
                        Email = item.Email
                    };

                    retResults = Insert(emp, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new Exception(retResults[1]);
                    }
                }
                #endregion

                #region Commit
                transaction.Commit();
                #endregion

                #region Success Response
                retResults[0] = "Success";
                retResults[1] = "Data saved successfully.";
                #endregion
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = "Exception occurred.";
                retResults[4] = ex.Message;

                transaction.Rollback();
            }

            return retResults;
        }

        public string[] Insert(EmployeeInfoForPFVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmplyeeInfoPF"; //Method Name
            EmployeeInfoForPFDAL _dal = new EmployeeInfoForPFDAL();

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

                #region Save

                #region SqlText
                string startdata = @"Select * from EmployeeInfo where Code=@Code ";
                SqlCommand cmdd = new SqlCommand(startdata, currConn, transaction);
                cmdd.Parameters.AddWithValue("@Code", vm.Code);
                SqlDataAdapter adapterd = new SqlDataAdapter(cmdd);
                DataTable dt = new DataTable();
                adapterd.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    sqlText += @" Update EmployeeInfo set
                                     Code=@Code
                                     ,Name=@Name
                                     ,Department=@Department
                                     ,Designation =@Designation                                 
                                     ,DateOfBirth=@DateOfBirth   
                                     ,JoinDate=@JoinDate                                                       
                                     ,IsActive=@IsActive 
                                     ,IsArchive=@IsArchive                                    
                                     ,LastUpdateBy=@LastUpdateBy 
                                     ,LastUpdateFrom=@LastUpdateFrom 
                                     ,PhotoName=@PhotoName  
                                     ,Remarks=@Remarks 
                                     ,ResignReason=@ResignReason        
                                     ,ResignDate=@ResignDate                                 
                                     ,NomineeName=@NomineeName
                                     ,NomineeRelation=@NomineeRelation
                                     ,NomineeAddress=@NomineeAddress
                                     ,NomineeDistrict=@NomineeDistrict
                                     ,NomineeDivision=@NomineeDivision
                                     ,NomineeCountry=@NomineeCountry
                                     ,NomineeCity=@NomineeCity
                                     ,NomineePostalCode=@NomineePostalCode
                                     ,NomineePhone=@NomineePhone
                                     ,NomineeMobile=@NomineeMobile
                                     ,NomineeBirthCertificateNo=@NomineeBirthCertificateNo
                                     ,NomineeFax=@NomineeFax
                                     ,NomineeFileName=@NomineeFileName
                                     ,NomineeRemarks=@NomineeRemarks
                                     ,NomineeNID=@NomineeNID
                                     ,GrossSalary=@GrossSalary
                                     ,BasicSalary=@BasicSalary 
                                     ,Email=@Email
                                     ,ContactNo=@ContactNo
                                     where Code=@Code   
                                 ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@Department", vm.Department);
                    cmdInsert.Parameters.AddWithValue("@Designation", vm.Designation);                  
                    cmdInsert.Parameters.AddWithValue("@DateOfBirth", vm.DateOfBirth);
                    cmdInsert.Parameters.AddWithValue("@JoinDate", vm.JoinDate);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmdInsert.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy ?? "");
                    cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? "");
                    cmdInsert.Parameters.AddWithValue("@PhotoName", vm.PhotoName ?? "");
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? "");
                    cmdInsert.Parameters.AddWithValue("@ResignReason", vm.ResignReason ?? "");
                    cmdInsert.Parameters.AddWithValue("@ResignDate", vm.ResignDate ?? "");
                    if (vm.ResignReason != null)
                    {
                        cmdInsert.Parameters.AddWithValue("@IsActive", false);
                    }
                    else
                    {
                        cmdInsert.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    }
                    cmdInsert.Parameters.AddWithValue("@NomineeName", vm.NomineeName ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeRelation", vm.NomineeRelation ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeAddress", vm.NomineeAddress ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeDistrict", vm.NomineeDistrict ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeDivision", vm.NomineeDivision ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeCountry", vm.NomineeCountry ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeCity", vm.NomineeCity ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineePostalCode", vm.NomineePostalCode ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineePhone", vm.NomineePhone ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeMobile", vm.NomineeMobile ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeBirthCertificateNo", vm.NomineeBirthCertificateNo ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeFax", vm.NomineeFax ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeFileName", vm.NomineeFileName ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeRemarks", vm.NomineeRemarks ?? "");
                    cmdInsert.Parameters.AddWithValue("@NomineeNID", vm.NomineeNID ?? "");
                    cmdInsert.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                    cmdInsert.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                    cmdInsert.Parameters.AddWithValue("@ContactNo", vm.ContactNo ?? "");
                    cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? "");
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeInfo
                                (
                                 Code                                
                                ,Name                                
                                ,Department
                                ,Designation
                                ,Project
                                ,Section 
                                ,BasicSalary
                                ,GrossSalary  
                                ,DateOfBirth   
                                ,JoinDate
                                ,IsActive
                                ,ContactNo
                                ,Email                     
                                ,Remarks                               
                                ,IsArchive
                                ,CreatedBy
                                ,CreatedAt
                                ,CreatedFrom
                                ,LastUpdateBy
                                ,LastUpdateAt
                                ,LastUpdateFrom
                                ,BranchId
                                ) VALUES (
                                 @Code
                                ,@Name                                
                                ,@Department
                                ,@Designation
                                ,@Project
                                ,@Section 
                                ,@BasicSalary
                                ,@GrossSalary 
                                ,@DateOfBirth   
                                ,@JoinDate
                                ,@IsActive
                                ,@ContactNo
                                ,@Email   
                                ,@Remarks                             
                                ,@IsArchive
                                ,@CreatedBy
                                ,@CreatedAt
                                ,@CreatedFrom
                                ,@LastUpdateBy
                                ,@LastUpdateAt
                                ,@LastUpdateFrom
                                ,@BranchId
                                 ) ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@Department", vm.Department);
                    cmdInsert.Parameters.AddWithValue("@Designation", vm.Designation);
                    cmdInsert.Parameters.AddWithValue("@Project", vm.Project);
                    cmdInsert.Parameters.AddWithValue("@Section", vm.Section);
                    cmdInsert.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                    cmdInsert.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                    cmdInsert.Parameters.AddWithValue("@DateOfBirth", vm.DateOfBirth);
                    cmdInsert.Parameters.AddWithValue("@JoinDate", vm.JoinDate);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@ContactNo", vm.ContactNo);
                    cmdInsert.Parameters.AddWithValue("@Email", vm.Email);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", "");
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", "");
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", "");
                    cmdInsert.Parameters.AddWithValue("@LastUpdateBy", "");
                    cmdInsert.Parameters.AddWithValue("@LastUpdateAt", "");
                    cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", "");
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    cmdInsert.ExecuteNonQuery();
                }
                #endregion

                #region SqlExecution

                #endregion


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


        public string[] UpdatePhoto(int EmployeeId, string PhotoName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeInfo Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeInfo"); }
                #endregion open connection and transaction
                if (!string.IsNullOrWhiteSpace(PhotoName))
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeInfo set";
                    sqlText += " PhotoName=@PhotoName";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@PhotoName", PhotoName);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeeId);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = EmployeeId.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EmployeeInfoVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeInfo Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update EmployeeInfo.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {
                        retResults[1] = "Unexpected error to update EmployeeInfo.";
                        return retResults;
                        // throw new ArgumentNullException("Unexpected error to update EmployeeInfo.", "EmployeeInfo");
                    }
                }
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
