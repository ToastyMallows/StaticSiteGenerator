using System;
using System.Collections.Generic;

namespace StaticSiteGenerator.Models
{
    public interface IBlogPostMetadata : IBasePageMetadata
    {
        DateTime Date { get; set; }

        string Title { get; set; }

        IList<string> Categories { get; set; }
    }
}
