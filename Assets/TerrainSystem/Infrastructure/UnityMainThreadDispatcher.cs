using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainSystem.Infrastructure
{
  public class UnityMainThreadDispatcher : MonoBehaviour
  {
    private static readonly Queue<Action> actions = new();

    public static void Enqueue(Action action)
    {
      lock (actions)
      {
        actions.Enqueue(action);
      }
    }

    private void Update()
    {
      lock (actions)
      {
        while (actions.Count > 0)
          actions.Dequeue()?.Invoke();
      }
    }
  }
}