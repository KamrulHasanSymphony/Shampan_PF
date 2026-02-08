using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymRepository.Common
{
    public class HomePageInfoDashboardRepo
    {
       
     
        public PfInfoDashboardVM GetPfInfoDashboard()
        {
            try
            {
                return new HomePageInfoDashboardDAL().GetPfInfoDashboard();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
     
    }
}
