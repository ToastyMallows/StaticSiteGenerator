using StaticSiteGenerator.Models;
using System.Collections.Generic;

namespace StaticSiteGenerator.Providers
{
    internal interface ITemplateProvider
    {
        bool TryMakeBlogPosts( IReadOnlyCollection<IBlogPost> blogPosts );
    }
}
