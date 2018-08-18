using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RWT.ArgParse.Test
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void BasicIntArg()
        {
            int i1 = 0, i2 = 0;
            var ap = new ArgParser(
                new Int32Arg("-n", "<int> sets the integer")
                {
                    Command = (n) => { i1 = n; i2 = n * n; }
                });
            var extras = ap.Parse(new string[] { "-n", "5" });
            Assert.IsTrue(extras.Count == 0);
            Assert.AreEqual(i1, 5);
            Assert.AreEqual(i2, 25);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgParseException))]
        public void MustHaveRequired()
        {
            int i1 = 0, i2 = 0;
            var ap = new ArgParser(
                new Int32Arg("-n", "<int> sets the integer")
                {
                    Command = (n) => { i1 = n; i2 = n * n; }
                },
                new StrArg("-f", "a required flag")
                {
                    Required = true
                });
            var extras = ap.Parse(new string[] { "-n", "5" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgParseException))]
        public void MultiplesNotAllowed()
        {
            int i1 = 0, i2 = 0;
            var ap = new ArgParser(
                new Int32Arg("-n", "<int> sets the integer")
                {
                    Command = (n) => { i1 = n; i2 = n * n; }
                });
            var extras = ap.Parse(new string[] { "-n", "5", "-n", "6" });
        }

        [TestMethod]
        public void MultiplesAllowed()
        {
            int sum = 0;
            var ap = new ArgParser(
                new Int32Arg("-n", "<int> accumulates given numbers")
                {
                    Command = (n) => { sum += n; },
                    Default = 100, // make sure it's not applied
                    MultiplesAllowed = true
                });
            var extras = ap.Parse(new string[] { "-n", "5", "-n", "6" });
            Assert.IsTrue(extras.Count == 0);
            Assert.AreEqual(sum, 11);
        }

        [TestMethod]
        public void CatchExtras()
        {
            int sum = 0;
            var ap = new ArgParser(
                new Int32Arg("-n", "<int> accumulates given numbers")
                {
                    Command = (n) => { sum += n; },
                    Default = 100,
                    MultiplesAllowed = true
                });
            var extras = ap.Parse(new string[] { "-n", "5", "what", "-n", "6", "else" });
            Assert.IsTrue(extras.Count == 2);
            Assert.AreEqual(extras[0], "what");
            Assert.AreEqual(extras[1], "else");
            Assert.AreEqual(sum, 11);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgParseException))]
        public void ExtraRangeChecks1()
        {
            int sum = 0;
            var ap = new ArgParser(
                new Int32Arg("-n", "<int> accumulates given numbers")
                {
                    Command = (n) => { sum += n; },
                    Default = 100,
                    MultiplesAllowed = true
                })
            {
                ExtrasRange = (0, 1)
            };
            var extras = ap.Parse(new string[] { "-n", "5", "what", "-n", "6", "else" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgParseException))]
        public void ExtraRangeChecks2()
        {
            int sum = 0;
            var ap = new ArgParser(
                new Int32Arg("-n", "<int> accumulates given numbers")
                {
                    Command = (n) => { sum += n; },
                    Default = 100,
                    MultiplesAllowed = true
                })
            {
                ExtrasRange = (3, 3)
            };
            var extras = ap.Parse(new string[] { "-n", "5", "what", "-n", "6", "else" });
        }

    }
}
