using StaticSiteGenerator.Contexts;

namespace StaticSiteGeneratorTest
{
    public sealed class TestErrorWriter : ErrorWriterContext
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

        public static void SetCurrentErrorWriterContext( out TestErrorWriter testErrorWriter )
        {
            testErrorWriter = new TestErrorWriter();
            ErrorWriterContext.Current = testErrorWriter;
        }
    }
}
