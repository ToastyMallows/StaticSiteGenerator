﻿using System.IO;

namespace StaticSiteGenerator.Contexts
{
    public abstract class IOContext : IIOContext
    {
        private static IIOContext _current;

        public static IIOContext Current
        {
            get
            {
                if (null == _current)
                {
                    _current = DefaultIOContext;
                }
                return _current;
            }
            set
            {
                IIOContext ioContext = value as IIOContext;
                Guard.VerifyArgumentNotNull(value, "IIOContextSetValue");
                _current = ioContext;
            }
        }

        public static IIOContext DefaultIOContext = new DotNetIOContext();

        public abstract bool DirectoryExists(string path);

        public abstract string GetCurrentDirectory();

        public abstract string[] GetFilesFromDirectory(string path, string searchPattern, SearchOption searchOption);

        public abstract string ReadAllTextFromFile(string path);

        public abstract DirectoryInfo CreateDirectory(string path);

        public abstract void FileCopy(string source, string desination, bool canOverwrite = true);

        private class DotNetIOContext : IIOContext
        {
            public bool DirectoryExists(string path)
            {
                return Directory.Exists(path);
            }

            public string GetCurrentDirectory()
            {
                return Directory.GetCurrentDirectory();
            }

            public string[] GetFilesFromDirectory(string path, string searchPattern, SearchOption searchOption)
            {
                return Directory.GetFiles(path, searchPattern, searchOption);
            }

            public string ReadAllTextFromFile(string path)
            {
                return File.ReadAllText(path);
            }

            public DirectoryInfo CreateDirectory(string path)
            {
                return Directory.CreateDirectory(path);
            }

            public void FileCopy(string source, string destination, bool canOverwrite = true)
            {
                File.Copy(source, destination, canOverwrite);
            }
        }
    }

    public interface IIOContext
    {
        string GetCurrentDirectory();

        bool DirectoryExists(string path);

        string[] GetFilesFromDirectory(string path, string searchPattern, SearchOption searchOption);

        string ReadAllTextFromFile(string path);

        DirectoryInfo CreateDirectory(string path);

        void FileCopy(string source, string destination, bool canOverwrite = true);
    }
}
