namespace SpreadSheetTests
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
    using NUnit.Framework.Internal.Commands;
    using SpreadsheetEngine;
    using System.ComponentModel;
    using System.Xml.Linq;

    /// <summary>
    /// Tests for our Spreadsheet Application.
    /// </summary>
    public class Tests
    {
        /// <summary>
        /// Setup for our Tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SelfReferenceTestNormal()
        {
            Spreadsheet TestSheet = new Spreadsheet(5, 5);
            Cell Testcell = TestSheet.GetCell(0, 0);
            Testcell._text = "=A1";
            Assert.AreEqual("!self reference", Testcell._value);
        }

        [Test]
        public void BadReferenceBoundryTest()
        {
            Spreadsheet TestSheet = new Spreadsheet(5, 5);
            Cell Testcell = TestSheet.GetCell(4, 4);
            Testcell._text = "=Ba";
            Assert.AreEqual("!(bad reference)", Testcell._value);
        }

        [Test]
        public void CircularExceptionalTest()
        {
            Spreadsheet TestSheet = new Spreadsheet(5, 5);
            Cell cell1 = TestSheet.GetCell(0, 0);
            Cell cell2 = TestSheet.GetCell(0, 1);
            Cell cell3 = TestSheet.GetCell(0, 2);
            Cell cell4 = TestSheet.GetCell(0, 3);

            cell1._text = "=B1";
            cell2._text = "=C1";
            cell3._text = "=D1";
            cell4._text = "=A1";

            Assert.AreEqual("!(circular reference)", cell1._value);
        }
    }
}