using System;
using System.ComponentModel;
using System.Reflection;

namespace StaticSiteGenerator
{
    public static class ExtensionMethods
    {
        public static string GetDescription( this Enum value )
        {
            Type type = value.GetType();
            string name = Enum.GetName( type, value );
            if ( null == name )
            {
                return null;
            }

            FieldInfo field = type.GetField( name );
            if ( null == field )
            {
                return null;
            }

            DescriptionAttribute attribute = Attribute.GetCustomAttribute( field, typeof( DescriptionAttribute ) ) as DescriptionAttribute;

            if ( null == attribute )
            {
                return null;
            }

            return attribute.Description;
        }
    }
}
