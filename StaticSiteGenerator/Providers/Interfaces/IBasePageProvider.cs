﻿using StaticSiteGenerator.Models;
using System.Collections.Generic;

namespace StaticSiteGenerator.Providers
{
    internal interface IBasePageProvider
    {
        IReadOnlyCollection<IBasePage> Pages { get; }
    }
}
