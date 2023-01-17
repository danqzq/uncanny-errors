using UnityEngine;

namespace Dan.UncannyErrors.Scripts
{
    [System.Serializable]
    public struct Phase
    {
        public int minErrorCount;
        public Texture2D texture;
        public AudioClip clip;
    }
}