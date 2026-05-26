namespace TerrainSystem.Runtime.Core
{
    public static class TerrainConstants
    {
        public const string RootFolder = "Assets/TerrainSystem/";
        public const string DataFolder = RootFolder + "Data/";
        public const string MaterialsFolder = DataFolder + "Materials/";
        public const string ProfilesFolder = DataFolder + "Profiles/";
        public const string GeneratedFolder = DataFolder + "Generated/";
        public const string DefaultMaterialName = "Terrain.mat";
        public const string DefaultTerrainDataName = "TerrainData.asset";
        public const string DefaultNoiseSettingsName = "NoiseSettings.asset";
        public const string DefaultShader = "Universal Render Pipeline/Lit";
        public const int MinChunkResolution = 2;
        public const int MaxChunkResolution = 512;
        public const int DefaultChunkResolution = 128;
        public const float DefaultChunkSize = 128f;
        public const float DefaultTerrainHeight = 40f;
        public const int DefaultVisibleRadius = 4;
        public const string ChunkObjectPrefix = "TerrainChunk_";
    }
}