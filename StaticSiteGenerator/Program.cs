using Newtonsoft.Json;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace StaticSiteGenerator
{
    public class Program
    {
        public static int Main( string[] args )
        {
            IOptions options = new Options();
            if ( !CommandLine.Parser.Default.ParseArguments( args, options ) )
            {
                Console.WriteLine( options.GetUsage() );
                return Constants.ERROR;
            }

            if ( !options.Generate )
            {
                Console.WriteLine( "No generate switch" );
                return Constants.ERROR;
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            string postsDirectory = Path.Combine( currentDirectory, Constants.POST_FOLDER );

            if ( !Directory.Exists( postsDirectory ) )
            {
                Console.WriteLine( "No posts folder" );
                return Constants.ERROR;
            }

            IEnumerable<string> jsonMetadataFiles = Directory.GetFiles( postsDirectory, "*.json", SearchOption.AllDirectories );
            IEnumerable<string> markdownFiles =
                Directory.GetFiles( postsDirectory, "*.md", SearchOption.AllDirectories ).Concat(
                    Directory.GetFiles( postsDirectory, "*.markdown", SearchOption.AllDirectories ) );

            IEnumerable<IBlogPostMetadata> blogPostMetadata = processJsonMetadataFiles( jsonMetadataFiles );

            return Constants.SUCCESS;
        }

        #region Helpers

        internal static IEnumerable<IBlogPostMetadata> processJsonMetadataFiles( IEnumerable<string> jsonMetadataFiles )
        {
            ICollection<IBlogPostMetadata> blogPostMetadata = new Collection<IBlogPostMetadata>();

            foreach ( string jsonFile in jsonMetadataFiles )
            {
                string json = File.ReadAllText( jsonFile );
                blogPostMetadata.Add( JsonConvert.DeserializeObject<BlogPostMetadata>( json ) );
            }

            return blogPostMetadata.OrderByDescending( metadata => metadata.Date );
        }

        #endregion

        #region Constants

        private static class Constants
        {
            public const string POST_FOLDER = "posts";
            public const int SUCCESS = 0;
            public const int ERROR = 1;
        }

        #endregion
    }
}
