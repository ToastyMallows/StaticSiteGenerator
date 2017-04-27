using System;
using System.Collections.Generic;
using StaticSiteGenerator.Models;
using System.Collections.ObjectModel;
using HeyRed.MarkdownSharp;
using StaticSiteGenerator.Contexts;
using System.IO;

namespace StaticSiteGenerator.Providers
{
    class FragmentProvider : IBasePageProvider
    {
        private IMetadataProvider _fragmentMetadataProvider;

        private readonly IList<IBasePage> _fragments;

        public IReadOnlyCollection<IBasePage> Pages
        {
            get
            {
                return new ReadOnlyCollection<IBasePage>(_fragments);
            }
        }

        public FragmentProvider(IMetadataProvider fragmentMetadataProvider)
        {
            Guard.VerifyArgumentNotNull(fragmentMetadataProvider, nameof(fragmentMetadataProvider));

            _fragmentMetadataProvider = fragmentMetadataProvider;

            _fragments = ParseMetadataFiles();
        }

        private IList<IBasePage> ParseMetadataFiles()
        {
            IList<IBasePage> returnFragments = new List<IBasePage>();
            Markdown markdownTransformer = new Markdown();

            foreach (IFragmentMetadata metadata in _fragmentMetadataProvider.Metadata)
            {
                try
                {
                    string pathToMarkdown = Path.GetFullPath(metadata.Markdown);
                    string markdown = File.ReadAllText(pathToMarkdown);
                    string html = markdownTransformer.Transform(markdown);
                    returnFragments.Add(new Fragment(metadata, html));
                }
                catch (Exception e)
                {
                    ErrorWriterContext.Current.WriteLine(e.ToString());
                    throw e;
                }
            }

            return returnFragments;
        }
    }
}
