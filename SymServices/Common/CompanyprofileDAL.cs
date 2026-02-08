using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using SymViewModel.Common;
using SymOrdinary;
using SymphonySofttech.Utilities;



namespace SymServices.Common
{
    public class CompanyprofileDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
       
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

        #region New Methods

        public string[] UpdateCompanyProfileNew(CompanyProfileVM companyProfiles, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(companyProfiles.CompanyName))
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Please enter company name.");
                }
                if (string.IsNullOrEmpty(companyProfiles.CompanyLegalName))
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Please enter company legal name.");
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

                #region CompanyProfile existence checking by id

                //select @Present = count(VehicleID) from Vehicles where VehicleID = @VehicleID;
                sqlText = "select count(CompanyID) from CompanyProfiles where CompanyID = '" + companyProfiles.CompanyID + "'";
                SqlCommand vhclIDExist = new SqlCommand(sqlText, currConn);
                vhclIDExist.Transaction = transaction;
				var exeRes = vhclIDExist.ExecuteScalar();
				countId = Convert.ToInt32(exeRes);
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateCompanyProfileNew", "Could not find requested Company Information.");
                }

                #endregion CompanyProfile existence checking by id

                #region companyProfiles existence checking by id and requied field

                //sqlText = "select count(CompanyName) from CompanyProfiles ";
                //sqlText += " where  CompanyID='" + companyProfiles.CompanyID + "'";
                //sqlText += " and CompanyName='" + companyProfiles.CompanyName + "'";
                //SqlCommand vhclNoExist = new SqlCommand(sqlText, currConn);
                //vhclNoExist.Transaction = transaction;
                //countId = (int)vhclNoExist.ExecuteScalar();
                //if (countId > 0)
                //{
                //    throw new ArgumentNullException("UpdateCompanyProfileNew", "Same company profile already used.");
                //}

                #endregion companyProfiles existence checking by id and requied field

                #region Update company profile

                sqlText = "";
                sqlText = sqlText + "  update CompanyProfiles set ";
                //sqlText = sqlText + " CompanyID='" + NewCompanyID + "',";//"@CompanyName,";
                sqlText = sqlText + " CompanyName='" + companyProfiles.CompanyName + "',";//"@CompanyName,";
                sqlText = sqlText + " CompanyLegalName='" + companyProfiles.CompanyLegalName + "',";//CompanyLegalName,";
                sqlText = sqlText + " Address1='" + companyProfiles.Address1 + "',";//Address1,";
                sqlText = sqlText + " Address2='" + companyProfiles.Address2 + "',";//Address2,";
                sqlText = sqlText + " Address3='" + companyProfiles.Address3 + "',";//Address3,";
                sqlText = sqlText + " City='" + companyProfiles.City + "',";//City,";
                sqlText = sqlText + " ZipCode='" + companyProfiles.ZipCode + "',";//ZipCode,";
                sqlText = sqlText + " TelephoneNo='" + companyProfiles.TelephoneNo + "',";//TelephoneNo,";
                sqlText = sqlText + " FaxNo='" + companyProfiles.FaxNo + "',";//FaxNo,";
                sqlText = sqlText + " Email='" + companyProfiles.Email + "',";//Email,";
                sqlText = sqlText + " ContactPerson='" + companyProfiles.ContactPerson + "',";//ContactPerson,";
                sqlText = sqlText + " ContactPersonDesignation='" + companyProfiles.ContactPersonDesignation + "',";//ContactPersonDesignation,";
                sqlText = sqlText + " ContactPersonTelephone='" + companyProfiles.ContactPersonTelephone + "',";//ContactPersonTelephone,";
                sqlText = sqlText + " ContactPersonEmail='" + companyProfiles.ContactPersonEmail + "',";//ContactPersonEmail,";
                sqlText = sqlText + " TINNo='" + companyProfiles.TINNo + "',";//TINNo,";
                sqlText = sqlText + " VatRegistrationNo='" + companyProfiles.VatRegistrationNo + "',";//VatRegistrationNo,";
                sqlText = sqlText + " Comments='" + companyProfiles.Comments + "',";//Comments,";
                sqlText = sqlText + " ActiveStatus='" + companyProfiles.ActiveStatus + "',";//ActiveStatus,";
                sqlText = sqlText + " LastModifiedBy='" + companyProfiles.LastModifiedBy + "',";//LastModifiedBy,";
                sqlText = sqlText + " LastModifiedOn='" + companyProfiles.LastModifiedOn + "'";//LastModifiedOn,";
                //sqlText = sqlText + " FYearStart='" + companyProfiles.FYearStart + "',";//FYearStart,";
                //sqlText = sqlText + " FYearEnd='" + companyProfiles.FYearEnd + "'";//FYearEnd
                if (!string.IsNullOrEmpty(companyProfiles.Tom))
                {
                    sqlText = sqlText + ", Tom='" + companyProfiles.Tom + "',";//"@CompanyName,";
                    sqlText = sqlText + " Jary='" + companyProfiles.Jary + "',";//CompanyLegalName,";
                    sqlText = sqlText + " Miki='" + companyProfiles.Miki + "',";//vat no";
                    sqlText = sqlText + " Mouse='" + companyProfiles.Mouse + "'";//processor id";
                }
                sqlText = sqlText + " where CompanyID='" + companyProfiles.CompanyID + "'";//CompanyID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
				exeRes = cmdUpdate.ExecuteNonQuery();
				transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("UpdateCompanyProfileNew", "Unable to Update Company Information ");
                }

                #region Update Sys DB Information

                string CompanyID = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyID);
                string CompanyName = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyName);
                sqlText = "";
                sqlText += " update CompanyInformations set " +
                           "CompanyName='" + CompanyName + "'" +
                           " where CompanyID='" + CompanyID + "'";
                currConn.ChangeDatabase("SymphonyVATSys");
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;

				exeRes = cmdPrefetch.ExecuteNonQuery();
				transResult = Convert.ToInt32(exeRes);
                if (transResult < 0)
                {
                    throw new ArgumentNullException("UpdateCompanyProfileNew", "Unable to Update Company Information ");

                }
                #endregion Update Sys DB Information
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                        retResults[0] = "Success";
                        retResults[1] = "Requested Company Profile Information Successfully Update.";
                        retResults[2] = "" + companyProfiles.CompanyID;

                    }
                    else
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update company profile.";
                        retResults[2] = "" + companyProfiles.CompanyID;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update company profile.";
                    retResults[2] = "" + companyProfiles.CompanyID;
                }

                #endregion Commit

                #endregion Update company profile

            }
            #region catch

            catch (SqlException sqlex)
            {
                retResults[0] = "Fail";//Success or Fail
                if (Vtransaction == null) { transaction.Rollback(); }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                if (Vtransaction == null) { transaction.Rollback(); }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return retResults;
        }

        #endregion

        #region Old Methods

        public DataTable SearchCompanyProfile()
        {
            #region Objects & Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("CProfile");
            #endregion
            #region try
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
CompanyId,
isnull(CompanyName,'N/A')CompanyName, 
isnull(CompanyLegalName,'N/A')CompanyLegalName ,
isnull(Address1,'N/A')Address1,
isnull(Address2,'N/A')Address2,
isnull(Address3,'N/A')Address3,
isnull(City,'N/A')City,
isnull(ZipCode,'N/A')ZipCode,
isnull(TelephoneNo,'N/A')TelephoneNo ,
isnull(FaxNo,'N/A')FaxNo ,
isnull(Email,'N/A')Email,
isnull(ContactPerson,'N/A')ContactPerson,
isnull(ContactPersonDesignation,'N/A')ContactPersonDesignation,
isnull(ContactPersonTelephone,'N/A')ContactPersonTelephone,
isnull(ContactPersonEmail ,'N/A')ContactPersonEmail,
isnull(VatRegistrationNo,'N/A')VatRegistrationNo,
isnull(TINNo,'N/A')TINNo,
isnull(Comments,'N/A')Comments,
isnull(ActiveStatus,'N')ActiveStatus,
convert(varchar, StartDateTime,120)StartDateTime,
convert(varchar, FYearStart,120)FYearStart,
convert(varchar, FYearEnd,120)FYearEnd

FROM  CompanyProfiles";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;

                
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {
               
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //throw ex;
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
            return dataTable;
        }

        #endregion

        public DataSet ComapnyProfileString(string CompanyID)
        {
            SqlConnection currConn = null;
            string sqlText = "";
            DataSet dataTable = new DataSet();

            try
            {
                #region open connection

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection

                sqlText = @"
SELECT 
CompanyID,
CompanyName,
CompanyLegalName,
Address1,
Address2,
Address3,
City,
ZipCode,
TelephoneNo,
FaxNo,
Email,
ContactPerson,
ContactPersonDesignation,
ContactPersonTelephone,
ContactPersonEmail,
TINNo,
VatRegistrationNo,
Comments,
ActiveStatus,
convert(varchar(200), FYearStart,120)FYearStart,
convert (varchar(200),FYearEnd,120)FYearEnd

FROM  CompanyProfiles
                 
WHERE (CompanyId  =  @CompanyId ) ;

SELECT IssueFromBOM.IssueFromBOM,PrepaidVAT.PrepaidVAT
FROM
(SELECT s.SettingValue IssueFromBOM FROM Settings s
WHERE s.SettingName='IssueFromBOM') IssueFromBOM,
(SELECT s.SettingValue PrepaidVAT FROM Settings s
WHERE s.SettingName='PrepaidVAT') PrepaidVAT;

SELECT SettingGroup,SettingName,SettingValue  FROM Settings WHERE ActiveStatus='Y' ORDER BY SettingGroup
";

                SqlCommand objCommBankInformation = new SqlCommand();
                objCommBankInformation.Connection = currConn;
                objCommBankInformation.CommandText = sqlText;
                objCommBankInformation.CommandType = CommandType.Text;



                if (!objCommBankInformation.Parameters.Contains("@CompanyID"))
                {
                    objCommBankInformation.Parameters.AddWithValue("@CompanyID", CompanyID);
                }
                else
                {
                    objCommBankInformation.Parameters["@CompanyID"].Value = CompanyID;
                }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBankInformation);
                dataAdapter.Fill(dataTable);


            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            return dataTable;

        }

        public DataSet ComapnyProfile(string CompanyID)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataSet dataSet = new DataSet("ReportVAT16");
            //DataTable dataTable = new DataTable("ReportVAT16");

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText = @"
                        SELECT 
CompanyID,
CompanyName,
CompanyLegalName,
Address1,
Address2,
Address3,
City,
ZipCode,
TelephoneNo,
FaxNo,
Email,
ContactPerson,
ContactPersonDesignation,
ContactPersonTelephone,
ContactPersonEmail,
TINNo,
VatRegistrationNo,
Comments,
ActiveStatus,
convert(varchar(200), FYearStart,120)FYearStart,
convert (varchar(200),FYearEnd,120)FYearEnd

FROM  CompanyProfiles
                 
WHERE (CompanyId  =  @CompanyId ) 

";

                #endregion

                #region SQL Command

                SqlCommand objCommVAT16 = new SqlCommand();
                objCommVAT16.Connection = currConn;

                objCommVAT16.CommandText = sqlText;
                objCommVAT16.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                objCommVAT16.CommandText = sqlText;
                objCommVAT16.CommandType = CommandType.Text;

                if (!objCommVAT16.Parameters.Contains("@CompanyID"))
                {
                    objCommVAT16.Parameters.AddWithValue("@CompanyID", CompanyID);
                }
                else
                {
                    objCommVAT16.Parameters["@CompanyID"].Value = CompanyID;
                }


                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT16);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return dataSet;
        }

        public DataSet ComapnyProfileSecurity(string CompanyID)
        {
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataSet dataTable = new DataSet();

            try
            {
                #region open connection

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection

                sqlText = @"
SELECT 
CompanyID,
CompanyName,
CompanyLegalName,
VatRegistrationNo,
Tom,
Jary,
Miki,
Mouse

FROM  CompanyProfiles
                 
WHERE (CompanyId  =  @CompanyId ) ;";


                SqlCommand objCommBankInformation = new SqlCommand();
                objCommBankInformation.Connection = currConn;
                objCommBankInformation.CommandText = sqlText;
                objCommBankInformation.CommandType = CommandType.Text;



                if (!objCommBankInformation.Parameters.Contains("@CompanyID"))
                {
                    objCommBankInformation.Parameters.AddWithValue("@CompanyID", CompanyID);
                }
                else
                {
                    objCommBankInformation.Parameters["@CompanyID"].Value = CompanyID;
                }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBankInformation);
                dataAdapter.Fill(dataTable);


            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            return dataTable;

        }
    }
}
