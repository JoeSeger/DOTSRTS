namespace DOTSRTS.Utilities
{
    public static class Hash128Converter
    {
        public static Unity.Entities.Hash128 ConvertToEntitiesHash128(UnityEngine.Hash128 unityEngineHash)
        {
            // Convert UnityEngine.Hash128 to string
            string hashString = unityEngineHash.ToString();

            // Parse the string back to Unity.Entities.Hash128
            return new Unity.Entities.Hash128(hashString);
        }

        public static UnityEngine.Hash128 ConvertToUnityEngineHash128(Unity.Entities.Hash128 entitiesHash)
        {
            // Combine the four 32-bit integers back into two 64-bit values
            ulong u64_0 = ((ulong)entitiesHash.Value.y << 32) | entitiesHash.Value.x;
            ulong u64_1 = ((ulong)entitiesHash.Value.w << 32) | entitiesHash.Value.z;

            // Create and return a UnityEngine.Hash128 using the combined values
            return new UnityEngine.Hash128(u64_0, u64_1);
        }
    }
}