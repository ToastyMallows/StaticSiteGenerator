using HeyRed.MarkdownSharp;
using Newtonsoft.Json;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace StaticSiteGenerator.Providers
{
    internal sealed class BlogPostProvider : IBasePageProvider
    {
        private IMetadataProvider _blogPostMetadataProvider;

        private readonly IList<IBasePage> _pages;

        public IReadOnlyCollection<IBasePage> Pages
        {
            get
            {
                return new ReadOnlyCollection<IBasePage>(_pages);
            }
        }

        public BlogPostProvider(IMetadataProvider blogPostMetadataProvider)
        {
            Guard.VerifyArgumentNotNull(blogPostMetadataProvider, nameof(blogPostMetadataProvider));

            _blogPostMetadataProvider = blogPostMetadataProvider;

            _pages = ParseMetadataFiles();
        }

        private IList<IBasePage> ParseMetadataFiles()
        {
            IList<IBasePage> returnBlogPosts = new List<IBasePage>();
            Markdown markdownTransformer = new Markdown();

            foreach (IBlogPostMetadata metadata in _blogPostMetadataProvider.Metadata)
            {
                try
                {
                    string pathToMarkdown = Path.GetFullPath(metadata.Markdown);
                    string markdown = File.ReadAllText(pathToMarkdown);
                    string html = markdownTransformer.Transform(markdown);
                    returnBlogPosts.Add(new BlogPost(metadata, html));
                }
                catch (Exception e)
                {
                    ErrorWriterContext.Current.WriteLine(e.ToString());
                    throw e;
                }
            }

            return returnBlogPosts.OrderByDescending(blogPost => ((IBlogPost)blogPost).Metadata.Date).ToList();
        }
    }
}
