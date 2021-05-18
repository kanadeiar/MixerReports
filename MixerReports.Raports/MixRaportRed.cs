using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;
using Ap = DocumentFormat.OpenXml.ExtendedProperties;
using Vt = DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using MixerReports.lib.Models;
using X15ac = DocumentFormat.OpenXml.Office2013.ExcelAc;
using X15 = DocumentFormat.OpenXml.Office2013.Excel;
using A = DocumentFormat.OpenXml.Drawing;
using Thm15 = DocumentFormat.OpenXml.Office2013.Theme;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;

namespace MixerReports.Raports
{
    public class MixRaportRed
    {
        /// <summary> Дневные данные заливки </summary>
        public IEnumerable<Mix> DayMixes { get; set; }
        /// <summary> Ночные данные заливки </summary>
        public IEnumerable<Mix> NightMixes { get; set; }
        // Creates a SpreadsheetDocument.
        public void CreatePackage(string filePath)
        {
            using (SpreadsheetDocument package = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                CreateParts(package);
            }
        }

        #region Остальное

        // Adds child parts and generates content of the specified part.
        private void CreateParts(SpreadsheetDocument document)
        {
            ExtendedFilePropertiesPart extendedFilePropertiesPart1 = document.AddNewPart<ExtendedFilePropertiesPart>("rId3");
            GenerateExtendedFilePropertiesPart1Content(extendedFilePropertiesPart1);

            WorkbookPart workbookPart1 = document.AddWorkbookPart();
            GenerateWorkbookPart1Content(workbookPart1);

            ThemePart themePart1 = workbookPart1.AddNewPart<ThemePart>("rId3");
            GenerateThemePart1Content(themePart1);

            WorksheetPart worksheetPart1 = workbookPart1.AddNewPart<WorksheetPart>("rId2");
            GenerateWorksheetPart1Content(worksheetPart1);

            WorksheetPart worksheetPart2 = workbookPart1.AddNewPart<WorksheetPart>("rId1");
            GenerateWorksheetPart2Content(worksheetPart2);

            SpreadsheetPrinterSettingsPart spreadsheetPrinterSettingsPart1 = worksheetPart2.AddNewPart<SpreadsheetPrinterSettingsPart>("rId1");
            GenerateSpreadsheetPrinterSettingsPart1Content(spreadsheetPrinterSettingsPart1);

            CalculationChainPart calculationChainPart1 = workbookPart1.AddNewPart<CalculationChainPart>("rId6");
            GenerateCalculationChainPart1Content(calculationChainPart1);

            SharedStringTablePart sharedStringTablePart1 = workbookPart1.AddNewPart<SharedStringTablePart>("rId5");
            GenerateSharedStringTablePart1Content(sharedStringTablePart1);

            WorkbookStylesPart workbookStylesPart1 = workbookPart1.AddNewPart<WorkbookStylesPart>("rId4");
            GenerateWorkbookStylesPart1Content(workbookStylesPart1);

            SetPackageProperties(document);
        }

        // Generates content of extendedFilePropertiesPart1.
        private void GenerateExtendedFilePropertiesPart1Content(ExtendedFilePropertiesPart extendedFilePropertiesPart1)
        {
            Ap.Properties properties1 = new Ap.Properties();
            properties1.AddNamespaceDeclaration("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
            Ap.Application application1 = new Ap.Application();
            application1.Text = "Microsoft Excel";
            Ap.DocumentSecurity documentSecurity1 = new Ap.DocumentSecurity();
            documentSecurity1.Text = "0";
            Ap.ScaleCrop scaleCrop1 = new Ap.ScaleCrop();
            scaleCrop1.Text = "false";

            Ap.HeadingPairs headingPairs1 = new Ap.HeadingPairs();

            Vt.VTVector vTVector1 = new Vt.VTVector() { BaseType = Vt.VectorBaseValues.Variant, Size = (UInt32Value)2U };

            Vt.Variant variant1 = new Vt.Variant();
            Vt.VTLPSTR vTLPSTR1 = new Vt.VTLPSTR();
            vTLPSTR1.Text = "Листы";

            variant1.Append(vTLPSTR1);

            Vt.Variant variant2 = new Vt.Variant();
            Vt.VTInt32 vTInt321 = new Vt.VTInt32();
            vTInt321.Text = "2";

            variant2.Append(vTInt321);

            vTVector1.Append(variant1);
            vTVector1.Append(variant2);

            headingPairs1.Append(vTVector1);

            Ap.TitlesOfParts titlesOfParts1 = new Ap.TitlesOfParts();

            Vt.VTVector vTVector2 = new Vt.VTVector() { BaseType = Vt.VectorBaseValues.Lpstr, Size = (UInt32Value)2U };
            Vt.VTLPSTR vTLPSTR2 = new Vt.VTLPSTR();
            vTLPSTR2.Text = "08-20";
            Vt.VTLPSTR vTLPSTR3 = new Vt.VTLPSTR();
            vTLPSTR3.Text = "20-08";

            vTVector2.Append(vTLPSTR2);
            vTVector2.Append(vTLPSTR3);

            titlesOfParts1.Append(vTVector2);
            Ap.Company company1 = new Ap.Company();
            company1.Text = "";
            Ap.LinksUpToDate linksUpToDate1 = new Ap.LinksUpToDate();
            linksUpToDate1.Text = "false";
            Ap.SharedDocument sharedDocument1 = new Ap.SharedDocument();
            sharedDocument1.Text = "false";
            Ap.HyperlinksChanged hyperlinksChanged1 = new Ap.HyperlinksChanged();
            hyperlinksChanged1.Text = "false";
            Ap.ApplicationVersion applicationVersion1 = new Ap.ApplicationVersion();
            applicationVersion1.Text = "15.0300";

            properties1.Append(application1);
            properties1.Append(documentSecurity1);
            properties1.Append(scaleCrop1);
            properties1.Append(headingPairs1);
            properties1.Append(titlesOfParts1);
            properties1.Append(company1);
            properties1.Append(linksUpToDate1);
            properties1.Append(sharedDocument1);
            properties1.Append(hyperlinksChanged1);
            properties1.Append(applicationVersion1);

            extendedFilePropertiesPart1.Properties = properties1;
        }

        // Generates content of workbookPart1.
        private void GenerateWorkbookPart1Content(WorkbookPart workbookPart1)
        {
            Workbook workbook1 = new Workbook() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x15" } };
            workbook1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            workbook1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            workbook1.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
            FileVersion fileVersion1 = new FileVersion() { ApplicationName = "xl", LastEdited = "6", LowestEdited = "6", BuildVersion = "14420" };
            WorkbookProperties workbookProperties1 = new WorkbookProperties() { DefaultThemeVersion = (UInt32Value)153222U };

            AlternateContent alternateContent1 = new AlternateContent();
            alternateContent1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");

            AlternateContentChoice alternateContentChoice1 = new AlternateContentChoice() { Requires = "x15" };

            X15ac.AbsolutePath absolutePath1 = new X15ac.AbsolutePath() { Url = "C:\\Develop\\MixerReports\\MixerReports.Raports\\" };
            absolutePath1.AddNamespaceDeclaration("x15ac", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/ac");

            alternateContentChoice1.Append(absolutePath1);

            alternateContent1.Append(alternateContentChoice1);

            BookViews bookViews1 = new BookViews();
            WorkbookView workbookView1 = new WorkbookView() { XWindow = 0, YWindow = 0, WindowWidth = (UInt32Value)28800U, WindowHeight = (UInt32Value)16380U };

            bookViews1.Append(workbookView1);

            Sheets sheets1 = new Sheets();
            Sheet sheet1 = new Sheet() { Name = "08-20", SheetId = (UInt32Value)2U, Id = "rId1" };
            Sheet sheet2 = new Sheet() { Name = "20-08", SheetId = (UInt32Value)1U, Id = "rId2" };

            sheets1.Append(sheet1);
            sheets1.Append(sheet2);
            CalculationProperties calculationProperties1 = new CalculationProperties() { CalculationId = (UInt32Value)152511U };

            WorkbookExtensionList workbookExtensionList1 = new WorkbookExtensionList();

            WorkbookExtension workbookExtension1 = new WorkbookExtension() { Uri = "{140A7094-0E35-4892-8432-C4D2E57EDEB5}" };
            workbookExtension1.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
            X15.WorkbookProperties workbookProperties2 = new X15.WorkbookProperties() { ChartTrackingReferenceBase = true };

            workbookExtension1.Append(workbookProperties2);

            workbookExtensionList1.Append(workbookExtension1);

            workbook1.Append(fileVersion1);
            workbook1.Append(workbookProperties1);
            workbook1.Append(alternateContent1);
            workbook1.Append(bookViews1);
            workbook1.Append(sheets1);
            workbook1.Append(calculationProperties1);
            workbook1.Append(workbookExtensionList1);

            workbookPart1.Workbook = workbook1;
        }

        // Generates content of themePart1.
        private void GenerateThemePart1Content(ThemePart themePart1)
        {
            A.Theme theme1 = new A.Theme() { Name = "Тема Office" };
            theme1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

            A.ThemeElements themeElements1 = new A.ThemeElements();

            A.ColorScheme colorScheme1 = new A.ColorScheme() { Name = "Стандартная" };

            A.Dark1Color dark1Color1 = new A.Dark1Color();
            A.SystemColor systemColor1 = new A.SystemColor() { Val = A.SystemColorValues.WindowText, LastColor = "000000" };

            dark1Color1.Append(systemColor1);

            A.Light1Color light1Color1 = new A.Light1Color();
            A.SystemColor systemColor2 = new A.SystemColor() { Val = A.SystemColorValues.Window, LastColor = "FFFFFF" };

            light1Color1.Append(systemColor2);

            A.Dark2Color dark2Color1 = new A.Dark2Color();
            A.RgbColorModelHex rgbColorModelHex1 = new A.RgbColorModelHex() { Val = "44546A" };

            dark2Color1.Append(rgbColorModelHex1);

            A.Light2Color light2Color1 = new A.Light2Color();
            A.RgbColorModelHex rgbColorModelHex2 = new A.RgbColorModelHex() { Val = "E7E6E6" };

            light2Color1.Append(rgbColorModelHex2);

            A.Accent1Color accent1Color1 = new A.Accent1Color();
            A.RgbColorModelHex rgbColorModelHex3 = new A.RgbColorModelHex() { Val = "5B9BD5" };

            accent1Color1.Append(rgbColorModelHex3);

            A.Accent2Color accent2Color1 = new A.Accent2Color();
            A.RgbColorModelHex rgbColorModelHex4 = new A.RgbColorModelHex() { Val = "ED7D31" };

            accent2Color1.Append(rgbColorModelHex4);

            A.Accent3Color accent3Color1 = new A.Accent3Color();
            A.RgbColorModelHex rgbColorModelHex5 = new A.RgbColorModelHex() { Val = "A5A5A5" };

            accent3Color1.Append(rgbColorModelHex5);

            A.Accent4Color accent4Color1 = new A.Accent4Color();
            A.RgbColorModelHex rgbColorModelHex6 = new A.RgbColorModelHex() { Val = "FFC000" };

            accent4Color1.Append(rgbColorModelHex6);

            A.Accent5Color accent5Color1 = new A.Accent5Color();
            A.RgbColorModelHex rgbColorModelHex7 = new A.RgbColorModelHex() { Val = "4472C4" };

            accent5Color1.Append(rgbColorModelHex7);

            A.Accent6Color accent6Color1 = new A.Accent6Color();
            A.RgbColorModelHex rgbColorModelHex8 = new A.RgbColorModelHex() { Val = "70AD47" };

            accent6Color1.Append(rgbColorModelHex8);

            A.Hyperlink hyperlink1 = new A.Hyperlink();
            A.RgbColorModelHex rgbColorModelHex9 = new A.RgbColorModelHex() { Val = "0563C1" };

            hyperlink1.Append(rgbColorModelHex9);

            A.FollowedHyperlinkColor followedHyperlinkColor1 = new A.FollowedHyperlinkColor();
            A.RgbColorModelHex rgbColorModelHex10 = new A.RgbColorModelHex() { Val = "954F72" };

            followedHyperlinkColor1.Append(rgbColorModelHex10);

            colorScheme1.Append(dark1Color1);
            colorScheme1.Append(light1Color1);
            colorScheme1.Append(dark2Color1);
            colorScheme1.Append(light2Color1);
            colorScheme1.Append(accent1Color1);
            colorScheme1.Append(accent2Color1);
            colorScheme1.Append(accent3Color1);
            colorScheme1.Append(accent4Color1);
            colorScheme1.Append(accent5Color1);
            colorScheme1.Append(accent6Color1);
            colorScheme1.Append(hyperlink1);
            colorScheme1.Append(followedHyperlinkColor1);

            A.FontScheme fontScheme1 = new A.FontScheme() { Name = "Стандартная" };

            A.MajorFont majorFont1 = new A.MajorFont();
            A.LatinFont latinFont1 = new A.LatinFont() { Typeface = "Calibri Light", Panose = "020F0302020204030204" };
            A.EastAsianFont eastAsianFont1 = new A.EastAsianFont() { Typeface = "" };
            A.ComplexScriptFont complexScriptFont1 = new A.ComplexScriptFont() { Typeface = "" };
            A.SupplementalFont supplementalFont1 = new A.SupplementalFont() { Script = "Jpan", Typeface = "ＭＳ Ｐゴシック" };
            A.SupplementalFont supplementalFont2 = new A.SupplementalFont() { Script = "Hang", Typeface = "맑은 고딕" };
            A.SupplementalFont supplementalFont3 = new A.SupplementalFont() { Script = "Hans", Typeface = "宋体" };
            A.SupplementalFont supplementalFont4 = new A.SupplementalFont() { Script = "Hant", Typeface = "新細明體" };
            A.SupplementalFont supplementalFont5 = new A.SupplementalFont() { Script = "Arab", Typeface = "Times New Roman" };
            A.SupplementalFont supplementalFont6 = new A.SupplementalFont() { Script = "Hebr", Typeface = "Times New Roman" };
            A.SupplementalFont supplementalFont7 = new A.SupplementalFont() { Script = "Thai", Typeface = "Tahoma" };
            A.SupplementalFont supplementalFont8 = new A.SupplementalFont() { Script = "Ethi", Typeface = "Nyala" };
            A.SupplementalFont supplementalFont9 = new A.SupplementalFont() { Script = "Beng", Typeface = "Vrinda" };
            A.SupplementalFont supplementalFont10 = new A.SupplementalFont() { Script = "Gujr", Typeface = "Shruti" };
            A.SupplementalFont supplementalFont11 = new A.SupplementalFont() { Script = "Khmr", Typeface = "MoolBoran" };
            A.SupplementalFont supplementalFont12 = new A.SupplementalFont() { Script = "Knda", Typeface = "Tunga" };
            A.SupplementalFont supplementalFont13 = new A.SupplementalFont() { Script = "Guru", Typeface = "Raavi" };
            A.SupplementalFont supplementalFont14 = new A.SupplementalFont() { Script = "Cans", Typeface = "Euphemia" };
            A.SupplementalFont supplementalFont15 = new A.SupplementalFont() { Script = "Cher", Typeface = "Plantagenet Cherokee" };
            A.SupplementalFont supplementalFont16 = new A.SupplementalFont() { Script = "Yiii", Typeface = "Microsoft Yi Baiti" };
            A.SupplementalFont supplementalFont17 = new A.SupplementalFont() { Script = "Tibt", Typeface = "Microsoft Himalaya" };
            A.SupplementalFont supplementalFont18 = new A.SupplementalFont() { Script = "Thaa", Typeface = "MV Boli" };
            A.SupplementalFont supplementalFont19 = new A.SupplementalFont() { Script = "Deva", Typeface = "Mangal" };
            A.SupplementalFont supplementalFont20 = new A.SupplementalFont() { Script = "Telu", Typeface = "Gautami" };
            A.SupplementalFont supplementalFont21 = new A.SupplementalFont() { Script = "Taml", Typeface = "Latha" };
            A.SupplementalFont supplementalFont22 = new A.SupplementalFont() { Script = "Syrc", Typeface = "Estrangelo Edessa" };
            A.SupplementalFont supplementalFont23 = new A.SupplementalFont() { Script = "Orya", Typeface = "Kalinga" };
            A.SupplementalFont supplementalFont24 = new A.SupplementalFont() { Script = "Mlym", Typeface = "Kartika" };
            A.SupplementalFont supplementalFont25 = new A.SupplementalFont() { Script = "Laoo", Typeface = "DokChampa" };
            A.SupplementalFont supplementalFont26 = new A.SupplementalFont() { Script = "Sinh", Typeface = "Iskoola Pota" };
            A.SupplementalFont supplementalFont27 = new A.SupplementalFont() { Script = "Mong", Typeface = "Mongolian Baiti" };
            A.SupplementalFont supplementalFont28 = new A.SupplementalFont() { Script = "Viet", Typeface = "Times New Roman" };
            A.SupplementalFont supplementalFont29 = new A.SupplementalFont() { Script = "Uigh", Typeface = "Microsoft Uighur" };
            A.SupplementalFont supplementalFont30 = new A.SupplementalFont() { Script = "Geor", Typeface = "Sylfaen" };

            majorFont1.Append(latinFont1);
            majorFont1.Append(eastAsianFont1);
            majorFont1.Append(complexScriptFont1);
            majorFont1.Append(supplementalFont1);
            majorFont1.Append(supplementalFont2);
            majorFont1.Append(supplementalFont3);
            majorFont1.Append(supplementalFont4);
            majorFont1.Append(supplementalFont5);
            majorFont1.Append(supplementalFont6);
            majorFont1.Append(supplementalFont7);
            majorFont1.Append(supplementalFont8);
            majorFont1.Append(supplementalFont9);
            majorFont1.Append(supplementalFont10);
            majorFont1.Append(supplementalFont11);
            majorFont1.Append(supplementalFont12);
            majorFont1.Append(supplementalFont13);
            majorFont1.Append(supplementalFont14);
            majorFont1.Append(supplementalFont15);
            majorFont1.Append(supplementalFont16);
            majorFont1.Append(supplementalFont17);
            majorFont1.Append(supplementalFont18);
            majorFont1.Append(supplementalFont19);
            majorFont1.Append(supplementalFont20);
            majorFont1.Append(supplementalFont21);
            majorFont1.Append(supplementalFont22);
            majorFont1.Append(supplementalFont23);
            majorFont1.Append(supplementalFont24);
            majorFont1.Append(supplementalFont25);
            majorFont1.Append(supplementalFont26);
            majorFont1.Append(supplementalFont27);
            majorFont1.Append(supplementalFont28);
            majorFont1.Append(supplementalFont29);
            majorFont1.Append(supplementalFont30);

            A.MinorFont minorFont1 = new A.MinorFont();
            A.LatinFont latinFont2 = new A.LatinFont() { Typeface = "Calibri", Panose = "020F0502020204030204" };
            A.EastAsianFont eastAsianFont2 = new A.EastAsianFont() { Typeface = "" };
            A.ComplexScriptFont complexScriptFont2 = new A.ComplexScriptFont() { Typeface = "" };
            A.SupplementalFont supplementalFont31 = new A.SupplementalFont() { Script = "Jpan", Typeface = "ＭＳ Ｐゴシック" };
            A.SupplementalFont supplementalFont32 = new A.SupplementalFont() { Script = "Hang", Typeface = "맑은 고딕" };
            A.SupplementalFont supplementalFont33 = new A.SupplementalFont() { Script = "Hans", Typeface = "宋体" };
            A.SupplementalFont supplementalFont34 = new A.SupplementalFont() { Script = "Hant", Typeface = "新細明體" };
            A.SupplementalFont supplementalFont35 = new A.SupplementalFont() { Script = "Arab", Typeface = "Arial" };
            A.SupplementalFont supplementalFont36 = new A.SupplementalFont() { Script = "Hebr", Typeface = "Arial" };
            A.SupplementalFont supplementalFont37 = new A.SupplementalFont() { Script = "Thai", Typeface = "Tahoma" };
            A.SupplementalFont supplementalFont38 = new A.SupplementalFont() { Script = "Ethi", Typeface = "Nyala" };
            A.SupplementalFont supplementalFont39 = new A.SupplementalFont() { Script = "Beng", Typeface = "Vrinda" };
            A.SupplementalFont supplementalFont40 = new A.SupplementalFont() { Script = "Gujr", Typeface = "Shruti" };
            A.SupplementalFont supplementalFont41 = new A.SupplementalFont() { Script = "Khmr", Typeface = "DaunPenh" };
            A.SupplementalFont supplementalFont42 = new A.SupplementalFont() { Script = "Knda", Typeface = "Tunga" };
            A.SupplementalFont supplementalFont43 = new A.SupplementalFont() { Script = "Guru", Typeface = "Raavi" };
            A.SupplementalFont supplementalFont44 = new A.SupplementalFont() { Script = "Cans", Typeface = "Euphemia" };
            A.SupplementalFont supplementalFont45 = new A.SupplementalFont() { Script = "Cher", Typeface = "Plantagenet Cherokee" };
            A.SupplementalFont supplementalFont46 = new A.SupplementalFont() { Script = "Yiii", Typeface = "Microsoft Yi Baiti" };
            A.SupplementalFont supplementalFont47 = new A.SupplementalFont() { Script = "Tibt", Typeface = "Microsoft Himalaya" };
            A.SupplementalFont supplementalFont48 = new A.SupplementalFont() { Script = "Thaa", Typeface = "MV Boli" };
            A.SupplementalFont supplementalFont49 = new A.SupplementalFont() { Script = "Deva", Typeface = "Mangal" };
            A.SupplementalFont supplementalFont50 = new A.SupplementalFont() { Script = "Telu", Typeface = "Gautami" };
            A.SupplementalFont supplementalFont51 = new A.SupplementalFont() { Script = "Taml", Typeface = "Latha" };
            A.SupplementalFont supplementalFont52 = new A.SupplementalFont() { Script = "Syrc", Typeface = "Estrangelo Edessa" };
            A.SupplementalFont supplementalFont53 = new A.SupplementalFont() { Script = "Orya", Typeface = "Kalinga" };
            A.SupplementalFont supplementalFont54 = new A.SupplementalFont() { Script = "Mlym", Typeface = "Kartika" };
            A.SupplementalFont supplementalFont55 = new A.SupplementalFont() { Script = "Laoo", Typeface = "DokChampa" };
            A.SupplementalFont supplementalFont56 = new A.SupplementalFont() { Script = "Sinh", Typeface = "Iskoola Pota" };
            A.SupplementalFont supplementalFont57 = new A.SupplementalFont() { Script = "Mong", Typeface = "Mongolian Baiti" };
            A.SupplementalFont supplementalFont58 = new A.SupplementalFont() { Script = "Viet", Typeface = "Arial" };
            A.SupplementalFont supplementalFont59 = new A.SupplementalFont() { Script = "Uigh", Typeface = "Microsoft Uighur" };
            A.SupplementalFont supplementalFont60 = new A.SupplementalFont() { Script = "Geor", Typeface = "Sylfaen" };

            minorFont1.Append(latinFont2);
            minorFont1.Append(eastAsianFont2);
            minorFont1.Append(complexScriptFont2);
            minorFont1.Append(supplementalFont31);
            minorFont1.Append(supplementalFont32);
            minorFont1.Append(supplementalFont33);
            minorFont1.Append(supplementalFont34);
            minorFont1.Append(supplementalFont35);
            minorFont1.Append(supplementalFont36);
            minorFont1.Append(supplementalFont37);
            minorFont1.Append(supplementalFont38);
            minorFont1.Append(supplementalFont39);
            minorFont1.Append(supplementalFont40);
            minorFont1.Append(supplementalFont41);
            minorFont1.Append(supplementalFont42);
            minorFont1.Append(supplementalFont43);
            minorFont1.Append(supplementalFont44);
            minorFont1.Append(supplementalFont45);
            minorFont1.Append(supplementalFont46);
            minorFont1.Append(supplementalFont47);
            minorFont1.Append(supplementalFont48);
            minorFont1.Append(supplementalFont49);
            minorFont1.Append(supplementalFont50);
            minorFont1.Append(supplementalFont51);
            minorFont1.Append(supplementalFont52);
            minorFont1.Append(supplementalFont53);
            minorFont1.Append(supplementalFont54);
            minorFont1.Append(supplementalFont55);
            minorFont1.Append(supplementalFont56);
            minorFont1.Append(supplementalFont57);
            minorFont1.Append(supplementalFont58);
            minorFont1.Append(supplementalFont59);
            minorFont1.Append(supplementalFont60);

            fontScheme1.Append(majorFont1);
            fontScheme1.Append(minorFont1);

            A.FormatScheme formatScheme1 = new A.FormatScheme() { Name = "Стандартная" };

            A.FillStyleList fillStyleList1 = new A.FillStyleList();

            A.SolidFill solidFill1 = new A.SolidFill();
            A.SchemeColor schemeColor1 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

            solidFill1.Append(schemeColor1);

            A.GradientFill gradientFill1 = new A.GradientFill() { RotateWithShape = true };

            A.GradientStopList gradientStopList1 = new A.GradientStopList();

            A.GradientStop gradientStop1 = new A.GradientStop() { Position = 0 };

            A.SchemeColor schemeColor2 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.LuminanceModulation luminanceModulation1 = new A.LuminanceModulation() { Val = 110000 };
            A.SaturationModulation saturationModulation1 = new A.SaturationModulation() { Val = 105000 };
            A.Tint tint1 = new A.Tint() { Val = 67000 };

            schemeColor2.Append(luminanceModulation1);
            schemeColor2.Append(saturationModulation1);
            schemeColor2.Append(tint1);

            gradientStop1.Append(schemeColor2);

            A.GradientStop gradientStop2 = new A.GradientStop() { Position = 50000 };

            A.SchemeColor schemeColor3 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.LuminanceModulation luminanceModulation2 = new A.LuminanceModulation() { Val = 105000 };
            A.SaturationModulation saturationModulation2 = new A.SaturationModulation() { Val = 103000 };
            A.Tint tint2 = new A.Tint() { Val = 73000 };

            schemeColor3.Append(luminanceModulation2);
            schemeColor3.Append(saturationModulation2);
            schemeColor3.Append(tint2);

            gradientStop2.Append(schemeColor3);

            A.GradientStop gradientStop3 = new A.GradientStop() { Position = 100000 };

            A.SchemeColor schemeColor4 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.LuminanceModulation luminanceModulation3 = new A.LuminanceModulation() { Val = 105000 };
            A.SaturationModulation saturationModulation3 = new A.SaturationModulation() { Val = 109000 };
            A.Tint tint3 = new A.Tint() { Val = 81000 };

            schemeColor4.Append(luminanceModulation3);
            schemeColor4.Append(saturationModulation3);
            schemeColor4.Append(tint3);

            gradientStop3.Append(schemeColor4);

            gradientStopList1.Append(gradientStop1);
            gradientStopList1.Append(gradientStop2);
            gradientStopList1.Append(gradientStop3);
            A.LinearGradientFill linearGradientFill1 = new A.LinearGradientFill() { Angle = 5400000, Scaled = false };

            gradientFill1.Append(gradientStopList1);
            gradientFill1.Append(linearGradientFill1);

            A.GradientFill gradientFill2 = new A.GradientFill() { RotateWithShape = true };

            A.GradientStopList gradientStopList2 = new A.GradientStopList();

            A.GradientStop gradientStop4 = new A.GradientStop() { Position = 0 };

            A.SchemeColor schemeColor5 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.SaturationModulation saturationModulation4 = new A.SaturationModulation() { Val = 103000 };
            A.LuminanceModulation luminanceModulation4 = new A.LuminanceModulation() { Val = 102000 };
            A.Tint tint4 = new A.Tint() { Val = 94000 };

            schemeColor5.Append(saturationModulation4);
            schemeColor5.Append(luminanceModulation4);
            schemeColor5.Append(tint4);

            gradientStop4.Append(schemeColor5);

            A.GradientStop gradientStop5 = new A.GradientStop() { Position = 50000 };

            A.SchemeColor schemeColor6 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.SaturationModulation saturationModulation5 = new A.SaturationModulation() { Val = 110000 };
            A.LuminanceModulation luminanceModulation5 = new A.LuminanceModulation() { Val = 100000 };
            A.Shade shade1 = new A.Shade() { Val = 100000 };

            schemeColor6.Append(saturationModulation5);
            schemeColor6.Append(luminanceModulation5);
            schemeColor6.Append(shade1);

            gradientStop5.Append(schemeColor6);

            A.GradientStop gradientStop6 = new A.GradientStop() { Position = 100000 };

            A.SchemeColor schemeColor7 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.LuminanceModulation luminanceModulation6 = new A.LuminanceModulation() { Val = 99000 };
            A.SaturationModulation saturationModulation6 = new A.SaturationModulation() { Val = 120000 };
            A.Shade shade2 = new A.Shade() { Val = 78000 };

            schemeColor7.Append(luminanceModulation6);
            schemeColor7.Append(saturationModulation6);
            schemeColor7.Append(shade2);

            gradientStop6.Append(schemeColor7);

            gradientStopList2.Append(gradientStop4);
            gradientStopList2.Append(gradientStop5);
            gradientStopList2.Append(gradientStop6);
            A.LinearGradientFill linearGradientFill2 = new A.LinearGradientFill() { Angle = 5400000, Scaled = false };

            gradientFill2.Append(gradientStopList2);
            gradientFill2.Append(linearGradientFill2);

            fillStyleList1.Append(solidFill1);
            fillStyleList1.Append(gradientFill1);
            fillStyleList1.Append(gradientFill2);

            A.LineStyleList lineStyleList1 = new A.LineStyleList();

            A.Outline outline1 = new A.Outline() { Width = 6350, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

            A.SolidFill solidFill2 = new A.SolidFill();
            A.SchemeColor schemeColor8 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

            solidFill2.Append(schemeColor8);
            A.PresetDash presetDash1 = new A.PresetDash() { Val = A.PresetLineDashValues.Solid };
            A.Miter miter1 = new A.Miter() { Limit = 800000 };

            outline1.Append(solidFill2);
            outline1.Append(presetDash1);
            outline1.Append(miter1);

            A.Outline outline2 = new A.Outline() { Width = 12700, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

            A.SolidFill solidFill3 = new A.SolidFill();
            A.SchemeColor schemeColor9 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

            solidFill3.Append(schemeColor9);
            A.PresetDash presetDash2 = new A.PresetDash() { Val = A.PresetLineDashValues.Solid };
            A.Miter miter2 = new A.Miter() { Limit = 800000 };

            outline2.Append(solidFill3);
            outline2.Append(presetDash2);
            outline2.Append(miter2);

            A.Outline outline3 = new A.Outline() { Width = 19050, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

            A.SolidFill solidFill4 = new A.SolidFill();
            A.SchemeColor schemeColor10 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

            solidFill4.Append(schemeColor10);
            A.PresetDash presetDash3 = new A.PresetDash() { Val = A.PresetLineDashValues.Solid };
            A.Miter miter3 = new A.Miter() { Limit = 800000 };

            outline3.Append(solidFill4);
            outline3.Append(presetDash3);
            outline3.Append(miter3);

            lineStyleList1.Append(outline1);
            lineStyleList1.Append(outline2);
            lineStyleList1.Append(outline3);

            A.EffectStyleList effectStyleList1 = new A.EffectStyleList();

            A.EffectStyle effectStyle1 = new A.EffectStyle();
            A.EffectList effectList1 = new A.EffectList();

            effectStyle1.Append(effectList1);

            A.EffectStyle effectStyle2 = new A.EffectStyle();
            A.EffectList effectList2 = new A.EffectList();

            effectStyle2.Append(effectList2);

            A.EffectStyle effectStyle3 = new A.EffectStyle();

            A.EffectList effectList3 = new A.EffectList();

            A.OuterShadow outerShadow1 = new A.OuterShadow() { BlurRadius = 57150L, Distance = 19050L, Direction = 5400000, Alignment = A.RectangleAlignmentValues.Center, RotateWithShape = false };

            A.RgbColorModelHex rgbColorModelHex11 = new A.RgbColorModelHex() { Val = "000000" };
            A.Alpha alpha1 = new A.Alpha() { Val = 63000 };

            rgbColorModelHex11.Append(alpha1);

            outerShadow1.Append(rgbColorModelHex11);

            effectList3.Append(outerShadow1);

            effectStyle3.Append(effectList3);

            effectStyleList1.Append(effectStyle1);
            effectStyleList1.Append(effectStyle2);
            effectStyleList1.Append(effectStyle3);

            A.BackgroundFillStyleList backgroundFillStyleList1 = new A.BackgroundFillStyleList();

            A.SolidFill solidFill5 = new A.SolidFill();
            A.SchemeColor schemeColor11 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

            solidFill5.Append(schemeColor11);

            A.SolidFill solidFill6 = new A.SolidFill();

            A.SchemeColor schemeColor12 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Tint tint5 = new A.Tint() { Val = 95000 };
            A.SaturationModulation saturationModulation7 = new A.SaturationModulation() { Val = 170000 };

            schemeColor12.Append(tint5);
            schemeColor12.Append(saturationModulation7);

            solidFill6.Append(schemeColor12);

            A.GradientFill gradientFill3 = new A.GradientFill() { RotateWithShape = true };

            A.GradientStopList gradientStopList3 = new A.GradientStopList();

            A.GradientStop gradientStop7 = new A.GradientStop() { Position = 0 };

            A.SchemeColor schemeColor13 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Tint tint6 = new A.Tint() { Val = 93000 };
            A.SaturationModulation saturationModulation8 = new A.SaturationModulation() { Val = 150000 };
            A.Shade shade3 = new A.Shade() { Val = 98000 };
            A.LuminanceModulation luminanceModulation7 = new A.LuminanceModulation() { Val = 102000 };

            schemeColor13.Append(tint6);
            schemeColor13.Append(saturationModulation8);
            schemeColor13.Append(shade3);
            schemeColor13.Append(luminanceModulation7);

            gradientStop7.Append(schemeColor13);

            A.GradientStop gradientStop8 = new A.GradientStop() { Position = 50000 };

            A.SchemeColor schemeColor14 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Tint tint7 = new A.Tint() { Val = 98000 };
            A.SaturationModulation saturationModulation9 = new A.SaturationModulation() { Val = 130000 };
            A.Shade shade4 = new A.Shade() { Val = 90000 };
            A.LuminanceModulation luminanceModulation8 = new A.LuminanceModulation() { Val = 103000 };

            schemeColor14.Append(tint7);
            schemeColor14.Append(saturationModulation9);
            schemeColor14.Append(shade4);
            schemeColor14.Append(luminanceModulation8);

            gradientStop8.Append(schemeColor14);

            A.GradientStop gradientStop9 = new A.GradientStop() { Position = 100000 };

            A.SchemeColor schemeColor15 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Shade shade5 = new A.Shade() { Val = 63000 };
            A.SaturationModulation saturationModulation10 = new A.SaturationModulation() { Val = 120000 };

            schemeColor15.Append(shade5);
            schemeColor15.Append(saturationModulation10);

            gradientStop9.Append(schemeColor15);

            gradientStopList3.Append(gradientStop7);
            gradientStopList3.Append(gradientStop8);
            gradientStopList3.Append(gradientStop9);
            A.LinearGradientFill linearGradientFill3 = new A.LinearGradientFill() { Angle = 5400000, Scaled = false };

            gradientFill3.Append(gradientStopList3);
            gradientFill3.Append(linearGradientFill3);

            backgroundFillStyleList1.Append(solidFill5);
            backgroundFillStyleList1.Append(solidFill6);
            backgroundFillStyleList1.Append(gradientFill3);

            formatScheme1.Append(fillStyleList1);
            formatScheme1.Append(lineStyleList1);
            formatScheme1.Append(effectStyleList1);
            formatScheme1.Append(backgroundFillStyleList1);

            themeElements1.Append(colorScheme1);
            themeElements1.Append(fontScheme1);
            themeElements1.Append(formatScheme1);
            A.ObjectDefaults objectDefaults1 = new A.ObjectDefaults();
            A.ExtraColorSchemeList extraColorSchemeList1 = new A.ExtraColorSchemeList();

            A.OfficeStyleSheetExtensionList officeStyleSheetExtensionList1 = new A.OfficeStyleSheetExtensionList();

            A.OfficeStyleSheetExtension officeStyleSheetExtension1 = new A.OfficeStyleSheetExtension() { Uri = "{05A4C25C-085E-4340-85A3-A5531E510DB2}" };

            Thm15.ThemeFamily themeFamily1 = new Thm15.ThemeFamily() { Name = "Office Theme", Id = "{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}", Vid = "{4A3C46E8-61CC-4603-A589-7422A47A8E4A}" };
            themeFamily1.AddNamespaceDeclaration("thm15", "http://schemas.microsoft.com/office/thememl/2012/main");

            officeStyleSheetExtension1.Append(themeFamily1);

            officeStyleSheetExtensionList1.Append(officeStyleSheetExtension1);

            theme1.Append(themeElements1);
            theme1.Append(objectDefaults1);
            theme1.Append(extraColorSchemeList1);
            theme1.Append(officeStyleSheetExtensionList1);

            themePart1.Theme = theme1;
        }

        #endregion

        // Generates content of worksheetPart1.
        private void GenerateWorksheetPart1Content(WorksheetPart worksheetPart1)
        {
            #region Страница и колонки

            Worksheet worksheet1 = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            worksheet1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            SheetDimension sheetDimension1 = new SheetDimension() { Reference = "A1:AB4" };

            SheetViews sheetViews1 = new SheetViews();
            SheetView sheetView1 = new SheetView() { WorkbookViewId = (UInt32Value)0U };

            sheetViews1.Append(sheetView1);
            SheetFormatProperties sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, DyDescent = 0.25D };

            Columns columns1 = new Columns();
            Column column1 = new Column() { Min = (UInt32Value)1U, Max = (UInt32Value)1U, Width = 4D, CustomWidth = true };
            Column column2 = new Column() { Min = (UInt32Value)2U, Max = (UInt32Value)2U, Width = 15.28515625D, CustomWidth = true };
            Column column3 = new Column() { Min = (UInt32Value)3U, Max = (UInt32Value)3U, Width = 8.28515625D, CustomWidth = true };
            Column column4 = new Column() { Min = (UInt32Value)4U, Max = (UInt32Value)4U, Width = 8.7109375D, CustomWidth = true };
            Column column5 = new Column() { Min = (UInt32Value)5U, Max = (UInt32Value)5U, Width = 9.42578125D, BestFit = true, CustomWidth = true };
            Column column6 = new Column() { Min = (UInt32Value)6U, Max = (UInt32Value)9U, Width = 9D, CustomWidth = true };
            Column column7 = new Column() { Min = (UInt32Value)10U, Max = (UInt32Value)11U, Width = 9.42578125D, BestFit = true, CustomWidth = true };
            Column column8 = new Column() { Min = (UInt32Value)12U, Max = (UInt32Value)13U, Width = 8.85546875D, CustomWidth = true };
            Column column9 = new Column() { Min = (UInt32Value)14U, Max = (UInt32Value)21U, Width = 9D, CustomWidth = true };
            Column column10 = new Column() { Min = (UInt32Value)26U, Max = (UInt32Value)26U, Width = 9.42578125D, BestFit = true, CustomWidth = true };

            columns1.Append(column1);
            columns1.Append(column2);
            columns1.Append(column3);
            columns1.Append(column4);
            columns1.Append(column5);
            columns1.Append(column6);
            columns1.Append(column7);
            columns1.Append(column8);
            columns1.Append(column9);
            columns1.Append(column10);

            #endregion

            SheetData sheetData1 = new SheetData();

            #region Названия столбцов

            Row row1 = new Row() { RowIndex = (UInt32Value)1U, Spans = new ListValue<StringValue>() { InnerText = "1:28" }, Height = 75D, DyDescent = 0.25D };

            Cell cell1 = new Cell() { CellReference = "A1", StyleIndex = (UInt32Value)4U, DataType = CellValues.SharedString };
            CellValue cellValue1 = new CellValue();
            cellValue1.Text = "0";

            cell1.Append(cellValue1);

            Cell cell2 = new Cell() { CellReference = "B1", StyleIndex = (UInt32Value)4U, DataType = CellValues.SharedString };
            CellValue cellValue2 = new CellValue();
            cellValue2.Text = "1";

            cell2.Append(cellValue2);

            Cell cell3 = new Cell() { CellReference = "C1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue3 = new CellValue();
            cellValue3.Text = "2";

            cell3.Append(cellValue3);

            Cell cell4 = new Cell() { CellReference = "D1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue4 = new CellValue();
            cellValue4.Text = "3";

            cell4.Append(cellValue4);

            Cell cell5 = new Cell() { CellReference = "E1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue5 = new CellValue();
            cellValue5.Text = "4";

            cell5.Append(cellValue5);

            Cell cell6 = new Cell() { CellReference = "F1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue6 = new CellValue();
            cellValue6.Text = "5";

            cell6.Append(cellValue6);

            Cell cell7 = new Cell() { CellReference = "G1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue7 = new CellValue();
            cellValue7.Text = "6";

            cell7.Append(cellValue7);

            Cell cell8 = new Cell() { CellReference = "H1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue8 = new CellValue();
            cellValue8.Text = "7";

            cell8.Append(cellValue8);

            Cell cell9 = new Cell() { CellReference = "I1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue9 = new CellValue();
            cellValue9.Text = "8";

            cell9.Append(cellValue9);

            Cell cell10 = new Cell() { CellReference = "J1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue10 = new CellValue();
            cellValue10.Text = "9";

            cell10.Append(cellValue10);

            Cell cell11 = new Cell() { CellReference = "K1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue11 = new CellValue();
            cellValue11.Text = "10";

            cell11.Append(cellValue11);

            Cell cell12 = new Cell() { CellReference = "L1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue12 = new CellValue();
            cellValue12.Text = "11";

            cell12.Append(cellValue12);

            Cell cell13 = new Cell() { CellReference = "M1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue13 = new CellValue();
            cellValue13.Text = "12";

            cell13.Append(cellValue13);

            Cell cell14 = new Cell() { CellReference = "N1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue14 = new CellValue();
            cellValue14.Text = "13";

            cell14.Append(cellValue14);

            Cell cell15 = new Cell() { CellReference = "O1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue15 = new CellValue();
            cellValue15.Text = "14";

            cell15.Append(cellValue15);

            Cell cell16 = new Cell() { CellReference = "P1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue16 = new CellValue();
            cellValue16.Text = "15";

            cell16.Append(cellValue16);

            Cell cell17 = new Cell() { CellReference = "Q1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue17 = new CellValue();
            cellValue17.Text = "16";

            cell17.Append(cellValue17);

            Cell cell18 = new Cell() { CellReference = "R1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue18 = new CellValue();
            cellValue18.Text = "17";

            cell18.Append(cellValue18);

            Cell cell19 = new Cell() { CellReference = "S1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue19 = new CellValue();
            cellValue19.Text = "18";

            cell19.Append(cellValue19);

            Cell cell20 = new Cell() { CellReference = "T1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue20 = new CellValue();
            cellValue20.Text = "19";

            cell20.Append(cellValue20);

            Cell cell21 = new Cell() { CellReference = "U1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue21 = new CellValue();
            cellValue21.Text = "20";

            cell21.Append(cellValue21);

            Cell cell22 = new Cell() { CellReference = "V1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue22 = new CellValue();
            cellValue22.Text = "21";

            cell22.Append(cellValue22);

            Cell cell23 = new Cell() { CellReference = "W1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue23 = new CellValue();
            cellValue23.Text = "22";

            cell23.Append(cellValue23);

            Cell cell24 = new Cell() { CellReference = "X1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue24 = new CellValue();
            cellValue24.Text = "23";

            cell24.Append(cellValue24);

            Cell cell25 = new Cell() { CellReference = "Y1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue25 = new CellValue();
            cellValue25.Text = "24";

            cell25.Append(cellValue25);

            Cell cell26 = new Cell() { CellReference = "Z1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue26 = new CellValue();
            cellValue26.Text = "25";

            cell26.Append(cellValue26);

            Cell cell27 = new Cell() { CellReference = "AA1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue27 = new CellValue();
            cellValue27.Text = "27";

            cell27.Append(cellValue27);

            Cell cell28 = new Cell() { CellReference = "AB1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue28 = new CellValue();
            cellValue28.Text = "28";

            cell28.Append(cellValue28);

            row1.Append(cell1);
            row1.Append(cell2);
            row1.Append(cell3);
            row1.Append(cell4);
            row1.Append(cell5);
            row1.Append(cell6);
            row1.Append(cell7);
            row1.Append(cell8);
            row1.Append(cell9);
            row1.Append(cell10);
            row1.Append(cell11);
            row1.Append(cell12);
            row1.Append(cell13);
            row1.Append(cell14);
            row1.Append(cell15);
            row1.Append(cell16);
            row1.Append(cell17);
            row1.Append(cell18);
            row1.Append(cell19);
            row1.Append(cell20);
            row1.Append(cell21);
            row1.Append(cell22);
            row1.Append(cell23);
            row1.Append(cell24);
            row1.Append(cell25);
            row1.Append(cell26);
            row1.Append(cell27);
            row1.Append(cell28);


            #endregion

            sheetData1.Append(row1);

            int i = 0;
            foreach (var mix in NightMixes)
            {
                AddNewPartRow(sheetData1, i, mix);
                i++;
            }

            #region Ненужная строка

            Row row3 = new Row() { RowIndex = (UInt32Value)3U, Spans = new ListValue<StringValue>() { InnerText = "1:28" }, DyDescent = 0.25D };

            Cell cell57 = new Cell() { CellReference = "A3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue57 = new CellValue();
            cellValue57.Text = "105";

            cell57.Append(cellValue57);

            Cell cell58 = new Cell() { CellReference = "B3", StyleIndex = (UInt32Value)2U };
            CellValue cellValue58 = new CellValue();
            cellValue58.Text = "44326.831828703704";

            cell58.Append(cellValue58);

            Cell cell59 = new Cell() { CellReference = "C3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue59 = new CellValue();
            cellValue59.Text = "17";

            cell59.Append(cellValue59);

            Cell cell60 = new Cell() { CellReference = "D3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue60 = new CellValue();
            cellValue60.Text = "2001";

            cell60.Append(cellValue60);

            Cell cell61 = new Cell() { CellReference = "E3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue61 = new CellValue();
            cellValue61.Text = "43.2";

            cell61.Append(cellValue61);

            Cell cell62 = new Cell() { CellReference = "F3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue62 = new CellValue();
            cellValue62.Text = "1161.71";

            cell62.Append(cellValue62);

            Cell cell63 = new Cell() { CellReference = "G3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue63 = new CellValue();
            cellValue63.Text = "1147";

            cell63.Append(cellValue63);

            Cell cell64 = new Cell() { CellReference = "H3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue64 = new CellValue();
            cellValue64.Text = "2193.34";

            cell64.Append(cellValue64);

            Cell cell65 = new Cell() { CellReference = "I3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue65 = new CellValue();
            cellValue65.Text = "2193.1999999999998";

            cell65.Append(cellValue65);

            Cell cell66 = new Cell() { CellReference = "J3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue66 = new CellValue();
            cellValue66.Text = "280.77999999999997";

            cell66.Append(cellValue66);

            Cell cell67 = new Cell() { CellReference = "K3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue67 = new CellValue();
            cellValue67.Text = "275.8";

            cell67.Append(cellValue67);

            Cell cell68 = new Cell() { CellReference = "L3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue68 = new CellValue();
            cellValue68.Text = "23.31";

            cell68.Append(cellValue68);

            Cell cell69 = new Cell() { CellReference = "M3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue69 = new CellValue();
            cellValue69.Text = "23.6";

            cell69.Append(cellValue69);

            Cell cell70 = new Cell() { CellReference = "N3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue70 = new CellValue();
            cellValue70.Text = "0";

            cell70.Append(cellValue70);

            Cell cell71 = new Cell() { CellReference = "O3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue71 = new CellValue();
            cellValue71.Text = "0";

            cell71.Append(cellValue71);

            Cell cell72 = new Cell() { CellReference = "P3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue72 = new CellValue();
            cellValue72.Text = "687.82";

            cell72.Append(cellValue72);

            Cell cell73 = new Cell() { CellReference = "Q3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue73 = new CellValue();
            cellValue73.Text = "682.9";

            cell73.Append(cellValue73);

            Cell cell74 = new Cell() { CellReference = "R3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue74 = new CellValue();
            cellValue74.Text = "0";

            cell74.Append(cellValue74);

            Cell cell75 = new Cell() { CellReference = "S3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue75 = new CellValue();
            cellValue75.Text = "0";

            cell75.Append(cellValue75);

            Cell cell76 = new Cell() { CellReference = "T3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue76 = new CellValue();
            cellValue76.Text = "554.23";

            cell76.Append(cellValue76);

            Cell cell77 = new Cell() { CellReference = "U3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue77 = new CellValue();
            cellValue77.Text = "550.5";

            cell77.Append(cellValue77);

            Cell cell78 = new Cell() { CellReference = "V3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue78 = new CellValue();
            cellValue78.Text = "0";

            cell78.Append(cellValue78);

            Cell cell79 = new Cell() { CellReference = "W3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue79 = new CellValue();
            cellValue79.Text = "0";

            cell79.Append(cellValue79);

            Cell cell80 = new Cell() { CellReference = "X3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue80 = new CellValue();
            cellValue80.Text = "2.75";

            cell80.Append(cellValue80);

            Cell cell81 = new Cell() { CellReference = "Y3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue81 = new CellValue();
            cellValue81.Text = "2.714";

            cell81.Append(cellValue81);

            Cell cell82 = new Cell() { CellReference = "Z3", StyleIndex = (UInt32Value)1U };
            CellValue cellValue82 = new CellValue();
            cellValue82.Text = "1443.12";

            cell82.Append(cellValue82);

            Cell cell83 = new Cell() { CellReference = "AA3", StyleIndex = (UInt32Value)8U, DataType = CellValues.SharedString };
            CellValue cellValue83 = new CellValue();
            cellValue83.Text = "27";

            cell83.Append(cellValue83);
            Cell cell84 = new Cell() { CellReference = "AB3", StyleIndex = (UInt32Value)8U };

            row3.Append(cell57);
            row3.Append(cell58);
            row3.Append(cell59);
            row3.Append(cell60);
            row3.Append(cell61);
            row3.Append(cell62);
            row3.Append(cell63);
            row3.Append(cell64);
            row3.Append(cell65);
            row3.Append(cell66);
            row3.Append(cell67);
            row3.Append(cell68);
            row3.Append(cell69);
            row3.Append(cell70);
            row3.Append(cell71);
            row3.Append(cell72);
            row3.Append(cell73);
            row3.Append(cell74);
            row3.Append(cell75);
            row3.Append(cell76);
            row3.Append(cell77);
            row3.Append(cell78);
            row3.Append(cell79);
            row3.Append(cell80);
            row3.Append(cell81);
            row3.Append(cell82);
            row3.Append(cell83);
            row3.Append(cell84);

            #endregion

            AddNewResult(sheetData1, NightMixes.Count());

            PageMargins pageMargins1 = new PageMargins() { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };

            worksheet1.Append(sheetDimension1);
            worksheet1.Append(sheetViews1);
            worksheet1.Append(sheetFormatProperties1);
            worksheet1.Append(columns1);
            worksheet1.Append(sheetData1);
            worksheet1.Append(pageMargins1);

            worksheetPart1.Worksheet = worksheet1;
        }

        // Generates content of worksheetPart2.
        private void GenerateWorksheetPart2Content(WorksheetPart worksheetPart2)
        {
            #region Страница и колонки

            Worksheet worksheet2 = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            worksheet2.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet2.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet2.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            SheetDimension sheetDimension2 = new SheetDimension() { Reference = "A1:AB4" };

            SheetViews sheetViews2 = new SheetViews();
            SheetView sheetView2 = new SheetView() { TabSelected = true, WorkbookViewId = (UInt32Value)0U };

            sheetViews2.Append(sheetView2);
            SheetFormatProperties sheetFormatProperties2 = new SheetFormatProperties() { DefaultRowHeight = 15D, DyDescent = 0.25D };

            Columns columns2 = new Columns();
            Column column11 = new Column() { Min = (UInt32Value)1U, Max = (UInt32Value)1U, Width = 4D, CustomWidth = true };
            Column column12 = new Column() { Min = (UInt32Value)2U, Max = (UInt32Value)2U, Width = 15.28515625D, CustomWidth = true };
            Column column13 = new Column() { Min = (UInt32Value)3U, Max = (UInt32Value)3U, Width = 8.28515625D, CustomWidth = true };
            Column column14 = new Column() { Min = (UInt32Value)4U, Max = (UInt32Value)4U, Width = 8.7109375D, CustomWidth = true };
            Column column15 = new Column() { Min = (UInt32Value)5U, Max = (UInt32Value)5U, Width = 9.42578125D, BestFit = true, CustomWidth = true };
            Column column16 = new Column() { Min = (UInt32Value)6U, Max = (UInt32Value)9U, Width = 9D, CustomWidth = true };
            Column column17 = new Column() { Min = (UInt32Value)10U, Max = (UInt32Value)11U, Width = 9.42578125D, BestFit = true, CustomWidth = true };
            Column column18 = new Column() { Min = (UInt32Value)12U, Max = (UInt32Value)13U, Width = 8.85546875D, CustomWidth = true };
            Column column19 = new Column() { Min = (UInt32Value)14U, Max = (UInt32Value)21U, Width = 9D, CustomWidth = true };
            Column column20 = new Column() { Min = (UInt32Value)26U, Max = (UInt32Value)26U, Width = 9.42578125D, BestFit = true, CustomWidth = true };

            columns2.Append(column11);
            columns2.Append(column12);
            columns2.Append(column13);
            columns2.Append(column14);
            columns2.Append(column15);
            columns2.Append(column16);
            columns2.Append(column17);
            columns2.Append(column18);
            columns2.Append(column19);
            columns2.Append(column20);

            #endregion

            SheetData sheetData2 = new SheetData();

            #region Названия столбцов

            Row row5 = new Row() { RowIndex = (UInt32Value)1U, Spans = new ListValue<StringValue>() { InnerText = "1:28" }, Height = 75D, DyDescent = 0.25D };

            Cell cell113 = new Cell() { CellReference = "A1", StyleIndex = (UInt32Value)4U, DataType = CellValues.SharedString };
            CellValue cellValue106 = new CellValue();
            cellValue106.Text = "0";

            cell113.Append(cellValue106);

            Cell cell114 = new Cell() { CellReference = "B1", StyleIndex = (UInt32Value)4U, DataType = CellValues.SharedString };
            CellValue cellValue107 = new CellValue();
            cellValue107.Text = "1";

            cell114.Append(cellValue107);

            Cell cell115 = new Cell() { CellReference = "C1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue108 = new CellValue();
            cellValue108.Text = "2";

            cell115.Append(cellValue108);

            Cell cell116 = new Cell() { CellReference = "D1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue109 = new CellValue();
            cellValue109.Text = "3";

            cell116.Append(cellValue109);

            Cell cell117 = new Cell() { CellReference = "E1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue110 = new CellValue();
            cellValue110.Text = "4";

            cell117.Append(cellValue110);

            Cell cell118 = new Cell() { CellReference = "F1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue111 = new CellValue();
            cellValue111.Text = "5";

            cell118.Append(cellValue111);

            Cell cell119 = new Cell() { CellReference = "G1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue112 = new CellValue();
            cellValue112.Text = "6";

            cell119.Append(cellValue112);

            Cell cell120 = new Cell() { CellReference = "H1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue113 = new CellValue();
            cellValue113.Text = "7";

            cell120.Append(cellValue113);

            Cell cell121 = new Cell() { CellReference = "I1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue114 = new CellValue();
            cellValue114.Text = "8";

            cell121.Append(cellValue114);

            Cell cell122 = new Cell() { CellReference = "J1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue115 = new CellValue();
            cellValue115.Text = "9";

            cell122.Append(cellValue115);

            Cell cell123 = new Cell() { CellReference = "K1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue116 = new CellValue();
            cellValue116.Text = "10";

            cell123.Append(cellValue116);

            Cell cell124 = new Cell() { CellReference = "L1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue117 = new CellValue();
            cellValue117.Text = "11";

            cell124.Append(cellValue117);

            Cell cell125 = new Cell() { CellReference = "M1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue118 = new CellValue();
            cellValue118.Text = "12";

            cell125.Append(cellValue118);

            Cell cell126 = new Cell() { CellReference = "N1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue119 = new CellValue();
            cellValue119.Text = "13";

            cell126.Append(cellValue119);

            Cell cell127 = new Cell() { CellReference = "O1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue120 = new CellValue();
            cellValue120.Text = "14";

            cell127.Append(cellValue120);

            Cell cell128 = new Cell() { CellReference = "P1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue121 = new CellValue();
            cellValue121.Text = "15";

            cell128.Append(cellValue121);

            Cell cell129 = new Cell() { CellReference = "Q1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue122 = new CellValue();
            cellValue122.Text = "16";

            cell129.Append(cellValue122);

            Cell cell130 = new Cell() { CellReference = "R1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue123 = new CellValue();
            cellValue123.Text = "17";

            cell130.Append(cellValue123);

            Cell cell131 = new Cell() { CellReference = "S1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue124 = new CellValue();
            cellValue124.Text = "18";

            cell131.Append(cellValue124);

            Cell cell132 = new Cell() { CellReference = "T1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue125 = new CellValue();
            cellValue125.Text = "19";

            cell132.Append(cellValue125);

            Cell cell133 = new Cell() { CellReference = "U1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue126 = new CellValue();
            cellValue126.Text = "20";

            cell133.Append(cellValue126);

            Cell cell134 = new Cell() { CellReference = "V1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue127 = new CellValue();
            cellValue127.Text = "21";

            cell134.Append(cellValue127);

            Cell cell135 = new Cell() { CellReference = "W1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue128 = new CellValue();
            cellValue128.Text = "22";

            cell135.Append(cellValue128);

            Cell cell136 = new Cell() { CellReference = "X1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue129 = new CellValue();
            cellValue129.Text = "23";

            cell136.Append(cellValue129);

            Cell cell137 = new Cell() { CellReference = "Y1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue130 = new CellValue();
            cellValue130.Text = "24";

            cell137.Append(cellValue130);

            Cell cell138 = new Cell() { CellReference = "Z1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue131 = new CellValue();
            cellValue131.Text = "25";

            cell138.Append(cellValue131);

            Cell cell139 = new Cell() { CellReference = "AA1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue132 = new CellValue();
            cellValue132.Text = "27";

            cell139.Append(cellValue132);

            Cell cell140 = new Cell() { CellReference = "AB1", StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString };
            CellValue cellValue133 = new CellValue();
            cellValue133.Text = "28";

            cell140.Append(cellValue133);

            row5.Append(cell113);
            row5.Append(cell114);
            row5.Append(cell115);
            row5.Append(cell116);
            row5.Append(cell117);
            row5.Append(cell118);
            row5.Append(cell119);
            row5.Append(cell120);
            row5.Append(cell121);
            row5.Append(cell122);
            row5.Append(cell123);
            row5.Append(cell124);
            row5.Append(cell125);
            row5.Append(cell126);
            row5.Append(cell127);
            row5.Append(cell128);
            row5.Append(cell129);
            row5.Append(cell130);
            row5.Append(cell131);
            row5.Append(cell132);
            row5.Append(cell133);
            row5.Append(cell134);
            row5.Append(cell135);
            row5.Append(cell136);
            row5.Append(cell137);
            row5.Append(cell138);
            row5.Append(cell139);
            row5.Append(cell140);

            #endregion

            sheetData2.Append(row5);

            int i = 0;
            foreach (var mix in DayMixes)
            {
                AddNewPartRow(sheetData2, i, mix);
                i++;
            }

            AddNewResult(sheetData2, DayMixes.Count());
            
            PageMargins pageMargins2 = new PageMargins() { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };

            worksheet2.Append(sheetDimension2);
            worksheet2.Append(sheetViews2);
            worksheet2.Append(sheetFormatProperties2);
            worksheet2.Append(columns2);
            worksheet2.Append(sheetData2);
            worksheet2.Append(pageMargins2);

            worksheetPart2.Worksheet = worksheet2;
        }

        #region Вспомогательные

        private static void AddNewPartRow(SheetData sheetData2, int index, Mix mix)
        {
            #region Нужная строка
            
            uint styleNumber = 1U;
            uint styleData = 2U;
            uint styleString = 8U;
            if (!mix.Normal)
            {
                styleNumber = 6U;
                styleData = 7U;
                styleString = 9U;
            }

            Row row6 = new Row() 
                { RowIndex = (UInt32Value) Convert.ToUInt32(index + 2), Spans = new ListValue<StringValue>() { InnerText = "1:28" }, DyDescent = 0.25D };

            Cell cell141 = new Cell() { CellReference = $"A{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue134 = new CellValue();
            cellValue134.Text = mix.Number.ToString();
            cell141.Append(cellValue134);

            Cell cell142 = new Cell() { CellReference = $"B{index + 2}", StyleIndex = styleData };
            CellValue cellValue135 = new CellValue();
            cellValue135.Text = mix.DateTime.ToOADate().ToString(CultureInfo.InvariantCulture);
            cell142.Append(cellValue135);

            Cell cell143 = new Cell() { CellReference = $"C{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue136 = new CellValue();
            cellValue136.Text = mix.FormNumber.ToString();
            cell143.Append(cellValue136);

            Cell cell144 = new Cell() { CellReference = $"D{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue137 = new CellValue();
            cellValue137.Text = mix.RecipeNumber.ToString();
            cell144.Append(cellValue137);

            Cell cell145 = new Cell() { CellReference = $"E{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue138 = new CellValue();
            cellValue138.Text = mix.MixerTemperature.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell145.Append(cellValue138);

            Cell cell146 = new Cell() { CellReference = $"F{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue139 = new CellValue();
            cellValue139.Text = mix.SetRevertMud.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell146.Append(cellValue139);

            Cell cell147 = new Cell() { CellReference = $"G{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue140 = new CellValue();
            cellValue140.Text = mix.ActRevertMud.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell147.Append(cellValue140);

            Cell cell148 = new Cell() { CellReference = $"H{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue141 = new CellValue();
            cellValue141.Text = mix.SetSandMud.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell148.Append(cellValue141);

            Cell cell149 = new Cell() { CellReference = $"I{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue142 = new CellValue();
            cellValue142.Text = mix.ActSandMud.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell149.Append(cellValue142);

            Cell cell150 = new Cell() { CellReference = $"J{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue143 = new CellValue();
            cellValue143.Text = mix.SetColdWater.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell150.Append(cellValue143);

            Cell cell151 = new Cell() { CellReference = $"K{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue144 = new CellValue();
            cellValue144.Text = mix.ActColdWater.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell151.Append(cellValue144);

            Cell cell152 = new Cell() { CellReference = $"L{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue145 = new CellValue();
            cellValue145.Text = mix.SetHotWater.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell152.Append(cellValue145);

            Cell cell153 = new Cell() { CellReference = $"M{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue146 = new CellValue();
            cellValue146.Text = mix.ActHotWater.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell153.Append(cellValue146);

            Cell cell154 = new Cell() { CellReference = $"N{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue147 = new CellValue();
            cellValue147.Text = mix.SetMixture1.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell154.Append(cellValue147);

            Cell cell155 = new Cell() { CellReference = $"O{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue148 = new CellValue();
            cellValue148.Text = mix.ActMixture1.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell155.Append(cellValue148);

            Cell cell156 = new Cell() { CellReference = $"P{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue149 = new CellValue();
            cellValue149.Text = mix.SetMixture2.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell156.Append(cellValue149);

            Cell cell157 = new Cell() { CellReference = $"Q{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue150 = new CellValue();
            cellValue150.Text = mix.ActMixture2.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell157.Append(cellValue150);

            Cell cell158 = new Cell() { CellReference = $"R{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue151 = new CellValue();
            cellValue151.Text = mix.SetCement1.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell158.Append(cellValue151);

            Cell cell159 = new Cell() { CellReference = $"S{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue152 = new CellValue();
            cellValue152.Text = mix.ActCement1.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell159.Append(cellValue152);

            Cell cell160 = new Cell() { CellReference = $"T{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue153 = new CellValue();
            cellValue153.Text = mix.SetCement2.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell160.Append(cellValue153);

            Cell cell161 = new Cell() { CellReference = $"U{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue154 = new CellValue();
            cellValue154.Text = mix.ActCement2.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell161.Append(cellValue154);

            Cell cell162 = new Cell() { CellReference = $"V{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue155 = new CellValue();
            cellValue155.Text = mix.SetAluminium1.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell162.Append(cellValue155);

            Cell cell163 = new Cell() { CellReference = $"W{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue156 = new CellValue();
            cellValue156.Text = mix.ActAluminium1.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell163.Append(cellValue156);

            Cell cell164 = new Cell() { CellReference = $"X{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue157 = new CellValue();
            cellValue157.Text = mix.SetAluminium2.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell164.Append(cellValue157);

            Cell cell165 = new Cell() { CellReference = $"Y{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue158 = new CellValue();
            cellValue158.Text = mix.ActAluminium2.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell165.Append(cellValue158);

            Cell cell166 = new Cell() { CellReference = $"Z{index + 2}", StyleIndex = styleNumber };
            CellValue cellValue159 = new CellValue();
            cellValue159.Text = mix.SandInMud.ToString("F2", new NumberFormatInfo { CurrencyDecimalSeparator = "." });
            cell166.Append(cellValue159);

            Cell cell167 = new Cell() { CellReference = $"AA{index + 2}", StyleIndex = styleString, DataType = CellValues.String };
            CellValue cellValue160 = new CellValue();
            cellValue160.Text = mix.NormalStr;
            cell167.Append(cellValue160);

            Cell cell168 = new Cell() { CellReference = $"AB{index + 2}", StyleIndex = styleString, DataType = CellValues.String };
            CellValue cellValue161 = new CellValue();
            cellValue161.Text = mix.CharsStr;
            cell168.Append(cellValue161);

            row6.Append(cell141);
            row6.Append(cell142);
            row6.Append(cell143);
            row6.Append(cell144);
            row6.Append(cell145);
            row6.Append(cell146);
            row6.Append(cell147);
            row6.Append(cell148);
            row6.Append(cell149);
            row6.Append(cell150);
            row6.Append(cell151);
            row6.Append(cell152);
            row6.Append(cell153);
            row6.Append(cell154);
            row6.Append(cell155);
            row6.Append(cell156);
            row6.Append(cell157);
            row6.Append(cell158);
            row6.Append(cell159);
            row6.Append(cell160);
            row6.Append(cell161);
            row6.Append(cell162);
            row6.Append(cell163);
            row6.Append(cell164);
            row6.Append(cell165);
            row6.Append(cell166);
            row6.Append(cell167);
            row6.Append(cell168);

            #endregion

            sheetData2.Append(row6);
        }

        private static void AddNewResult(SheetData sheetData2, int count)
        {
            #region Итоги

            Row row8 = new Row() 
                { RowIndex = (UInt32Value) Convert.ToUInt32(count + 2), Spans = new ListValue<StringValue>() { InnerText = "1:28" }, DyDescent = 0.25D };

            Cell cell197 = new Cell() { CellReference = $"A{count + 2}", StyleIndex = (UInt32Value)3U, DataType = CellValues.SharedString };
            CellValue cellValue189 = new CellValue();
            cellValue189.Text = "26";

            cell197.Append(cellValue189);
            Cell cell198 = new Cell() { CellReference = $"B{count + 2}", StyleIndex = (UInt32Value)3U };
            Cell cell199 = new Cell() { CellReference = $"C{count + 2}", StyleIndex = (UInt32Value)3U };
            Cell cell200 = new Cell() { CellReference = $"D{count + 2}", StyleIndex = (UInt32Value)3U };
            Cell cell201 = new Cell() { CellReference = $"E{count + 2}", StyleIndex = (UInt32Value)3U };

            Cell cell202 = new Cell() { CellReference = $"F{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula22 = new CellFormula() { FormulaType = CellFormulaValues.Shared, Reference = $"F{count + 2}:Z{count + 2}", SharedIndex = (UInt32Value)0U };
            cellFormula22.Text = $"SUM(F1:F{count + 1})";
            cell202.Append(cellFormula22);
            
            Cell cell203 = new Cell() { CellReference = $"G{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula23 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula23.Text = "";
            cell203.Append(cellFormula23);

            Cell cell204 = new Cell() { CellReference = $"H{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula24 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula24.Text = "";
            cell204.Append(cellFormula24);

            Cell cell205 = new Cell() { CellReference = $"I{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula25 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula25.Text = "";
            cell205.Append(cellFormula25);

            Cell cell206 = new Cell() { CellReference = $"J{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula26 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula26.Text = "";
            cell206.Append(cellFormula26);

            Cell cell207 = new Cell() { CellReference = $"K{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula27 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula27.Text = "";
            cell207.Append(cellFormula27);

            Cell cell208 = new Cell() { CellReference = $"L{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula28 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula28.Text = "";
            cell208.Append(cellFormula28);

            Cell cell209 = new Cell() { CellReference = $"M{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula29 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula29.Text = "";
            cell209.Append(cellFormula29);

            Cell cell210 = new Cell() { CellReference = $"N{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula30 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula30.Text = "";
            cell210.Append(cellFormula30);

            Cell cell211 = new Cell() { CellReference = $"O{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula31 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula31.Text = "";
            cell211.Append(cellFormula31);

            Cell cell212 = new Cell() { CellReference = $"P{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula32 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula32.Text = "";
            cell212.Append(cellFormula32);

            Cell cell213 = new Cell() { CellReference = $"Q{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula33 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula33.Text = "";
            cell213.Append(cellFormula33);

            Cell cell214 = new Cell() { CellReference = $"R{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula34 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula34.Text = "";
            cell214.Append(cellFormula34);

            Cell cell215 = new Cell() { CellReference = $"S{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula35 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula35.Text = "";
            cell215.Append(cellFormula35);

            Cell cell216 = new Cell() { CellReference = $"T{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula36 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula36.Text = "";
            cell216.Append(cellFormula36);

            Cell cell217 = new Cell() { CellReference = $"U{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula37 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula37.Text = "";
            cell217.Append(cellFormula37);

            Cell cell218 = new Cell() { CellReference = $"V{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula38 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula38.Text = "";
            cell218.Append(cellFormula38);

            Cell cell219 = new Cell() { CellReference = $"W{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula39 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula39.Text = "";
            cell219.Append(cellFormula39);

            Cell cell220 = new Cell() { CellReference = $"X{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula40 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula40.Text = "";
            cell220.Append(cellFormula40);

            Cell cell221 = new Cell() { CellReference = $"Y{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula41 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula41.Text = "";
            cell221.Append(cellFormula41);

            Cell cell222 = new Cell() { CellReference = $"Z{count + 2}", StyleIndex = (UInt32Value)3U };
            CellFormula cellFormula42 = new CellFormula() { FormulaType = CellFormulaValues.Shared, SharedIndex = (UInt32Value)0U };
            cellFormula42.Text = "";
            cell222.Append(cellFormula42);

            Cell cell223 = new Cell() { CellReference = $"AA{count + 2}", StyleIndex = (UInt32Value)8U };
            Cell cell224 = new Cell() { CellReference = $"AB{count + 2}", StyleIndex = (UInt32Value)8U };

            row8.Append(cell197);
            row8.Append(cell198);
            row8.Append(cell199);
            row8.Append(cell200);
            row8.Append(cell201);
            row8.Append(cell202);
            row8.Append(cell203);
            row8.Append(cell204);
            row8.Append(cell205);
            row8.Append(cell206);
            row8.Append(cell207);
            row8.Append(cell208);
            row8.Append(cell209);
            row8.Append(cell210);
            row8.Append(cell211);
            row8.Append(cell212);
            row8.Append(cell213);
            row8.Append(cell214);
            row8.Append(cell215);
            row8.Append(cell216);
            row8.Append(cell217);
            row8.Append(cell218);
            row8.Append(cell219);
            row8.Append(cell220);
            row8.Append(cell221);
            row8.Append(cell222);
            row8.Append(cell223);
            row8.Append(cell224);

            #endregion

            sheetData2.Append(row8);
        }

        #endregion

        // Generates content of spreadsheetPrinterSettingsPart1.
        private void GenerateSpreadsheetPrinterSettingsPart1Content(SpreadsheetPrinterSettingsPart spreadsheetPrinterSettingsPart1)
        {
            System.IO.Stream data = GetBinaryDataStream(spreadsheetPrinterSettingsPart1Data);
            spreadsheetPrinterSettingsPart1.FeedData(data);
            data.Close();
        }

        #region Порядок вычисления формул

        // Generates content of calculationChainPart1.
        private void GenerateCalculationChainPart1Content(CalculationChainPart calculationChainPart1)
        {
            CalculationChain calculationChain1 = new CalculationChain();

            AddChainToPart(NightMixes.Count(), calculationChain1, 1);

            AddChainToPart(DayMixes.Count(), calculationChain1, 2);

            calculationChainPart1.CalculationChain = calculationChain1;
        }

        private static void AddChainToPart(int count, CalculationChain calculationChain1, int sheetId)
        {
            CalculationCell calculationCell1 = new CalculationCell() { CellReference = $"Z{count + 2}", SheetId = sheetId, NewLevel = true };
            CalculationCell calculationCell2 = new CalculationCell() { CellReference = $"Y{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell3 = new CalculationCell() { CellReference = $"X{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell4 = new CalculationCell() { CellReference = $"W{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell5 = new CalculationCell() { CellReference = $"V{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell6 = new CalculationCell() { CellReference = $"U{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell7 = new CalculationCell() { CellReference = $"T{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell8 = new CalculationCell() { CellReference = $"S{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell9 = new CalculationCell() { CellReference = $"R{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell10 = new CalculationCell() { CellReference = $"Q{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell11 = new CalculationCell() { CellReference = $"P{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell12 = new CalculationCell() { CellReference = $"O{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell13 = new CalculationCell() { CellReference = $"N{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell14 = new CalculationCell() { CellReference = $"M{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell15 = new CalculationCell() { CellReference = $"L{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell16 = new CalculationCell() { CellReference = $"K{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell17 = new CalculationCell() { CellReference = $"J{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell18 = new CalculationCell() { CellReference = $"I{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell19 = new CalculationCell() { CellReference = $"H{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell20 = new CalculationCell() { CellReference = $"G{count + 2}", SheetId = sheetId };
            CalculationCell calculationCell21 = new CalculationCell() { CellReference = $"F{count + 2}", SheetId = sheetId };
            calculationChain1.Append(calculationCell1);
            calculationChain1.Append(calculationCell2);
            calculationChain1.Append(calculationCell3);
            calculationChain1.Append(calculationCell4);
            calculationChain1.Append(calculationCell5);
            calculationChain1.Append(calculationCell6);
            calculationChain1.Append(calculationCell7);
            calculationChain1.Append(calculationCell8);
            calculationChain1.Append(calculationCell9);
            calculationChain1.Append(calculationCell10);
            calculationChain1.Append(calculationCell11);
            calculationChain1.Append(calculationCell12);
            calculationChain1.Append(calculationCell13);
            calculationChain1.Append(calculationCell14);
            calculationChain1.Append(calculationCell15);
            calculationChain1.Append(calculationCell16);
            calculationChain1.Append(calculationCell17);
            calculationChain1.Append(calculationCell18);
            calculationChain1.Append(calculationCell19);
            calculationChain1.Append(calculationCell20);
            calculationChain1.Append(calculationCell21);
        }

        #endregion

        #region Остальное

        // Generates content of sharedStringTablePart1.
        private void GenerateSharedStringTablePart1Content(SharedStringTablePart sharedStringTablePart1)
        {
            SharedStringTable sharedStringTable1 = new SharedStringTable() { Count = (UInt32Value)64U, UniqueCount = (UInt32Value)31U };

            SharedStringItem sharedStringItem1 = new SharedStringItem();
            Text text1 = new Text();
            text1.Text = "№";

            sharedStringItem1.Append(text1);

            SharedStringItem sharedStringItem2 = new SharedStringItem();
            Text text2 = new Text();
            text2.Text = "Дата и время";

            sharedStringItem2.Append(text2);

            SharedStringItem sharedStringItem3 = new SharedStringItem();
            Text text3 = new Text();
            text3.Text = "Номер\nформы";

            sharedStringItem3.Append(text3);

            SharedStringItem sharedStringItem4 = new SharedStringItem();
            Text text4 = new Text();
            text4.Text = "Номер\nрецепта";

            sharedStringItem4.Append(text4);

            SharedStringItem sharedStringItem5 = new SharedStringItem();
            Text text5 = new Text();
            text5.Text = "Температура\nсмесителя";

            sharedStringItem5.Append(text5);

            SharedStringItem sharedStringItem6 = new SharedStringItem();
            Text text6 = new Text();
            text6.Text = "Заданный вес\nобратный шлам";

            sharedStringItem6.Append(text6);

            SharedStringItem sharedStringItem7 = new SharedStringItem();
            Text text7 = new Text();
            text7.Text = "Факт вес\nобратный шлам";

            sharedStringItem7.Append(text7);

            SharedStringItem sharedStringItem8 = new SharedStringItem();
            Text text8 = new Text();
            text8.Text = "Заданный вес\nпесчанный шлам";

            sharedStringItem8.Append(text8);

            SharedStringItem sharedStringItem9 = new SharedStringItem();
            Text text9 = new Text();
            text9.Text = "Факт вес\nпесчанный шлам";

            sharedStringItem9.Append(text9);

            SharedStringItem sharedStringItem10 = new SharedStringItem();
            Text text10 = new Text();
            text10.Text = "Заданный вес\nхолодная вода";

            sharedStringItem10.Append(text10);

            SharedStringItem sharedStringItem11 = new SharedStringItem();
            Text text11 = new Text();
            text11.Text = "Факт вес\nхолодная вода";

            sharedStringItem11.Append(text11);

            SharedStringItem sharedStringItem12 = new SharedStringItem();
            Text text12 = new Text();
            text12.Text = "Заданный вес\nгорячая вода";

            sharedStringItem12.Append(text12);

            SharedStringItem sharedStringItem13 = new SharedStringItem();
            Text text13 = new Text();
            text13.Text = "Факт вес\nгорячая вода";

            sharedStringItem13.Append(text13);

            SharedStringItem sharedStringItem14 = new SharedStringItem();
            Text text14 = new Text();
            text14.Text = "Заданный вес\nИПВ №1";

            sharedStringItem14.Append(text14);

            SharedStringItem sharedStringItem15 = new SharedStringItem();
            Text text15 = new Text();
            text15.Text = "Факт вес\nИПВ №1";

            sharedStringItem15.Append(text15);

            SharedStringItem sharedStringItem16 = new SharedStringItem();
            Text text16 = new Text();
            text16.Text = "Заданный вес\nИПВ №2";

            sharedStringItem16.Append(text16);

            SharedStringItem sharedStringItem17 = new SharedStringItem();
            Text text17 = new Text();
            text17.Text = "Факт вес\nИПВ №2";

            sharedStringItem17.Append(text17);

            SharedStringItem sharedStringItem18 = new SharedStringItem();
            Text text18 = new Text();
            text18.Text = "Заданный вес\nцемент №1";

            sharedStringItem18.Append(text18);

            SharedStringItem sharedStringItem19 = new SharedStringItem();
            Text text19 = new Text();
            text19.Text = "Факт вес\nцемент №1";

            sharedStringItem19.Append(text19);

            SharedStringItem sharedStringItem20 = new SharedStringItem();
            Text text20 = new Text();
            text20.Text = "Заданный вес\nцемент №2";

            sharedStringItem20.Append(text20);

            SharedStringItem sharedStringItem21 = new SharedStringItem();
            Text text21 = new Text();
            text21.Text = "Факт вес\nцемент №2";

            sharedStringItem21.Append(text21);

            SharedStringItem sharedStringItem22 = new SharedStringItem();
            Text text22 = new Text();
            text22.Text = "Заданный вес\nалюминий №1";

            sharedStringItem22.Append(text22);

            SharedStringItem sharedStringItem23 = new SharedStringItem();
            Text text23 = new Text();
            text23.Text = "Факт вес\nалюминий №1";

            sharedStringItem23.Append(text23);

            SharedStringItem sharedStringItem24 = new SharedStringItem();
            Text text24 = new Text();
            text24.Text = "Заданный вес\nалюминий №2";

            sharedStringItem24.Append(text24);

            SharedStringItem sharedStringItem25 = new SharedStringItem();
            Text text25 = new Text();
            text25.Text = "Факт вес\nалюминий №2";

            sharedStringItem25.Append(text25);

            SharedStringItem sharedStringItem26 = new SharedStringItem();
            Text text26 = new Text();
            text26.Text = "Песок \nв шламе";

            sharedStringItem26.Append(text26);

            SharedStringItem sharedStringItem27 = new SharedStringItem();
            Text text27 = new Text();
            text27.Text = "Сумма:";

            sharedStringItem27.Append(text27);

            SharedStringItem sharedStringItem28 = new SharedStringItem();
            Text text28 = new Text();
            text28.Text = "Норма";

            sharedStringItem28.Append(text28);

            SharedStringItem sharedStringItem29 = new SharedStringItem();
            Text text29 = new Text();
            text29.Text = "Характе-\nристики";

            sharedStringItem29.Append(text29);

            SharedStringItem sharedStringItem30 = new SharedStringItem();
            Text text30 = new Text();
            text30.Text = "Плохо";

            sharedStringItem30.Append(text30);

            SharedStringItem sharedStringItem31 = new SharedStringItem();
            Text text31 = new Text();
            text31.Text = "Недоросток";

            sharedStringItem31.Append(text31);

            sharedStringTable1.Append(sharedStringItem1);
            sharedStringTable1.Append(sharedStringItem2);
            sharedStringTable1.Append(sharedStringItem3);
            sharedStringTable1.Append(sharedStringItem4);
            sharedStringTable1.Append(sharedStringItem5);
            sharedStringTable1.Append(sharedStringItem6);
            sharedStringTable1.Append(sharedStringItem7);
            sharedStringTable1.Append(sharedStringItem8);
            sharedStringTable1.Append(sharedStringItem9);
            sharedStringTable1.Append(sharedStringItem10);
            sharedStringTable1.Append(sharedStringItem11);
            sharedStringTable1.Append(sharedStringItem12);
            sharedStringTable1.Append(sharedStringItem13);
            sharedStringTable1.Append(sharedStringItem14);
            sharedStringTable1.Append(sharedStringItem15);
            sharedStringTable1.Append(sharedStringItem16);
            sharedStringTable1.Append(sharedStringItem17);
            sharedStringTable1.Append(sharedStringItem18);
            sharedStringTable1.Append(sharedStringItem19);
            sharedStringTable1.Append(sharedStringItem20);
            sharedStringTable1.Append(sharedStringItem21);
            sharedStringTable1.Append(sharedStringItem22);
            sharedStringTable1.Append(sharedStringItem23);
            sharedStringTable1.Append(sharedStringItem24);
            sharedStringTable1.Append(sharedStringItem25);
            sharedStringTable1.Append(sharedStringItem26);
            sharedStringTable1.Append(sharedStringItem27);
            sharedStringTable1.Append(sharedStringItem28);
            sharedStringTable1.Append(sharedStringItem29);
            sharedStringTable1.Append(sharedStringItem30);
            sharedStringTable1.Append(sharedStringItem31);

            sharedStringTablePart1.SharedStringTable = sharedStringTable1;
        }

        // Generates content of workbookStylesPart1.
        private void GenerateWorkbookStylesPart1Content(WorkbookStylesPart workbookStylesPart1)
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            NumberingFormats numberingFormats1 = new NumberingFormats() { Count = (UInt32Value)1U };
            NumberingFormat numberingFormat1 = new NumberingFormat() { NumberFormatId = (UInt32Value)164U, FormatCode = "dd/mm/yy\\ h:mm;@" };

            numberingFormats1.Append(numberingFormat1);

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)3U, KnownFonts = true };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            Color color1 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontCharSet fontCharSet1 = new FontCharSet() { Val = 204 };
            FontScheme fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontCharSet1);
            font1.Append(fontScheme2);

            Font font2 = new Font();
            FontSize fontSize2 = new FontSize() { Val = 11D };
            Color color2 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName2 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme3 = new FontScheme() { Val = FontSchemeValues.Minor };

            font2.Append(fontSize2);
            font2.Append(color2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);
            font2.Append(fontScheme3);

            Font font3 = new Font();
            Bold bold1 = new Bold();
            FontSize fontSize3 = new FontSize() { Val = 11D };
            Color color3 = new Color() { Rgb = "FFFF0000" };
            FontName fontName3 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering3 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme4 = new FontScheme() { Val = FontSchemeValues.Minor };

            font3.Append(bold1);
            font3.Append(fontSize3);
            font3.Append(color3);
            font3.Append(fontName3);
            font3.Append(fontFamilyNumbering3);
            font3.Append(fontScheme4);

            fonts1.Append(font1);
            fonts1.Append(font2);
            fonts1.Append(font3);

            Fills fills1 = new Fills() { Count = (UInt32Value)2U };

            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };

            fill1.Append(patternFill1);

            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

            fill2.Append(patternFill2);

            fills1.Append(fill1);
            fills1.Append(fill2);

            Borders borders1 = new Borders() { Count = (UInt32Value)2U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            Border border2 = new Border();

            LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color color4 = new Color() { Indexed = (UInt32Value)64U };

            leftBorder2.Append(color4);

            RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color color5 = new Color() { Indexed = (UInt32Value)64U };

            rightBorder2.Append(color5);

            TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color color6 = new Color() { Indexed = (UInt32Value)64U };

            topBorder2.Append(color6);

            BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color color7 = new Color() { Indexed = (UInt32Value)64U };

            bottomBorder2.Append(color7);
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            borders1.Append(border1);
            borders1.Append(border2);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)10U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)164U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyNumberFormat = true, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true };
            CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = true };

            CellFormat cellFormat6 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true };
            Alignment alignment1 = new Alignment() { Horizontal = HorizontalAlignmentValues.Center };

            cellFormat6.Append(alignment1);

            CellFormat cellFormat7 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true };
            Alignment alignment2 = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, WrapText = true };

            cellFormat7.Append(alignment2);
            CellFormat cellFormat8 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true };
            CellFormat cellFormat9 = new CellFormat() { NumberFormatId = (UInt32Value)164U, FontId = (UInt32Value)2U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyNumberFormat = true, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true };
            CellFormat cellFormat10 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyBorder = true };
            CellFormat cellFormat11 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);
            cellFormats1.Append(cellFormat5);
            cellFormats1.Append(cellFormat6);
            cellFormats1.Append(cellFormat7);
            cellFormats1.Append(cellFormat8);
            cellFormats1.Append(cellFormat9);
            cellFormats1.Append(cellFormat10);
            cellFormats1.Append(cellFormat11);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Обычный", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append(slicerStyles1);

            StylesheetExtension stylesheetExtension2 = new StylesheetExtension() { Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
            stylesheetExtension2.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
            X15.TimelineStyles timelineStyles1 = new X15.TimelineStyles() { DefaultTimelineStyle = "TimeSlicerStyleLight1" };

            stylesheetExtension2.Append(timelineStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);
            stylesheetExtensionList1.Append(stylesheetExtension2);

            stylesheet1.Append(numberingFormats1);
            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);

            workbookStylesPart1.Stylesheet = stylesheet1;
        }

        private void SetPackageProperties(OpenXmlPackage document)
        {
            document.PackageProperties.Creator = "HSE";
            document.PackageProperties.Created = XmlConvert.ToDateTime("2021-05-11T04:12:15Z", XmlDateTimeSerializationMode.RoundtripKind);
            document.PackageProperties.Modified = XmlConvert.ToDateTime("2021-05-18T04:48:00Z", XmlDateTimeSerializationMode.RoundtripKind);
            document.PackageProperties.LastModifiedBy = "ИнженерКИПиА";
        }

        #region Binary Data
        private string spreadsheetPrinterSettingsPart1Data = "UwBhAG0AcwB1AG4AZwAgAE0ATAAtADEAOAA2ADAAIABTAGUAcgBpAGUAcwAgACgAVQBTAEIAMAAwADEAAAAAAAEEAATcACQnD9eBAwEACQCaCzQIZAABAAcAWAIAAAEAAAADAAEAQQA0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFNFQ0QAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHgALQAAAEEAcgBpAGEAbAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAAAICAgAAAAJABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABVAG4AdABpAHQAbABlAGQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwADoAMAA6ADAAOgAwADoAMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAABkAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUAAABYAgAAMgAyADIAMgAyADIAMgAyADIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABBADQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPAAAAAAAAAAABZABkAAEAZAAAZAAAZAAAZAAAAAACAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAABAAAAAAAAAAABAQABAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQEBAQEAAAAAAAAAAQEBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAAAAkAAAcAAAAJAAAHAAAACQAAAAAHADQImgs0CJoLAAAAAABAAEAAAAAFBgQANAiaCwEAAgAAADQImgv8J3FQAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGAgIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQQByAGkAYQBsAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQgAjIyMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAABwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";

        private Stream GetBinaryDataStream(string base64String)
        {
            return new MemoryStream(Convert.FromBase64String(base64String));
        }

#endregion

        #endregion

    }
}
