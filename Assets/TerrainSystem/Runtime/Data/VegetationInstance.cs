using System;
using UnityEngine;

namespace TerrainSystem.Runtime.Data
{
  [Serializable]
  public class VegetationInstance
  {
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    public int PrefabIndex;
  }
}