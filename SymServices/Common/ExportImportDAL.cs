using SymOrdinary;

using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Common
{
    public class ExportImportDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion


        //==================SelectAll=================



        //        public DataTable SelectAll()
        //        {

        //            #region Variables

        //            SqlConnection currConn = null;
        //            string sqlText = "";
        //            List<ExportImportVM> exportImportVMs = new List<ExportImportVM>();
        //            ExportImportVM exportImportVM;
        //            #endregion
        //            try
        //            {
        //                #region open connection and transaction
        //                currConn = _dbsqlConnection.GetConnection();
        //                if (currConn.State != ConnectionState.Open)
        //                {
        //                    currConn.Open();
        //                }
        //                #endregion open connection and transaction

        //                #region sql statement
        //                sqlText = @"
        //SELECT
        // e.Id
        //,e.BranchId
        //,e.EmpName 
        //,e.Code
        //,e.Designation
        //,e.Branch
        //,e.Department
        //,e.Section
        //,e.Project 
        //,e.Salutation_E
        //,e.MiddleName
        //,e.LastName
        //,e.JoinDate
        //,empt.Other1
        //,empt.Other2
        //,empt.Other3
        //,empt.Other4
        //,empt.Other5
        //From ViewEmployeeInformation e
        //left outer join EmployeeTransfer empt on  e.EmployeeId=empt.EmployeeId
        //Where e.IsArchive=0 AND e.IsActive=1
        //and empt.IsCurrent=1
        //
        //";

        //                sqlText += "     ORDER BY e.Department, e.EmpName desc";
        //                SqlCommand objComm = new SqlCommand(sqlText, currConn);

        //                SqlDataReader dr;
        //                dr = objComm.ExecuteReader();
        //                while (dr.Read())
        //                {
        //                    vm = new EmployeeInfoVM();
        //                    vm.Id = dr["Id"].ToString();
        //                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
        //                    vm.Code = dr["Code"].ToString();
        //                    vm.Salutation_E = dr["Salutation_E"].ToString();
        //                    vm.MiddleName = dr["MiddleName"].ToString();
        //                    vm.LastName = dr["LastName"].ToString();
        //                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
        //                    vm.EmpName = dr["EmpName"].ToString();
        //                    vm.Code = dr["Code"].ToString();
        //                    vm.Designation = dr["Designation"].ToString();
        //                    vm.Branch = dr["Branch"].ToString();
        //                    vm.Department = dr["Department"].ToString();
        //                    vm.Section = dr["Section"].ToString();
        //                    vm.Project = dr["Project"].ToString();
        //                    vm.Other1 = dr["Other1"].ToString();
        //                    vm.Other2 = dr["Other2"].ToString();
        //                    vm.Other3 = dr["Other3"].ToString();
        //                    vm.Other4 = dr["Other4"].ToString();
        //                    vm.Other5 = dr["Other5"].ToString();
        //                    VMs.Add(vm);
        //                }
        //                dr.Close();
        //                #endregion
        //            }
        //            #region catch
        //            catch (SqlException sqlex)
        //            {
        //                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //            }
        //            #endregion
        //            #region finally
        //            finally
        //            {
        //                if (currConn != null)
        //                {
        //                    if (currConn.State == ConnectionState.Open)
        //                    {
        //                        currConn.Close();
        //                    }
        //                }
        //            }
        //            #endregion

        //            return exportImportVMs;
        //        }

        public DataTable SelectEmpInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"
SELECT
      vei.[EmployeeId]
	  ,vei.code
	  ,vei.EmpName
	  ,case when vei.Gender = 'Male' then 'Leave for Male' else 'Leave for female' end LeaveStructureName
      ,'' [LeaveYear]
From ViewEmployeeInformation vei
Where vei.IsArchive=0 AND vei.IsActive=1
 and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


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

        public DataTable SelectEmployeeInfo(ExportImportVM VM)
        {
            #region Variables
            SqlConnection currConn = null;
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

                #region SQL query
                string sqlText = @"
             SELECT
                         a.Code, 
                         a.Name, 
                         b.Name AS Department,
                         c.Name AS Designation,
                         d.Name AS Project,
                         e.Name AS Section,
                         a.BasicSalary,
                         a.GrossSalary,
                         a.DateOfBirth,
                         a.JoinDate,
                         a.IsActive,
                         a.ContactNo,
                         a.Email,
                         a.Remarks

                        , a.NomineeName
                        , a.NomineeDateofBirth
                        , a.NomineeRelation
                        , a.NomineeAddress
                        , a.NomineeDistrict
                        ,a.NomineeDivision
                        , a.NomineeCountry
                        , a.NomineeCity
                        , a.NomineePostalCode
                        , a.NomineePhone
                        , a.NomineeMobile
                        , a.NomineeBirthCertificateNo
                        , a.NomineeFax
                        , a.NomineeFileName
                        , a.NomineeRemarks
                        , a.NomineeNID
                        , a.BranchId
                        from EmployeeInfo a 
						left join Department b on a.Department = b.Id
						left join Designation c on a.Designation = c.Id
						left join Project d on a.Project = d.Id
						left join Section e on a.Section = e.Id
                        where a.BranchId = @BranchId and a.IsActive =1 ";

                #endregion SQL query

                // Create SqlCommand and SqlDataAdapter for the query
                SqlCommand cmdd = new SqlCommand(sqlText, currConn);
                cmdd.Parameters.AddWithValue("@BranchId", VM.BranchId ?? "");
                SqlDataAdapter adapterd = new SqlDataAdapter(cmdd);

                // Fill the DataTable with data from the database
                adapterd.Fill(dt);

                #region Additional sample data handling
                // If the DataTable is empty, you can add sample data for demo purposes
                if (dt.Rows.Count == 0)
                {
                    sqlText = @"
                SELECT
                    '1001' Code, 
                    'Enter Name' Name, 
                    'Development' Department,
                    'Sr. Software Developer' Designation,
                    '1_1' Project,
                    '1_1' Section,
                    0 BasicSalary,
                    0 GrossSalary,
                    '01-Jan-1999' DateOfBirth,
                    '01-Jan-1999' JoinDate,
                    'True' IsActive,
                    '01710-0000000' ContactNo,
                    'example@gmail.com' Email,
                    'Remarks' Remarks,
                    'Nominee Name' NomineeName,
                    'Nominee Date of Birth' NomineeDateofBirth,
                    'Nominee Relation' NomineeRelation,
                    'Nominee Address' NomineeAddress,
                    'Nominee District' NomineeDistrict,
                    'Nominee Division' NomineeDivision,
                    'Nominee Country' NomineeCountry,
                    'Nominee City' NomineeCity,
                    'Nominee Postal Code' NomineePostalCode,
                    'Nominee Phone' NomineePhone,
                    'Nominee Mobile' NomineeMobile,
                    'Nominee Birth Certificate No' NomineeBirthCertificateNo,
                    'Nominee Fax' NomineeFax,
                    'Nominee File Name' NomineeFileName,
                    'Nominee Remarks' NomineeRemarks,
                    'Nominee NID' NomineeNID
                    'Branch' BranchId";

                    // Execute the second query if no data is found
                    SqlCommand cmddSample = new SqlCommand(sqlText, currConn);
                    SqlDataAdapter adapterSample = new SqlDataAdapter(cmddSample);
                    adapterSample.Fill(dt);
                }
                #endregion Additional sample data handling
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + FieldDelimeter + sqlex.Message);
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + FieldDelimeter + ex.Message);
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


        public DataTable SelectPersonalDetail(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT
vei.EmployeeId
,vei.Code
,epd.OtherId
,vei.EmpName 
,epd.FatherName
,epd.MotherName
,epd.SpouseName
,epd.PersonalContactNo
,epd.CorporateContactNo
,epd.CorporateContactLimit
,epd.Gender_E
,epd.MaritalStatus_E
,epd.Nationality_E
,epd.DateOfBirth
,epd.NickName
,CASE
WHEN epd.Smoker =1  THEN '1'  ELSE '0'
END Smoker
,epd.NID
,epd.PassportNumber
,epd.ExpiryDate
,epd.Religion
,epd.TIN
,epd.KindsOfDisability
,CASE
WHEN epd.IsDisable =1  THEN '1'  ELSE '0'
END IsDisable
,epd.Email
,epd.BloodGroup_E
,epd.PlaceOfBirth
,epd.MarriageDate
,epd.SpouseProfession
,epd.SpouseDateOfBirth
,epd.SpouseBloodGroup
,epd.HRMSCode
,epd.WDCode
,epd.PersonalEmail
,epd.TPNCode
,epd.DisabilityType
,CASE
WHEN epd.IsVaccineDose1Complete =1  THEN '1'  ELSE '0'
END IsVaccineDose1Complete
,CASE
WHEN epd.IsVaccineDose2Complete =1  THEN '1'  ELSE '0'
END IsVaccineDose2Complete
,CASE
WHEN epd.IsVaccineDose3Complete =1  THEN '1'  ELSE '0'
END IsVaccineDose3Complete
,epd.VaccineDose1Date
,epd.VaccineDose1Name
,epd.VaccineDose2Date
,epd.VaccineDose2Name
,epd.VaccineDose3Date
,epd.VaccineDose3Name
,epd.NoChildren
,epd.Heightft
,epd.HeightIn
,epd.Weight
,epd.ChestIn
From ViewEmployeeInformation vei
left outer join EmployeePersonalDetail epd on epd.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "DateOfBirth");
                dt = Ordinary.DtColumnStringToDate(dt, "ExpiryDate");
                dt = Ordinary.DtColumnStringToDate(dt, "MarriageDate");
                dt = Ordinary.DtColumnStringToDate(dt, "SpouseDateOfBirth");
                dt = Ordinary.DtColumnStringToDate(dt, "VaccineDose1Date");
                dt = Ordinary.DtColumnStringToDate(dt, "VaccineDose2Date");
                dt = Ordinary.DtColumnStringToDate(dt, "VaccineDose3Date");

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

        public DataTable SelectEmployeeJob(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT
      vei.[EmployeeId]
	  ,vei.code
	  ,vei.EmpName
      ,j.[JoinDate]
      ,j.[LeftDate]
      ,j.[ProbationEnd]
      ,j.[DateOfPermanent]
      ,j.[EmploymentStatus_E]
      ,j.[EmploymentType_E]
      ,j.[Supervisor]
      ,j.[Remarks]
,CASE
WHEN j.IsActive =1  THEN '1'  ELSE '0'
END IsActive
,CASE
WHEN j.IsArchive =1  THEN '1'  ELSE '0'
END IsArchive

,CASE
WHEN j.IsPermanent =1  THEN '1'  ELSE '0'
END IsPermanent
      
      ,j.[StructureGroupId]
      ,j.[GrossSalary]
      ,j.[BasicSalary]
      ,j.[BankInfo]
      ,j.[BankAccountNo]
      ,j.[ProbationMonth]
	  ,P.Name ProjectName
	  ,D.Name DepartmenName
	  ,Sec.Name SectionName
	  ,Deg.Name Designation
      ,j.[BankPayAmount]
      ,j.[Other1]
      ,j.[Other2]
      ,j.[Other3]
      ,j.[Other4]
      ,j.[Other5]
      ,j.[AccountType]
,CASE
WHEN j.IsJobBefore =1  THEN '1'  ELSE '0'
END IsJobBefore
      ,j.[FirstHoliday]
      ,j.[SecondHoliday]
      ,j.[Other1Id]
      ,j.[Other2Id]
      ,j.[Other3Id]
      ,j.[Other4Id]
      ,j.[Other5Id]
,CASE
WHEN j.ExtraOT =1  THEN '1'  ELSE '0'
END ExtraOT
,CASE
WHEN j.OTBayer =1  THEN '1'  ELSE '0'
END OTBayer
,CASE
WHEN j.OTOrginal =1  THEN '1'  ELSE '0'
END OTOrginal
,CASE
WHEN j.OTAlloawance =1  THEN '1'  ELSE '0'
END OTAlloawance
      ,j.[AttendenceBonus]
      ,j.[BankAccountName]
      ,j.[Routing_No]
,CASE
WHEN j.IsTAXApplicable =1  THEN '1'  ELSE '0'
END IsTAXApplicable
,CASE
WHEN j.IsCarTAXApplicable =1  THEN '1'  ELSE '0'
END IsCarTAXApplicable
      ,j.[BonusNumber]
      ,j.[GFStartFrom]
      ,j.[ExtendedProbationMonth]
,CASE
WHEN j.IsPFApplicable =1  THEN '1'  ELSE '0'
END IsPFApplicable
,CASE
WHEN IsGFApplicable =1  THEN '1'  ELSE '0'
END IsGFApplicable 
,CASE
WHEN j.IsInactive =1  THEN '1'  ELSE '0'
END IsInactive

      ,j.[FromDate]
      ,j.[ToDate]
      ,j.[Force]
      ,j.[Rank]
      ,j.[Duration]
      ,j.[Retirement]
      ,j.[EmpJobType]
      ,j.[EmpCategory]
      ,j.[IsBuild]
,CASE
WHEN j.IsBuild =1  THEN '1'  ELSE '0'
END IsBuild
      ,j.[ContrExDate]
      ,j.[Extentionyn]
      ,j.[secondExDate]
      ,j.[fristExDate]
      ,j.[RetirementDate]
      ,j.[DotedLineReport]

From ViewEmployeeInformation vei
left outer join EmployeeJob j on j.EmployeeId=vei.EmployeeId
left outer join Department D on j.DepartmentId=D.Id
left outer join Designation Deg on j.DesignationId=Deg.Id
left outer join Section Sec on j.DesignationId=Sec.Id
left outer join Project P on j.ProjectId=P.Id
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "LeftDate");
                dt = Ordinary.DtColumnStringToDate(dt, "LeftDate");
                dt = Ordinary.DtColumnStringToDate(dt, "JoinDate");
                dt = Ordinary.DtColumnStringToDate(dt, "FromDate");
                dt = Ordinary.DtColumnStringToDate(dt, "ToDate");
                dt = Ordinary.DtColumnStringToDate(dt, "ContrExDate");
                dt = Ordinary.DtColumnStringToDate(dt, "secondExDate");
                dt = Ordinary.DtColumnStringToDate(dt, "fristExDate");
                dt = Ordinary.DtColumnStringToDate(dt, "RetirementDate");
                dt = Ordinary.DtColumnStringToDate(dt, "DateOfPermanent");





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

        public DataTable SelectAsset(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT
		edu.AssetId
       ,vei.[EmployeeId]
	   ,vei.code
	   ,vei.EmpName
      ,edu.[IssueDate]
      ,edu.[FileName]
      ,edu.[Remarks]
      ,CASE
WHEN edu.IsActive =1  THEN '1'  ELSE '0'
END IsActive
,CASE
WHEN edu.IsArchive =1  THEN '1'  ELSE '0'
END IsArchive
      ,edu.[CreatedBy]
      ,edu.[CreatedAt]
      ,edu.[CreatedFrom]
      ,edu.[LastUpdateBy]
      ,edu.[LastUpdateAt]
      ,edu.[LastUpdateFrom]
From ViewEmployeeInformation vei
left outer join [EmployeeAssets] edu on edu.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "IssueDate");

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

        public DataTable SelectEmployeeEducation(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT

       vei.[EmployeeId]
	   ,vei.code
	   ,vei.EmpName
      ,edu.[Degree_E]
      ,edu.[Institute]
      ,edu.[Major]
      ,edu.[YearOfPassing]
      ,edu.[IsLast]
      
      ,edu.[Remarks]
      ,edu.[CGPA]
      ,edu.[Scale]
      ,edu.[Result]
      ,edu.[Marks]
      ,edu.[TotalYear]

From ViewEmployeeInformation vei
left outer join EmployeeEducation edu on edu.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "YearOfPassing");

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

        public DataTable SelectEmployeeProfessionalDegree(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT

		vei.[EmployeeId]
	   ,vei.code
	   ,vei.EmpName
      ,p.[Degree_E]
      ,p.[Institute]
      ,p.[YearOfPassing]
      ,p.[IsLast]
      
      ,p.[Remarks]
      ,CASE
WHEN p.IsActive =1  THEN '1'  ELSE '0'
END IsActive
,CASE
WHEN p.IsArchive =1  THEN '1'  ELSE '0'
END IsArchive

      ,p.[Marks]
      ,p.[TotalYear]
      ,p.[Level]
From ViewEmployeeInformation vei
left outer join EmployeeProfessionalDegree p on p.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "YearOfPassing");

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

        public DataTable SelectEmployeeLanguage(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT

	  vei.[EmployeeId]
	  ,vei.code
	  ,vei.EmpName
      ,L.[Language_E]
      ,L.[Fluency_E]
      ,L.[Competency_E]
      ,L.[Remarks]
,CASE
WHEN L.IsActive =1  THEN '1'  ELSE '0'
END IsActive
,CASE
WHEN L.IsArchive =1  THEN '1'  ELSE '0'
END IsArchive

From ViewEmployeeInformation vei
left outer join EmployeeLanguage L on L.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


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

        public DataTable SelectEmployeeExtraCurriculumActivities(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"


SELECT

	  vei.[EmployeeId]
	  ,vei.code
	  ,vei.EmpName
      ,ex.[Skill]
      ,ex.[YearsOfExperience]
      ,ex.[SkillQuality_E]
      ,ex.[Institute]
      ,ex.[Address]
      ,ex.[Date]
      ,ex.[Achievement]
      ,ex.[Remarks]
,CASE
WHEN ex.IsActive =1  THEN '1'  ELSE '0'
END IsActive
,CASE
WHEN ex.IsArchive =1  THEN '1'  ELSE '0'
END IsArchive

From ViewEmployeeInformation vei
left outer join EmployeeExtraCurriculumActivities ex on ex.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "Date");
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

        public DataTable SelectEmployeeImmigration(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT
      vei.[EmployeeId]
	  ,vei.Code
	  ,vei.EmpName
      ,im.[ImmigrationType_E]
      ,im.[ImmigrationNumber]
      ,im.[IssueDate]
      ,im.[ExpireDate]
      ,im.[IssuedBy_E]
      ,im.[EligibleReviewDate]
      ,im.[Remarks]
   ,CASE
WHEN im.IsActive =1  THEN '1'  ELSE '0'
END IsActive
,CASE
WHEN im.IsArchive =1  THEN '1'  ELSE '0'
END IsArchive

From ViewEmployeeInformation vei
left outer join EmployeeImmigration im on im.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "IssueDate");
                dt = Ordinary.DtColumnStringToDate(dt, "ExpireDate");
                dt = Ordinary.DtColumnStringToDate(dt, "EligibleReviewDate");

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

        public DataTable SelectEmployeeTraining(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"
SELECT

	   vei.[EmployeeId]
	   ,vei.Code
	   ,vei.EmpName
      ,tr.[TrainingStatus_E]
      ,tr.[TrainingPlace_E]
      ,tr.[Topics]
      ,tr.[InstituteName]
      ,tr.[Location]
      ,tr.[FundedBy]
      ,tr.[DurationMonth]
      ,tr.[DurationDay]
      ,tr.[Achievement]
      ,tr.[AllowancesTotalTk]
      ,tr.[Remarks]
      ,tr.[DateFrom]
      ,tr.[DateTo]
From ViewEmployeeInformation vei
left outer join EmployeeTraining tr  on tr.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "DateFrom");
                dt = Ordinary.DtColumnStringToDate(dt, "DateTo");
                //dt = Ordinary.DtColumnStringToDate(dt, "EligibleReviewDate");
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

        public DataTable SelectEmployeeTravel(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT

        vei.[EmployeeId]
	   ,vei.Code
	   ,vei.EmpName
      ,tv.[TravelType_E]
      ,tv.[TravelFromAddress]
      ,tv.[TravelToAddress]
      ,tv.[FromDate]
      ,tv.[ToDate]
      ,tv.[FromTime]
      ,tv.[ToTime]
      ,tv.[Remarks]
     ,CASE
WHEN tv.IsActive =1  THEN '1'  ELSE '0'
END IsActive
,CASE
WHEN tv.IsArchive =1  THEN '1'  ELSE '0'
END IsArchive

      ,tv.[Allowances]
      ,tv.[IssueDate]
      ,tv.[ExpiryDate]
      ,tv.[Country]
      ,tv.[PassportNumber]
      ,tv.[EmbassyName]

From ViewEmployeeInformation vei
left outer join EmployeeTravel tv on tv.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "FromDate");
                dt = Ordinary.DtColumnStringToDate(dt, "ToDate");

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

        public DataTable SelectEmployeeNominee(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT

       vei.[EmployeeId]
       ,vei.Code
	   ,vei.EmpName

      ,nome.[DateofBirth]
      ,nome.[Name]
      ,nome.[Relation]
      ,nome.[Address]
      ,nome.[District]
      ,nome.[Division]
      ,nome.[Country]
      ,nome.[City]
      ,nome.[PostalCode]
      ,nome.[Phone]
      ,nome.[Mobile]
      ,nome.[BirthCertificateNo]
      ,nome.[Fax]
      
      ,nome.[Remarks]
      ,nome.[IsActive]
      ,nome.[IsArchive]
      ,nome.[NID]
      ,nome.[IsVaccineDose1Complete]
      ,nome.[VaccineDose1Date]
      ,nome.[VaccineDose1Name]
      ,nome.[IsVaccineDose2Complete]
      ,nome.[VaccineDose2Date]
      ,nome.[VaccineDose2Name]
      ,nome.[IsVaccineDose3Complete]
      ,nome.[VaccineDose3Date]
      ,nome.[VaccineDose3Name]
      ,nome.[PostOffice]
      ,nome.[VaccineFile3]
      ,nome.[VaccineFiles2]
      ,nome.[VaccineFile1]
From  ViewEmployeeInformation  vei
left outer join EmployeeNominee nome  on nome.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "DateofBirth");
                dt = Ordinary.DtColumnStringToDate(dt, "VaccineDose1Date");
                dt = Ordinary.DtColumnStringToDate(dt, "VaccineDose2Date");
                dt = Ordinary.DtColumnStringToDate(dt, "VaccineDose3Date");



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

        public DataTable SelectEmployeeDependent(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT

       vei.[EmployeeId]
       ,vei.Code
	   ,vei.EmpName
      ,dpt.[Name]
      ,dpt.[Relation]
      ,dpt.[DateofBirth]
      ,dpt.[Address]
      ,dpt.[District]
      ,dpt.[Division]
      ,dpt.[Country]
      ,dpt.[City]
      ,dpt.[BirthCertificateNo]
      ,dpt.[PostalCode]
      ,dpt.[Phone]
      ,dpt.[Mobile]
      ,dpt.[Fax]
      ,dpt.[Remarks]


      ,dpt.[Gender]
      ,dpt.[EducationQualification]
      ,dpt.[BloodGroup]
,CASE
WHEN dpt.IsDependentAllowance =1  THEN '1'  ELSE '0'
END IsDependentAllowance
      ,dpt.[PostOffice]
,CASE
WHEN dpt.IsVaccineDose1Complete =1  THEN '1'  ELSE '0'
END IsVaccineDose1Complete
    
      ,dpt.[VaccineDose1Date]
      ,dpt.[VaccineDose1Name]
    ,CASE
WHEN dpt.IsVaccineDose2Complete =1  THEN '1'  ELSE '0'
END IsVaccineDose2Complete
      ,dpt.[VaccineDose2Date]
      ,dpt.[VaccineDose2Name]
     ,CASE
WHEN dpt.IsVaccineDose3Complete =1  THEN '1'  ELSE '0'
END IsVaccineDose3Complete
      ,dpt.[VaccineDose3Date]
      ,dpt.[VaccineDose3Name]
      ,dpt.[NID]
From ViewEmployeeInformation vei
left outer join EmployeeDependent dpt on dpt.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "DateofBirth");
                dt = Ordinary.DtColumnStringToDate(dt, "VaccineDose1Date");
                dt = Ordinary.DtColumnStringToDate(dt, "VaccineDose2Date");
                dt = Ordinary.DtColumnStringToDate(dt, "VaccineDose3Date");

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

        public DataTable SelectEmployeeLeftInformation(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT

	  vei.[EmployeeId]
	  ,vei.Code
	  ,vei.EmpName
      ,LT.[LeftType_E]
      ,LT.[EntryLeftDate]
      ,LT.[LeftDate]
      ,LT.[Remarks]
,CASE
WHEN LT.IsActive =1  THEN '1'  ELSE '0'
END IsActive
,CASE
WHEN LT.IsArchive =1  THEN '1'  ELSE '0'
END IsArchive

From ViewEmployeeInformation vei
left outer join EmployeeLeftInformation LT on LT.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "EntryLeftDate");
                dt = Ordinary.DtColumnStringToDate(dt, "LeftDate");
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

        public DataTable SelectEmployeeEmergencyContact(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT

      vei.[EmployeeId]
	  ,vei.Code
	  ,vei.EmpName
      ,EM.[Name]
      ,EM.[Relation]
      ,EM.[Address]
      ,EM.[District]
      ,EM.[Division]
      ,EM.[Country]
      ,EM.[City]
      ,EM.[PostalCode]
      ,EM.[Phone]
      ,EM.[Mobile]
      ,EM.[Fax]
      ,EM.[Remarks]
      ,CASE
WHEN EM.IsActive =1  THEN '1'  ELSE '0'
END IsActive
,CASE
WHEN EM.IsArchive =1  THEN '1'  ELSE '0'
END IsArchive

      ,EM.[Email]
      ,EM.[PostOffice]
From ViewEmployeeInformation vei
left outer join EmployeeEmergencyContact EM on EM.EmployeeId=vei.EmployeeId
Where vei.IsArchive=0 AND vei.IsActive=1
and  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


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
        public DataTable SelectDepartment(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"
SELECT 
       ''[Code]
      ,''[Name]
      ,''[Remarks]
  FROM [Department]";


                #endregion
                #region More Conditions


                #endregion


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
        public DataTable SelectDesignation(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"
SELECT  top 100
      ''[Code]
      ,''[Name]
      ,''[Remarks]
FROM Designation";


                #endregion
                #region More Conditions


                #endregion


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
        public DataTable SelectDesignationGroup(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT  top 100
      ''[Code]
      ,''[Name]
      ,''[Remarks]
FROM DesignationGroup";
                #endregion
                #region More Conditions


                #endregion


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
        public DataTable SelectBank(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT 
      ''[Code]
      ,''[Name]
      ,''[Remarks]
FROM Bank";

                #endregion
                #region More Conditions


                #endregion


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
        public DataTable SelectBranch(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT 
      ''[Code]
      ,''[Name]
      ,''[Remarks]
FROM Branch";

                #endregion
                #region More Conditions


                #endregion


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

        public DataTable SelectEmployeePF(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT
      vei.[EmployeeId]
      ,vei.EmpName EmployeeName
      ,vei.[Designation]
      
	  ,PF.Name PfStructureName
      ,EPF.[PFValue]
      ,CASE
WHEN EPF.IsFixed =1  THEN '1'  ELSE '0'
END IsFixed
      ,EPF.[PortionSalaryType]
	 
      
     
From ViewEmployeeInformation vei
left outer join EmployeePF EPF on EPF.EmployeeId=vei.EmployeeId
left outer join PFStructure PF on EPF.PFStructureId= PF.Id

where  vei.EmployeeId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


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

        public DataTable SelectEmployeeGroup(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"

SELECT 
      [BranchId]
      ,[Code]
      ,[Name]
      ,[Remarks]
FROM [EmployeeGroup]
Where IsArchive=0 AND IsActive=1
and  BranchId in (";

                int len = VM.IDs.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + VM.IDs[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                #endregion
                #region More Conditions


                #endregion


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
        public string[] InsertEmpInfo(EmployeeInfoVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeInfo"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();

            #endregion
            #region Try
            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(EmployeeInfoVM.DepartmentId))
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
                #region Save
                SettingDAL _settingDal = new SettingDAL();
                CommonDAL _commonDal = new CommonDAL();
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var AutoCode = _settingDal.settingValue("AutoCode", "Employee", currConn, transaction);
                if (AutoCode != "Y")
                {
                    if (string.IsNullOrWhiteSpace(vm.Code))
                    {
                        retResults[1] = "Please Enter the Employee Code!";
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    #region Code Exists
                    sqlText = @"Select isnull( isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) ,0)
                     from EmployeeInfo where BranchId=@BranchId and Code=@Code";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn, transaction);
                    cmdExist.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdExist.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    var exeRes = cmdExist.ExecuteScalar();
                    int count2 = Convert.ToInt32(exeRes);
                    if (count2 > 0)
                    {
                        retResults[1] = "Already this Employee Code is exist!";
                        throw new ArgumentNullException("Already this Employee Code is exist!", "");
                    }
                    #endregion Code Exists
                }

                #region Generate Id
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EmployeeInfo where BranchId=@BranchId";
                SqlCommand cmdIdGen = new SqlCommand(sqlText, currConn, transaction);
                cmdIdGen.Parameters.AddWithValue("@BranchId", vm.BranchId);
                var count = cmdIdGen.ExecuteScalar();
                vm.Id = vm.BranchId.ToString() + "_" + (Convert.ToInt32(count) + 1);
                //int foundId = (int)objfoundId;
                #endregion Generate Id
                #region No. of Employee Permission Check
                int NumberOfEmployees = 0, permitedEmployee = 0;
                sqlText = "select count(Id) NumberOfEmployees from EmployeeInfo";
                SqlCommand cmdExistingEmployees = new SqlCommand(sqlText, currConn, transaction);
                SqlDataReader dr;
                dr = cmdExistingEmployees.ExecuteReader();
                while (dr.Read())
                {
                    NumberOfEmployees = Convert.ToInt32(dr["NumberOfEmployees"]) + 1;
                }
                dr.Close();
                sqlText = "select NumberOfEmployees From Company";
                SqlCommand cmdPermitedEmployees = new SqlCommand(sqlText, currConn, transaction);
                dr = cmdPermitedEmployees.ExecuteReader();
                while (dr.Read())
                {
                    permitedEmployee = Convert.ToInt32(dr["NumberOfEmployees"]);
                }
                dr.Close();
                if (NumberOfEmployees > permitedEmployee)
                {
                    retResults[1] = "You have only " + permitedEmployee + " Employee Licence";
                    throw new ArgumentNullException("You have only " + permitedEmployee + " Employee Licence", "");
                }
                #endregion No. of Employee Permission Check


                if (AutoCode == "Y")
                {
                    #region Generate EmployeeCode
                    var databaseName = License.DataBaseName(currConn.Database);
                    var databaseNameSecond = databaseName.Split('~')[1].ToString();
                    var employeecode = _cDal.NextCode("EmployeeInfo", currConn, transaction);
                    currConn.ChangeDatabase(databaseNameSecond);
                    var employeecodeSecond = _cDal.NextCode("EmployeeInfo", currConn, transaction);
                    databaseName = databaseName.Split('~')[0].ToString();
                    currConn.ChangeDatabase(databaseName);
                    if (employeecode < employeecodeSecond)
                    {
                        vm.Code = employeecodeSecond.ToString();
                    }
                    else
                    {
                        vm.Code = employeecode.ToString();
                    }
                }
                    #endregion Generate EmployeeCode
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeInfo(
                                 Id
                                ,BranchId
                                ,Code
                                ,Salutation_E
                                ,MiddleName
                                ,LastName
                                ,Remarks
                                ,IsActive
                                ,IsArchive
                                ,PhotoName
                                ) 
                                   VALUES (
                                @Id
                                ,@BranchId
                                ,@Code
                                ,@Salutation_E
                                ,@MiddleName
                                ,@LastName
                                ,@Remarks
                                ,@IsActive
                                ,@IsArchive
                                ,@PhotoName
                                ) 
                                        ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdInsert.Parameters.AddWithValue("@Salutation_E", vm.Salutation_E);
                    cmdInsert.Parameters.AddWithValue("@MiddleName", vm.MiddleName);
                    cmdInsert.Parameters.AddWithValue("@LastName", vm.LastName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@PhotoName", "0.jpg");
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This EmployeeInfo already used";
                    throw new ArgumentNullException("Please Input EmployeeInfo Value", "");
                }
                

                #region User Create
                string AutoUser = _settingDal.settingValue("AutoUser", "Employee", currConn, transaction);
                string AutoPassword = _settingDal.settingValue("AutoPassword", "Employee", currConn, transaction);

                UserInformationDAL _userInfoDal = new UserInformationDAL();
                UserLogsVM userLogsVM = new UserLogsVM();
                if (AutoUser == "Y")
                {
                    EmployeeInfoDAL _empInfoDal = new EmployeeInfoDAL();
                    EmployeeInfoVM empInfoVM = new EmployeeInfoVM();
                    empInfoVM = _empInfoDal.SelectById(vm.Id, currConn, transaction);

                    userLogsVM.LogID = empInfoVM.Code;
                    userLogsVM.EmployeeCode = empInfoVM.Code;
                    userLogsVM.FullName = empInfoVM.EmpName;
                    userLogsVM.CreatedAt = empInfoVM.CreatedAt;
                    userLogsVM.CreatedBy = empInfoVM.CreatedBy;
                    userLogsVM.CreatedFrom = empInfoVM.CreatedFrom;
                    userLogsVM.BranchId = empInfoVM.BranchId;
                    userLogsVM.EmployeeId = empInfoVM.Id;
                    userLogsVM.GroupId = 6; //ESS

                    userLogsVM.Password = Ordinary.Encrypt(AutoPassword, true);
                    retResults = _userInfoDal.Insert(userLogsVM, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        retResults[1] = "User Not Created";
                        throw new ArgumentNullException(retResults[1], "");
                    }

                }


                #endregion User Create

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
                retResults[2] = vm.Id;
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
            #region Results
            return retResults;
            #endregion
        }



        public string[] InsertPersonalDetailXX(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initialize
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            //string sqlText = "";
            string[] retResults = { "Fail", "Fail", Id.ToString(), sqlText, "ex", "InsertPersonalDetail" };
            //0 - Success or Fail//1 - Success or Fail Message//2 - Return Id//3 - SQL Query //4 - catch ex//5 - Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion open connection and transaction
                #region Save
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string DateOfBirth = Ordinary.StringToDate(item["DateOfBirth"].ToString());
                        string ExpiryDate = Ordinary.StringToDate(item["ExpiryDate"].ToString());

                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        if (!string.IsNullOrWhiteSpace(DateOfBirth))
                        {
                            if (!Ordinary.IsDate(DateOfBirth))
                            {
                                retResults[1] = "Date Of Birth not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }

                        if (!Ordinary.IsNumeric(item["CorporateContactLimit"].ToString()))
                        {
                            retResults[1] = "Corporate Contact Limit not in Correct Format!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Data Format Validation Check
                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeePersonalDetail", "EmployeeId", EmployeeId, currConn, transaction);
                        if (returnDt != null && returnDt.Rows.Count > 0)
                        {
                            EmployeeId = returnDt.Rows[0]["EmployeeId"].ToString();
                        }
                        else
                        {
                            retResults[1] = "Employee Not Found for Employee " + EmployeeId;
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Finding EmployeeId Using EmployeeId
                        #region Duplicate Check
                        bool Exist = _cDal.ExistCheck("EmployeePersonalDetail", "EmployeeId", EmployeeId
                            , currConn, transaction);
                        if (Exist)
                        {


                            #region Update Settings

                            sqlText = "";
                            sqlText = "update EmployeePersonalDetail set";
                            sqlText += " EmployeeId=@EmployeeId,";
                            sqlText += " OtherId=@OtherId,";
                            sqlText += " Gender_E=@Gender_E,";
                            sqlText += " FatherName=@FatherName,";
                            sqlText += " MotherName=@MotherName,";
                            sqlText += " SpouseName=@SpouseName,";
                            sqlText += " PersonalContactNo=@PersonalContactNo,";
                            sqlText += " CorporateContactNo	=@CorporateContactNo,";
                            sqlText += " CorporateContactLimit=@CorporateContactLimit,";

                            sqlText += " MaritalStatus_E=@MaritalStatus_E,";
                            sqlText += " Nationality_E=@Nationality_E,";
                            sqlText += " DateOfBirth=@DateOfBirth,";
                            sqlText += " NickName=@NickName,";
                            sqlText += " Smoker=@Smoker,";
                            sqlText += " NID=@NID,";

                            sqlText += " PassportNumber=@PassportNumber,";
                            sqlText += " ExpiryDate=@ExpiryDate,";
                            sqlText += " Religion=@Religion,";
                            sqlText += " TIN=@TIN,";
                            sqlText += " IsDisable=@IsDisable,";
                            sqlText += " KindsOfDisability=@KindsOfDisability,";

                            sqlText += " BloodGroup_E=@BloodGroup_E,";

                            sqlText += " PlaceOfBirth=@PlaceOfBirth,";
                            sqlText += " MarriageDate=@MarriageDate,";
                            sqlText += " SpouseProfession=@SpouseProfession,";
                            sqlText += " SpouseDateOfBirth=@SpouseDateOfBirth,";
                            sqlText += " SpouseBloodGroup=@SpouseBloodGroup,";

                            sqlText += " HRMSCode                     =@HRMSCode,";
                            sqlText += " WDCode                       =@WDCode,";
                            sqlText += " TPNCode                      =@TPNCode,";
                            sqlText += " PersonalEmail                =@PersonalEmail,";
                            sqlText += " IsVaccineDose1Complete       =@IsVaccineDose1Complete,";
                            sqlText += " VaccineDose1Date             =@VaccineDose1Date,";
                            sqlText += " VaccineDose1Name             =@VaccineDose1Name,";
                            sqlText += " IsVaccineDose2Complete       =@IsVaccineDose2Complete,";
                            sqlText += " VaccineDose2Date             =@VaccineDose2Date,";
                            sqlText += " VaccineDose2Name             =@VaccineDose2Name,";
                            sqlText += " IsVaccineDose3Complete       =@IsVaccineDose3Complete,";
                            sqlText += " VaccineDose3Date             =@VaccineDose3Date,";
                            sqlText += " VaccineDose3Name             =@VaccineDose3Name,";

                            sqlText += " Heightft             =@Heightft,";
                            sqlText += " HeightIn             =@HeightIn,";
                            sqlText += " Weight             =@Weight,";
                            sqlText += " NoChildren             =@NoChildren,";
                            sqlText += " ChestIn             =@ChestIn,";
                            sqlText += " Email=@Email,";
                            sqlText += " IsActive=@IsActive";


                            //sqlText += " Remarks=@Remarks,";
                            //sqlText += " Email=@Email,";
                            //sqlText += " IsActive=@IsActive,";
                            //sqlText += " LastUpdateBy=@LastUpdateBy,";
                            //sqlText += " LastUpdateAt=@LastUpdateAt,";
                            //sqlText += " LastUpdateFrom=@LastUpdateFrom";
                            //sqlText += "where EmployeeId=@EmployeeId";

                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"].ToString().Trim());
                            cmdUpdate.Parameters.AddWithValue("@OtherId", item["OtherId"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Gender_E", item["Gender_E"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@FatherName", item["FatherName"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@MotherName", item["MotherName"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@SpouseName", item["SpouseName"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@PersonalContactNo", item["PersonalContactNo"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@CorporateContactNo", item["CorporateContactNo"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@CorporateContactLimit", item["CorporateContactLimit"]);
                            cmdUpdate.Parameters.AddWithValue("@MaritalStatus_E", item["MaritalStatus_E"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Nationality_E", item["Nationality_E"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@DateOfBirth", Ordinary.DateToString(item["DateOfBirth"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@NickName", item["NickName"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@BloodGroup_E", item["BloodGroup_E"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Smoker", item["Smoker"]);
                            cmdUpdate.Parameters.AddWithValue("@NID", item["NID"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@PassportNumber", item["PassportNumber"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@ExpiryDate", Ordinary.DateToString(item["ExpiryDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@Religion", item["Religion"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@TIN", item["TIN"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsDisable", item["IsDisable"]);
                            cmdUpdate.Parameters.AddWithValue("@KindsOfDisability", item["KindsOfDisability"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@PlaceOfBirth", item["PlaceOfBirth"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@MarriageDate", Ordinary.DateToString(item["MarriageDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@SpouseProfession", item["SpouseProfession"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@SpouseDateOfBirth", Ordinary.DateToString(item["SpouseDateOfBirth"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@SpouseBloodGroup", item["SpouseBloodGroup"] ?? Convert.DBNull);



                            cmdUpdate.Parameters.AddWithValue("@HRMSCode", item["HRMSCode"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@WDCode", item["WDCode"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@TPNCode", item["TPNCode"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@PersonalEmail", item["PersonalEmail"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose1Complete", item["IsVaccineDose1Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose1Date", item["VaccineDose1Date"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose1Name", item["VaccineDose1Name"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose2Complete", item["IsVaccineDose2Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose2Date", item["VaccineDose2Date"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose2Name", item["VaccineDose2Name"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose3Complete", item["IsVaccineDose3Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose3Date", item["VaccineDose3Date"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose3Name", item["VaccineDose3Name"] ?? Convert.DBNull);


                            cmdUpdate.Parameters.AddWithValue("@Heightft", item["Heightft"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@HeightIn", item["HeightIn"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Weight", item["Weight"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@NoChildren", item["NoChildren"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@ChestIn", item["ChestIn"] ?? Convert.DBNull);


                            //cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Email", item["Email"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                            //cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", item["LastUpdateBy"]);
                            //cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", item["LastUpdateAt"]);
                            //cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", item["LastUpdateFrom"]);

                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();

                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query

                            #region Commit

                            if (transResult <= 0)
                            {
                            }

                            #endregion Commit

                            #endregion Update Settings

                        }
                        else
                        {
                        #endregion Duplicate Check
                            #region Sql Satement
                            sqlText = "";
                            sqlText += @" INSERT INTO EmployeePersonalDetail(
EmployeeId,OtherId
,FatherName	
,MotherName	
,SpouseName	
,PersonalContactNo	
,CorporateContactNo	
,CorporateContactLimit
,Gender_E,MaritalStatus_E,Nationality_E,DateOfBirth,NickName,Smoker,NID,IsActive
,PassportNumber
,ExpiryDate
,Religion
,TIN
,Email
,IsDisable
,BloodGroup_E
,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@EmployeeId,@OtherId
,@FatherName	
,@MotherName	
,@SpouseName	
,@PersonalContactNo	
,@CorporateContactNo	
,@CorporateContactLimit
,@Gender_E,@MaritalStatus_E,@Nationality_E,@DateOfBirth,@NickName,@Smoker,@NID,@IsActive
,@PassportNumber
,@ExpiryDate
,@Religion
,@TIN
,@Email
,@IsDisable
,@BloodGroup_E
,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@FatherName", item["FatherName"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@MotherName", item["MotherName"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@SpouseName", item["SpouseName"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@PersonalContactNo", item["PersonalContactNo"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@CorporateContactNo", item["CorporateContactNo"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@CorporateContactLimit", item["CorporateContactLimit"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@OtherId", item["OtherId"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Gender_E", item["Gender"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@MaritalStatus_E", item["MaritalStatus"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Nationality_E", item["Nationality"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@DateOfBirth", Ordinary.DateToString(item["DateOfBirth"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@NickName", item["NickName"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Smoker", Ordinary.ConvertToBool(item["Smoker"].ToString()));
                            cmdInsert.Parameters.AddWithValue("@NID", item["NID"].ToString() ?? Convert.DBNull);
                            //cmdInsert.Parameters.AddWithValue("@NIDFile", "");
                            cmdInsert.Parameters.AddWithValue("@PassportNumber", item["PassportNumber"].ToString() ?? Convert.DBNull);
                            //cmdInsert.Parameters.AddWithValue("@PassportFile", "");
                            cmdInsert.Parameters.AddWithValue("@ExpiryDate", Ordinary.DateToString(item["ExpiryDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Religion", item["Religion"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@BloodGroup_E", item["BloodGroup"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@TIN", item["TIN"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Email", item["Email"].ToString() ?? Convert.DBNull);
                            //cmdInsert.Parameters.AddWithValue("@IsDisable", Ordinary.ConvertToBool(item["IsDisable"].ToString()));
                            cmdInsert.Parameters.AddWithValue("@IsDisable", false);

                            cmdInsert.Parameters.AddWithValue("@KindsOfDisability", item["KindsOfDisability"].ToString() ?? Convert.DBNull);
                            if (!Ordinary.ConvertToBool(item["IsDisable"].ToString()))
                            {
                                //item["DisabilityFile = "";
                            }
                            //cmdInsert.Parameters.AddWithValue("@DisabilityFile", "");
                            //cmdInsert.Parameters.AddWithValue("@Signature", item["Signature"].ToString() ?? Convert.DBNull);
                            //cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"].ToString() ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsert.Parameters.AddWithValue("@CreatedBy", "Admin");
                            cmdInsert.Parameters.AddWithValue("@CreatedAt", "19000101");
                            cmdInsert.Parameters.AddWithValue("@CreatedFrom", "local");

                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                retResults[1] = "Data Save Successfully!";
                retResults[2] = Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                try { transaction.Rollback(); }
                catch (Exception) { return retResults; }
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


        public string[] InsertPersonalDetail(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertPersonalDetail"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string DateOfBirth = Ordinary.StringToDate(item["DateOfBirth"].ToString());
                        string ExpiryDate = Ordinary.StringToDate(item["ExpiryDate"].ToString());
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        if (!string.IsNullOrWhiteSpace(DateOfBirth))
                        {
                            if (!Ordinary.IsDate(DateOfBirth))
                            {
                                retResults[1] = "Date Of Birth not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }

                        if (!Ordinary.IsNumeric(item["CorporateContactLimit"].ToString()))
                        {
                            retResults[1] = "Corporate Contact Limit not in Correct Format!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeePersonalDetail", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText = "update EmployeePersonalDetail set";
                            //sqlText += " EmployeeId=@EmployeeId,";
                            sqlText += " OtherId=@OtherId,";
                            sqlText += " Gender_E=@Gender_E,";
                            sqlText += " FatherName=@FatherName,";
                            sqlText += " MotherName=@MotherName,";
                            sqlText += " SpouseName=@SpouseName,";
                            sqlText += " PersonalContactNo=@PersonalContactNo,";
                            sqlText += " CorporateContactNo	=@CorporateContactNo,";
                            sqlText += " CorporateContactLimit=@CorporateContactLimit,";
                            sqlText += " MaritalStatus_E=@MaritalStatus_E,";
                            sqlText += " Nationality_E=@Nationality_E,";
                            sqlText += " DateOfBirth=@DateOfBirth,";
                            sqlText += " NickName=@NickName,";
                            sqlText += " Smoker=@Smoker,";
                            sqlText += " NID=@NID,";
                            sqlText += " PassportNumber=@PassportNumber,";
                            sqlText += " ExpiryDate=@ExpiryDate,";
                            sqlText += " Religion=@Religion,";
                            sqlText += " TIN=@TIN,";
                            sqlText += " IsDisable=@IsDisable,";
                            sqlText += " KindsOfDisability=@KindsOfDisability,";
                            sqlText += " BloodGroup_E=@BloodGroup_E,";
                            sqlText += " PlaceOfBirth=@PlaceOfBirth,";
                            sqlText += " MarriageDate=@MarriageDate,";
                            sqlText += " SpouseProfession=@SpouseProfession,";
                            sqlText += " SpouseDateOfBirth=@SpouseDateOfBirth,";
                            sqlText += " SpouseBloodGroup=@SpouseBloodGroup,";
                            sqlText += " HRMSCode                     =@HRMSCode,";
                            sqlText += " WDCode                       =@WDCode,";
                            sqlText += " TPNCode                      =@TPNCode,";
                            sqlText += " PersonalEmail                =@PersonalEmail,";
                            sqlText += " IsVaccineDose1Complete       =@IsVaccineDose1Complete,";
                            sqlText += " VaccineDose1Date             =@VaccineDose1Date,";
                            sqlText += " VaccineDose1Name             =@VaccineDose1Name,";
                            sqlText += " IsVaccineDose2Complete       =@IsVaccineDose2Complete,";
                            sqlText += " VaccineDose2Date             =@VaccineDose2Date,";
                            sqlText += " VaccineDose2Name             =@VaccineDose2Name,";
                            sqlText += " IsVaccineDose3Complete       =@IsVaccineDose3Complete,";
                            sqlText += " VaccineDose3Date             =@VaccineDose3Date,";
                            sqlText += " VaccineDose3Name             =@VaccineDose3Name,";
                            sqlText += " Heightft             =@Heightft,";
                            sqlText += " HeightIn             =@HeightIn,";
                            sqlText += " Weight             =@Weight,";
                            sqlText += " NoChildren             =@NoChildren,";
                            sqlText += " ChestIn             =@ChestIn,";
                            sqlText += " Email=@Email,";
                            sqlText += " IsActive=@IsActive";
                            //sqlText += " Remarks=@Remarks,";
                            //sqlText += " Email=@Email,";
                            //sqlText += " IsActive=@IsActive,";
                            //sqlText += " LastUpdateBy=@LastUpdateBy,";
                            //sqlText += " LastUpdateAt=@LastUpdateAt,";
                            //sqlText += " LastUpdateFrom=@LastUpdateFrom";
                            sqlText += " where EmployeeId=@EmployeeId";

                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"].ToString().Trim());
                            cmdUpdate.Parameters.AddWithValue("@OtherId", item["OtherId"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Gender_E", item["Gender_E"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@FatherName", item["FatherName"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@MotherName", item["MotherName"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@SpouseName", item["SpouseName"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@PersonalContactNo", item["PersonalContactNo"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@CorporateContactNo", item["CorporateContactNo"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@CorporateContactLimit", item["CorporateContactLimit"]);
                            cmdUpdate.Parameters.AddWithValue("@MaritalStatus_E", item["MaritalStatus_E"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Nationality_E", item["Nationality_E"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@DateOfBirth", Ordinary.DateToString(item["DateOfBirth"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@NickName", item["NickName"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@BloodGroup_E", item["BloodGroup_E"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Smoker", item["Smoker"]);
                            cmdUpdate.Parameters.AddWithValue("@NID", item["NID"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@PassportNumber", item["PassportNumber"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@ExpiryDate", Ordinary.DateToString(item["ExpiryDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@Religion", item["Religion"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@TIN", item["TIN"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsDisable", item["IsDisable"]);
                            cmdUpdate.Parameters.AddWithValue("@KindsOfDisability", item["KindsOfDisability"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@PlaceOfBirth", item["PlaceOfBirth"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@MarriageDate", Ordinary.DateToString(item["MarriageDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@SpouseProfession", item["SpouseProfession"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@SpouseDateOfBirth", Ordinary.DateToString(item["SpouseDateOfBirth"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@SpouseBloodGroup", item["SpouseBloodGroup"] ?? Convert.DBNull);



                            cmdUpdate.Parameters.AddWithValue("@HRMSCode", item["HRMSCode"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@WDCode", item["WDCode"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@TPNCode", item["TPNCode"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@PersonalEmail", item["PersonalEmail"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose1Complete", item["IsVaccineDose1Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose1Date", item["VaccineDose1Date"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose1Name", item["VaccineDose1Name"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose2Complete", item["IsVaccineDose2Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose2Date", item["VaccineDose2Date"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose2Name", item["VaccineDose2Name"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose3Complete", item["IsVaccineDose3Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose3Date", item["VaccineDose3Date"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose3Name", item["VaccineDose3Name"] ?? Convert.DBNull);


                            cmdUpdate.Parameters.AddWithValue("@Heightft", item["Heightft"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@HeightIn", item["HeightIn"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Weight", item["Weight"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@NoChildren", item["NoChildren"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@ChestIn", item["ChestIn"] ?? Convert.DBNull);


                            //cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Email", item["Email"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                            //cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", identity.Name);
                            //cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", item["LastUpdateAt"]);
                            //cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", item["identity.WorkStationIP"]);

                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();

                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query



                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = "";
                            sqlText += @" INSERT INTO EmployeePersonalDetail(
EmployeeId,OtherId
,FatherName	
,MotherName	
,SpouseName	
,PersonalContactNo	
,CorporateContactNo	
,CorporateContactLimit
,Gender_E,MaritalStatus_E,Nationality_E,DateOfBirth,NickName,Smoker,NID,IsActive
,PassportNumber
,ExpiryDate
,Religion
,TIN
,Email
,IsDisable
,BloodGroup_E
,IsArchive
) VALUES (
@EmployeeId,@OtherId
,@FatherName	
,@MotherName	
,@SpouseName	
,@PersonalContactNo	
,@CorporateContactNo	
,@CorporateContactLimit
,@Gender_E,@MaritalStatus_E,@Nationality_E,@DateOfBirth,@NickName,@Smoker,@NID,@IsActive
,@PassportNumber
,@ExpiryDate
,@Religion
,@TIN
,@Email
,@IsDisable
,@BloodGroup_E
,@IsArchive
) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@FatherName", item["FatherName"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@MotherName", item["MotherName"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@SpouseName", item["SpouseName"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@PersonalContactNo", item["PersonalContactNo"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@CorporateContactNo", item["CorporateContactNo"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@CorporateContactLimit", item["CorporateContactLimit"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@OtherId", item["OtherId"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Gender_E", item["Gender"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@MaritalStatus_E", item["MaritalStatus"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Nationality_E", item["Nationality"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@DateOfBirth", Ordinary.DateToString(item["DateOfBirth"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@NickName", item["NickName"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Smoker", Ordinary.ConvertToBool(item["Smoker"].ToString()));
                            cmdInsert.Parameters.AddWithValue("@NID", item["NID"].ToString() ?? Convert.DBNull);
                            //cmdInsert.Parameters.AddWithValue("@NIDFile", "");
                            cmdInsert.Parameters.AddWithValue("@PassportNumber", item["PassportNumber"].ToString() ?? Convert.DBNull);
                            //cmdInsert.Parameters.AddWithValue("@PassportFile", "");
                            cmdInsert.Parameters.AddWithValue("@ExpiryDate", Ordinary.DateToString(item["ExpiryDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Religion", item["Religion"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@BloodGroup_E", item["BloodGroup"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@TIN", item["TIN"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Email", item["Email"].ToString() ?? Convert.DBNull);
                            //cmdInsert.Parameters.AddWithValue("@IsDisable", Ordinary.ConvertToBool(item["IsDisable"].ToString()));
                            cmdInsert.Parameters.AddWithValue("@IsDisable", false);

                            cmdInsert.Parameters.AddWithValue("@KindsOfDisability", item["KindsOfDisability"].ToString() ?? Convert.DBNull);
                            if (!Ordinary.ConvertToBool(item["IsDisable"].ToString()))
                            {
                                //item["DisabilityFile = "";
                            }
                            //cmdInsert.Parameters.AddWithValue("@DisabilityFile", "");
                            //cmdInsert.Parameters.AddWithValue("@Signature", item["Signature"].ToString() ?? Convert.DBNull);
                            //cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"].ToString() ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);


                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeePersonalDetail.";
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

        public string[] InsertEmployeeProfessionalDegree(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeProfessionalDegree"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {



                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeeProfessionalDegree", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText = "update EmployeeProfessionalDegree set";
                            //sqlText += " EmployeeId=@EmployeeId,";

                            sqlText += " Degree_E=@Degree_E";
                            sqlText += ",Institute=@Institute";
                            sqlText += ",Level=@Level";
                            sqlText += " ,TotalYear=@TotalYear";
                            sqlText += " ,YearOfPassing=@YearOfPassing";

                            sqlText += " ,Marks=@Marks";
                            sqlText += " ,IsActive=@IsActive";
                            sqlText += " ,Remarks=@Remarks,";
                            //sqlText += " Email=@Email,";
                            sqlText += " where EmployeeId=@EmployeeId";
                            sqlText += " AND Id=@Id";
                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"].ToString().Trim());
                            cmdUpdate.Parameters.AddWithValue("@Id", returnDt.Rows[0]["Id"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@Degree_E", item["Degree_E"]);
                            cmdUpdate.Parameters.AddWithValue("@Institute", item["Institute"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Level", item["Level"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@TotalYear", item["TotalYear"]);
                            cmdUpdate.Parameters.AddWithValue("@YearOfPassing", item["YearOfPassing"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Marks", item["Marks"]);
                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);


                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();

                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query



                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement professional Degree
                            sqlText = "  ";
                            sqlText += @" 
INSERT INTO EmployeeEducation(
EmployeeId
,Degree_E
,Institute
,Major
,TotalYear
,YearOfPassing
,IsLast

,CGPA
,Scale
,Result
,Marks
,Remarks
,IsActive
,IsArchive

) VALUES (
 @EmployeeId
,@Degree_E
,@Institute
,@Major
,@TotalYear
,@YearOfPassing
,@IsLast

,@CGPA
,@Scale
,@Result
,@Marks
,@Remarks
,@IsActive
,@IsArchive

) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@Degree_E", item["Degree"].ToString());
                            cmdInsert.Parameters.AddWithValue("@Institute", item["Institute"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Major", item["Major"].ToString() ?? Convert.DBNull);
                            if (!string.IsNullOrWhiteSpace(item["TotalYear"].ToString()))
                            {
                                cmdInsert.Parameters.AddWithValue("@TotalYear", item["TotalYear"].ToString());
                            }
                            else
                            {
                                cmdInsert.Parameters.AddWithValue("@TotalYear", "0");
                            }
                            cmdInsert.Parameters.AddWithValue("@YearOfPassing", item["YearOfPassing"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsLast", Ordinary.ConvertToBool(item["IsLast"].ToString()));
                            cmdInsert.Parameters.AddWithValue("@FileName", "");
                            if (!string.IsNullOrWhiteSpace(item["CGPA"].ToString()))
                            {
                                cmdInsert.Parameters.AddWithValue("@CGPA", item["CGPA"].ToString());
                            }
                            else
                            {
                                cmdInsert.Parameters.AddWithValue("@CGPA", "0");
                            }
                            if (!string.IsNullOrWhiteSpace(item["Scale"].ToString()))
                            {
                                cmdInsert.Parameters.AddWithValue("@Scale", item["Scale"].ToString());
                            }
                            else
                            {
                                cmdInsert.Parameters.AddWithValue("@Scale", "0");
                            }

                            cmdInsert.Parameters.AddWithValue("@Result", item["Result"].ToString());

                            if (!string.IsNullOrWhiteSpace(item["Marks"].ToString()))
                            {
                                cmdInsert.Parameters.AddWithValue("@Marks", item["Marks"].ToString());
                            }
                            else
                            {
                                cmdInsert.Parameters.AddWithValue("@Marks", "0");
                            }

                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);


                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution

                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeEducation.";
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
        public string[] InsertEmployeeImmigration(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeImmigration"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string IssueDate = Ordinary.StringToDate(item["IssueDate"].ToString());
                        string ExpireDate = Ordinary.StringToDate(item["ExpireDate"].ToString());
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        if (!string.IsNullOrWhiteSpace(IssueDate))
                        {
                            if (!Ordinary.IsDate(IssueDate))
                            {
                                retResults[1] = "IssueDate  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(ExpireDate))
                        {
                            if (!Ordinary.IsDate(ExpireDate))
                            {
                                retResults[1] = "ExpireDate Of Birth not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        //if (!Ordinary.IsNumeric(item["CorporateContactLimit"].ToString()))
                        //{
                        //    retResults[1] = "Corporate Contact Limit not in Correct Format!";
                        //    throw new ArgumentNullException(retResults[1], "");
                        //}
                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeeImmigration", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText = "update EmployeeImmigration set";
                            //sqlText += " EmployeeId=@EmployeeId,";
                            sqlText += " EmployeeId=@EmployeeId,";
                            sqlText += " ImmigrationType_E=@ImmigrationType_E,";
                            sqlText += " ImmigrationNumber=@ImmigrationNumber,";
                            sqlText += " IssueDate=@IssueDate,";
                            sqlText += " ExpireDate=@ExpireDate,";
                            sqlText += " IssuedBy_E=@IssuedBy_E,";
                            sqlText += " EligibleReviewDate=@EligibleReviewDate,";
                            sqlText += " Remarks=@Remarks,";
                            //       sqlText += " IsActive=@IsActive,";

                            sqlText += " where EmployeeId=@EmployeeId";
                            sqlText += " AND Id=@Id";
                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"].ToString().Trim());
                            cmdUpdate.Parameters.AddWithValue("@Id", returnDt.Rows[0]["Id"].ToString());
                            //cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                            cmdUpdate.Parameters.AddWithValue("@ImmigrationType_E", item["ImmigrationType_E"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@ImmigrationNumber", item["ImmigrationNumber"]);
                            cmdUpdate.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(item["IssueDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@ExpireDate", Ordinary.DateToString(item["ExpireDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@IssuedBy_E", item["IssuedBy_E"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@EligibleReviewDate", item["EligibleReviewDate"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsArchive", false);

                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query

                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = "  ";
                            sqlText += @" INSERT INTO EmployeeImmigration(	
EmployeeId,ImmigrationType_E,ImmigrationNumber,IssueDate,ExpireDate,IssuedBy_E,EligibleReviewDate
,Remarks,IsActive,IsArchive
) VALUES (
@EmployeeId,@ImmigrationType_E,@ImmigrationNumber,@IssueDate,@ExpireDate,@IssuedBy_E,@EligibleReviewDate
,@Remarks,@IsActive,@IsArchive)
SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@ImmigrationType_E", item["ImmigrationType"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ImmigrationNumber", item["ImmigrationNumber"].ToString());
                            cmdInsert.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(item["IssueDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ExpireDate", Ordinary.DateToString(item["ExpireDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IssuedBy_E", item["IssuedBy"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@FileName", "");
                            cmdInsert.Parameters.AddWithValue("@EligibleReviewDate", Ordinary.DateToString(item["EligibleReviewDate"].ToString()) ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            //cmdInsert.Parameters.AddWithValue("@CreatedBy", "Admin");
                            //cmdInsert.Parameters.AddWithValue("@CreatedAt", "19000101");
                            //cmdInsert.Parameters.AddWithValue("@CreatedFrom", "local");

                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeImmigration.";
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

        public string[] InsertEmployeeExtraCurriculumActivities(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertBranch"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();

                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        //if (!string.IsNullOrWhiteSpace(YearOfPassing))
                        //{
                        //    if (!Ordinary.IsString(YearOfPassing))
                        //    {
                        //        retResults[1] = "Year Of Passing  not in Correct Format!";
                        //        throw new ArgumentNullException(retResults[1], "");
                        //    }
                        //}
                        //if (!string.IsNullOrWhiteSpace(TotalYear))
                        //{
                        //    if (!Ordinary.IsString(TotalYear))
                        //    {
                        //        retResults[1] = "Total Year  not in Correct Format!";
                        //        throw new ArgumentNullException(retResults[1], "");
                        //    }
                        //}

                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeeEducation", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText = "update Branch set";
                            //sqlText += " EmployeeId=@EmployeeId";
                            sqlText += " Degree_E=@Degree_E";
                            sqlText += ", Institute=@Institute";
                            sqlText += ", Major=@Major";
                            sqlText += ", TotalYear=@TotalYear";
                            sqlText += ", CGPA=@CGPA";
                            sqlText += ", YearOfPassing=@YearOfPassing";
                            sqlText += ", Scale=@Scale";
                            sqlText += ", Result=@Result";
                            sqlText += ", Marks=@Marks";
                            sqlText += ", IsLast=@IsLast";
                            sqlText += ", Remarks=@Remarks";
                            sqlText += ", IsActive=@IsActive";
                            sqlText += " where EmployeeId=@EmployeeId";
                            sqlText += " And Id=@Id";

                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                            cmdUpdate.Parameters.AddWithValue("@Id", returnDt.Rows[0]["Id"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                            cmdUpdate.Parameters.AddWithValue("@Degree_E", item["Degree_E"]);
                            cmdUpdate.Parameters.AddWithValue("@Institute", item["Institute"]);
                            cmdUpdate.Parameters.AddWithValue("@Major", item["Major"]);
                            cmdUpdate.Parameters.AddWithValue("@TotalYear", item["TotalYear"]);
                            cmdUpdate.Parameters.AddWithValue("@CGPA", item["CGPA"]);
                            cmdUpdate.Parameters.AddWithValue("@YearOfPassing", item["YearOfPassing"] ?? Convert.DBNull);
                            //cmdUpdate.Parameters.AddWithValue("@YearOfPassing", Ordinary.DateToString(item["YearOfPassing"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@Scale", item["Scale"]);
                            cmdUpdate.Parameters.AddWithValue("@Result", item["Result"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Marks", item["Marks"]);
                            cmdUpdate.Parameters.AddWithValue("@IsLast", item["IsLast"]);
                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);


                            //cmdUpdate.Transaction = transaction;
                            //transResult = (int)cmdUpdate.ExecuteNonQuery();

                            //retResults[2] = transResult.ToString();// Return Id
                            //retResults[3] = sqlText; //  SQL Query

                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();

                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query

                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = "";
                            sqlText += @" INSERT INTO EmployeeEducation(
EmployeeId
,Degree_E
,Institute
,Major
,TotalYear
,YearOfPassing
,IsLast
,Remarks
,IsActive
,IsArchive
,CGPA
,Scale
,Result
,Marks
) VALUES (
 @EmployeeId
,@Degree_E
,@Institute
,@Major
,@TotalYear
,@YearOfPassing
,@IsLast
,@Remarks
,@IsActive
,@IsArchive
,@CGPA
,@Scale
,@Result
,@Marks
) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@Degree_E", item["Degree_E"]);
                            cmdInsert.Parameters.AddWithValue("@Institute", item["Institute"]);
                            cmdInsert.Parameters.AddWithValue("@Major", item["Major"]);
                            cmdInsert.Parameters.AddWithValue("@TotalYear", item["TotalYear"]);
                            cmdInsert.Parameters.AddWithValue("@YearOfPassing", item["YearOfPassing"]);
                            cmdInsert.Parameters.AddWithValue("@IsLast", item["IsLast"]);
                            //cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"]);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                            cmdInsert.Parameters.AddWithValue("@CGPA", item["CGPA"]);
                            cmdInsert.Parameters.AddWithValue("@Scale", item["Scale"]);
                            cmdInsert.Parameters.AddWithValue("@Result", item["Result"]);
                            cmdInsert.Parameters.AddWithValue("@Marks", item["Marks"]);

                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeePersonalDetail.";
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


        public string[] InsertEmployeeTraining(DataTable dt, EmployeeInfoVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeTraining"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["EmployeeId"].ToString() != "")
                {
                    foreach (DataRow item in dt.Rows)
                    {

                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(Code))
                        {
                            string DateFrom = Ordinary.StringToDate(item["DateFrom"].ToString());
                            string DateTo = Ordinary.StringToDate(item["DateTo"].ToString());
                            if (string.IsNullOrWhiteSpace(Code))
                            {
                                retResults[1] = "Employee Id Can't Be Blank!";
                                throw new ArgumentNullException(retResults[1], "");
                            }

                        #endregion Required Field
                            #region Data Format Validation Check
                            if (string.IsNullOrWhiteSpace(DateFrom))
                            {
                                if (!Ordinary.IsDate(DateFrom))
                                {
                                    retResults[1] = "DateFrom  not in Correct Format!";
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(DateTo))
                            {
                                if (!Ordinary.IsDate(DateTo))
                                {
                                    retResults[1] = "DateTo Of  not in Correct Format!";
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }
                            //if (!Ordinary.IsNumeric(item["CorporateContactLimit"].ToString()))
                            //{
                            //    retResults[1] = "Corporate Contact Limit not in Correct Format!";
                            //    throw new ArgumentNullException(retResults[1], "");
                            //}
                            #endregion Data Format Validation Check


                            #region Finding EmployeeId Using EmployeeId
                            string EmployeeId = item["EmployeeId"].ToString();
                            string Topics = item["Topics"].ToString();
                            DataTable returnDt = _cDal.SelectByMultiCondition("EmployeeTraining", new[] { "EmployeeId", "Topics" }, new[] { EmployeeId, Topics }, currConn, transaction);

                            #endregion Finding EmployeeId Using EmployeeId

                            bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                            if (Exist)
                            {
                                #region Update Settings

                                sqlText = "";
                                sqlText = "update EmployeeTraining set";
                                //sqlText += " EmployeeId=@EmployeeId,";

                                sqlText += " TrainingStatus_E=@TrainingStatus_E,";
                                sqlText += " TrainingPlace_E=@TrainingPlace_E,";
                                sqlText += " Topics=@Topics,";
                                sqlText += " InstituteName=@InstituteName,";
                                sqlText += " Location=@Location,";
                                sqlText += " FundedBy=@FundedBy,";
                                sqlText += " DurationMonth=@DurationMonth,";
                                sqlText += " DurationDay=@DurationDay,";
                                sqlText += " Achievement=@Achievement,";
                                sqlText += " AllowancesTotalTk=@AllowancesTotalTk,";

                                sqlText += " DateFrom=@DateFrom,";
                                sqlText += " DateTo=@DateTo,";
                                sqlText += " Remarks=@Remarks";
                                //sqlText += " IsActive=@IsActive,";

                                sqlText += " where EmployeeId=@EmployeeId";
                                sqlText += " AND Id=@Id";
                                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                                cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"].ToString().Trim());
                                //cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                                cmdUpdate.Parameters.AddWithValue("@Id", returnDt.Rows[0]["Id"].ToString());
                                cmdUpdate.Parameters.AddWithValue("@TrainingStatus_E", item["TrainingStatus_E"] ?? Convert.DBNull);
                                cmdUpdate.Parameters.AddWithValue("@TrainingPlace_E", item["TrainingPlace_E"] ?? Convert.DBNull);
                                cmdUpdate.Parameters.AddWithValue("@Topics", item["Topics"] ?? Convert.DBNull);
                                cmdUpdate.Parameters.AddWithValue("@InstituteName", item["InstituteName"] ?? Convert.DBNull);
                                cmdUpdate.Parameters.AddWithValue("@Location", item["Location"] ?? Convert.DBNull);
                                cmdUpdate.Parameters.AddWithValue("@FundedBy", item["FundedBy"] ?? Convert.DBNull);
                                cmdUpdate.Parameters.AddWithValue("@DurationMonth", item["DurationMonth"]);
                                cmdUpdate.Parameters.AddWithValue("@DurationDay", item["DurationDay"]);
                                cmdUpdate.Parameters.AddWithValue("@Achievement", item["Achievement"] ?? Convert.DBNull);
                                cmdUpdate.Parameters.AddWithValue("@AllowancesTotalTk", item["AllowancesTotalTk"]);

                                cmdUpdate.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(item["DateFrom"].ToString()));
                                cmdUpdate.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(item["DateTo"].ToString()));
                                cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                                //cmdUpdate.Parameters.AddWithValue("@IsActive", employeeTrainingVM.IsActive);

                                cmdUpdate.Transaction = transaction;
                                transResult = (int)cmdUpdate.ExecuteNonQuery();
                                retResults[2] = transResult.ToString();// Return Id
                                retResults[3] = sqlText; //  SQL Query



                                #endregion Update Settings

                            }
                            else
                            {
                                #region Sql Satement
                                sqlText = "  ";
                                sqlText += @" INSERT INTO EmployeeTraining(	EmployeeId,TrainingStatus_E
,TrainingPlace_E,Topics,InstituteName,Location,FundedBy,DurationMonth,DurationDay,Achievement,AllowancesTotalTk,DateFrom,DateTo,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@EmployeeId,@TrainingStatus_E
,@TrainingPlace_E,@Topics,@InstituteName,@Location,@FundedBy,@DurationMonth,@DurationDay,@Achievement,@AllowancesTotalTk,@DateFrom,@DateTo,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";
                                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                                cmdInsert.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                                cmdInsert.Parameters.AddWithValue("@TrainingStatus_E", item["TrainingStatus_E"] ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@TrainingPlace_E", item["TrainingPlace_E"] ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@Topics", item["Topics"] ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@InstituteName", item["InstituteName"] ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@Location", item["Location"] ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@FundedBy", item["FundedBy"] ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@DurationMonth", item["DurationMonth"]);
                                cmdInsert.Parameters.AddWithValue("@DurationDay", item["DurationDay"]);
                                cmdInsert.Parameters.AddWithValue("@Achievement", item["Achievement"] ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@AllowancesTotalTk", item["AllowancesTotalTk"]);
                                //cmd.Parameters.AddWithValue("@FileName", employeeTrainingVM.FileName ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(item["DateFrom"].ToString()));
                                cmdInsert.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(item["DateTo"].ToString()));
                                cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                                cmdInsert.Parameters.AddWithValue("@IsArchive", true);
                                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                                cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                                cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                                cmdInsert.ExecuteNonQuery();
                                #endregion Sql Execution
                            }
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeImmigration.";
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

        public string[] InsertEmployeeTravels(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeTravels"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string FromDate = Ordinary.StringToDate(item["FromDate"].ToString());
                        string ToDate = Ordinary.StringToDate(item["ToDate"].ToString());
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        if (!string.IsNullOrWhiteSpace(FromDate))
                        {
                            if (!Ordinary.IsDate(FromDate))
                            {
                                retResults[1] = "FromDate  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(ToDate))
                        {
                            if (!Ordinary.IsDate(ToDate))
                            {
                                retResults[1] = "ToDate Of  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        //if (!Ordinary.IsNumeric(item["CorporateContactLimit"].ToString()))
                        //{
                        //    retResults[1] = "Corporate Contact Limit not in Correct Format!";
                        //    throw new ArgumentNullException(retResults[1], "");
                        //}
                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeeTravel", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText = "update EmployeeTravel set";
                            //sqlText += " EmployeeId=@EmployeeId,";

                            sqlText += " TravelType_E=@TravelType_E,";

                            sqlText += " TravelFromAddress=@TravelFromAddress,";
                            sqlText += " TravelToAddress=@TravelToAddress,";
                            sqlText += " FromDate=@FromDate,";
                            sqlText += " ToDate=@ToDate,";
                            sqlText += " FromTime=@FromTime,";
                            sqlText += " ToTime=@ToTime,";

                            sqlText += " Allowances=@Allowances,";
                            sqlText += " IssueDate=@IssueDate,";
                            sqlText += " ExpiryDate=@ExpiryDate,";
                            sqlText += " Country=@Country,";
                            sqlText += " PassportNumber=@PassportNumber,";
                            sqlText += " EmbassyName=@EmbassyName,";

                            sqlText += " Remarks=@Remarks,";

                            sqlText += " where EmployeeId=@EmployeeId";

                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                            cmdUpdate.Parameters.AddWithValue("@TravelType_E", item["TravelType_E"]);

                            cmdUpdate.Parameters.AddWithValue("@Allowances", item["Allowances"]);
                            cmdUpdate.Parameters.AddWithValue("@TravelFromAddress", item["TravelFromAddress"]);
                            cmdUpdate.Parameters.AddWithValue("@TravelToAddress", item["TravelToAddress"]);
                            cmdUpdate.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(item["FromDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(item["ToDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@FromTime", Ordinary.TimeToString(item["FromTime"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@ToTime", Ordinary.TimeToString(item["ToTime"].ToString()));

                            cmdUpdate.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(item["IssueDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@ExpiryDate", Ordinary.DateToString(item["ExpiryDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@Country", item["Country"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@PassportNumber", item["PassportNumber"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@EmbassyName", item["EmbassyName"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            //cmdUpdate.Parameters.AddWithValue("@IsActive", employeeTrainingVM.IsActive);


                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query



                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = "  ";
                            sqlText += @" INSERT INTO EmployeeTravel(	
	EmployeeId,TravelType_E,TravelFromAddress,TravelToAddress,FromDate,ToDate,FromTime,ToTime,Allowances
,IssueDate,ExpiryDate,Country,PassportNumber,EmbassyName
,Remarks,IsActive,IsArchive
)  VALUES (
@EmployeeId,@TravelType_E,@TravelFromAddress,@TravelToAddress,@FromDate,@ToDate,@FromTime,@ToTime,@Allowances
,@IssueDate,@ExpiryDate,@Country,@PassportNumber,@EmbassyName
,@Remarks,@IsActive,@IsArchive
) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                            cmdInsert.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                            cmdInsert.Parameters.AddWithValue("@TravelType_E", item["TravelType_E"]);

                            cmdInsert.Parameters.AddWithValue("@TravelFromAddress", item["TravelFromAddress"]);
                            cmdInsert.Parameters.AddWithValue("@TravelToAddress", item["TravelToAddress"]);
                            cmdInsert.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(item["FromDate"].ToString()));
                            cmdInsert.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(item["ToDate"].ToString()));
                            cmdInsert.Parameters.AddWithValue("@FromTime", Ordinary.TimeToString(item["FromTime"].ToString()));//employeeTravelVM.FromTime ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ToTime", Ordinary.TimeToString(item["ToTime"].ToString()));//employeeTravelVM.ToTime ?? Convert.DBNull);
                            //cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Allowances", item["Allowances"]);
                            cmdInsert.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(item["IssueDate"].ToString()));
                            cmdInsert.Parameters.AddWithValue("@ExpiryDate", Ordinary.DateToString(item["ExpiryDate"].ToString()));
                            cmdInsert.Parameters.AddWithValue("@Country", item["Country"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@PassportNumber", item["PassportNumber"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@EmbassyName", item["EmbassyName"] ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);


                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeImmigration.";
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

        public string[] InsertEmployeeNominee(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeNominee"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string DateofBirth = Ordinary.StringToDate(item["DateofBirth"].ToString());
                        //string ToDate = Ordinary.StringToDate(item["ToDate"].ToString());
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        if (!string.IsNullOrWhiteSpace(DateofBirth))
                        {
                            if (!Ordinary.IsDate(DateofBirth))
                            {
                                retResults[1] = "FromDate  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }

                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeeNominee", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings
                            sqlText = "";
                            sqlText = "update EmployeeNominee set";
                            sqlText += " EmployeeId=@EmployeeId,";
                            sqlText += " Name=@Name,";
                            sqlText += " Relation=@Relation,";
                            sqlText += " Address=@Address,";
                            sqlText += " BirthCertificateNo=@BirthCertificateNo,";
                            sqlText += " District=@District,";
                            sqlText += "NID=@NID, ";
                            sqlText += " Division=@Division,";
                            sqlText += " Country=@Country,";
                            sqlText += " City=@City,";
                            sqlText += " PostalCode=@PostalCode,";
                            sqlText += " PostOffice=@PostOffice,";
                            sqlText += " Phone=@Phone,";
                            sqlText += " Mobile=@Mobile,";
                            sqlText += " Fax=@Fax,";

                            sqlText += " IsVaccineDose1Complete=@IsVaccineDose1Complete,";
                            sqlText += "VaccineDose1Date=@VaccineDose1Date, ";
                            sqlText += " VaccineDose1Name=@VaccineDose1Name,";

                            sqlText += " IsVaccineDose2Complete=@IsVaccineDose2Complete,";
                            sqlText += "VaccineDose2Date=@VaccineDose2Date, ";
                            sqlText += " VaccineDose2Name=@VaccineDose2Name,";

                            sqlText += " IsVaccineDose3Complete=@IsVaccineDose3Complete,";
                            sqlText += "VaccineDose3Date=@VaccineDose3Date, ";
                            sqlText += " VaccineDose3Name=@VaccineDose3Name,";

                            sqlText += " Remarks=@Remarks,";

                            sqlText += " where EmployeeId=@EmployeeId";
                            sqlText += " AND Id=@Id";
                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

                            cmdUpdate.Parameters.AddWithValue("@Id", returnDt.Rows[0]["Id"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                            cmdUpdate.Parameters.AddWithValue("@Name", item["Name"]);
                            cmdUpdate.Parameters.AddWithValue("@Relation", item["Relation"]);
                            cmdUpdate.Parameters.AddWithValue("@Address", item["Address"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@BirthCertificateNo", item["BirthCertificateNo"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@District", item["District"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("NID", item["NID"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Division", item["Division"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Country", item["Country"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@City", item["City"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@PostalCode", item["PostalCode"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@PostOffice", item["PostOffice"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Phone", item["Phone"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Mobile", item["Mobile"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Fax", item["Fax"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose1Complete", item["IsVaccineDose1Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose1Date", Ordinary.DateToString(item["VaccineDose1Date"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose1Name", item["VaccineDose1Name"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose2Complete", item["IsVaccineDose2Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose2Date", Ordinary.DateToString(item["VaccineDose2Date"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose2Name", item["VaccineDose2Name"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose3Complete", item["IsVaccineDose3Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose3Date", Ordinary.DateToString(item["VaccineDose3Date"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose3Name", item["VaccineDose3Name"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@DateofBirth", Ordinary.DateToString(item["DateofBirth"].ToString()));
                            //cmdUpdate.Parameters.AddWithValue("@IsActive", employeeTrainingVM.IsActive);


                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query



                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = "  ";
                            sqlText += @" INSERT INTO EmployeeNominee(
EmployeeId,Name,Relation,Address,District,Division,Country,City,PostalCode,Mobile,Phone,Fax,DateofBirth,BirthCertificateNo
,Remarks,IsActive,IsArchive
) VALUES (
@EmployeeId,@Name,@Relation,@Address,@District,@Division,@Country,@City,@PostalCode,@Mobile,@Phone,@Fax
,@DateofBirth,@BirthCertificateNo
,@Remarks,@IsActive,@IsArchive
) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@Name", item["Name"].ToString());
                            cmdInsert.Parameters.AddWithValue("@Relation", item["Relation"].ToString());
                            cmdInsert.Parameters.AddWithValue("@Address", item["Address"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@District", item["District"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Division", item["Division"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Country", item["Country"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@City", item["City"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@PostalCode", item["PostalCode"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Mobile", item["Mobile"].ToString() ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@Phone", item["Phone"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Fax", item["Fax"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@DateofBirth", Ordinary.DateToString(item["DateofBirth"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@BirthCertificateNo", item["BirthCertificateNo"].ToString() ?? Convert.DBNull);
                            //cmdInsert.Parameters.AddWithValue("@FileName", "");

                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);


                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeImmigration.";
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

        public string[] InsertEmployeeDependent(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeDependent"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string DateofBirth = Ordinary.StringToDate(item["DateofBirth"].ToString());
                        //string ToDate = Ordinary.StringToDate(item["ToDate"].ToString());
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        if (!string.IsNullOrWhiteSpace(DateofBirth))
                        {
                            if (!Ordinary.IsDate(DateofBirth))
                            {
                                retResults[1] = "FromDate  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        //if (!string.IsNullOrWhiteSpace(ToDate))
                        //{
                        //    if (!Ordinary.IsDate(ToDate))
                        //    {
                        //        retResults[1] = "ToDate Of  not in Correct Format!";
                        //        throw new ArgumentNullException(retResults[1], "");
                        //    }
                        //}
                        //if (!Ordinary.IsNumeric(item["CorporateContactLimit"].ToString()))
                        //{
                        //    retResults[1] = "Corporate Contact Limit not in Correct Format!";
                        //    throw new ArgumentNullException(retResults[1], "");
                        //}
                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeeDependent", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings
                            sqlText = "";
                            sqlText = "update EmployeeDependent set";
                            //sqlText += " EmployeeId=@EmployeeId,";
                            sqlText += " Name=@Name,";
                            sqlText += " Relation=@Relation,";

                            sqlText += " BirthCertificateNo=@BirthCertificateNo,";
                            sqlText += " NID=@NID,";
                            sqlText += " Address=@Address,";
                            sqlText += " District=@District,";
                            sqlText += " Division=@Division,";
                            sqlText += " Country=@Country,";
                            sqlText += " City=@City,";
                            sqlText += " PostalCode=@PostalCode,";
                            sqlText += " PostOffice=@PostOffice,";
                            sqlText += " Phone=@Phone,";
                            sqlText += " Mobile=@Mobile,";
                            sqlText += " Fax=@Fax,";

                            sqlText += " Gender=@Gender,";
                            sqlText += " EducationQualification=@EducationQualification,";
                            sqlText += " BloodGroup=@BloodGroup,";
                            sqlText += " Remarks=@Remarks,";
                            // sqlText += " IsActive=@IsActive,";

                            sqlText += " IsVaccineDose1Complete=@IsVaccineDose1Complete,";
                            sqlText += " VaccineDose1Date=@VaccineDose1Date,";
                            sqlText += " VaccineDose1Name=@VaccineDose1Name,";

                            sqlText += " IsVaccineDose2Complete=@IsVaccineDose2Complete,";
                            sqlText += " VaccineDose2Date=@VaccineDose2Date,";
                            sqlText += " VaccineDose2Name=@VaccineDose2Name,";

                            sqlText += " IsVaccineDose3Complete=@IsVaccineDose3Complete,";
                            sqlText += " VaccineDose3Date=@VaccineDose3Date,";
                            sqlText += " VaccineDose3Name=@VaccineDose3Name,";


                            sqlText += " IsDependentAllowance=@IsDependentAllowance";
                            sqlText += " where EmployeeId=@EmployeeId";
                            sqlText += " AND Id=@Id";
                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

                            cmdUpdate.Parameters.AddWithValue("@Id", returnDt.Rows[0]["Id"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                            cmdUpdate.Parameters.AddWithValue("@IsDependentAllowance", item["IsDependentAllowance"]);
                            cmdUpdate.Parameters.AddWithValue("@Name", item["Name"]);
                            cmdUpdate.Parameters.AddWithValue("@Relation", item["Relation"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@BirthCertificateNo", item["BirthCertificateNo"] ?? Convert.DBNull);
                            //cmdUpdate.Parameters.AddWithValue("@NID", vm.NID);
                            cmdUpdate.Parameters.AddWithValue("@NID", item["NID"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@DateofBirth", Ordinary.DateToString(item["DateofBirth"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@Address", item["Address"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@District", item["District"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Division", item["Division"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Country", item["Country"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@City", item["City"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@PostalCode", item["PostalCode"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@PostOffice", item["PostOffice"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Phone", item["Phone"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Mobile", item["Mobile"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Fax", item["Fax"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Gender", item["Gender"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@EducationQualification", item["EducationQualification"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@BloodGroup", item["BloodGroup"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose1Complete", item["IsVaccineDose1Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose1Date", Ordinary.DateToString(item["VaccineDose1Date"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose1Name", item["VaccineDose1Name"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose2Complete", item["IsVaccineDose2Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose2Date", Ordinary.DateToString(item["VaccineDose2Date"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose2Name", item["VaccineDose2Name"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@IsVaccineDose3Complete", item["IsVaccineDose3Complete"]);
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose3Date", Ordinary.DateToString(item["VaccineDose3Date"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@VaccineDose3Name", item["VaccineDose3Name"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);

                            //cmdUpdate.Parameters.AddWithValue("@IsActive", employeeTrainingVM.IsActive);

                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query



                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = "  ";
                            sqlText += @" INSERT INTO EmployeeDependent(
EmployeeId,Name,Relation,DateofBirth,BirthCertificateNo,Address,District,Division,Country,City,PostalCode,PostOffice
,Phone,Mobile,Fax,Remarks,IsActive,IsArchive) 
VALUES (
@EmployeeId,@Name,@Relation,@DateofBirth,@BirthCertificateNo,@Address,@District,@Division,@Country,@City,@PostalCode,@PostOffice
,@Phone,@Mobile,@Fax,@Remarks,@IsActive,@IsArchive) 
SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@Name", item["Name"].ToString());
                            cmdInsert.Parameters.AddWithValue("@Relation", item["Relation"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@DateofBirth", Ordinary.DateToString(item["DateofBirth"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Address", item["Address"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@BirthCertificateNo", item["BirthCertificateNo"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@District", item["District"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Division", item["Division"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Country", item["Country"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@City", item["City"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@PostalCode", item["PostalCode"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Phone", item["Phone"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Mobile", item["Mobile"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Fax", item["Fax"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@PostOffice", item["PostOffice"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"].ToString() ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);


                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeImmigration.";
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

        public string[] InsertEmployeeAssets(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeAssets"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string IssueDate = Ordinary.StringToDate(item["IssueDate"].ToString());
                        //string ToDate = Ordinary.StringToDate(item["ToDate"].ToString());
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        if (!string.IsNullOrWhiteSpace(IssueDate))
                        {
                            if (!Ordinary.IsDate(IssueDate))
                            {
                                retResults[1] = "IssueDate  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        //if (!string.IsNullOrWhiteSpace(ToDate))
                        //{
                        //    if (!Ordinary.IsDate(ToDate))
                        //    {
                        //        retResults[1] = "ToDate Of  not in Correct Format!";
                        //        throw new ArgumentNullException(retResults[1], "");
                        //    }
                        //}
                        //if (!Ordinary.IsNumeric(item["CorporateContactLimit"].ToString()))
                        //{
                        //    retResults[1] = "Corporate Contact Limit not in Correct Format!";
                        //    throw new ArgumentNullException(retResults[1], "");
                        //}
                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeeAssets", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings
                            sqlText = "";
                            sqlText = "UPDATE EmployeeAssets SET";
                            //sqlText += " EmployeeId=@EmployeeId";
                            sqlText += " ,AssetId=@AssetId";
                            sqlText += " ,IssueDate=@IssueDate";
                            sqlText += " ,Remarks=@Remarks";
                            sqlText += " ,IsActive=@IsActive";
                            sqlText += " WHERE EmployeeId=@EmployeeId";
                            sqlText += " AND Id=@Id";
                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                            cmdUpdate.Parameters.AddWithValue("@Id", returnDt.Rows[0]["Id"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                            cmdUpdate.Parameters.AddWithValue("@Id", returnDt.Rows[0]["Id"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@AssetId", item["AssetId"]);
                            cmdUpdate.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(item["IssueDate"].ToString()));

                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);

                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query



                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = " ";
                            sqlText += @" INSERT INTO EmployeeAssets(
EmployeeId,AssetId,IssueDate, Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
VALUES (@Id,@EmployeeId,1,@IssueDate,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
";
                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@AssetName", item["AssetName"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(item["IssueDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"].ToString() ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsert.Parameters.AddWithValue("@CreatedBy", "Admin");
                            cmdInsert.Parameters.AddWithValue("@CreatedAt", "19000101");
                            cmdInsert.Parameters.AddWithValue("@CreatedFrom", "local");

                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeImmigration.";
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

        public string[] InsertEmployeeLeftInformation(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeLeftInformation"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string IssueDate = Ordinary.StringToDate(item["IssueDate"].ToString());
                        //string ToDate = Ordinary.StringToDate(item["ToDate"].ToString());
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        if (!string.IsNullOrWhiteSpace(IssueDate))
                        {
                            if (!Ordinary.IsDate(IssueDate))
                            {
                                retResults[1] = "IssueDate  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        //if (!string.IsNullOrWhiteSpace(ToDate))
                        //{
                        //    if (!Ordinary.IsDate(ToDate))
                        //    {
                        //        retResults[1] = "ToDate Of  not in Correct Format!";
                        //        throw new ArgumentNullException(retResults[1], "");
                        //    }
                        //}
                        //if (!Ordinary.IsNumeric(item["CorporateContactLimit"].ToString()))
                        //{
                        //    retResults[1] = "Corporate Contact Limit not in Correct Format!";
                        //    throw new ArgumentNullException(retResults[1], "");
                        //}
                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeeLeftInformation", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings
                            sqlText = "";
                            sqlText = "UPDATE EmployeeAssets SET";
                            //sqlText += " EmployeeId=@EmployeeId";
                            sqlText += " ,AssetId=@AssetId";
                            sqlText += " ,IssueDate=@IssueDate";
                            sqlText += " ,Remarks=@Remarks";
                            sqlText += " ,IsActive=@IsActive";

                            sqlText += " WHERE EmployeeId=@EmployeeId";
                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                            cmdUpdate.Parameters.AddWithValue("@AssetId", item["AssetId"]);
                            cmdUpdate.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(item["IssueDate"].ToString()));

                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);


                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query



                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = " ";
                            sqlText += @" INSERT INTO EmployeeAssets(
EmployeeId,AssetId,IssueDate, Remarks,IsActive,IsArchive) 
VALUES (@Id,@EmployeeId,1,@IssueDate,@Remarks,@IsActive,@IsArchive) 
";
                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@AssetName", item["AssetName"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(item["IssueDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeImmigration.";
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

        //        public string[] EmployeeLeaveStructureDt(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        //        {
        //            #region Initializ
        //            string sqlText = "";
        //            int Id = 0;
        //            int transResult = 0;
        //            string[] retResults = new string[6];
        //            retResults[0] = "Fail";//Success or Fail
        //            retResults[1] = "Fail";// Success or Fail Message
        //            retResults[2] = Id.ToString();// Return Id
        //            retResults[3] = sqlText; //  SQL Query
        //            retResults[4] = "ex"; //catch ex
        //            retResults[5] = "InsertEmployeeLeftInformation"; //Method Name
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;
        //            //EmployeeInfoVM vm = new EmployeeInfoVM();
        //            CommonDAL _cDal = new CommonDAL();
        //            #endregion
        //            #region Try
        //            try
        //            {
        //                #region Validation

        //                #endregion Validation
        //                #region open connection and transaction
        //                #region New open connection and transaction
        //                if (VcurrConn != null)
        //                {
        //                    currConn = VcurrConn;
        //                }
        //                if (Vtransaction != null)
        //                {
        //                    transaction = Vtransaction;
        //                }
        //                #endregion New open connection and transaction
        //                if (currConn == null)
        //                {
        //                    currConn = _dbsqlConnection.GetConnection();
        //                    if (currConn.State != ConnectionState.Open)
        //                    {
        //                        currConn.Open();
        //                    }
        //                }
        //                if (transaction == null)
        //                {
        //                    transaction = currConn.BeginTransaction("");
        //                }
        //                #endregion open connection and transaction
        //                #region Save
        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    foreach (DataRow item in dt.Rows)
        //                    {
        //                        #region Required Field
        //                        string Code = item["EmployeeId"].ToString().Trim();
        //                        string IssueDate = Ordinary.StringToDate(item["IssueDate"].ToString());
        //                        //string ToDate = Ordinary.StringToDate(item["ToDate"].ToString());
        //                        if (string.IsNullOrWhiteSpace(Code))
        //                        {
        //                            retResults[1] = "Employee Id Can't Be Blank!";
        //                            throw new ArgumentNullException(retResults[1], "");
        //                        }

        //                        #endregion Required Field
        //                        #region Data Format Validation Check
        //                        if (!string.IsNullOrWhiteSpace(IssueDate))
        //                        {
        //                            if (!Ordinary.IsDate(IssueDate))
        //                            {
        //                                retResults[1] = "IssueDate  not in Correct Format!";
        //                                throw new ArgumentNullException(retResults[1], "");
        //                            }
        //                        }
        //                        //if (!string.IsNullOrWhiteSpace(ToDate))
        //                        //{
        //                        //    if (!Ordinary.IsDate(ToDate))
        //                        //    {
        //                        //        retResults[1] = "ToDate Of  not in Correct Format!";
        //                        //        throw new ArgumentNullException(retResults[1], "");
        //                        //    }
        //                        //}
        //                        //if (!Ordinary.IsNumeric(item["CorporateContactLimit"].ToString()))
        //                        //{
        //                        //    retResults[1] = "Corporate Contact Limit not in Correct Format!";
        //                        //    throw new ArgumentNullException(retResults[1], "");
        //                        //}
        //                        #endregion Data Format Validation Check


        //                        #region Finding EmployeeId Using EmployeeId
        //                        string EmployeeId = item["EmployeeId"].ToString();
        //                        DataTable returnDt = _cDal.SelectByCondition("EmployeeLeftInformation", "EmployeeId", EmployeeId, currConn, transaction);

        //                        #endregion Finding EmployeeId Using EmployeeId

        //                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
        //                        if (Exist)
        //                        {
        //                            #region Update Settings
        //                            sqlText = "";
        //                            sqlText = "UPDATE EmployeeAssets SET";
        //                            //sqlText += " EmployeeId=@EmployeeId";
        //                            sqlText += " ,AssetId=@AssetId";
        //                            sqlText += " ,IssueDate=@IssueDate";
        //                            sqlText += " ,Remarks=@Remarks";
        //                            sqlText += " ,IsActive=@IsActive";
        //                            sqlText += " ,LastUpdateBy=@LastUpdateBy";
        //                            sqlText += " ,LastUpdateAt=@LastUpdateAt";
        //                            sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
        //                            sqlText += " WHERE EmployeeId=@EmployeeId";
        //                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
        //                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
        //                            cmdUpdate.Parameters.AddWithValue("@AssetId", item["AssetId"]);
        //                            cmdUpdate.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(item["IssueDate"].ToString()));

        //                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
        //                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);
        //                            cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", item["LastUpdateBy"]);
        //                            cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", item["LastUpdateAt"]);
        //                            cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", item["LastUpdateFrom"]);

        //                            cmdUpdate.Transaction = transaction;
        //                            transResult = (int)cmdUpdate.ExecuteNonQuery();
        //                            retResults[2] = transResult.ToString();// Return Id
        //                            retResults[3] = sqlText; //  SQL Query



        //                            #endregion Update Settings

        //                        }
        //                        else
        //                        {
        //                            #region Sql Satement
        //                            sqlText = " ";
        //                            sqlText += @" INSERT INTO EmployeeAssets(
        //EmployeeId,AssetId,IssueDate, Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
        //VALUES (@Id,@EmployeeId,1,@IssueDate,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
        //";
        //                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
        //                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
        //                            cmdInsert.Parameters.AddWithValue("@AssetName", item["AssetName"].ToString() ?? Convert.DBNull);
        //                            cmdInsert.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(item["IssueDate"].ToString()) ?? Convert.DBNull);
        //                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"].ToString() ?? Convert.DBNull);

        //                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
        //                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
        //                            cmdInsert.Parameters.AddWithValue("@CreatedBy", "Admin");
        //                            cmdInsert.Parameters.AddWithValue("@CreatedAt", "19000101");
        //                            cmdInsert.Parameters.AddWithValue("@CreatedFrom", "local");

        //                            cmdInsert.ExecuteNonQuery();
        //                            #endregion Sql Execution
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    retResults[1] = "Unexpected Error!";
        //                    throw new ArgumentNullException("Unexpected Error!", "");
        //                }
        //                #endregion Save

        //                #region Commit
        //                if (Vtransaction == null)
        //                {
        //                    if (transaction != null)
        //                    {
        //                        transaction.Commit();
        //                    }
        //                }
        //                #endregion Commit
        //                #region SuccessResult
        //                retResults[0] = "Success";
        //                retResults[1] = "Data Save Successfully";
        //                #endregion SuccessResult
        //            }
        //            #endregion try
        //            #region Catch and Finall
        //            catch (Exception ex)
        //            {
        //                retResults[0] = "Fail";//Success or Fail
        //                retResults[4] = ex.Message.ToString(); //catch ex
        //                if (Vtransaction != null)
        //                {
        //                    try
        //                    {
        //                        if (Vtransaction == null) { transaction.Rollback(); }
        //                    }
        //                    catch (Exception)
        //                    {
        //                        retResults[1] = "Unexpected error to update EmployeeImmigration.";
        //                        return retResults;
        //                    }
        //                }
        //                return retResults;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null)
        //                {
        //                    if (currConn != null)
        //                    {
        //                        if (currConn.State == ConnectionState.Open)
        //                        {
        //                            currConn.Close();
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            #region Results
        //            return retResults;
        //            #endregion
        //        }


        public string[] InsertEmployeeEducation(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeEducation"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string YearOfPassing = Ordinary.StringToDate(item["YearOfPassing"].ToString());
                        string TotalYear = Ordinary.StringToDate(item["TotalYear"].ToString());
                        bool IsNumeric = Ordinary.IsNumeric(item["CGPA"].ToString());
                        //bool IsNumeric = Ordinary.IsNumeric(item["Scale"].ToString());
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        if (!string.IsNullOrWhiteSpace(YearOfPassing))
                        {
                            if (!Ordinary.IsString(YearOfPassing))
                            {
                                retResults[1] = "Year Of Passing  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(TotalYear))
                        {
                            if (!Ordinary.IsString(TotalYear))
                            {
                                retResults[1] = "Total Year  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        if (!Ordinary.IsNumeric(item["CGPA"].ToString()))
                        {
                            retResults[1] = "CGPA not in Correct Format!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        if (!Ordinary.IsNumeric(item["Scale"].ToString()))
                        {
                            retResults[1] = "Scale not in Correct Format!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        if (!Ordinary.IsBool(item["IsLast"].ToString()))
                        {
                            retResults[1] = "IsLast not in Correct Format!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeeEducation", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText = "update EmployeeEducation set";
                            //sqlText += " EmployeeId=@EmployeeId";
                            sqlText += " Degree_E=@Degree_E";
                            sqlText += ", Institute=@Institute";
                            sqlText += ", Major=@Major";
                            sqlText += ", TotalYear=@TotalYear";
                            sqlText += ", CGPA=@CGPA";
                            sqlText += ", YearOfPassing=@YearOfPassing";
                            sqlText += ", Scale=@Scale";
                            sqlText += ", Result=@Result";
                            sqlText += ", Marks=@Marks";
                            sqlText += ", IsLast=@IsLast";

                            sqlText += ", Remarks=@Remarks";


                            //sqlText += ", IsActive=@IsActive";
                            sqlText += " where EmployeeId=@EmployeeId";
                            sqlText += " And Id=@Id";

                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                            cmdUpdate.Parameters.AddWithValue("@Id", returnDt.Rows[0]["Id"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                            cmdUpdate.Parameters.AddWithValue("@Degree_E", item["Degree_E"]);
                            cmdUpdate.Parameters.AddWithValue("@Institute", item["Institute"]);
                            cmdUpdate.Parameters.AddWithValue("@Major", item["Major"]);
                            cmdUpdate.Parameters.AddWithValue("@TotalYear", item["TotalYear"]);
                            cmdUpdate.Parameters.AddWithValue("@CGPA", item["CGPA"]);
                            cmdUpdate.Parameters.AddWithValue("@YearOfPassing", item["YearOfPassing"] ?? Convert.DBNull);
                            //cmdUpdate.Parameters.AddWithValue("@YearOfPassing", Ordinary.DateToString(item["YearOfPassing"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@Scale", item["Scale"]);
                            cmdUpdate.Parameters.AddWithValue("@Result", item["Result"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Marks", item["Marks"]);
                            cmdUpdate.Parameters.AddWithValue("@IsLast", item["IsLast"]);
                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            //cmdUpdate.Parameters.AddWithValue("@IsActive", true);


                            //cmdUpdate.Transaction = transaction;
                            //transResult = (int)cmdUpdate.ExecuteNonQuery();

                            //retResults[2] = transResult.ToString();// Return Id
                            //retResults[3] = sqlText; //  SQL Query

                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();

                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query

                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = "";
                            sqlText += @" INSERT INTO EmployeeEducation(
EmployeeId
,Degree_E
,Institute
,Major
,TotalYear
,YearOfPassing
,IsLast
,Remarks
,CGPA
,Scale
,Result
,Marks
) VALUES (
 @EmployeeId
,@Degree_E
,@Institute
,@Major
,@TotalYear
,@YearOfPassing
,@IsLast
,@Remarks
,@CGPA
,@Scale
,@Result
,@Marks
) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@Degree_E", item["Degree_E"]);
                            cmdInsert.Parameters.AddWithValue("@Institute", item["Institute"]);
                            cmdInsert.Parameters.AddWithValue("@Major", item["Major"]);
                            cmdInsert.Parameters.AddWithValue("@TotalYear", item["TotalYear"]);
                            cmdInsert.Parameters.AddWithValue("@YearOfPassing", item["YearOfPassing"]);
                            cmdInsert.Parameters.AddWithValue("@IsLast", item["IsLast"]);
                            //cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"]);
                            //cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            //cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsert.Parameters.AddWithValue("@CGPA", item["CGPA"]);
                            cmdInsert.Parameters.AddWithValue("@Scale", item["Scale"]);
                            cmdInsert.Parameters.AddWithValue("@Result", item["Result"]);
                            cmdInsert.Parameters.AddWithValue("@Marks", item["Marks"]);

                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeePersonalDetail.";
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

        public string[] InsertEmployeeLanguage(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeLanguage"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string Language_E = item["Language_E"].ToString();
                        string Fluency_E = item["Fluency_E"].ToString();
                        string Competency_E = item["Competency_E"].ToString();
                        //bool IsNumeric = Ordinary.IsNumeric(item["CGPA"].ToString());
                        ////bool IsNumeric = Ordinary.IsNumeric(item["Scale"].ToString());
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check
                        if (!string.IsNullOrWhiteSpace(Language_E))
                        {
                            if (!Ordinary.IsString(Language_E))
                            {
                                retResults[1] = "Language_E  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(Fluency_E))
                        {
                            if (!Ordinary.IsString(Fluency_E))
                            {
                                retResults[1] = "Fluency_E  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(Competency_E))
                        {
                            if (!Ordinary.IsString(Competency_E))
                            {
                                retResults[1] = "Competency_E  not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }

                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByMultiCondition("EmployeeLanguage", new[] { "EmployeeId", "Language_E" }, new[] { EmployeeId, Language_E }, currConn, transaction);


                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText = "update EmployeeLanguage set";
                            //sqlText += " EmployeeId=@EmployeeId";
                            sqlText += " Language_E=@Language_E";
                            sqlText += ", Fluency_E=@Fluency_E";
                            sqlText += ", Competency_E=@Competency_E";
                            sqlText += ", Remarks=@Remarks";

                            sqlText += ", IsActive=@IsActive";
                            sqlText += " where EmployeeId=@EmployeeId";
                            sqlText += " And Id=@Id";
                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

                            cmdUpdate.Parameters.AddWithValue("@Id", returnDt.Rows[0]["Id"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdUpdate.Parameters.AddWithValue("@Language_E", item["Language_E"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@Fluency_E", item["Fluency_E"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@Competency_E", item["Competency_E"].ToString());
                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);

                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();

                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query
                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = "  ";
                            sqlText += @" INSERT INTO EmployeeLanguage(	EmployeeId,Language_E,Fluency_E,Competency_E
,Remarks,IsActive,IsArchive
) VALUES (
@EmployeeId,@Language_E,@Fluency_E,@Competency_E
,@Remarks,@IsActive,@IsArchive) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@Language_E", item["Language_E"].ToString());
                            cmdInsert.Parameters.AddWithValue("@Fluency_E", item["Fluency_E"].ToString());
                            cmdInsert.Parameters.AddWithValue("@Competency_E", item["Competency_E"].ToString());
                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeLanguage.";
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


        public string[] InsertEmployeeEmergencyContact(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeEmergencyContact"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        string Code = item["EmployeeId"].ToString().Trim();
                        string Name = item["Name"].ToString();
                        string Relation = item["Relation"].ToString();
                        string Email = item["Email"].ToString();
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Name Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        #endregion Required Field
                        #region Data Format Validation Check

                        if (!string.IsNullOrWhiteSpace(Relation))
                        {
                            if (!Ordinary.IsDate(Relation))
                            {
                                retResults[1] = "Relation not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(Email))
                        {
                            if (!Ordinary.IsDate(Email))
                            {
                                retResults[1] = "Email not in Input the file !";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }

                        #endregion Data Format Validation Check


                        #region Finding EmployeeId Using EmployeeId
                        string EmployeeId = item["EmployeeId"].ToString();
                        DataTable returnDt = _cDal.SelectByCondition("EmployeeEmergencyContact", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText = "update EmployeeEmergencyContact set";
                            sqlText += " EmployeeId=@EmployeeId,";

                            sqlText += " Name=@Name,";
                            sqlText += " Relation=@Relation,";
                            sqlText += " Address=@Address,";
                            sqlText += " District=@District,";
                            sqlText += " Division=@Division,";
                            sqlText += " Country=@Country,";
                            sqlText += " City=@City,";
                            sqlText += " PostalCode=@PostalCode,";
                            sqlText += " Phone=@Phone,";
                            sqlText += " Mobile=@Mobile,";
                            sqlText += " Fax=@Fax,";

                            sqlText += " Email=@Email,";
                            sqlText += " Remarks=@Remarks,";
                            sqlText += " IsActive=@IsActive,";
                            sqlText += " where EmployeeId=@EmployeeId";

                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);

                            cmdUpdate.Parameters.AddWithValue("@Name", item["Name"]);
                            cmdUpdate.Parameters.AddWithValue("@Relation", item["Relation"]);
                            cmdUpdate.Parameters.AddWithValue("@Address", item["Address"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@District", item["District"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Division", item["Division"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Country", item["Country"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@City", item["City"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@PostalCode", item["PostalCode"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Phone", item["Phone"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Mobile", item["Mobile"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Fax", item["Fax"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@Email", item["Email"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);


                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();

                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query



                            #endregion Update Settings

                        }
                        else
                        {
                            #region Sql Satement
                            sqlText = "";
                            sqlText += @" INSERT INTO EmployeeEmergencyContact(
EmployeeId,Name,Relation,Address,District,Division,Country,City,PostalCode,PostOffice,Phone,Mobile,Fax,Email
,Remarks,IsActive,IsArchive

) VALUES (
@EmployeeId,@Name,@Relation,@Address,@District,@Division,@Country,@City,@PostalCode,@PostOffice,@Phone,@Mobile,@Fax,@Email

,@Remarks,@IsActive,@IsArchive
) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@Name", item["Name"]);
                            cmdInsert.Parameters.AddWithValue("@Relation", item["Relation"]);
                            cmdInsert.Parameters.AddWithValue("@Address", item["Address"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@District", item["District"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Division", item["Division"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Country", item["Country"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@City", item["City"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@PostalCode", item["PostalCode"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@PostOffice", item["PostOffice"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Phone", item["Phone"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Mobile", item["Mobile"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Fax", item["Fax"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Email", item["Email"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeEmergencyContact";
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


        public string[] InsertEmployeeJob(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string DepartmentId = "";
            string DesignationId = "";
            string ProjectId = "";
            string SectionId = "";
            string[] conditionFields = new string[10];
            string[] conditionValues = new string[10];
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeJob"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        DataTable returnDt = new DataTable();

                        #region Required Field
                        string EmployeeId = item["EmployeeId"].ToString().Trim();
                        string JoinDate = Ordinary.StringToDate(item["JoinDate"].ToString());
                        string LeftDate = Ordinary.StringToDate(item["LeftDate"].ToString());
                        string Department = item["DepartmenName"].ToString().Trim();
                        string Designation = item["Designation"].ToString().Trim();
                        string Project = item["ProjectName"].ToString().Trim();
                        string Section = item["SectionName"].ToString().Trim();


                        #region Finding DepartmentId Using Department
                        returnDt = _cDal.SelectByCondition("Department", "Name", Department, currConn, transaction);
                        if (returnDt != null && returnDt.Rows.Count > 0)
                        {
                            DepartmentId = returnDt.Rows[0]["Id"].ToString();
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
                            DesignationId = returnDt.Rows[0]["Id"].ToString();
                        }
                        else
                        {
                            retResults[1] = "Designation Not Found for " + Designation;
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Finding DesignationId Using Designation
                        #region Finding ProjectId Using Project
                        returnDt = _cDal.SelectByCondition("Project", "Name", Project, currConn, transaction);
                        if (returnDt != null && returnDt.Rows.Count > 0)
                        {
                            ProjectId = returnDt.Rows[0]["Id"].ToString();
                        }
                        else
                        {
                            retResults[1] = "Project Not Found for " + Project;
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Finding ProjectId Using Project
                        #region Finding SectionId Using Section
                        returnDt = _cDal.SelectByCondition("Section", "Name", Section, currConn, transaction);
                        if (returnDt != null && returnDt.Rows.Count > 0)
                        {
                            SectionId = returnDt.Rows[0]["Id"].ToString();
                        }
                        else
                        {
                            retResults[1] = "Section Not Found for " + Section;
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Finding SectionId Using Section

                        if (string.IsNullOrWhiteSpace(EmployeeId))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        if (string.IsNullOrWhiteSpace(DepartmentId))
                        {
                            retResults[1] = "Department Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        if (string.IsNullOrWhiteSpace(SectionId))
                        {
                            retResults[1] = "Section Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Required Field
                        #region Data Format Validation Check

                        if (!string.IsNullOrWhiteSpace(JoinDate))
                        {
                            if (!Ordinary.IsDate(JoinDate))
                            {
                                retResults[1] = "JoinDate not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(LeftDate))
                        {
                            if (!Ordinary.IsDate(LeftDate))
                            {
                                retResults[1] = "LeftDate not in Correct Format!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        if (!Ordinary.IsNumeric(item["BasicSalary"].ToString()))
                        {
                            retResults[1] = "BasicSalary Limit not in Correct Format!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Data Format Validation Check

                        #region Finding EmployeeId Using EmployeeId
                        returnDt = _cDal.SelectByCondition("EmployeeJob", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
                        if (Exist)
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText = "update EmployeeJob set";
                            //sqlText += " EmployeeId=@EmployeeId,";

                            sqlText += " JoinDate=@JoinDate";
                            sqlText += ",FromDate=@FromDate";
                            sqlText += ",RetirementDate=@RetirementDate";
                            sqlText += ",fristExDate=@fristExDate";
                            sqlText += ",secondExDate=@secondExDate";
                            sqlText += ",ContrExDate=@ContrExDate";
                            sqlText += ",ToDate=@ToDate";
                            sqlText += ", ExtendedProbationMonth=@ExtendedProbationMonth";
                            sqlText += ", ProbationMonth=@ProbationMonth";
                            sqlText += ", ProbationEnd=@ProbationEnd";
                            sqlText += ", DateOfPermanent=@DateOfPermanent";
                            sqlText += ", EmploymentStatus_E=@EmploymentStatus_E";
                            sqlText += ", EmploymentType_E=@EmploymentType_E";
                            sqlText += ", Supervisor=@Supervisor";
                            sqlText += ", BankInfo=@BankInfo";
                            sqlText += ", BankAccountNo=@BankAccountNo";
                            sqlText += ", IsPermanent=@IsPermanent";
                            sqlText += " , IsBuild=@IsBuild";
                            sqlText += " , IsTAXApplicable=@IsTAXApplicable";
                            sqlText += " , IsPFApplicable=@IsPFApplicable";
                            sqlText += " , IsGFApplicable=@IsGFApplicable";
                            sqlText += " ,IsInactive=@IsInactive";
                            sqlText += ", Rank=@Rank";
                            sqlText += ", Duration=@Duration";
                            sqlText += ", DotedLineReport=@DotedLineReport";
                            sqlText += ", GrossSalary=@GrossSalary";
                            sqlText += ", BasicSalary=@BasicSalary";
                            sqlText += ", GFStartFrom=@GFStartFrom";
                            sqlText += ", Other1=@Other1";
                            sqlText += ", Other2=@Other2";
                            sqlText += ", Other3=@Other3";
                            sqlText += ", Other4=@Other4";
                            sqlText += ", Other5=@Other5";
                            sqlText += ", IsJobBefore=@IsJobBefore";

                            sqlText += ", AccountType=@AccountType";
                            sqlText += ", FirstHoliday=@FirstHoliday";
                            sqlText += ", SecondHoliday=@SecondHoliday";

                            sqlText += ", ProjectId=@ProjectId";
                            sqlText += ", DepartmentId=@DepartmentId";
                            sqlText += ", SectionId=@SectionId";
                            sqlText += ", DesignationId=@DesignationId";
                            sqlText += ", BankAccountName=@BankAccountName";
                            sqlText += ", Routing_No=@Routing_No";
                            sqlText += ", EmpCategory=@EmpCategory";
                            sqlText += ", EmpJobType=@EmpJobType";
                            sqlText += ", Retirement=@Retirement";
                            sqlText += ", Force=@Force";
                            sqlText += ", Extentionyn=@Extentionyn";

                            //sqlText +=, " StructureGroupId=@StructureGroupId";
                            sqlText += ", Remarks=@Remarks";
                            sqlText += ", IsActive=@IsActive";
                            sqlText += " where EmployeeId=@EmployeeId";




                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);

                            cmdUpdate.Parameters.AddWithValue("@JoinDate", Ordinary.DateToString(item["JoinDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(item["FromDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@RetirementDate", Ordinary.DateToString(item["RetirementDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@fristExDate", Ordinary.DateToString(item["fristExDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@secondExDate", Ordinary.DateToString(item["secondExDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@ContrExDate", Ordinary.DateToString(item["ContrExDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(item["ToDate"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@GFStartFrom", Ordinary.DateToString(item["GFStartFrom"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@ProbationMonth", item["ProbationMonth"]);
                            cmdUpdate.Parameters.AddWithValue("@ExtendedProbationMonth", item["ExtendedProbationMonth"]);
                            cmdUpdate.Parameters.AddWithValue("@ProbationEnd", Ordinary.DateToString(item["ProbationEnd"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@DateOfPermanent", Ordinary.DateToString(item["DateOfPermanent"].ToString()));
                            cmdUpdate.Parameters.AddWithValue("@EmploymentStatus_E", item["EmploymentStatus_E"]);
                            cmdUpdate.Parameters.AddWithValue("@EmploymentType_E", item["EmploymentType_E"]);
                            cmdUpdate.Parameters.AddWithValue("@Supervisor", item["Supervisor"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@BankInfo", item["BankInfo"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@BankAccountNo", item["BankAccountNo"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsPermanent", item["IsPermanent"]);
                            cmdUpdate.Parameters.AddWithValue("@IsBuild", item["IsBuild"]);
                            cmdUpdate.Parameters.AddWithValue("@IsTAXApplicable", item["IsTAXApplicable"]);
                            cmdUpdate.Parameters.AddWithValue("@IsPFApplicable", item["IsPFApplicable"]);
                            cmdUpdate.Parameters.AddWithValue("@IsGFApplicable", item["IsGFApplicable"]);
                            cmdUpdate.Parameters.AddWithValue("@Rank", item["Rank"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Duration", item["Duration"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@DotedLineReport", item["DotedLineReport"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsInactive", item["IsInactive"]);
                            cmdUpdate.Parameters.AddWithValue("@GrossSalary", item["GrossSalary"]);
                            cmdUpdate.Parameters.AddWithValue("@BasicSalary", item["BasicSalary"]);
                            cmdUpdate.Parameters.AddWithValue("@Other1", item["Other1"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Other2", item["Other2"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Other3", item["Other3"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Other4", item["Other4"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Other5", item["Other5"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsJobBefore", item["IsJobBefore"]);
                            cmdUpdate.Parameters.AddWithValue("@AccountType", item["AccountType"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@FirstHoliday", item["FirstHoliday"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@SecondHoliday", item["SecondHoliday"] ?? Convert.DBNull);

                            cmdUpdate.Parameters.AddWithValue("@ProjectId", ProjectId);
                            cmdUpdate.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                            cmdUpdate.Parameters.AddWithValue("@SectionId", SectionId);
                            cmdUpdate.Parameters.AddWithValue("@DesignationId", DesignationId);
                            cmdUpdate.Parameters.AddWithValue("@BankAccountName", item["BankAccountName"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Routing_No", item["Routing_No"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@EmpCategory", item["EmpCategory"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@EmpJobType", item["EmpJobType"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Retirement", item["Retirement"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Force", item["Force"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Extentionyn", item["Extentionyn"] ?? Convert.DBNull);

                            //cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                            //cmdUpdate.Parameters.AddWithValue("@Email", item["Email"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@IsActive", true);

                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();

                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query



                            #endregion Update Settings

                        }
                        else
                        {
                            #region Designation

                            sqlText = " update EmployeePromotion set IsCurrent=0 where EmployeeId =@EmployeeId";
                            SqlCommand cmdUpdatePromotion = new SqlCommand(sqlText, currConn, transaction);
                            cmdUpdatePromotion.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            try
                            {
                                cmdUpdatePromotion.ExecuteNonQuery();
                            }
                            catch (Exception)
                            {
                                throw new ArgumentNullException("Promotion is not complete", "");
                            }

                            sqlText = "  ";
                            sqlText += @" INSERT INTO EmployeePromotion(	EmployeeId,DesignationId,IsPromotion,PromotionDate
,IsCurrent,FileName,Remarks,IsActive,IsArchive) 
VALUES (@EmployeeId,@DesignationId,@IsPromotion,@PromotionDate
,@IsCurrent,@FileName,@Remarks,@IsActive,@IsArchive) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsertPromotion = new SqlCommand(sqlText, currConn, transaction);

                            cmdInsertPromotion.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmdInsertPromotion.Parameters.AddWithValue("@DesignationId", DesignationId);
                            cmdInsertPromotion.Parameters.AddWithValue("@IsPromotion", false);
                            cmdInsertPromotion.Parameters.AddWithValue("@PromotionDate", item["JoinDate"].ToString());
                            cmdInsertPromotion.Parameters.AddWithValue("@IsCurrent", true);
                            cmdInsertPromotion.Parameters.AddWithValue("@FileName", "");
                            cmdInsertPromotion.Parameters.AddWithValue("@Remarks", "");
                            cmdInsertPromotion.Parameters.AddWithValue("@IsActive", true);
                            cmdInsertPromotion.Parameters.AddWithValue("@IsArchive", false);

                            cmdInsertPromotion.ExecuteNonQuery();

                            #endregion

                            #region Department
                            //                            string transfer_Id = _cDal.NextIdWithBranch("EmployeeTransfer", Program.BranchId, currConn, transaction);

                            //                            sqlText = " update EmployeeTransfer set IsCurrent=0 where EmployeeId =@EmployeeId";
                            //                            SqlCommand cmdUpdateTransfer = new SqlCommand(sqlText, currConn, transaction);
                            //                            cmdUpdateTransfer.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            //                            try
                            //                            {
                            //                                cmdUpdateTransfer.ExecuteNonQuery();
                            //                            }
                            //                            catch (Exception)
                            //                            {
                            //                                throw new ArgumentNullException("Transfer is not complete", "");
                            //                            }

                            //                            sqlText = "  ";
                            //                            sqlText += @" INSERT INTO EmployeeTransfer(
                            //Id,EmployeeId,BranchId,ProjectId,DepartmentId,SectionId,TransferDate,IsCurrent,FileName
                            //,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                            //) VALUES (
                            //@Id,@EmployeeId,@BranchId,@ProjectId,@DepartmentId,@SectionId,@TransferDate,@IsCurrent,@FileName
                            //,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                            //)";

                            //                            SqlCommand cmdInsertTransfer = new SqlCommand(sqlText, currConn, transaction);

                            //                            cmdInsertTransfer.Parameters.AddWithValue("@Id", transfer_Id);
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@BranchId", Program.BranchId);
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@ProjectId", ProjectId);
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@TransferDate", Ordinary.DateToString(item["JoinDate"].ToString()) ?? Convert.DBNull);
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@IsCurrent", true);
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@SectionId", SectionId);
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@Remarks", "");
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@FileName", "");

                            //                            cmdInsertTransfer.Parameters.AddWithValue("@IsActive", true);
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@IsArchive", false);
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@CreatedBy", "Admin");
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@CreatedAt", "19000101");
                            //                            cmdInsertTransfer.Parameters.AddWithValue("@CreatedFrom", "local");
                            //                            cmdInsertTransfer.ExecuteNonQuery();
                            #endregion
                            #region Sql Satement
                            sqlText = "";
                            sqlText += @" INSERT INTO EmployeeJob(
EmployeeId
,JoinDate
,FromDate
,RetirementDate
,fristExDate
,secondExDate
,ContrExDate
,ToDate
,ProbationMonth
,ExtendedProbationMonth
,ProbationEnd
,DateOfPermanent
,EmploymentStatus_E
,EmploymentType_E
,Supervisor
,BankInfo
,BankAccountNo
,IsPermanent
,IsBuild
,IsTAXApplicable
,IsPFApplicable
,IsGFApplicable
,IsInactive
,GrossSalary
,BasicSalary
,Other1
,Other2
,Other3
,Other4
,Other5
,IsJobBefore
,AccountType
,FirstHoliday
,SecondHoliday
,Force
,Rank
,Duration
,DotedLineReport
,Retirement
,Extentionyn
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,EmpCategory
,EmpJobType
,Remarks
,IsActive
,IsArchive
,BankAccountName
,Routing_No
,GFStartFrom
) VALUES (

@EmployeeId
,@JoinDate
,@FromDate
,@RetirementDate
,@fristExDate
,@secondExDate
,@ContrExDate
,@ToDate
,@ProbationMonth
,@ExtendedProbationMonth
,@ProbationEnd
,@DateOfPermanent
,@EmploymentStatus_E
,@EmploymentType_E
,@Supervisor
,@BankInfo
,@BankAccountNo
,@IsPermanent
,@IsBuild
,@IsTAXApplicable
,@IsPFApplicable 
,@IsGFApplicable
,@IsInactive
,@GrossSalary
,@BasicSalary
,@Other1
,@Other2
,@Other3
,@Other4
,@Other5
,@IsJobBefore
,@AccountType
,@FirstHoliday
,@SecondHoliday
,@Force
,@Rank
,@Duration
,@DotedLineReport
,@Retirement
,@Extentionyn
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@EmpCategory
,@EmpJobType
,@Remarks
,@IsActive
,@IsArchive
,@BankAccountName
,@Routing_No
,@GFStartFrom

) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                            cmdInsert.Parameters.AddWithValue("@JoinDate", Ordinary.DateToString(item["JoinDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(item["FromDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@RetirementDate", Ordinary.DateToString(item["RetirementDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@fristExDate", Ordinary.DateToString(item["fristExDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@secondExDate", Ordinary.DateToString(item["secondExDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ContrExDate", Ordinary.DateToString(item["ContrExDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(item["ToDate"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@GFStartFrom", Ordinary.DateToString(item["GFStartFrom"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ProbationMonth", item["ProbationMonth"]);

                            cmdInsert.Parameters.AddWithValue("@ExtendedProbationMonth", item["ExtendedProbationMonth"]);
                            cmdInsert.Parameters.AddWithValue("@ProbationEnd", Ordinary.DateToString(item["ProbationEnd"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@DateOfPermanent", Ordinary.DateToString(item["DateOfPermanent"].ToString()) ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@EmploymentStatus_E", item["EmploymentStatus_E"]);
                            cmdInsert.Parameters.AddWithValue("@EmploymentType_E", item["EmploymentType_E"]);
                            cmdInsert.Parameters.AddWithValue("@BankInfo", item["BankInfo"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@BankAccountNo", item["BankAccountNo"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Supervisor", item["Supervisor"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsPermanent", item["IsPermanent"]);
                            cmdInsert.Parameters.AddWithValue("@IsBuild", item["IsBuild"]);
                            cmdInsert.Parameters.AddWithValue("@IsTAXApplicable", item["IsTAXApplicable"]);
                            cmdInsert.Parameters.AddWithValue("@IsPFApplicable", item["IsPFApplicable"]);
                            cmdInsert.Parameters.AddWithValue("@IsGFApplicable", item["IsGFApplicable"]);
                            cmdInsert.Parameters.AddWithValue("@IsInactive", item["IsInactive"]);

                            cmdInsert.Parameters.AddWithValue("@GrossSalary", item["GrossSalary"]);
                            cmdInsert.Parameters.AddWithValue("@BasicSalary", item["BasicSalary"]);
                            cmdInsert.Parameters.AddWithValue("@Other1", item["Other1"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Other2", item["Other2"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Other3", item["Other3"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Other4", item["Other4"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Other5", item["Other5"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsJobBefore", item["IsJobBefore"]);
                            cmdInsert.Parameters.AddWithValue("@AccountType", item["AccountType"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@FirstHoliday", item["FirstHoliday"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@SecondHoliday", item["SecondHoliday"] ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@ProjectId", ProjectId);
                            cmdInsert.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                            cmdInsert.Parameters.AddWithValue("@SectionId", SectionId);
                            cmdInsert.Parameters.AddWithValue("@DesignationId", DesignationId);
                            cmdInsert.Parameters.AddWithValue("@Force", item["Force"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Retirement", item["Retirement"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Extentionyn", item["Extentionyn"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Duration", item["Duration"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Rank", item["Rank"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@DotedLineReport", item["DotedLineReport"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@EmpCategory", item["EmpCategory"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@EmpJobType", item["EmpJobType"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@BankAccountName", item["BankAccountName"] ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                            cmdInsert.ExecuteNonQuery();
                            #endregion Sql Execution
                        }
                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeePersonalDetail.";
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

        #region test

        #endregion

        //        public string[] InsertEmployeePF(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        //        {
        //            #region Initializ
        //            string sqlText = "";
        //            int Id = 0;
        //            int transResult = 0;
        //            string PFStructureId = "";

        //            string[] conditionFields = new string[10];
        //            string[] conditionValues = new string[10];
        //            string[] retResults = new string[6];
        //            retResults[0] = "Fail";//Success or Fail
        //            retResults[1] = "Fail";// Success or Fail Message
        //            retResults[2] = Id.ToString();// Return Id
        //            retResults[3] = sqlText; //  SQL Query
        //            retResults[4] = "ex"; //catch ex
        //            retResults[5] = "InsertEmployeeJob"; //Method Name
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;
        //            //EmployeeInfoVM vm = new EmployeeInfoVM();
        //            CommonDAL _cDal = new CommonDAL();
        //            #endregion
        //            #region Try
        //            try
        //            {
        //                #region Validation

        //                #endregion Validation
        //                #region open connection and transaction
        //                #region New open connection and transaction
        //                if (VcurrConn != null)
        //                {
        //                    currConn = VcurrConn;
        //                }
        //                if (Vtransaction != null)
        //                {
        //                    transaction = Vtransaction;
        //                }
        //                #endregion New open connection and transaction
        //                if (currConn == null)
        //                {
        //                    currConn = _dbsqlConnection.GetConnection();
        //                    if (currConn.State != ConnectionState.Open)
        //                    {
        //                        currConn.Open();
        //                    }
        //                }
        //                if (transaction == null)
        //                {
        //                    transaction = currConn.BeginTransaction("");
        //                }
        //                #endregion open connection and transaction
        //                #region Save
        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    foreach (DataRow item in dt.Rows)
        //                    {
        //                        DataTable returnDt = new DataTable();

        //                        #region Required Field
        //                        string EmployeeId = item["EmployeeId"].ToString().Trim();
        //                        string Name = item["EmployeeName"].ToString();
        //                        string Designation = item["Designation"].ToString();
        //                        string PFStructure = item["PfStructureName"].ToString().Trim();
        //                        string PFValue = item["PFValue"].ToString().Trim();
        //                        string IsFixed = item["IsFixed"].ToString().Trim();
        //                        string PortionSalaryType = item["PortionSalaryType"].ToString().Trim();


        //                        #region Finding PfStructureId Using pfstructure
        //                        returnDt = _cDal.SelectByCondition("PFStructure", "Name", PFStructure, currConn, transaction);
        //                        if (returnDt != null && returnDt.Rows.Count > 0)
        //                        {
        //                            PFStructureId = returnDt.Rows[0]["Id"].ToString();
        //                        }
        //                        else
        //                        {
        //                            retResults[1] = "ID Not Found for " + PFStructure;
        //                            throw new ArgumentNullException(retResults[1], "");
        //                        }
        //                        #endregion Finding DepartmentId Using Departmen

        //                        if (string.IsNullOrWhiteSpace(EmployeeId))
        //                        {
        //                            retResults[1] = "Employee Id Can't Be Blank!";
        //                            throw new ArgumentNullException(retResults[1], "");
        //                        }

        //                        #endregion Required Field

        //                        #region Finding EmployeeId Using EmployeeId
        //                        returnDt = _cDal.SelectByCondition("EmployeePF", "EmployeeId", EmployeeId, currConn, transaction);

        //                        #endregion Finding EmployeeId Using EmployeeId

        //                        bool Exist = returnDt != null && returnDt.Rows.Count > 0;
        //                        if (Exist)
        //                        {
        //                            #region Update Settings

        //                            sqlText = "";
        //                            sqlText = "update EmployeePF set";
        //                            //sqlText += " EmployeeId=@EmployeeId,";

        //                            sqlText += " PFStructureId=@PFStructureId";
        //                            sqlText += ",PFValue=@PFValue";
        //                            sqlText += ",IsFixed=@IsFixed";
        //                            sqlText += ",PortionSalaryType=@PortionSalaryType";
        //                            sqlText += " where EmployeeId=@EmployeeId";




        //                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

        //                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);

        //                            cmdUpdate.Parameters.AddWithValue("@PFStructureId", PFStructureId);
        //                            cmdUpdate.Parameters.AddWithValue("@PFValue", item["PFValue"]);
        //                            cmdUpdate.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
        //                            cmdUpdate.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);

        //                            cmdUpdate.Transaction = transaction;
        //                            transResult = (int)cmdUpdate.ExecuteNonQuery();

        //                            retResults[2] = transResult.ToString();// Return Id
        //                            retResults[3] = sqlText; //  SQL Query



        //                            #endregion Update Settings

        //                        }
        //                        else
        //                        {
        //                            #region Designation

        //                            sqlText = " update EmployeePF set IsActive=0 where EmployeeId =@EmployeeId";
        //                            SqlCommand cmdUpdatePromotion = new SqlCommand(sqlText, currConn, transaction);
        //                            cmdUpdatePromotion.Parameters.AddWithValue("@EmployeeId", EmployeeId);
        //                            try
        //                            {
        //                                cmdUpdatePromotion.ExecuteNonQuery();
        //                            }
        //                            catch (Exception)
        //                            {
        //                                throw new ArgumentNullException("Promotion is not complete", "");
        //                            }

        //                            sqlText = "  ";
        //                            sqlText += @" INSERT INTO EmployeePF(EmployeeId,PFStructureId,PFValue,IsFixed,
        //PortionSalaryType) 
        //VALUES (@EmployeeId,@PFStructureId,@PFValue,@IsFixed,
        //@PortionSalaryType) SELECT SCOPE_IDENTITY()";

        //                            SqlCommand cmdInsertU = new SqlCommand(sqlText, currConn, transaction);

        //                            cmdInsertU.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
        //                            cmdInsertU.Parameters.AddWithValue("@PFStructureId", PFStructureId);
        //                            cmdInsertU.Parameters.AddWithValue("@PFValue", item["PFValue"]);
        //                            cmdInsertU.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
        //                            cmdInsertU.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);
        //                            cmdInsertU.ExecuteNonQuery();

        //                            #endregion


        //                            #region Sql Satement
        //                            sqlText = "";
        //                            sqlText += @" INSERT INTO EmployeePF(
        //EmployeeId,PFStructureId,PFValue,IsFixed,
        //PortionSalaryType) 
        //VALUES (@EmployeeId,@PFStructureId,@PFValue,@IsFixed,
        //@PortionSalaryType) SELECT SCOPE_IDENTITY()";

        //                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
        //                            cmdInsert.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
        //                            cmdInsert.Parameters.AddWithValue("@PFStructureId", PFStructureId);
        //                            cmdInsert.Parameters.AddWithValue("@PFValue", item["PFValue"]);
        //                            cmdInsert.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
        //                            cmdInsert.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);

        //                            cmdInsert.ExecuteNonQuery();
        //                            #endregion Sql Execution
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    retResults[1] = "Unexpected Error!";
        //                    throw new ArgumentNullException("Unexpected Error!", "");
        //                }
        //                #endregion Save

        //                #region Commit
        //                if (Vtransaction == null)
        //                {
        //                    if (transaction != null)
        //                    {
        //                        transaction.Commit();
        //                    }
        //                }
        //                #endregion Commit
        //                #region SuccessResult
        //                retResults[0] = "Success";
        //                retResults[1] = "Data Save Successfully";
        //                #endregion SuccessResult
        //            }
        //            #endregion try
        //            #region Catch and Finall
        //            catch (Exception ex)
        //            {
        //                retResults[0] = "Fail";//Success or Fail
        //                retResults[4] = ex.Message.ToString(); //catch ex
        //                if (Vtransaction != null)
        //                {
        //                    try
        //                    {
        //                        if (Vtransaction == null) { transaction.Rollback(); }
        //                    }
        //                    catch (Exception)
        //                    {
        //                        retResults[1] = "Unexpected error to update EmployeePersonalDetail.";
        //                        return retResults;
        //                    }
        //                }
        //                return retResults;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null)
        //                {
        //                    if (currConn != null)
        //                    {
        //                        if (currConn.State == ConnectionState.Open)
        //                        {
        //                            currConn.Close();
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            #region Results
        //            return retResults;
        //            #endregion
        //        }

        //        public string[] InsertEmployeePF(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //        {

        //            string sqlText = "";
        //            int Id = 0;

        //            string PFStructureId = "";
        //            string[] conditionFields = new string[10];
        //            string[] conditionValues = new string[10];
        //            string[] retResults = new string[6];
        //            retResults[0] = "Fail";//Success or Fail
        //            retResults[1] = "Fail";// Success or Fail Message
        //            retResults[2] = Id.ToString();// Return Id
        //            retResults[3] = sqlText; //  SQL Query
        //            retResults[4] = "ex"; //catch ex
        //            retResults[5] = "InsertPF"; //Method Name
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;

        //            CommonDAL _cDal = new CommonDAL();


        //            try
        //            {



        //                if (VcurrConn != null)
        //                {
        //                    currConn = VcurrConn;
        //                }
        //                if (Vtransaction != null)
        //                {
        //                    transaction = Vtransaction;
        //                }

        //                if (currConn == null)
        //                {
        //                    currConn = _dbsqlConnection.GetConnection();
        //                    if (currConn.State != ConnectionState.Open)
        //                    {
        //                        currConn.Open();
        //                    }
        //                }
        //                if (transaction == null)
        //                {
        //                    transaction = currConn.BeginTransaction("");
        //                }



        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    foreach (DataRow item in dt.Rows)
        //                    {
        //                        DataTable returnDt = new DataTable();



        //                        string EmployeeId = item["EmployeeId"].ToString().Trim();
        //                        string Name = item["EmployeeName"].ToString();
        //                        string Designation = item["Designation"].ToString();
        //                        string PFStructure = item["PfStructureName"].ToString().Trim();
        //                        string PFValue = item["PFValue"].ToString().Trim();
        //                        string IsFixed = item["IsFixed"].ToString().Trim();
        //                        string PortionSalaryType = item["PortionSalaryType"].ToString().Trim();


        //                        #region Finding PfStructureId Using pfstructure
        //                        returnDt = _cDal.SelectByCondition("PFStructure", "Name", PFStructure, currConn, transaction);
        //                        if (returnDt != null && returnDt.Rows.Count > 0)
        //                        {
        //                            PFStructureId = returnDt.Rows[0]["Id"].ToString();
        //                        }
        //                        else
        //                        {
        //                            retResults[1] = "ID Not Found for " + PFStructure;
        //                            throw new ArgumentNullException(retResults[1], "");
        //                        }
        //                        #endregion Finding DepartmentId Using Departmen

        //                        if (string.IsNullOrWhiteSpace(EmployeeId))
        //                        {
        //                            retResults[1] = "Employee Id Can't Be Blank!";
        //                            throw new ArgumentNullException(retResults[1], "");
        //                        }


        //                        object exeRes;


        //                        sqlText = "  ";
        //                        sqlText += @" update  EmployeePF set 
        //                            PFStructureId=@PFStructureId
        //                            ,PFValue=@PFValue
        //                            ,IsFixed=@IsFixed
        //                            ,PortionSalaryType=@PortionSalaryType
        //                            where EmployeeId=@EmployeeId   ";
        //                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
        //                        cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);

        //                        cmdUpdate.Parameters.AddWithValue("@PFStructureId", PFStructureId);
        //                        cmdUpdate.Parameters.AddWithValue("@PFValue", item["PFValue"]);
        //                        cmdUpdate.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
        //                        cmdUpdate.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);
        //                        cmdUpdate.ExecuteNonQuery();

        //                        sqlText = @"Select count(Id) from EmployeePF 
        //                                where EmployeeId=@EmployeeId";
        //                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn, transaction);
        //                        cmdExist.Parameters.AddWithValue("@EmployeeId", EmployeeId);

        //                        exeRes = cmdExist.ExecuteScalar();
        //                        int alreadyExist = Convert.ToInt32(exeRes);
        //                        sqlText = "Select MAX(Id+1)Id from EmployeePF";
        //                        SqlCommand cmdId = new SqlCommand(sqlText, currConn, transaction);
        //                        exeRes = cmdId.ExecuteScalar();
        //                        int PFId = Convert.ToInt32(exeRes);
        //                        sqlText = "";
        //                        sqlText += @" INSERT INTO EmployeePF(Id,
        //EmployeeId,PFStructureId,PFValue,IsFixed,
        //PortionSalaryType,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
        //VALUES (@Id,@EmployeeId,@PFStructureId,@PFValue,@IsFixed,
        //@PortionSalaryType,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";

        //                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
        //                        cmdInsert.Parameters.AddWithValue("@Id", (PFId + 1));
        //                        cmdInsert.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
        //                        cmdInsert.Parameters.AddWithValue("@PFStructureId", PFStructureId);
        //                        cmdInsert.Parameters.AddWithValue("@PFValue", item["PFValue"]);
        //                        cmdInsert.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
        //                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);
        //                        cmdInsert.Parameters.AddWithValue("@IsActive", 1);
        //                        cmdInsert.Parameters.AddWithValue("@IsArchive", 0);
        //                        cmdInsert.Parameters.AddWithValue("@CreatedBy", "Admin");
        //                        cmdInsert.Parameters.AddWithValue("@CreatedAt", "19000101");
        //                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", "local");
        //                        cmdInsert.ExecuteNonQuery();
        //                        PFId++;

        //                    }
        //                }


        //               else
        //                {
        //                    retResults[1] = "Unexpected Error!";
        //                    throw new ArgumentNullException("Unexpected Error!", "");
        //                }

        //                if (Vtransaction == null)
        //                {
        //                    if (transaction != null)
        //                    {
        //                        transaction.Commit();
        //                    }
        //                }

        //                retResults[0] = "Success";
        //                retResults[1] = "Data Save Successfully";

        //            }


        //            catch (Exception ex)
        //            {
        //                retResults[4] = ex.Message.ToString(); //catch ex
        //                try { transaction.Rollback(); }
        //                catch (Exception) { return retResults; }
        //                return retResults;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();
        //                }
        //            }

        //            return retResults;

        //        }
        //        public string[] InsertEmployeeLeaveStructure(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //        {
        //            #region Initialize
        //            string sqlText = "";
        //            int Id = 0;
        //            string[] retResults = { "Fail", "Fail", Id.ToString(), sqlText, "ex", "InsertPersonalDetail" };
        //            //0 - Success or Fail//1 - Success or Fail Message//2 - Return Id//3 - SQL Query //4 - catch ex//5 - Method Name
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;
        //            string EmployeeId = "";
        //            #endregion
        //            #region Try
        //            try
        //            {
        //                #region open connection and transaction
        //                if (VcurrConn == null)
        //                {
        //                    currConn = _dbsqlConnection.GetConnection();
        //                    if (currConn.State != ConnectionState.Open)
        //                    {
        //                        currConn.Open();
        //                    }
        //                }
        //                else if (VcurrConn != null)
        //                {
        //                    currConn = VcurrConn;
        //                }

        //                if (Vtransaction == null)
        //                {
        //                    transaction = currConn.BeginTransaction("");
        //                }
        //                else if (Vtransaction != null)
        //                {
        //                    transaction = Vtransaction;
        //                }

        //                #endregion open connection and transaction

        //                dt.Columns.Add("LeaveStructureId", typeof(string));
        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    DataTable leaveStDt = _cDal.SelectByCondition("LeaveStructure", "Name", dt.Rows[i]["LeaveStructure"].ToString(), currConn, transaction);
        //                    if (leaveStDt != null && leaveStDt.Rows.Count > 0)
        //                    {
        //                        dt.Rows[i]["LeaveStructureId"] = leaveStDt.Rows[0]["Id"].ToString();
        //                    }
        //                }
        //                //DataTable dtTemp = new DataTable();
        //                //dtTemp = EmployeeStructureGroup(dt, currConn, transaction);
        //                //dt = new DataTable();
        //                //dt = dtTemp;
        //                //if (retResults[0] == "Fail")
        //                //{
        //                //    retResults[1] = "EmployeeStructureGroup Insert Fail!";
        //                //    throw new ArgumentNullException(retResults[1], "");
        //                //}


        //                #region Save
        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    foreach (DataRow item in dt.Rows)
        //                    {
        //                        #region Required Field
        //                        string Code = item["EmployeeId"].ToString().Trim();
        //                        if (string.IsNullOrWhiteSpace(EmployeeId))
        //                        {
        //                            retResults[1] = "EmployeeId Can't Be Blank!";
        //                            throw new ArgumentNullException(retResults[1], "");
        //                        }

        //                        #endregion Required Field
        //                        #region Duplicate Check
        //                        //bool Exist = _cDal.ExistCheck("EmployeeStructureGroup", "EmployeeId", EmployeeId
        //                        //    , currConn, transaction);
        //                        //if (Exist)
        //                        //{
        //                        //    retResults[1] = "Code: " + EmployeeCode + " Already Used!";
        //                        //    throw new ArgumentNullException(retResults[1], "");
        //                        //}
        //                        #endregion Duplicate Check
        //                        EmployeeId = item["EmployeeId"].ToString();

        //                        object exeRes;
        //                        #region ESG Update
        //                        sqlText = "  ";
        //                        sqlText += @" update  EmployeeStructureGroup set 
        //                            LeaveStructureId=@LeaveStructureId
        //                            ,CreatedBy=@CreatedBy
        //                            ,CreatedAt=@CreatedAt
        //                            ,CreatedFrom=@CreatedFrom
        //                            where EmployeeId=@EmployeeId   ";
        //                        SqlCommand cmdSGUpdate = new SqlCommand(sqlText, currConn, transaction);
        //                        cmdSGUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
        //                        cmdSGUpdate.Parameters.AddWithValue("@LeaveStructureId", item["LeaveStructureId"]);
        //                        cmdSGUpdate.Parameters.AddWithValue("@CreatedBy", "Admin");
        //                        cmdSGUpdate.Parameters.AddWithValue("@CreatedAt", "19000101");
        //                        cmdSGUpdate.Parameters.AddWithValue("@CreatedFrom", "local");
        //                        cmdSGUpdate.ExecuteNonQuery();
        //                        #endregion ESG Update
        //                        #region Save
        //                        sqlText = @"Select count(Id) from EmployeeLeaveStructure 
        //                                where EmployeeId=@EmployeeId and LeaveYear=@LeaveYear
        //                            ";
        //                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn, transaction);
        //                        cmdExist.Parameters.AddWithValue("@EmployeeId", EmployeeId);
        //                        cmdExist.Parameters.AddWithValue("@LeaveYear", item["LeaveYear"]);
        //                        exeRes = cmdExist.ExecuteScalar();
        //                        int alreadyExist = Convert.ToInt32(exeRes);
        //                        sqlText = "Select count(Id) from EmployeeLeaveStructure ";
        //                        SqlCommand cmdId = new SqlCommand(sqlText, currConn, transaction);
        //                        exeRes = cmdId.ExecuteScalar();
        //                        int ELStructureId = Convert.ToInt32(exeRes);
        //                        if (alreadyExist > 0)
        //                        {
        //                            ////Update
        //                            #region sql statement
        //                            sqlText = @"SELECT *
        //                                from LeaveStructureDetail
        //                                where  LeaveStructureId=@LeaveStructureId ";
        //                            SqlCommand cmdLSD2 = new SqlCommand(sqlText, currConn, transaction);
        //                            cmdLSD2.Parameters.AddWithValue("@LeaveStructureId", item["LeaveStructureId"]);
        //                            DataTable dtLeaveStDetail = new DataTable();
        //                            SqlDataAdapter da1 = new SqlDataAdapter(cmdLSD2);
        //                            da1.Fill(dtLeaveStDetail);
        //                            if (dtLeaveStDetail.Rows.Count > 0)
        //                            {
        //                                foreach (DataRow itemLeaveStDetail in dtLeaveStDetail.Rows)
        //                                {
        //                                    var tt = itemLeaveStDetail["LeaveType_E"].ToString();
        //                                    //Update
        //                                    sqlText = @"SELECT *
        //                                FROM EmployeeLeaveStructure
        //                                WHERE  
        //                                EmployeeId=@EmployeeId AND 
        //                                LeaveYear=@LeaveYear AND
        //                                LeaveType_E=@LeaveType_E
        //                                ";
        //                                    SqlCommand cmdEmplLeaveSt = new SqlCommand(sqlText, currConn, transaction);
        //                                    cmdEmplLeaveSt.Parameters.AddWithValue("@EmployeeId", EmployeeId);
        //                                    cmdEmplLeaveSt.Parameters.AddWithValue("@LeaveYear", item["LeaveYear"]);
        //                                    cmdEmplLeaveSt.Parameters.AddWithValue("@LeaveType_E", itemLeaveStDetail["LeaveType_E"].ToString());
        //                                    DataTable dtEmpLeaveSt = new DataTable();
        //                                    SqlDataAdapter daEmpLeaveSt = new SqlDataAdapter(cmdEmplLeaveSt);
        //                                    daEmpLeaveSt.Fill(dtEmpLeaveSt);
        //                                    if (dtEmpLeaveSt.Rows.Count > 0)
        //                                    {
        //                                        // Update
        //                                        string EmployeeLeaveStructureId = dtEmpLeaveSt.Rows[0]["Id"].ToString();
        //                                        #region Update Settings
        //                                        sqlText = "";
        //                                        sqlText = "update EmployeeLeaveStructure set";
        //                                        sqlText += " LeaveDays=@LeaveDays,";
        //                                        sqlText += " IsEarned=@IsEarned,";
        //                                        sqlText += " IsCompensation=@IsCompensation,";
        //                                        sqlText += " LastUpdateBy=@LastUpdateBy,";
        //                                        sqlText += " LastUpdateAt=@LastUpdateAt,";
        //                                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
        //                                        sqlText += " where Id=@Id";
        //                                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
        //                                        cmdUpdate.Parameters.AddWithValue("@Id", EmployeeLeaveStructureId);
        //                                        cmdUpdate.Parameters.AddWithValue("@LeaveDays", itemLeaveStDetail["LeaveDays"].ToString());
        //                                        cmdUpdate.Parameters.AddWithValue("@IsEarned", itemLeaveStDetail["IsEarned"].ToString());
        //                                        cmdUpdate.Parameters.AddWithValue("@IsCompensation", itemLeaveStDetail["IsCompensation"].ToString());
        //                                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", "Admin");
        //                                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", "19000101");
        //                                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", "local");
        //                                        cmdUpdate.ExecuteNonQuery();
        //                                        #endregion Update Settings
        //                                    }
        //                                    else
        //                                    {
        //                                        //Insert
        //                                        #region Save
        //                                        sqlText = "  ";
        //                                        sqlText += @"   INSERT INTO EmployeeLeaveStructure(
        //                                            Id,EmployeeId,LeaveStructureId,LeaveYear,LeaveType_E,LeaveDays,OpeningLeaveDays,IsEarned,IsCompensation,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,IsCarryForward,MaxBalance         
        //                                            ) VALUES (
        //                                            @Id,@EmployeeId,@LeaveStructureId,@LeaveYear,@LeaveType_E,@LeaveDays,@OpeningLeaveDays,@IsEarned,@IsCompensation,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@IsCarryForward,@MaxBalance
        //                                            )  ";
        //                                        SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
        //                                        cmdIns.Parameters.AddWithValue("@Id", (ELStructureId + 1));
        //                                        cmdIns.Parameters.AddWithValue("@EmployeeId", EmployeeId);
        //                                        cmdIns.Parameters.AddWithValue("@LeaveStructureId", item["LeaveStructureId"]);
        //                                        cmdIns.Parameters.AddWithValue("@LeaveYear", item["LeaveYear"]);
        //                                        cmdIns.Parameters.AddWithValue("@LeaveType_E", itemLeaveStDetail["LeaveType_E"].ToString());
        //                                        cmdIns.Parameters.AddWithValue("@LeaveDays", itemLeaveStDetail["LeaveDays"].ToString());
        //                                        cmdIns.Parameters.AddWithValue("@OpeningLeaveDays", "0");
        //                                        cmdIns.Parameters.AddWithValue("@IsEarned", itemLeaveStDetail["IsEarned"].ToString());
        //                                        cmdIns.Parameters.AddWithValue("@IsCompensation", itemLeaveStDetail["IsCompensation"].ToString());
        //                                        cmdIns.Parameters.AddWithValue("@Remarks", itemLeaveStDetail["Remarks"].ToString() ?? Convert.DBNull);
        //                                        cmdIns.Parameters.AddWithValue("@IsActive", true);
        //                                        cmdIns.Parameters.AddWithValue("@IsArchive", false);
        //                                        cmdIns.Parameters.AddWithValue("@CreatedBy", "Admin");
        //                                        cmdIns.Parameters.AddWithValue("@CreatedAt", "19000101");
        //                                        cmdIns.Parameters.AddWithValue("@CreatedFrom", "local");
        //                                        cmdIns.Parameters.AddWithValue("@IsCarryForward", itemLeaveStDetail["IsCarryForward"].ToString());
        //                                        cmdIns.Parameters.AddWithValue("@MaxBalance", itemLeaveStDetail["MaxBalance"].ToString());
        //                                        cmdIns.Transaction = transaction;
        //                                        cmdIns.ExecuteNonQuery();
        //                                        ELStructureId++;
        //                                        #endregion Save
        //                                    }
        //                                }
        //                            }
        //                            #endregion
        //                        }
        //                        else  //Insert
        //                        {
        //                            //Insert
        //                            #region sql statement
        //                            sqlText = @"SELECT *
        //                                from LeaveStructureDetail
        //                                where  LeaveStructureId=@LeaveStructureId ";
        //                            SqlCommand cmdLSD = new SqlCommand(sqlText, currConn, transaction);
        //                            cmdLSD.Parameters.AddWithValue("@LeaveStructureId", item["LeaveStructureId"]);
        //                            SqlDataAdapter daLSD = new SqlDataAdapter(cmdLSD);
        //                            DataTable dtLSD = new DataTable();
        //                            daLSD.Fill(dtLSD);
        //                            foreach (DataRow dr in dtLSD.Rows)
        //                            {
        //                                #region Save
        //                                sqlText = "  ";
        //                                sqlText += @"   INSERT INTO EmployeeLeaveStructure(
        //                                            Id,EmployeeId,LeaveStructureId,LeaveYear,LeaveType_E,LeaveDays,OpeningLeaveDays,IsEarned,IsCompensation,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,IsCarryForward,MaxBalance         
        //                                            ) VALUES (
        //                                            @Id,@EmployeeId,@LeaveStructureId,@LeaveYear,@LeaveType_E,@LeaveDays,@OpeningLeaveDays,@IsEarned,@IsCompensation,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@IsCarryForward,@MaxBalance
        //                                            )  ";
        //                                SqlCommand cmdIns = new SqlCommand(sqlText, currConn, transaction);
        //                                cmdIns.Parameters.AddWithValue("@Id", (ELStructureId + 1));
        //                                cmdIns.Parameters.AddWithValue("@EmployeeId", EmployeeId);
        //                                cmdIns.Parameters.AddWithValue("@LeaveStructureId", item["LeaveStructureId"]);
        //                                cmdIns.Parameters.AddWithValue("@LeaveYear", item["LeaveYear"]);
        //                                cmdIns.Parameters.AddWithValue("@LeaveType_E", dr["LeaveType_E"].ToString());
        //                                cmdIns.Parameters.AddWithValue("@LeaveDays", dr["LeaveDays"].ToString());
        //                                cmdIns.Parameters.AddWithValue("@OpeningLeaveDays", "0");
        //                                cmdIns.Parameters.AddWithValue("@IsEarned", dr["IsEarned"].ToString());
        //                                cmdIns.Parameters.AddWithValue("@IsCompensation", dr["IsCompensation"].ToString());
        //                                cmdIns.Parameters.AddWithValue("@Remarks", dr["Remarks"].ToString() ?? Convert.DBNull);
        //                                cmdIns.Parameters.AddWithValue("@IsActive", true);
        //                                cmdIns.Parameters.AddWithValue("@IsArchive", false);
        //                                cmdIns.Parameters.AddWithValue("@CreatedBy", "Admin");
        //                                cmdIns.Parameters.AddWithValue("@CreatedAt", "19000101");
        //                                cmdIns.Parameters.AddWithValue("@CreatedFrom", "local");
        //                                cmdIns.Parameters.AddWithValue("@IsCarryForward", dr["IsCarryForward"].ToString());
        //                                cmdIns.Parameters.AddWithValue("@MaxBalance", dr["MaxBalance"].ToString());
        //                                cmdIns.ExecuteNonQuery();
        //                                ELStructureId++;
        //                                #endregion Save
        //                            }
        //                            #endregion
        //                        }
        //                        #endregion Save

        //                    }
        //                }
        //                else
        //                {
        //                    retResults[1] = "Unexpected Error!";
        //                    throw new ArgumentNullException("Unexpected Error!", "");
        //                }
        //                dt.Columns.Remove("LeaveStructureId");
        //                #endregion Save
        //                #region Commit
        //                if (Vtransaction == null && transaction != null)
        //                {
        //                    transaction.Commit();
        //                }
        //                #endregion Commit
        //                #region SuccessResult
        //                retResults[0] = "Success";
        //                retResults[1] = "Data Save Successfully!";
        //                retResults[2] = Id.ToString();
        //                #endregion SuccessResult
        //            }
        //            #endregion try
        //            #region Catch and Finall
        //            catch (Exception ex)
        //            {
        //                retResults[4] = ex.Message.ToString(); //catch ex
        //                try { transaction.Rollback(); }
        //                catch (Exception) { return retResults; }
        //                return retResults;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();
        //                }
        //            }
        //            #endregion
        //            #region Results
        //            return retResults;
        //            #endregion
        //        }

        //          public string[] InsertEmployeePFXXX(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //        {

        //            string sqlText = "";
        //            int Id = 0;

        //            string PFStructureId = "";
        //            string[] conditionFields = new string[10];
        //            string[] conditionValues = new string[10];
        //            string[] retResults = new string[6];
        //            retResults[0] = "Fail";//Success or Fail
        //            retResults[1] = "Fail";// Success or Fail Message
        //            retResults[2] = Id.ToString();// Return Id
        //            retResults[3] = sqlText; //  SQL Query
        //            retResults[4] = "ex"; //catch ex
        //            retResults[5] = "InsertPF"; //Method Name
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;

        //            CommonDAL _cDal = new CommonDAL();


        //            try
        //            {



        //                if (VcurrConn != null)
        //                {
        //                    currConn = VcurrConn;
        //                }
        //                if (Vtransaction != null)
        //                {
        //                    transaction = Vtransaction;
        //                }

        //                if (currConn == null)
        //                {
        //                    currConn = _dbsqlConnection.GetConnection();
        //                    if (currConn.State != ConnectionState.Open)
        //                    {
        //                        currConn.Open();
        //                    }
        //                }
        //                if (transaction == null)
        //                {
        //                    transaction = currConn.BeginTransaction("");
        //                }



        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    foreach (DataRow item in dt.Rows)
        //                    {
        //                        DataTable returnDt = new DataTable();



        //                        string EmployeeId = item["EmployeeId"].ToString().Trim();
        //                        string Name = item["EmployeeName"].ToString();
        //                        string Designation = item["Designation"].ToString();
        //                        string PFStructure = item["PfStructureName"].ToString().Trim();
        //                        string PFValue = item["PFValue"].ToString().Trim();
        //                        string IsFixed = item["IsFixed"].ToString().Trim();
        //                        string PortionSalaryType = item["PortionSalaryType"].ToString().Trim();


        //                        #region Finding PfStructureId Using pfstructure
        //                        returnDt = _cDal.SelectByCondition("PFStructure", "Name", PFStructure, currConn, transaction);
        //                        if (returnDt != null && returnDt.Rows.Count > 0)
        //                        {
        //                            PFStructureId = returnDt.Rows[0]["Id"].ToString();
        //                        }
        //                        else
        //                        {
        //                            retResults[1] = "ID Not Found for " + PFStructure;
        //                            throw new ArgumentNullException(retResults[1], "");
        //                        }
        //                        #endregion Finding DepartmentId Using Departmen

        //                        if (string.IsNullOrWhiteSpace(EmployeeId))
        //                        {
        //                            retResults[1] = "Employee Id Can't Be Blank!";
        //                            throw new ArgumentNullException(retResults[1], "");
        //                        }


        //                        object exeRes;


        ////                        sqlText = "  ";
        ////                        sqlText += @" update  EmployeePF set 
        ////                            PFStructureId=@PFStructureId
        ////                            ,PFValue=@PFValue
        ////                            ,IsFixed=@IsFixed
        ////                            ,PortionSalaryType=@PortionSalaryType
        ////                            where EmployeeId=@EmployeeId   ";
        ////                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
        ////                        cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);

        ////                        cmdUpdate.Parameters.AddWithValue("@PFStructureId", PFStructureId);
        ////                        cmdUpdate.Parameters.AddWithValue("@PFValue", item["PFValue"]);
        ////                        cmdUpdate.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
        ////                        cmdUpdate.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);
        ////                        cmdUpdate.ExecuteNonQuery();

        //                        sqlText = @"Select count(Id) from EmployeePF 
        //                                where EmployeeId=@EmployeeId";
        //                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn, transaction);
        //                        cmdExist.Parameters.AddWithValue("@EmployeeId", EmployeeId);

        //                        exeRes = cmdExist.ExecuteScalar();
        //                        int alreadyExist = Convert.ToInt32(exeRes);
        //                        sqlText = "Select MAX(Id+1)Id from EmployeePF";
        //                        SqlCommand cmdId = new SqlCommand(sqlText, currConn, transaction);
        //                        exeRes = cmdId.ExecuteScalar();
        //                        int PFId = 0; // default value in case exeRes is null or DBNull
        //                        if (!Convert.IsDBNull(exeRes))
        //                        {
        //                            PFId = Convert.ToInt32(exeRes);
        //                        }

        //                        if (alreadyExist > 0)
        //                        {
        //                            sqlText = @"SELECT *
        //                                from EmployeePf
        //                                where  PFStructureId=@PFStructureId ";
        //                            SqlCommand cmdLSD2 = new SqlCommand(sqlText, currConn, transaction);
        //                            cmdLSD2.Parameters.AddWithValue("@PFStructureId", item["PFStructureId"]);
        //                            DataTable dtEPF = new DataTable();
        //                            SqlDataAdapter da1 = new SqlDataAdapter(cmdLSD2);
        //                            da1.Fill(dtEPF);
        //                            if (dtEPF.Rows.Count > 0)
        //                            {
        //                                foreach (DataRow itemEPF in dtEPF.Rows)
        //                                {

        //                                    //Update
        //                                    sqlText = @"SELECT *
        //                                FROM EmployeeEPF
        //                                WHERE  
        //                                EmployeeId=@EmployeeId
        //                                ";
        //                                    SqlCommand cmdEmpPF= new SqlCommand(sqlText, currConn, transaction);
        //                                    cmdEmpPF.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);

        //                        cmdEmpPF.Parameters.AddWithValue("@PFStructureId", PFStructureId);
        //                        cmdEmpPF.Parameters.AddWithValue("@PFValue", item["PFValue"]);
        //                        cmdEmpPF.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
        //                        cmdEmpPF.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);
        //                        cmdEmpPF.ExecuteNonQuery();
        //                                    DataTable dtEmpPF = new DataTable();
        //                                    SqlDataAdapter daEmpPF= new SqlDataAdapter(cmdEmpPF);
        //                                    daEmpPF.Fill(dtEmpPF);
        //                                    if (dtEPF.Rows.Count > 0)
        //                                    {
        //                                        // Update
        //                                        string pfStructureId = dtEmpPF.Rows[0]["Id"].ToString();
        //                                        #region Update Setting
        //                                        sqlText = "";
        //                                        sqlText += @" update  EmployeePF set 
        //                            PFStructureId=@PFStructureId
        //                            ,PFValue=@PFValue
        //                            ,IsFixed=@IsFixed
        //                            ,PortionSalaryType=@PortionSalaryType
        //                            where EmployeeId=@EmployeeId   ";
        //                        SqlCommand cmdPFUpdate = new SqlCommand(sqlText, currConn, transaction);
        //                        cmdPFUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);

        //                        cmdPFUpdate.Parameters.AddWithValue("@PFStructureId", PFStructureId);
        //                        cmdPFUpdate.Parameters.AddWithValue("@PFValue", item["PFValue"]);
        //                        cmdPFUpdate.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
        //                        cmdPFUpdate.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);
        //                        cmdPFUpdate.ExecuteNonQuery();
        //                        #endregion Update Settings
        //                                    }
        //                       else{
        //                        sqlText = "";
        //                        sqlText += @" INSERT INTO EmployeePF(Id,
        //EmployeeId,PFStructureId,PFValue,IsFixed,
        //PortionSalaryType,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
        //VALUES (@Id,@EmployeeId,@PFStructureId,@PFValue,@IsFixed,
        //@PortionSalaryType,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";

        //                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
        //                        cmdInsert.Parameters.AddWithValue("@Id", (PFId + 1));
        //                        cmdInsert.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
        //                        cmdInsert.Parameters.AddWithValue("@PFStructureId", PFStructureId);
        //                        cmdInsert.Parameters.AddWithValue("@PFValue", item["PFValue"]);
        //                        cmdInsert.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
        //                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);
        //                        cmdInsert.Parameters.AddWithValue("@IsActive", 1);
        //                        cmdInsert.Parameters.AddWithValue("@IsArchive", 0);
        //                        cmdInsert.Parameters.AddWithValue("@CreatedBy", "Admin");
        //                        cmdInsert.Parameters.AddWithValue("@CreatedAt", "19000101");
        //                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", "local");
        //                        cmdInsert.ExecuteNonQuery();
        //                        PFId++;

        //                    }
        //                  }
        //               }
        //       }        
        //else{
        //    #region sql statement
        //    sqlText = @"SELECT *
        //                                from EmployeePF
        //                                where  PFStructureId=@PFStructureId ";
        //    SqlCommand cmdEPD = new SqlCommand(sqlText, currConn, transaction);
        //    cmdEPD.Parameters.AddWithValue("@PFStructureId", item["PFStructureId"]);
        //    SqlDataAdapter daEPD = new SqlDataAdapter(cmdEPD);
        //    DataTable dtEPD = new DataTable();
        //    daEPD.Fill(dtEPD);
        //    foreach (DataRow dr in dtEPD.Rows)
        //    {
        //        #region Save
        //        sqlText = "  ";
        //        sqlText += @"  INSERT INTO EmployeePF(Id,
        //EmployeeId,PFStructureId,PFValue,IsFixed,
        //PortionSalaryType,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
        //VALUES (@Id,@EmployeeId,@PFStructureId,@PFValue,@IsFixed,
        //@PortionSalaryType,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";

        //        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
        //        cmdInsert.Parameters.AddWithValue("@Id", (PFId + 1));
        //        cmdInsert.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
        //        cmdInsert.Parameters.AddWithValue("@PFStructureId", PFStructureId);
        //        cmdInsert.Parameters.AddWithValue("@PFValue", item["PFValue"]);
        //        cmdInsert.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
        //        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);
        //        cmdInsert.Parameters.AddWithValue("@IsActive", 1);
        //        cmdInsert.Parameters.AddWithValue("@IsArchive", 0);
        //        cmdInsert.Parameters.AddWithValue("@CreatedBy", "Admin");
        //        cmdInsert.Parameters.AddWithValue("@CreatedAt", "19000101");
        //        cmdInsert.Parameters.AddWithValue("@CreatedFrom", "local");
        //        cmdInsert.ExecuteNonQuery();
        //        PFId++;

        //        #endregion Save
        //    }
        //    #endregion


        //}
        //                    }
        //                         }



        //                else
        //                {
        //                    retResults[1] = "Unexpected Error!";
        //                    throw new ArgumentNullException("Unexpected Error!", "");
        //                }

        //                if (Vtransaction == null)
        //                {
        //                    if (transaction != null)
        //                    {
        //                        transaction.Commit();
        //                    }
        //                }

        //                retResults[0] = "Success";
        //                retResults[1] = "Data Save Successfully";

        //            }


        //            catch (Exception ex)
        //            {
        //                retResults[4] = ex.Message.ToString(); //catch ex
        //                try { transaction.Rollback(); }
        //                catch (Exception) { return retResults; }
        //                return retResults;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();
        //                }
        //            }

        //            return retResults;

        //        }

        public string[] InsertEmployeePF(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {

            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string PFStructureId = "";
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeePF"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
            bool alreadyExist = false;

            try
            {


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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                object exeRes;
                sqlText = "Select (isnull(MAX(Id),0)+1)Id from EmployeePF";
                SqlCommand cmdId = new SqlCommand(sqlText, currConn, transaction);
                exeRes = cmdId.ExecuteScalar();
                int PFId = 0; // default value in case exeRes is null or DBNull
                if (!Convert.IsDBNull(exeRes))
                {
                    PFId = Convert.ToInt32(exeRes);
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {

                        DataTable returnDt = new DataTable();
                        string EmployeeId = item["EmployeeId"].ToString().Trim();
                        string Name = item["EmployeeName"].ToString();
                        string Designation = item["Designation"].ToString();
                        string PFStructure = item["PfStructureName"].ToString().Trim();
                        string PFValue = item["PFValue"].ToString().Trim();
                        string IsFixed = item["IsFixed"].ToString().Trim();
                        string PortionSalaryType = item["PortionSalaryType"].ToString().Trim();



                        #region Finding EmployeeId Using EmployeeId
                        returnDt = _cDal.SelectByCondition("EmployeePF", "EmployeeId", EmployeeId, currConn, transaction);

                        #endregion Finding EmployeeId Using EmployeeId

                        alreadyExist = returnDt != null && returnDt.Rows.Count > 0;





                        returnDt = _cDal.SelectByCondition("PFStructure", "Name", PFStructure, currConn, transaction);
                        if (returnDt != null && returnDt.Rows.Count > 0)
                        {
                            PFStructureId = returnDt.Rows[0]["Id"].ToString();
                        }
                        else
                        {
                            retResults[1] = "ID Not Found for " + PFStructure;
                            throw new ArgumentNullException(retResults[1], "");
                        }


                        if (alreadyExist)
                        {

                            sqlText = "";
                            sqlText = "update EmployeePF set";
                            //sqlText += " EmployeeId=@EmployeeId,";
                            sqlText += " PFStructureId=@PFStructureId,";
                            sqlText += " [PFValue]=@[PFValue],";

                            sqlText += " IsFixed=@IsFixed,";
                            sqlText += " PortionSalaryType=@PortionSalaryType,";
                            sqlText += " IsActive=@IsActive,";
                            sqlText += " IsArchive=@IsArchive,";
                            //sqlText += " CreatedBy=@CreatedBy,";
                            //sqlText += " CreatedAt=@CreatedAt,";
                            //sqlText += " CreatedFrom=@CreatedFrom,";


                            sqlText += " where EmployeeId=@EmployeeId";
                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

                            cmdUpdate.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                            cmdUpdate.Parameters.AddWithValue("@PFStructureId", PFStructureId);
                            cmdUpdate.Parameters.AddWithValue("@PFValue", item["PFValue"]);
                            cmdUpdate.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
                            cmdUpdate.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);


                            cmdUpdate.Transaction = transaction;
                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            retResults[2] = transResult.ToString();// Return Id
                            retResults[3] = sqlText; //  SQL Query

                        }

                        else
                        {

                            sqlText = "";
                            sqlText += @" INSERT INTO EmployeePF(Id,
EmployeeId,PFStructureId,PFValue,IsFixed,
PortionSalaryType,IsActive,IsArchive) 
VALUES (@Id,@EmployeeId,@PFStructureId,@PFValue,@IsFixed,
@PortionSalaryType,@IsActive,@IsArchive) SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@Id", PFId);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"]);
                            cmdInsert.Parameters.AddWithValue("@PFStructureId", PFStructureId);
                            cmdInsert.Parameters.AddWithValue("@PFValue", item["PFValue"]);
                            cmdInsert.Parameters.AddWithValue("@IsFixed", item["IsFixed"]);
                            cmdInsert.Parameters.AddWithValue("@PortionSalaryType", item["PortionSalaryType"]);
                            cmdInsert.Parameters.AddWithValue("@IsActive", 1);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", 0);

                            cmdInsert.ExecuteNonQuery();
                            PFId++;

                        }
                    }


                }


                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
                }

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully";

            }


            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                try { transaction.Rollback(); }
                catch (Exception) { return retResults; }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            return retResults;

        }


        public string[] InsertEmployeeLeaveStructure(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] conditionFields = new string[10];
            string[] conditionValues = new string[10];
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertLeaveStructure"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            CommonDAL _cDal = new CommonDAL();


            try
            {




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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }


                dt.Columns.Add("LeaveStructureId", typeof(string));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataTable leaveStDt = _cDal.SelectByCondition("LeaveStructure", "Name", dt.Rows[i]["LeaveStructureName"].ToString(), currConn, transaction);
                    if (leaveStDt != null && leaveStDt.Rows.Count > 0)
                    {
                        dt.Rows[i]["LeaveStructureId"] = leaveStDt.Rows[0]["Id"].ToString();
                    }
                }

                DataTable dtTemp = new DataTable();
                dtTemp = EmployeeStructureGroup(dt, currConn, transaction);
                dt = new DataTable();
                dt = dtTemp;

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {


                        string EmployeeId = item["EmployeeId"].ToString().Trim();

                        if (string.IsNullOrWhiteSpace(EmployeeId))
                        {
                            retResults[1] = "Employee Id Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }



                        EmployeeId = item["EmployeeId"].ToString();





                        object exeRes;

                        sqlText = "  ";
                        sqlText += @" update  EmployeeStructureGroup set 
                            LeaveStructureId=@LeaveStructureId
                            
                            where EmployeeId=@EmployeeId   ";
                        SqlCommand cmdSGUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdSGUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdSGUpdate.Parameters.AddWithValue("@LeaveStructureId", item["LeaveStructureId"]);
                        cmdSGUpdate.ExecuteNonQuery();

                        sqlText = @"Select count(Id) from EmployeeLeaveStructure 
                                where EmployeeId=@EmployeeId and LeaveYear=@LeaveYear
                            ";
                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn, transaction);
                        cmdExist.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdExist.Parameters.AddWithValue("@LeaveYear", item["LeaveYear"]);
                        exeRes = cmdExist.ExecuteScalar();
                        int alreadyExist = Convert.ToInt32(exeRes);
                        sqlText = "Select MAX(isnull(Id,0)+1)Id from EmployeeLeaveStructure";
                        SqlCommand cmdId = new SqlCommand(sqlText, currConn, transaction);
                        exeRes = cmdId.ExecuteScalar();
                        int ELStructureId = Convert.ToInt32(exeRes);
                        if (alreadyExist > 0)
                        {
                            ////Update

                            sqlText = @"SELECT *
                                from LeaveStructureDetail
                                where  LeaveStructureId=@LeaveStructureId ";
                            SqlCommand cmdLSD2 = new SqlCommand(sqlText, currConn, transaction);
                            cmdLSD2.Parameters.AddWithValue("@LeaveStructureId", item["LeaveStructureId"]);
                            DataTable dtLeaveStDetail = new DataTable();
                            SqlDataAdapter da1 = new SqlDataAdapter(cmdLSD2);
                            da1.Fill(dtLeaveStDetail);
                            if (dtLeaveStDetail.Rows.Count > 0)
                            {
                                foreach (DataRow itemLeaveStDetail in dtLeaveStDetail.Rows)
                                {
                                    var tt = itemLeaveStDetail["LeaveType_E"].ToString();
                                    //Update
                                    sqlText = @"SELECT *
                                FROM EmployeeLeaveStructure
                                WHERE  
                                EmployeeId=@EmployeeId AND 
                                LeaveYear=@LeaveYear AND
                                LeaveType_E=@LeaveType_E
                                ";
                                    SqlCommand cmdEmplLeaveSt = new SqlCommand(sqlText, currConn, transaction);
                                    cmdEmplLeaveSt.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                                    cmdEmplLeaveSt.Parameters.AddWithValue("@LeaveYear", item["LeaveYear"]);
                                    cmdEmplLeaveSt.Parameters.AddWithValue("@LeaveType_E", itemLeaveStDetail["LeaveType_E"].ToString());
                                    DataTable dtEmpLeaveSt = new DataTable();
                                    SqlDataAdapter daEmpLeaveSt = new SqlDataAdapter(cmdEmplLeaveSt);
                                    daEmpLeaveSt.Fill(dtEmpLeaveSt);
                                    if (dtEmpLeaveSt.Rows.Count > 0)
                                    {
                                        // Update
                                        string EmployeeLeaveStructureId = dtEmpLeaveSt.Rows[0]["Id"].ToString();

                                        sqlText = "";
                                        sqlText = "update EmployeeLeaveStructure set";
                                        sqlText += " LeaveDays=@LeaveDays,";
                                        sqlText += " IsEarned=@IsEarned,";
                                        sqlText += " IsCompensation=@IsCompensation,";
                                        sqlText += " where Id=@Id";
                                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                                        cmdUpdate.Parameters.AddWithValue("@Id", EmployeeLeaveStructureId);
                                        cmdUpdate.Parameters.AddWithValue("@LeaveDays", itemLeaveStDetail["LeaveDays"].ToString());
                                        cmdUpdate.Parameters.AddWithValue("@IsEarned", itemLeaveStDetail["IsEarned"].ToString());
                                        cmdUpdate.Parameters.AddWithValue("@IsCompensation", itemLeaveStDetail["IsCompensation"].ToString());
                                        cmdUpdate.ExecuteNonQuery();

                                    }
                                    else
                                    {

                                        sqlText = "  ";
                                        sqlText += @"   INSERT INTO EmployeeLeaveStructure(
                                            Id,EmployeeId,LeaveStructureId,LeaveYear,LeaveType_E,LeaveDays,OpeningLeaveDays,IsEarned,IsCompensation,Remarks,IsActive,IsArchive,IsCarryForward,MaxBalance         
                                            ) VALUES (
                                            @Id,@EmployeeId,@LeaveStructureId,@LeaveYear,@LeaveType_E,@LeaveDays,@OpeningLeaveDays,@IsEarned,@IsCompensation,@Remarks,@IsActive,@IsArchive,@IsCarryForward,@MaxBalance
                                            )  ";
                                        SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                                        cmdIns.Parameters.AddWithValue("@Id", (ELStructureId + 1));
                                        cmdIns.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                                        cmdIns.Parameters.AddWithValue("@LeaveStructureId", item["LeaveStructureId"]);
                                        cmdIns.Parameters.AddWithValue("@LeaveYear", item["LeaveYear"]);
                                        cmdIns.Parameters.AddWithValue("@LeaveType_E", itemLeaveStDetail["LeaveType_E"].ToString());
                                        cmdIns.Parameters.AddWithValue("@LeaveDays", itemLeaveStDetail["LeaveDays"].ToString());
                                        cmdIns.Parameters.AddWithValue("@OpeningLeaveDays", "0");
                                        cmdIns.Parameters.AddWithValue("@IsEarned", itemLeaveStDetail["IsEarned"].ToString());
                                        cmdIns.Parameters.AddWithValue("@IsCompensation", itemLeaveStDetail["IsCompensation"].ToString());
                                        cmdIns.Parameters.AddWithValue("@Remarks", itemLeaveStDetail["Remarks"].ToString() ?? Convert.DBNull);
                                        cmdIns.Parameters.AddWithValue("@IsActive", true);
                                        cmdIns.Parameters.AddWithValue("@IsArchive", false);
                                        cmdIns.Parameters.AddWithValue("@IsCarryForward", itemLeaveStDetail["IsCarryForward"].ToString());
                                        cmdIns.Parameters.AddWithValue("@MaxBalance", itemLeaveStDetail["MaxBalance"].ToString());
                                        cmdIns.Transaction = transaction;
                                        cmdIns.ExecuteNonQuery();
                                        ELStructureId++;

                                    }
                                }
                            }

                        }
                        else  //Insert
                        {
                            //Insert

                            sqlText = @"SELECT *
                                from LeaveStructureDetail
                                where  LeaveStructureId=@LeaveStructureId ";
                            SqlCommand cmdLSD = new SqlCommand(sqlText, currConn, transaction);
                            cmdLSD.Parameters.AddWithValue("@LeaveStructureId", item["LeaveStructureId"]);
                            SqlDataAdapter daLSD = new SqlDataAdapter(cmdLSD);
                            DataTable dtLSD = new DataTable();
                            daLSD.Fill(dtLSD);
                            foreach (DataRow dr in dtLSD.Rows)
                            {


                                sqlText = "  ";
                                sqlText += @"   INSERT INTO EmployeeLeaveStructure(
                                            Id,EmployeeId,LeaveStructureId,LeaveYear,LeaveType_E,LeaveDays,OpeningLeaveDays,IsEarned,IsCompensation,Remarks,IsActive,IsArchive,IsCarryForward,MaxBalance         
                                            ) VALUES (
                                            @Id,@EmployeeId,@LeaveStructureId,@LeaveYear,@LeaveType_E,@LeaveDays,@OpeningLeaveDays,@IsEarned,@IsCompensation,@Remarks,@IsActive,@IsArchive,@IsCarryForward,@MaxBalance
                                            )  ";
                                SqlCommand cmdIns = new SqlCommand(sqlText, currConn, transaction);
                                cmdIns.Parameters.AddWithValue("@Id", (ELStructureId + 1));
                                cmdIns.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                                cmdIns.Parameters.AddWithValue("@LeaveStructureId", item["LeaveStructureId"]);
                                cmdIns.Parameters.AddWithValue("@LeaveYear", item["LeaveYear"]);
                                cmdIns.Parameters.AddWithValue("@LeaveType_E", dr["LeaveType_E"].ToString());
                                cmdIns.Parameters.AddWithValue("@LeaveDays", dr["LeaveDays"].ToString());
                                cmdIns.Parameters.AddWithValue("@OpeningLeaveDays", "0");
                                cmdIns.Parameters.AddWithValue("@IsEarned", dr["IsEarned"].ToString());
                                cmdIns.Parameters.AddWithValue("@IsCompensation", dr["IsCompensation"].ToString());
                                cmdIns.Parameters.AddWithValue("@Remarks", dr["Remarks"].ToString() ?? Convert.DBNull);
                                cmdIns.Parameters.AddWithValue("@IsActive", true);
                                cmdIns.Parameters.AddWithValue("@IsArchive", false);
                                cmdIns.Parameters.AddWithValue("@IsCarryForward", dr["IsCarryForward"].ToString());
                                cmdIns.Parameters.AddWithValue("@MaxBalance", dr["MaxBalance"].ToString());
                                cmdIns.ExecuteNonQuery();
                                ELStructureId++;

                            }

                        }

                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
                }
                dt.Columns.Remove("LeaveStructureId");


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }


                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully!";
                retResults[2] = Id.ToString();

            }



            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                try { transaction.Rollback(); }
                catch (Exception) { return retResults; }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }



            return retResults;


        }

        public DataTable EmployeeStructureGroup(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            string sqlText = "";
            int Id = 0;
            string[] retResults = { "Fail", "Fail", Id.ToString(), sqlText, "ex", "InsertPersonalDetail" };
            //0 - Success or Fail//1 - Success or Fail Message//2 - Return Id//3 - SQL Query //4 - catch ex//5 - Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            try
            {

                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }



                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {

                        bool exist = _cDal.ExistCheck("EmployeeStructureGroup", "EmployeeId", item["EmployeeId"].ToString(), currConn, transaction);
                        if (!exist)
                        {
                            #region EmployeeStructureGroup Insert
                            int empStGroupId = _cDal.NextId("EmployeeStructureGroup", currConn, transaction);
                            sqlText = "  ";
                            sqlText += @" INSERT INTO EmployeeStructureGroup( EmployeeId,IsGross,EmployeeGroupId,LeaveStructureId,SalaryStructureId,PFStructureId
                            ,TaxStructureId,BonusStructureId,Remarks,IsActive,IsArchive
                            ) 
                            VALUES ( @EmployeeId,1,'1_1','1_1','1_1','1_1','1_1','1_1',@Remarks,@IsActive,@IsArchive
                            ) ";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                            cmdInsert.Parameters.AddWithValue("@Id", empStGroupId);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", item["EmployeeId"].ToString());
                            cmdInsert.Parameters.AddWithValue("@Remarks", "");
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                            cmdInsert.ExecuteNonQuery();
                            #endregion EmployeeStructureGroup Insert
                        }

                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
                }

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }


                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully!";
                retResults[2] = Id.ToString();

            }


            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                try { transaction.Rollback(); }
                catch (Exception) { return dt; }
                return dt;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }


            return dt;

        }

        public string[] InsertDepartment(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertDepartment"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        CommonDAL cdal = new CommonDAL();
                        bool check = false;

                        string Code = item["Code"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Employee Code Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        string tableName = "Department";
                        string[] fieldName = { "Code", "Name" };
                        string[] fieldValue = { item["Code"].ToString().Trim(), item["Name"].ToString().Trim() };
                        for (int i = 0; i < fieldName.Length; i++)
                        {
                            check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], 1, currConn, transaction);
                            if (check == true)
                            {
                                retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                                throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                            }
                        }
                        #endregion Required Field


                        #region Save
                        sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from Department where BranchId=@BranchId";
                        SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                        cmd2.Parameters.AddWithValue("@BranchId", 1);
                        cmd2.Transaction = transaction;
                        var exeRes = cmd2.ExecuteScalar();
                        int count = Convert.ToInt32(exeRes);
                        string ID = 1 + "_" + (count + 1);
                        //int foundId = (int)objfoundId;

                        sqlText = "  ";
                        sqlText += @" INSERT INTO Department(Id,BranchId,Code,Name,Remarks,IsActive,IsArchive) 
                                VALUES (@Id,@BranchId,@Code,@Name,@Remarks,@IsActive,@IsArchive) 
                                        ";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Transaction = transaction;

                        cmdInsert.Parameters.AddWithValue("@Id", ID);
                        cmdInsert.Parameters.AddWithValue("@BranchId", "1");
                        cmdInsert.Parameters.AddWithValue("@Code", item["Code"].ToString().Trim());
                        cmdInsert.Parameters.AddWithValue("@Name", item["Name"].ToString().Trim());
                        cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.ExecuteNonQuery();
                        #endregion Sql Execution

                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update EmployeeLanguage.";
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

        public string[] InsertDesignation(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertDesignation"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        CommonDAL cdal = new CommonDAL();
                        bool check = false;

                        string Code = item["Code"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Designation Code Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        string tableName = "Designation";
                        string[] fieldName = { "Code", "Name" };
                        string[] fieldValue = { item["Code"].ToString().Trim(), item["Name"].ToString().Trim() };
                        for (int i = 0; i < fieldName.Length; i++)
                        {
                            check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], 1, currConn, transaction);
                            if (check == true)
                            {
                                retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                                throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                            }
                        }
                        #endregion Required Field


                        #region Save
                        sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from Designation where BranchId=@BranchId";
                        SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                        cmd2.Parameters.AddWithValue("@BranchId", 1);

                        cmd2.Transaction = transaction;
                        var exeRes = cmd2.ExecuteScalar();
                        int count = Convert.ToInt32(exeRes);
                        string ID = 1 + "_" + (count + 1);
                        //int foundId = (int)objfoundId;

                        sqlText = "  ";
                        sqlText += @" INSERT INTO Designation(Id,BranchId,DesignationGroupId,Code,Name,Remarks,IsActive,IsArchive) 
                                VALUES (@Id,@BranchId,@DesignationGroupId,@Code,@Name,@Remarks,@IsActive,@IsArchive) 
                                        ";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Transaction = transaction;

                        cmdInsert.Parameters.AddWithValue("@Id", ID);
                        cmdInsert.Parameters.AddWithValue("@BranchId", "1");
                        cmdInsert.Parameters.AddWithValue("@DesignationGroupId", "1_10");

                        cmdInsert.Parameters.AddWithValue("@Code", item["Code"].ToString().Trim());
                        cmdInsert.Parameters.AddWithValue("@Name", item["Name"].ToString().Trim());
                        cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                        cmdInsert.ExecuteNonQuery();
                        #endregion Sql Execution

                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update Designation";
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

        public string[] InsertBank(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertBank"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        CommonDAL cdal = new CommonDAL();
                        bool check = false;

                        string Code = item["Code"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "Bank Code Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        string tableName = "Bank";
                        string[] fieldName = { "Code", "Name" };
                        string[] fieldValue = { item["Code"].ToString().Trim(), item["Name"].ToString().Trim() };
                        for (int i = 0; i < fieldName.Length; i++)
                        {
                            check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], 1, currConn, transaction);
                            if (check == true)
                            {
                                retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                                throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                            }
                        }
                        #endregion Required Field


                        #region Save
                        sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from Designation where BranchId=@BranchId";
                        SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                        cmd2.Parameters.AddWithValue("@BranchId", 1);
                        cmd2.Transaction = transaction;
                        var exeRes = cmd2.ExecuteScalar();
                        int count = Convert.ToInt32(exeRes);
                        string ID = 1 + "_" + (count + 1);
                        //int foundId = (int)objfoundId;

                        sqlText = "  ";
                        sqlText += @" INSERT INTO Bank(Id,BranchId,Code,Name,Remarks,IsActive,IsArchive) 
                                VALUES (@Id,@BranchId,@Code,@Name,@Remarks,@IsActive,@IsArchive) 
                                        ";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Transaction = transaction;

                        cmdInsert.Parameters.AddWithValue("@Id", ID);
                        cmdInsert.Parameters.AddWithValue("@BranchId", "1");
                        cmdInsert.Parameters.AddWithValue("@Code", item["Code"].ToString().Trim());
                        cmdInsert.Parameters.AddWithValue("@Name", item["Name"].ToString().Trim());
                        cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);


                        cmdInsert.ExecuteNonQuery();
                        #endregion Sql Execution

                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update Bank";
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

        public string[] InsertDesignationGroup(DataTable dt, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertDesignationGroup"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Required Field
                        CommonDAL cdal = new CommonDAL();
                        bool check = false;

                        string Code = item["Code"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(Code))
                        {
                            retResults[1] = "DesignationGroup Code Can't Be Blank!";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                        string tableName = "DesignationGroup";
                        string[] fieldName = { "Code", "Name" };
                        string[] fieldValue = { item["Code"].ToString().Trim(), item["Name"].ToString().Trim() };
                        for (int i = 0; i < fieldName.Length; i++)
                        {
                            check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], 1, currConn, transaction);
                            if (check == true)
                            {
                                retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                                throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                            }
                        }
                        #endregion Required Field


                        #region Save
                        sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from DesignationGroup where BranchId=@BranchId";
                        SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                        cmd2.Parameters.AddWithValue("@BranchId", 1);
                        cmd2.Transaction = transaction;
                        var exeRes = cmd2.ExecuteScalar();
                        int count = Convert.ToInt32(exeRes);
                        string ID = 1 + "_" + (count + 1);
                        //int foundId = (int)objfoundId;

                        var SL = _cDal.NextId("DesignationGroup", currConn, transaction);

                        sqlText = "  ";
                        sqlText += @" INSERT INTO DesignationGroup(Id,Serial,BranchId,Code,Name,Remarks,IsActive,IsArchive) 
                                VALUES (@Id,@Serial,@BranchId,@Code,@Name,@Remarks,@IsActive,@IsArchive) 
                                        ";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Transaction = transaction;

                        cmdInsert.Parameters.AddWithValue("@Id", ID);
                        cmdInsert.Parameters.AddWithValue("@BranchId", "1");
                        cmdInsert.Parameters.AddWithValue("@Serial", item["Serial"].ToString().Trim());
                        cmdInsert.Parameters.AddWithValue("@Code", item["Code"].ToString().Trim());
                        cmdInsert.Parameters.AddWithValue("@Name", item["Name"].ToString().Trim());
                        cmdInsert.Parameters.AddWithValue("@Remarks", item["Remarks"] ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);


                        cmdInsert.ExecuteNonQuery();
                        #endregion Sql Execution

                    }
                }
                else
                {
                    retResults[1] = "Unexpected Error!";
                    throw new ArgumentNullException("Unexpected Error!", "");
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
                        retResults[1] = "Unexpected error to update DesignationGroup";
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

        public string[] InsertPFJournalTemp(GLJournalDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertBank"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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

                sqlText = "  ";
                sqlText += @" INSERT INTO tempGLJournalDetails(
                             GLJournalId
                            ,COAName
                            ,TransactionDate
                            ,TransactionType
                            ,JournalType
                            ,IsDr
                            ,DrAmount
                            ,CrAmount
                            ,Remarks
                            ,TransType
                            ,IsYearClosing
                            ) 
                            VALUES (
                            @GLJournalId
                            ,@COAName
                            ,@TransactionDate
                            ,@TransactionType
                            ,@JournalType
                            ,@IsDr
                            ,@DrAmount
                            ,@CrAmount
                            ,@Remarks
                            ,@TransType
                            ,@IsYearClosing) ";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@GLJournalId", vm.GLJournalId);
                cmdInsert.Parameters.AddWithValue("@COAName", vm.COAName);
                cmdInsert.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
                cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                cmdInsert.Parameters.AddWithValue("@JournalType", vm.JournalType);
                cmdInsert.Parameters.AddWithValue("@IsDr", vm.IsDr);
                cmdInsert.Parameters.AddWithValue("@DrAmount", vm.DrAmount);
                cmdInsert.Parameters.AddWithValue("@CrAmount", vm.CrAmount);
                cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks);
                cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType);
                cmdInsert.Parameters.AddWithValue("@IsYearClosing", vm.IsYearClosing);
                cmdInsert.ExecuteNonQuery();

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
                        retResults[1] = "Unexpected error to update Bank";
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

        public DataTable GetAccName(string AccName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            string[] retResults = new string[6];
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();
            try
            {
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                sqlText = @"
                    SELECT  *
                    FROM COAs where Name=@Name";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@Name", AccName);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully!";
            }
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                try { transaction.Rollback(); }
                catch (Exception) { return dt; }
                return dt;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            return dt;
        }

        public DataTable GetAccHeader(SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            string[] retResults = new string[6];
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();
            try
            {
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                sqlText = @"SELECT 
                        [GLJournalId] [Id]
                        ,'JV-001/' [Code]    
                        ,[TransactionDate]                     
                        ,SUM([DrAmount]) [TransactionValue]   
                        ,[Remarks] 
                        FROM [EGCB_PF_Test].[dbo].[tempGLJournalDetails]
                        where DrAmount>1
                        Group by 
                        [GLJournalId]	
                        ,[TransactionDate]
                        ,[JournalType]
                        ,[TransactionType]
                        ,[IsDr],[Remarks]  
                        ";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);               
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully!";
            }
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                try { transaction.Rollback(); }
                catch (Exception) { return dt; }
                return dt;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            return dt;
        }

        public string[] InsertHeader(GLJournalVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertBank"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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

                sqlText = "  ";
                sqlText += @" INSERT INTO GLJournals(
                           [Code]
                          ,[TransactionDate]
                          ,[JournalType]
                          ,[TransactionType]
                          ,[TransactionValue]
                          ,[Remarks]
                          ,[IsActive]
                          ,[IsArchive]
                          ,[CreatedBy]
                          ,[CreatedAt]
                          ,[CreatedFrom]                         
                          ,[Post]
                          ,[TransType]
                          ,[IsYearClosing]
                            ) 
                            VALUES (
                           @Code
                          ,@TransactionDate
                          ,@JournalType
                          ,@TransactionType
                          ,@TransactionValue
                          ,@Remarks
                          ,@IsActive
                          ,@IsArchive
                          ,@CreatedBy
                          ,@CreatedAt
                          ,@CreatedFrom                        
                          ,@Post
                          ,@TransType
                          ,@IsYearClosing)";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                cmdInsert.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
                cmdInsert.Parameters.AddWithValue("@JournalType", vm.JournalType);
                cmdInsert.Parameters.AddWithValue("@TransactionType",vm.TransactionType);
                cmdInsert.Parameters.AddWithValue("@TransactionValue",vm.TransactionValue);
                cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks);
                cmdInsert.Parameters.AddWithValue("@IsActive", vm.IsActive);
                cmdInsert.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);               
                cmdInsert.Parameters.AddWithValue("@Post", vm.Post);
                cmdInsert.Parameters.AddWithValue("@TransType",vm.TransType);
                cmdInsert.Parameters.AddWithValue("@IsYearClosing",vm.IsYearClosing);              
              
                cmdInsert.ExecuteNonQuery();

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
                        retResults[1] = "Unexpected error to update Bank";
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

        public DataTable GetAccDetailsTemp(int GLJournalId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            string[] retResults = new string[6];
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();
            try
            {
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                sqlText = @"SELECT 
                        *
                        FROM tempGLJournalDetails
                         where GLJournalId=@GLJournalId                      
                        ";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@GLJournalId", GLJournalId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }               
            }
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                try { transaction.Rollback(); }
                catch (Exception) { return dt; }
                return dt;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            return dt;
        }

        public string[] InsertPFJournalDetails(GLJournalDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int transResult = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertBank"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //EmployeeInfoVM vm = new EmployeeInfoVM();
            CommonDAL _cDal = new CommonDAL();
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
                              
                vm.Id = _cDal.NextId("GLJournals", currConn, transaction);

                sqlText = "  ";
                sqlText += @"  INSERT INTO GLJournalDetails (
                         GLJournalId
                        ,COAId
                        ,IsDr
                        ,DrAmount
                        ,CrAmount
                        ,TransactionDate
                        ,TransactionType
                        ,JournalType
                        ,Remarks
                        ,TransType
                        ,IsYearClosing
                        ) VALUES (
                        @GLJournalId
                        ,@COAId
                        ,@IsDr
                        ,@DrAmount
                        ,@CrAmount
                        ,@TransactionDate
                        ,@TransactionType
                        ,@JournalType
                        ,@Remarks
                        ,@TransType
                        ,@IsYearClosing
                        ) ";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@GLJournalId", vm.Id);
                cmdInsert.Parameters.AddWithValue("@COAId", vm.COAId);
                cmdInsert.Parameters.AddWithValue("@IsDr", vm.IsDr);
                cmdInsert.Parameters.AddWithValue("@DrAmount", vm.DrAmount);
                cmdInsert.Parameters.AddWithValue("@CrAmount", vm.CrAmount);
                cmdInsert.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
                cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                cmdInsert.Parameters.AddWithValue("@JournalType", vm.JournalType);
                cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks);
                cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType);
                cmdInsert.Parameters.AddWithValue("@IsYearClosing", vm.IsYearClosing);              

                cmdInsert.ExecuteNonQuery();

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
                        retResults[1] = "Unexpected error to update Bank";
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
        public DataTable SelectDepartmentInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @" SELECT BranchId,Code,Name,OrderNo,Remarks
                FROM Department";
                #endregion
                #region More Conditions
                #endregion
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
        public DataTable SelectAssetInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"SELECT Code, Name, Remarks
                            FROM Asset";
                #endregion
                #region More Conditions


                #endregion


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



        public DataTable SelectDesignationGroupInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"SELECT Serial,BranchId,Code,Name,Remarks
                            FROM DesignationGroup";
                #endregion
                #region More Conditions
                #endregion
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


        public DataTable SelectBranchInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @" SELECT 
	                         CompanyId,
	                         Code,
	                         Name,
	                         Address,
	                         District,
	                         Division,
	                         Country,
	                         City,
	                         PostalCode,
	                         Phone,
	                         Mobile
	                         ,Fax
	                         ,Remarks
                         FROM Branch";
                #endregion
                #region More Conditions
                #endregion
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

        public DataTable SelectSectionInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"SELECT 
	                       BranchId
                          ,Code
                          ,Name
                          ,Remarks
                          ,OrderNo
                          FROM Section";
                #endregion
                #region More Conditions
                #endregion
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

        
        public DataTable SelectGradeInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"SELECT 
	                       SL
                          ,BranchId
                          ,Code
                          ,Name
                          ,MinSalary
                          ,MaxSalary
                          ,Remarks
                          FROM Grade";
                #endregion
                #region More Conditions
                #endregion
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

     
        public DataTable SelectLeaveTypeInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"SELECT 
	                       Name                         
                          ,IsWithoutPay
                          ,LType
                          ,Remarks
                          FROM EnumLeaveType";
                #endregion
                #region More Conditions
                #endregion
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

        public DataTable SelectBankInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"SELECT 
	                           BranchId
                              ,Code
                              ,Name
                              ,Remarks
                          FROM Bank";
                #endregion
                #region More Conditions
                #endregion
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

        public DataTable SelectProjectInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"SELECT 
	                              BranchId
                                  ,Code
                                  ,Name
                                  ,Startdate
                                  ,EndDate
                                  ,ManpowerRequired
                                  ,ContactPerson
                                  ,ContactPersonDesignation
                                  ,Address
                                  ,District
                                  ,Division
                                  ,Country
                                  ,City
                                  ,PostalCode
                                  ,Phone
                                  ,Mobile
                                  ,Fax
                                  ,Remarks
                          FROM Project";
                #endregion
                #region More Conditions
                #endregion
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

        public DataTable SelectDesignationInfo(ExportImportVM VM)
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
                #region sqlText
                sqlText = @"SELECT 
	                              BranchId
                                  ,Code
                                  ,Name
                                  ,Remarks
                          FROM Designation";
                #endregion
                #region More Conditions
                #endregion
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
