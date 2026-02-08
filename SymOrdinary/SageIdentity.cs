using System;
using System.Security.Principal;

namespace SymOrdinary
{
    public class SageIdentity : IIdentity
    {
        public SageIdentity(string basicTicket)
        {
            string[] ticketData = basicTicket.Split(new string[] { "__#" }, StringSplitOptions.None);
            this.UserId =ticketData[0];
            this.Name = ticketData[1];
            this.IsAdmin = Convert.ToBoolean(ticketData[2]);
            this.IsAuditor = Convert.ToBoolean(ticketData[3]);
            this.IsExpense = Convert.ToBoolean(ticketData[4]);
            this.IsExpenseRequisition = Convert.ToBoolean(ticketData[5]);
            this.IsBDERequisition = Convert.ToBoolean(ticketData[6]);
            this.HaveExpenseApproval = Convert.ToBoolean(ticketData[7]);
            this.HaveExpenseRequisitionApproval = Convert.ToBoolean(ticketData[8]);
            this.HaveBDERequisitionApproval = Convert.ToBoolean(ticketData[9]);
            this.HaveApproval = Convert.ToBoolean(ticketData[10]);
            this.WorkStationIP = ticketData[11];
            this.BranchId = Convert.ToInt32(ticketData[12]);
            this.IsHO = Convert.ToBoolean(ticketData[13]);
            this.IsAuthenticated = true;
        }


        public string Name { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string AuthenticationType { get { return "SageAuthentication"; } }

        public string   UserId                          { get; private set; }
        public bool     IsAdmin                         { get; private set; }
        public bool     IsAuditor                       { get; private set; }
        public bool     IsExpense                       { get; private set; }
        public bool     IsExpenseRequisition            { get; private set; }
        public bool     IsBDERequisition                { get; private set; }
        public bool     HaveExpenseApproval             { get; private set; }
        public bool     HaveExpenseRequisitionApproval  { get; private set; }
        public bool     HaveBDERequisitionApproval      { get; private set; }
        public bool     HaveApproval              { get; private set; }
        public string   WorkStationIP { get; private set; }
        public int      BranchId { get; private set; }
        public bool     IsHO { get; private set; }
        public string[] PermittedRoles { get; private set; }

        public static string CreateBasicTicket(
                                            string  userId,
                                            string  name,
                                            bool    isAdmin                       ,
                                            bool    isAuditor                     ,
                                            bool    isExpense                     ,
                                            bool    isExpenseRequisition          ,
                                            bool    isBDERequisition              ,
                                            bool    haveExpenseApproval           ,
                                            bool    haveExpenseRequisitionApproval,
                                            bool    haveBDERequisitionApproval    ,
                                            bool    haveApproval            ,
                                            string  workStationIP,
                                            int branchId  ,                    
                                            bool isHO                      
            )
        {
            return userId + "__#"
                + name + "__#"
                +  isAdmin                        + "__#"
                +  isAuditor                      + "__#"
                +  isExpense                      + "__#"
                +  isExpenseRequisition           + "__#"
                +  isBDERequisition               + "__#"
                +  haveExpenseApproval            + "__#"
                +  haveExpenseRequisitionApproval + "__#"
                +  haveBDERequisitionApproval     + "__#"
                +  haveApproval             + "__#"
                + workStationIP + "__#"
                + branchId + "__#"
                + isHO + "__#"
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
