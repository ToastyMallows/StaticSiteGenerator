using StaticSiteGenerator.Models;
using System.Collections.Generic;

namespace StaticSiteGenerator.Providers
{
    internal interface IBlogPostProvider
    {
        bool TryGetBlogPostsDescending( out IReadOnlyCollection<IBlogPost> blogPosts );
    }
}
