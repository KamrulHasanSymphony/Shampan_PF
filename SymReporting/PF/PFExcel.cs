using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymOrdinary;
using SymServices.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymReporting.PF
{
    public class PFExcel
    {

        public PFReportVM InvestmentStatement(PFParameterVM paramVM)
        {
            PFReportVM rptVM = new PFReportVM();
            rptVM.FileName = "Investment Statement";

            try
            {

                #region Pull Data

                PFReportDAL _rptDAL = new PFReportDAL();
                DataSet ds = new DataSet();
                ds = _rptDAL.InvestmentStatement(paramVM);
                #endregion

                #region Validations

                if (ds.Tables.Count == 0)
                {
                    throw new ArgumentNullException();
                }
                #endregion

                #region Prepare Excel

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Investment");

                #endregion

                DataTable dt = new DataTable();
                DataTable dtSummary = new DataTable();

                dt = ds.Tables[0];
                dtSummary = ds.Tables[1];

                #region Report Headers, Rows, Columns

                string Line1 = "BRAC EPL STOCK BROKERAGE LIMITED - Employee Provident Fund";
                string Line2 = "Investment Statement ";
                string Line3 = "";

                string[] ReportHeaders = new string[] { "", Line1, Line2, Line3 };


                int TableHeadRow = 0;
                TableHeadRow = ReportHeaders.Length + 2;

                int RowCount = 0;
                RowCount = dt.Rows.Count;

                int ColumnCount = 0;
                ColumnCount = dt.Columns.Count;

                #endregion

                #region Summary Rows, Columns

                int SpaceRows = 3;

                string SummaryLine1 = "Summary";
                string[] SummaryReportHeaders = new string[] { "", SummaryLine1 };

                int SummaryReportHeaderRow = 0;
                SummaryReportHeaderRow = TableHeadRow + RowCount + SpaceRows + 1;

                int SummaryTableHeadRow = 0;
                SummaryTableHeadRow = SummaryReportHeaderRow + SummaryReportHeaders.Length + 1;

                int SummaryRowCount = 0;
                SummaryRowCount = dtSummary.Rows.Count;

                int SummaryColumnCount = 0;
                SummaryColumnCount = dtSummary.Columns.Count;

                #endregion

                #region Detail Load

                Ordinary.DtColumnNameSentenceCase(dt);

                workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        workSheet.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {

                        workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    }

                }

                workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 1)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };


                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Merge = true;
                    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.Font.Bold = true;
                    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.Font.Size = 14 - i;
                    workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                }

                workSheet.Row(TableHeadRow).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(TableHeadRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(TableHeadRow).Style.WrapText = true;




                #endregion

                #endregion


                #region Summary Load

                Ordinary.DtColumnNameSentenceCase(dtSummary);

                workSheet.Cells[SummaryTableHeadRow, 1].LoadFromDataTable(dtSummary, true);

                #region Format

                colNumber = 0;

                foreach (DataColumn col in dtSummary.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        workSheet.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {

                        workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    }

                }

                workSheet.Cells[SummaryTableHeadRow, 1, SummaryTableHeadRow, SummaryColumnCount].Style.Font.Bold = true;
                workSheet.Cells["A" + (SummaryTableHeadRow) + ":" + Ordinary.Alphabet[(SummaryColumnCount - 1)] + (SummaryTableHeadRow + SummaryRowCount + 1)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A" + (SummaryTableHeadRow) + ":" + Ordinary.Alphabet[(SummaryColumnCount)] + (SummaryTableHeadRow + SummaryRowCount)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };


                for (int i = 0; i < SummaryReportHeaders.Length; i++)
                {
                    workSheet.Cells[SummaryReportHeaderRow + i, 1, (SummaryReportHeaderRow + i), (ColumnCount)].Merge = true;
                    workSheet.Cells[SummaryReportHeaderRow + i, 1, (SummaryReportHeaderRow + i), (ColumnCount)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[SummaryReportHeaderRow + i, 1, (SummaryReportHeaderRow + i), (ColumnCount)].Style.Font.Bold = true;
                    workSheet.Cells[SummaryReportHeaderRow + i, 1, (SummaryReportHeaderRow + i), (ColumnCount)].Style.Font.Size = 14 - i;
                    workSheet.Cells[SummaryReportHeaderRow + i, 1].LoadFromText(SummaryReportHeaders[i], format);

                }

                workSheet.Row(SummaryTableHeadRow).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(SummaryTableHeadRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(SummaryTableHeadRow).Style.WrapText = true;




                #endregion

                #endregion





                #region MemoryStream

                rptVM.MemStream = new MemoryStream();
                excel.SaveAs(rptVM.MemStream);

                ////byte[] byteInfo = rptVM.MemStream.ToArray();
                ////rptVM.MemStream.Write(byteInfo, 0, byteInfo.Length);
                ////rptVM.MemStream.Position = 0;

                #endregion
            }
            catch (Exception)
            {
                rptVM = new PFReportVM();

            }
            finally
            {

            }
            return rptVM;
        }

    }
}
