//#define ENABLE_DEBUG_LOOPS

using PrcTest.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static FastNoise;

namespace PrcTest
{
    public class PlanetGenerator
    {
        [Config("Width of the produced image.", 126, Int32.MaxValue)]
        public int ALL_RES_X = 2048;
        [Config("Height of the produced image.", 126, Int32.MaxValue)]
        public int ALL_RES_Y = 1024;
        [Config("Number that acts as the seed for the random number generator. Same numbers will always produce same results. -1 for 'random' seed.")]
        public int ALL_RANDOM_SEED = -1;

        [Config("Determines how much lighter mountains get the closer they are to the camera.")]
        public int MOUNTAINS_BRIGHTNESS_INCREASE = 4;
        [Config()]
        public bool MOUNTAINS_LIGHT_TO_DARK = true;
        [Config("Determines how dark the randomly generated colors for mountains can be.", 0, 254)]
        public int MOUNTAINS_RANDOM_COLOR_MIN_VALUE = 100;
        [Config("Determines how light the randomly generated colors for mountains can be.", 1, 255)]
        public int MOUNTAINS_RANDOM_COLOR_MAX_VALUE = 150;
        [Config("Determines the distance between different layers of mountains.", 1, 80)]
        public int MOUNTAINS_PARALLAX_OFFSET = 6;
        [Config("Determines how many layers of mountains there are.", 1, 99)]
        public int MOUNTAINS_PARALLAX_SLICES = 30;
        [Config("Make mountain layers closer to the camera lower?")]
        public bool MOUNTAINS_PARALLAX_GRADIENT = true;
        [Config("How much lower should mountains closer to the camera be? Lower value = lower mountains. (Only has an effect when 'MOUNTAINS_PARALLAX_GRADIENT' is true)", .001f, 1f)]
        public float MOUNTAINS_PARALLAX_GRADIENT_SCALE = .35f;
        [Config("General modifier for the height of mountains. Lower value = lower mountains.", 0f, 1f)]
        public float MOUNTAINS_HEIGHT_SCALE = 1f;
        [Config("The noise algorithm used to generate the mountains.")]
        public NoiseType MOUNTAINS_NOISE_TYPE = NoiseType.CubicFractal;
        [Config("Frequency of the noise texture used to generate mountains.", .001f, 3f)]
        public float MOUNTAINS_NOISE_FREQUENCY = .01f;

        [Config("Determines how dark the sky can be.", 0, 254)]
        public int SKY_RANDOM_COLOR_MIN_VALUE = 120;
        [Config("Determines how light the sky can be.", 1, 255)]
        public int SKY_RANDOM_COLOR_MAX_VALUE = 190;
        [Config("The ammount of variance in the sky color.")]
        public int SKY_COLOR_DIFFERENCE = 65;
        [Config("How high the fog goes. Higher values = higher fog.", 0f, 1f)]
        public float SKY_FOG_HEIGHT = .4f;
        [Config("How translucent the fog is. Lower values = more translucent fog.", 0f, 1f)]
        public float SKY_FOG_OPACITY = .8f;
        [Config("Use a darker color for the fog?")]
        public bool SKY_FOG_DARK = false;

        [Config("The minimum ammount of planets in the sky.", 0, 50)]
        public int TRABANT_MIN_AMMOUNT = 1;
        [Config("The maximum ammount of planets in the sky.", 0, 50)]
        public int TRABANT_MAX_AMMOUNT = 3;
        [Config("Determines the scaling-factor of all trabants. Lower value = smaller planets.", .0001f, 4f)]
        public float TRABANT_SCALE = 1f;
        [Config("How much the planets in the background are obscured by the sky. Lower values = lower visibility.", .0001f, 1f)]
        public float TRABANT_OPACITY = .45f;
        [Config("The noise algorithm used to generate the surface of trabants.")]
        public NoiseType TRABANT_SURFACE_NOISE_TYPE = NoiseType.CubicFractal;
        [Config("Frequency of the noise texture used to generate the surface of trabants.", .001f, 3f)]
        public float TRABANT_SURFACE_NOISE_FREQUENCY = .0095f;
        [Config("Determines how dark the first color on trabants' surfaces can be.", 0, 254)]
        public int TRABANT_SURFACE_COLOR1_MIN_VALUE = 150;
        [Config("Determines how light the first color on trabants' surfaces can be.", 1, 255)]
        public int TRABANT_SURFACE_COLOR1_MAX_VALUE = 220;
        [Config("Determines how dark the second color on trabants' surfaces can be.", 0, 254)]
        public int TRABANT_SURFACE_COLOR2_MIN_VALUE = 90;
        [Config("Determines how light the second color on trabants' surfaces can be.", 1, 255)]
        public int TRABANT_SURFACE_COLOR2_MAX_VALUE = 160;
        [Config("The ratio between the first and second color on trabants.", .0001f, 1f)]
        public float TRABANT_SURFACE_COLOR_THRESHOLD = .5f;
        [Config("Should 50% of trabants have clouds?")]
        public bool TRABANT_CLOUDS_ENABLED = true;
        [Config("The noise algorithm used to generate clouds on trabants.")]
        public NoiseType TRABANT_CLOUDS_NOISE_TYPE = NoiseType.SimplexFractal;
        [Config("Frequency of the noise texture used to generate clouds on trabants.", .0001f, 1f)]
        public float TRABANT_CLOUDS_NOISE_FREQUENCY = .0065f;
        [Config(.0001f, 1f)]
        public float TRABANT_CLOUDS_OPACITY = .25f;
        [Config()]
        public int TRABANT_CLOUDS_COLOR_MIN_VALUE = 230;
        [Config()]
        public int TRABANT_CLOUDS_COLOR_MAX_VALUE = 255;
        [Config("Better not use this. Doesn't look very good 99% of the time...")]
        public bool TRABANT_FRESNEL_ENABLED = true;
        [Config("", 0.0001f, 1f)]
        public float TRABANT_FRESNEL_INTENSITY = .6f;
        [Config("")]
        public int TRABANT_FRESNEL_RADIUS = 8;
        [Config("Determines how much the planets in the background are blurred. Has a huge performance hit with higher radii (because I suck at coding). Set to 0 for no blur.", 0, 99)]
        public int TRABANT_BLUR_RADIUS = 2;
        [Config("Not implemented yet...")]
        public bool TRABANT_BLUR_USE_EDGEBLUR = false;

        [Config("Render debug data (heightmaps, colormaps, gradients, etc.) as images?")]
        public bool DEBUG_RENDER_MISC = false;

        private Random _rng = new Random();
        private int _seed = -1;

        private readonly float PI2 = (float)(Math.PI * 2);
#if ENABLE_DEBUG_LOOPS
        private readonly ParallelOptions DEBUG_OPTIONS = new ParallelOptions() { MaxDegreeOfParallelism = 1 };
#else
        private readonly ParallelOptions DEBUG_OPTIONS = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
#endif

        public int GeneratePlanet( string outputName )
        {
#if ENABLE_DEBUG_LOOPS
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Debug loops are enabled. This means that performance is decreased. Recompile with preprocessor variable ENABLE_DEBUG_LOOPS not defined.");
#endif
            Console.ForegroundColor = ConsoleColor.White;

            // set up seed and noise generator
            _seed = ALL_RANDOM_SEED;
            if ( _seed == -1 )
            {
                unchecked
                {
                    _seed = DateTime.Now.ToBinary().GetHashCode(); ;

                }
            }
            _rng = new Random( _seed );

            FastNoise noise = new FastNoise( _seed );
            noise.SetNoiseType( MOUNTAINS_NOISE_TYPE );
            noise.SetFrequency( MOUNTAINS_NOISE_FREQUENCY );

            // generate mountain scaling gradient
            int mountainScalingGradientSize = (int)(ALL_RES_Y * MOUNTAINS_PARALLAX_GRADIENT_SCALE + .5f);
            float[] mountainScalingGradient = generateLinearGradient(1, 0, mountainScalingGradientSize);

            // generate mountains-heightmap
            float[,] heightmap = new float[ALL_RES_X, ALL_RES_Y];
            using ( Bitmap img = new Bitmap( ALL_RES_X, ALL_RES_Y, PixelFormat.Format32bppArgb ) )
            {
                var lockedData = img.LockBits(new Rectangle(0, 0, ALL_RES_X, ALL_RES_Y), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                Parallel.For( 0, ALL_RES_X, u =>
                 {
                     Parallel.For( 0, ALL_RES_Y, v =>
                     {
                         float value = ( ( noise.GetNoise( u, v ) + 1 ) / 2 ) * MOUNTAINS_HEIGHT_SCALE;
                         if ( MOUNTAINS_PARALLAX_GRADIENT )
                             value *= ( mountainScalingGradient[ Math.Min( v, mountainScalingGradientSize - 1 ) ] );

                         heightmap[ u, v ] = value;

                         int valueInt = (int)(value * 255f + .5f);
                         int argb = (0xFF << 24) | (valueInt << 16) | (valueInt << 8) | (valueInt << 0);
                         Marshal.WriteInt32( lockedData.Scan0, ( v * ALL_RES_X + u ) * 4, argb );
                     } );
                 } );

                img.UnlockBits( lockedData );

                if ( DEBUG_RENDER_MISC )
                    img.Save( outputName + "_mountains_heightmap.png" );
            }

            using ( Bitmap result = new Bitmap( ALL_RES_X, ALL_RES_Y, PixelFormat.Format32bppArgb ) )
            {
                // draw sky
                Console.WriteLine( "Drawing sky..." );
                int colorSkyLow = getRandomDistributedColor(SKY_RANDOM_COLOR_MIN_VALUE, SKY_RANDOM_COLOR_MAX_VALUE);
                int colorSkyHigh = modifyColor(colorSkyLow, SKY_COLOR_DIFFERENCE);

                using ( Graphics g = Graphics.FromImage( result ) )
                {
                    Rectangle toFill = new Rectangle(0, 0, ALL_RES_X, ALL_RES_Y);
                    g.FillRectangle( new LinearGradientBrush( toFill, Color.FromArgb( colorSkyLow ), Color.FromArgb( colorSkyHigh ), -90 ), toFill );
                    g.Flush();
                }

                // draw trabants
                var lockedData = result.LockBits(new Rectangle(0, 0, ALL_RES_X, ALL_RES_Y), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int trabantCount = _rng.Next(TRABANT_MIN_AMMOUNT, TRABANT_MAX_AMMOUNT + 1);
                Console.WriteLine( "Drawing " + trabantCount + " trabant" + ( trabantCount == 1 ? String.Empty : "s" ) + "..." );

                List<int[]> alreadyUsed = new List<int[]>();
                HashSet<long> alreadyUsedPosHashes = new HashSet<long>();
                for ( int i = 0; i < trabantCount; i++ )
                {
                    int trabantSize = (int)(ALL_RES_X * lerp(.08f, .18f, (float)_rng.NextDouble()) * TRABANT_SCALE);
                    if ( trabantSize % 2 != 0 )
                        trabantSize++;
                    int posX = _rng.Next(0, ALL_RES_X - trabantSize / 2 - 1);
                    int posY = _rng.Next(11, (int)(ALL_RES_X * .2f + .5f));

                    int[] trabantBackgroundGradient = new int[trabantSize + 20];
                    for ( int b = 0; b < trabantSize + 20; b++ )
                        trabantBackgroundGradient[ b ] = Marshal.ReadInt32( lockedData.Scan0, ( posY - 10 ) * lockedData.Stride + posX * 4 );

                    using ( Bitmap trabantImg = generateTrabant( noise, trabantSize, trabantBackgroundGradient, outputName ) )
                    {
                        var lockedTrabantData = trabantImg.LockBits(new Rectangle(0, 0, trabantSize, trabantSize), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                        Parallel.For( 0, trabantSize, u =>
                         {
                             Parallel.For( 0, trabantSize, v =>
                             {
                                 int absX = posX + u;
                                 int absY = posY + v;
                                 long posHash = ((long)absX << 32) | (long)absY;

                                 if ( !alreadyUsedPosHashes.Contains( posHash ) )
                                 {
                                     int trabantColor = Marshal.ReadInt32(lockedTrabantData.Scan0, v * lockedTrabantData.Stride + u * 4);
                                     if ( trabantColor != 0 )
                                     {
                                         int skyColor = Marshal.ReadInt32(lockedData.Scan0, absY * lockedData.Stride + absX * 4);
                                         //Marshal.WriteInt32( lockedData.Scan0, absY * lockedData.Stride + absX * 4, interpolateColors( skyColor, trabantColor, TRABANT_OPACITY ) );
                                         Marshal.WriteInt32( lockedData.Scan0, absY * lockedData.Stride + absX * 4, alphaBlend( skyColor, trabantColor, TRABANT_OPACITY ) );

                                         lock ( alreadyUsedPosHashes )
                                             alreadyUsedPosHashes.Add( posHash );
                                     }
                                 }
                             } );
                         } );
                        trabantImg.UnlockBits( lockedTrabantData );

                    }
                }

                // draw mountains
                Console.WriteLine( "Drawing mountains..." );
                int slicePos = 0;

                int colorMountains = getRandomDistributedColor(MOUNTAINS_RANDOM_COLOR_MIN_VALUE, MOUNTAINS_RANDOM_COLOR_MAX_VALUE);
                for ( int s = 0; s < MOUNTAINS_PARALLAX_SLICES; s++ )
                {
                    Parallel.For( 0, ALL_RES_X, i =>
                     {
                         int maxY = (int)(heightmap[i, slicePos] * ALL_RES_Y + .5f);
                         Parallel.For( 0, maxY + 1, j =>
                         {
                             Marshal.WriteInt32( lockedData.Scan0, ( lockedData.Stride * ( ALL_RES_Y - 1 ) ) - ( j * ALL_RES_X + i ) * 4, colorMountains );
                         } );
                     } );

                    slicePos += MOUNTAINS_PARALLAX_OFFSET;

                    if ( MOUNTAINS_LIGHT_TO_DARK )
                        colorMountains = modifyColor( colorMountains, -MOUNTAINS_BRIGHTNESS_INCREASE );
                    else
                        colorMountains = modifyColor( colorMountains, MOUNTAINS_BRIGHTNESS_INCREASE );
                }

                // draw fog (new)
                Console.WriteLine( "Drawing fog..." );

                int[] alphaValues = generateLinearGradient( ( int )( 255f * SKY_FOG_OPACITY + .5f ), 0, (int)(ALL_RES_Y * SKY_FOG_HEIGHT + .5f) ).Select( n => ( int )n ).ToArray();
                int fogColor = SKY_FOG_DARK
                    ? getArgb( getR( colorSkyLow ), getG( colorSkyLow ), getB( colorSkyLow ) )      //, ( int )( 255f * SKY_FOG_OPACITY + .5f ) )
                    : getArgb( getR( colorSkyHigh ), getG( colorSkyHigh ), getB( colorSkyHigh ) );  //, ( int )( 255f * SKY_FOG_OPACITY + .5f ) );

                Parallel.For( 0, ALL_RES_X, x =>
                {
                    Parallel.For( 0, alphaValues.Length, y =>
                    {
                        int bgColor = readColor( lockedData, x, ALL_RES_Y - y - 1 );
                        int fogColorAtPosition = getArgb( getR( fogColor ), getG( fogColor ), getB( fogColor ), alphaValues[y] );
                        int newColor = alphaBlend( bgColor, fogColorAtPosition );

                        writeColor( lockedData, x, ALL_RES_Y - y - 1, newColor );
                    } );
                } );

                result.UnlockBits( lockedData );

                // draw fog
                //Console.WriteLine( "Drawing fog..." );
                //using ( Graphics g = Graphics.FromImage( result ) )
                //{
                //    int fogBaseColor = (SKY_FOG_DARK ? colorSkyLow : colorSkyHigh);
                //    byte fogOpacity = (byte)(255 * SKY_FOG_OPACITY + .5f);
                //    int colorFog = (fogOpacity << 24
                //        | (fogBaseColor & 0x0000FF)
                //        | (fogBaseColor & 0x00FF00)
                //        | (fogBaseColor & 0xFF0000));

                //    Rectangle toFill = new Rectangle(0, ALL_RES_Y - (int)(ALL_RES_Y * SKY_FOG_HEIGHT + .5f), ALL_RES_X, (int)(ALL_RES_Y * SKY_FOG_HEIGHT + .5f));
                //    g.FillRectangle( new LinearGradientBrush( toFill, Color.FromArgb( colorFog ), Color.Transparent, -90 ), toFill );
                //    g.Flush();
                //}

                result.Save( outputName + ".png" );
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write( "Finished generating! " );
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine( "Seed used: " + _seed );

            _trabantCounter = 1;
            return _seed;
        }

        private int _trabantCounter = 1;
        private Bitmap generateTrabant( FastNoise noise, int size, int[] backgroundGradient, string outputName )
        {
            Bitmap result = new Bitmap(size, size);
            using ( Graphics g = Graphics.FromImage( result ) )
                g.Clear( Color.Transparent );
            var lockedData = result.LockBits(new Rectangle(0, 0, size, size), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int colorTrabantHigh = getRandomDistributedColor(TRABANT_SURFACE_COLOR1_MIN_VALUE, TRABANT_SURFACE_COLOR1_MAX_VALUE);
            int colorTrabantLow = getRandomDistributedColor(TRABANT_SURFACE_COLOR2_MIN_VALUE, TRABANT_SURFACE_COLOR2_MAX_VALUE);

            int surfaceSeed = _seed * (_trabantCounter + 1);
            int cloudsSeed = surfaceSeed ^ (_trabantCounter + 1);

            noise.SetNoiseType( TRABANT_SURFACE_NOISE_TYPE );
            noise.SetFrequency( TRABANT_SURFACE_NOISE_FREQUENCY );
            noise.SetSeed( surfaceSeed );

            FastNoise cloudsNoise = new FastNoise(cloudsSeed);
            cloudsNoise.SetFrequency( TRABANT_CLOUDS_NOISE_FREQUENCY );
            cloudsNoise.SetNoiseType( TRABANT_CLOUDS_NOISE_TYPE );

            bool trabantHasClouds = _rng.Next(2) == 0;
            int cloudsColor = getRandomDistributedColor(
                TRABANT_CLOUDS_COLOR_MIN_VALUE,
                TRABANT_CLOUDS_COLOR_MAX_VALUE);

            float[,] trabantHeightmap = new float[size * 2, size * 2];
            int[,] trabantColormap = new int[size * 2, size * 2];
            int[,] cloudsColormap = new int[size * 2, size * 2];
            Parallel.For( 0, size * 2, u =>
             {
                 Parallel.For( 0, size * 2, v =>
                 {
                     float valueSurface = (noise.GetNoise(u, v) + 1) / 2;
                     trabantHeightmap[ u, v ] = valueSurface;

                     if ( valueSurface >= TRABANT_SURFACE_COLOR_THRESHOLD )
                         trabantColormap[ u, v ] = colorTrabantHigh;
                     else
                         trabantColormap[ u, v ] = colorTrabantLow;

                     // sphere mapping
                     float uM = lerp(-1, 1, u / (size * 2f));
                     float vM = lerp(-1, 1, v / (size * 2f));

                     float x = (float)((uM * 2) / (1 + Math.Pow(uM, 2) + Math.Pow(vM, 2)));
                     float y = (float)((vM * 2) / (1 + Math.Pow(uM, 2) + Math.Pow(vM, 2)));
                     x = rlerp( -1, 1, x );
                     y = rlerp( -1, 1, y );

                     int xResult = (int)(x * (size - 1) + .5f);
                     int yResult = (int)(y * (size - 1) + .5f);
                     Marshal.WriteInt32( lockedData.Scan0, yResult * lockedData.Stride + xResult * 4, trabantColormap[ u, v ] );

                     if ( trabantHasClouds )
                     {
                         float valueClouds = (cloudsNoise.GetNoise(u, v) + 1) / 2;
                         if ( valueClouds > .5f )
                         {
                             int colorOnPixel = trabantColormap[u, v];
                             //int mixedColorOnPixel = interpolateColors(colorOnPixel, cloudsColor, TRABANT_CLOUDS_OPACITY);
                             int mixedColorOnPixel = alphaBlend(colorOnPixel, cloudsColor, TRABANT_CLOUDS_OPACITY);
                             cloudsColormap[ u, v ] = mixedColorOnPixel;
                             trabantColormap[ u, v ] = mixedColorOnPixel;
                             //Marshal.WriteInt32( lockedData.Scan0, yResult * lockedData.Stride + xResult * 4, mixedColorOnPixel );
                             writeColor( lockedData, xResult, yResult, mixedColorOnPixel );
                         }
                     }
                 } );
             } );
            result.UnlockBits( lockedData );

            if ( TRABANT_FRESNEL_ENABLED )
                applyQuickAndDirtyFresnel( result, TRABANT_FRESNEL_RADIUS, outputName );

            result = applyGaussianBlur( result, TRABANT_BLUR_RADIUS, backgroundGradient );

            if ( DEBUG_RENDER_MISC )
            {
                using ( Bitmap trabantHeightmapDebug = arrayToBitmap( trabantHeightmap ) )
                    trabantHeightmapDebug.Save( outputName + "_trabant" + _trabantCounter + "_heightmap.png" );
                using ( Bitmap trabantColormapDebug = arrayToBitmap( trabantColormap ) )
                    trabantColormapDebug.Save( outputName + "_trabant" + _trabantCounter + "_colormap.png" );
                using ( Bitmap trabantCloudsColormapDebug = arrayToBitmap( cloudsColormap ) )
                    trabantCloudsColormapDebug.Save( outputName + "_trabant" + _trabantCounter + "_clouds_colormap.png" );
                result.Save( outputName + "_trabant" + _trabantCounter + ".png" );
            }

            _trabantCounter++;
            return result;
        }

        // ToDo: wieder zu einem einfachen Blur machen, ohne die BG Gradients. Dafür Trabs nicht direkt auf den Himmel zeichnen, sondern
        //       einen Ausschnitt, der größer ist, als der Trab vom BG nehmen, den Trab 'draufstampen', das ganze Teil blurren und dann an
        //       entsprechender Stelle über den Himmel zeichnen.
        private Bitmap applyGaussianBlur( Bitmap img, int radius, int[] backgroundGradient = null )
        {
            if ( radius == 0 )
                return img;

            int imgWidth = img.Width;
            int imgHeight = img.Height;
            Bitmap result = new Bitmap(imgWidth, imgHeight);

            var lockedDataImg = img.LockBits(new Rectangle(0, 0, imgWidth, imgHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var lockedDataResult = result.LockBits(new Rectangle(0, 0, imgWidth, imgHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            for ( int r = 0; r < radius; r++ )
            {
                Parallel.For( 0, imgWidth, DEBUG_OPTIONS, u =>
                 {
                     Parallel.For( 0, imgHeight, DEBUG_OPTIONS, v =>
                     {
                         int neighbourRadius = 3 + r * 2;
                         int foundNeighbours = 0;
                         int foundTransparentNeighbours = 0;
                         int totalNeighbourR = 0, totalNeighbourG = 0, totalNeighbourB = 0, totalNeighbourA = 0;

                         // top and bottom
                         for ( int n = 0; n < neighbourRadius; n++ )
                         {
                             int x = u - (r - 1) + n;
                             int yTop = v - 1 - r;
                             int yBot = v + 1 + r;

                             if ( x < 0 || x >= imgWidth )
                                 continue;
                             if ( yTop > 0 && yTop < imgHeight )
                             {
                                 int neighbourColor = Marshal.ReadInt32(lockedDataImg.Scan0, lockedDataImg.Stride * yTop + x * 4);
                                 if ( neighbourColor == 0 && backgroundGradient != null )  // ToDo: == 0 ist kein sicherer Check, ob die Farbe transparent ist
                                 {
                                     int bgColor = backgroundGradient[9 + v];
                                     totalNeighbourR += getR( bgColor ); totalNeighbourG += getG( bgColor ); totalNeighbourB += getB( bgColor );
                                     foundTransparentNeighbours++;
                                 }
                                 else
                                 {
                                     totalNeighbourR += getR( neighbourColor ); totalNeighbourG += getG( neighbourColor ); totalNeighbourB += getB( neighbourColor );
                                     totalNeighbourA += getA( neighbourColor );
                                 }
                                 foundNeighbours++;
                             }
                             if ( yBot > 0 && yBot < imgHeight )
                             {
                                 int neighbourColor = Marshal.ReadInt32(lockedDataImg.Scan0, lockedDataImg.Stride * yBot + x * 4);
                                 if ( neighbourColor == 0 && backgroundGradient != null )
                                 {
                                     int bgColor = backgroundGradient[9 + v];
                                     totalNeighbourR += getR( bgColor ); totalNeighbourG += getG( bgColor ); totalNeighbourB += getB( bgColor );
                                     foundTransparentNeighbours++;
                                 }
                                 else
                                 {
                                     totalNeighbourR += getR( neighbourColor ); totalNeighbourG += getG( neighbourColor ); totalNeighbourB += getB( neighbourColor );
                                     totalNeighbourA += getA( neighbourColor );
                                 }
                                 foundNeighbours++;
                             }
                         }

                         // left and right
                         for ( int n = 0; n < neighbourRadius - 2; n++ )
                         {
                             int y = v - (r - 1) + n;
                             int xLeft = u - 1 - r;
                             int xRight = u + 1 + r;

                             if ( y < 0 || y >= imgHeight )
                                 continue;
                             if ( xLeft > 0 && xLeft < imgWidth )
                             {
                                 int neighbourColor = Marshal.ReadInt32(lockedDataImg.Scan0, lockedDataImg.Stride * y + xLeft * 4);
                                 if ( neighbourColor == 0 && backgroundGradient != null )
                                 {
                                     int bgColor = backgroundGradient[9 + v];
                                     totalNeighbourR += getR( bgColor ); totalNeighbourG += getG( bgColor ); totalNeighbourB += getB( bgColor );
                                     foundTransparentNeighbours++;
                                 }
                                 else
                                 {
                                     totalNeighbourR += getR( neighbourColor ); totalNeighbourG += getG( neighbourColor ); totalNeighbourB += getB( neighbourColor );
                                     totalNeighbourA += getA( neighbourColor );
                                 }
                                 foundNeighbours++;
                             }
                             if ( xRight > 0 && xRight < imgWidth )
                             {
                                 int neighbourColor = Marshal.ReadInt32(lockedDataImg.Scan0, lockedDataImg.Stride * y + xRight * 4);
                                 if ( neighbourColor == 0 && backgroundGradient != null )
                                 {
                                     int bgColor = backgroundGradient[9 + v];
                                     totalNeighbourR += getR( bgColor ); totalNeighbourG += getG( bgColor ); totalNeighbourB += getB( bgColor );
                                     foundTransparentNeighbours++;
                                 }
                                 else
                                 {
                                     totalNeighbourR += getR( neighbourColor ); totalNeighbourG += getG( neighbourColor ); totalNeighbourB += getB( neighbourColor );
                                     totalNeighbourA += getA( neighbourColor );
                                 }
                                 foundNeighbours++;
                             }

                         }

                         if ( foundNeighbours != foundTransparentNeighbours )
                         {
                             int pixelColor = Marshal.ReadInt32(lockedDataImg.Scan0, v * lockedDataImg.Stride + u * 4);
                             int meanR = (getR(pixelColor) + totalNeighbourR) / (foundNeighbours + 1);
                             int meanG = (getG(pixelColor) + totalNeighbourG) / (foundNeighbours + 1);
                             int meanB = (getB(pixelColor) + totalNeighbourB) / (foundNeighbours + 1);
                             int meanA = (getA(pixelColor) + totalNeighbourA) / (foundNeighbours + 1);
                             Marshal.WriteInt32( lockedDataResult.Scan0, v * lockedDataResult.Stride + u * 4, getArgb( meanR, meanG, meanB, meanA ) );
                         }
                         else
                             Marshal.WriteInt32( lockedDataResult.Scan0, v * lockedDataResult.Stride + u * 4, 0x00000000 );
                     } );
                 } );
            }

            img.UnlockBits( lockedDataImg );
            result.UnlockBits( lockedDataResult );
            return result;
        }

        // meh
        // ToDo: hart optimieren (wenn ich es überhaupt behalte)
        private void applyQuickAndDirtyFresnel( Bitmap trabantImg, int radius, string outputName )
        {
            Bitmap fresnelMapDebug = new Bitmap(trabantImg.Width, trabantImg.Height);

            var lockedData = trabantImg.LockBits(new Rectangle(0, 0, trabantImg.Width, trabantImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int fresnelColor = getAverageColor(lockedData, true);
            int fresnelColorValue = getValue(fresnelColor);
            if ( fresnelColorValue < 230 )
                fresnelColor = modifyColor( fresnelColor, 230 - fresnelColorValue );
            //trabantImg.UnlockBits( lockedData );

            for ( int r = 0; r < radius; r++ )
            {
                for ( int i = 0; i < 1000; i++ )
                {
                    float x = (float)Math.Cos(((float)i / 1000f) * PI2) * (trabantImg.Width + 1 - r);   // do NOT question the +1
                    float y = (float)Math.Sin(((float)i / 1000f) * PI2) * (trabantImg.Height + 1 - r);

                    float procentualX = rlerp(-trabantImg.Width, trabantImg.Width, x);
                    float procentualY = rlerp(-trabantImg.Height, trabantImg.Height, y);

                    int actualX = (int)(lerp(0, trabantImg.Width - 1, procentualX) + .5f);
                    int actualY = (int)(lerp(0, trabantImg.Height - 1, procentualY) + .5f);

                    int c = -1;
                    unchecked
                    {
                        //c = interpolateColors( trabantImg.GetPixel( actualX, actualY ).ToArgb(), fresnelColor, ( 1 - r / ( float )radius ) * TRABANT_FRESNEL_INTENSITY );
                        c = interpolateColors( readColor( lockedData, actualX, actualY ), fresnelColor, ( 1 - r / ( float )radius ) * TRABANT_FRESNEL_INTENSITY );
                    }

                    //trabantImg.SetPixel( actualX, actualY, Color.FromArgb( c ) );
                    writeColor( lockedData, actualX, actualY, c );

                    if ( DEBUG_RENDER_MISC )
                        fresnelMapDebug.SetPixel( actualX, actualY, Color.FromArgb( c ) );
                }
            }
            trabantImg.UnlockBits( lockedData );

            if ( DEBUG_RENDER_MISC )
                fresnelMapDebug.Save( outputName + "_trabant" + _trabantCounter + "_fresnelmask.png" );

            #region Old
            /*
            // ToDo: InitialCapacity auf einen heuristischen Wert setzen => Performance!
            List<long> nextCandidates = new List<long>();
            HashSet<long> touched = new HashSet<long>();

            var lockedData = trabantImg.LockBits(new Rectangle(0, 0, trabantImg.Width, trabantImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Parallel.For(0, lockedData.Width, DEBUG_OPTIONS, u =>
            {
                Parallel.For(0, lockedData.Height, DEBUG_OPTIONS, v =>
                {
                    int stride = lockedData.Stride;
                    int offset = stride * v + u * 4;
                    int pixelColor = Marshal.ReadInt32(lockedData.Scan0, offset);

                    if (pixelColor != 0)
                    {
                        long?[] neighbours = new long?[4];
                        if (offset - stride >= 0)
                            neighbours[0] = ((long)Marshal.ReadInt32(lockedData.Scan0, offset - stride) << 32) | ((long)offset - stride);
                        if (offset + 1 < lockedData.Height * stride)
                            neighbours[1] = ((long)Marshal.ReadInt32(lockedData.Scan0, offset + 1) << 32) | ((long)offset + 1);
                        if (offset + stride < lockedData.Height * stride)
                            neighbours[2] = ((long)Marshal.ReadInt32(lockedData.Scan0, offset + stride) << 32) | ((long)offset + stride);
                        if (offset - 1 >= 0)
                            neighbours[3] = ((long)Marshal.ReadInt32(lockedData.Scan0, offset - 1) << 32) | ((long)offset - 1);

                        foreach (long? neighbour in neighbours)
                        {
                            if (!neighbour.HasValue)
                                continue;

                            int neighbourColor = (int)(neighbour.Value >> 32);

                            if (neighbourColor == 0)
                            {
                                unchecked
                                {
                                    Marshal.WriteInt32(lockedData.Scan0, offset, interpolateColors(pixelColor, (int)0xFFFFFFFF, TRABANT_FRESNEL_INTENSITY));
                                    nextCandidates.AddRange(neighbours
                                        .Where(n => n.HasValue)
                                        .Select(n => n.Value)
                                        .Where(n => n != 0 && !nextCandidates.Contains(n))
                                        .ToArray());
                                    touched.Add(neighbour.Value);
                                    break;
                                }
                            }
                        }
                    }
                });
            });

            for (int i = 0; i < radius - 1; i++)
            {
                List<long> newNextCandidates = new List<long>(nextCandidates.Count);

                foreach (long nextCandidate in nextCandidates)
                {
                    int offset = (int)(nextCandidate & Int32.MaxValue);
                    int pixelColor = (int)(nextCandidate >> 32);
                    int stride = lockedData.Stride;
                    //int pixelColor = Marshal.ReadInt32(lockedData.Scan0, offset);

                    if (pixelColor != 0)
                    {
                        long?[] neighbours = new long?[4];
                        if (offset - stride >= 0)
                            neighbours[0] = ((long)Marshal.ReadInt32(lockedData.Scan0, offset - stride) << 32) | ((long)offset - stride);
                        if (offset + 1 < lockedData.Height * stride)
                            neighbours[1] = ((long)Marshal.ReadInt32(lockedData.Scan0, offset + 1) << 32) | ((long)offset + 1);
                        if (offset + stride < lockedData.Height * stride)
                            neighbours[2] = ((long)Marshal.ReadInt32(lockedData.Scan0, offset + stride) << 32) | ((long)offset + stride);
                        if (offset - 1 >= 0)
                            neighbours[3] = ((long)Marshal.ReadInt32(lockedData.Scan0, offset - 1) << 32) | ((long)offset - 1);

                        foreach (long? neighbour in neighbours)
                        {
                            if (!neighbour.HasValue)
                                continue;

                            int neighbourColor = (int)(neighbour.Value >> 32);

                            if (touched.Contains(neighbour.Value))
                            {
                                unchecked
                                {
                                    Marshal.WriteInt32(lockedData.Scan0, offset, interpolateColors(pixelColor, (int)(0xFFFFFFFF * (int)(1 - (float)i / (radius - 1))), TRABANT_FRESNEL_INTENSITY));
                                    newNextCandidates.AddRange(neighbours
                                        .Where(n => n.HasValue)
                                        .Select(n => n.Value)
                                        .Where(n => n != 0 && !newNextCandidates.Contains(n) && !touched.Contains(n))
                                        .ToArray());
                                    touched.Add(neighbour.Value);
                                    break;
                                }
                            }
                        }
                    }
                }

                nextCandidates = newNextCandidates;
            }

            trabantImg.UnlockBits(lockedData);
            */
            #endregion
        }

        private int readColor( BitmapData data, int x, int y ) => Marshal.ReadInt32( data.Scan0, y * data.Stride + x * 4 );
        private void writeColor( BitmapData data, int x, int y, int color ) => Marshal.WriteInt32( data.Scan0, y * data.Stride + x * 4, color );

        private int alphaBlend( int colorBase, int colorTop )
        {
            float rBase = getR( colorBase ) / 255f;
            float gBase = getG( colorBase ) / 255f;
            float bBase = getB( colorBase ) / 255f;

            float rTop = getR( colorTop ) / 255f;
            float gTop = getG( colorTop ) / 255f;
            float bTop = getB( colorTop ) / 255f;
            float aTop = getA( colorTop ) / 255f;

            float rResult = aTop * rTop + ( 1 - aTop )  * rBase;
            float gResult = aTop * gTop + ( 1 - aTop )  * gBase;
            float bResult = aTop * bTop + ( 1 - aTop )  * bBase;

            return getArgb( ( int )( 255 * rResult + .5f ), ( int )( 255 * gResult + .5f ), ( int )( 255 * bResult + .5f ) );
        }

        private int alphaBlend( int colorBase, int colorTop, float topOpacity )
        {
            int colorTopNew = getArgb( getR( colorTop ), getB( colorTop ), getB( colorTop ), ( int )( topOpacity * 255f + .5f) );
            return alphaBlend( colorBase, colorTopNew );
        }

        private int getAverageColor( BitmapData data, bool ignoreTransparent )
        {
            long tR = 0, tG = 0, tB = 0;
            long totalPixels = (long)data.Width * data.Height;

            Parallel.For( 0, data.Width, u =>
             {
                 Parallel.For( 0, data.Height, v =>
                 {
                     int c = Marshal.ReadInt32(data.Scan0, v * data.Stride + u * 4);
                     if ( ignoreTransparent && getA( c ) == 0 )
                         totalPixels--;
                     else
                     {
                         tR += getR( c );
                         tG += getG( c );
                         tB += getB( c );
                     }
                 } );
             } );

            tR /= totalPixels;
            tG /= totalPixels;
            tB /= totalPixels;

            return getArgb( ( int )tR, ( int )tG, ( int )tB );
        }

        private int getRandomDistributedColor( int min, int max, int alpha = 0xFF )
        {
            int totalValue = _rng.Next(min * 3, max * 3);

            float percR = (float)_rng.NextDouble();
            float percG = (1 - percR) * (float)_rng.NextDouble();
            float percB = 1 - percR - percG;

            byte r = (byte)(percR * totalValue + .5f);
            byte g = (byte)(percG * totalValue + .5f);
            byte b = (byte)(percB * totalValue + .5f);

            return ( alpha << 24 ) | ( b << 16 ) | ( g << 8 ) | ( r << 0 );
        }

        private int modifyColor( int color, int value )
        {
            //int b = (color & 0x0000FF);
            //int g = (color & 0x00FF00) >> 8;
            //int r = (color & 0xFF0000) >> 16;
            int r = getR( color );
            int g = getG( color );
            int b = getB( color );

            r = Math.Max( Math.Min( r + value, 255 ), 0 );
            g = Math.Max( Math.Min( g + value, 255 ), 0 );
            b = Math.Max( Math.Min( b + value, 255 ), 0 );

            return getArgb( r, g, b );
            //return ( 0xFF << 24 ) | ( r << 16 ) | ( g << 8 ) | ( b << 0 );
        }

        private int interpolateColors( int colorA, int colorB, float value )
        {
            int aR = getR(colorA);
            int aG = getG(colorA);
            int aB = getB(colorA);

            int bR = getR(colorB);
            int bG = getG(colorB);
            int bB = getB(colorB);

            int mR = (int)lerp(aR, bR, value);
            int mG = (int)lerp(aG, bG, value);
            int mB = (int)lerp(aB, bB, value);

            return getArgb( mR, mG, mB );
            //return ( 0xFF << 24 ) | ( mR << 16 ) | ( mG << 8 ) | ( mB << 0 );
        }

        private float[] generateLinearGradient( float start, float end, int length )
        {
            float[] result = new float[length];
            Parallel.For( 0, length, i => result[ i ] = lerp( start, end, ( float )i / length ) );
            return result;
        }

        private Bitmap arrayToBitmap( float[,] data )
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            Bitmap result = new Bitmap(width, height);
            var lockedData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Parallel.For( 0, width, u =>
             {
                 Parallel.For( 0, height, v =>
                 {
                     int valueInt = (int)(data[u, v] * 255 + .5f);
                     int color = (0xFF << 24) | (valueInt << 16) | (valueInt << 8) | (valueInt << 0);

                     Marshal.WriteInt32( lockedData.Scan0, v * lockedData.Stride + u * 4, color );
                 } );
             } );

            result.UnlockBits( lockedData );
            return result;
        }

        private Bitmap arrayToBitmap( int[,] data )
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            Bitmap result = new Bitmap(width, height);
            var lockedData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Parallel.For( 0, width, u =>
             {
                 Parallel.For( 0, height, v =>
                 {
                     Marshal.WriteInt32( lockedData.Scan0, v * lockedData.Stride + u * 4, data[ u, v ] );
                 } );
             } );

            result.UnlockBits( lockedData );
            return result;
        }

        private int getR( int color ) => ( color & 0xFF0000 ) >> 16;
        private int getG( int color ) => ( color & 0x00FF00 ) >> 8;
        private int getB( int color ) => ( color & 0x0000FF ) >> 0;
        private int getA( int color ) => ( int )( ( color & 0xFF000000 ) >> 24 );

        private int getArgb( int r, int g, int b, int a = 0xFF ) => ( a << 24 ) | ( r << 16 ) | ( g << 8 ) | ( b << 0 );
        private int getValue( int color ) => ( getR( color ) + getG( color ) + getB( color ) ) / 3;

        /// <summary>
        /// linear interpolation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private float lerp( float x, float y, float value )
        {
            return x + ( y - x ) * value;
        }

        /// <summary>
        /// reverse linear interpolation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static float rlerp( float x, float y, float value )
        {
            return ( x - value ) / ( x - y );
        }

        public void LoadSettings( XElement root )
        {
            Type generatorType = this.GetType();
            foreach ( XElement element in root.Elements() )
            {
                Type elementType = null;
                switch ( element.Name.ToString() )
                {
                    case "Int32":
                        elementType = typeof( Int32 );
                        break;
                    case "Bool":
                        elementType = typeof( Boolean );
                        break;
                    case "Float":
                        elementType = typeof( Single );
                        break;
                    case "EnumNoiseType":
                        elementType = typeof( NoiseType );
                        break;
                }

                string elementName = element.Attribute( "Name" ).Value;
                string elementValue = element.Attribute( "Value" ).Value;

                if ( elementType != typeof( NoiseType ) )
                {
                    generatorType.GetField( elementName ).SetValue( this, Convert.ChangeType( elementValue, elementType ) );
                }
                else
                {
                    generatorType.GetField( elementName ).SetValue( this, Enum.Parse( typeof( NoiseType ), elementValue ) );
                }
            }
        }
    }
}