using System;
using System.ComponentModel;

namespace StaticSiteGenerator
{
    public enum LayoutType
    {
        [Obsolete("Not to be used by code, error state")]
        Undefined = 0,

        [Description("post")]
        Post = 1,

        [Description("page")]
        Page = 2,

        [Description("fragment")]
        Fragment = 3,
    }

    public enum SpecialFragmentType
    {
        [Obsolete("Not to be used by code, error state")]
        Undefined = 0,

        SSGHeader = 1,

        SSGFooter = 2
    }
}
