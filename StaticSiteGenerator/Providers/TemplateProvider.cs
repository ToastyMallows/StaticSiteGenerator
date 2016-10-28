using HtmlAgilityPack;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using System;
using System.IO;

namespace StaticSiteGenerator.Providers
{
    internal sealed class TemplateProvider : ITemplateProvider
    {
        private readonly IOptions _options;

        public TemplateProvider( IOptions options )
        {
            Guard.VerifyArgumentNotNull( options, nameof( options ) );

            _options = options;
        }

        public bool TryGetBlogTempalte( out HtmlDocument htmlDocument )
        {
            htmlDocument = null;

            string pathToTemplate;
            try
            {
                pathToTemplate = Path.GetFullPath( _options.TemplateFile );
            }
            catch ( Exception e )
            {
                ErrorWriterContext.Current.WriteLine( e.ToString() );
                return false;
            }

            htmlDocument = new HtmlDocument();

            try
            {
                htmlDocument.Load( pathToTemplate );
            }
            catch ( Exception e )
            {
                ErrorWriterContext.Current.WriteLine( e.ToString() );
                return false;
            }

            return true;
        }
    }
}
