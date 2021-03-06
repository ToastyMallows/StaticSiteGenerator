﻿using HeyRed.MarkdownSharp;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace StaticSiteGenerator.Providers
{
    internal class PageProvider : IBasePageProvider
    {
        private IMetadataProvider _pageMetadataProvider;

        private readonly IList<IBasePage> _pages;

        public IReadOnlyCollection<IBasePage> Pages
        {
            get
            {
                return new ReadOnlyCollection<IBasePage>(_pages);
            }
        }

        public PageProvider( IMetadataProvider pageMetadataProvider )
        {
            Guard.VerifyArgumentNotNull(pageMetadataProvider, nameof(pageMetadataProvider));

            _pageMetadataProvider = pageMetadataProvider;

            _pages = ParseMetadataFiles();
        }

        // TODO: Reconcile this logic with the BlogPostProvider
        private IList<IBasePage> ParseMetadataFiles()
        {
            IList<IBasePage> returnPages = new List<IBasePage>();
            Markdown markdownTransformer = new Markdown();

            foreach (IPageMetadata metadata in _pageMetadataProvider.Metadata)
            {
                try
                {
                    string pathToMarkdown = Path.GetFullPath(metadata.Markdown);
                    string markdown = File.ReadAllText(pathToMarkdown);
                    string html = markdownTransformer.Transform(markdown);
                    returnPages.Add(new Page(metadata, html));
                }
                catch (Exception e)
                {
                    ErrorWriterContext.Current.WriteLine(e.ToString());
                    throw e;
                }
            }

            return returnPages;
        }
    }
}
