namespace StaticSiteGenerator.Models
{
    public interface IBasePageMetadata
    {
        LayoutType LayoutType { get; set; }

        string Markdown { get; set; }
    }
}
