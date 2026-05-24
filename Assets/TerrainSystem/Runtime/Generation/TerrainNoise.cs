using UnityEngine;

namespace TerrainSystem
{
    public static class TerrainNoise
    {
        /// <summary>
        /// Samples fractional Brownian motion (fBm) noise at world position (wx, wz).
        /// Returns a value in [0, 1].
        /// </summary>
        public static float Sample(float wx, float wz, TerrainSettings s)
        {
            System.Random prng = new System.Random(s.noiseSeed);
            Vector2[] octaveOffsets = new Vector2[s.octaves];
            for (int i = 0; i < s.octaves; i++)
            {
                float ox = prng.Next(-100000, 100000) + s.noiseOffset.x;
                float oz = prng.Next(-100000, 100000) + s.noiseOffset.y;
                octaveOffsets[i] = new Vector2(ox, oz);
            }

            float amplitude = 1f;
            float frequency = 1f;
            float noiseHeight = 0f;
            float maxPossible = 0f;

            for (int i = 0; i < s.octaves; i++)
            {
                float sx = (wx * s.noiseScale * frequency) + octaveOffsets[i].x;
                float sz = (wz * s.noiseScale * frequency) + octaveOffsets[i].y;
                float perlinValue = Mathf.PerlinNoise(sx, sz) * 2f - 1f; // remap to [-1, 1]
                noiseHeight += perlinValue * amplitude;
                maxPossible += amplitude;

                amplitude *= s.persistence;
                frequency *= s.lacunarity;
            }

            // Normalize to [0, 1]
            return Mathf.InverseLerp(-maxPossible, maxPossible, noiseHeight);
        }
    }
}
