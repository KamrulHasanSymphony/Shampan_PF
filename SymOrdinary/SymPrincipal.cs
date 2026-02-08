using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SymOrdinary
{
    public class SymPrincipal : IPrincipal
    {

        public SymPrincipal(SymIdentity identity)
        {
            this.Identity = identity;
        }
        public IIdentity Identity { get; private set; }

        public bool IsInRole(string roleId)
        {
            return ((SymIdentity)this.Identity).PermittedRoles.Any(x => x.Contains(roleId));
        }
    }
}
