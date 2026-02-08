using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SymOrdinary
{
    public class SymIdentity : IIdentity
    {

        public string AuthenticationType { get { return "SymAuthentication"; } }
        public SymIdentity(string basicTicket)  
        {
            string[] ticketData = basicTicket.Split(new string[] { "__#" }, StringSplitOptions.None);
            this.UserId = Convert.ToInt32(ticketData[0]);
            this.BranchId = Convert.ToInt32(ticketData[1]);
            this.WorkStationIP = ticketData[2];
            this.Name = ticketData[3].Replace("^", " ");
            this.SessionDate = ticketData[4];
            this.IsAuthenticated = true;
        }
        public int UserId { get; private set; }
        public int BranchId { get; private set; }
        public string WorkStationIP { get; private set; }
        public string SessionDate { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public string Name { get; private set; }

        public string ErrorText = "";

        public string[] PermittedRoles { get; private set; }


        public static string CreateBasicTicket(
                                            int userId,
                                            int organizationId,
                                            string workStationIP,
                                            string sessionDate
            )
        {
            return userId + "__#"
                + organizationId + "__#"
                + workStationIP + "__#"
                + sessionDate + "__#"
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
