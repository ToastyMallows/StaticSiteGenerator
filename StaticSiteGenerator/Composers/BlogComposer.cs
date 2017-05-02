using HtmlAgilityPack;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static System.FormattableString;

namespace StaticSiteGenerator.Composers
{
    internal class BlogComposer : TemplateComposer
    {
        private const string NextPostXPath = "//a[@id='" + Constants.NextID + "']";
        private const string PreviousPostXPath = "//a[@id='" + Constants.PreviousID + "']";
        private const string BlogXPath = "//div[@id='" + Constants.BlogID + "']";
        private const string TitleXPath = "//*[contains(@class,'" + Constants.TitleClass + "')]";
        private const string HeadXPath = "//head";
        private const string DateXPath = "//*[contains(@class,'" + Constants.DateClass + "')]";
        private const string HREF = "href";

        private readonly IBasePageProvider _blogProvider;
        private readonly IReadOnlyDictionary<string, Func<LayoutType,HtmlDocument>> _composedFragments;

        public BlogComposer(IBasePageProvider blogProvider, IReadOnlyDictionary<string, Func<LayoutType,HtmlDocument>> composedFragments)
        {
            Guard.VerifyArgumentNotNull(blogProvider, nameof(blogProvider));
            Guard.VerifyArgumentNotNull(composedFragments, nameof(composedFragments));

            _blogProvider = blogProvider;
            _composedFragments = composedFragments;

            ensureTemplateHtmlIsValid();
        }

        protected override string TemplateFile
        {
            get
            {
                return OptionsContext.Current.Options.BlogTemplateFile;
            }
        }

        public override void Compose()
        {
            #region Copy CSS File to Output directory

            string cssFile = OptionsContext.Current.Options.BlogCSSFile;
            string cssFolder = Path.Combine(OptionsContext.Current.Options.OutputDirectory, "css");
            IOContext.Current.CreateDirectory(cssFolder);
            IOContext.Current.FileCopy(cssFile, Path.Combine(cssFolder, cssFile));

            #endregion

            IReadOnlyCollection <IBasePage> blogPosts = _blogProvider.Pages;
            int index = 1;
            int posts = blogPosts.Count;

            IList<PopulatedTemplate> populatedTemplates = new List<PopulatedTemplate>();
            PopulatedTemplate rootIndexTemplate = null;

            foreach (IBlogPost blogPost in blogPosts)
            {
                // ASSUMPTION: First blog post in the list is the newest, should have been sorted by date in the BlogPostProvider.
                // Only run this code once.
                if (rootIndexTemplate == null)
                {
                    rootIndexTemplate = createRootIndexPopulatedTemplate(blogPost, posts);
                }

                try
                {
                    HtmlDocument template = CopyOfTemplate;

                    addBlogCSS(template, blogPost);
                    replaceBlogDiv(template, blogPost);
                    replaceAllTitles(template, blogPost);
                    replaceAllDates(template, blogPost);
                    replaceAllFragments(template);

                    NavigationButtons buttonsNeeded;

                    if (posts == 1)
                    {
                        buttonsNeeded = NavigationButtons.None;
                    }
                    else
                    {
                        if (index == 1)
                        {
                            buttonsNeeded = NavigationButtons.PreviousOnly;
                        }
                        else if (index == posts)
                        {
                            buttonsNeeded = NavigationButtons.NextOnly;
                        }
                        else
                        {
                            buttonsNeeded = NavigationButtons.Both;
                        }
                    }

                    populatedTemplates.Add(new PopulatedTemplate(blogPost, template, buttonsNeeded, OptionsContext.Current.Options.OutputDirectory));
                }
                catch (Exception e)
                {
                    ErrorWriterContext.Current.WriteLine(Invariant($"Error creating blog post with title {blogPost.Metadata.Title}."));
                    ErrorWriterContext.Current.WriteLine(e.ToString());
                    throw e;
                }

                index++;
            }

            #region Root Index page creation

            // Create the main page root index file first
            if (rootIndexTemplate == null)
            {
                throw new InvalidOperationException("Stopped because the main page wasn't going to be created");
            }

            // Only two possible cases:
            //      1. There is only one blog post
            //      2. This is the first blog post of many
            // The first blog post should not need next or both navigation buttons.
            switch (rootIndexTemplate.ButtonsNeeded)
            {
                case NavigationButtons.None:
                    {
                        hideNext(rootIndexTemplate);
                        hidePrevious(rootIndexTemplate);
                    }
                    break;
                case NavigationButtons.PreviousOnly:
                    {
                        // ASSUMPTION: The first index will be the previous page, based on date sorting
                        PopulatedTemplate previousTemplate = populatedTemplates[1];
                        replacePrevious(rootIndexTemplate, previousTemplate.RootRelativePath);
                        hideNext(rootIndexTemplate);
                    }
                    break;
                default:
                    throw new InvalidOperationException(Invariant($"Enum value {rootIndexTemplate.ButtonsNeeded} is not valid for the first blog post"));
            }

            rootIndexTemplate.SaveAsRootIndex();

            #endregion

            // Now create the rest of the blog pages
            for (int i = 0; i < populatedTemplates.Count; i++)
            {
                PopulatedTemplate currentTemplate = populatedTemplates[i];

                switch (currentTemplate.ButtonsNeeded)
                {
                    case NavigationButtons.None:
                        {
                            hideNext(currentTemplate);
                            hidePrevious(currentTemplate);
                        }
                        break;
                    case NavigationButtons.NextOnly:
                        {
                            PopulatedTemplate nextTemplate = populatedTemplates[i - 1];
                            replaceNext(currentTemplate, nextTemplate.RelativePath);
                            hidePrevious(currentTemplate);
                        }
                        break;
                    case NavigationButtons.PreviousOnly:
                        {
                            PopulatedTemplate previousTemplate = populatedTemplates[i + 1];
                            replacePrevious(currentTemplate, previousTemplate.RelativePath);
                            hideNext(currentTemplate);
                        }
                        break;
                    case NavigationButtons.Both:
                        {
                            PopulatedTemplate previousTemplate = populatedTemplates[i + 1];
                            replacePrevious(currentTemplate, previousTemplate.RelativePath);

                            PopulatedTemplate nextTemplate = populatedTemplates[i - 1];
                            replaceNext(currentTemplate, nextTemplate.RelativePath);
                        }
                        break;
                    default:
                        throw new InvalidOperationException(Invariant($"Enum value {currentTemplate.ButtonsNeeded} not supported"));
                }

                currentTemplate.Save();
            }
        }

        private PopulatedTemplate createRootIndexPopulatedTemplate(IBlogPost blogPost, int posts)
        {
            try
            {
                HtmlDocument template = CopyOfTemplate;

                addBlogCSS(template, blogPost, LayoutType.MainPage);
                replaceBlogDiv(template, blogPost);
                replaceAllTitles(template, blogPost);
                replaceAllDates(template, blogPost);
                replaceAllFragments(template, LayoutType.MainPage);

                NavigationButtons buttonsNeeded;

                // Only two possible cases:
                //      1. There is only one blog post
                //      2. This is the first blog post of many
                // The first blog post should not need next or both navigation buttons.
                if (posts == 1)
                {
                    buttonsNeeded = NavigationButtons.None;
                }
                else
                {
                    buttonsNeeded = NavigationButtons.PreviousOnly;
                }

                return new PopulatedTemplate(blogPost, template, buttonsNeeded, OptionsContext.Current.Options.OutputDirectory);
            }
            catch (Exception e)
            {
                ErrorWriterContext.Current.WriteLine(Invariant($"Error creating blog post with title {blogPost.Metadata.Title}."));
                ErrorWriterContext.Current.WriteLine(e.ToString());
                throw e;
            }
        }

        private void replaceAllFragments(HtmlDocument template, LayoutType layoutType = LayoutType.Post)
        {
            foreach (string fragmentID in _composedFragments.Keys)
            {
                string fragmentXPath = "//*[@id='" + fragmentID + "']";
                HtmlNodeCollection fragmentNodes = template.DocumentNode.SelectNodes(fragmentXPath);

                if (null == fragmentNodes)
                {
                    continue;
                }

                Func<LayoutType,HtmlDocument> composedFragmentFunc = _composedFragments[fragmentID];
                foreach (HtmlNode fragmentNode in fragmentNodes)
                {
                    fragmentNode.InnerHtml = composedFragmentFunc(layoutType).DocumentNode.InnerHtml;
                }
            }
        }

        private void addBlogCSS(HtmlDocument template, IBlogPost blogPost, LayoutType layoutType = LayoutType.Post)
        {
            string cssLink = "../../css".AppendPath(OptionsContext.Current.Options.BlogCSSFile);

            // If we're doing the main page, we don't need to go up any directories
            if (layoutType == LayoutType.MainPage)
            {
                cssLink = "css".AppendPath(OptionsContext.Current.Options.BlogCSSFile);
            }

            string linkNodeString = Invariant($"<link rel=\"stylesheet\" type=\"text/css\" href=\"{cssLink}\" />");
            HtmlNode headNode = template.DocumentNode.SelectSingleNode(HeadXPath);
            headNode.InnerHtml = (headNode.InnerHtml + linkNodeString);
        }

        private static void replaceBlogDiv(HtmlDocument template, IBlogPost blogPost)
        {
            HtmlNode blogDiv = template.DocumentNode.SelectSingleNode(BlogXPath);
            blogDiv.InnerHtml = blogPost.HTML;
        }

        private static void replaceAllTitles(HtmlDocument template, IBlogPost blogPost)
        {
            HtmlNodeCollection titles = template.DocumentNode.SelectNodes(TitleXPath);

            foreach (HtmlNode title in titles)
            {
                title.InnerHtml = blogPost.Metadata.Title;
            }
        }

        private static void replaceAllDates(HtmlDocument template, IBlogPost blogPost)
        {
            HtmlNodeCollection dates = template.DocumentNode.SelectNodes(DateXPath);

            string dateString = blogPost.Metadata.Date.ToString();
            foreach (HtmlNode date in dates)
            {
                date.InnerHtml = dateString;
            }
        }

        private static void replacePrevious(PopulatedTemplate template, string relativeUri)
        {
            HtmlDocument populatedDocument = template.PopulatedDocument;

            HtmlNode previous = populatedDocument.DocumentNode.SelectSingleNode(PreviousPostXPath);

            previous.Attributes.Add(HREF, relativeUri);
        }

        private static void hidePrevious(PopulatedTemplate template)
        {
            HtmlDocument populatedDocument = template.PopulatedDocument;

            HtmlNode previous = populatedDocument.DocumentNode.SelectSingleNode(PreviousPostXPath);

            previous.Remove();
        }

        private static void replaceNext(PopulatedTemplate template, string relativeUri)
        {
            HtmlDocument populatedDocument = template.PopulatedDocument;

            HtmlNode next = populatedDocument.DocumentNode.SelectSingleNode(NextPostXPath);

            next.Attributes.Add(HREF, relativeUri);
        }

        private static void hideNext(PopulatedTemplate template)
        {
            HtmlDocument populatedDocument = template.PopulatedDocument;

            HtmlNode next = populatedDocument.DocumentNode.SelectSingleNode(NextPostXPath);

            next.Remove();
        }

        private class PopulatedTemplate
        {
            private const string FILE_NAME = "index.html";
            private readonly string _outputDirectory;

            public PopulatedTemplate(IBlogPost blogPost, HtmlDocument populatedDocument, NavigationButtons buttonsNeeded, string outputDirectory)
            {
                BlogPost = blogPost;
                PopulatedDocument = populatedDocument;
                ButtonsNeeded = buttonsNeeded;
                _outputDirectory = outputDirectory;
            }

            public IBlogPost BlogPost
            {
                get;
            }

            public HtmlDocument PopulatedDocument
            {
                get;
            }

            public NavigationButtons ButtonsNeeded
            {
                get;
            }

            public string RelativePath
            {
                get
                {
                    return "..".AppendPath(FolderName, FILE_NAME);
                }
            }

            public string RootRelativePath
            {
                get
                {
                    return ".".AppendPath("blog", FolderName, FILE_NAME);
                }
            }

            public string FolderName
            {
                get
                {
                    // TODO: length limit?
                    return getFolderName(BlogPost.Metadata.Title).ToLower();
                }
            }

            public void Save()
            {
                PopulatedDocument.Save(FullSavePath);
            }

            public void SaveAsRootIndex()
            {
                PopulatedDocument.Save(RootFullSavePath);
            }

            private string RootFullSavePath
            {
                get
                {
                    return Path.Combine(_outputDirectory, FILE_NAME);
                }
            }

            private string FullSavePath
            {
                get
                {
                    // TODO: do not hard code "blog"
                    string folderToCreate = Path.Combine(_outputDirectory, "blog", FolderName);
                    string folderToCreateFullPath = Path.GetFullPath(folderToCreate);

                    IOContext.Current.CreateDirectory(folderToCreate);

                    return Path.Combine(folderToCreateFullPath, FILE_NAME);
                }
            }

            // TODO: Reconcile this with all other copied functions
            private static string getFolderName(string path)
            {
                return Regex.Replace(path, @"[!@#$%^&*()\[\]\\\/:;'"".,?=+{}|_~`<>]", string.Empty).Replace(" ", "-");
            }
        }

        private enum NavigationButtons
        {
            None,
            NextOnly,
            PreviousOnly,
            Both
        }
    }
}
