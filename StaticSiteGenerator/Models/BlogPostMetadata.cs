using System;
using System.Collections.Generic;

namespace StaticSiteGenerator.Models
{
    internal class BlogPostMetadata : IBlogPostMetadata
    {
        public Layout Layout { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public IList<string> Categories { get; set; }
        public string Markdown { get; set; }
    }
}
