namespace StaticSiteGenerator.Models
{
    public interface IBlogPost
    {
        IBlogPostMetadata Metadata
        {
            get;
        }

        string PostHTML
        {
            get;
        }
    }
}
