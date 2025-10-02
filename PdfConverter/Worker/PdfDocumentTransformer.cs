using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using GdPicture14;
using PdfConverter.Worker.Stamp;

namespace PdfConverter.Worker
{
    public class PdfDocumentTransformer
    {

        private const string processedSuffix = " (stamp).pdf";
        private const string tempSuffix = " (temp).pdf";
        private double progress = 0;

        public static string MakeOutputDocumentName(string inputDocumentName, string suffix = processedSuffix)
        {
            return Path.Combine(Path.GetDirectoryName(inputDocumentName), Path.GetFileNameWithoutExtension(inputDocumentName) + suffix);
        }

        public void ConvertFileToPDFA(string inputDocumentName, string outputDocumentName)
        {
            using (GdPictureDocumentConverter docConverter = new GdPictureDocumentConverter())

            {
                var documentFormat = GdPictureDocumentUtilities.GetDocumentFormatFromFileName(inputDocumentName);
                GdPictureStatus status = docConverter.LoadFromFile(inputDocumentName, documentFormat);

                if (status == GdPictureStatus.OK)
                {
                    status = docConverter.SaveAsPDF(FilePath: outputDocumentName,
                                                    Conformance: PdfConformance.PDF_A_1a);
                    if (status == GdPictureStatus.OK)
                    {

                    }
                    else
                    {
                        throw new TransformError("Ошибка сохранения: " + status.ToString());
                    }
                }
                else
                {
                    throw new TransformError("Ошибка загрузки: " + status.ToString());
                }
            }
        }

        private void CheckGdStatus(GdPictureStatus status)
        {
            if (status != GdPictureStatus.OK)
            {
                throw new TransformError(status.ToString());
            }
        }

        public void ProcessDocument(Certificate certificate, BackgroundWorker worker, DoWorkEventArgs e)
        {
            try
            {
                int sleep = 0;
                if (e.Argument != null)
                {
                    if (e.Argument is int)
                    {
                        sleep = (int)e.Argument;
                    }
                }

                progress = 10;
                worker.ReportProgress((int)Math.Floor(progress), new State("Процесс конвертации начат"));
                string tmpOutputPath = MakeOutputDocumentName(inputDocumentName: certificate.FilePath,
                                                           suffix: tempSuffix);
                ConvertFileToPDFA(inputDocumentName: certificate.FilePath,
                                  outputDocumentName: tmpOutputPath);

                progress = 20;
                worker.ReportProgress((int)Math.Floor(progress), new State("Конвертация в PDF/A"));

                using (GdPicturePDF pdf = new GdPicturePDF())
                {
                    var status = pdf.LoadFromFile(FilePath: tmpOutputPath);
                    CheckGdStatus(status);

                    pdf.SetMeasurementUnit(PdfMeasurementUnit.PdfMeasurementUnitMillimeter);

                    var fontResName = pdf.AddTrueTypeFontFromFileU(FilePath: Stamp.Abstract.StampElementText.FontResource,
                                                                           FileName: Stamp.Abstract.StampElementText.FontName,
                                                                           Bold: false,
                                                                           Italic: false,
                                                                           EnableSubset: true);
                    CheckGdStatus(pdf.GetStat());

                    var n = pdf.GetPageCount();

                    double progressIncrement = 70.0 / (n + 1);

                    //throw new TransformError("!!!");
                    for (int i = 1; i <= n; i++)
                    {
                        CheckGdStatus(pdf.SelectPage(i));
                        var pageWidth = pdf.GetPageWidth();
                        CheckGdStatus(status: pdf.SetTextSize(TextSize: Stamp.Abstract.StampElementText.FontSize));
                        if (i < n)
                        {
                            var footerPosition1 = new Tuple<float, float>(3, 3);
                            var footerPosition2 = new Tuple<float, float>(3, 1);

                            var footerText1 = "Документ подписан электронной подписью";
                            var footerText2 = string.Format("{0} Сертификат:{1} Срок действия: с {2:d} по {3:d} ",
                                                           certificate.Fio,
                                                           certificate.Fingerprint,
                                                           certificate.DateFrom,
                                                           certificate.DateTo);

                            using (var ft = new StampElementFooter(footerPosition1, footerText1))
                            {
                                CheckGdStatus(pdf.DrawText(fontResName,
                                                         DstX: ft.Position.Item1,
                                                         DstY: ft.Position.Item2,
                                                         Text: ft.Text));


                            }
                            using (var ft = new StampElementFooter(footerPosition2, footerText2))
                            {
                                CheckGdStatus(pdf.DrawText(fontResName,
                                                         DstX: ft.Position.Item1,
                                                         DstY: ft.Position.Item2,
                                                         Text: ft.Text));


                            }
                            progress += progressIncrement;
                            worker.ReportProgress((int)Math.Floor(progress), new State(string.Format("Страница {0}/{1}", i, n)));
                        }
                        else
                        {
                            using (var imgCls = new GdPictureImaging())
                            {
                                var imgId = imgCls.CreateGdPictureImageFromFile(StampElement.PictureResource);
                                CheckGdStatus(pdf.GetStat());

                                var imageRes = pdf.AddImageFromGdPictureImage(ImageID: imgId,
                                                                              ImageMask: false,
                                                                              DrawImage: false);
                                CheckGdStatus(pdf.GetStat());

                                using (var stamp = new StampElement(pageWidth))
                                {
                                    var position = stamp.Position;
                                    var dimensions = stamp.Dimensions;

                                    CheckGdStatus(pdf.DrawImage(ImageResName: imageRes,
                                                              DstX: position.Item1,
                                                              DstY: position.Item2,
                                                              Width: dimensions.Item1,
                                                              Height: dimensions.Item2));


                                    progress += progressIncrement;
                                    worker.ReportProgress((int)Math.Floor(progress), new State(string.Format("Страница {0}/{1}", i, n)));


                                    using (var crtElement = new StampElementCert(position, certificate.Fingerprint))
                                    {
                                        CheckGdStatus(pdf.DrawText(fontResName,
                                                                 DstX: crtElement.Position.Item1,
                                                                 DstY: crtElement.Position.Item2,
                                                                 Text: crtElement.Text));
                                    }

                                    using (var fioElement = new StampElementFio(position, certificate.Fio))
                                    {
                                        CheckGdStatus(pdf.DrawText(fontResName,
                                                                 DstX: fioElement.Position.Item1,
                                                                 DstY: fioElement.Position.Item2,
                                                                 Text: fioElement.Text));
                                    }

                                    var dateString = string.Format("с {0:d} по {1:d}", certificate.DateFrom, certificate.DateTo);

                                    using (var dateElement = new StampElementDates(position, dateString))
                                    {
                                        CheckGdStatus(pdf.DrawText(fontResName,
                                                                 DstX: dateElement.Position.Item1,
                                                                 DstY: dateElement.Position.Item2,
                                                                 Text: dateElement.Text));
                                    }

                                    progress += progressIncrement;
                                    worker.ReportProgress((int)Math.Floor(progress), new State(string.Format("Страница {0}/{1}", i, n)));
                                }
                            }
                        }
                    }
                    progress = 90;
                    worker.ReportProgress((int)Math.Floor(progress), new State(string.Format("Страница {0}/{1}", n, n)));
                    var finalOutput = MakeOutputDocumentName(inputDocumentName: certificate.FilePath,
                                                         suffix: processedSuffix);
                    CheckGdStatus(pdf.SaveToFile(finalOutput));

                    CheckGdStatus(pdf.CloseDocument());

                    File.Delete(tmpOutputPath);
                    progress = 100;
                    worker.ReportProgress((int)Math.Floor(progress), new State(string.Format("Страница {0}/{1}", n, n)));

                    e.Result = finalOutput;
                }
            }
            catch (TransformError tr)
            {
                worker.ReportProgress((int)Math.Floor(progress), new State($"Ошибка обработки документа: {tr.Message}", true));
            }
            catch (Exception ex)
            {
                worker.ReportProgress((int)Math.Floor(progress), new State($"Ошибка: {ex.Message}", true));
            }


        }
    }
}