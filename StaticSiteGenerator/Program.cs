using StaticSiteGenerator.Composers;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Generators;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Providers;
using System.Collections.Generic;
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
            OptionsContext.Current = new ProgramOptionsContext( options );


            if ( IOContext.Current.DirectoryExists( OptionsContext.Current.Options.OutputDirectory ) )
            {
                if ( !OptionsContext.Current.Options.Force )
                {
                    ErrorWriterContext.Current.WriteLine( Invariant( $"Output Directory '{OptionsContext.Current.Options.OutputDirectory}' already exists in the current folder." ) );
                    return Constants.ERROR;
                }
                else
                {
                    System.IO.Directory.Delete( OptionsContext.Current.Options.OutputDirectory, true );
                }
            }


            ICollection<IGenerator> generators = new List<IGenerator>();

            IBasePageProvider blogPostProvider = new BlogPostProvider();
            ITemplateComposer blogPostComposer = new BlogTemplateComposer();

            generators.Add( new BasePageGenerator( blogPostProvider, blogPostComposer ) );

            IBasePageProvider pageProvider = new PageProvider();
            ITemplateComposer pageComposer = new PageTemplateComposer();

            generators.Add( new BasePageGenerator( pageProvider, pageComposer ) );

            IGenerator siteGenerator = new SiteGenerator( generators );

            if ( siteGenerator.TryGenerate() )
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
