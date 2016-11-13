using HeyRed.MarkdownSharp;
using Newtonsoft.Json;
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
        private const string PAGE_FOLDER = "pages";

        public PageProvider( /*pageMetadataProvider*/ )
        {
            /*pageMetadata provider will have already parsed all the information about the
             page metadata files and provided it to the headerFragmentComposer. */
        }

        // TODO: Reconcile this logic with the BlogPostProvider
        public bool TryParseMetadataFiles( out IReadOnlyCollection<IBasePage> pages )
        {
            pages = null;
            string pagesDirectory = Path.Combine( IOContext.Current.GetCurrentDirectory(), PAGE_FOLDER );

            if ( !IOContext.Current.DirectoryExists( pagesDirectory ) )
            {
                ErrorWriterContext.Current.WriteLine( "No pages folder" );
                return false;
            }

            IEnumerable<string> jsonMetadataFiles = IOContext.Current.GetFilesFromDirectory( pagesDirectory, "*.json", SearchOption.AllDirectories );

            ICollection<IPageMetadata> pagesMetadata = processJsonMetadataFiles( jsonMetadataFiles );

            if ( pagesMetadata.IsEmpty() )
            {
                ErrorWriterContext.Current.WriteLine( "No page metadata files" );
                return false;
            }

            return processPages( pagesMetadata, out pages );
        }

        private static ICollection<IPageMetadata> processJsonMetadataFiles( IEnumerable<string> jsonMetadataFiles )
        {
            ICollection<IPageMetadata> pageMetadata = new Collection<IPageMetadata>();

            foreach ( string jsonFile in jsonMetadataFiles )
            {
                string json = File.ReadAllText( jsonFile );

                IPageMetadata metadata = JsonConvert.DeserializeObject<PageMetadata>( json );
                if ( metadata.LayoutType != LayoutType.Page )
                {
                    // This metadata file isn't for a page, move on!
                    continue;
                }

                pageMetadata.Add( metadata );
            }

            return pageMetadata;
        }

        private bool processPages( ICollection<IPageMetadata> pagesMetadata, out IReadOnlyCollection<IBasePage> pages )
        {
            pages = null;
            IList<IPage> returnPages = new List<IPage>();
            Markdown markdownTransformer = new Markdown();

            foreach ( IPageMetadata metadata in pagesMetadata )
            {
                try
                {
                    string pathToMarkdown = Path.GetFullPath( metadata.Markdown );
                    string markdown = File.ReadAllText( pathToMarkdown );
                    string html = markdownTransformer.Transform( markdown );
                    returnPages.Add( new Page( metadata, html ) );
                }
                catch ( Exception e )
                {
                    ErrorWriterContext.Current.WriteLine( e.ToString() );
                    return false;
                }
            }

            pages = new ReadOnlyCollection<IPage>( returnPages );

            return true;
        }
    }
}
