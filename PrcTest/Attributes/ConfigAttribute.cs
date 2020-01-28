using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrcTest.Attributes
{
    public class ConfigAttribute : Attribute
    {
        public string Description { get; set; }
        public int IntFrom { get; set; }
        public int IntTo { get; set; }
        public float FloatFrom { get; set; }
        public float FloatTo { get; set; }

        public ConfigAttribute( string description, int intFrom, int intTo, float floatFrom, float floatTo )
        {
            Description = description;
            IntFrom = intFrom;
            IntTo = intTo;
            FloatFrom = floatFrom;
            FloatTo = floatTo;
        }

        public ConfigAttribute()
            : this( String.Empty, Int32.MinValue, Int32.MaxValue, Single.MinValue, Single.MaxValue )
        { }

        public ConfigAttribute( string description )
            : this( description, Int32.MinValue, Int32.MaxValue, Single.MinValue, Single.MaxValue )
        { }

        public ConfigAttribute( int from, int to )
            : this( String.Empty, from, to, Single.MinValue, Single.MaxValue )
        { }

        public ConfigAttribute( string description, int from, int to )
            : this( description, from, to, Single.MinValue, Single.MaxValue )
        { }

        public ConfigAttribute( float from, float to )
            : this( String.Empty, Int32.MinValue, Int32.MaxValue, from, to )
        { }

        public ConfigAttribute( string description, float from, float to )
            : this( description, Int32.MinValue, Int32.MaxValue, from, to )
        { }
    }
}
