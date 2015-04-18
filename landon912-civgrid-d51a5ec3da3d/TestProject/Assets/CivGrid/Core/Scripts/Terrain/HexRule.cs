using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CivGrid;

namespace CivGrid
{
    /// <summary>
    /// Contains all possible tiles and features for the improvement or resource to spawn upon.
    /// </summary>
    [System.Serializable]
    public class HexRule
    {
        /// <summary>
        /// Array of tiles that the resource or improvement can spawn upon
        /// </summary>
        /// <remarks>
        /// Tiles are represented by their index. Zero being the first in the tile array.
        /// </remarks>
        public int[] possibleTiles;
        /// <summary>
        /// Array of Features that the resource or improvement can spawn upon
        /// </summary>
        public Feature[] possibleFeatures;

        public int[] ValidateData(TileManager tileManager)
        {
            List<int> tiles = possibleTiles.ToList();
            foreach(int t in tiles)
            {
                if (tileManager.tiles[t] == null)
                {
                    tiles.Remove(t);
                }
            }
            return tiles.ToArray();
        }

        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="possibleTiles">Index position of tiles that we can spawn on</param>
        /// <param name="possibleFeatures">Features that we can spawn on</param>
        public HexRule(int[] possibleTiles, Feature[] possibleFeatures)
        {
            this.possibleTiles = possibleTiles;
            this.possibleFeatures = possibleFeatures;
        }
    }

    /// <summary>
    /// Contains testing logic for HexRules.
    /// </summary>
    public static class RuleTest
    {
        /// <summary>
        /// Checks all rules in the rule list.
        /// </summary>
        /// <param name="hex">Hex to compare the rules upon</param>
        /// <param name="rule">Rules to check</param>
        /// <param name="tileManager">The scene tile manager</param>
        /// <returns>If the hex passed the tests</returns>
        public static bool Test(Hex hex, HexRule rule, TileManager tileManager)
        {
            //check tile rules
            for (int i = 0; i < rule.possibleTiles.Length; i++)
            {
                //if the hex's tile type is in the list of possible tiles, break out of the loop and check features
                if (TestRule(hex, tileManager.tiles[rule.possibleTiles[i]]) == true) { break; }
                //the hex's tile type was not in the list of possible tiles, return false 
                if (i == (rule.possibleTiles.Length - 1)) { return false; }
            }
            //check feature rules
            for (int i = 0; i < rule.possibleFeatures.Length; i++)
            {
                //if hex's feature type is in the list of possible features, return true since both rules have been passed
                if (TestRule(hex, rule.possibleFeatures[i]) == true) { return true; }
                //the hex's feature type was not in the list of possible features, return false 
                if (i == (rule.possibleFeatures.Length - 1)) { return false; }
            }
            //unreachable mandatory code because c# is funky
            return false;
        }

        /// <summary>
        /// Check if the hex's tile type is the provided tile
        /// </summary>
        /// <param name="hex">Hex to compare to the tile</param>
        /// <param name="tile">Tile to compare to the hex</param>
        /// <returns></returns>
        private static bool TestRule(Hex hex, Tile tile)
        {
            if (hex.terrainType == tile) { return true; } else { return false; }
        }

        /// <summary>
        /// Check if the hex's feature type is the provided feature
        /// </summary>
        /// <param name="hex">Hex to compare to the feature</param>
        /// <param name="feature">Feature to compare to the hex</param>
        /// <returns></returns>
        private static bool TestRule(Hex hex, Feature feature)
        {
            if (hex.terrainFeature == feature) { return true; } else { return false; }
        }
    }
}