using HtmlAgilityPack;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using static System.FormattableString;

namespace StaticSiteGenerator.Providers
{
    // TODO: Rename?
    internal sealed class TemplateProvider : ITemplateProvider
    {
        private readonly IOptions _options;
        private readonly string _fullPathToTemplate;

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
                            buttonsNeeded = NavigationButtons.NextOnly;
                        }
                        else if ( index == posts )
                        {
                            buttonsNeeded = NavigationButtons.PreviousOnly;
                        }
                        else
                        {
                            buttonsNeeded = NavigationButtons.Both;
                        }
                    }

                    populatedTemplates.Add( new PopulatedTemplate( blogPost, template, buttonsNeeded ) );
                }
                catch ( Exception e )
                {
                    ErrorWriterContext.Current.WriteLine( Invariant( $"Error creating blog post with title {blogPost.Metadata.Title}." ) );
                    ErrorWriterContext.Current.WriteLine( e.ToString() );
                    return false;
                }

                index++;
            }

            foreach ( PopulatedTemplate populatedTemplate in populatedTemplates )
            {
                // Do work for next and previous buttons

                SavePopulatedTemplate( populatedTemplate );
            }

            return true;
        }

        private static void replaceBlogDiv( HtmlDocument template, IBlogPost blogPost )
        {
            HtmlNode blogDiv = template.DocumentNode.SelectSingleNode( "//div[contains(@id,'" + Constants.BlogID + "')]" );
            blogDiv.InnerHtml = blogPost.PostHTML;
        }

        private static void replaceAllTitles( HtmlDocument template, IBlogPost blogPost )
        {
            HtmlNodeCollection titles = template.DocumentNode.SelectNodes( "//*[contains(@class,'" + Constants.TitleClass + "')]" );

            foreach ( HtmlNode title in titles )
            {
                title.InnerHtml = blogPost.Metadata.Title;
            }
        }

        private static void replaceAllDates( HtmlDocument template, IBlogPost blogPost )
        {
            HtmlNodeCollection dates = template.DocumentNode.SelectNodes( "//*[contains(@class,'" + Constants.DateClass + "')]" );

            string dateString = blogPost.Metadata.Date.ToString();
            foreach ( HtmlNode date in dates )
            {
                date.InnerHtml = dateString;
            }
        }

        private static void hidePrevious( HtmlDocument template )
        {
            HtmlNode previous = template.DocumentNode.SelectSingleNode( "//*[contains(@id,'" + Constants.PreviousID + "')]" );

            previous.Remove();
        }

        private static void hideNext( HtmlDocument template )
        {
            HtmlNode previous = template.DocumentNode.SelectSingleNode( "//*[contains(@id,'" + Constants.NextID + "')]" );

            previous.Remove();
        }

        private string getFullPathToTemplate()
        {
            return Path.GetFullPath( _options.TemplateFile );
        }

        private void ensureTemplateHtmlIsValid()
        {
            ( new HtmlDocument() ).Load( _fullPathToTemplate );
        }

        private void SavePopulatedTemplate( PopulatedTemplate populatedTemplate )
        {
            string folderToCreate = Path.Combine( _options.OutputDirectory, populatedTemplate.FolderName );
            string folderToCreateFullPath = Path.GetFullPath( folderToCreate );

            IOContext.Current.CreateDirectory( folderToCreate );

            populatedTemplate.Save( Path.Combine( folderToCreateFullPath, "index.html" ) );
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
            public PopulatedTemplate( IBlogPost blogPost, HtmlDocument populatedDocument, NavigationButtons buttonsNeeded )
            {
                BlogPost = blogPost;
                PopulatedDocument = populatedDocument;
                ButtonsNeeded = buttonsNeeded;
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

            public string FolderName
            {
                get
                {
                    // TODO: length limit?
                    return BlogPost.Metadata.Title.Replace( ' ', '-' ).ToLower();
                }
            }

            public void Save( string path )
            {
                PopulatedDocument.Save( path );
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
