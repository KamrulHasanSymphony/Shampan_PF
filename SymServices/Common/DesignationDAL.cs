using SymOrdinary;
using SymServices.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Common
{
   public class DesignationDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll Designation=================
        public List<DesignationVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<DesignationVM> DesignationVMs = new List<DesignationVM>();
            DesignationVM designationVM;
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
, Name
, Code
, BranchId
, ISNULL(EPZ, 0 )EPZ
, ISNULL(Other, 0 )Other
, ISNULL(DinnerAmount, 0 )DinnerAmount
, ISNULL(IfterAmount, 0 )IfterAmount
, ISNULL(TiffinAmount, 0 )TiffinAmount
, ISNULL(ETiffinAmount, 0 )ETiffinAmount
, OTAlloawance
, OTOrginal
, OTBayer
, ExtraOT
, ISNULL(AttendenceBonus, 0 )AttendenceBonus
, ISNULL(PriorityLevel, 0 )PriorityLevel

, Remarks
, IsActive
, IsArchive
, CreatedBy
, CreatedAt
, CreatedFrom
, LastUpdateBy
, LastUpdateAt
, LastUpdateFrom
, ISNULL(OrderNo, 0 )OrderNo


FROM Designation 
Where IsArchive=0
ORDER BY Name
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    designationVM = new DesignationVM();
                    designationVM.Id = dr["Id"].ToString();
                    designationVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    designationVM.Name = dr["Name"].ToString();
                    designationVM.Code = dr["Code"].ToString();
                    designationVM.EPZ = Convert.ToDecimal(dr["EPZ"].ToString());
                    designationVM.Other = Convert.ToDecimal(dr["Other"].ToString());
                    designationVM.DinnerAmount = Convert.ToDecimal(dr["DinnerAmount"].ToString());
                    designationVM.IfterAmount = Convert.ToDecimal(dr["IfterAmount"].ToString());
                    designationVM.TiffinAmount = Convert.ToDecimal(dr["TiffinAmount"].ToString());
                    designationVM.ETiffinAmount = Convert.ToDecimal(dr["ETiffinAmount"].ToString());
                    designationVM.OTAlloawance = Convert.ToBoolean(dr["OTAlloawance"].ToString());
                    designationVM.OTOrginal = Convert.ToBoolean(dr["OTOrginal"].ToString());
                    designationVM.OTBayer = Convert.ToBoolean(dr["OTBayer"].ToString());
                    designationVM.ExtraOT = Convert.ToBoolean(dr["ExtraOT"].ToString());
                    designationVM.AttendenceBonus = Convert.ToDecimal(dr["AttendenceBonus"].ToString());
                    designationVM.PriorityLevel = Convert.ToInt32(dr["PriorityLevel"].ToString());
                    designationVM.OrderNo = Convert.ToInt32(dr["OrderNo"].ToString());

                    designationVM.Remarks = dr["Remarks"].ToString();
                    //designationVM.Id = dr["GradeId"].ToString();
                    designationVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    designationVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    designationVM.CreatedBy = dr["CreatedBy"].ToString();
                    designationVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    designationVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    designationVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    designationVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    DesignationVMs.Add(designationVM);
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

            return DesignationVMs;
        }


        public DesignationVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DesignationVM designationVM = new DesignationVM(); ;
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
, Name
, Code
, BranchId
, ISNULL(EPZ, 0 )EPZ
, ISNULL(Other, 0 )Other
, ISNULL(DinnerAmount, 0 )DinnerAmount
, ISNULL(IfterAmount, 0 )IfterAmount
, ISNULL(TiffinAmount, 0 )TiffinAmount
, ISNULL(ETiffinAmount, 0 )ETiffinAmount
, ISNULL(AttendenceBonus,0)AttendenceBonus
, ISNULL(PriorityLevel,0)PriorityLevel
, ISNULL(OrderNo,0)OrderNo
, OTAlloawance
, OTOrginal
, OTBayer
, ExtraOT
, Remarks
, Remarks
, IsActive
, IsArchive
, CreatedBy
, CreatedAt
, CreatedFrom
, LastUpdateBy
, LastUpdateAt
, LastUpdateFrom
,DesignationGroupId
,GradeId
,HospitalPlanC1
,HospitalPlanC2
,HospitalPlanC3
,HospitalPlanC4
,HospitalPlanC5
,DeathCoveragePlanC6
,MaternityPlanC7
,MaternityPlanC8
,MaternityPlanC9
,EntitlementC1
,EntitlementC2
,EntitlementC3
,EntitlementC4
,EntitlementC5
,MobileExpenseC1
,MobileExpenseC2
,MobileExpenseC3
,MobileExpenseC4
,InternationalTravelC1
,InternationalTravelC2
,InternationalTravelC3
,DomesticlTravelC1
,DomesticTravelC2
,DomesticTravelC3
,DomesticTravelC4
,DomesticTravelC5

FROM Designation
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
                   // designation = new DesignationVM();
                    designationVM.Id = dr["Id"].ToString();
                    designationVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    designationVM.Name = dr["Name"].ToString();
                    designationVM.Code = dr["Code"].ToString();
                    designationVM.EPZ = Convert.ToDecimal(dr["EPZ"].ToString());
                    designationVM.Other = Convert.ToDecimal(dr["Other"].ToString());
                    designationVM.DinnerAmount = Convert.ToDecimal(dr["DinnerAmount"].ToString());
                    designationVM.IfterAmount = Convert.ToDecimal(dr["IfterAmount"].ToString());
                    designationVM.TiffinAmount = Convert.ToDecimal(dr["TiffinAmount"].ToString());
                    designationVM.ETiffinAmount = Convert.ToDecimal(dr["ETiffinAmount"].ToString());
                    designationVM.OTAlloawance = Convert.ToBoolean(dr["OTAlloawance"].ToString());
                    designationVM.OTOrginal = Convert.ToBoolean(dr["OTOrginal"].ToString());
                    designationVM.OTBayer = Convert.ToBoolean(dr["OTBayer"].ToString());
                    designationVM.ExtraOT = Convert.ToBoolean(dr["ExtraOT"].ToString());
                    designationVM.AttendenceBonus = Convert.ToDecimal(dr["AttendenceBonus"].ToString());
                    designationVM.PriorityLevel = Convert.ToInt32(dr["PriorityLevel"].ToString());
                    designationVM.Remarks = dr["Remarks"].ToString();
                    designationVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    designationVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    designationVM.CreatedBy = dr["CreatedBy"].ToString();
                    designationVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    designationVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    designationVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    designationVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    designationVM.DesignationGroupId = dr["DesignationGroupId"].ToString();
                    designationVM.GradeId = dr["GradeId"].ToString();
                    designationVM.OrderNo = Convert.ToInt32(dr["OrderNo"].ToString());

                    designationVM.HospitalPlanC1 = dr["HospitalPlanC1"].ToString();
                    designationVM.HospitalPlanC2 = dr["HospitalPlanC2"].ToString();
                    designationVM.HospitalPlanC3 = dr["HospitalPlanC3"].ToString();
                    designationVM.HospitalPlanC4 = dr["HospitalPlanC4"].ToString();
                    designationVM.HospitalPlanC5 = dr["HospitalPlanC5"].ToString();
                    designationVM.DeathCoveragePlanC6 = dr["DeathCoveragePlanC6"].ToString();
                    designationVM.MaternityPlanC7 = dr["MaternityPlanC7"].ToString();
                    designationVM.MaternityPlanC8 = dr["MaternityPlanC8"].ToString();
                    designationVM.MaternityPlanC9 = dr["MaternityPlanC9"].ToString();

                    designationVM.EntitlementC1 = dr["EntitlementC1"].ToString();
                    designationVM.EntitlementC2 = dr["EntitlementC2"].ToString();
                    designationVM.EntitlementC3 = dr["EntitlementC3"].ToString();
                    designationVM.EntitlementC4 = dr["EntitlementC4"].ToString();
                    designationVM.EntitlementC5 = dr["EntitlementC5"].ToString();

                    designationVM.MobileExpenseC1 = dr["MobileExpenseC1"].ToString();
                    designationVM.MobileExpenseC2 = dr["MobileExpenseC2"].ToString();
                    designationVM.MobileExpenseC3 = dr["MobileExpenseC3"].ToString();
                    designationVM.MobileExpenseC4 = dr["MobileExpenseC4"].ToString();

                    designationVM.InternationalTravelC1 = dr["InternationalTravelC1"].ToString();
                    designationVM.InternationalTravelC2 = dr["InternationalTravelC2"].ToString();
                    designationVM.InternationalTravelC3 = dr["InternationalTravelC3"].ToString();

                    designationVM.DomesticlTravelC1 = dr["DomesticlTravelC1"].ToString();
                    designationVM.DomesticTravelC2 = dr["DomesticTravelC2"].ToString();
                    designationVM.DomesticTravelC3 = dr["DomesticTravelC3"].ToString();
                    designationVM.DomesticTravelC4 = dr["DomesticTravelC4"].ToString();
                    designationVM.DomesticTravelC5 = dr["DomesticTravelC5"].ToString();

                    //designationVM.HospitalPlanC1 = dr["HospitalPlanC1"].ToString();
                    //designationVM.HospitalPlanC2 = dr["HospitalPlanC2"].ToString();
                    //designationVM.HospitalPlanC3 = dr["HospitalPlanC3"].ToString();
                    //designationVM.IntensiveCareUniteC4 =dr["IntensiveCareUniteC4"].ToString();
                    //designationVM.HospitalServiceC5 = dr["HospitalServiceC5"].ToString();
                    //designationVM.DeathCoverageC6 = dr["DeathCoverageC6"].ToString();
                    //designationVM.PatientMaternityPlanC7 = dr["PatientMaternityPlanC7"].ToString();

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

            return designationVM;
        }

        public DesignationVM SelectByEmployeeId(string EmployeeId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DesignationVM designationVM = new DesignationVM(); ;
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
d.Id
, d.Name
, d.Code
, d.BranchId
, ISNULL(d.EPZ, 0 )EPZ
, ISNULL(d.Other, 0 )Other
, ISNULL(d.DinnerAmount, 0 )DinnerAmount
, ISNULL(d.IfterAmount, 0 )IfterAmount
, ISNULL(d.TiffinAmount, 0 )TiffinAmount
, ISNULL(d.ETiffinAmount, 0 )ETiffinAmount
, ISNULL(d.AttendenceBonus,0)AttendenceBonus
, ISNULL(d.PriorityLevel,0)PriorityLevel
, d.OTAlloawance
, d.OTOrginal
, d.OTBayer
, d.ExtraOT
, ISNULL(d.OrderNo,0)OrderNo

FROM Designation d
left outer join EmployeePromotion ep on d.id=ep.DesignationId and ep.IsCurrent=1
Where  ep.EmployeeId=@EmployeeId 
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.Transaction = transaction;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId ", EmployeeId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    // designation = new DesignationVM();
                    designationVM.Id = dr["Id"].ToString();
                    designationVM.EPZ = Convert.ToDecimal(dr["EPZ"].ToString());
                    designationVM.Other = Convert.ToDecimal(dr["Other"].ToString());
                    designationVM.DinnerAmount = Convert.ToDecimal(dr["DinnerAmount"].ToString());
                    designationVM.IfterAmount = Convert.ToDecimal(dr["IfterAmount"].ToString());
                    designationVM.TiffinAmount = Convert.ToDecimal(dr["TiffinAmount"].ToString());
                    designationVM.ETiffinAmount = Convert.ToDecimal(dr["ETiffinAmount"].ToString());
                    designationVM.AttendenceBonus = Convert.ToDecimal(dr["AttendenceBonus"].ToString());
                    designationVM.PriorityLevel = Convert.ToInt32(dr["PriorityLevel"].ToString());

                    designationVM.OTAlloawance = Convert.ToBoolean(dr["OTAlloawance"].ToString());
                    designationVM.OTOrginal = Convert.ToBoolean(dr["OTOrginal"].ToString());
                    designationVM.OTBayer = Convert.ToBoolean(dr["OTBayer"].ToString());
                    designationVM.ExtraOT = Convert.ToBoolean(dr["ExtraOT"].ToString());
                    designationVM.OrderNo = Convert.ToInt32(dr["OrderNo"].ToString());

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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

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


            return designationVM;
        }


        public string[] Insert(DesignationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "DesignationDataInsert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(vm.Code))
                {
                    retResults[1] = "Please Input Designation Code";
                    return retResults;
                }
                else if (string.IsNullOrEmpty(vm.Name))
                {
                    retResults[1] = "Please Input Designation Name";
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
                string tableName = "Designation";
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }

                }
                #endregion Exist

                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(Code)Code FROM Designation ";
                //sqlText += " WHERE Code=@Code and BranchId=@BranchId";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Code", vm.Code);
                //cmdExist.Parameters.AddWithValue("@BranchId", vm.BranchId);
                //var exeRes = cmdExist.ExecuteNonQuery();
                //int objfoundId = Convert.ToInt32(exeRes);

                //if (objfoundId > 0)
                //{
                //    retResults[1] = "Code already used!";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Code already used!", "");
                //}
                //#endregion Exist
                #region Save

                //int foundId = (int)objfoundId;


                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from Designation where BranchId=@BranchId";
                SqlCommand cmd111 = new SqlCommand(sqlText, currConn);
                cmd111.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd111.Transaction = transaction;
				var exeRes1 = cmd111.ExecuteScalar();
				int count = Convert.ToInt32(exeRes1);

                vm.Id = vm.BranchId.ToString() + "_" + (count + 1);

                //if (foundId <= 0)
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO dbo.Designation ( 
Id,
            BranchId 
           , Code 
           , Name 
           , Remarks 
           , IsActive 
           , IsArchive 
           , CreatedBy 
           , CreatedAt 
           , CreatedFrom 
           , AttendenceBonus 
           , PriorityLevel 
           , EPZ 
           , Other 
           , DinnerAmount 
           , IfterAmount 
           , TiffinAmount 
           , ETiffinAmount 
           , OTAlloawance 
           , OTOrginal 
           , OTBayer 
           , ExtraOT
           ,DesignationGroupId
           ,GradeId
           ,OrderNo
           ,HospitalPlanC1
           ,HospitalPlanC2
           ,HospitalPlanC3
           ,HospitalPlanC4
           ,HospitalPlanC5
           ,DeathCoveragePlanC6
           ,MaternityPlanC7
           ,MaternityPlanC8
           ,MaternityPlanC9
           ,EntitlementC1
           ,EntitlementC2
           ,EntitlementC3
           ,EntitlementC4
           ,EntitlementC5
           ,MobileExpenseC1
           ,MobileExpenseC2
           ,MobileExpenseC3
           ,MobileExpenseC4
           ,InternationalTravelC1
           ,InternationalTravelC2
           ,InternationalTravelC3
           ,DomesticlTravelC1
           ,DomesticTravelC2
           ,DomesticTravelC3
           ,DomesticTravelC4
           ,DomesticTravelC5
           
)";
           sqlText += @" VALUES (
@Id,
           @BranchId 
           , @Code 
           , @Name 
           , @Remarks 
           , @IsActive 
           , @IsArchive 
           , @CreatedBy 
           , @CreatedAt 
           , @CreatedFrom 
           , @AttendenceBonus
           , @PriorityLevel 
           , @EPZ 
           , @Other 
           , @DinnerAmount 
           , @IfterAmount 
           , @TiffinAmount 
           , @ETiffinAmount 
           , @OTAlloawance 
           , @OTOrginal 
           , @OTBayer 
           , @ExtraOT 
           , @DesignationGroupId
           , @GradeId
           , @OrderNo
           , @HospitalPlanC1
           , @HospitalPlanC2
           , @HospitalPlanC3
           , @HospitalPlanC4
           , @HospitalPlanC5
           , @DeathCoveragePlanC6
           , @MaternityPlanC7
           , @MaternityPlanC8
           , @MaternityPlanC9
           , @EntitlementC1
           , @EntitlementC2
           , @EntitlementC3
           , @EntitlementC4
           , @EntitlementC5
           , @MobileExpenseC1
           , @MobileExpenseC2
           , @MobileExpenseC3
           , @MobileExpenseC4
           , @InternationalTravelC1
           , @InternationalTravelC2
           , @InternationalTravelC3
           , @DomesticlTravelC1
           , @DomesticTravelC2
           , @DomesticTravelC3
           , @DomesticTravelC4
           , @DomesticTravelC5
           
           
) ";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name.Trim());
                    cmdUpdate.Parameters.AddWithValue("@AttendenceBonus", vm.AttendenceBonus);
                    cmdUpdate.Parameters.AddWithValue("@PriorityLevel", vm.PriorityLevel);
                    cmdUpdate.Parameters.AddWithValue("@EPZ", vm.EPZ);
                    cmdUpdate.Parameters.AddWithValue("@Other", vm.Other);
                    cmdUpdate.Parameters.AddWithValue("@DinnerAmount", vm.DinnerAmount);
                    cmdUpdate.Parameters.AddWithValue("@IfterAmount", vm.IfterAmount);
                    cmdUpdate.Parameters.AddWithValue("@TiffinAmount", vm.TiffinAmount);
                    cmdUpdate.Parameters.AddWithValue("@ETiffinAmount", vm.ETiffinAmount);
                    cmdUpdate.Parameters.AddWithValue("@OTAlloawance", vm.OTAlloawance);
                    cmdUpdate.Parameters.AddWithValue("@OTOrginal", vm.OTOrginal);
                    cmdUpdate.Parameters.AddWithValue("@OTBayer", vm.OTBayer);
                    cmdUpdate.Parameters.AddWithValue("@ExtraOT", vm.ExtraOT);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", false);
                    cmdUpdate.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdUpdate.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdUpdate.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdUpdate.Parameters.AddWithValue("@DesignationGroupId", vm.DesignationGroupId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@GradeId", vm.GradeId);
                    cmdUpdate.Parameters.AddWithValue("@OrderNo", vm.OrderNo);

                    cmdUpdate.Parameters.AddWithValue("@HospitalPlanC1", vm.HospitalPlanC1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@HospitalPlanC2", vm.HospitalPlanC2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@HospitalPlanC3", vm.HospitalPlanC3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@HospitalPlanC4", vm.HospitalPlanC4 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@HospitalPlanC5", vm.HospitalPlanC5 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DeathCoveragePlanC6", vm.DeathCoveragePlanC6 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MaternityPlanC7", vm.MaternityPlanC7 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MaternityPlanC8", vm.MaternityPlanC8 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MaternityPlanC9", vm.MaternityPlanC9 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@EntitlementC1", vm.EntitlementC1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EntitlementC2", vm.EntitlementC2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EntitlementC3", vm.EntitlementC3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EntitlementC4", vm.EntitlementC4 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EntitlementC5", vm.EntitlementC5 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@MobileExpenseC1", vm.MobileExpenseC1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MobileExpenseC2", vm.MobileExpenseC2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MobileExpenseC3", vm.MobileExpenseC3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MobileExpenseC4", vm.MobileExpenseC4 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@InternationalTravelC1", vm.InternationalTravelC1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@InternationalTravelC2", vm.InternationalTravelC2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@InternationalTravelC3", vm.InternationalTravelC3 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@DomesticlTravelC1", vm.DomesticlTravelC1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DomesticTravelC2", vm.DomesticTravelC2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DomesticTravelC3", vm.DomesticTravelC3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DomesticTravelC4", vm.DomesticTravelC4 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DomesticTravelC5", vm.DomesticTravelC5 ?? Convert.DBNull);



                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.ExecuteNonQuery();

                 
                }
                else
                {
                    retResults[1] = "Please Input Designation Value";
                    throw new ArgumentNullException("Please Input Designation Value", "");
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
        //==================Update Designation=================
        public string[] Update(DesignationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DesignationUpdate"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToDesignation"); }

                #endregion open connection and transaction
                #region Exist

                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Designation";
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInUpdateWithBranch(vm.Id, tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }

                }
                #endregion Exist
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE Designation set";
                    sqlText += " Id=@Id";
                    sqlText += " ,BranchId=@BranchId";
                    sqlText += " , Code=@Code";
                    sqlText += " , Name=@Name";
                    sqlText += " , EPZ=@EPZ ";
                    sqlText += " , Other=@Other ";

                    sqlText += " , DinnerAmount=@DinnerAmount ";
                    sqlText += " , IfterAmount=@IfterAmount ";
                    sqlText += " , TiffinAmount=@TiffinAmount ";
                    sqlText += " , ETiffinAmount=@ETiffinAmount";
                    sqlText += " , OTAlloawance=@OTAlloawance ";
                    sqlText += " , OTOrginal=@OTOrginal ";
                    sqlText += " , OTBayer=@OTBayer ";
                    sqlText += " , ExtraOT=@ExtraOT";
                    sqlText += " , AttendenceBonus=@AttendenceBonus";

                    sqlText += " , PriorityLevel=@PriorityLevel ";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";

                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";

                    //sqlText += ",DesignationGroupId=@DesignationGroupId";
                    sqlText += " , GradeId=@GradeId";
                    sqlText += " , OrderNo=@OrderNo ";

                    sqlText += " , HospitalPlanC1=@HospitalPlanC1 ";
                    sqlText += " , HospitalPlanC2=@HospitalPlanC2 ";
                    sqlText += " , HospitalPlanC3=@HospitalPlanC3 ";
                    sqlText += " , HospitalPlanC4=@HospitalPlanC4 ";
                    sqlText += " , HospitalPlanC5=@HospitalPlanC5 ";

                    sqlText += " , DeathCoveragePlanC6=@DeathCoveragePlanC6 ";
                    sqlText += " , MaternityPlanC7=@MaternityPlanC7 ";
                    sqlText += " , MaternityPlanC8=@MaternityPlanC8 ";
                    sqlText += " , MaternityPlanC9=@MaternityPlanC9 ";

                    sqlText += " , EntitlementC1=@EntitlementC1 ";
                    sqlText += " , EntitlementC2=@EntitlementC2 ";
                    sqlText += " , EntitlementC3=@EntitlementC3 ";
                    sqlText += " , EntitlementC4=@EntitlementC4 ";
                    sqlText += " , EntitlementC5=@EntitlementC5 ";

                    sqlText += " , MobileExpenseC1=@MobileExpenseC1 ";
                    sqlText += " , MobileExpenseC2=@MobileExpenseC2 ";
                    sqlText += " , MobileExpenseC3=@MobileExpenseC3 ";
                    sqlText += " , MobileExpenseC4=@MobileExpenseC4 ";

                    sqlText += " , InternationalTravelC1=@InternationalTravelC1 ";
                    sqlText += " , InternationalTravelC2=@InternationalTravelC2 ";
                    sqlText += " , InternationalTravelC3=@InternationalTravelC3 ";

                    sqlText += " , DomesticlTravelC1=@DomesticlTravelC1 ";
                    sqlText += " , DomesticTravelC2=@DomesticTravelC2 ";
                    sqlText += " , DomesticTravelC3=@DomesticTravelC3 ";
                    sqlText += " , DomesticTravelC4=@DomesticTravelC4 ";
                    sqlText += " , DomesticTravelC5=@DomesticTravelC5 ";
                    


                    sqlText += " where id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id.Trim());
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name.Trim());
                    cmdUpdate.Parameters.AddWithValue("@EPZ", vm.EPZ );
                    cmdUpdate.Parameters.AddWithValue("@Other", vm.Other );

                    cmdUpdate.Parameters.AddWithValue("@DinnerAmount", vm.DinnerAmount);
                    cmdUpdate.Parameters.AddWithValue("@IfterAmount", vm.IfterAmount);
                    cmdUpdate.Parameters.AddWithValue("@TiffinAmount", vm.TiffinAmount);
                    cmdUpdate.Parameters.AddWithValue("@ETiffinAmount", vm.ETiffinAmount);
                    cmdUpdate.Parameters.AddWithValue("@OTAlloawance ", vm.OTAlloawance);
                    cmdUpdate.Parameters.AddWithValue("@OTOrginal ", vm.OTOrginal);
                    cmdUpdate.Parameters.AddWithValue("@OTBayer ", vm.OTBayer);
                    cmdUpdate.Parameters.AddWithValue("@ExtraOT", vm.ExtraOT);
                    cmdUpdate.Parameters.AddWithValue("@AttendenceBonus", vm.AttendenceBonus);

                    cmdUpdate.Parameters.AddWithValue("@PriorityLevel", vm.PriorityLevel);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);

                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    //cmdUpdate.Parameters.AddWithValue("@DesignationGroupId", vm.DesignationGroupId);
                    cmdUpdate.Parameters.AddWithValue("@GradeId", vm.GradeId);
                    cmdUpdate.Parameters.AddWithValue("@OrderNo", vm.OrderNo);

                    cmdUpdate.Parameters.AddWithValue("@HospitalPlanC1", vm.HospitalPlanC1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@HospitalPlanC2", vm.HospitalPlanC2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@HospitalPlanC3", vm.HospitalPlanC3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@HospitalPlanC4", vm.HospitalPlanC4 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@HospitalPlanC5", vm.HospitalPlanC5 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@DeathCoveragePlanC6", vm.DeathCoveragePlanC6 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@MaternityPlanC7", vm.MaternityPlanC7 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MaternityPlanC8", vm.MaternityPlanC8 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MaternityPlanC9", vm.MaternityPlanC9 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@EntitlementC1", vm.EntitlementC1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EntitlementC2", vm.EntitlementC2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EntitlementC3", vm.EntitlementC3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EntitlementC4", vm.EntitlementC4 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EntitlementC5", vm.EntitlementC5 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@MobileExpenseC1", vm.MobileExpenseC1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MobileExpenseC2", vm.MobileExpenseC2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MobileExpenseC3", vm.MobileExpenseC3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MobileExpenseC4", vm.MobileExpenseC4 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@InternationalTravelC1", vm.InternationalTravelC1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@InternationalTravelC2", vm.InternationalTravelC2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@InternationalTravelC3", vm.InternationalTravelC3 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@DomesticlTravelC1", vm.DomesticlTravelC1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DomesticTravelC2", vm.DomesticTravelC2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DomesticTravelC3", vm.DomesticTravelC3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DomesticTravelC4", vm.DomesticTravelC4 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DomesticTravelC5", vm.DomesticTravelC5 ?? Convert.DBNull);

                    //cmdUpdate.Parameters.AddWithValue("@HospitalizationBenifiteC1", vm.HospitalizationBenifiteC1);
                    //cmdUpdate.Parameters.AddWithValue("@RoomBoardLimitC2", vm.RoomBoardLimitC2);
                    //cmdUpdate.Parameters.AddWithValue("@DaliyRoomLimitC3", vm.DaliyRoomLimitC3);
                    //cmdUpdate.Parameters.AddWithValue("@IntensiveCareUniteC4", vm.IntensiveCareUniteC4);
                    //cmdUpdate.Parameters.AddWithValue("@HospitalServiceC5", vm.HospitalServiceC5);
                    //cmdUpdate.Parameters.AddWithValue("@DeathCoverageC6", vm.DeathCoverageC6);
                    //cmdUpdate.Parameters.AddWithValue("@PatientMaternityPlanC7", vm.PatientMaternityPlanC7);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("DesignationUpdate", vm.Name + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("DesignationUpdate", "Could not found any item.");
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
                    retResults[1] = "Data Update Successfully";

                }
                else
                {
                    retResults[1] = "Unexpected error to update Designation.";
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
        public string[] Delete(DesignationVM vm, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DesignationDelete"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToDesignation"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used
                if (ids.Length>1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update Designation set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }
                    

                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("DesignationDelete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("DesignationDelete", "Could not found any item.");
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
                    retResults[1] = "Data Delete Successfully";
                }
                else
                {
                    retResults[1] = "Unexpected error to delete Designation.";
                    throw new ArgumentNullException("","");
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
        public List<DesignationVM> DropDown()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<DesignationVM> VMs = new List<DesignationVM>();
            DesignationVM vm;
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
Name
   FROM Designation
WHERE IsArchive=0 and IsActive=1
    ORDER BY Name
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DesignationVM();
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
                sqlText = @"SELECT Id, Name    FROM Designation ";
                sqlText += @" WHERE Name like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY Name";



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
                vms.Sort();
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

            return vms;
        }


        public List<DesignationGroupVM> DesignationGroupDropDown(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<DesignationGroupVM> VMs = new List<DesignationGroupVM>();
            DesignationGroupVM vm;
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
Id
,Name
FROM DesignationGroup

";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DesignationGroupVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
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
            return VMs;
        }



        #endregion
    }
}
