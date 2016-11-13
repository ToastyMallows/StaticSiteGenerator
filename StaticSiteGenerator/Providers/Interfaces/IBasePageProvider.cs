using StaticSiteGenerator.Models;
using System.Collections.Generic;

namespace StaticSiteGenerator.Providers
{
    internal interface IBasePageProvider
    {
        bool TryParseMetadataFiles( out IReadOnlyCollection<IBasePage> basePage );
    }
}
