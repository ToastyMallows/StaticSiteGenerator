using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Providers;
using System.Collections.Generic;

namespace StaticSiteGenerator
{
    public class Program
    {
        public static int Main( string[] args )
        {
            IOptions options = new Options();
            if ( !CommandLine.Parser.Default.ParseArguments( args, options ) )
            {
                ErrorWriterContext.Current.WriteLine( options.GetUsage() );
                return Constants.ERROR;
            }

            IBlogPostProvider blogPostProvider = new BlogPostProvider();

            IEnumerable<IBlogPost> blogPosts;
            bool blogPostProcessingSuccess = blogPostProvider.TryGetBlogPostsDescending( out blogPosts );

            if ( !blogPostProcessingSuccess )
            {
                ErrorWriterContext.Current.WriteLine( "Error processing blog post files" );
                return Constants.ERROR;
            }

            return Constants.SUCCESS;
        }

        #region Constants

        private static class Constants
        {
            public const int SUCCESS = 0;
            public const int ERROR = 1;
        }

        #endregion
    }
}
