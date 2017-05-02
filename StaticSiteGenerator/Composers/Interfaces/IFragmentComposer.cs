using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace StaticSiteGenerator.Composers
{
    interface IFragmentComposer
    {
        IReadOnlyDictionary<string, Func<LayoutType,HtmlDocument>> FragmentComposingFunctions { get; }
    }
}
