using PrcTest.Attributes;
using PrcTest.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using static FastNoise;

namespace PrcTest
{
    class Program
    {
        [STAThread]
        static void Main( string[] args )
        {
            PlanetGenerator gen = new PlanetGenerator();

            bool nogui = false;
            string outputName = String.Empty;
            XElement presetRoot = null;
            foreach ( string arg in args )
            {
                if ( arg.ToLower().Equals( "--help" ) || arg.ToLower().Equals( "-h" ) )
                {
                    Console.WriteLine( $"\nPlanetGenerator {FileVersionInfo.GetVersionInfo( Assembly.GetEntryAssembly().Location ).FileVersion} by Marc D." );
                    Console.WriteLine( "This highly configurable tool will generate procedural planet surfaces. Please keep in mind that this is unfinished software." );
                    Console.WriteLine( "\n" + Properties.Resources.HelpText.Replace( "\\n", Environment.NewLine ).Replace( "\\t", "\t" ) + "\n" );
                    Console.WriteLine( "Parameters:" );

                    foreach ( var fieldInfo in gen.GetType().GetFields().Where( f => f.Attributes.HasFlag( FieldAttributes.Public )
                        && f.CustomAttributes.Any( a => a.AttributeType == typeof( ConfigAttribute ) ) ).OrderBy( n => n.Name ) )
                    {
                        string fieldName = fieldInfo.Name;
                        string fieldDescription = fieldInfo.GetCustomAttribute<ConfigAttribute>().Description;
                        string fieldType = "null";
                        if ( fieldInfo.FieldType == typeof( Int32 ) )
                        {
                            fieldType = "Int32";
                        }
                        else if ( fieldInfo.FieldType == typeof( Boolean ) )
                        {
                            fieldType = "Boolean";
                        }
                        else if ( fieldInfo.FieldType == typeof( Single ) )
                        {
                            fieldType = "Float";
                        }
                        else if ( fieldInfo.FieldType == typeof( NoiseType ) )
                        {
                            fieldType = "EnumNoiseType";
                        }

                        Console.WriteLine( $"{fieldName} ({fieldType}): {fieldDescription}" );
                    }

                    Environment.Exit( 1 );
                }

                if ( arg.ToLower().Equals( "--generate" ) || arg.ToLower().Equals( "-g" ) )
                {
                    nogui = true;
                    continue;
                }
                if ( arg.ToLower().StartsWith( "--output" ) || arg.ToLower().StartsWith( "-o" ) )
                {
                    outputName = arg.Split( ':' )[ 1 ];
                    continue;
                }
                if ( arg.ToLower().StartsWith( "--preset" ) || arg.ToLower().StartsWith( "-p" ) )
                {
                    presetRoot = XElement.Load( arg.Split( ':' )[ 1 ] );
                    continue;
                }

                var split = arg.Split(':');
                string name = split[0].TrimStart('-', '+');
                string value = split[1];

                Type t = gen.GetType();
                var field = t.GetField(name);
                if ( field == null )
                    continue;
                if ( field.FieldType == typeof( int ) )
                    field.SetValue( gen, Int32.Parse( value ) );
                else if ( field.FieldType == typeof( float ) )
                    field.SetValue( gen, Single.Parse( value ) );
                else if ( field.FieldType == typeof( bool ) )
                    field.SetValue( gen, Boolean.Parse( value ) );
                else if ( field.FieldType == typeof( FastNoise.NoiseType ) )
                    field.SetValue( gen, Enum.Parse( typeof( FastNoise.NoiseType ), value ) );
            }

            if ( presetRoot != null )
                gen.LoadSettings( presetRoot );

            if ( !nogui )
            {
                frmConfig cfg = new frmConfig(gen);
                cfg.ShowDialog();
            }
            else
            {
                if ( String.IsNullOrEmpty( outputName ) || String.IsNullOrWhiteSpace( outputName ) )
                    gen.GeneratePlanet( Path.Combine( Environment.CurrentDirectory, "result" ) );
                else
                    gen.GeneratePlanet( Path.Combine( Environment.CurrentDirectory, outputName ) );
            }
        }
    }
}
