using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Security.Principal;
using DocumentFormat.OpenXml;
using GitCreativeWorkRaport.Models;

namespace GitCreativeWorkRaport
{
    internal enum Months
    {
        styczniu,
        lutym,
        marcu,
        kwietniu,
        maju,
        czerwcu,
        lipcu,
        sierpniu,
        wrześniu,
        październiku,
        listopadzie,
        grudniu
    }

    internal class DocumentFormater
    {
        private readonly WordprocessingDocument _document;
        private readonly Body _body;

        public DocumentFormater(string filepath)
        {
            _document = WordprocessingDocument.Create(filepath, WordprocessingDocumentType.Document);
            MainDocumentPart mainPart = _document.AddMainDocumentPart();
            mainPart.Document = new Document();
            _body = mainPart.Document.AppendChild(new Body());
        }

        public void CreateHeading(DateTime firstDay, DateTime lastDay)
        {
            Text[] cityAndDate = [new("Słupsk " + DateTime.Now.ToString("dd-MM-yyyy"))];
            CreateTextWithAlignement(JustificationValues.Right, cityAndDate);


            Text[] raportTittle = [new("Raport od " + firstDay.ToString("dd-MM-yyyy") + " do " + lastDay.ToString("dd-MM-yyyy"))];
            CreateTextWithAlignement(JustificationValues.Center, raportTittle);


            string description = "Lista przekazanych utworów objętych majątkowym prawem autorskim, wytworzonych i przekazanych pracodawcy przez pracownika: ";
            Text[] descriptionText = [new(description + GetNameAndSurname() + ".")];
            CreateTextWithAlignement(JustificationValues.Left, descriptionText);
        }

        public void CreateTable(ObservableCollection<RaportDataModel> raportDataModel)
        {
            Table table = new();

            TableProperties tblProp = new(
                new TableBorders(
                    new TopBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 10
                    },
                    new BottomBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 10
                    },
                    new LeftBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 10
                    },
                    new RightBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 10
                    },
                    new InsideHorizontalBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 10
                    },
                    new InsideVerticalBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 10
                    }
                ));

            table.AppendChild<TableProperties>(tblProp);

            TableRow tr = new();


            AddCell(tr, "1400", "Data");
            AddCell(tr, "1400", "Repozytorium");
            AddCell(tr, "1200", "Id");
            AddCell(tr, "3000", "Nazwa commita");
            AddCell(tr, "1500", "Czas poświęcony na stworzenie utworu w godzinach");
            table.Append(tr);

            foreach (var item in raportDataModel)
            {
                TableRow tr1 = new();
                AddCell(tr1, "1400", item.Date);
                AddCell(tr1, "1400", item.RepoName);
                AddCell(tr1, "1200", item.Id);
                AddCell(tr1, "3000", item.CommitName);
                AddCell(tr1, "1500", item.Time.ToString());
                table.Append(tr1);
            }

            _body.Append(table);
        }

        private static void AddCell(TableRow tr, string width, string text)
        {
            TableCell tc1 = new();

            tc1.Append(new TableCellProperties(
                new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = width }));

            tc1.Append(new Paragraph(new Run(new Text(text))));

            tr.Append(tc1);
        }

        public void SaveAndClose()
        {
            _document.Save();
            _document.Dispose();
        }

        public void CreateEnding(double hours, DateTime date)
        {
            Months month = (Months)(date.Month - 1);
            string text = "Łączny czas pracy poświęcony na wytworzenie utworów objętych prawem autorskim w miesiącu ";
            string textWithDate = text + month;
            string textWithHours = textWithDate + ": " + hours.ToString() + " godzin.";
            Text[] fullText = [new(textWithHours)];
            CreateTextWithAlignement(JustificationValues.Left, fullText);
            Paragraph para = _body.AppendChild(new Paragraph());

            Text[] signature = [ new("Podpis pracodawcy \t\t\t\t\t\t") { Space = SpaceProcessingModeValues.Preserve }
                                            , new("Podpis pracownika") ];

            CreateTextWithAlignement(JustificationValues.Center, signature);
        }

        private void CreateTextWithAlignement(JustificationValues value, Text[] text)
        {
            ParagraphProperties paragraphProperties = new();
            Justification justification = new() { Val = value };
            paragraphProperties.Append(justification);
            Paragraph para = _body.AppendChild(new Paragraph(paragraphProperties));

            Run run = new();
            run.Append(text);
            para.Append(run);

        }

        private string GetNameAndSurname()
        {
            var identity = WindowsIdentity.GetCurrent();
            var user = new DirectoryEntry($"LDAP://<SID={identity.User.Value}>");

            user.RefreshCache(["givenName", "sn"]);
            var firstName = user.Properties["givenName"].Value;
            var lastName = user.Properties["sn"].Value;

            return firstName + " " + lastName;
        }

    }
}
