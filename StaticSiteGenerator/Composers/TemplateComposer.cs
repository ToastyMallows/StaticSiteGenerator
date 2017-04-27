using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;

namespace StaticSiteGenerator.Composers
{
    internal abstract class TemplateComposer : ITemplateComposer
    {
        protected string fullPathToTemplate = null;

        public abstract void Compose();

        protected abstract string TemplateFile { get; }

        protected string FullPathToTemplate
        {
            get
            {
                if ( fullPathToTemplate == null )
                {
                    fullPathToTemplate = Path.GetFullPath( TemplateFile );
                }
                return fullPathToTemplate;
            }
        }

        protected void ensureTemplateHtmlIsValid()
        {
            ( new HtmlDocument() ).Load( FullPathToTemplate );
        }

        protected string getFolderName( string path )
        {
            return Regex.Replace( path, @"[!@#$%^&*()\[\]\\\/:;'"".,?=+{}|_~`<>]", string.Empty ).Replace( " ", "-" );
        }

        protected HtmlDocument CopyOfTemplate
        {
            get
            {
                HtmlDocument template = new HtmlDocument();
                template.Load( FullPathToTemplate );
                return template;
            }
        }

        protected static class Constants
        {
            // Common Constants
            public const string FILE_NAME = "index.html";
            public const string TitleClass = "SSGTitle";

            // Blog Constants
            public const string BlogID = "SSGBlog";
            public const string DateClass = "SSGDate";
            public const string NextID = "SSGNextPost";
            public const string PreviousID = "SSGPreviousPost";

            // Page Constants
            public const string PageID = "SSGPage";
        }
    }
}
