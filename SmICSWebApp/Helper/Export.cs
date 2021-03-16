/*using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SmICSWebApp.Data;
using System;
using System.IO;

namespace SmICSWebApp.Helper
{
    public class Export
    {
        //Export States as Pdf
        public void GeneratePDFStates(IJSRuntime js, State state)
        {
            PdfState pdfState = new PdfState();
            js.InvokeAsync<StateAttributes>("saveAsFile","Bundesländerlist.pdf",Convert.ToBase64String(pdfState.PdfReport(state)));
        }

        //Export States as Excel
        public void GenerateExcelStates(IJSRuntime iJSRunteim, State states)
        {
            if (states != null)
            {
                byte[] statesAsByte;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    // of the first row
                    worksheet.Row(1).Height = 20;
                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Headers Row
                    string[] head = { "ID", "Bundesland", "Fallzahl", "Todes­fälle", "Fälle pro 100000 EW", "7-Tage-Inzi­denz" };
                    int y = 0;
                    for (int i = 1; i <= 6; i++)
                    {
                        worksheet.Cells[1, i].Value = head[y];
                        y++;
                    }        

                    int recordIndex = 2;

                    foreach (StateFeature stateFeature in states.features)
                    {
                        worksheet.Cells[recordIndex, 1].Value = stateFeature.attributes.Id;
                        worksheet.Cells[recordIndex, 2].Value = stateFeature.attributes.Bundesland;
                        worksheet.Cells[recordIndex, 3].Value = stateFeature.attributes.Fallzahl;
                        worksheet.Cells[recordIndex, 4].Value = stateFeature.attributes.Todesfaelle;
                        worksheet.Cells[recordIndex, 5].Value = stateFeature.attributes.FaellePro100000Ew;
                        worksheet.Cells[recordIndex, 6].Value = stateFeature.attributes.Faelle7BlPro100K;

                        recordIndex++;
                    }

                    for (int i = 1; i < 6; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    statesAsByte = package.GetAsByteArray();
                }
                iJSRunteim.InvokeAsync<StateAttributes>("saveAsFile", "BundesländerList.xlsx", Convert.ToBase64String(statesAsByte));
            }
        }


        //Export Districts as Excel
        public void GenerateExcelDistricts(IJSRuntime iJSRunteim, District district)
        {
            if (district != null)
            {
                byte[] fileContent;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {

                    var worksheet = package.Workbook.Worksheets.Add("Sheet2");
                    // of the first row
                    worksheet.Row(1).Height = 20;
                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Headers Row
                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Stadt";
                    worksheet.Cells[1, 3].Value = "Landkreis";
                    worksheet.Cells[1, 4].Value = "Bundesland";
                    worksheet.Cells[1, 5].Value = "Fallzahl";
                    worksheet.Cells[1, 6].Value = "Todes­fälle";
                    worksheet.Cells[1, 7].Value = "Sterberate";
                    worksheet.Cells[1, 8].Value = "Fälle pro 100000_EW";
                    worksheet.Cells[1, 9].Value = "Betroffenenrate %";
                    worksheet.Cells[1, 10].Value = "Fälle letzte 7 Tage/100.000 EW";

                    int recordIndex = 2;

                    foreach (DistrictFeature districtFeature in district.features)
                    {
                        worksheet.Cells[recordIndex, 1].Value = districtFeature.districtAttributes.OBJECTID;
                        worksheet.Cells[recordIndex, 2].Value = districtFeature.districtAttributes.GEN;
                        worksheet.Cells[recordIndex, 3].Value = districtFeature.districtAttributes.county;
                        worksheet.Cells[recordIndex, 4].Value = districtFeature.districtAttributes.BL;
                        worksheet.Cells[recordIndex, 5].Value = districtFeature.districtAttributes.cases;
                        worksheet.Cells[recordIndex, 6].Value = districtFeature.districtAttributes.deaths;
                        worksheet.Cells[recordIndex, 7].Value = districtFeature.districtAttributes.death_rate;
                        worksheet.Cells[recordIndex, 8].Value = districtFeature.districtAttributes.cases_per_100k;
                        worksheet.Cells[recordIndex, 9].Value = districtFeature.districtAttributes.cases_per_population;
                        worksheet.Cells[recordIndex, 10].Value = districtFeature.districtAttributes.cases7_per_100k;

                        recordIndex++;
                    }

                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    worksheet.Column(5).AutoFit();
                    worksheet.Column(6).AutoFit();
                    worksheet.Column(7).AutoFit();
                    worksheet.Column(8).AutoFit();
                    worksheet.Column(9).AutoFit();
                    worksheet.Column(10).AutoFit();

                    fileContent = package.GetAsByteArray();
                }

                iJSRunteim.InvokeAsync<StateAttributes>("saveAsFile", "LandkreisList.xlsx", Convert.ToBase64String(fileContent));


            }

        }
    }

    public class PdfState
    {
        int maxColumn = 6;
        Document document;
        PdfPTable pdfTable = new PdfPTable(6);
        PdfPCell pdfCell;
        Font fontStyle;
        MemoryStream _memoryStream = new MemoryStream();
        State state = new State();

        public byte[] PdfReport(State state)
        {
            this.state = state;
            document = new Document(PageSize.A4, 10f, 10f, 20f, 30f);
            pdfTable.WidthPercentage = 100;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfWriter.GetInstance(document, _memoryStream);
            document.Open();

            float[] sizes = new float[maxColumn];

            for (var i = 0; i < maxColumn; i++)
            {
                if (i == 0) { sizes[i] = 50; }
                else { sizes[i] = 100; }
            }
            pdfTable.SetWidths(sizes);


            this.Header();
            this.Body();

            pdfTable.HeaderRows = 2;
            document.Add(pdfTable);
            document.Close();

            return _memoryStream.ToArray();
        }

        private void Header()
        {
            fontStyle = FontFactory.GetFont("Tahoma", 18f, 1);
            pdfCell = new PdfPCell(new Phrase("RKI-Dashboard", fontStyle));
            pdfCell.Colspan = maxColumn;
            pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell.Border = 0;
            pdfCell.ExtraParagraphSpace = 0;
            pdfTable.AddCell(pdfCell);
            pdfTable.CompleteRow();

            pdfCell = new PdfPCell(new Phrase("Bundesländer", fontStyle));
            pdfCell.Colspan = maxColumn;
            pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell.Border = 0;
            pdfCell.ExtraParagraphSpace = 0;
            pdfTable.AddCell(pdfCell);
            pdfTable.CompleteRow();
        }

        private void Body()
        {
            this.fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
            var fontStyleNew = FontFactory.GetFont("Tahoma", 9f, 0);

            string[] body = { "ID", "Bundesland", "Fallzahl", "Todes­fälle" , "Fälle pro 100000 EW", "7-Tage-Inzi­denz" };


            for (int y = 0; y < 6; y++)
            {
                pdfCell = new PdfPCell(new Phrase(body[y], this.fontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfTable.AddCell(pdfCell);

            }

            pdfTable.CompleteRow();

            int i = 1;

           
            foreach (StateFeature stateFeature in state.features)
            {

                pdfCell = new PdfPCell(new Phrase(i++.ToString(), fontStyleNew));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfCell.BackgroundColor = BaseColor.WHITE;
                pdfTable.AddCell(pdfCell);

                pdfCell = new PdfPCell(new Phrase(stateFeature.attributes.Bundesland, fontStyleNew));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfCell.BackgroundColor = BaseColor.WHITE;
                pdfTable.AddCell(pdfCell);

                pdfCell = new PdfPCell(new Phrase(stateFeature.attributes.Fallzahl.ToString(), fontStyleNew));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfCell.BackgroundColor = BaseColor.WHITE;
                pdfTable.AddCell(pdfCell);

                pdfCell = new PdfPCell(new Phrase(stateFeature.attributes.Todesfaelle.ToString(), fontStyleNew));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfCell.BackgroundColor = BaseColor.WHITE;
                pdfTable.AddCell(pdfCell);

                pdfCell = new PdfPCell(new Phrase(stateFeature.attributes.FaellePro100000Ew.ToString(), fontStyleNew));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfCell.BackgroundColor = BaseColor.WHITE;
                pdfTable.AddCell(pdfCell);

                pdfCell = new PdfPCell(new Phrase(stateFeature.attributes.Faelle7BlPro100K.ToString(), fontStyleNew));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfCell.BackgroundColor = BaseColor.WHITE;
                pdfTable.AddCell(pdfCell);

                pdfTable.CompleteRow();

            }

        }

    }
}
*/