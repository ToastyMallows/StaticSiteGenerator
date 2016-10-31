using HtmlAgilityPack;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static System.FormattableString;

namespace StaticSiteGenerator.Providers
{
    // TODO: Rename?
    internal sealed class TemplateProvider : ITemplateProvider
    {
        private readonly IOptions _options;
        private readonly string _fullPathToTemplate;

        private const string NextPostXPath = "//a[@id='" + Constants.NextID + "']";
        private const string PreviousPostXPath = "//a[@id='" + Constants.PreviousID + "']";
        private const string BlogXPath = "//div[@id='" + Constants.BlogID + "']";
        private const string TitleXPath = "//*[contains(@class,'" + Constants.TitleClass + "')]";
        private const string DateXPath = "//*[contains(@class,'" + Constants.DateClass + "')]";

        private const string HREF = "href";

        public TemplateProvider( IOptions options )
        {
            Guard.VerifyArgumentNotNull( options, nameof( options ) );

            _options = options;
            _fullPathToTemplate = getFullPathToTemplate();

            ensureTemplateHtmlIsValid();
        }

        private HtmlDocument CopyOfTemplate
        {
            get
            {
                HtmlDocument template = new HtmlDocument();
                template.Load( _fullPathToTemplate );
                return template;
            }
        }

        public bool TryMakeBlogPosts( IReadOnlyCollection<IBlogPost> blogPosts )
        {
            int index = 1;
            int posts = blogPosts.Count;

            IList<PopulatedTemplate> populatedTemplates = new List<PopulatedTemplate>();

            foreach ( IBlogPost blogPost in blogPosts )
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

                    populatedTemplates.Add( new PopulatedTemplate( blogPost, template, buttonsNeeded, _options.OutputDirectory ) );
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
            blogDiv.InnerHtml = blogPost.PostHTML;
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

        private string getFullPathToTemplate()
        {
            return Path.GetFullPath( _options.TemplateFile );
        }

        private void ensureTemplateHtmlIsValid()
        {
            ( new HtmlDocument() ).Load( _fullPathToTemplate );
        }

        private class Constants
        {
            public const string BlogID = "SSGBlog";
            public const string TitleClass = "SSGTitle";
            public const string DateClass = "SSGDate";
            public const string NextID = "SSGNextPost";
            public const string PreviousID = "SSGPreviousPost";
        }

        private struct PopulatedTemplate
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
