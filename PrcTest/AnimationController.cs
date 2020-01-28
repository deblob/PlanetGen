using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PrcTest
{
    public class AnimationController
    {
        private class _animInfoInt
        {
            public int From { get; set; }
            public int To { get; set; }
        }

        private class _animInfoFloat
        {
            public float From { get; set; }
            public float To { get; set; }
        }

        private readonly PlanetGenerator _generator;
        private readonly Dictionary<FieldInfo, _animInfoInt> _intAnims = new Dictionary<FieldInfo, _animInfoInt>();
        private readonly Dictionary<FieldInfo, _animInfoFloat> _floatAnims = new Dictionary<FieldInfo, _animInfoFloat>();

        public int TotalFrames { get; set; } = 100;

        public AnimationController( PlanetGenerator generator )
        {
            _generator = generator;
        }

        public void AddAnimation( FieldInfo fieldInfo, int from, int to )
            => _intAnims.Add( fieldInfo, new _animInfoInt() { From = from, To = to } );
        public void AddAnimation( FieldInfo fieldInfo, float from, float to )
            => _floatAnims.Add( fieldInfo, new _animInfoFloat { From = from, To = to } );

        public void AnimateTest( string folderName )
        {
            for ( int frame = 0; frame < TotalFrames; frame++ )
            {
                foreach ( FieldInfo fieldInfo in _intAnims.Keys )
                    fieldInfo.SetValue( _generator, ( int )( lerp( _intAnims[ fieldInfo ].From, _intAnims[ fieldInfo ].To, ( float )frame / TotalFrames ) + .5f ) );
                foreach ( FieldInfo fieldInfo in _floatAnims.Keys )
                    fieldInfo.SetValue( _generator, lerp( _floatAnims[ fieldInfo ].From, _floatAnims[ fieldInfo ].To, ( float )frame / TotalFrames ) );

                string fileName = Path.Combine(Environment.CurrentDirectory, folderName, "frame_" + frame.ToString().PadLeft(4, '0'));
                if ( !Directory.Exists( Path.Combine( Environment.CurrentDirectory, folderName ) ) )
                    Directory.CreateDirectory( Path.Combine( Environment.CurrentDirectory, folderName ) );

                _generator.GeneratePlanet( fileName );
            }
        }

        private float lerp( float x, float y, float value ) => x + ( y - x ) * value;
    }
}
