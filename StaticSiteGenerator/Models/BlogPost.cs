using System.Diagnostics;

namespace StaticSiteGenerator.Models
{
    [DebuggerDisplay( "Date = {Metadata.Date}, Title = {Metadata.Title}" )]
    internal class BlogPost : IBlogPost
    {

        public BlogPost( IBlogPostMetadata metadata, string postHTML )
        {
            Guard.VerifyArgumentNotNull( metadata, nameof( metadata ) );
            Guard.VerifyArgumentNotNull( postHTML, nameof( postHTML ) );

            Metadata = metadata;
            PostHTML = postHTML;
        }

        public IBlogPostMetadata Metadata
        {
            get;
        }

        public string PostHTML
        {
            get;
        }
    }
}
