using StaticSiteGenerator.Contexts;
using System.IO;

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

    public sealed class TestDirectoryProvider : DirectoryProviderContext
    {
        private readonly bool _exists;
        private readonly string _currentDirectory;
        private readonly string[] _files;

        public TestDirectoryProvider(
            bool exists = Constants.DirectoryDoesNotExist,
            string currentDirectory = Constants.CurrentDirectory,
            string[] files = null )
        {
            files = files ?? Constants.NoFiles;

            _exists = exists;
            _currentDirectory = currentDirectory;
            _files = files;
        }
        public override bool Exists( string path )
        {
            return _exists;
        }

        public override string GetCurrentDirectory()
        {
            return _currentDirectory;
        }

        public override string[] GetFiles( string path, string searchPattern, SearchOption searchOption )
        {
            return _files;
        }

        public static void SetCurrentDirectorProviderContext(
            out TestDirectoryProvider testDirectoryProvider,
            bool exists = Constants.DirectoryDoesNotExist,
            string currentDirectory = Constants.CurrentDirectory,
            string[] files = null )
        {
            files = files ?? Constants.NoFiles;

            testDirectoryProvider = new TestDirectoryProvider( exists: exists, currentDirectory: currentDirectory, files: files );
            DirectoryProviderContext.Current = testDirectoryProvider;
        }
    }
}
