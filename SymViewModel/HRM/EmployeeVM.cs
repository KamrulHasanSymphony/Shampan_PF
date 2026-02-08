using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SymViewModel.Leave;

namespace SymViewModel.HRM
{
    public class EmployeeVM
    {

        /// <summary>
        public string Name { get; set; }
        [Display(Name = "Father's Name")]
        public string FatherName { get; set; }
        public EmployeeVM employee { get; set; }
        public DesignationVM designationvm { get; set; }
        /// </summary>
        public string Id { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        [Display(Name = "Code")]
        public string Code { get; set; }
        [Display(Name = "Name")]
        public string FullName { get; set; }

        public string Department { get; set; }
        [Display(Name = "Transfer Date")]
        public string TransferDate { get; set; }

        public string Designation { get; set; }
        [Display(Name = "Promotion Date")]
        public string PromotionDate { get; set; }

        public string Section { get; set; }

        public string Project { get; set; }

        public string Branch { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        public string jobjoinDate { get; set; }
        public bool IsPermanent { get; set; }

        public IEnumerable<EmployeeLeaveBalanceVM> employeeLeaveBalanceVM { get; set; }
        public EmployeeLeaveVM employeeLeaveVM { get; set; }
    }

    public class CredentialModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ApiKey { get; set; }

        public string Grant_type { get; set; }

        public string PathName { get; set; }

        public CredentialModel()
        {
            Grant_type = "password";
        }
    }


    public class AuthModel
    {
        public string Access_token { get; set; }

        public string Token_type { get; set; }

        public string Expires_in { get; set; }
    }

    public class Result
    {
        public string StatusCode { get; set; }

        public string Message { get; set; }


    }

    public class RootModel<TModel> where TModel : class, new()
    {
        public string StatusCode { get; set; }

        public string Message { get; set; }
        public List<TModel> Data { get; set; }
    }
}
