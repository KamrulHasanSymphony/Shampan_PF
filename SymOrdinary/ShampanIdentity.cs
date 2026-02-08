using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SymOrdinary
{
    public class ShampanIdentity : IIdentity
    {
        public ShampanIdentity(string basicTicket)
        {
            string[] ticketData = basicTicket.Split(new string[] { "__#" }, StringSplitOptions.None);
            this.Name = ticketData[0];
            this.FullName = ticketData[1];
            this.CompanyId = ticketData[2];
            this.CompanyName = ticketData[3];
            this.BranchId = ticketData[4];
            this.BranchName = ticketData[5];
            this.WorkStationIP = ticketData[6];
            this.WorkStationName = ticketData[7];
            this.FunctionalCurrency = ticketData[8];
            this.SessionDate = ticketData[9];
            this.Site = ticketData[10];
            this.EmployeeId = ticketData[11];
            this.EmployeeCode = ticketData[12];
            this.UserId = ticketData[13];
            this.IsAdmin = Convert.ToBoolean(ticketData[14]);
            this.IsESS = Convert.ToBoolean(ticketData[15]);
            this.IsHRM = Convert.ToBoolean(ticketData[16]);
            this.IsAttenance = Convert.ToBoolean(ticketData[17]);
            this.IsPayroll = Convert.ToBoolean(ticketData[18]);
            this.IsTAX = Convert.ToBoolean(ticketData[19]);
            this.IsPF = Convert.ToBoolean(ticketData[20]);
            this.IsGF = Convert.ToBoolean(ticketData[21]);
            this.Location = ticketData[22];
            this.IsAuditor = Convert.ToBoolean( ticketData[23]);
            this.IsExpense  =  Convert.ToBoolean(ticketData[24]);
            this.IsExpenseRequisition  = Convert.ToBoolean( ticketData[25]);
            this.IsBDERequisition  = Convert.ToBoolean( ticketData[26]);
            this.HaveExpenseApproval  = Convert.ToBoolean( ticketData[27]);
            this.HaveExpenseRequisitionApproval =  Convert.ToBoolean(ticketData[28]);
            this.HaveBDERequisitionApproval  = Convert.ToBoolean( ticketData[29]);
            this.HaveApproval  =  Convert.ToBoolean(ticketData[30]);
            this.IsHO = Convert.ToBoolean(ticketData[31]);
            this.IsAccount = Convert.ToBoolean(ticketData[32]);
            this.IsBDEExpense = Convert.ToBoolean(ticketData[33]);
            this.HaveBDEExpenseApproval = Convert.ToBoolean(ticketData[34]);
            this.IsBDEAccount = Convert.ToBoolean(ticketData[35]);
            this.IsAgentCommissionExpense = Convert.ToBoolean(ticketData[36]);
            this.HaveAgentCommissionExpenseApproval = Convert.ToBoolean(ticketData[37]);
            this.IsESSEditPermission = Convert.ToBoolean(ticketData[38]);


            this.IsAuthenticated = true;
        }

        public string Name { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string AuthenticationType { get { return "ShampanAuthentication"; } }

        public string FullName { get; private set; }
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string OrganizationId { get; private set; }
        public string OrganizationName { get; private set; }
        public string BranchId { get; private set; }
        public string BranchName { get; private set; }
        public string WorkStationIP { get; private set; }
        public string WorkStationName { get; private set; }
        public string FunctionalCurrency { get; private set; }
        public string SessionDate { get; private set; }
        public string Site { get; private set; }
        public string EmployeeId { get; private set; }
        public string EmployeeCode { get; private set; }
        public string UserId { get; private set; }
        public bool IsAdmin { get; private set; }
        public bool IsESS { get; private set; }
        public bool IsHRM { get; private set; }
        public bool IsAttenance { get; private set; }
        public bool IsPayroll { get; private set; }
        public bool IsTAX { get; private set; }
        public bool IsPF { get; private set; }
        public bool IsGF { get; private set; }
        public string Location { get; private set; }
        public string[] PermittedRoles { get; private set; }


        public bool IsAuditor { get; private set; }
        public bool IsExpense { get; private set; }
        public bool IsExpenseRequisition { get; private set; }
        public bool IsBDERequisition { get; private set; }
        public bool HaveExpenseApproval { get; private set; }
        public bool HaveExpenseRequisitionApproval { get; private set; }
        public bool HaveBDERequisitionApproval { get; private set; }
        public bool HaveApproval { get; private set; }
        public bool IsHO { get; private set; }
        public bool IsAccount { get; private set; }

        public bool IsBDEExpense { get; private set; }
        public bool HaveBDEExpenseApproval { get; private set; }
        public bool IsBDEAccount { get; private set; }
        public bool IsAgentCommissionExpense { get; private set; }
        public bool HaveAgentCommissionExpenseApproval { get; private set; }
        public bool IsESSEditPermission { get; private set; }

                
        

        public static string CreateBasicTicket(
                                            string name
                                            , string fullName
                                            , string companyId
                                            , string companyName
                                            , string branchId
                                            , string branchName
                                            , string workStationIP
                                            , string workStationName
                                            , string functionalCurrency
                                            , string sessionDate
                                            , string site
                                            , string employeeId
                                            , string employeeCode
                                            , string userId
                                            , bool isAdmin
                                            , bool isESS
                                            , bool isHRM
                                            , bool isAttenance
                                            , bool isPayroll
                                            , bool isTAX
                                            , bool isPF
                                            , bool isGF
                                            , string Location
                                            , bool  isAuditor 
                                            , bool  isExpense 
                                            , bool  isExpenseRequisition 
                                            , bool  isBDERequisition 
                                            , bool  haveExpenseApproval 
                                            , bool  haveExpenseRequisitionApproval
                                            , bool  haveBDERequisitionApproval 
                                            , bool  haveApproval 
                                            , bool  isHO   
                                            , bool  isAccount   

                                            , bool  isBDEExpense   
                                            , bool  haveBDEExpenseApproval
                                            , bool isBDEAccount
                                            , bool isAgentCommissionExpense
                                            , bool haveAgentCommissionExpenseApproval
                                            , bool IsESSEditPermission   
            

            //public bool IsAuditor 
            //public bool IsExpense 
            //public bool IsExpenseRequisition 
            //public bool IsBDERequisition 
            //public bool HaveExpenseApproval 
            //public bool HaveExpenseRequisitionApproval 
            //public bool HaveBDERequisitionApproval 
            //public bool HaveApproval 
            //public bool IsHO  

            )
        {
            return name 
               + "__#" + fullName 
               + "__#" + companyId 
               + "__#" + companyName 
               + "__#" + branchId 
               + "__#" + branchName 
               + "__#" + workStationIP 
               + "__#" + workStationName 
               + "__#" + functionalCurrency 
               + "__#" + sessionDate 
               + "__#" + site 
               + "__#" + employeeId 
               + "__#" + employeeCode 
               + "__#" + userId 
               + "__#" + isAdmin 
               + "__#" + isESS 
               + "__#" + isHRM 
               + "__#" + isAttenance 
               + "__#" + isPayroll 
               + "__#" + isTAX 
               + "__#" + isPF 
               + "__#" + isGF 
               + "__#" + Location 
               + "__#" +isAuditor 

               + "__#" +isExpense 
               + "__#" +isExpenseRequisition 
               + "__#" +isBDERequisition 
               + "__#" +haveExpenseApproval 
               + "__#" +haveExpenseRequisitionApproval
               + "__#" +haveBDERequisitionApproval 
               + "__#" +haveApproval 
               + "__#" +isHO 

               + "__#" +isAccount

               + "__#" +isBDEExpense
               + "__#" +haveBDEExpenseApproval
               + "__#" + isBDEAccount
               + "__#" + isAgentCommissionExpense
               + "__#" + haveAgentCommissionExpenseApproval
               + "__#" + IsESSEditPermission

                ;
        }

        public static string CreateRoleTicket(string[] roles)
        {
            string rolesString = "";
            for (int i = 0; i < roles.Length; i++)
            {
                rolesString += roles[i] + ",";
            }
            rolesString.TrimEnd(new char[] { ',' });

            return rolesString + "__#";
        }

        public void SetRoles(string roleTicket)
        {
            this.PermittedRoles = roleTicket == "" ? new string[0] : roleTicket.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
