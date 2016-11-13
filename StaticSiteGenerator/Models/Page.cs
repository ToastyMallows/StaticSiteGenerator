namespace StaticSiteGenerator.Models
{
    internal class Page : IPage
    {
        public Page( IPageMetadata metadata, string html )
        {
            Guard.VerifyArgumentNotNull( metadata, nameof( metadata ) );
            Guard.VerifyArgumentNotNull( html, nameof( html ) );

            Metadata = metadata;
            HTML = html;
        }

        public IPageMetadata Metadata
        {
            get;
        }

        public string HTML
        {
            get;
        }
    }
}
