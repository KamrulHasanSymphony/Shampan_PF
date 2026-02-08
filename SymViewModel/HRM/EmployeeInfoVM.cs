using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SymViewModel.Payroll;
using SymViewModel.Leave;
using SymViewModel.Tax;
using SymViewModel.PF;
using SymViewModel.Loan;
using SymViewModel.Attendance;
using SymViewModel.GF;
using SymViewModel.Common;
namespace SymViewModel.HRM
{

    public class EmployeeInfoVM
    {
        public List<string> CodeList;
        public string Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        public string EmployeeId { get; set; }
        [Display(Name = "Employee Code")]
        public string Code { get; set; }
        [Display(Name = "First & Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        //public string fileName { get; set; }
         [Display(Name = "Salutation")]
        public string Salutation_E { get; set; }
        [Display(Name = "Proximity Id")]
        public string AttnUserId { get; set; }
        public string DateOfBirth { get; set; }

        public string SalaryMonth { get; set; }

        public string EmpJobType { get; set; }
        public string EmpCategory { get; set; }

        public string SupervisorEmail { get; set; }
        public string DotedLineManager { get; set; }
        public string DotedLineManagerEmail { get; set; }

        public string RetirementDate { get; set; }
        //public HttpPostedFileBase NIDF { get; set; }
        //public HttpPostedFileBase DisabilityFile { get; set; }
        //public HttpPostedFileBase PassportFile { get; set; }
        [Display(Name = "Team Lead For")]
        public string EmpName { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        public EmployeePersonalDetailVM personalDetail { get; set; }
        public EmployeeFilesVM employeeFiles { get; set; }
        public EmployeeJobVM employeeJob { get; set; }
        public EmployeeStructureGroupVM employeeSG { get; set; }
        //public EmployeeEmploymentCommencementVM employmentCommencement { get; set; }
        public EmployeeLeftInformationVM leftInformation { get; set; }
        public EmployeePermanentAddressBanglaVM permanentAddressBangla { get; set; }
        public EmployeePermanentAddressVM permanentAddress { get; set; }
        public EmployeePresentAddressBanglaVM presentAddressBangla { get; set; }
        public EmployeePresentAddressVM presentAddress { get; set; }
        public List<SingleEmployeeSalaryStructureVM> SingleEmployeeSalaryStructureVM { get; set; }
        public List<SalaryStructureMatrixVM> SalaryStructureMatrixVM { get; set; }
        public List<EmployeeSalaryStructureVM> EmployeeSalaryStructureVMs { get; set; }
        public DesignationVM designationVM { get; set; }
        public EmployeeEmergencyContactVM emergencyContactVM { get; set; }
        public EmployeeLeaveVM empleavevm { get; set; }
        public EmployeeCompensatoryLeaveVM empCompensatoryleavevm { get; set; }

        public AppraisalEvaluationVM AppraisalEvaluationVM { get; set; }
        public UserRoleForAppraisalVM UserRoleForAppraisalVM { get; set; }

        public EmployeeEducationVM educationVM { get; set; }
        public EmployeeProfessionalDegreeVM professionalDegreeVM { get; set; }
        public EmployeeLanguageVM languageVM { get; set; }
        public EmployeeImmigrationVM immigrationVM { get; set; }
        public EmployeeTrainingVM trainingVM { get; set; }
        public EmployeeTravelVM travelVM { get; set; }
        public EmployeeNomineeVM nomineeVM { get; set; }
        public EmployeeDependentVM dependentVM { get; set; }
        public EmployeeReferenceVM referenceVM { get; set; }
        public EmployeeTransferVM transferVM { get; set; }
        public EmployeePromotionVM promotionVM { get; set; }
        public EmployeeJobHistoryVM jobHistoryVM { get; set; }
        public EmployeeAssetVM AssetVM { get; set; }
        public EmployeeExtraCurriculumActivityVM extraCurriculumVM { get; set; }
        public SalaryTaxDetailVM salaryTaxDetail { get; set; }
        public SalaryPFDetailVM saPFDvm { get; set; }
        public AttLogsVM AttLogsVM { get; set; }
        #region Report
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Section { get; set; }
        public string Project { get; set; }
        public string Branch { get; set; }
        [Display(Name = "Employment Type")]
        public string EmploymentType_E { get; set; }
        public string Age { get; set; }
        [Display(Name = "Service Length")]
        public string ServiceLength { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        public string Grade { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }
        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        // training 
        public string Topics { get; set; }
        public string Institute { get; set; }
        public string Location { get; set; }
        public decimal Duration { get; set; }
        // transfer,promotion
        public string Date { get; set; }
        [Display(Name = "FromD esignation")]
        public string FromDesignation { get; set; }
        //traning ..... .....
        [Display(Name = "Date From")]
        public string DateFrom { get; set; }
        [Display(Name = "Date To")]
        public string DateTo { get; set; }
        //Use for Employee Bonus
        [Display(Name = "DOJ From")]
        public string DOJFrom { get; set; }
        [Display(Name = "DOJ To")]
        public string DOJTo { get; set; }
        // Display for
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }
        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }
        #endregion
        #region Payroll
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        [Display(Name = "Grade")]
        public string GradeId { get; set; }
        [Display(Name = "Permanent")]
        public bool IsPermanent { get; set; }
        #endregion
        #region pf and Tax calculation
        [Display(Name = "PF Amount")]
        public decimal PFValue { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxValue { get; set; }
        [Display(Name = "Earnging Amount")]
        public decimal EarngingValue { get; set; }
        [Display(Name = "Overtime Amount")]
        public decimal OvertimeValue { get; set; }
        [Display(Name = "Conveyance Amount")]
        public decimal ConveyanceValue { get; set; }
        [Display(Name = "Deduction Amount")]
        public decimal DeductionValue { get; set; }
        [Display(Name = "Arrear Amount")]
        public decimal ArrearValue { get; set; }
        [Display(Name = "Fixed / Rate")]
        public bool IsFixed { get; set; }
        [Display(Name = "Earning / Deduction")]
        public bool IsEarning { get; set; }
        [Display(Name = "Portion Salary Type")]
        public string PortionSalaryType { get; set; }
        public string PFStructureId { get; set; }
        public string TaxStructureId { get; set; }
        public EmployeeOtherEarningVM EmployeeOtherEarningVM { get; set; }
        public EmployeeLeaveEncashmentVM EmployeeLeaveEncashmentVM { get; set; }
        public IEnumerable<EmployeeOtherEarningVM> EmployeeOtherEarningVMs { get; set; }
        public IEnumerable<EmployeeLeaveEncashmentVM> EmployeeLeaveEncashmentVMs { get; set; }
        public EmployeeOtherDeductionVM EmployeeOtherDeductionVM { get; set; }
        public EmployeeEarningLeaveVM EmployeeEarningLeaveVM { get; set; }
        public IEnumerable<EmployeeLeaveBalanceVM> employeeLeaveBalanceVMs { get; set; }
        public EmployeeLeaveVM employeeLeaveVM { get; set; }
        public AttendanceStructureVM attvm { get; set; }
        public SalaryOtherEarningVM SalaryOtherEarningVM { get; set; }
        public SalaryOtherDeductionVM SalaryOtherDeductionVM { get; set; }
        public IEnumerable<SalaryEarningDetailVM> SalaryEarningVMs { get; set; }
        public IEnumerable<SalaryLoanDetailVM> SalaryLoanDetailVMs { get; set; }
        public IEnumerable<SalaryStructureDetailVM> SalaryStructureDetailVM { get; set; }
        #region PF

        public EmployeePFOpeinigVM empPFOpeinigVM { get; set; }
        public IEnumerable<EmployeePFOpeinigVM> empPFOpeinigVMs { get; set; }

        public EmployeePFForfeitureVM empPFForfeitureVM { get; set; }
        public IEnumerable<EmployeePFForfeitureVM> empPFForfeitureVMs { get; set; }

        public EmployeeBreakMonthPFVM empBreakMonthPFVM { get; set; }
        public IEnumerable<EmployeeBreakMonthPFVM> empBreakMonthPFVMs { get; set; }


        public EmployeePFPaymentVM empPFPaymentVM { get; set; }
        public IEnumerable<EmployeePFPaymentVM> empPFPaymentVMs { get; set; }

        public EmployeeTransferVM empPFForTransferVM { get; set; }
        public IEnumerable<EmployeeTransferVM> empPFForTransferVMs { get; set; }
        #endregion PF
        #region GF

        public EmployeeGFOpeinigVM empGFOpeinigVM { get; set; }
        public IEnumerable<EmployeeGFOpeinigVM> empGFOpeinigVMs { get; set; }

        public EmployeeGFForfeitureVM empGFForfeitureVM { get; set; }
        public IEnumerable<EmployeeGFForfeitureVM> empGFForfeitureVMs { get; set; }

        public EmployeeBreakMonthGFVM empBreakMonthGFVM { get; set; }
        public IEnumerable<EmployeeBreakMonthGFVM> empBreakMonthGFVMs { get; set; }


        public EmployeeGFPaymentVM empGFPaymentVM { get; set; }
        public IEnumerable<EmployeeGFPaymentVM> empGFPaymentVMs { get; set; }



        #endregion GF

        public TAX108AVM empTAX108AVM { get; set; }
        public IEnumerable<TAX108AVM> empTAX108AVMs { get; set; }


        #endregion
        #region  Salary Earning
        [Display(Name = "Salary Type")]
        public string SalaryTypeId { get; set; }
        [Display(Name = "Salary Type")]
        public string SalaryType { get; set; }
        public decimal Portion { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Interest Amount")]
        public decimal InterestAmount { get; set; }
        public string EmployeeSalaryStructureId { get; set; }
        public int FiscalYearDetailId { get; set; }
        #endregion
        #region Loan
        public string EmployeeLoanId { get; set; }
        public int EmployeeLoanDetailId { get; set; }
        [Display(Name = "Fiscal Period")]
        public string FiscalPeriod { get; set; }
        #endregion
        [Display(Name = "Left Date")]
        public string LeftDate { get; set; }
        [Display(Name = "Entry Left Date")]
        public string EntryLeftDate { get; set; }
        #region EmpProfileFull
        //public string Code          { get; set; }
        //public string EmpName       { get; set; }
        //public string Department    { get; set; }
        //public string Section       { get; set; }
        //public string JoinDate      { get; set; }
        [Display(Name = "Grade SL")]
        public string GradeSL { get; set; }
        //public string Project       { get; set; }
        //public string AttnUserId    { get; set; }
        [Display(Name = "Other Id")]
        public string OtherId { get; set; }
        [Display(Name = "Nick Name")]
        public string NickName { get; set; }
        [Display(Name = "Date Of Birth")]
        public string Religion { get; set; }
        [Display(Name = "BloodGroup")]
        public string BloodGroup_E { get; set; }
        [Display(Name = "Gender")]
        public string Gender_E { get; set; }
        [Display(Name = "MaritalStatus")]
        public string MaritalStatus_E { get; set; }
        [Display(Name = "Nationality")]
        public string Nationality_E { get; set; }
        //public string Email         { get; set; }
        public bool Smoker { get; set; }
        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; }
        [Display(Name = "Expiry Date")]
        public string ExpiryDate { get; set; }
        [Display(Name = "Trade Identification No.")]
        public string TIN { get; set; }
        [Display(Name = "National ID")]
        public string NID { get; set; }
        [Display(Name = "Disable")]
        public bool IsDisable { get; set; }
        [Display(Name = "Kinds Of Disability")]
        public string KindsOfDisability { get; set; }
        #endregion EmpProfileFull
        [Display(Name = "Code To")]
        public string CodeT { get; set; }
        [Display(Name = "Code From")]
        public string CodeF { get; set; }
        [Display(Name = "GB")]
        public bool GB { get; set; }
        [Display(Name = "FR")]
        public bool FR { get; set; }
        [Display(Name = "Step Name")]
        public string StepName { get; set; }
        [Display(Name = "Step SL")]
        public string StepSL { get; set; }
        [Display(Name = "From Department")]
        public string FromDepartment { get; set; }
        [Display(Name = "From Section")]
        public string FromSection { get; set; }
        [Display(Name = "From Project")]
        public string FromProject { get; set; }
        [Display(Name = "From Branch")]
        public string FromBranch { get; set; }
        public int Person { get; set; }
        public string PhotoName { get; set; }

        public bool IsAdmin { get; set; }

        public string Other1 { get; set; }
        public string Other2 { get; set; }
        public string Other3 { get; set; }
        public string Other4 { get; set; }
        public string Other5 { get; set; }

        public string AttendanceSystem { get; set; }
        public string ClientEmployeeIndex { get; set; }


        public string OrderByCode { get; set; }

        public List<string> DesignationList { get; set; }

        public List<string> DepartmentList { get; set; }

        public List<string> SectionList { get; set; }

        public List<string> ProjectList { get; set; }

        public List<string> Other1List { get; set; }

        public List<string> Other2List { get; set; }

        public List<string> Other3List { get; set; }

        public string Status { get; set; }

        public string EmpInfo { get; set; }

        public string JoinDateFrom { get; set; }

        public string JoinDateTo { get; set; }

        public string ReportName { get; set; }

        public string EDStructureId { get; set; }

        public string LeaveStructureId { get; set; }

        public string GroupId { get; set; }

        public string MultipleCode { get; set; }
        public string MultipleDesignation { get; set; }
        public string MultipleDepartment { get; set; }
        public string MultipleSection { get; set; }
        public string MultipleProject { get; set; }
        public string MultipleOther1 { get; set; }
        public string MultipleOther2 { get; set; }
        public string MultipleOther3 { get; set; }
        public int Year { get; set; }

        public string MultiOther3 { get; set; }



        public bool IsGross { get; set; }
        public bool IsBasic { get; set; }
        public bool IsRate { get; set; }
        //////public bool IsFixed { get; set; }
        public decimal IncrementValue { get; set; }
        public decimal IncrementAmount { get; set; }


        public string DBName { get; set; }

        public string ReportNo { get; set; }

        public string LetterName { get; set; }
         [Display(Name = "Issue Date")]
        public string IssueDate { get; set; }

        public string ReferenceNumber { get; set; }

        public string View { get; set; }

        public string AttnStatus { get; set; }
        [Display(Name = "Full OT")]
        public bool FullOT { get; set; }

        public string TransactionType { get; set; }

        public string EmploymentType { get; set; }



        public object PeriodStart { get; set; }

        public object PeriodEnd { get; set; }

        public string OrderBy { get; set; }

        public string CompanyName { get; set; }

        public string SalaryPeriodId { get; set; }
        public string TransType { get; set; }
        public string Operation { get; set; }
        public string AutoCode { get; set; }
        public string Supervisor { get; set; }


        public string NomineeName { get; set; }
        public string NRelation { get; set; }
        public string NMobile { get; set; }
        public string NDateofBirth { get; set; }
        public string NAddress { get; set; }
        public string NPostalCode { get; set; }
        public string NDistrict { get; set; }
        public string NDivision { get; set; }
        public string NCountry { get; set; }

        public string DependentName { get; set; }
        public string DRelation { get; set; }
        public string DMobile { get; set; }
        public string DDateofBirth { get; set; }
        public string DAddress { get; set; }
        public string DPostalCode { get; set; }
        public string DDistrict { get; set; }
        public string DDivision { get; set; }
        public string DCountry { get; set; }

        public bool IsPFApplicable { get; set; }
        public string MonthTo { get; set; }
        public string Password { get; set; }


        public IEnumerable<AppraisalEvaluationDetailVM> AppraisalEvaluationDetailVM { get; set; }

        public EmployeeInfoVM()
        {
            AppraisalEvaluationDetailVM = new List<AppraisalEvaluationDetailVM>();
        }


    }


    public class ViewEmployeeInfoVM
    {
        public string Id { get; set; }
        public string Code { get; set; }
        [Display(Name = "Salutation")]
        public string Salutation_E { get; set; }
        [Display(Name = "First & Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        [Display(Name = "Probation End")]
        public string ProbationEnd { get; set; }
        [Display(Name = "Date Of Permanent")]
        public string DateOfPermanent { get; set; }
        [Display(Name = "Employment Status")]
        public string EmploymentStatus { get; set; }
        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; }
        public string Project { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        [Display(Name = "Transfer Date")]
        public string TransferDate { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        [Display(Name = "Promotion")]
        public bool IsPromotion { get; set; }
        [Display(Name = "Promotion Date")]
        public string PromotionDate { get; set; }
        [Display(Name = "Project")]
        public string ProjectId { get; set; }
        [Display(Name = "Section")]
        public string SectionId { get; set; }
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }
        [Display(Name = "Designation")]
        public string DesignationId { get; set; }
        [Display(Name = "Grade")]
        public string GradeId { get; set; }
        public string BranchId { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        [Display(Name = "Proximity Id")]
        public string AttnUserId { get; set; }
        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }
        public string EmployeeId { get; set; }

        public string EmpEmail { get; set; }
        public string PFStructureId { get; set; }
        public string Password { get; set; }


        public string Other1 { get; set; }
        public string Other2 { get; set; }
        public string Other3 { get; set; }
        public string Other4 { get; set; }
        public string Other5 { get; set; }


    }

    public class ViewEmployeeInfoAllVM
    {
        #region EmployeeInfo
        public string Id { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        //public string EmployeeId { get; set; }
        [Display(Name = "Employee Code")]
        public string Code { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        //[Display(Name = "Salutation")]
        //public string Salutation { get; set; }
        //[Display(Name = "First & Middle Name")]
        //public string MiddleName { get; set; }
        //[Display(Name = "Last Name")]
        //public string LastName { get; set; }
        //public string fileName { get; set; }
        [Display(Name = "Proximity Id")]
        public string AttnUserId { get; set; }
        //public HttpPostedFileBase NIDF { get; set; }
        //public HttpPostedFileBase DisabilityFile { get; set; }
        //public HttpPostedFileBase PassportFile { get; set; }
        //[Display(Name = "Employee")]
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        #endregion EmployeeInfo
        //**Care Later**
        //public EmployeeStructureGroupVM employeeSG { get; set; }
        //public DesignationVM designationVM { get; set; }
        #region Dependent
        //public EmployeeDependentVM dependentVM { get; set; }
        [Required]
        [Display(Name = "Dependent Name")]
        public string DependentName { get; set; }
        [Required]
        [Display(Name = "Dependent Relation")]
        public string DependentRelation { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        public string DependentDateofBirth { get; set; }
        [Display(Name = "Dependent Address")]
        public string DependentAddress { get; set; }
        [Display(Name = "Dependent District")]
        public string DependentDistrict { get; set; }
        [Display(Name = "Dependent Division")]
        public string DependentDivision { get; set; }
        [Display(Name = "Dependent Country")]
        public string DependentCountry { get; set; }
        [Display(Name = "Dependent City")]
        public string DependentCity { get; set; }
        [Display(Name = "Postal Code")]
        public string DependentPostalCode { get; set; }
        [Display(Name = "Dependent Phone")]
        public string DependentPhone { get; set; }
        [Required]
        [Display(Name = "Dependent Mobile")]
        public string DependentMobile { get; set; }
        [Display(Name = "Dependent Fax")]
        public string DependentFax { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Remarks")]
        public string DependentRemarks { get; set; }
        #endregion Dependent
        #region Education
        //public EmployeeEducationVM educationVM { get; set; }
        [Required]
        [Display(Name = "Degree")]
        public string EducationDegree { get; set; }
        [Display(Name = "Education Institute")]
        public string EducationInstitute { get; set; }
        [Display(Name = "Major/Group")]
        public string EducationMajor { get; set; }
        [Required]
        [Display(Name = "Total Year")]
        public decimal EducationTotalYear { get; set; }
        [Required]
        [Display(Name = "Year Of Passing")]
        public string EducationYearOfPassing { get; set; }
        [Display(Name = "Education Result")]
        public string EducationResult { get; set; }
        [Display(Name = "Education CGPA")]
        public decimal EducationCGPA { get; set; }
        [Display(Name = "Education Scale")]
        public decimal EducationScale { get; set; }
        [Display(Name = "Marks(%)")]
        public decimal EducationMarks { get; set; }
        [Display(Name = "Last")]
        public bool IsLast { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Remarks")]
        public string EducationRemarks { get; set; }
        #endregion Education
        #region EmergencyContact
        //public EmployeeEmergencyContactVM emergencyContactVM { get; set; }
        //EmCon
        [Required]
        [Display(Name = "Emergency Contact Name")]
        public string EmConName { get; set; }
        [Required]
        [Display(Name = "Emergency Relation")]
        public string EmConRelation { get; set; }
        [Display(Name = "Emergency Address")]
        public string EmConAddress { get; set; }
        [Display(Name = "Emergency District")]
        public string EmConDistrict { get; set; }
        [Display(Name = "Emergency Division")]
        public string EmConDivision { get; set; }
        [Display(Name = "Emergency Country")]
        public string EmConCountry { get; set; }
        [Display(Name = "Emergency City")]
        public string EmConCity { get; set; }
        [Display(Name = "Postal Code")]
        public string EmConPostalCode { get; set; }
        [Display(Name = "Emergency Phone")]
        public string EmConPhone { get; set; }
        [Required]
        [Display(Name = "Emergency Mobile")]
        public string EmConMobile { get; set; }
        [Display(Name = "Emergency Fax")]
        public string EmConFax { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Remarks")]
        public string EmConRemarks { get; set; }
        #endregion EmergencyContact
        #region ExtraCurriculumActivity
        //public EmployeeExtraCurriculumActivityVM extraCurriculumVM { get; set; }
        //ExCurr
        [Required]
        public string ExCurrSkill { get; set; }
        [Required]
        [Display(Name = " Year Of Experience")]
        public decimal ExCurrYearsOfExperience { get; set; }
        [Display(Name = "Skill Quality")]
        public string ExCurrSkillQuality_E { get; set; }
        [Display(Name = "Institute")]
        public string ExCurrInstitute { get; set; }
        [Display(Name = "Address")]
        public string ExCurrAddress { get; set; }
        [Display(Name = "Date")]
        public string ExCurrDate { get; set; }
        [Display(Name = "Achievement")]
        public string ExCurrAchievement { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string ExCurrRemarks { get; set; }
        #endregion ExtraCurriculumActivity
        #region Immigration
        //public EmployeeImmigrationVM immigrationVM { get; set; }
        [Display(Name = "Immigration Type")]
        public string ImmigrationType { get; set; }
        [Display(Name = "Immigration Number")]
        [Required]
        public string ImmigrationNumber { get; set; }
        [Required]
        [Display(Name = "Issue Date")]
        public string ImmigrationIssueDate { get; set; }
        [Required]
        [Display(Name = "Expire Date")]
        public string ImmigrationExpireDate { get; set; }
        [Required]
        [Display(Name = "Issued By")]
        public string ImmigrationIssuedBy { get; set; }
        [Required]
        [Display(Name = "Eligible Review Date")]
        public string ImmigrationEligibleReviewDate { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Active")]
        public string ImmigrationRemarks { get; set; }
        #endregion Immigration
        #region Job
        //public EmployeeJobVM employeeJob { get; set; }
        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        [Display(Name = "Probation End Date")]
        public string ProbationEnd { get; set; }
        [Display(Name = "Confirm Date")]
        public string DateOfPermanent { get; set; }
        [Display(Name = "Employment Status")]
        public string EmploymentStatus { get; set; }
        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; }
        [Display(Name = "Supervisor")]
        public string Supervisor { get; set; }
        [Display(Name = "Confirmed")]
        public bool IsPermanent { get; set; }
        [Display(Name = "Structure Group")]
        public string StructureGroupId { get; set; }
        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Remarks")]
        public string JobRemarks { get; set; }
        [Display(Name = "Designation")]
        public string DesignationId { get; set; }
        [Display(Name = "Grade")]
        public string GradeId { get; set; }
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }
        [Display(Name = "Project")]
        public string ProjectId { get; set; }
        [Display(Name = "Section")]
        public string SectionId { get; set; }
        [Display(Name = "Bank Information")]
        public string BankInfo { get; set; }
        [Display(Name = "Bank Account No.")]
        public string BankAccountNo { get; set; }
        [Display(Name = "Grade")]
        public string GradeName { get; set; }
        public string Designation { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        [Display(Name = "Employment Status Name")]
        public string EmploymentStatusName { get; set; }
        [Display(Name = "Employment Type Name")]
        public string EmploymentTypeName { get; set; }
        #endregion Job
        #region JobHistory
        //public EmployeeJobHistoryVM jobHistoryVM { get; set; }
        //JH
        public string JHCompany { get; set; }
        [Display(Name = "Job Title")]
        public string JHJobTitle { get; set; }
        [Display(Name = "Job From")]
        public string JHJobFrom { get; set; }
        [Display(Name = "Job To")]
        public string JHJobTo { get; set; }
        [Display(Name = "Service Length")]
        public string JHServiceLength { get; set; }
        [StringLength(450, ErrorMessage = "Job Description cannot be longer than 40 characters.")]
        [Display(Name = "Remarks")]
        public string JHRemarks { get; set; }
        #endregion JobHistory
        #region Language
        //public EmployeeLanguageVM languageVM { get; set; }
        //Language
        [Required]
        [Display(Name = "Language")]
        public string LanguageName { get; set; }
        [Required]
        [Display(Name = "Fluency")]
        public string LanguageFluency { get; set; }
        [Required]
        [Display(Name = "Language Competency")]
        public string LanguageCompetency { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Remarks")]
        public string LanguageRemarks { get; set; }
        #endregion LanguageVM
        #region LeftInformation
        //public EmployeeLeftInformationVM leftInformation { get; set; }
        //Left
        [Display(Name = "Left Type")]
        public string LeftType { get; set; }
        [Required]
        [Display(Name = "Left Date")]
        public string LeftDate { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Remarks")]
        public string LeftRemarks { get; set; }
        #endregion LeftInformation
        #region Nominee
        //public EmployeeNomineeVM nomineeVM { get; set; }
        [Display(Name = "Nominee Name")]
        public string NomineeName { get; set; }
        [Display(Name = "Nominee Relation")]
        public string NomineeRelation { get; set; }
        [Display(Name = "Nominee Address")]
        public string NomineeAddress { get; set; }
        [Display(Name = "Nominee District")]
        public string NomineeDistrict { get; set; }
        [Display(Name = "Nominee Division")]
        public string NomineeDivision { get; set; }
        [Display(Name = "Nominee Country")]
        public string NomineeCountry { get; set; }
        [Display(Name = "Nominee City")]
        public string NomineeCity { get; set; }
        [Display(Name = "Postal Code")]
        public string NomineePostalCode { get; set; }
        [Display(Name = "Nominee Phone")]
        public string NomineePhone { get; set; }
        [Display(Name = "Date of Birth")]
        public string NomineeDateofBirth { get; set; }
        [Display(Name = "Birth Certificate / NID No.")]
        public string NomineeBirthReg { get; set; }
        [Display(Name = "Passport No.")]
        public string NomineeNomineePassport { get; set; }
        [Display(Name = "Nominee Mobile")]
        public string NomineeMobile { get; set; }
        [Display(Name = "Nominee Fax")]
        public string NomineeFax { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Remarks")]
        public string NomineeRemarks { get; set; }
        #endregion Nominee
        #region PermanentAddress
        //public EmployeePermanentAddressVM permanentAddress { get; set; }
        [StringLength(450, ErrorMessage = "Address cannot be longer than 450 characters.")]
        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; }
        [Display(Name = "Permanent District")]
        public string PermanentDistrict { get; set; }
        [Display(Name = "Permanent Division")]
        public string PermanentDivision { get; set; }
        [Display(Name = "Permanent Country")]
        public string PermanentCountry { get; set; }
        [Display(Name = "Permanent City")]
        public string PermanentCity { get; set; }
        [Display(Name = "Postal Code")]
        public string PermanentPostalCode { get; set; }
        [Display(Name = "Permanent Phone")]
        public string PermanentPhone { get; set; }
        [Display(Name = "Permanent Mobile")]
        public string PermanentMobile { get; set; }
        [Display(Name = "Permanent Fax")]
        public string PermanentFax { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Remarks")]
        public string PermanentRemarks { get; set; }
        #endregion PermanentAddress
        #region PersonalDetail
        //public EmployeePersonalDetailVM personalDetail { get; set; }
        [Display(Name = "Other ID")]
        public string OtherId { get; set; }
        [Required]
        public string Gender { get; set; }
        [Display(Name = "Marital Status")]
        [Required]
        public string MaritalStatus { get; set; }
        [Display(Name = "Nationality")]
        [Required]
        public string Nationality { get; set; }
        [Display(Name = "Date Of Birth")]
        [Required]
        public string DateOfBirth { get; set; }
        [Display(Name = "Nick Name")]
        public string NickName { get; set; }
        public bool Smoker { get; set; }
        public string NID { get; set; }
        public string NIDFile { get; set; }
        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; }
        [Display(Name = "Expiry Date")]
        public string ExpiryDate { get; set; }
        public string Religion { get; set; }
        [Display(Name = "TIN")]
        public string TIN { get; set; }
        [Display(Name = "Disability")]
        public bool IsDisable { get; set; }
        [Display(Name = "Kinds Of Disability")]
        public string KindsOfDisability { get; set; }
        //[Display(Name = "Disability File")]
        //public string DisabilityFile { get; set; }
        //public string PassportFile { get; set; }
        public string NIDFD { get; set; }
        //public string DisabilityFileName { get; set; }
        //public string PassportFileName { get; set; }
        //public string NIDFDName { get; set; }
        public string PersonalEmail { get; set; }
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Remarks")]
        public string PDRemarks { get; set; }
        #endregion PersonalDetail
        #region PresentAddress
        //public EmployeePresentAddressVM presentAddress { get; set; }
        [StringLength(450, ErrorMessage = "Address cannot be longer than 450 characters.")]
        [Display(Name = "Present Address")]
        public string PresentAddress { get; set; }
        [Display(Name = "Present District")]
        public string PresentDistrict { get; set; }
        [Display(Name = "Present Division")]
        public string PresentDivision { get; set; }
        [Display(Name = "Present Country")]
        public string PresentCountry { get; set; }
        [Display(Name = "Present City")]
        public string PresentCity { get; set; }
        [Display(Name = "Postal Code")]
        public string PresentPostalCode { get; set; }
        [Display(Name = "Present Phone")]
        public string PresentPhone { get; set; }
        [Display(Name = "Present Mobile")]
        public string PresentMobile { get; set; }
        [Display(Name = "Present Fax")]
        public string PresentFax { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string PresentRemarks { get; set; }
        #endregion PresentAddress
        #region Promotion
        //public EmployeePromotionVM promotionVM { get; set; }
        //Repeated in Employee Job
        //[Required]
        //[Display(Name = "Designation")]
        //public string DesignationId { get; set; } 
        [Required]
        [Display(Name = "Promotion")]
        public bool IsPromotion { get; set; }
        [Required]
        [Display(Name = "Promotion Date")]
        public string PromotionDate { get; set; }
        //[Display(Name = "Grade")]
        //public string GradeName { get; set; }
        //[Display(Name = "Grade")]
        //public string GradeId { get; set; }
        //public bool IsCurrent { get; set; }
        public string FileName { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string PromotionRemarks { get; set; }
        #endregion Promotion
        #region Reference
        //public EmployeeReferenceVM referenceVM { get; set; }
        [Display(Name = "Reference Name")]
        public string ReferenceName { get; set; }
        [Display(Name = "Reference Relation")]
        public string ReferenceRelation { get; set; }
        [Display(Name = "Reference Address")]
        public string ReferenceAddress { get; set; }
        [Display(Name = "Reference District")]
        public string ReferenceDistrict { get; set; }
        [Display(Name = "Reference Division")]
        public string ReferenceDivision { get; set; }
        [Display(Name = "Reference Country")]
        public string ReferenceCountry { get; set; }
        [Display(Name = "Reference City")]
        public string ReferenceCity { get; set; }
        [Display(Name = "Reference Postal Code")]
        public string ReferencePostalCode { get; set; }
        [Display(Name = "Reference Phone")]
        public string ReferencePhone { get; set; }
        [Display(Name = "Reference Mobile")]
        public string ReferenceMobile { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string ReferenceRemarks { get; set; }
        #endregion Reference
        #region Training
        //public EmployeeTrainingVM trainingVM { get; set; }
        [Display(Name = "Training Topics")]
        public string TrainingTopics { get; set; }
        [Display(Name = "Institute Name")]
        public string TrainingInstituteName { get; set; }
        [Display(Name = "Training Location")]
        public string TrainingLocation { get; set; }
        [Display(Name = "Funded By")]
        public string TrainingFundedBy { get; set; }
        [Required]
        [Display(Name = "Duration(Mon)")]
        public int TrainingDurationMonth { get; set; }
        [Required]
        [Display(Name = "Duration(Days)")]
        public int TrainingDurationDay { get; set; }
        [Display(Name = "Training Achievement")]
        public string TrainingAchievement { get; set; }
        [Display(Name = "Allowances(Tk)")]
        public decimal TrainingAllowancesTotalTk { get; set; }
        [Required]
        [Display(Name = "Training Place")]
        public string TrainingPlace { get; set; }
        [Display(Name = "Traning Time")]
        public string TraningTime { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string TrainingRemarks { get; set; }
        #endregion Training
        #region Transfer
        //public EmployeeTransferVM transferVM { get; set; }
        //**Repeated in EmployeeJob**
        //[Required]
        //[Display(Name = "Project")]
        //public string ProjectId { get; set; }
        //[Required]
        //[Display(Name = "Department")]
        //public string DepartmentId { get; set; }
        //[Required]
        //[Display(Name = "Section")]
        //public string SectionId { get; set; }
        [Required]
        [Display(Name = "Transfer Date")]
        public string TransferDate { get; set; }
        //[Required]
        //public bool IsCurrent { get; set; }
        //public string FileName { get; set; }
        //public HttpPostedFileBase FileNames { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string TransferRemarks { get; set; }
        #endregion Transfer
        #region Travel
        //public EmployeeTravelVM travelVM { get; set; }
        [Required]
        [Display(Name = "Travel Type")]
        public string TravelType { get; set; }
        [Required]
        [Display(Name = "Travel From Address")]
        public string TravelFromAddress { get; set; }
        [Required]
        [Display(Name = "Travel To Address")]
        public string TravelToAddress { get; set; }
        [Display(Name = "From Date")]
        public string TravelFromDate { get; set; }
        [Display(Name = "To Date")]
        public string TravelToDate { get; set; }
        [Required]
        [Display(Name = "From Time")]
        public string TravelFromTime { get; set; }
        [Required]
        [Display(Name = "To Time")]
        public string TravelToTime { get; set; }
        [Display(Name = "Bill/Voucher")]
        public string TravleTime { get; set; }
        [Display(Name = "Reason")]
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string TravelRemarks { get; set; }
        [Display(Name = "Allowances(TK)")]
        public decimal TravelAllowances { get; set; }
        #endregion Travel
        //public EmployeeLeaveVM empleavevm { get; set; }
        [Display(Name = "Code From")]
        public string CodeF { get; set; }
        [Display(Name = "Code To")]
        public string CodeT { get; set; }
        [Display(Name = "DOP From")]
        public string dtpFrom { get; set; }
        [Display(Name = "DOP To")]
        public string dtpTo { get; set; }
        public string Grade { get; set; }

        public string MulitpleColumn { get; set; }
    }

}
