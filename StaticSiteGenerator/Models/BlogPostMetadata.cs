using System;
using System.Collections.Generic;

namespace StaticSiteGenerator.Models
{
    internal class BlogPostMetadata : IBlogPostMetadata
    {
        public LayoutType LayoutType { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public IList<string> Categories { get; set; }
        public string Markdown { get; set; }
    }
}
