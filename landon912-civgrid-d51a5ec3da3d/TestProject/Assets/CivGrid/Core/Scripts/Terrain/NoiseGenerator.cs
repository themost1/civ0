using UnityEngine;
using System.Collections;
using CivGrid;
using System.Linq;

namespace CivGrid
{


    /// <summary>
    /// Makes and cleans noise for world generation
    /// </summary>
    public static class NoiseGenerator
    {
        #region SimpleNoise
        /// <summary>
        /// A perlin noise generator
        /// </summary>
        /// <param name="xSize">Amount of tiles in the x-axis of the map </param>
        /// <param name="ySize">Amount of tiles in the y-axis of the map </param>
        /// <param name="noiseScale">Scale of the noise </param>
        /// <returns> TileMap in Texture2D format </returns>
        public static Texture2D PerlinNoise(int xSize, int ySize, float noiseScale)
        {
            //texture to return
            Texture2D tex = new Texture2D(xSize, ySize);

            //loop through all pixels
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    //calculate a modified version of perlin noise and assign its value to the pixel
                    float randomValue = Random.Range(0, 3);
                    float pixelValue = Mathf.PerlinNoise(x * noiseScale + randomValue, y * noiseScale + randomValue) * 1.3f;

                    pixelValue = Mathf.RoundToInt(pixelValue);
                    tex.SetPixel(x, y, new Color(pixelValue, pixelValue, pixelValue, 1));
                }
            }

            //return completed perlin noise texture
            return tex;
        }


        /// <summary>
        /// An inverted version of perlin noise with ocean smoothing
        /// </summary>
        /// <param name="xSize">Amount of tiles in the x-axis of the map</param>
        /// <param name="ySize">Amount of tiles in the y-axis of the map</param>
        /// <param name="noiseScale">Scale of the noise</param>
        /// <param name="smoothingCutoff">Amount of tiles needed to remain water/ground</param>
        /// <returns>A TileMap in Texture2D format</returns>
        public static Texture2D SmoothPerlinNoise(int xSize, int ySize, float noiseScale, int smoothingCutoff)
        {
            //texture to return
            Texture2D tex = new Texture2D(xSize, ySize);

            //loop through all the pixels
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    //calculate a smooth modified version of perlin noise and assign the value to the pixel
                    float randomValue = Random.value;
                    float pixelValue = Mathf.PerlinNoise(x * noiseScale + randomValue, y * noiseScale + randomValue);

                    if (pixelValue >= 0.5f) { pixelValue *= 1.35f; }

                    pixelValue = StabalizeFloat(pixelValue);

                    tex.SetPixel(x, y, new Color(pixelValue, pixelValue, pixelValue, 1));
                }
            }

            //smooth the land vs water
            CleanWater(tex, noiseScale, smoothingCutoff);

            //return perlin noise texture
            return tex;
        }

        public static Texture2D SmoothPerlinNoiseRaw(int xSize, int ySize, float noiseScale, int octaves)
        {
            //texture to return
            Texture2D tex = new Texture2D(xSize, ySize);

            float[][,] colors = new float[octaves][,];

            for (int i = 0; i < octaves; i++)
            {
                colors[i] = new float[xSize, ySize];
            }

            float multiplier = 1;


            for (int o = 0; o < octaves; o++)
            {
                multiplier *= Random.Range(0.2f, 1.5f);


                //loop through all the pixels
                for (int x = 0; x < xSize; x++)
                {
                    for (int y = 0; y < ySize; y++)
                    {
                        //calculate a smooth modified version of perlin noise and assign the value to the pixel
                        float randomValue = Random.value;
                        float pixelValue = Mathf.PerlinNoise(x * noiseScale * multiplier * +randomValue, y * noiseScale * multiplier + randomValue);

                        colors[o][x, y] = pixelValue;
                    }
                }
            }

            float value;
            //loop through all the pixels
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    value = colors[0][x, y];
                    for (int o = 0; o <= octaves; o++)
                    {
                        value *= colors[0][x, y];
                        value /= 2;
                    }
                    tex.SetPixel(x, y, new Color(value, value, value, 1));
                }
            }


            //smooth the land vs water
            //CleanWater(tex, noiseScale, 3);

            //return perlin noise texture
            return tex;
        }

        /// <summary>
        /// Smooths perlin noise to generate more realistic and smooth terrain
        /// </summary>
        /// <param name="texture">Texture to smooth</param>
        /// <param name="noiseScale">Noise scale you used to generate the texture</param>
        /// <param name="smoothingCutoff">The number of like tiles surronding a pixel needed in order for it to remain the original tupe</param>
        private static void CleanWater(Texture2D texture, float noiseScale, int smoothingCutoff)
        {
            //loop through all pixels in the texture
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    //get the pixels around this pixel
                    float[] surrondingTiles = Utility.GetSurrondingPixels(texture, x, y);

                    //WATER
                    if (texture.GetPixel(x, y).r == 0)
                    {
                        //amount of water tiles around this tile
                        int surrondingWater = 0;

                        foreach (float tile in surrondingTiles)
                        {
                            if (tile.Equals(0f))
                            {
                                surrondingWater += 1;
                            }
                        }

                        //not enough water around; set to land
                        if (surrondingWater < smoothingCutoff)
                        {
                            //generate noise for the pixel we are setting to land
                            float randomValue = Random.value;
                            float pixelValue = Mathf.PerlinNoise(x * noiseScale + randomValue, y * noiseScale + randomValue);
                            pixelValue = StabalizeFloat(pixelValue);

                            if (pixelValue < 0.5)
                            {
                                pixelValue = 0.5f;
                            }

                            texture.SetPixel(x, y, new Color(pixelValue, pixelValue, pixelValue));
                        }
                    }
                    //LAND
                    else
                    {
                        //amount of water tiles around this tile
                        int surrondingLand = 0;

                        foreach (float tile in surrondingTiles)
                        {
                            if (tile > 0f)
                            {
                                surrondingLand += 1;
                            }
                        }

                        //not enough land around; set to water
                        if (surrondingLand < smoothingCutoff)
                        {
                            texture.SetPixel(x, y, new Color(0f, 0f, 0f));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Uses internal logic to round floats to usable values(0,0.5,0.8,1)
        /// </summary>
        /// <param name="f">float to round</param>
        /// <returns>The rounded float</returns>
        public static float StabalizeFloat(float f)
        {
            //round to water
            if (f < 0.5f)
            {
                f = 0;
                return f;
            }
            //round to land
            else if (f >= 0.5 && f <= 1)
            {
                f = 0.5f;
                return f;
            }
            //round to hill
            else if (f > 1f && f < 1.15f)
            {
                f = 0.8f;
                return f;
            }
            //round to mountain
            else if (f >= 1.15f) { f = 1f; }
            else { Debug.LogError("Rounding failed inverting to 0"); f = 0; }

            return f;
        }

        /// <summary>
        /// Overlays a texture with perlin noise; formated for mountain generation
        /// </summary>
        /// <param name="texture">Texture to add perlin noise onto</param>
        /// <param name="position">Pixel position to read the perlin noise from</param>
        /// <param name="noiseScale">Controls amount of possible change from pixel-to-pixel</param>
        /// <param name="noiseSize">Scales the value of the noise pixel</param>
        /// <param name="finalSize">Scales the value of the final pixel</param>
        /// <param name="maxHeight">Maximum height the final pixel can be</param>
        /// <param name="ignoreBlack">Avoids adding noise to fully black pixels of the source texture</param>
        /// <param name="noiseFalloff">Whether or not to use smooth noise falloff</param>
        /// <returns></returns>
        public static Texture2D RandomOverlay(Texture2D texture, float position, float noiseScale, float noiseSize, float finalSize, float maxHeight, bool ignoreBlack, bool noiseFalloff)
        {
            //shifts the source over
            position *= Random.Range(0.2f, 5f);

            //creates the return texture
            Texture2D returnTexture = new Texture2D(texture.width, texture.height);

            //loop through all pixels in the source texture
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    //get the pixel from the source texture
                    float pixelColor = texture.GetPixel(x, y).grayscale;

                    float noiseValue;

                    //overlay noise upon the pixel
                    if (ignoreBlack == false || pixelColor > 0.05f)
                    {
                        if (noiseFalloff)
                        {
                            noiseValue = (Mathf.PerlinNoise(x * noiseScale + position, y * noiseScale + position) * noiseSize) * (pixelColor * 2);
                        }
                        else
                        {
                            noiseValue = Mathf.PerlinNoise(x * noiseScale + position, y * noiseScale + position) * noiseSize;
                        }
                    }
                    //no overlay noise
                    else
                    {
                        noiseValue = 0;
                    }

                    //add the base pixel and the noise value to make a combined pixel color
                    float finalValue = pixelColor + noiseValue;
                    finalValue = Mathf.Clamp(finalValue, 0, maxHeight);
                    finalValue *= finalSize;
                    Color totalColor = new Color(finalValue, finalValue, finalValue, 1);

                    returnTexture.SetPixel(x, y, totalColor);
                }
            }

            //return the source texture with noise overlay
            return returnTexture;
        }
        #endregion


        #region RealNoise

        private static System.Random _random = new System.Random();
        private static int[] _permutation;

        private static Vector2[] _gradients;

        static NoiseGenerator()
        {
            CalculatePermutation(out _permutation);
            CalculateGradients(out _gradients);
        }

        private static void CalculatePermutation(out int[] p)
        {
            p = Enumerable.Range(0, 256).ToArray();

            /// shuffle the array
            for (var i = 0; i < p.Length; i++)
            {
                var source = _random.Next(p.Length);

                var t = p[i];
                p[i] = p[source];
                p[source] = t;
            }
        }

        /// <summary>
        /// generate a new permutation.
        /// </summary>
        public static void Reseed()
        {
            CalculatePermutation(out _permutation);
        }

        private static void CalculateGradients(out Vector2[] grad)
        {
            grad = new Vector2[256];

            for (var i = 0; i < grad.Length; i++)
            {
                Vector2 gradient;

                do
                {
                    gradient = new Vector2((float)(_random.NextDouble() * 2 - 1), (float)(_random.NextDouble() * 2 - 1));
                }
                while (gradient.sqrMagnitude >= 1);

                gradient.Normalize();

                grad[i] = gradient;
            }

        }

        private static float Drop(float t)
        {
            t = Mathf.Abs(t);
            return 1f - t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Q(float u, float v)
        {
            return Drop(u) * Drop(v);
        }

        public static float Noise(float x, float y)
        {
            var cell = new Vector2((float)Mathf.Floor(x), (float)Mathf.Floor(y));

            var total = 0f;

            var corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

            foreach (var n in corners)
            {
                var ij = cell + n;
                var uv = new Vector2(x - ij.x, y - ij.y);

                var index = _permutation[(int)ij.x % _permutation.Length];
                index = _permutation[(index + (int)ij.y) % _permutation.Length];

                var grad = _gradients[index % _gradients.Length];

                total += Q(uv.x, uv.y) * Vector2.Dot(grad, uv);
            }

            return Mathf.Max(Mathf.Min(total, 1f), -1f);
        }

        public static Texture2D PerlinNoiseRaw(int width, int height, int octaves, float frequency, float amplitude)
        {
            var data = new float[width * height];

            Texture2D noiseTexture = new Texture2D(width, height);

            /// track min and max noise value. Used to normalize the result to the 0 to 1.0 range.
            var min = float.MaxValue;
            var max = float.MinValue;

            /// rebuild the permutation table to get a different noise pattern. 
            /// Leave this out if you want to play with changing the number of octaves while 
            /// maintaining the same overall pattern.
            NoiseGenerator.Reseed();

            //frequency = 0.5f;
            //amplitude = 1f;

            for (var octave = 0; octave < octaves; octave++)
            {
                /// parallel loop - easy and fast.
                for (int offset = 0; offset < width * height; offset++)
                {

                    var i = offset % width;
                    var j = offset / width;
                    var noise = NoiseGenerator.Noise(i * frequency * 1f / width, j * frequency * 1f / height);
                    noise = data[j * width + i] += noise * amplitude;

                    min = Mathf.Min(min, noise);
                    max = Mathf.Max(max, noise);

                }


                frequency *= 2;
                amplitude /= 2;
            }

            var colors = data.Select(
                (f) =>
                {
                    var norm = (f - min) / (max - min);
                    return new Color(norm, norm, norm, 1);
                }
            ).ToArray();

            noiseTexture.SetPixels(colors);

            return noiseTexture;
        }
        #endregion
    }
}