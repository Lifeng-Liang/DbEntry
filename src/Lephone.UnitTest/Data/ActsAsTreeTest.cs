using System.Collections.Generic;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    public class File : DbObjectModel<File>
    {
        public string Name { get; set; }

        [HasMany(OrderBy = "Id")]
        public IList<File> Children { get; private set; }

        [BelongsTo, DbColumn("BelongsTo_Id")]
        public File Parent { get; set; }
    }

    [DbTable("File")]
    public class TheFile : DbObjectModelAsTree<TheFile>
    {
        public string Name { get; set; }
    }

    [TestFixture]
    public class ActsAsTreeTest : DataTestBase
    {
        [Test]
        public void TestRead()
        {
            File f = File.FindById(1);
            Assert.AreEqual("Root", f.Name);

            Assert.IsNull(f.Parent);
            Assert.AreEqual(5, f.Children.Count);
            Assert.AreEqual("Windows", f.Children[0].Name);
            Assert.AreEqual("Program Files", f.Children[1].Name);
            Assert.AreEqual("Tools", f.Children[2].Name);
            Assert.AreEqual("Config.sys", f.Children[3].Name);
            Assert.AreEqual("Command.com", f.Children[4].Name);

            File fw = f.Children[0];
            Assert.AreEqual("Root", fw.Parent.Name);
            Assert.AreEqual(3, fw.Children.Count);
            Assert.AreEqual("regedit.exe", fw.Children[0].Name);
            Assert.AreEqual("notepad.exe", fw.Children[1].Name);
            Assert.AreEqual("System32", fw.Children[2].Name);

            File fs = fw.Children[2];
            Assert.AreEqual(1, fs.Children.Count);
            Assert.AreEqual("regsvr32.exe", fs.Children[0].Name);

            File fp = f.Children[1];
            Assert.AreEqual(1, fp.Children.Count);
            Assert.AreEqual("Office", fp.Children[0].Name);

            File fo = fp.Children[0];
            Assert.AreEqual(3, fo.Children.Count);
            Assert.AreEqual("Word.exe", fo.Children[0].Name);
            Assert.AreEqual("Outlook.exe", fo.Children[1].Name);
            Assert.AreEqual("Excel.exe", fo.Children[2].Name);

            File ft = f.Children[2];
            Assert.AreEqual(2, ft.Children.Count);
            Assert.AreEqual("LocPlus", ft.Children[0].Name);
            Assert.AreEqual("cConv", ft.Children[1].Name);

            File fl = ft.Children[0];
            Assert.AreEqual(2, fl.Children.Count);
            Assert.AreEqual("LocPlus.exe", fl.Children[0].Name);
            Assert.AreEqual("LocPlus.ini", fl.Children[1].Name);

            File fc = ft.Children[1];
            Assert.AreEqual(2, fc.Children.Count);
            Assert.AreEqual("cConv.exe", fc.Children[0].Name);
            Assert.AreEqual("cConv.ini", fc.Children[1].Name);

            Assert.AreEqual(0, fc.Children[0].Children.Count);
        }

        [Test]
        public void TestWrite()
        {
            File f = File.FindById(16);
            Assert.AreEqual(2, f.Children.Count);
            var nf = new File {Name = "gbk.tbl"};
            f.Children.Add(nf);
            f.Save();

            File f1 = File.FindById(16);
            Assert.AreEqual(3, f1.Children.Count);
            Assert.AreEqual("cConv.exe", f1.Children[0].Name);
            Assert.AreEqual("cConv.ini", f1.Children[1].Name);
            Assert.AreEqual("gbk.tbl", f1.Children[2].Name);
        }

        [Test]
        public void TestTheFileRead()
        {
            TheFile f = TheFile.FindById(1);
            Assert.AreEqual("Root", f.Name);

            Assert.IsNull(f.Parent);
            Assert.AreEqual(5, f.Children.Count);
            Assert.AreEqual("Windows", f.Children[0].Name);
            Assert.AreEqual("Program Files", f.Children[1].Name);
            Assert.AreEqual("Tools", f.Children[2].Name);
            Assert.AreEqual("Config.sys", f.Children[3].Name);
            Assert.AreEqual("Command.com", f.Children[4].Name);

            TheFile fw = f.Children[0];
            Assert.AreEqual("Root", fw.Parent.Name);
            Assert.AreEqual(3, fw.Children.Count);
            Assert.AreEqual("regedit.exe", fw.Children[0].Name);
            Assert.AreEqual("notepad.exe", fw.Children[1].Name);
            Assert.AreEqual("System32", fw.Children[2].Name);

            TheFile fs = fw.Children[2];
            Assert.AreEqual(1, fs.Children.Count);
            Assert.AreEqual("regsvr32.exe", fs.Children[0].Name);

            TheFile fp = f.Children[1];
            Assert.AreEqual(1, fp.Children.Count);
            Assert.AreEqual("Office", fp.Children[0].Name);

            TheFile fo = fp.Children[0];
            Assert.AreEqual(3, fo.Children.Count);
            Assert.AreEqual("Word.exe", fo.Children[0].Name);
            Assert.AreEqual("Outlook.exe", fo.Children[1].Name);
            Assert.AreEqual("Excel.exe", fo.Children[2].Name);

            TheFile ft = f.Children[2];
            Assert.AreEqual(2, ft.Children.Count);
            Assert.AreEqual("LocPlus", ft.Children[0].Name);
            Assert.AreEqual("cConv", ft.Children[1].Name);

            TheFile fl = ft.Children[0];
            Assert.AreEqual(2, fl.Children.Count);
            Assert.AreEqual("LocPlus.exe", fl.Children[0].Name);
            Assert.AreEqual("LocPlus.ini", fl.Children[1].Name);

            TheFile fc = ft.Children[1];
            Assert.AreEqual(2, fc.Children.Count);
            Assert.AreEqual("cConv.exe", fc.Children[0].Name);
            Assert.AreEqual("cConv.ini", fc.Children[1].Name);

            Assert.AreEqual(0, fc.Children[0].Children.Count);
        }

        [Test]
        public void TestTheFileWrite()
        {
            TheFile f = TheFile.FindById(16);
            Assert.AreEqual(2, f.Children.Count);
            var nf = new TheFile {Name = "gbk.tbl"};
            f.Children.Add(nf);
            f.Save();

            TheFile f1 = TheFile.FindById(16);
            Assert.AreEqual(3, f1.Children.Count);
            Assert.AreEqual("cConv.exe", f1.Children[0].Name);
            Assert.AreEqual("cConv.ini", f1.Children[1].Name);
            Assert.AreEqual("gbk.tbl", f1.Children[2].Name);
        }
    }
}
