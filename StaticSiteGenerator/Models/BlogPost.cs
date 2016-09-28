namespace StaticSiteGenerator.Models
{
    internal class BlogPost : IBlogPost
    {
        public BlogPost( IBlogPostMetadata metadata, string postHTML )
        {
            Guard.VerifyArgumentNotNull( metadata, nameof( metadata ) );
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
