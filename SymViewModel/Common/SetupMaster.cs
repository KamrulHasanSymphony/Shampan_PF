using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.Common
{
    public class SetupMaster
    {
        public string PurchaseP { get; set; }
        public decimal PurchaseIDL { get; set; }
        public string PurchaseNYID { get; set; }
        public string PurchaseTradingP { get; set; }
        public decimal PurchaseTradingIDL { get; set; }
        public string PurchaseTradingNYID { get; set; }
        public string IssueP { get; set; }
        public decimal IssueIDL { get; set; }
        public string IssueNYID { get; set; }
        public string IssueReturnP { get; set; }
        public decimal IssueReturnIDL { get; set; }
        public string IssueReturnNYID { get; set; }
        public string ReceiveP { get; set; }
        public decimal ReceiveIDL { get; set; }
        public string ReceiveNYID { get; set; }
        public string TransferP { get; set; }
        public decimal TransferIDL { get; set; }
        public string TransferNYID { get; set; }
        public string SaleP { get; set; }
        public decimal SaleIDL { get; set; }
        public string SaleNYID { get; set; }
        public string SaleServiceP { get; set; }
        public decimal SaleServiceIDL { get; set; }
        public string SaleServiceNYID { get; set; }
        public string SaleExportP { get; set; }
        public decimal SaleExportIDL { get; set; }
        public string SaleExportNYID { get; set; }
        public string SaleTradingP { get; set; }
        public decimal SaleTradingIDL { get; set; }
        public string SaleTradingNYID { get; set; }
        public string SaleTenderP { get; set; }
        public decimal SaleTenderIDL { get; set; }
        public string SaleTenderNYID { get; set; }
        public string DNP { get; set; }
        public decimal DNIDL { get; set; }
        public string DNNYID { get; set; }
        public string CNP { get; set; }
        public decimal CNIDL { get; set; }
        public string CNNYID { get; set; }
        public string DepositP { get; set; }
        public decimal DepositIDL { get; set; }
        public string DepositNYID { get; set; }
        public string VDSP { get; set; }
        public decimal VDSIDL { get; set; }
        public string VDSNYID { get; set; }
        public string TollIssueP { get; set; }
        public decimal TollIssueIDL { get; set; }
        public string TollIssueNYID { get; set; }
        public string TollReceiveP { get; set; }
        public decimal TollReceiveIDL { get; set; }
        public string TollReceiveNYID { get; set; }
        public string DSFP { get; set; }
        public decimal DSFIDL { get; set; }
        public string DSFNYID { get; set; }
        public string DSRP { get; set; }
        public decimal DSRIDL { get; set; }
        public string DSRNYID { get; set; }
        public string IssueFromBOM { get; set; }
        public string PrepaidVAT { get; set; }
        public string CYear { get; set; }
        public string databaseName { get; set; }
    }
}
