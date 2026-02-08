using Excel;
using SymOrdinary;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Payroll
{
    public class GradeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<GradeVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<GradeVM> gradeVMs = new List<GradeVM>();
            GradeVM gradeVM;
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
,Code
,Name
,isnull(MinSalary  ,0)MinSalary
,isnull(MaxSalary,0)MaxSalary
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Grade
Where IsArchive=0
    ORDER BY Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    gradeVM = new GradeVM();
                    gradeVM.Id = dr["Id"].ToString();
                    gradeVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    gradeVM.Code = dr["Code"].ToString();
                    gradeVM.Name = dr["Name"].ToString();
                    gradeVM.MinSalary = Convert.ToDecimal( dr["MinSalary"]);
                    gradeVM.MaxSalary = Convert.ToDecimal( dr["MaxSalary"]);
                    gradeVM.Remarks = dr["Remarks"].ToString();
                    gradeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    gradeVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    gradeVM.CreatedBy = dr["CreatedBy"].ToString();
                    gradeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    gradeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    gradeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    gradeVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    gradeVMs.Add(gradeVM);
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

            return gradeVMs;
        }
        //==================SelectByID=================
        public GradeVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            GradeVM gradeVM = new GradeVM();

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
,Code
,Name
,isnull(Area ,'')Area
,isnull(GradeNo ,0)GradeNo
,isnull(CurrentBasic ,0)CurrentBasic
,isnull(BasicNextYearFactor ,0)BasicNextYearFactor
,isnull(BasicNextStepFactor ,0)BasicNextStepFactor
,isnull(IsHouseRentFactorFromBasic ,0)IsHouseRentFactorFromBasic
,isnull(HouseRentFactor ,0)HouseRentFactor
,isnull(IsTAFactorFromBasic ,0)IsTAFactorFromBasic
,isnull(TAFactor ,0)TAFactor
,isnull(IsMedicalFactorFromBasic ,0)IsMedicalFactorFromBasic
,isnull(MedicalFactor ,0)MedicalFactor
,isnull(MinSalary  ,0)MinSalary
,isnull(MaxSalary,0)MaxSalary
,isnull(IsFixedHouseRent  ,0)IsFixedHouseRent
,isnull(HouseRentAllowance,0)HouseRentAllowance
,isnull(IsFixedSpecialAllowance  ,0)IsFixedSpecialAllowance
,isnull(SpecialAllowance,0)SpecialAllowance

,isnull(LowerLimit,0)LowerLimit
,isnull(MedianLimit,0)MedianLimit
,isnull(UpperLimit,0)UpperLimit

,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Grade
Where  id=@Id and IsArchive=0
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
                    gradeVM.Id = dr["Id"].ToString();
                    gradeVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    gradeVM.Code = dr["Code"].ToString();
                    gradeVM.Name = dr["Name"].ToString();
                    gradeVM.Area = dr["Area"].ToString();
                    gradeVM.GradeNo = Convert.ToInt32(dr["GradeNo"]);
                    gradeVM.CurrentBasic = Convert.ToDecimal(dr["CurrentBasic"]);
                    gradeVM.BasicNextYearFactor = Convert.ToDecimal(dr["BasicNextYearFactor"]);
                    gradeVM.BasicNextStepFactor = Convert.ToDecimal(dr["BasicNextStepFactor"]);
                    gradeVM.IsHouseRentFactorFromBasic = Convert.ToBoolean(dr["IsHouseRentFactorFromBasic"]);
                    gradeVM.HouseRentFactor = Convert.ToDecimal(dr["HouseRentFactor"]);
                    gradeVM.IsTAFactorFromBasic = Convert.ToBoolean(dr["IsTAFactorFromBasic"]);
                    gradeVM.TAFactor = Convert.ToDecimal(dr["TAFactor"]);
                    gradeVM.IsMedicalFactorFromBasic = Convert.ToBoolean(dr["IsMedicalFactorFromBasic"]);
                    gradeVM.MedicalFactor = Convert.ToDecimal(dr["MedicalFactor"]);
                    gradeVM.MinSalary = Convert.ToDecimal(dr["MinSalary"]);
                    gradeVM.MaxSalary = Convert.ToDecimal(dr["MaxSalary"]);
                    gradeVM.IsFixedHouseRent = Convert.ToBoolean(dr["IsFixedHouseRent"]);
                    gradeVM.HouseRentAllowance = Convert.ToDecimal(dr["HouseRentAllowance"]);
                    gradeVM.IsFixedSpecialAllowance = Convert.ToBoolean(dr["IsFixedSpecialAllowance"]);
                    gradeVM.SpecialAllowance = Convert.ToDecimal(dr["SpecialAllowance"]);

                    gradeVM.LowerLimit = Convert.ToDecimal(dr["LowerLimit"]);
                    gradeVM.MedianLimit = Convert.ToDecimal(dr["MedianLimit"]);
                    gradeVM.UpperLimit = Convert.ToDecimal(dr["UpperLimit"]);


                    gradeVM.Remarks = dr["Remarks"].ToString();
                    gradeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    gradeVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    gradeVM.CreatedBy = dr["CreatedBy"].ToString();
                    gradeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    gradeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    gradeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    gradeVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return gradeVM;
        }
        //==================Insert =================
        public string[] Insert(GradeVM gradeVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertGrade"; //Method Name


            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(GradeVM.GradeId))
                //{
                //    retResults[1] = "Please Input Employee Travel Course";
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

                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM Grade ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", GradeVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", GradeVM.Name.Trim());
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Travel Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Travel Value", "");
                //}
                //#endregion Exist
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Code FROM Grade ";
                sqlText += " WHERE Code=@Code and BranchId=@BranchId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Code", gradeVM.Code.Trim());
                cmdExist.Parameters.AddWithValue("@BranchId", gradeVM.BranchId);
				var exeRes = cmdExist.ExecuteScalar();
				int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    retResults[1] = "Code already used!";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Code already used!", "");
                }
                #endregion Exist

                #region Save
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from Grade where BranchId=@BranchId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", gradeVM.BranchId);
                cmd2.Transaction = transaction;
				exeRes = cmd2.ExecuteScalar();
				int count = Convert.ToInt32(exeRes);

                gradeVM.Id = gradeVM.BranchId.ToString() + "_" + (count + 1);
                //int foundId = (int)objfoundId;
                if (true)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO Grade(Id,BranchId,Code,Name,Area,GradeNo,CurrentBasic,BasicNextYearFactor,BasicNextStepFactor,IsHouseRentFactorFromBasic,HouseRentFactor,
                                 IsTAFactorFromBasic,TAFactor,IsMedicalFactorFromBasic,MedicalFactor,MinSalary ,MaxSalary,IsFixedHouseRent,HouseRentAllowance,IsFixedSpecialAllowance,SpecialAllowance,LowerLimit,MedianLimit,UpperLimit,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@BranchId,@Code,@Name,@Area,@GradeNo,@CurrentBasic,@BasicNextYearFactor,@BasicNextStepFactor,@IsHouseRentFactorFromBasic,@HouseRentFactor,
                              @IsTAFactorFromBasic,@TAFactor,@IsMedicalFactorFromBasic,@MedicalFactor,@MinSalary ,@MaxSalary,@IsFixedHouseRent,@HouseRentAllowance,@IsFixedSpecialAllowance,@SpecialAllowance,@LowerLimit,@MedianLimit,@UpperLimit,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        ";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);

                    cmdExist1.Parameters.AddWithValue("@Id", gradeVM.Id);
                    cmdExist1.Parameters.AddWithValue("@BranchId", gradeVM.BranchId);
                    cmdExist1.Parameters.AddWithValue("@Code", gradeVM.Code.Trim());
                    cmdExist1.Parameters.AddWithValue("@MinSalary", gradeVM.MinSalary);
                    cmdExist1.Parameters.AddWithValue("@MaxSalary", gradeVM.MaxSalary);
                    cmdExist1.Parameters.AddWithValue("@Name", gradeVM.Name.Trim());
                    cmdExist1.Parameters.AddWithValue("@Area", gradeVM.Area.Trim());
                    cmdExist1.Parameters.AddWithValue("@GradeNo",gradeVM.GradeNo);
                    cmdExist1.Parameters.AddWithValue("@CurrentBasic",gradeVM.CurrentBasic);
                    cmdExist1.Parameters.AddWithValue("@BasicNextYearFactor", gradeVM.BasicNextYearFactor);
                    cmdExist1.Parameters.AddWithValue("@BasicNextStepFactor", gradeVM.BasicNextStepFactor);
                    cmdExist1.Parameters.AddWithValue("@IsHouseRentFactorFromBasic", gradeVM.IsHouseRentFactorFromBasic);
                    cmdExist1.Parameters.AddWithValue("@HouseRentFactor", gradeVM.HouseRentFactor);
                    cmdExist1.Parameters.AddWithValue("@IsTAFactorFromBasic", gradeVM.IsTAFactorFromBasic);
                    cmdExist1.Parameters.AddWithValue("@TAFactor", gradeVM.TAFactor);
                    cmdExist1.Parameters.AddWithValue("@IsMedicalFactorFromBasic", gradeVM.IsMedicalFactorFromBasic);
                    cmdExist1.Parameters.AddWithValue("@MedicalFactor", gradeVM.MedicalFactor);

                    cmdExist1.Parameters.AddWithValue("@IsFixedHouseRent", gradeVM.IsFixedHouseRent);
                    cmdExist1.Parameters.AddWithValue("@HouseRentAllowance", gradeVM.HouseRentAllowance);
                    cmdExist1.Parameters.AddWithValue("@IsFixedSpecialAllowance", gradeVM.IsFixedSpecialAllowance);
                    cmdExist1.Parameters.AddWithValue("@SpecialAllowance", gradeVM.SpecialAllowance);

                    cmdExist1.Parameters.AddWithValue("@LowerLimit", gradeVM.LowerLimit);
                    cmdExist1.Parameters.AddWithValue("@MedianLimit", gradeVM.MedianLimit);
                    cmdExist1.Parameters.AddWithValue("@UpperLimit", gradeVM.UpperLimit);


                    cmdExist1.Parameters.AddWithValue("@Remarks", gradeVM.Remarks ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@IsActive", true);
                    cmdExist1.Parameters.AddWithValue("@IsArchive", false);
                    cmdExist1.Parameters.AddWithValue("@CreatedBy", gradeVM.CreatedBy);
                    cmdExist1.Parameters.AddWithValue("@CreatedAt", gradeVM.CreatedAt);
                    cmdExist1.Parameters.AddWithValue("@CreatedFrom", gradeVM.CreatedFrom);

                    cmdExist1.Transaction = transaction;
                    cmdExist1.ExecuteNonQuery();

           
                }
                else
                {
                    retResults[1] = "This Grade already used";
                    throw new ArgumentNullException("Please Input Grade Value", "");
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
                retResults[2] = gradeVM.Id;

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
        //==================Update =================
        public string[] Update(GradeVM gradeVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Grade Update"; //Method Name

            int transResult = 0;

            string sqlText = "";

            bool iSTransSuccess = false;

            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToGrade"); }

                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Code FROM Grade ";
                sqlText += " WHERE Code=@Code AND Id<>@Id and BranchId=@BranchId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", gradeVM.Id);
                cmdExist.Parameters.AddWithValue("@Code", gradeVM.Code.Trim());
                cmdExist.Parameters.AddWithValue("@BranchId", gradeVM.BranchId);
				var exeRes = cmdExist.ExecuteScalar();
				int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    retResults[1] = "Code already used!";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Code already used!", "");
                }

                #endregion Exist

                if (gradeVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update Grade set";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";
                    sqlText += " Area=@Area,";
                    sqlText += " GradeNo=@GradeNo,";
                    sqlText += " CurrentBasic=@CurrentBasic,";
                    sqlText += " BasicNextYearFactor=@BasicNextYearFactor,";
                    sqlText += " BasicNextStepFactor=@BasicNextStepFactor,";
                    sqlText += " IsHouseRentFactorFromBasic=@IsHouseRentFactorFromBasic,";
                    sqlText += " HouseRentFactor=@HouseRentFactor,";
                    sqlText += " IsTAFactorFromBasic=@IsTAFactorFromBasic,";
                    sqlText += " TAFactor=@TAFactor,";
                    sqlText += " IsMedicalFactorFromBasic=@IsMedicalFactorFromBasic,";
                    sqlText += " MedicalFactor=@MedicalFactor,";
                    sqlText += " MinSalary=@MinSalary,";
                    sqlText += " MaxSalary=@MaxSalary,";

                    sqlText += " LowerLimit=@LowerLimit,";
                    sqlText += " MedianLimit=@MedianLimit,";
                    sqlText += " UpperLimit=@UpperLimit,";
                    sqlText += " IsFixedHouseRent=@IsFixedHouseRent,";
                    sqlText += " HouseRentAllowance=@HouseRentAllowance,";
                    sqlText += " IsFixedSpecialAllowance=@IsFixedSpecialAllowance,";
                    sqlText += " SpecialAllowance=@SpecialAllowance,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", gradeVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", gradeVM.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Code", gradeVM.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", gradeVM.Name.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Area", gradeVM.Area.Trim());
                    cmdUpdate.Parameters.AddWithValue("@GradeNo", gradeVM.GradeNo);
                    cmdUpdate.Parameters.AddWithValue("@CurrentBasic", gradeVM.CurrentBasic);
                    cmdUpdate.Parameters.AddWithValue("@BasicNextYearFactor", gradeVM.BasicNextYearFactor);
                    cmdUpdate.Parameters.AddWithValue("@BasicNextStepFactor", gradeVM.BasicNextStepFactor);
                    cmdUpdate.Parameters.AddWithValue("@IsHouseRentFactorFromBasic", gradeVM.IsHouseRentFactorFromBasic);
                    cmdUpdate.Parameters.AddWithValue("@HouseRentFactor", gradeVM.HouseRentFactor);
                    cmdUpdate.Parameters.AddWithValue("@IsTAFactorFromBasic", gradeVM.IsTAFactorFromBasic);
                    cmdUpdate.Parameters.AddWithValue("@TAFactor", gradeVM.TAFactor);
                    cmdUpdate.Parameters.AddWithValue("@IsMedicalFactorFromBasic", gradeVM.IsMedicalFactorFromBasic);
                    cmdUpdate.Parameters.AddWithValue("@MedicalFactor", gradeVM.MedicalFactor);
                    cmdUpdate.Parameters.AddWithValue("@MinSalary", gradeVM.MinSalary);
                    cmdUpdate.Parameters.AddWithValue("@MaxSalary", gradeVM.MaxSalary);

                    cmdUpdate.Parameters.AddWithValue("@LowerLimit", gradeVM.LowerLimit);
                    cmdUpdate.Parameters.AddWithValue("@MedianLimit", gradeVM.MedianLimit);
                    cmdUpdate.Parameters.AddWithValue("@UpperLimit", gradeVM.UpperLimit);
                    cmdUpdate.Parameters.AddWithValue("@IsFixedHouseRent", gradeVM.IsFixedHouseRent);
                    cmdUpdate.Parameters.AddWithValue("@HouseRentAllowance", gradeVM.HouseRentAllowance);
                    cmdUpdate.Parameters.AddWithValue("@IsFixedSpecialAllowance", gradeVM.IsFixedSpecialAllowance);
                    cmdUpdate.Parameters.AddWithValue("@SpecialAllowance", gradeVM.SpecialAllowance); 

                    cmdUpdate.Parameters.AddWithValue("@Remarks", gradeVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", gradeVM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", gradeVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", gradeVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", gradeVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = gradeVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", GradeVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Grade Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Grade.";
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

        //==================Delete =================
        public string[] Delete(GradeVM gradeVM, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGrade"; //Method Name

            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            bool iSTransSuccess = false;

            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToGrade"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length>1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update Grade set";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", gradeVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", gradeVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", gradeVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }
                  



                    retResults[2] ="";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Grade Delete", gradeVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Grade Information Delete", "Could not found any item.");
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
                    retResults[1] = "Data Delete Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to delete Grade Information.";
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
        public List<GradeVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<GradeVM> VMs = new List<GradeVM>();
            GradeVM vm;
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
Name,
Code

   FROM Grade
WHERE IsArchive=0 and IsActive=1
    ORDER BY SL
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GradeVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Code"].ToString();
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
                sqlText = "";
                sqlText = @"SELECT Id, Name    FROM Grade ";
                sqlText += @" WHERE Name like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY SL";



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

        #endregion
       
    }
}
