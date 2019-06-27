using System;

namespace D3DLab.Std.Engine.Core.Utilities {
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Numerics;
    using D3DLab.Std.Engine.Core.Common;

    public interface ILabNoise {
        void Render();
        float GetHeigthValue(int x, int y);
        MemoryStream GetBitmap();
    }

    public static class LabAlgos {
        static LabAlgos() {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// info about params - http://libnoise.sourceforge.net/tutorials/tutorial4.html
    /// </remarks>
    public struct NoiseParams {
        public static NoiseParams Terrain(int seed, int width, int height, NoiseGradients gr) => new NoiseParams {
            Seed = seed,
            Map = NoiseMaps.Planar,
            seamless = true,
            Size = new Size(width, height),
            //Scale = new NoiseScale { scale = 1f, bias = -0.8f },

            //frequency = FilterModule.DEFAULT_FREQUENCY,
            //lacunarity = FilterModule.DEFAULT_LACUNARITY,
            //octaveCount = FilterModule.DEFAULT_OCTAVE_COUNT,
            //offset = FilterModule.DEFAULT_OFFSET,
            //gain = FilterModule.DEFAULT_GAIN,

            Gradient = gr
        };

        public int Seed;

        public bool HasScale => Scale.scale != 0 && Scale.bias != 0;

        public NoiseGradients Gradient;
        public NoiseScale Scale;
        public NoiseMaps Map;
        public bool seamless;
        public Size Size;
        /// <summary>
        /// It’s sometimes useful to think of wavelength, which is the inverse of frequency. 
        /// Doubling the frequency makes everything half the size. Doubling the wavelength makes everything twice the size. 
        /// The wavelength is a distance, measured in pixels or tiles or meters or whatever you use for your maps.
        /// It’s related to frequency: wavelength = map_size / frequency.
        /// </summary>
        public float frequency;
        public float lacunarity;
        /// <summary>
        /// To make the height map more interesting we’re going add noise at different frequencies:
        /// </summary>
        /// <example>
        /// in this case we have 3 octaves, 3 times to invoke noise with diff frequencies
        /// elevation[y][x] = 
        ///          1 * noise(frequency * nx, frequency * ny) 
        ///     +  0.5 * noise(frequency*2 * nx, 2 * ny) 
        ///     + 0.25 * noise(frequency*4 * nx, 2 * ny);
        /// </example>
        public float octaveCount;
        public float offset;
        public float gain;

        //FilterModule.DEFAULT_SPECTRAL_EXPONENT
    }
    public struct NoiseScale {
        public float scale;//1f
        public float bias;// -0.8f
    }

    public enum NoiseMaps {
        Spherical, Cylindrical, Planar
    }
    public enum NoiseGradients {
        Grayscale,
        Terrain
    }

    /*
    public class LibNoiseAdapter : ILabNoise {
        readonly ImprovedPerlin improvedPerlin;
        readonly FilterModule fModule;
        readonly PrimitiveModule pModule;

        IModule3D finalModule;
        NoiseMapBuilder projection;
        NoiseMap noiseMap;
        NoiseParams _params;

        public LibNoiseAdapter(NoiseParams _params) {
            this._params = _params;
            var quality = PrimitiveModule.DefaultQuality;

            pModule = new SimplexPerlin();//https://en.wikipedia.org/wiki/Perlin_noise

            pModule.Quality = NoiseQuality.Best;
            pModule.Seed = _params.Seed;

            fModule = new SumFractal();// MultiFractal();

            fModule.Frequency = _params.frequency;
            fModule.Lacunarity = _params.lacunarity;
            fModule.OctaveCount = _params.octaveCount;
            fModule.Offset = _params.offset;
            fModule.Gain = _params.gain;
            fModule.Primitive3D = (IModule3D)pModule;

            if (_params.HasScale) {
                // Used to show the difference with our gradient color (-1 + 1)
                finalModule = new ScaleBias(fModule, _params.Scale.scale, _params.Scale.bias);
            } else {
                finalModule = (IModule3D)fModule;
            }

            switch (_params.Map) {
                case NoiseMaps.Planar:
                    float bound = 2f;
                    projection = new NoiseMapBuilderPlane(bound, bound * 2, bound, bound * 2, _params.seamless);
                    break;
            }
            noiseMap = new NoiseMap();


            projection.SetSize(_params.Size.Width, _params.Size.Height);
            projection.SourceModule = finalModule;
            projection.NoiseMap = noiseMap;
        }

        public float GetHeigthValue(int x, int y) {
            return noiseMap.GetValue(x, y);
        }
        public MemoryStream GetBitmap() {
            return memory;
        }
        MemoryStream memory;
        public void Render() {
            projection.Build();

            var renderer = new ImageRenderer();
            renderer.NoiseMap = noiseMap;
            switch (_params.Gradient) {
                case NoiseGradients.Terrain:
                    renderer.Gradient = GradientColors.Terrain;
                    break;
                case NoiseGradients.Grayscale:
                    renderer.Gradient = GradientColors.Grayscale;
                    break;
            }
            renderer.LightBrightness = 2;
            renderer.LightContrast = 8;
            //renderer.LightEnabled = true;

            var bmpAdaptater = new BitmapAdaptater(_params.Size.Width, _params.Size.Height);
            renderer.Image = bmpAdaptater;

            renderer.Render();


            var btm = bmpAdaptater.Bitmap;

            // btm.Save(@"C:\Storage\projects\sv\3d\d3dlab\D3DLab.Wpf.Engine.App\Resources\terrain\test.bmp");

            memory = new MemoryStream();
            btm.Save(memory, ImageFormat.Bmp);
        }

    }
    */

    /*
     * TO TEST 
     private void noiseButton_Click(object sender, EventArgs e)
{
    PerlinNoise perlinNoise = new PerlinNoise(99);
    Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
    double widthDivisor = 1 / (double)pictureBox.Width;
    double heightDivisor = 1 / (double)pictureBox.Height;
    bitmap.\(
        (point, color) =>
        {
            // Note that the result from the noise function is in the range -1 to 1, but I want it in the range of 0 to 1
            // that's the reason of the strange code
            double v =
                // First octave
                (perlinNoise.Noise(2 * point.X * widthDivisor, 2 * point.Y * heightDivisor, -0.5) + 1) / 2 * 0.7 +
                // Second octave
                (perlinNoise.Noise(4 * point.X * widthDivisor, 4 * point.Y * heightDivisor, 0) + 1) / 2 * 0.2 +
                // Third octave
                (perlinNoise.Noise(8 * point.X * widthDivisor, 8 * point.Y * heightDivisor, +0.5) + 1) / 2 * 0.1;
 
            v = Math.Min(1, Math.Max(0, v));
            byte b = (byte)(v * 255);
            return Color.FromArgb(b, b, b);
        });
    pictureBox.Image = bitmap;
}
         */
    public class PerlinNoise {
        private const int GradientSizeTable = 256;
        private readonly Random _random;
        private readonly double[] _gradients = new double[GradientSizeTable * 3];
        /* Borrowed from Darwyn Peachey (see references above).
           The gradient table is indexed with an XYZ triplet, which is first turned
           into a single random index using a lookup in this table. The table simply
           contains all numbers in [0..255] in random order. */
        private readonly byte[] _perm = new byte[] {
              225,155,210,108,175,199,221,144,203,116, 70,213, 69,158, 33,252,
                5, 82,173,133,222,139,174, 27,  9, 71, 90,246, 75,130, 91,191,
              169,138,  2,151,194,235, 81,  7, 25,113,228,159,205,253,134,142,
              248, 65,224,217, 22,121,229, 63, 89,103, 96,104,156, 17,201,129,
               36,  8,165,110,237,117,231, 56,132,211,152, 20,181,111,239,218,
              170,163, 51,172,157, 47, 80,212,176,250, 87, 49, 99,242,136,189,
              162,115, 44, 43,124, 94,150, 16,141,247, 32, 10,198,223,255, 72,
               53,131, 84, 57,220,197, 58, 50,208, 11,241, 28,  3,192, 62,202,
               18,215,153, 24, 76, 41, 15,179, 39, 46, 55,  6,128,167, 23,188,
              106, 34,187,140,164, 73,112,182,244,195,227, 13, 35, 77,196,185,
               26,200,226,119, 31,123,168,125,249, 68,183,230,177,135,160,180,
               12,  1,243,148,102,166, 38,238,251, 37,240,126, 64, 74,161, 40,
              184,149,171,178,101, 66, 29, 59,146, 61,254,107, 42, 86,154,  4,
              236,232,120, 21,233,209, 45, 98,193,114, 78, 19,206, 14,118,127,
               48, 79,147, 85, 30,207,219, 54, 88,234,190,122, 95, 67,143,109,
              137,214,145, 93, 92,100,245,  0,216,186, 60, 83,105, 97,204, 52};

        public PerlinNoise(int seed) {
            _random = new Random(seed);
            InitGradients();
        }

        public double Noise(double x, double y, double z) {
            /* The main noise function. Looks up the pseudorandom gradients at the nearest
               lattice points, dots them with the input vector, and interpolates the
               results to produce a single output value in [0, 1] range. */

            int ix = (int)Math.Floor(x);
            double fx0 = x - ix;
            double fx1 = fx0 - 1;
            double wx = Smooth(fx0);

            int iy = (int)Math.Floor(y);
            double fy0 = y - iy;
            double fy1 = fy0 - 1;
            double wy = Smooth(fy0);

            int iz = (int)Math.Floor(z);
            double fz0 = z - iz;
            double fz1 = fz0 - 1;
            double wz = Smooth(fz0);

            double vx0 = Lattice(ix, iy, iz, fx0, fy0, fz0);
            double vx1 = Lattice(ix + 1, iy, iz, fx1, fy0, fz0);
            double vy0 = Lerp(wx, vx0, vx1);

            vx0 = Lattice(ix, iy + 1, iz, fx0, fy1, fz0);
            vx1 = Lattice(ix + 1, iy + 1, iz, fx1, fy1, fz0);
            double vy1 = Lerp(wx, vx0, vx1);

            double vz0 = Lerp(wy, vy0, vy1);

            vx0 = Lattice(ix, iy, iz + 1, fx0, fy0, fz1);
            vx1 = Lattice(ix + 1, iy, iz + 1, fx1, fy0, fz1);
            vy0 = Lerp(wx, vx0, vx1);

            vx0 = Lattice(ix, iy + 1, iz + 1, fx0, fy1, fz1);
            vx1 = Lattice(ix + 1, iy + 1, iz + 1, fx1, fy1, fz1);
            vy1 = Lerp(wx, vx0, vx1);

            double vz1 = Lerp(wy, vy0, vy1);
            return Lerp(wz, vz0, vz1);
        }

        private void InitGradients() {
            for (int i = 0; i < GradientSizeTable; i++) {
                double z = 1f - 2f * _random.NextDouble();
                double r = Math.Sqrt(1f - z * z);
                double theta = 2 * Math.PI * _random.NextDouble();
                _gradients[i * 3] = r * Math.Cos(theta);
                _gradients[i * 3 + 1] = r * Math.Sin(theta);
                _gradients[i * 3 + 2] = z;
            }
        }

        private int Permutate(int x) {
            const int mask = GradientSizeTable - 1;
            return _perm[x & mask];
        }

        private int Index(int ix, int iy, int iz) {
            // Turn an XYZ triplet into a single gradient table index.
            return Permutate(ix + Permutate(iy + Permutate(iz)));
        }

        private double Lattice(int ix, int iy, int iz, double fx, double fy, double fz) {
            // Look up a random gradient at [ix,iy,iz] and dot it with the [fx,fy,fz] vector.
            int index = Index(ix, iy, iz);
            int g = index * 3;
            return _gradients[g] * fx + _gradients[g + 1] * fy + _gradients[g + 2] * fz;
        }

        private double Lerp(double t, double value0, double value1) {
            // Simple linear interpolation.
            return value0 + t * (value1 - value0);
        }

        private double Smooth(double x) {
            /* Smoothing curve. This is used to calculate interpolants so that the noise
              doesn't look blocky when the frequency is low. */
            return x * x * (3 - 2 * x);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// https://www.youtube.com/watch?v=E1B4UoSQMFw
    /// <see cref="https://en.wikipedia.org/wiki/L-system"/>
    public class LSystem {

    }

    /*
     * https://www.redblobgames.com/articles/noise/2d/#spectrum
     * 
     */



   
}
