using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Providers;
using static System.FormattableString;

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

            if ( IOContext.Current.DirectoryExists( options.OutputDirectory ) )
            {
                if ( !options.Force )
                {
                    ErrorWriterContext.Current.WriteLine( Invariant( $"Output Directory '{options.OutputDirectory}' already exists in the current folder." ) );
                    return Constants.ERROR;
                }
                else
                {
                    System.IO.Directory.Delete( options.OutputDirectory, true );
                }
            }

            IBlogPostProvider blogPostProvider = new BlogPostProvider( options );
            ITemplateProvider templateProvider = new TemplateProvider( options );

            // TODO: Make SiteGeneratorFactory?
            ISiteGenerator siteGenerator = new SiteGenerator( blogPostProvider, templateProvider );

            bool siteGenerationSuccess = siteGenerator.TryGenerateSite();

            if ( siteGenerationSuccess )
            {
                return Constants.SUCCESS;
            }

            return Constants.ERROR;
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
