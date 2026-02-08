using SymServices.Common;
using SymServices.BWWF;
using SymViewModel.Common;
using SymViewModel.BWWF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.BWWF
{
    public class PFStructureRepo
    {
        PFStructureDAL _dal = new PFStructureDAL();
        #region Methods
        public List<PFStructureVM> DropDown(int branch)
        {
            try
            {
                return _dal.DropDown(branch);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> Autocomplete(string term)
        {
            try
            {
                return _dal.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<PFStructureVM> SelectAll()
        {
            try
            {
                return new PFStructureDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public PFStructureVM SelectById(string Id)
        {
            try
            {
                return new PFStructureDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(PFStructureVM vm)
        {
            try
            {
                return new PFStructureDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(PFStructureVM vm)
        {
            try
            {
                return new PFStructureDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(PFStructureVM vm, string[] Ids)
        {
            try
            {
                return new PFStructureDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
