using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOrdinary
{
   public class MessageVM
   {
       public static string msgDataAlreadyUsed = "The Information you want to delete already user in";
       public static string msgDeleteOperationterminated = "\nDelete Operation is terminated";

       public static string msgNegPrice = "Price can not be negative or zero";

       public static string msgSettingValueNotSave = "Settings Value Not Save, Please contact with Administrator";

       public static string msgNotPost = "Transaction not Saved, Please Save first";

       public static string msgWantToPost = "Do you want to Post this transaction?";
       public static string msgWantToPost1 = "After posted no changes will be performed";
       public static string msgNotPostAccess = "You do not have the post permission, please contact with administrator";

       public static string ThisTransactionWasPosted = "This transaction was posted";

       public static string msgWantToRemoveRow = "Do you want to Remove this transaction?";

       public static string msgNotAddAccess = "You do not have the add permission, please contact with administrator";
       public static string msgNotEditAccess = "You do not have the update permission, please contact with administrator";
       

       #region New DatabaseCreate
       public static string dbMsgMethodName = "NewDBCreate";

       public static string dbMsgNoCompanyName = "Unable to find Company Name, Please input Name";
       public static string dbMsgNoFiscalYear = "Unable to find Fiscal Year, Please input Fiscal year";
       public static string dbMsgNoCompanyInformation = "Unable to find Company Information, Please input Company Detail";
       public static string dbMsgCompanyInformationNotSave = "Unable to find Company Information";
       public static string dbMsgCFiscalYearNotSave = "Unable to find Fiscal Year Information";
       public static string dbMsgDBExist = "Database Already Exist";
       public static string dbMsgDBInfoInsert = "Database Information not save";
       public static string dbMsgDBNotCreate = "Database not create";
       public static string dbMsgTableNotCreate = "Table not create";
       public static string dbMsgTableDefaultData = "Default Data not saved";
  
       #endregion New DatabaseCreate

       public static string msgFiscalYearisLock = "Fiscal calender is locked, transaction not complete";
       public static string msgFiscalYearNotExist = "Fiscal calender not created, transaction not complete";
       public static string msgTransactionNotDeclared = "Transaction Not Declared ";

        #region FiscalYear  

       // Complete
       public static string FYMsgMethodNameInsert = "Fiscal Year Insert";
       public static string FYMsgMethodNameUpdate = "Fiscal Year Update";

       public static string FYMsgMethodNameProcess = "ExecuteYearProcess";

       public static string FYMsgAlreadyExist = "Requested Information already Added, Transaction not complete";
       public static string FYMsgPreviouseYearNotLock = " Please Lock previous year first, Transaction not complete";
       public static string FYMsgNoDataToSave = "There is no data to save";
       public static string FYMsgSaveNotSuccessfully = "Requested Information not Added";
       public static string FYMsgNotExist = "Requested Information not find, Transaction not complete";
       public static string FYMsgSaveSuccessfully = "Requested Information successfully Added";
       public static string FYMsgNoDataToUpdate = "There is no data to Update, Please Save first.";
       public static string FYMsgUpdateNotSuccessfully = "Requested Information not Update";
       public static string FYMsgUpdateSuccessfully = "Requested Information successfully Update";
       
       public static string FYMsgIDNotExist = "Requested Information not find, Transaction not complete";
       public static string FYMsgIDUpdateNotSuccessfully = "Requested Information not Update";
       public static string FYMsgIDUpdateSuccessfully = "Requested Information successfully Update";



        #endregion FiscalYear

       #region Release Note

       public static string msgAlpha =
           @"The alpha phase of the release life cycle is the first phase to begin software testing.
In this phase, developers generally test the software. It may contain known bugs, and almost certainly contains
unknown bugs. Features intended for the final release may be incomplete or missing.
Alpha software can be unstable and could cause crashes or data loss.";

       public static string msgBeta =
           @"Beta is the software development phase following alpha.
It generally begins when the software is feature complete. 
Software in the beta phase will generally have many more bugs in it than completed software, 
as well as speed/performance issues and may still cause crashes or data loss.
The focus of beta testing is reducing impacts to users, often incorporating usability testing. 
The process of delivering a beta version to the users is called beta release and this is typically 
the first time that the software is available outside of the development team.";

       public static string msgAccessPermision =
           "You do not have to access this form. Please contact with administrator";


       #endregion




   }
}
