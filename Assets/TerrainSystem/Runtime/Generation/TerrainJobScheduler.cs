using System.Collections.Concurrent;
using System.Threading.Tasks;
using TerrainSystem.Infrastructure;
using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Generation;
using UnityEngine;

public class TerrainJobScheduler
{
  private readonly ConcurrentQueue<Vector2Int> queue = new();
  private bool running;

  public void Enqueue(Vector2Int coord)
  {
    queue.Enqueue(coord);
  }

  public void Start(System.Action<Vector2Int, float[]> callback, int resolution, TerrainDataAsset data)
  {
    if (running) return;
    running = true;

    Task.Run(() =>
    {
      while (queue.TryDequeue(out var coord))
      {
        float[] heights = new float[(resolution + 1) * (resolution + 1)];

        for (int z = 0; z <= resolution; z++)
        for (int x = 0; x <= resolution; x++)
        {
          float worldX = x + coord.x * resolution;
          float worldZ = z + coord.y * resolution;

          heights[z * (resolution + 1) + x] =
            TerrainNoiseGenerator.GenerateHeight(
              worldX,
              worldZ,
              data.NoiseScale,
              data.HeightMultiplier,
              data.Seed,
              data.HeightCurve
            );
        }

        // back to main thread
        UnityMainThreadDispatcher.Enqueue(() =>
        {
          callback(coord, heights);
        });
      }

      running = false;
    });
  }
}