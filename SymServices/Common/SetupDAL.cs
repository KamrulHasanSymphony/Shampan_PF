using System;
using System.Data;
using System.Data.SqlClient;
using SymViewModel.Common;
using SymOrdinary;



namespace SymServices.Common
{
    public class SetupDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

        #region New Methods

       
        public string[] InsertToSetupNew(SetupMaster setupMaster)
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
                if (setupMaster != null)
                {
                    #region Validation

                    if (string.IsNullOrEmpty(setupMaster.PurchaseP))
                    {
                        throw new ArgumentNullException("InsertToSetupNew", "Please enter purchase.");
                    }
                    if (string.IsNullOrEmpty(setupMaster.SaleP))
                    {
                        throw new ArgumentNullException("InsertToSetupNew", "Please enter sale.");
                    }

                    #endregion Validation

                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    if (transaction == null) { transaction = currConn.BeginTransaction("InsertToSetupNew"); }

                    #endregion open connection and transaction

                    #region Setup existence checking by id

                    sqlText = "select count(PurchaseP) from Setup";
                    SqlCommand setupExist = new SqlCommand(sqlText, currConn);
                    setupExist.Transaction = transaction;
					var exeRes = setupExist.ExecuteScalar();
					countId = Convert.ToInt32(exeRes);
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("InsertToSetupNew", "Could not find requested setup.");
                    }

                    #endregion Setup existence checking by id

                    #region Update Setup

                    sqlText = "";
                    sqlText = "update Setup set";
                    sqlText += " PurchaseP='" + setupMaster.PurchaseP + "',";
                    sqlText += " PurchaseIDL='" + setupMaster.PurchaseIDL + "',";
                    sqlText += " PurchaseNYID='" + setupMaster.PurchaseNYID + "',";
                    sqlText += " PurchaseTradingP='" + setupMaster.PurchaseTradingP + "',";
                    sqlText += " PurchaseTradingIDL='" + setupMaster.PurchaseTradingIDL + "',";
                    sqlText += " PurchaseTradingNYID='" + setupMaster.PurchaseTradingNYID + "',";
                    sqlText += " IssueP='" + setupMaster.IssueP + "',";
                    sqlText += " IssueIDL='" + setupMaster.IssueIDL + "',";
                    sqlText += " IssueNYID='" + setupMaster.IssueNYID + "',";
                    sqlText += " IssueReturnP='" + setupMaster.IssueReturnP + "',";
                    sqlText += " IssueReturnIDL='" + setupMaster.IssueReturnIDL + "',";
                    sqlText += " IssueReturnNYID='" + setupMaster.IssueReturnNYID + "',";
                    sqlText += " ReceiveP='" + setupMaster.ReceiveP + "',";
                    sqlText += " ReceiveIDL='" + setupMaster.ReceiveIDL + "',";
                    sqlText += " ReceiveNYID='" + setupMaster.ReceiveNYID + "',";
                    sqlText += " TransferP='" + setupMaster.TransferP + "',";
                    sqlText += " TransferIDL='" + setupMaster.TransferIDL + "',";
                    sqlText += " TransferNYID='" + setupMaster.TransferNYID + "',";

                    sqlText += " SaleP='" + setupMaster.SaleP + "',";
                    sqlText += " SaleIDL='" + setupMaster.SaleIDL + "',";
                    sqlText += " SaleNYID='" + setupMaster.SaleNYID + "',";
                    sqlText += " SaleServiceP='" + setupMaster.SaleServiceP + "',";
                    sqlText += " SaleServiceIDL='" + setupMaster.SaleServiceIDL + "',";
                    sqlText += " SaleServiceNYID='" + setupMaster.SaleServiceNYID + "',";

                    sqlText += " SaleExportP='" + setupMaster.SaleExportP + "',";
                    sqlText += " SaleExportIDL='" + setupMaster.SaleExportIDL + "',";
                    sqlText += " SaleExportNYID='" + setupMaster.SaleExportNYID + "',";
                    sqlText += " SaleTradingP='" + setupMaster.SaleTradingP + "',";
                    sqlText += " SaleTradingIDL='" + setupMaster.SaleTradingIDL + "',";
                    sqlText += " SaleTradingNYID='" + setupMaster.SaleTradingNYID + "',";

                    sqlText += " SaleTenderP='" + setupMaster.SaleTenderP + "',";
                    sqlText += " SaleTenderIDL='" + setupMaster.SaleTenderIDL + "',";
                    sqlText += " SaleTenderNYID='" + setupMaster.SaleTenderNYID + "',";
                    sqlText += " DNP='" + setupMaster.DNP + "',";
                    sqlText += " DNIDL='" + setupMaster.DNIDL + "',";
                    sqlText += " DNNYID='" + setupMaster.DNNYID + "',";

                    sqlText += " CNP='" + setupMaster.CNP + "',";
                    sqlText += " CNIDL='" + setupMaster.CNIDL + "',";
                    sqlText += " CNNYID='" + setupMaster.CNNYID + "',";
                    sqlText += " DepositP='" + setupMaster.DepositP + "',";
                    sqlText += " DepositIDL='" + setupMaster.DepositIDL + "',";
                    sqlText += " DepositNYID='" + setupMaster.DepositNYID + "',";

                    sqlText += " VDSP='" + setupMaster.VDSP + "',";
                    sqlText += " VDSIDL='" + setupMaster.VDSIDL + "',";
                    sqlText += " VDSNYID='" + setupMaster.VDSNYID + "',";
                    sqlText += " TollIssueP='" + setupMaster.TollIssueP + "',";
                    sqlText += " TollIssueIDL='" + setupMaster.TollIssueIDL + "',";
                    sqlText += " TollIssueNYID='" + setupMaster.TollIssueNYID + "',";

                    sqlText += " TollReceiveP='" + setupMaster.TollReceiveP + "',";
                    sqlText += " TollReceiveIDL='" + setupMaster.TollReceiveIDL + "',";
                    sqlText += " TollReceiveNYID='" + setupMaster.TollReceiveNYID + "',";
                    sqlText += " DSFP='" + setupMaster.DSFP + "',";
                    sqlText += " DSFIDL='" + setupMaster.DSFIDL + "',";
                    sqlText += " DSFNYID='" + setupMaster.DSFNYID + "',";

                    sqlText += " DSRP='" + setupMaster.DSRP + "',";
                    sqlText += " DSRIDL='" + setupMaster.DSRIDL + "',";
                    sqlText += " DSRNYID='" + setupMaster.DSRNYID + "',";
                    sqlText += " IssueFromBOM='" + setupMaster.IssueFromBOM + "',";
                    sqlText += " PrepaidVAT='" + setupMaster.PrepaidVAT + "',";
                    sqlText += " CYear='" + setupMaster.CYear + "'";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
					exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    #region Commit

                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested Setup Information Successfully Updated.";
                            retResults[2] = "" + 1;

                        }
                        else
                        {
                            transaction.Rollback(); 
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update setup.";
                            retResults[2] = "" + 0;
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update setup information.";
                        retResults[2] = "" + 0;
                    }

                    #endregion Commit

                    #endregion Update Setup
                }
                else
                {
                    throw new ArgumentNullException("SetupMaster", "Setup is null.");
                }

            }
            #region catch

            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
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

            return retResults;
        }

        public string SearchSetupNew(string databaseName)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            string sqlReturn = string.Empty;

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
                            PurchaseP
                            ,PurchaseIDL
                            ,PurchaseNYID
                            ,PurchaseTradingP
                            ,PurchaseTradingIDL
                            ,PurchaseTradingNYID
                            ,IssueP
                            ,IssueIDL
                            ,IssueNYID
                            ,IssueReturnP
                            ,IssueReturnIDL
                            ,IssueReturnNYID
                            ,ReceiveP
                            ,ReceiveIDL
                            ,ReceiveNYID
                            ,TransferP
                            ,TransferIDL
                            ,TransferNYID
                            ,SaleP
                            ,SaleIDL
                            ,SaleNYID
                            ,SaleServiceP
                            ,SaleServiceIDL
                            ,SaleServiceNYID
                            ,SaleTradingP
                            ,SaleTradingIDL
                            ,SaleTradingNYID
                            ,SaleExportP
                            ,SaleExportIDL
                            ,SaleExportNYID
                            ,SaleTenderP
                            ,SaleTenderIDL
                            ,SaleTenderNYID
                            ,DNP
                            ,DNIDL
                            ,DNNYID
                            ,CNP
                            ,CNIDL
                            ,CNNYID
                            ,DepositP
                            ,DepositIDL
                            ,DepositNYID
                            ,VDSP
                            ,VDSIDL
                            ,VDSNYID
                            ,TollIssueP
                            ,TollIssueIDL
                            ,TollIssueNYID
                            ,TollReceiveP
                            ,TollReceiveIDL
                            ,TollReceiveNYID
                            ,DSFP
                            ,DSFIDL
                            ,DSFNYID
                            ,DSRP
                            ,DSRIDL
                            ,DSRNYID
                            ,IssueFromBOM
                            ,PrepaidVAT
                            ,CYear

                            FROM setup";

                SqlCommand objCommSearchSetup = new SqlCommand();
                objCommSearchSetup.Connection = currConn;
                objCommSearchSetup.CommandText = sqlText;
                objCommSearchSetup.CommandType = CommandType.Text;



                #endregion
            }
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

            return sqlReturn;
        }

      
        public DataTable SearchSetupDataTable(string databaseName)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Setup");

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
                                    PurchaseP
                                    ,PurchaseIDL
                                    ,PurchaseNYID
                                    ,PurchaseTradingP
                                    ,PurchaseTradingIDL
                                    ,PurchaseTradingNYID
                                    ,IssueP
                                    ,IssueIDL
                                    ,IssueNYID
                                    ,IssueReturnP
                                    ,IssueReturnIDL
                                    ,IssueReturnNYID
                                    ,ReceiveP
                                    ,ReceiveIDL
                                    ,ReceiveNYID
                                    ,TransferP
                                    ,TransferIDL
                                    ,TransferNYID
                                    ,SaleP
                                    ,SaleIDL
                                    ,SaleNYID
                                    ,SaleServiceP
                                    ,SaleServiceIDL
                                    ,SaleServiceNYID
                                    ,SaleTradingP
                                    ,SaleTradingIDL
                                    ,SaleTradingNYID
                                    ,SaleExportP
                                    ,SaleExportIDL
                                    ,SaleExportNYID
                                    ,SaleTenderP
                                    ,SaleTenderIDL
                                    ,SaleTenderNYID
                                    ,DNP
                                    ,DNIDL
                                    ,DNNYID
                                    ,CNP
                                    ,CNIDL
                                    ,CNNYID
                                    ,DepositP
                                    ,DepositIDL
                                    ,DepositNYID
                                    ,VDSP
                                    ,VDSIDL
                                    ,VDSNYID
                                    ,TollIssueP
                                    ,TollIssueIDL
                                    ,TollIssueNYID
                                    ,TollReceiveP
                                    ,TollReceiveIDL
                                    ,TollReceiveNYID
                                    ,DSFP
                                    ,DSFIDL
                                    ,DSFNYID
                                    ,DSRP
                                    ,DSRIDL
                                    ,DSRNYID
                                    ,IssueFromBOM
                                    ,PrepaidVAT
                                    ,CYear
                                    FROM setup";

                SqlCommand objCommSearchSetup = new SqlCommand();
                objCommSearchSetup.Connection = currConn;
                objCommSearchSetup.CommandText = sqlText;
                objCommSearchSetup.CommandType = CommandType.Text;

                #region param

                //no param

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSearchSetup);
                dataAdapter.Fill(dataTable);

                #endregion
            }
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


    }
}
