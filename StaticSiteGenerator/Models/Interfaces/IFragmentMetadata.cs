namespace StaticSiteGenerator.Models
{
    public interface IFragmentMetadata : IBasePageMetadata
    {
        string FragmentID { get; set; }
    }
}
