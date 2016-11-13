using StaticSiteGenerator.Composers;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using StaticSiteGenerator.Providers;
using System.Collections.Generic;

namespace StaticSiteGenerator.Generators
{
    internal class BasePageGenerator : IGenerator
    {
        private readonly IBasePageProvider _basePageProvider;
        private readonly ITemplateComposer _templateComposer;

        public BasePageGenerator( IBasePageProvider basePageProvider, ITemplateComposer templateComposer )
        {
            Guard.VerifyArgumentNotNull( basePageProvider, nameof( basePageProvider ) );
            Guard.VerifyArgumentNotNull( templateComposer, nameof( templateComposer ) );

            _basePageProvider = basePageProvider;
            _templateComposer = templateComposer;
        }

        public bool TryGenerate()
        {
            IReadOnlyCollection<IBasePage> basePages;
            bool basePageProcessingSuccess = _basePageProvider.TryParseMetadataFiles( out basePages );

            if ( !basePageProcessingSuccess )
            {
                ErrorWriterContext.Current.WriteLine( "Error processing metadata files" );
                return false;
            }

            bool blogPostsCreated = _templateComposer.TryCompose( basePages );
            if ( !blogPostsCreated )
            {
                ErrorWriterContext.Current.WriteLine( "Error composing files" );
                return false;
            }

            return true;
        }
    }
}
