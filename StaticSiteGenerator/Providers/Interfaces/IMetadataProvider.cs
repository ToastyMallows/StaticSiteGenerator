using StaticSiteGenerator.Models;
using System.Collections.Generic;

namespace StaticSiteGenerator.Providers
{
    internal interface IMetadataProvider
    {
        IReadOnlyCollection<IBasePageMetadata> Metadata { get; }
    }
}