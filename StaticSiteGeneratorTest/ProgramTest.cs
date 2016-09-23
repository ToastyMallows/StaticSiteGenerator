using NUnit.Framework;

namespace StaticSiteGenerator.Test
{
    [TestFixture]
    public sealed class ProgramTest
    {
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
        [Description( "Tests that the Main function returns error status if args does not contain the generate switch" )]
        public void Main_ReturnsError_ForNoGenerateSwitch()
        {
            string[] args = new string[] { "-test", "-switch" };
            int expectedStatus = Constants.ERROR_STATUS;

            Assert.AreEqual( expectedStatus, Program.Main( args ) );
        }

        // More tests

        #endregion

        private static class Constants
        {
            public const int ERROR_STATUS = 1;
            public const int SUCCESS_STATUS = 0;
        }
    }
}
