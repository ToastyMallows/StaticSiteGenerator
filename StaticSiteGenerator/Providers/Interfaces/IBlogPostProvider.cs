using StaticSiteGenerator.Models;
using System.Collections.Generic;

namespace StaticSiteGenerator.Providers
{
    interface IBlogPostProvider
    {
        bool TryGetBlogPostsDescending( out IEnumerable<IBlogPost> blogPosts );
    }
}
