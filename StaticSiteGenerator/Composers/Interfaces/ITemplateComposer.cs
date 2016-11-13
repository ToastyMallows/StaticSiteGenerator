using StaticSiteGenerator.Models;
using System.Collections.Generic;

namespace StaticSiteGenerator.Composers
{
    public interface ITemplateComposer
    {
        bool TryCompose( IReadOnlyCollection<IBasePage> basePageData );
    }
}
