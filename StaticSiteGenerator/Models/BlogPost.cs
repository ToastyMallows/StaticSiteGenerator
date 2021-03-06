﻿using System.Diagnostics;

namespace StaticSiteGenerator.Models
{
    [DebuggerDisplay("Date = {Metadata.Date}, Title = {Metadata.Title}")]
    internal class BlogPost : IBlogPost
    {

        public BlogPost(IBlogPostMetadata metadata, string html)
        {
            Guard.VerifyArgumentNotNull(metadata, nameof(metadata));
            Guard.VerifyArgumentNotNull(html, nameof(html));

            Metadata = metadata;
            HTML = html;
        }

        public IBlogPostMetadata Metadata
        {
            get;
        }

        public string HTML
        {
            get;
        }
    }
}
