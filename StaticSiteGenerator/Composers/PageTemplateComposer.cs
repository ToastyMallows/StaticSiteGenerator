using HtmlAgilityPack;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace StaticSiteGenerator.Composers
{
    internal class PageTemplateComposer : TemplateComposer
    {
        private const string PageXPath = "//div[@id='" + Constants.PageID + "']";

        public PageTemplateComposer()
        {
            ensureTemplateHtmlIsValid();
        }

        protected override string TemplateFile
        {
            get
            {
                return OptionsContext.Current.Options.PageTemplateFile;
            }
        }

        public override bool TryCompose( IReadOnlyCollection<IBasePage> basePageData )
        {
            foreach ( IPage page in basePageData )
            {
                try
                {
                    HtmlDocument templateToPopulate = CopyOfTemplate;

                    replacePageDiv( templateToPopulate, page );

                    string savePath = getFullPathWithFileName( page.Metadata.Title );

                    templateToPopulate.Save( savePath );
                }
                catch ( Exception e )
                {
                    ErrorWriterContext.Current.WriteLine( e.ToString() );
                    return false;
                }
            }

            return true;
        }

        private string getFullPathWithFileName( string pageTitle )
        {
            string folderToCreate = Path.Combine( OptionsContext.Current.Options.OutputDirectory, getFolderName( pageTitle ).ToLower() );
            IOContext.Current.CreateDirectory( folderToCreate );

            string folderToCreateFullPath = Path.GetFullPath( folderToCreate );

            return Path.Combine( folderToCreateFullPath, Constants.FILE_NAME );
        }

        private void replacePageDiv( HtmlDocument template, IPage page )
        {
            HtmlNode pageDiv = template.DocumentNode.SelectSingleNode( PageXPath );
            pageDiv.InnerHtml = page.HTML;
        }
    }
}
