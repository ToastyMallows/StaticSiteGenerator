using NUnit.Framework;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Providers;
using System.Collections.Generic;

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

            TestDirectoryProvider testDirectoryProvider;
            TestDirectoryProvider.SetCurrentDirectorProviderContext( out testDirectoryProvider );

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

            TestDirectoryProvider testDirectoryProvider;
            TestDirectoryProvider.SetCurrentDirectorProviderContext( out testDirectoryProvider, exists: Constants.DirectoryExists );

            BlogPostProvider blogPostProvider = new BlogPostProvider();

            Assert.AreEqual( false, blogPostProvider.TryGetBlogPostsDescending( out expectedBlogPosts ) );
            Assert.IsNull( expectedBlogPosts );
            Assert.AreEqual( "No metadata files", testErrorWriter.LastErrorString );
        }
    }
}
