using System.IO;

namespace StaticSiteGenerator.Contexts
{
    public abstract class DirectoryProviderContext
    {
        private static DirectoryProviderContext _current;

        public static DirectoryProviderContext Current
        {
            get
            {
                if ( null == _current )
                {
                    _current = DefaultDirectoryProviderContext;
                }

                return _current;
            }
            set
            {
                Guard.VerifyArgumentNotNull( value, "DirectoryProviderContextSetValue" );
                _current = value;
            }
        }

        public static DirectoryProviderContext DefaultDirectoryProviderContext = new DotNetDirectoryProvider();

        public abstract string GetCurrentDirectory();

        public abstract bool Exists( string path );

        public abstract string[] GetFiles( string path, string searchPattern, SearchOption searchOption );
    }

    internal sealed class DotNetDirectoryProvider : DirectoryProviderContext
    {
        public override bool Exists( string path )
        {
            return Directory.Exists( path );
        }

        public override string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public override string[] GetFiles( string path, string searchPattern, SearchOption searchOption )
        {
            return Directory.GetFiles( path, searchPattern, searchOption );
        }
    }
}
