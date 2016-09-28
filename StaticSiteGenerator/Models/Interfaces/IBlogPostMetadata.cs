using System;
using System.Collections.Generic;

namespace StaticSiteGenerator.Models
{
    public interface IBlogPostMetadata
    {
        Layout Layout { get; set; }

        string Title { get; set; }

        DateTime Date { get; set; }

        IList<string> Categories { get; set; }

        string Markdown { get; set; }
    }
}
