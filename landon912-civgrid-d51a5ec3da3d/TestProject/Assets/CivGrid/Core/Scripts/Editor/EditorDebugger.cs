using UnityEngine;
using System.Collections;
using UnityEditor;
using CivGrid;

namespace CivGrid.Debugging
{
    public class EditorDebugger
    {
        [MenuItem("CivGrid/Debug/GenerateRainfallMap")]
        private static void TestRainfallGeneration()
        {
            EditorUtility.DisplayProgressBar("Generating Noise", "Please Wait...", 0.5f);

            Texture2D tex = NoiseGenerator.PerlinNoiseRaw(1024, 1024, 8, 0.8f, 1f);

            FileUtility.SaveTexture(tex, "test", true);

            EditorUtility.ClearProgressBar();
        }
    }
}