using Moq;
using NUnit.Framework;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Providers;
using System.Collections.Generic;
using System.IO;

namespace StaticSiteGeneratorTest.ProviderTests
{
    [TestFixture]
    public sealed class BlogPostProviderTest
    {
        private static TestErrorWriter testErrorWriter;

        [OneTimeSetUp]
        public void ProgramTest_Setup()
        {
            TestErrorWriter.SetCurrentErrorWriterContext( out testErrorWriter );
        }

        [Test]
        [Description( "Tests that TryGetBlogPostsDescending returns false and null blog posts if posts directory does not exist" )]
        public void BlogPostProvider_TryGetBlogPostsDescending_NoPostsDirectory()
        {
            IEnumerable<IBlogPost> expectedBlogPosts;

            IIOContext testIOContext = createAndSetCurrentIOContext( currentDirectory: Constants.CurrentDirectory, exists: false );

            BlogPostProvider blogPostProvider = new BlogPostProvider();

            Assert.AreEqual( false, blogPostProvider.TryGetBlogPostsDescending( out expectedBlogPosts ) );
            Assert.IsNull( expectedBlogPosts );
            Assert.AreEqual( "No posts folder", testErrorWriter.LastErrorString );
        }

        [Test]
        [Description( "Tests that TryGetBlogPostsDescending returns false and null blog posts if there are not metadata files found in the current directory" )]
        public void BlogPostProvider_TryGetBlogPostsDescending_NoJSONFiles()
        {
            IEnumerable<IBlogPost> expectedBlogPosts;

            IIOContext testIOContext = createAndSetCurrentIOContext( currentDirectory: Constants.CurrentDirectory, exists: true, files: Constants.NoFiles );

            BlogPostProvider blogPostProvider = new BlogPostProvider();

            Assert.AreEqual( false, blogPostProvider.TryGetBlogPostsDescending( out expectedBlogPosts ) );
            Assert.IsNull( expectedBlogPosts );
            Assert.AreEqual( "No metadata files", testErrorWriter.LastErrorString );
        }

        private IIOContext createAndSetCurrentIOContext(
            bool? exists = null,
            string currentDirectory = null,
            string[] files = null,
            string readAllText = null )
        {
            bool existsWasSet = exists.HasValue;
            bool currentDirectoryWasSet = currentDirectory != null;
            bool filesWasSet = files != null;
            bool readAllTextWasSet = readAllText != null;

            Mock<IIOContext> mockIOContext = new Mock<IIOContext>( MockBehavior.Strict );

            if ( existsWasSet )
            {
                mockIOContext
                    .Setup( m => m.DirectoryExists( It.IsAny<string>() ) )
                        .Returns( exists.Value );
            }

            if ( currentDirectoryWasSet )
            {
                mockIOContext
                    .Setup( m => m.GetCurrentDirectory() )
                        .Returns( currentDirectory );
            }

            if ( filesWasSet )
            {
                mockIOContext
                    .Setup( m => m.GetFilesFromDirectory( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>() ) )
                        .Returns( files );
            }

            if ( readAllTextWasSet )
            {
                mockIOContext
                    .Setup( m => m.ReadAllTextFromFile( It.IsAny<string>() ) )
                        .Returns( readAllText );
            }

            IIOContext testIOContext = mockIOContext.Object;
            IOContext.Current = testIOContext;
            return testIOContext;
        }
    }
}
