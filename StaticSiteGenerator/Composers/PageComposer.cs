using HtmlAgilityPack;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Providers;
using System;
using System.Collections.Generic;
using System.IO;

namespace StaticSiteGenerator.Composers
{
    internal class PageComposer : TemplateComposer
    {
        private const string PageXPath = "//div[@id='" + Constants.PageID + "']";
        private const string TitleXPath = "//*[contains(@class,'" + Constants.TitleClass + "')]";
        private const string PAGES_FOLDER = "pages";
        private readonly IBasePageProvider _pageProvider;
        private readonly IReadOnlyDictionary<string, Func<Type,HtmlDocument>> _composedFragments;


        public PageComposer( IBasePageProvider pageProvider, IReadOnlyDictionary<string, Func<Type, HtmlDocument>> composedFragments )
        {
            Guard.VerifyArgumentNotNull(pageProvider, nameof(pageProvider));
            Guard.VerifyArgumentNotNull(composedFragments, nameof(composedFragments));

            _pageProvider = pageProvider;
            _composedFragments = composedFragments;

            ensureTemplateHtmlIsValid();
        }

        protected override string TemplateFile
        {
            get
            {
                return OptionsContext.Current.Options.PageTemplateFile;
            }
        }

        public override void Compose()
        {
            IReadOnlyCollection<IBasePage> basePageData = _pageProvider.Pages;
            foreach ( IPage page in basePageData )
            {
                try
                {
                    HtmlDocument templateToPopulate = CopyOfTemplate;

                    replacePageDiv(templateToPopulate, page);
                    replaceAllTitles(templateToPopulate, page);
                    replaceAllFragments(templateToPopulate);

                    string savePath = GetFullPathWithFileName( page.Metadata.Title );

                    templateToPopulate.Save( savePath );
                }
                catch ( Exception e )
                {
                    ErrorWriterContext.Current.WriteLine( e.ToString() );
                    throw e;
                }
            }
        }

        private string GetFullPathWithFileName( string pageTitle )
        {
            string folderToCreate = Path.Combine( OptionsContext.Current.Options.OutputDirectory, PAGES_FOLDER, getFolderName( pageTitle ).ToLower() );
            IOContext.Current.CreateDirectory( folderToCreate );

            string folderToCreateFullPath = Path.GetFullPath( folderToCreate );

            return Path.Combine( folderToCreateFullPath, Constants.FILE_NAME );
        }

        private void replaceAllTitles(HtmlDocument template, IPage page)
        {
            HtmlNodeCollection titles = template.DocumentNode.SelectNodes(TitleXPath);

            foreach (HtmlNode title in titles)
            {
                title.InnerHtml = page.Metadata.Title;
            }
        }

        private void replacePageDiv( HtmlDocument template, IPage page )
        {
            HtmlNode pageDiv = template.DocumentNode.SelectSingleNode( PageXPath );
            pageDiv.InnerHtml = page.HTML;
        }

        private void replaceAllFragments(HtmlDocument template)
        {
            foreach (string fragmentID in _composedFragments.Keys)
            {
                string fragmentXPath = "//*[@id='" + fragmentID + "']";
                HtmlNodeCollection fragmentNodes = template.DocumentNode.SelectNodes(fragmentXPath);

                if (null == fragmentNodes)
                {
                    continue;
                }

                Func<Type, HtmlDocument> composedFragment = _composedFragments[fragmentID];
                foreach (HtmlNode fragmentNode in fragmentNodes)
                {
                    fragmentNode.InnerHtml = composedFragment(typeof(Page)).DocumentNode.InnerHtml;
                }
            }
        }
    }
}
