using HtmlAgilityPack;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static System.FormattableString;

namespace StaticSiteGenerator.Composers
{
    internal class BlogTemplateComposer : TemplateComposer
    {
        //private readonly IHeaderProvider _headerProvider;
        //private readonly IFooterProvider _footerProvider;

        private const string NextPostXPath = "//a[@id='" + Constants.NextID + "']";
        private const string PreviousPostXPath = "//a[@id='" + Constants.PreviousID + "']";
        private const string BlogXPath = "//div[@id='" + Constants.BlogID + "']";
        private const string TitleXPath = "//*[contains(@class,'" + Constants.TitleClass + "')]";
        private const string DateXPath = "//*[contains(@class,'" + Constants.DateClass + "')]";
        private const string HREF = "href";

        public BlogTemplateComposer( /*IHeaderProvider headerProvider, IFooterProvider footerProvider*/ )
        {
            //Guard.VerifyArgumentNotNull( headerProvider, nameof( headerProvider ) );
            //Guard.VerifyArgumentNotNull( footerProvider, nameof( footerProvider ) );

            //_headerProvider = headerProvider;
            //_footerProvider = footerProvider;

            ensureTemplateHtmlIsValid();
        }

        protected override string TemplateFile
        {
            get
            {
                return OptionsContext.Current.Options.BlogTemplateFile;
            }
        }

        public override bool TryCompose( IReadOnlyCollection<IBasePage> basePageData )
        {
            int index = 1;
            int posts = basePageData.Count;

            IList<PopulatedTemplate> populatedTemplates = new List<PopulatedTemplate>();

            foreach ( IBlogPost blogPost in basePageData )
            {
                try
                {
                    HtmlDocument template = CopyOfTemplate;

                    replaceBlogDiv( template, blogPost );
                    replaceAllTitles( template, blogPost );
                    replaceAllDates( template, blogPost );

                    NavigationButtons buttonsNeeded;

                    if ( posts == 1 )
                    {
                        buttonsNeeded = NavigationButtons.None;
                    }
                    else
                    {
                        if ( index == 1 )
                        {
                            buttonsNeeded = NavigationButtons.PreviousOnly;
                        }
                        else if ( index == posts )
                        {
                            buttonsNeeded = NavigationButtons.NextOnly;
                        }
                        else
                        {
                            buttonsNeeded = NavigationButtons.Both;
                        }
                    }

                    populatedTemplates.Add( new PopulatedTemplate( blogPost, template, buttonsNeeded, OptionsContext.Current.Options.OutputDirectory ) );
                }
                catch ( Exception e )
                {
                    ErrorWriterContext.Current.WriteLine( Invariant( $"Error creating blog post with title {blogPost.Metadata.Title}." ) );
                    ErrorWriterContext.Current.WriteLine( e.ToString() );
                    return false;
                }

                index++;
            }

            for ( int i = 0; i < populatedTemplates.Count; i++ )
            {
                PopulatedTemplate currentTemplate = populatedTemplates[i];

                switch ( currentTemplate.ButtonsNeeded )
                {
                    case NavigationButtons.None:
                        hideNext( currentTemplate );
                        hidePrevious( currentTemplate );
                        break;
                    case NavigationButtons.NextOnly:
                        {
                            PopulatedTemplate nextTemplate = populatedTemplates[i - 1];
                            replaceNext( currentTemplate, nextTemplate.RelativePath );
                            hidePrevious( currentTemplate );
                        }
                        break;
                    case NavigationButtons.PreviousOnly:
                        {
                            PopulatedTemplate previousTemplate = populatedTemplates[i + 1];
                            replacePrevious( currentTemplate, previousTemplate.RelativePath );
                            hideNext( currentTemplate );
                        }
                        break;
                    case NavigationButtons.Both:
                        {
                            PopulatedTemplate previousTemplate = populatedTemplates[i + 1];
                            replacePrevious( currentTemplate, previousTemplate.RelativePath );

                            PopulatedTemplate nextTemplate = populatedTemplates[i - 1];
                            replaceNext( currentTemplate, nextTemplate.RelativePath );
                        }
                        break;
                    default:
                        throw new InvalidOperationException( Invariant( $"Enum value {currentTemplate.ButtonsNeeded} not supported" ) );
                }

                currentTemplate.Save();
            }

            return true;
        }

        private static void replaceBlogDiv( HtmlDocument template, IBlogPost blogPost )
        {
            HtmlNode blogDiv = template.DocumentNode.SelectSingleNode( BlogXPath );
            blogDiv.InnerHtml = blogPost.HTML;
        }

        private static void replaceAllTitles( HtmlDocument template, IBlogPost blogPost )
        {
            HtmlNodeCollection titles = template.DocumentNode.SelectNodes( TitleXPath );

            foreach ( HtmlNode title in titles )
            {
                title.InnerHtml = blogPost.Metadata.Title;
            }
        }

        private static void replaceAllDates( HtmlDocument template, IBlogPost blogPost )
        {
            HtmlNodeCollection dates = template.DocumentNode.SelectNodes( DateXPath );

            string dateString = blogPost.Metadata.Date.ToString();
            foreach ( HtmlNode date in dates )
            {
                date.InnerHtml = dateString;
            }
        }

        private static void replacePrevious( PopulatedTemplate template, string relativeUri )
        {
            HtmlDocument populatedDocument = template.PopulatedDocument;

            HtmlNode previous = populatedDocument.DocumentNode.SelectSingleNode( PreviousPostXPath );

            previous.Attributes.Add( HREF, relativeUri );
        }

        private static void hidePrevious( PopulatedTemplate template )
        {
            HtmlDocument populatedDocument = template.PopulatedDocument;

            HtmlNode previous = populatedDocument.DocumentNode.SelectSingleNode( PreviousPostXPath );

            previous.Remove();
        }

        private static void replaceNext( PopulatedTemplate template, string relativeUri )
        {
            HtmlDocument populatedDocument = template.PopulatedDocument;

            HtmlNode next = populatedDocument.DocumentNode.SelectSingleNode( NextPostXPath );

            next.Attributes.Add( HREF, relativeUri );
        }

        private static void hideNext( PopulatedTemplate template )
        {
            HtmlDocument populatedDocument = template.PopulatedDocument;

            HtmlNode next = populatedDocument.DocumentNode.SelectSingleNode( NextPostXPath );

            next.Remove();
        }

        private class PopulatedTemplate
        {
            private const string FILE_NAME = "index.html";
            private readonly string _outputDirectory;

            public PopulatedTemplate( IBlogPost blogPost, HtmlDocument populatedDocument, NavigationButtons buttonsNeeded, string outputDirectory )
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
                    return Path.Combine( "..", FolderName, FILE_NAME );
                }
            }

            public string FolderName
            {
                get
                {
                    // TODO: length limit?
                    return getFolderName( BlogPost.Metadata.Title ).ToLower();
                }
            }

            public void Save()
            {
                PopulatedDocument.Save( FullPath );
            }

            private string FullPath
            {
                get
                {
                    string folderToCreate = Path.Combine( _outputDirectory, FolderName );
                    string folderToCreateFullPath = Path.GetFullPath( folderToCreate );

                    IOContext.Current.CreateDirectory( folderToCreate );

                    return Path.Combine( folderToCreateFullPath, FILE_NAME );
                }
            }

            // TODO: Reconcile this with base class implementation
            private static string getFolderName( string path )
            {
                return Regex.Replace( path, @"[!@#$%^&*()\[\]\\\/:;'"".,?=+{}|_~`<>]", string.Empty ).Replace( " ", "-" );
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
