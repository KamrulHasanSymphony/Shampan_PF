using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class CommonVM
    {
        //public static int CreatedBy { get; set; }
        //public static string CreatedFrom { get; set; }
        //public static DateTime CreatedAt { get; set; }
        //public static int LastUpdateBy { get; set; }
        //public static string LastUpdateFrom { get; set; }
        //public static DateTime LastUpdateAt { get; set; }
        public static Color FomrBackgroundColor = Color.FromArgb(192, 192, 192);
        public static Color GridViewColor = Color.FromArgb(102, 205, 170);
    }
    public class DropDownVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string value { get; set; }
        public string Text { get; set; }
    }
    public class TicketVM
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Coach { get; set; }
        public string Seats { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        [Display(Name = "Seat Fare")]
        public string SeatFare { get; set; }
        [Display(Name = "Total Fare")]
        public string TotalFare { get; set; }
        public string PNR { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Boarding { get; set; }
        public string Dropping { get; set; }
        [Display(Name = "Issued On")]
        public string IssuedOn { get; set; }
        [Display(Name = "Issued By")]
        public string IssuedBy { get; set; }
        [Display(Name = "Serial No")]
        public string SerialNo { get; set; }
    }
    public class MyBkash
    {
        public decimal Amount { get; set; }
        [Display(Name = "TRX REF")]
        public string TRXREF { get; set; }
        [Display(Name = "TRX ID")]
        public string TRXID { get; set; }
    }
}
