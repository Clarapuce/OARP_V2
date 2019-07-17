using Microsoft.VisualStudio.TestTools.UnitTesting;
using OARP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using Accord.Math.Optimization;
using System.IO;
namespace OARP.Tests
{
    [TestClass()]
    public class OARPTests
    {

        [TestMethod()]
        public void ExtraireCsvTest()
        {
            string lien = "C:/Users/Clarapuce/Desktop/ENSC/2A/OARP_V2/Test expe goûté - Feuille 1.csv";
            int[,] matrice;
            matrice = System.Windows.Forms.OARP.ExtraireCsv(lien);
            Assert.Fail();
        }
        
    }
}