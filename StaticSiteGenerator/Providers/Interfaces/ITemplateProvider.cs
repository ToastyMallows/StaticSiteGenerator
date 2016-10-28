using HtmlAgilityPack;

namespace StaticSiteGenerator.Providers
{
    internal interface ITemplateProvider
    {
        bool TryGetBlogTempalte( out HtmlDocument htmlDocument );
    }
}
