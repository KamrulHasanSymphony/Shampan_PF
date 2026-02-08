using SymServices.Enum;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
    public class EnumOderByRepo
    {
        EnumOderByDAL _enumOderByDAL = new EnumOderByDAL();
        public List<EnumOderByVM> DropDown(string Module)
        {
            try
            {
                return _enumOderByDAL.DropDown(Module);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
