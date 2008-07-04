using Lephone.Util.Text;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [TestFixture]
    public class InflectorTest
    {
        [Test]
        public void TestPluralize()
        {
            Assert.AreEqual("posts", Inflector.Pluralize("post"));
            Assert.AreEqual("octopi", Inflector.Pluralize("octopus"));
            Assert.AreEqual("sheep", Inflector.Pluralize("sheep"));
            Assert.AreEqual("words", Inflector.Pluralize("words"));
            Assert.AreEqual("the blue mailmen", Inflector.Pluralize("the blue mailman"));
            Assert.AreEqual("CamelOctopi", Inflector.Pluralize("CamelOctopus"));
        }

        [Test]
        public void TestSingularize()
        {
            Assert.AreEqual("post", Inflector.Singularize("posts"));
            Assert.AreEqual("octopus", Inflector.Singularize("octopi"));
            Assert.AreEqual("sheep", Inflector.Singularize("sheep"));
            Assert.AreEqual("word", Inflector.Singularize("word"));
            Assert.AreEqual("the blue mailman", Inflector.Singularize("the blue mailmen"));
            Assert.AreEqual("CamelOctopus", Inflector.Singularize("CamelOctopi"));
        }

        [Test]
        public void TestCamelize()
        {
            Assert.AreEqual("ActiveRecord", Inflector.Camelize("active_record"));
            Assert.AreEqual("activeRecord", Inflector.Camelize("active_record", false));
            Assert.AreEqual("ActiveRecord::Errors", Inflector.Camelize("active_record/errors"));
            Assert.AreEqual("activeRecord::Errors", Inflector.Camelize("active_record/errors", false));
        }

        [Test]
        public void TestTitleize()
        {
            Assert.AreEqual("Man From The Boondocks", Inflector.Titleize("man from the boondocks"));
            Assert.AreEqual("X Men: The Last Stand", Inflector.Titleize("x-men: the last stand"));
        }

        [Test]
        public void TestUnderscore()
        {
            Assert.AreEqual("active_record", Inflector.Underscore("ActiveRecord"));
            Assert.AreEqual("active_record/errors", Inflector.Underscore("ActiveRecord::Errors"));
        }

        [Test]
        public void TestDasherize()
        {
            Assert.AreEqual("puni-puni", Inflector.Dasherize("puni_puni"));
        }

        [Test]
        public void TestHumanize()
        {
            Assert.AreEqual("Employee salary", Inflector.Humanize("employee_salary"));
        }

        [Test]
        public void TestDemodulize()
        {
            Assert.AreEqual("Inflections", Inflector.Demodulize("ActiveRecord::CoreExtensions::String::Inflections"));
            Assert.AreEqual("Inflections", Inflector.Demodulize("Inflections"));
        }

        [Test]
        public void TestTableize()
        {
            Assert.AreEqual("raw_scaled_scorers", Inflector.Tableize("RawScaledScorer"));
            Assert.AreEqual("egg_and_hams", Inflector.Tableize("egg_and_ham"));
            Assert.AreEqual("fancy_categories", Inflector.Tableize("fancyCategory"));
        }

        [Test]
        public void TestClassify()
        {
            Assert.AreEqual("EggAndHam", Inflector.Classify("egg_and_hams"));
            Assert.AreEqual("Post", Inflector.Classify("post"));
        }

        [Test]
        public void TestForeignKey()
        {
            Assert.AreEqual("message_id", Inflector.ForeignKey("Message"));
            Assert.AreEqual("messageid", Inflector.ForeignKey("Message", false));
            Assert.AreEqual("post_id", Inflector.ForeignKey("Admin::Post"));
        }

        [Test]
        public void TestOrdinalize()
        {
            Assert.AreEqual("1st", Inflector.Ordinalize(1));
            Assert.AreEqual("2nd", Inflector.Ordinalize(2));
            Assert.AreEqual("1002nd", Inflector.Ordinalize(1002));
            Assert.AreEqual("1003rd", Inflector.Ordinalize(1003));
        }
    }
}
