using System;
using System.IO;

namespace StaticSiteGenerator.Contexts
{
    public abstract class IOContext : IIOContext
    {
        private static IIOContext _current;

        public static IIOContext Current
        {
            get
            {
                if ( null == _current )
                {
                    _current = DefaultIOContext;
                }
                return _current;
            }
            set
            {
                IIOContext ioContext = value as IIOContext;
                Guard.VerifyArgumentNotNull( value, "IOContextSetValue" );
                _current = ioContext;
            }
        }

        public static IIOContext DefaultIOContext = new DotNetIOContext();

        public abstract bool DirectoryExists( string path );

        public abstract string GetCurrentDirectory();

        public abstract string[] GetFilesFromDirectory( string path, string searchPattern, SearchOption searchOption );

        public abstract string ReadAllTextFromFile( string path );

        private class DotNetIOContext : IIOContext
        {
            public bool DirectoryExists( string path )
            {
                return Directory.Exists( path );
            }

            public string GetCurrentDirectory()
            {
                return Directory.GetCurrentDirectory();
            }

            public string[] GetFilesFromDirectory( string path, string searchPattern, SearchOption searchOption )
            {
                return Directory.GetFiles( path, searchPattern, searchOption );
            }

            public string ReadAllTextFromFile( string path )
            {
                return File.ReadAllText( path );
            }
        }
    }

    public interface IIOContext
    {
        string GetCurrentDirectory();

        bool DirectoryExists( string path );

        string[] GetFilesFromDirectory( string path, string searchPattern, SearchOption searchOption );

        string ReadAllTextFromFile( string path );
    }
}
