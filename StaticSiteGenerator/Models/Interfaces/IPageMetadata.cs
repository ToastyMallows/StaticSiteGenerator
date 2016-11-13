namespace StaticSiteGenerator.Models
{
    public interface IPageMetadata : IBasePageMetadata
    {
        bool IsHeaderPage { get; set; }
    }
}
