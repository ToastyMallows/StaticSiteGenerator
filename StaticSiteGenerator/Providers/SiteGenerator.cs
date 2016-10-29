using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using System.Collections.Generic;

namespace StaticSiteGenerator.Providers
{
    internal sealed class SiteGenerator : ISiteGenerator
    {
        private readonly IBlogPostProvider _blogPostProvider;
        private readonly ITemplateProvider _templateProvider;

        public SiteGenerator( IBlogPostProvider blogPostProvider, ITemplateProvider templateProvider )
        {
            Guard.VerifyArgumentNotNull( blogPostProvider, nameof( blogPostProvider ) );
            Guard.VerifyArgumentNotNull( templateProvider, nameof( templateProvider ) );

            _blogPostProvider = blogPostProvider;
            _templateProvider = templateProvider;
        }

        public bool TryGenerateSite()
        {
            IReadOnlyCollection<IBlogPost> blogPosts;
            bool blogPostProcessingSuccess = _blogPostProvider.TryGetBlogPostsDescending( out blogPosts );

            if ( !blogPostProcessingSuccess )
            {
                ErrorWriterContext.Current.WriteLine( "Error processing blog post files" );
                return false;
            }

            // At this point blog posts should be in descending order

            bool blogPostsCreated = _templateProvider.TryMakeBlogPosts( blogPosts );
            if ( !blogPostsCreated )
            {
                ErrorWriterContext.Current.WriteLine( "Error creating blog posts" );
                return false;
            }

            return true;
        }
    }
}
