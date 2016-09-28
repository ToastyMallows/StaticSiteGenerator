using NUnit.Framework;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;

namespace StaticSiteGenerator.Test
{
    [TestFixture]
    public sealed class ProgramTest
    {
        private static string expectedUsageMessage;

        [OneTimeSetUp]
        public void ProgramTest_Setup()
        {
            expectedUsageMessage = ( new Options() ).GetUsage();
        }

        #region Program Tests

        //[Test]
        //[Description( "Tests that the Main function returns error status if args is null" )]
        //public void Main_ReturnsError_ForNullArgs()
        //{
        ///
        /// Main cannot be called with null
        /// 
        //}

        [Test]
        [Description( "Tests that the Main function returns error status if args is empty" )]
        public void Main_ReturnsError_ForEmptyArgs()
        {
            string[] args = new string[] { };
            int expectedStatus = Constants.ERROR_STATUS;

            Assert.AreEqual( expectedStatus, Program.Main( args ) );
        }

        [Test]
        [Description( "Tests that the Main function returns the usage message if the switches don't include known switches" )]
        public void Main_ReturnsError_ForNoKnownSwitches()
        {
            string[] args = new string[] { "-test", "-switches" };
            int expectedStatus = Constants.ERROR_STATUS;

            TestErrorWriter testErrorWriter = new TestErrorWriter();
            setErrorWriterContext( testErrorWriter );

            int actualStatus = Program.Main( args );
            Assert.AreEqual( expectedStatus, actualStatus );
            Assert.AreEqual( expectedUsageMessage, testErrorWriter.LastErrorString );
        }

        [Test]
        [Description( "Tests that the Main function returns the usage message if the generate switch is not included" )]
        public void Main_ReturnsError_ForNoGenerateSwitch()
        {
            string[] args = new string[] { "-t" };
            int expectedStatus = Constants.ERROR_STATUS;

            TestErrorWriter testErrorWriter = new TestErrorWriter();
            setErrorWriterContext( testErrorWriter );

            int actualStatus = Program.Main( args );
            Assert.AreEqual( expectedStatus, actualStatus );
            Assert.AreEqual( expectedUsageMessage, testErrorWriter.LastErrorString );
        }

        // More tests

        #endregion

        private void setErrorWriterContext( TestErrorWriter testErrorWriter )
        {
            ErrorWriterContext.Current = testErrorWriter;
        }

        private static class Constants
        {
            public const int ERROR_STATUS = 1;
            public const int SUCCESS_STATUS = 0;
        }

        private class TestErrorWriter : ErrorWriterContext
        {
            private string _lastErrorString = null;

            public string LastErrorString
            {
                get
                {
                    return _lastErrorString;
                }
            }

            public override void WriteLine( string error )
            {
                _lastErrorString = error;
            }
        }
    }
}
