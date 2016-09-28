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
    internal sealed class BlogPostProvider : IBlogPostProvider
    {
        private const string POST_FOLDER = "posts";

        public bool TryGetBlogPostsDescending( out IEnumerable<IBlogPost> blogPosts )
        {
            blogPosts = null;
            string postsDirectory = Path.Combine( DirectoryProviderContext.Current.GetCurrentDirectory(), POST_FOLDER );

            if ( !DirectoryProviderContext.Current.Exists( postsDirectory ) )
            {
                ErrorWriterContext.Current.WriteLine( "No posts folder" );
                return false;
            }

            IEnumerable<string> jsonMetadataFiles = DirectoryProviderContext.Current.GetFiles( postsDirectory, "*.json", SearchOption.AllDirectories );

            ICollection<IBlogPostMetadata> blogPostMetadata = processJsonMetadataFiles( jsonMetadataFiles );

            if ( blogPostMetadata.IsEmpty() )
            {
                ErrorWriterContext.Current.WriteLine( "No metadata files" );
                return false;
            }

            return processBlogPosts( blogPostMetadata, out blogPosts );
        }

        private static ICollection<IBlogPostMetadata> processJsonMetadataFiles( IEnumerable<string> jsonMetadataFiles )
        {
            ICollection<IBlogPostMetadata> blogPostMetadata = new Collection<IBlogPostMetadata>();

            foreach ( string jsonFile in jsonMetadataFiles )
            {
                string json = File.ReadAllText( jsonFile );
                blogPostMetadata.Add( JsonConvert.DeserializeObject<BlogPostMetadata>( json ) );
            }

            return blogPostMetadata;
        }

        private static bool processBlogPosts( IEnumerable<IBlogPostMetadata> blogPostMetadata, out IEnumerable<IBlogPost> blogPosts )
        {
            bool success = true;
            ICollection<IBlogPost> returnBlogPosts = new Collection<IBlogPost>();
            Markdown markdownTransformer = new Markdown();

            foreach ( IBlogPostMetadata metadata in blogPostMetadata )
            {
                try
                {
                    string pathToMarkdown = Path.GetFullPath( metadata.Markdown );
                    string markdown = File.ReadAllText( pathToMarkdown );
                    string html = markdownTransformer.Transform( markdown );
                    returnBlogPosts.Add( new BlogPost( metadata, html ) );
                }
                catch ( Exception e )
                {
                    ErrorWriterContext.Current.WriteLine( e.ToString() );
                    success = false;
                }
            }

            blogPosts = success ?
                    // Order by latest date first
                    returnBlogPosts.OrderByDescending( blogPost => blogPost.Metadata.Date ).AsEnumerable()
                    :
                    null;

            return success;
        }
    }
}
