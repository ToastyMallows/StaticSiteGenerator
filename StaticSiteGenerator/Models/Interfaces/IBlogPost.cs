﻿namespace StaticSiteGenerator.Models
{
    public interface IBlogPost : IBasePage
    {
        IBlogPostMetadata Metadata
        {
            get;
        }
    }
}
