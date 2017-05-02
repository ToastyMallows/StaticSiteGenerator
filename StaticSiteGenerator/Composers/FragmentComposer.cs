using HtmlAgilityPack;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static System.FormattableString;

namespace StaticSiteGenerator.Composers
{
    internal class FragmentComposer : IFragmentComposer
    {
        private static class Constants
        {
            public const string SSGHeaderPagesID = "SSGHeaderPages";
        }

        private const string SSG_HEADER_PAGES_XPATH = "//*[@id='" + Constants.SSGHeaderPagesID + "']";

        private readonly IBasePageProvider _fragmentProvider;
        private readonly IBasePageProvider _pageProvider;

        private readonly IDictionary<string, Func<LayoutType,HtmlDocument>> _fragmentComposingFunctions;

        public IReadOnlyDictionary<string, Func<LayoutType,HtmlDocument>> FragmentComposingFunctions
        {
            get
            {
                return new ReadOnlyDictionary<string, Func<LayoutType,HtmlDocument>>(_fragmentComposingFunctions);
            }
        }

        public FragmentComposer(FragmentProvider fragmentProvider, PageProvider pageProvider)
        {
            Guard.VerifyArgumentNotNull(fragmentProvider, nameof(fragmentProvider));
            Guard.VerifyArgumentNotNull(pageProvider, nameof(pageProvider));

            _fragmentProvider = fragmentProvider;
            _pageProvider = pageProvider;
            _fragmentComposingFunctions = new Dictionary<string, Func<LayoutType,HtmlDocument>>();

            compose();
        }

        private void compose()
        {
            IReadOnlyCollection<IBasePage> fragments = _fragmentProvider.Pages;
            foreach (IFragment fragment in fragments)
            {
                string composedFragmentID = fragment.Metadata.FragmentID;
                Func<LayoutType,HtmlDocument> composedFragment;

                SpecialFragmentType specialFragmentType;
                if (Enum.TryParse(composedFragmentID, out specialFragmentType))
                {
                    composedFragment = getSpecialFragmentComposer(fragment, specialFragmentType);
                }
                else
                {
                    composedFragment = getFragmentComposingFunction(fragment);
                }

                _fragmentComposingFunctions.Add(composedFragmentID, composedFragment);
            }
        }

        private Func<LayoutType,HtmlDocument> getSpecialFragmentComposer(IFragment fragment, SpecialFragmentType specialFragmentType)
        {
            switch (specialFragmentType)
            {
                case SpecialFragmentType.SSGHeader:
                    return getSpecialHeaderFragmentComposingFunction(fragment);
                case SpecialFragmentType.SSGFooter:
                    return getSpecialFooterFragmentComposingFunction(fragment);
                default:
                    throw new InvalidOperationException(Invariant($"The Special Fragment ID of {specialFragmentType.ToString()} is unknown and not supported"));
            }
        }

        private Func<LayoutType,HtmlDocument> getSpecialHeaderFragmentComposingFunction(IFragment fragment)
        {
            return new Func<LayoutType, HtmlDocument>((layoutType) => 
            {
                string parentDirectoriesSubpath = null;

                if (layoutType == LayoutType.Page)
                {
                    // If we're already on a page, just go up one level
                    parentDirectoriesSubpath = "..";
                }
                else if (layoutType == LayoutType.MainPage)
                {
                    // We're on the root level for the index page, don't go up any levels
                    parentDirectoriesSubpath = "./pages";
                }
                else
                {
                    // It's a blog post, go up two levels and go to the pages folder
                    parentDirectoriesSubpath = "../../pages";
                }

                HtmlDocument fragmentTemplate = new HtmlDocument();
                fragmentTemplate.LoadHtml(fragment.HTML);

                IEnumerable<IBasePage> headerPages = _pageProvider.Pages.Where(page => ((IPage)page).Metadata.IsHeaderPage);

                HtmlNode headerPagesNode = fragmentTemplate.DocumentNode.SelectSingleNode(SSG_HEADER_PAGES_XPATH);

                string headerPagesHTML = "";
                foreach (IPage page in headerPages)
                {
                    string linkText = page.Metadata.Title;
                    string path = parentDirectoriesSubpath.AppendPath(page.Path);

                    string link = $"<a href=\"{path}\">{linkText}</a>";

                    headerPagesHTML += link;
                }

                headerPagesNode.InnerHtml = headerPagesHTML;

                return fragmentTemplate;
            });
        }

        private Func<LayoutType,HtmlDocument> getSpecialFooterFragmentComposingFunction(IFragment fragment)
        {
            // IDEA:
            // SSGFooter could just populate from a json file of data and format it the same every time?
            return new Func<LayoutType, HtmlDocument>((layoutType) =>
             {
                 HtmlDocument fragmentTemplate = new HtmlDocument();
                 fragmentTemplate.LoadHtml(fragment.HTML);

                 return fragmentTemplate;
             });
        }

        private Func<LayoutType,HtmlDocument> getFragmentComposingFunction(IFragment fragment)
        {
            throw new NotImplementedException();
        }
    }
}
