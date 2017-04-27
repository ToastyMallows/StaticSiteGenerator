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

        private const string SSG_HEADER_PAGES_XPATH = "//div[@id='" + Constants.SSGHeaderPagesID + "']";

        private readonly IBasePageProvider _fragmentProvider;
        private readonly IBasePageProvider _pageProvider;

        private readonly IDictionary<string, Func<Type,HtmlDocument>> _fragmentComposingFunctions;

        public IReadOnlyDictionary<string, Func<Type,HtmlDocument>> FragmentComposingFunctions
        {
            get
            {
                return new ReadOnlyDictionary<string, Func<Type,HtmlDocument>>(_fragmentComposingFunctions);
            }
        }

        public FragmentComposer(FragmentProvider fragmentProvider, PageProvider pageProvider)
        {
            Guard.VerifyArgumentNotNull(fragmentProvider, nameof(fragmentProvider));
            Guard.VerifyArgumentNotNull(pageProvider, nameof(pageProvider));

            _fragmentProvider = fragmentProvider;
            _pageProvider = pageProvider;
            _fragmentComposingFunctions = new Dictionary<string, Func<Type,HtmlDocument>>();

            compose();
        }

        private void compose()
        {
            IReadOnlyCollection<IBasePage> fragments = _fragmentProvider.Pages;
            foreach (IFragment fragment in fragments)
            {
                string composedFragmentID = fragment.Metadata.FragmentID;
                Func<Type,HtmlDocument> composedFragment;

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

        private Func<Type,HtmlDocument> getSpecialFragmentComposer(IFragment fragment, SpecialFragmentType specialFragmentType)
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

        private Func<Type,HtmlDocument> getSpecialHeaderFragmentComposingFunction(IFragment fragment)
        {
            return new Func<Type, HtmlDocument>((type) => 
            {
                string parentDirectoriesSubpath = "../../pages";

                if (type == typeof(Page))
                {
                    // If we're already on a page, just go up one level
                    parentDirectoriesSubpath = "..";
                }

                HtmlDocument fragmentTemplate = new HtmlDocument();
                fragmentTemplate.LoadHtml(fragment.HTML);

                IEnumerable<IBasePage> headerPages = _pageProvider.Pages.Where(page => ((IPage)page).Metadata.IsHeaderPage);

                HtmlNode headerPagesDiv = fragmentTemplate.DocumentNode.SelectSingleNode(SSG_HEADER_PAGES_XPATH);

                string headerHTML = "<div>";
                foreach (IPage page in headerPages)
                {
                    string linkText = page.Metadata.Title;
                    string path = parentDirectoriesSubpath.AppendPath(page.Path);

                    string div = $"<span><a href=\"{path}\">{linkText}</a></span>";

                    headerHTML += div;
                }
                headerHTML += "</div>";

                headerPagesDiv.InnerHtml = headerHTML;

                return fragmentTemplate;
            });
        }

        private Func<Type,HtmlDocument> getSpecialFooterFragmentComposingFunction(IFragment fragment)
        {
            // IDEA:
            // SSGFooter could just populate from a json file of data and format it the same every time?
            return new Func<Type, HtmlDocument>((type) =>
             {
                 HtmlDocument document = new HtmlDocument();
                 document.LoadHtml("<div>Footer</div>");
                 return document;
             });
        }

        private Func<Type,HtmlDocument> getFragmentComposingFunction(IFragment fragment)
        {
            throw new NotImplementedException();
        }
    }
}
