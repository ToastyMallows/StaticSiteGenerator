using System;
using System.Globalization;

namespace StaticSiteGenerator
{
    public static class Guard
    {
        public static void VerifyArgumentNotNull( object obj, string objName )
        {
            if ( null == obj )
            {
                throw new ArgumentException( string.Format( CultureInfo.InvariantCulture, "Argument {0} was null", objName ) );
            }
        }
    }
}
