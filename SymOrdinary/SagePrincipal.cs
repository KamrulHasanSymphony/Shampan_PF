using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SymOrdinary
{
   public class SagePrincipal : IPrincipal
    {
       public SagePrincipal()
       {

       }
       public SagePrincipal(SageIdentity identity)
        {
            this.Identity = identity;
        }
        public IIdentity Identity { get; private set; }

        public bool IsInRole(string roleId)
        {
            return ((SageIdentity)this.Identity).PermittedRoles.Any(x => x.Contains(roleId));
        }
    }
}
