using LanProject.Domain;
using LanProject.MainApplication.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace LanProject.MainApplication.Method
{
    public static class Output
    {
        static List<int> pager;
        static Form Source;
        static IEnumerable<XElement> gridelements;
        private static readonly string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);//LanProject.Method.Configuration.ReadSetting("dataPath")
        private static readonly string foldername = LanProject.Method.Configuration.ReadSetting("appfoldername");
        private static readonly string tempfolder = LanProject.Method.Configuration.ReadSetting("temp");
        private static readonly string notefolder = LanProject.Method.Configuration.ReadSetting("notes");
        public static string Excel(Form source, bool Is_fill_page, string producer)
        {
            try
            {
                Source = source;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string formtype = source.Formtype == 0 ? "報價單" : "請款單";
                gridelements = new List<XElement>();
                if (source.Note != "無")
                {
                    string text = File.ReadAllText(Path.Combine(programFilesPath, foldername, notefolder, String.Format("{0}.xaml", source.Note)));
                    var doc = XDocument.Load(new StringReader(text));
                    gridelements = doc.Root.Elements();
                }
                pager = new List<int>();
                if (!Directory.Exists(Path.Combine(programFilesPath, foldername, tempfolder)))
                    Directory.CreateDirectory(Path.Combine(programFilesPath, foldername, tempfolder));
                var file = new FileInfo(Path.Combine(programFilesPath, foldername, tempfolder, String.Format("{0}.xlsx", source.ID))); // 檔案路徑
                using (var excel = new ExcelPackage())
                {
                    // 建立分頁
                    var ws = excel.Workbook.Worksheets.Add(String.Format("{0}_{1}", source.ID, formtype));
                    ws.PrinterSettings.Orientation = eOrientation.Landscape;
                    ws.PrinterSettings.PaperSize = ePaperSize.A4;
                    ws.PrinterSettings.TopMargin = (decimal)1.0 / 2.54M; // narrow border
                    ws.PrinterSettings.LeftMargin = (decimal).5 / 2.54M; // narrow border
                    ws.PrinterSettings.RightMargin = (decimal).5 / 2.54M; // narrow border
                    ws.PrinterSettings.BottomMargin = (decimal)1.0 / 2.54M; // narrow border

                    ws.Column(1).Width = GetTrueColumnWidth(8);
                    ws.Column(2).Width = GetTrueColumnWidth(7);
                    ws.Column(3).Width = GetTrueColumnWidth(7);
                    ws.Column(4).Width = GetTrueColumnWidth(6);
                    ws.Column(5).Width = GetTrueColumnWidth(6);
                    ws.Column(6).Width = GetTrueColumnWidth(6);
                    ws.Column(7).Width = GetTrueColumnWidth(6);
                    ws.Column(8).Width = GetTrueColumnWidth(7);
                    ws.Column(9).Width = GetTrueColumnWidth(7);
                    ws.Column(10).Width = GetTrueColumnWidth(6);
                    ws.Column(11).Width = GetTrueColumnWidth(6);
                    ws.Column(12).Width = GetTrueColumnWidth(9);
                    ws.Column(13).Width = GetTrueColumnWidth(9);
                    ws.Column(14).Width = GetTrueColumnWidth(6);
                    ws.Column(15).Width = GetTrueColumnWidth(6);
                    ws.Column(16).Width = GetTrueColumnWidth(6);
                    ws.Column(17).Width = GetTrueColumnWidth(6);
                    ws.Column(18).Width = GetTrueColumnWidth(8);
                    ws.Column(19).Width = GetTrueColumnWidth(8);

                    double rowHeight = 20;
                    ws.Row(1).Height = rowHeight;
                    ws.Row(2).Height = rowHeight;
                    ws.Row(3).Height = rowHeight;

                    ws.Cells[1, 1].Value = source.Myunit.Name;
                    ws.Cells[1, 1, 3, 12].Merge = true;
                    ws.Cells[1, 1, 3, 12].Style.Font.SetFromFont("標楷體", 36, true);
                    ws.Cells[1, 1, 3, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Distributed;
                    ws.Cells[1, 1, 3, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[1, 1, 3, 12].Style.Indent = 3;

                    ws.Cells[2, 13].Value = formtype;
                    ws.Cells[2, 13, 3, 16].Merge = true;
                    ws.Cells[2, 13, 3, 16].Style.Font.SetFromFont("標楷體", 20);
                    ws.Cells[2, 13, 3, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Distributed;
                    ws.Cells[2, 13, 3, 16].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[2, 13, 3, 16].Style.Indent = 2;

                    ws.Cells[1, 17].Value = string.Format("No. {0}", source.ID);
                    ws.Cells[1, 17, 1, 19].Merge = true;
                    ws.Cells[1, 17, 1, 19].Style.Font.SetFromFont("標楷體", 12);
                    ws.Cells[1, 17, 1, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[1, 17, 1, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[2, 17].Value = string.Format("頁{0} / 頁{1}", 1, 1);
                    pager.Add(2);
                    ws.Cells[2, 17, 2, 19].Merge = true;
                    ws.Cells[2, 17, 2, 19].Style.Font.SetFromFont("標楷體", 12);
                    ws.Cells[2, 17, 2, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[2, 17, 2, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Row(4).Height = 12;

                    for (var i = 5; i < 10; i++)
                        ws.Row(i).Height = 20;

                    ws.Cells[5, 1].Value = string.Format("工程單位：{0}", source.Customunit.Name);
                    ws.Cells[5, 1, 5, 12].Merge = true;
                    ws.Cells[5, 1, 5, 12].Style.Font.SetFromFont("標楷體", 16);
                    ws.Cells[5, 1, 5, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[5, 1, 5, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[5, 1, 5, 12].Style.Indent = 4;

                    ws.Cells[6, 1].Value = string.Format("　　統編：{0}", source.Customunit.DisplayTax);
                    ws.Cells[6, 1, 6, 12].Merge = true;
                    ws.Cells[6, 1, 6, 12].Style.Font.SetFromFont("標楷體", 16);
                    ws.Cells[6, 1, 6, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[6, 1, 6, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[6, 1, 6, 12].Style.Indent = 4;


                    ws.Cells[7, 1].Value = string.Format("公司地址：{0}", source.Customunit.Location.DisplayAddress);
                    ws.Cells[7, 1, 7, 12].Merge = true;
                    ws.Cells[7, 1, 7, 12].Style.Font.SetFromFont("標楷體", 16);
                    ws.Cells[7, 1, 7, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[7, 1, 7, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[7, 1, 7, 12].Style.Indent = 4;

                    ws.Cells[9, 1].Value = string.Format("工程地點：{0}", source.Location.DisplayAddress);
                    ws.Cells[9, 1, 9, 19].Merge = true;
                    ws.Cells[9, 1, 9, 19].Style.Font.SetFromFont("標楷體", 16);
                    ws.Cells[9, 1, 9, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[9, 1, 9, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[9, 1, 9, 19].Style.Indent = 4;

                    ws.Cells[5, 14].Value = string.Format("聯絡資訊");
                    ws.Cells[5, 14, 5, 16].Merge = true;
                    ws.Cells[5, 14, 5, 16].Style.Font.SetFromFont("標楷體", 16);
                    ws.Cells[5, 14, 5, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[5, 14, 5, 16].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[6, 14].Value = string.Format("CEL：{0}", source.Customunit.Cel);
                    ws.Cells[6, 14, 6, 19].Merge = true;
                    ws.Cells[6, 14, 6, 19].Style.Font.SetFromFont("標楷體", 16);
                    ws.Cells[6, 14, 6, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[6, 14, 6, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                    ws.Cells[7, 14].Value = string.Format("TEL：{0}", source.Customunit.Tel.DisplayContact);
                    ws.Cells[7, 14, 7, 19].Merge = true;
                    ws.Cells[7, 14, 7, 19].Style.Font.SetFromFont("標楷體", 16);
                    ws.Cells[7, 14, 7, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[7, 14, 7, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[8, 14].Value = string.Format("FAX：{0}", source.Customunit.Fax.DisplayContact);
                    ws.Cells[8, 14, 8, 19].Merge = true;
                    ws.Cells[8, 14, 8, 19].Style.Font.SetFromFont("標楷體", 16);
                    ws.Cells[8, 14, 8, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[8, 14, 8, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Row(10).Height = 12;

                    InsertProductTitle(ws, 11);

                    int RowIndex = 12;
                    double Total = 0.0;

                    if (!Is_fill_page)
                    {
                        for (var j = 0; j < source.Products.Count; j++)                      //22 * (pdlist.count)
                        {
                            Total = Total + InsertProductLine(ws, RowIndex, source.Products[j]);
                            RowIndex += 1;
                        }
                        InsertEmptyProductLine(ws, RowIndex);                        //22
                        RowIndex += 1;
                        InsertTotal(ws, RowIndex, Total, "稅前合計：");
                        RowIndex += 1;
                        InsertTotal(ws, RowIndex, Total * 0.05, "營業稅5%外加：");
                        RowIndex += 1;
                        InsertTotal(ws, RowIndex, Total * (1 + 0.05), "合計：");
                        RowIndex += 1;
                        ws.Row(RowIndex).Height = 12;
                        RowIndex += 1;
                        appendDetails(ws, RowIndex, source.Myunit, producer);
                    }
                    else
                    {
                        int CurrentPageHeight = 206;
                        int ONEPAGEHEIGHT = 557;
                        for (var j = 0; j < source.Products.Count; j++)                      //22 * (pdlist.count)
                        {
                            if ((CurrentPageHeight + 22) <= ONEPAGEHEIGHT - 22)
                            {
                                Total = Total + InsertProductLine(ws, RowIndex, source.Products[j]);
                                RowIndex += 1;
                                CurrentPageHeight += 22;
                            }
                            else
                            {
                                RowIndex = FillPage(ws, RowIndex, ONEPAGEHEIGHT - CurrentPageHeight);
                                CurrentPageHeight = 104;
                                j -= 1;
                            }
                        }
                        int RESTHEIGHT = 122 + (gridelements.Count() * 22);                //3 total + 2 details
                        int rest = ONEPAGEHEIGHT - (RESTHEIGHT + CurrentPageHeight);
                        if (rest >= 22)
                        {
                            InsertMergeLine(ws, RowIndex, rest, "以下空白");
                            RowIndex += 1;
                        }
                        else
                        {
                            RowIndex = FillPage(ws, RowIndex, ONEPAGEHEIGHT - CurrentPageHeight);
                            CurrentPageHeight = 104;
                            rest = ONEPAGEHEIGHT - (RESTHEIGHT + CurrentPageHeight);
                            InsertMergeLine(ws, RowIndex, rest, "以下空白");
                            RowIndex += 1;
                        }
                        InsertTotal(ws, RowIndex, Total, "稅前合計：");
                        RowIndex += 1;
                        InsertTotal(ws, RowIndex, Total * 0.05, "營業稅5%外加：");
                        RowIndex += 1;
                        InsertTotal(ws, RowIndex, Total * (1 + 0.05), "合計：");
                        RowIndex += 1;
                        ws.Row(RowIndex).Height = 12;
                        RowIndex += 1;

                        appendDetails(ws, RowIndex, source.Myunit, producer);                //22 * (2 + notelist.count)
                    }
                    for (var it = 0; it < pager.Count; it++)
                        ws.Cells[pager[it], 17].Value = string.Format("頁{0} / 頁{1}", it + 1, pager.Count);

                    excel.SaveAs(file);
                    //excel.SaveAs(file, "password");
                }
                return String.Format(Path.Combine(programFilesPath, foldername, tempfolder, source.ID));
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public static double InsertProductLine(ExcelWorksheet stws, int rowcount, Product data)
        {
            stws.Row(rowcount).Height = 22;
            stws.Cells[rowcount, 1].Value = data.Name;
            stws.Cells[rowcount, 1, rowcount, 3].Merge = true;
            stws.Cells[rowcount, 1, rowcount, 3].Style.Numberformat.Format = "@";
            stws.Cells[rowcount, 1, rowcount, 3].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 1, rowcount, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 1, rowcount, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 1, rowcount, 3]);

            stws.Cells[rowcount, 4].Value = Convert.ToDouble(data.Length);
            stws.Cells[rowcount, 4, rowcount, 5].Merge = true;
            stws.Cells[rowcount, 4, rowcount, 5].Style.Numberformat.Format = "0.00";
            stws.Cells[rowcount, 4, rowcount, 5].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 4, rowcount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 4, rowcount, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 4, rowcount, 5]);


            stws.Cells[rowcount, 6].Value = Convert.ToDouble(data.Width);
            stws.Cells[rowcount, 6, rowcount, 7].Merge = true;
            stws.Cells[rowcount, 6, rowcount, 7].Style.Numberformat.Format = "0.00";
            stws.Cells[rowcount, 6, rowcount, 7].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 6, rowcount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 6, rowcount, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 6, rowcount, 7]);

            stws.Cells[rowcount, 8].Value = Math.Round(Convert.ToDouble(data.Width) * Convert.ToDouble(data.Length), 2);
            stws.Cells[rowcount, 8, rowcount, 9].Merge = true;
            stws.Cells[rowcount, 8, rowcount, 9].Style.Numberformat.Format = "0.00";
            stws.Cells[rowcount, 8, rowcount, 9].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 8, rowcount, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 8, rowcount, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 8, rowcount, 9]);

            stws.Cells[rowcount, 10].Value = Convert.ToDouble(data.Ironmold);
            stws.Cells[rowcount, 10, rowcount, 11].Merge = true;
            stws.Cells[rowcount, 10, rowcount, 11].Style.Numberformat.Format = "0";
            stws.Cells[rowcount, 10, rowcount, 11].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 10, rowcount, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 10, rowcount, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 10, rowcount, 11]);

            stws.Cells[rowcount, 12].Value = Convert.ToDouble(data.Powercoating);
            stws.Cells[rowcount, 12, rowcount, 13].Merge = true;
            stws.Cells[rowcount, 12, rowcount, 13].Style.Numberformat.Format = "0";
            stws.Cells[rowcount, 12, rowcount, 13].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 12, rowcount, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 12, rowcount, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 12, rowcount, 13]);

            stws.Cells[rowcount, 14].Value = Convert.ToDouble(data.Ironslips);
            stws.Cells[rowcount, 14, rowcount, 15].Merge = true;
            stws.Cells[rowcount, 14, rowcount, 15].Style.Numberformat.Format = "0";
            stws.Cells[rowcount, 14, rowcount, 15].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 14, rowcount, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 14, rowcount, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 14, rowcount, 15]);

            stws.Cells[rowcount, 16].Value = Convert.ToDouble(data.Nut);
            stws.Cells[rowcount, 16, rowcount, 17].Merge = true;
            stws.Cells[rowcount, 16, rowcount, 17].Style.Numberformat.Format = "0";
            stws.Cells[rowcount, 16, rowcount, 17].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 16, rowcount, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 16, rowcount, 17].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 16, rowcount, 17]);
            stws.Cells[rowcount, 18].Value = Math.Round(Convert.ToDouble(data.Total));
            stws.Cells[rowcount, 18, rowcount, 19].Merge = true;
            stws.Cells[rowcount, 18, rowcount, 19].Style.Numberformat.Format = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
            stws.Cells[rowcount, 18, rowcount, 19].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 18, rowcount, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 18, rowcount, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 18, rowcount, 19]);
            return data.Total;
        }
        public static void setBorder(ExcelRange cells)
        {
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }
        public static void InsertProductTitle(ExcelWorksheet stws, int rowcount)
        {
            stws.Row(rowcount).Height = 22;
            stws.Cells[rowcount, 1].Value = "品名";
            stws.Cells[rowcount, 1, rowcount, 3].Merge = true;
            stws.Cells[rowcount, 1, rowcount, 3].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 1, rowcount, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Distributed;
            stws.Cells[rowcount, 1, rowcount, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            stws.Cells[rowcount, 1, rowcount, 3].Style.Indent = 4;
            setBorder(stws.Cells[rowcount, 1, rowcount, 3]);

            stws.Cells[rowcount, 4].Value = "長度(m)";
            stws.Cells[rowcount, 4, rowcount, 5].Merge = true;
            stws.Cells[rowcount, 4, rowcount, 5].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 4, rowcount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 4, rowcount, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 4, rowcount, 5]);

            stws.Cells[rowcount, 6].Value = "寬度(m)";
            stws.Cells[rowcount, 6, rowcount, 7].Merge = true;
            stws.Cells[rowcount, 6, rowcount, 7].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 6, rowcount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 6, rowcount, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 6, rowcount, 7]);

            using (ExcelRange Rng = stws.Cells[rowcount, 8])
            {
                Rng.Style.Font.SetFromFont("標楷體", 13);
                ExcelRichTextCollection RichTxtCollection = Rng.RichText;
                ExcelRichText RichText = RichTxtCollection.Add("數量(m");
                RichText = RichTxtCollection.Add("2");
                RichText.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;
                RichText = RichTxtCollection.Add(")");
            }
            stws.Cells[rowcount, 8, rowcount, 9].Merge = true;
            stws.Cells[rowcount, 8, rowcount, 9].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 8, rowcount, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 8, rowcount, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 8, rowcount, 9]);

            stws.Cells[rowcount, 10].Value = "鐵模單價";
            stws.Cells[rowcount, 10, rowcount, 11].Merge = true;
            stws.Cells[rowcount, 10, rowcount, 11].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 10, rowcount, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 10, rowcount, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 10, rowcount, 11]);

            stws.Cells[rowcount, 12].Value = "粉體塗裝(牙白)";
            stws.Cells[rowcount, 12, rowcount, 13].Merge = true;
            stws.Cells[rowcount, 12, rowcount, 13].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 12, rowcount, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 12, rowcount, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 12, rowcount, 13]);

            stws.Cells[rowcount, 14].Value = "鐵擋單價";
            stws.Cells[rowcount, 14, rowcount, 15].Merge = true;
            stws.Cells[rowcount, 14, rowcount, 15].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 14, rowcount, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 14, rowcount, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 14, rowcount, 15]);

            stws.Cells[rowcount, 16].Value = "鏍母單價";
            stws.Cells[rowcount, 16, rowcount, 17].Merge = true;
            stws.Cells[rowcount, 16, rowcount, 17].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 16, rowcount, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 16, rowcount, 17].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 16, rowcount, 17]);

            stws.Cells[rowcount, 18].Value = "$ 小計";
            stws.Cells[rowcount, 18, rowcount, 19].Merge = true;
            stws.Cells[rowcount, 18, rowcount, 19].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 18, rowcount, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 18, rowcount, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            setBorder(stws.Cells[rowcount, 18, rowcount, 19]);

        }
        public static void InsertEmptyProductLine(ExcelWorksheet stws, int rowcount)
        {
            stws.Row(rowcount).Height = 22;
            stws.Cells[rowcount, 1, rowcount, 3].Merge = true;
            setBorder(stws.Cells[rowcount, 1, rowcount, 3]);

            stws.Cells[rowcount, 4, rowcount, 5].Merge = true;
            setBorder(stws.Cells[rowcount, 4, rowcount, 5]);

            stws.Cells[rowcount, 6, rowcount, 7].Merge = true;
            setBorder(stws.Cells[rowcount, 6, rowcount, 7]);

            stws.Cells[rowcount, 8, rowcount, 9].Merge = true;
            setBorder(stws.Cells[rowcount, 8, rowcount, 9]);

            stws.Cells[rowcount, 10, rowcount, 11].Merge = true;
            setBorder(stws.Cells[rowcount, 10, rowcount, 11]);

            stws.Cells[rowcount, 12, rowcount, 13].Merge = true;
            setBorder(stws.Cells[rowcount, 12, rowcount, 13]);

            stws.Cells[rowcount, 14, rowcount, 15].Merge = true;
            setBorder(stws.Cells[rowcount, 14, rowcount, 15]);

            stws.Cells[rowcount, 16, rowcount, 17].Merge = true;
            setBorder(stws.Cells[rowcount, 16, rowcount, 17]);

            stws.Cells[rowcount, 18, rowcount, 19].Merge = true;
            setBorder(stws.Cells[rowcount, 18, rowcount, 19]);
        }
        public static void InsertMergeLine(ExcelWorksheet stws, int rowcount, int height, string text)
        {
            stws.Row(rowcount).Height = height;
            stws.Cells[rowcount, 1].Value = text;
            stws.Cells[rowcount, 1, rowcount, 19].Merge = true;
            setBorder(stws.Cells[rowcount, 1, rowcount, 19]);
            stws.Cells[rowcount, 1, rowcount, 19].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 1, rowcount, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            stws.Cells[rowcount, 1, rowcount, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
        public static int FillPage(ExcelWorksheet stws, int rowcount, int height)
        {
            InsertMergeLine(stws, rowcount, height, "承次頁");
            rowcount += 1;
            InsertPageInfo(stws, rowcount, 20, string.Format("No. {0}", Source.ID));
            rowcount += 1;
            InsertPageInfo(stws, rowcount, 20, string.Format("頁{0} / 頁{1}", 1, 1));
            pager.Add(rowcount);
            rowcount += 1;
            InsertPageInfo(stws, rowcount, 20, "");
            rowcount += 1;
            InsertProductTitle(stws, rowcount);
            rowcount += 1;
            InsertMergeLine(stws, rowcount, 22, "承前頁");
            rowcount += 1;
            return rowcount;
        }
        public static void InsertPageInfo(ExcelWorksheet stws, int rowcount, int height, string text)
        {
            stws.Row(rowcount).Height = height;
            stws.Cells[rowcount, 17].Value = text;
            stws.Cells[rowcount, 17, rowcount, 19].Merge = true;
            stws.Cells[rowcount, 1, rowcount, 19].Style.Font.SetFromFont("標楷體", 12);
            stws.Cells[rowcount, 1, rowcount, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            stws.Cells[rowcount, 1, rowcount, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        }
        public static double GetTrueColumnWidth(double width)
        {
            //DEDUCE WHAT THE COLUMN WIDTH WOULD REALLY GET SET TO
            double z = 1d;
            if (width >= (1 + 2 / 3))
            {
                z = Math.Round((Math.Round(7 * (width - 1 / 256), 0) - 5) / 7, 2);
            }
            else
            {
                z = Math.Round((Math.Round(12 * (width - 1 / 256), 0) - Math.Round(5 * width, 0)) / 12, 2);
            }

            //HOW FAR OFF? (WILL BE LESS THAN 1)
            double errorAmt = width - z;

            //CALCULATE WHAT AMOUNT TO TACK ONTO THE ORIGINAL AMOUNT TO RESULT IN THE CLOSEST POSSIBLE SETTING 
            double adj = 0d;
            if (width >= (1 + 2 / 3))
            {
                adj = (Math.Round(7 * errorAmt - 7 / 256, 0)) / 7;
            }
            else
            {
                adj = ((Math.Round(12 * errorAmt - 12 / 256, 0)) / 12) + (2 / 12);
            }

            //RETURN A SCALED-VALUE THAT SHOULD RESULT IN THE NEAREST POSSIBLE VALUE TO THE TRUE DESIRED SETTING
            if (z > 0)
            {
                return width + adj;
            }

            return 0d;
        }
        public static void InsertTotal(ExcelWorksheet stws, int rowcount, double total,string text)
        {
            stws.Row(rowcount).Height = 22;
            stws.Cells[rowcount, 1].Value = text;
            stws.Cells[rowcount, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            stws.Cells[rowcount, 1, rowcount, 11].Merge = true;
            stws.Cells[rowcount, 1, rowcount, 11].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 1, rowcount, 11].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            stws.Cells[rowcount, 1, rowcount, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            stws.Cells[rowcount, 1, rowcount, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            stws.Cells[rowcount, 1, rowcount, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            stws.Cells[rowcount, 12].Value = total;
            stws.Cells[rowcount, 19].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            stws.Cells[rowcount, 12, rowcount, 19].Merge = true;
            stws.Cells[rowcount, 12, rowcount, 19].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 12, rowcount, 19].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            stws.Cells[rowcount, 12, rowcount, 19].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            stws.Cells[rowcount, 12, rowcount, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            stws.Cells[rowcount, 12, rowcount, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            stws.Cells[rowcount, 12, rowcount, 19].Style.Numberformat.Format = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
            //stws.Cells[rowcount, 12, rowcount, 19].Style.Numberformat.Format = "\"NT$\"#,##0.00_);(\"NT$\"#,##0.00)";
        }
        public static void appendDetails(ExcelWorksheet stws, int rowcount, Unit local, string Is_producer)
        {
            bool firstline = true;
            foreach (var lp in gridelements)
            {
                using (ExcelRange Rng = stws.Cells[rowcount, 1])
                {
                    ExcelRichTextCollection RichTxtCollection = Rng.RichText;
                    ExcelRichText RichText = RichTxtCollection.Add("&&&title&&&");
                    var already_del = RichText;
                    var lpelement = lp.Elements();
                    foreach (var item in lpelement)
                    {
                        var clr = item.Attribute("Foreground");
                        var value = item.Value;
                        if (clr == null)
                        {
                            RichText = RichTxtCollection.Add(value);
                            RichText.Color = Color.Black;
                        }
                        else
                        {
                            Color colFromHex = ColorTranslator.FromHtml(clr.Value.ToString());
                            RichText = RichTxtCollection.Add(value);
                            RichText.Color = colFromHex;
                        }
                    }
                    RichTxtCollection.Remove(already_del);
                }
                stws.Row(rowcount).Height = 22;
                stws.Cells[rowcount, 1, rowcount, 19].Merge = true;
                stws.Cells[rowcount, 19].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                stws.Cells[rowcount, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                if (firstline)
                {
                    stws.Cells[rowcount, 1, rowcount, 19].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    firstline = false;
                }
                stws.Cells[rowcount, 1, rowcount, 19].Style.Font.SetFromFont("標楷體", 13);
                stws.Cells[rowcount, 1, rowcount, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                stws.Cells[rowcount, 1, rowcount, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rowcount++;
            }
            stws.Row(rowcount).Height = 22;
            stws.Cells[rowcount, 1, rowcount, 19].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            stws.Cells[rowcount, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            stws.Cells[rowcount, 1].Value = string.Format("統編：{0}", local.DisplayTax);
            stws.Cells[rowcount, 1, rowcount, 3].Merge = true;
            stws.Cells[rowcount, 1, rowcount, 3].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 1, rowcount, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            stws.Cells[rowcount, 1, rowcount, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            stws.Cells[rowcount, 5].Value = string.Format("CEL：{0}", local.Cel);
            stws.Cells[rowcount, 5, rowcount, 8].Merge = true;
            stws.Cells[rowcount, 5, rowcount, 8].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 5, rowcount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 5, rowcount, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            stws.Cells[rowcount, 10].Value = string.Format("TEL：{0}", local.Tel.DisplayContact);

            stws.Cells[rowcount, 10, rowcount, 13].Merge = true;
            stws.Cells[rowcount, 10, rowcount, 13].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 10, rowcount, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            stws.Cells[rowcount, 10, rowcount, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            stws.Cells[rowcount, 15].Value = string.Format("FAX：{0} ", local.Fax.DisplayContact);
            stws.Cells[rowcount, 15, rowcount, 19].Merge = true;
            stws.Cells[rowcount, 15, rowcount, 19].Style.Font.SetFromFont("標楷體", 13);
            stws.Cells[rowcount, 15, rowcount, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            stws.Cells[rowcount, 15, rowcount, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            stws.Cells[rowcount, 15, rowcount, 19].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            rowcount++;
            stws.Row(rowcount).Height = 22;
            stws.Cells[rowcount, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;


            stws.Cells[rowcount, 1].Value = string.Format("公司地址：{0}", local.Location.DisplayAddress);
            if (Is_producer == String.Empty)
            {
                stws.Cells[rowcount, 1, rowcount, 10].Merge = true;
                stws.Cells[rowcount, 1, rowcount, 10].Style.Font.SetFromFont("標楷體", 13);
                stws.Cells[rowcount, 1, rowcount, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                stws.Cells[rowcount, 1, rowcount, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                stws.Cells[rowcount, 1, rowcount, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                stws.Cells[rowcount, 11].Value = string.Format("製表日期：{0}年{1}月{2}日", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                stws.Cells[rowcount, 11, rowcount, 19].Merge = true;
                stws.Cells[rowcount, 11, rowcount, 19].Style.Font.SetFromFont("標楷體", 13);
                stws.Cells[rowcount, 11, rowcount, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                stws.Cells[rowcount, 11, rowcount, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                stws.Cells[rowcount, 11, rowcount, 19].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                stws.Cells[rowcount, 11, rowcount, 19].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }
            else
            {
                stws.Cells[rowcount, 1, rowcount, 9].Merge = true;
                stws.Cells[rowcount, 1, rowcount, 9].Style.Font.SetFromFont("標楷體", 13);
                stws.Cells[rowcount, 1, rowcount, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                stws.Cells[rowcount, 1, rowcount, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                stws.Cells[rowcount, 1, rowcount, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                stws.Cells[rowcount, 10].Value = string.Format("製表人：{0}", Is_producer);
                stws.Cells[rowcount, 10, rowcount, 13].Merge = true;
                stws.Cells[rowcount, 10, rowcount, 13].Style.Font.SetFromFont("標楷體", 13);
                stws.Cells[rowcount, 10, rowcount, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                stws.Cells[rowcount, 10, rowcount, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                stws.Cells[rowcount, 10, rowcount, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                stws.Cells[rowcount, 14].Value = string.Format("製表日期：{0}年{1}月{2}日", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                stws.Cells[rowcount, 14, rowcount, 19].Merge = true;
                stws.Cells[rowcount, 14, rowcount, 19].Style.Font.SetFromFont("標楷體", 13);
                stws.Cells[rowcount, 14, rowcount, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                stws.Cells[rowcount, 14, rowcount, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                stws.Cells[rowcount, 14, rowcount, 19].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                stws.Cells[rowcount, 14, rowcount, 19].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }
        }
    }
}
