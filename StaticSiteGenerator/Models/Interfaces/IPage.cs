namespace StaticSiteGenerator.Models
{
    public interface IPage : IBasePage
    {
        IPageMetadata Metadata
        {
            get;
        }
    }
}
