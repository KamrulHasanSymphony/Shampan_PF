using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class SetupRepo
    {
        SetupDAL _setupDAL = new SetupDAL();


        #region New Methods


        public string[] InsertToSetupNew(SetupMaster setupMaster)
        {
             string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            try
            {
                retResults = _setupDAL.InsertToSetupNew(setupMaster);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        

            return retResults;
        }

        public string SearchSetupNew(string databaseName)
        {

            string sqlReturn = string.Empty;
            try
            {
                sqlReturn = _setupDAL.SearchSetupNew(databaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
             

            return sqlReturn;
        }


        public DataTable SearchSetupDataTable(string databaseName)
        {
            DataTable dataTable = new DataTable("Setup");
            try
            {
                dataTable = _setupDAL.SearchSetupDataTable(databaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        
            return dataTable;
        }

        #endregion
    }
}
