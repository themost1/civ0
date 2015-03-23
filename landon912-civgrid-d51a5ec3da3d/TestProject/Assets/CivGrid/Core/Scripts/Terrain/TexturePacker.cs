using UnityEngine;
using System.Collections;
using CivGrid;

namespace CivGrid
{
    /// <summary>
    /// Packs textures into atlases.
    /// </summary>
    public static class TexturePacker
    {
        /// <summary>
        /// Creates a single texture atlas from the provided source textures.
        /// </summary>
        /// <param name="textures">Textures to combine into one</param>
        /// <param name="textureSize">Size of the texture atlas to create</param>
        /// <param name="rectAreas">Rect locations of each texture</param>
        /// <returns>The created atlased texture</returns>
        /// <example>
        /// This will atlas the two textures, TextureA and TextureB, into one efficient texture map.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        /// 
        /// class TextureTest : MonoBehaviour
        /// {
        ///     Texture2D[] texturesToCombine;
        ///     public Texture2D texture1;
        ///     public Texture2D texture2;
        ///     
        ///     Texture2D atlasedTexture;
        ///     Rect[] rectLocations;
        ///     
        ///     void Start()
        ///     {
        ///         texturesToCombine = new Texture2D[2];
        ///         texturesToCombine[0] = texture1;
        ///         texturesToCombine[1] = texture2;
        ///         
        ///         atlasedTexture = TexturePacker.AtlasTextures(texturesToCombine, 2048, out rectLocations);
        ///     }
        /// }
        /// </code>
        /// </example>
        public static Texture2D AtlasTextures(Texture2D[] textures, int textureSize, out Rect[] rectAreas)
        {
            //creates return texture atlas
            Texture2D packedTexture = new Texture2D(textureSize, textureSize);

            //packs all source textures into one
            rectAreas = packedTexture.PackTextures(textures, 0, textureSize);
            packedTexture.Apply();

            //returns texture atlas
            return packedTexture;
        }
    }
}