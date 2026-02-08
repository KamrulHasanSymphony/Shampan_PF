using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class UserInformationRepo
    {
        UserInformationDAL _userInformationDAL = new UserInformationDAL();

        public List<UserLogsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return _userInformationDAL.SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region New Methods

        public string[] InsertToUserInformationNew(string UserID, string UserName, string UserPassword, string ActiveStatus, string LastLoginDateTime, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName)
        {
             string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            try
            {
                retResults = _userInformationDAL.InsertToUserInformationNew(UserID, UserName, UserPassword, ActiveStatus, LastLoginDateTime, CreatedBy, CreatedOn, LastModifiedBy, LastModifiedOn, databaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retResults;
        }

        public string InsertUserLogin(string LogID, string ComputerName, string ComputerLoginUserName, string ComputerIPAddress, string SoftwareUserId, string SessionDate, string LogInDateTime, string LogOutDateTime)
        {
            string retResults = string.Empty;
            try
            {
                retResults = _userInformationDAL.InsertUserLogin(LogID, ComputerName, ComputerLoginUserName, ComputerIPAddress, SoftwareUserId, SessionDate, LogInDateTime, LogOutDateTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            
            return retResults;
        }
        public string InsertUserLogin(List<UserLogsVM> Details, string LogOutDateTime)
        {
            string retResults = string.Empty;
            try
            {
                retResults = _userInformationDAL.InsertUserLogin(Details, LogOutDateTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        

            return retResults;
        }
        public string InsertUserLogOut(List<UserLogsVM> Details, string LogOutDateTime)
        {
            string retResults = string.Empty;
            try
            {
                retResults = _userInformationDAL.InsertUserLogOut(Details, LogOutDateTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
 

            return retResults;
        }

        public string InsertUserLogOut(string LogID, string LogOutDateTime)
        {
            string retResults = string.Empty;
            try
            {
                retResults = _userInformationDAL.InsertUserLogOut(LogID, LogOutDateTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }

          

            return retResults;
        }
        public DataTable SearchUserLog(string ComputerLoginUserName, string SoftwareUserName, string ComputerName, string StartDate, string EndDate)
        {
            DataTable dataTable = new DataTable("SearchtUserLog");
            try
            {
                dataTable = _userInformationDAL.SearchUserLog(ComputerLoginUserName, SoftwareUserName, ComputerName, StartDate, EndDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }

           
            return dataTable;

        }



        public string[] UpdateToUserInformationNew(string UserID, string UserName, string UserPassword, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, string databaseName)
        {
              string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            try
            {
                retResults = _userInformationDAL.UpdateToUserInformationNew(UserID, UserName, UserPassword, ActiveStatus, LastModifiedBy, LastModifiedOn, databaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

          
            return retResults;
        }

        public string[] UpdateUserPasswordNew(string UserName, string UserPassword, string LastModifiedBy, string LastModifiedOn, string databaseName)
        {
           string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";


            try
            {
                retResults = _userInformationDAL.UpdateUserPasswordNew(UserName, UserPassword, LastModifiedBy, LastModifiedOn, databaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
 
            return retResults;

        }

        //==================Search User=================
        public DataTable SearchUserDataTable(string UserID, string UserName, string ActiveStatus, string databaseName)
        {
            DataTable dataTable = new DataTable("User Search");

            try
            {
                dataTable = _userInformationDAL.SearchUserDataTable(UserID, UserName, ActiveStatus, databaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
 
            return dataTable;

        }

 
        public DataTable SearchUserHasNew(string UserName, string databaseName)
        {
            DataTable dataTable = new DataTable("SearchUserHas");

            try
            {
                dataTable = _userInformationDAL.SearchUserHasNew(UserName, databaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataTable;

        }

        #endregion
        public Tuple<bool, UserLogsVM> UserLogIn(UserLogsVM vm)
        {
            try
            {
                return _userInformationDAL.UserLogIn(vm);
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, vm);
            }
        }
        public List<EmployeeInfoVM> SelectAllAdminUser(string Id = "", string Code = "")
        {
            try
            {
                return _userInformationDAL.SelectAllAdminUser(Id, Code);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public List<EmployeeInfoVM> SelectAllUser(string Id = null)
        {
            try
            {
                return _userInformationDAL.SelectAllUser(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public List<EmployeeInfoVM> SelectAllActiveEmp()
        {
            try
            {
                return _userInformationDAL.SelectAllActiveEmp();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public UserLogsVM SelectUserByEmployee(string employeeId = null, string UserId = null)
        {
            UserLogsVM vm = new UserLogsVM();
            try
            {
                vm = _userInformationDAL.SelectUserByEmployee(employeeId, UserId);
            }
            catch (Exception)
            {
            }
            return vm;
        }

        //==================Insert =================
        public string[] Insert(UserLogsVM vm)
        {
            try
            {
                return _userInformationDAL.Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(UserLogsVM vm)
        {
            try
            {
                return _userInformationDAL.Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(UserLogsVM vm, string[] roles)
        {
            try
            {
                return _userInformationDAL.Update(vm, roles, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ChangePassword(UserLogsVM vm)
        {
            try
            {
                return _userInformationDAL.ChangePassword(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertToUserRoll(List<UserRollVM> userRollVMs, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            try
            {
                retResults = _userInformationDAL.InsertToUserRoll(userRollVMs, databaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
			
			 
            #region Result
            return retResults;
            #endregion Result

        }

        public string SearchUserRoll(string UserID)
        {
            string encriptedData = string.Empty;
           
            try
            {
                encriptedData = _userInformationDAL.SearchUserRoll(UserID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
			 
            return encriptedData;
        }

        public DataTable SearchUserHas(string UserName)
        {
            DataTable dt = new DataTable("UserHas");

            try
            {
                dt = _userInformationDAL.SearchUserHas(UserName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }


        public List<EmployeeInfoVM> DropDown()
        {
            try
            {
                return _userInformationDAL.DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> GetUser(string deptId)
        {
            try
            {
                return _userInformationDAL.GetUser(deptId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
