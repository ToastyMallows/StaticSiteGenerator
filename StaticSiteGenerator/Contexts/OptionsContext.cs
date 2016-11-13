using StaticSiteGenerator.Models;
using System;

namespace StaticSiteGenerator.Contexts
{
    public abstract class OptionsContext : IOptionsContext
    {
        private static IOptionsContext _current;

        public static IOptionsContext Current
        {
            get
            {
                if ( null == _current )
                {
                    _current = DefaultIOptionsContext;
                }

                return _current;
            }
            set
            {
                IOptionsContext iOptionsContext = value as IOptionsContext;
                Guard.VerifyArgumentNotNull( iOptionsContext, "IOptionsContextSetValue" );
                _current = iOptionsContext;
            }
        }

        public abstract IOptions Options { get; }

        public static IOptionsContext DefaultIOptionsContext = new DefaultOptionsContext();

        private class DefaultOptionsContext : IOptionsContext
        {
            public IOptions Options
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }

    public class ProgramOptionsContext : IOptionsContext
    {
        private readonly IOptions _options;

        public ProgramOptionsContext( IOptions options )
        {
            Guard.VerifyArgumentNotNull( options, nameof( options ) );

            _options = options;
        }

        public IOptions Options
        {
            get
            {
                return _options;
            }
        }
    }

    public interface IOptionsContext
    {
        IOptions Options { get; }
    }
}
