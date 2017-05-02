using StaticSiteGenerator.Composers;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Generators;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Providers;
using System;
using System.Collections.Generic;
using static System.FormattableString;

namespace StaticSiteGenerator
{
    public class Program
    {
        public static int Main(string[] args)
        {
            IOptions options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                ErrorWriterContext.Current.WriteLine(options.GetUsage());
                return Constants.ERROR;
            }
            OptionsContext.Current = new ProgramOptionsContext(options);


            if (IOContext.Current.DirectoryExists(OptionsContext.Current.Options.OutputDirectory))
            {
                if (!OptionsContext.Current.Options.Force)
                {
                    ErrorWriterContext.Current.WriteLine(Invariant($"Output Directory '{OptionsContext.Current.Options.OutputDirectory}' already exists in the current folder."));
                    return Constants.ERROR;
                }
                else
                {
                    System.IO.Directory.Delete(OptionsContext.Current.Options.OutputDirectory, true);
                }
            }

            // Metadata Providers
            IMetadataProvider fragmentMetadataProvider = new MetadataProvider(LayoutType.Fragment);
            IMetadataProvider pageMetadataProvider = new MetadataProvider(LayoutType.Page);
            IMetadataProvider blogPostMetadataProvider = new MetadataProvider(LayoutType.Post);

            // Page Providers
            FragmentProvider fragmentProvider = new FragmentProvider(fragmentMetadataProvider);
            PageProvider pageProvider = new PageProvider(pageMetadataProvider);
            BlogPostProvider blogPostProvider = new BlogPostProvider(blogPostMetadataProvider);
            // IDEA: have option to pick between static page and first blog page?
            // TODO: make "Blog" an optional header page that takes the user to the latest blog post

            IFragmentComposer fragmentComposer = new FragmentComposer(fragmentProvider, pageProvider);
            IReadOnlyDictionary<string, Func<LayoutType, HtmlAgilityPack.HtmlDocument>> fragmentComposingFunctions = fragmentComposer.FragmentComposingFunctions;

            // Composers
            ITemplateComposer pageComposer = new PageComposer(pageProvider, fragmentComposingFunctions);
            ITemplateComposer blogPostComposer = new BlogComposer(blogPostProvider, fragmentComposingFunctions);

            IList<IGenerator> generators = new List<IGenerator>()
            {
                new BasePageGenerator(pageComposer),
                new BasePageGenerator(blogPostComposer)
            };

            IGenerator siteGenerator = new SiteGenerator(generators);

            try
            {
                siteGenerator.Generate();
            }
            catch
            {
                // Other places that throw should write out an error message
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
