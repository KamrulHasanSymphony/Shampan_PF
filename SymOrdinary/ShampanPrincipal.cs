using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SymOrdinary
{
   public class ShampanPrincipal : IPrincipal
    {
       public ShampanPrincipal()
       {

       }
        public ShampanPrincipal(ShampanIdentity identity)
        {
            this.Identity = identity;
        }
        public IIdentity Identity { get; private set; }

        public bool IsInRole(string roleId)
        {
            return ((ShampanIdentity)this.Identity).PermittedRoles.Any(x => x.Contains(roleId));
        }
    }
}
