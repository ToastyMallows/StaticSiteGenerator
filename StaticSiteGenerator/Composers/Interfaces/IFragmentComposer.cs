using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace StaticSiteGenerator.Composers
{
    interface IFragmentComposer
    {
        IReadOnlyDictionary<string, Func<Type,HtmlDocument>> FragmentComposingFunctions { get; }
    }
}
