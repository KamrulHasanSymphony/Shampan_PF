using SymServices.Enum;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
    public class EnumReportRepo
    {
        public List<EnumReportVM> DropDown(string ReportType)
        {
            try
            {
                return new EnumReportDAL().DropDown(ReportType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EnumReportVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EnumReportDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
