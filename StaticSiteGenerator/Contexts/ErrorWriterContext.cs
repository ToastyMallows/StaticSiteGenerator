using System;

namespace StaticSiteGenerator.Contexts
{
    public abstract class ErrorWriterContext
    {
        private static ErrorWriterContext _current;

        public static ErrorWriterContext Current
        {
            get
            {
                if ( null == _current )
                {
                    _current = DefaultErrorWriterContext;
                }
                return _current;
            }
            set
            {
                Guard.VerifyArgumentNotNull( value, "ErrorWriterContextSetValue" );
                _current = value;
            }
        }

        public static ErrorWriterContext DefaultErrorWriterContext = new ConsoleErrorWriter();

        public abstract void WriteLine( string error );
    }

    internal sealed class ConsoleErrorWriter : ErrorWriterContext
    {
        public override void WriteLine( string error )
        {
            Console.Error.WriteLine( error );
        }
    }
}
