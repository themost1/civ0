using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CivGrid;

namespace CivGrid
{
    /// <summary>
    /// Contains all possible improvements.
    /// Handles the addition and removal of these improvements upon hexagons.
    /// </summary>
    public class ImprovementManager : MonoBehaviour
    {
        /// <summary>
        /// Possible improvements to spawn.
        /// </summary>
        public List<Improvement> improvements;
        /// <summary>
        /// Names of the possible improvements to spawn.
        /// </summary>
        /// <remarks>
        /// The index is the same for the respective improvement.
        /// </remarks>
        public string[] improvementNames;

        //internal array for speed
        private Improvement[] internalImprovements; 

        //cached managers
        internal ResourceManager resourceManager;
        internal TileManager tileManager;

        /// <summary>
        /// Sets up the improvement manager.
        /// Caches all needed values.
        /// </summary>
        public void SetUp()
        {
            //insert default "None" improvement into the improvement array
            improvements.Insert(0, new Improvement("None", null, null, false, null));

            //set internal array
            internalImprovements = improvements.ToArray();

            //cache managers
            resourceManager = GetComponent<ResourceManager>();
            tileManager = GetComponent<TileManager>();

            //instatiate the improvement name array
            if (improvementNames == null)
            {
                UpdateImprovementNames();
            }
        }

        public void DeleteDependencies(int index)
        {
            foreach (Improvement i in improvements)
            {
                List<int> list = i.rule.possibleTiles.ToList<int>();
                list.Remove(index);
				
				i.rule.possibleTiles = list.ToArray();
            }
        }

        /// <summary>
        /// Creates the improvement GameObject and switches the hex texture.
        /// </summary>
        /// <param name="hex">Hex to create the improvement on</param>
        /// <param name="i">Improvement to add</param>
        private void InitiateImprovementsOnHexs(Hex hex, Improvement i)
        {
            //get the parent chunk object; this is where we will parent the improvement objects to
            GameObject resourceHolder = hex.parentChunk.gameObject;
            if (resourceHolder == null) { Debug.LogError("Could not find the resource holder!"); }

            //remove current improvements
            Destroy(hex.iObject);

            //remove current resource gameobjects
            Destroy(hex.rObject);

            //switch the hex's texture to this improvement's ground texture
            hex.ChangeTextureToImprovement();


            //spawn gameObject if there is a mesh to spawn
            if (i.meshToSpawn != null)
            {
                float y = (hex.worldPosition.y + hex.hexExt.y) - ((hex.hexExt.y) / 5f); if (y == 0) { y -= ((hex.worldPosition.y + hex.hexExt.y) / Random.Range(4, 8)); }
                GameObject holder = new GameObject(i.name + " at " + hex.AxialCoordinates, typeof(MeshFilter), typeof(MeshRenderer));
                holder.GetComponent<MeshFilter>().mesh = hex.currentImprovement.meshToSpawn;
                holder.transform.position = new Vector3((hex.worldPosition.x + hex.hexCenter.x + Random.Range(-0.2f, 0.2f)), y, (hex.worldPosition.z + hex.hexCenter.z + Random.Range(-0.2f, 0.2f)));
                holder.transform.rotation = Quaternion.identity;
                holder.GetComponent<Renderer>().material.mainTexture = i.meshTexture;
                holder.transform.parent = hex.parentChunk.transform;

                hex.iObject = holder;
            }
        }

        /// <summary>
        /// Adds an improvement to the improvement array.
        /// </summary>
        /// <param name="i">Improvement to add</param>
        /// <remarks>
        /// This method is safe to use after world generation. However, the improvement must be present on
        /// a world load to be safe./n 
        /// For example if you add an improvment and the user adds it a tile. Saves the game. And then loads it,
        /// you must re-add the same improvement at the same index before world generation for it to safely load.
        /// </remarks>
        public void AddImprovement(Improvement i)
        {
            improvements.Add(i);
            internalImprovements = improvements.ToArray();
            UpdateImprovementNames();
        }

        /// <summary>
        /// Adds an improvement to the improvement array at the provided index.
        /// </summary>
        /// <param name="i">Improvement to add</param>
        /// <param name="index">Index in which to add the improvement</param>
        /// <remarks>
        /// This method is safe to use after world generation. However, the improvement must be present on
        /// a world load to be safe./n 
        /// For example if you add an improvment and the user adds it a tile. Saves the game. And then loads it,
        /// you must re-add the same improvement at the same index before world generation for it to safely load.
        /// </remarks>
        public void AddImprovementAtIndex(Improvement i, int index)
        {
            improvements.Insert(index, i);
            internalImprovements = improvements.ToArray();
            UpdateImprovementNames();
        }

        /// <summary>
        /// Removes an improvement from the improvement array.
        /// </summary>
        /// <param name="i">Improvement to remove</param>
        /// <remarks>
        /// Removing a improvement that is referenced elsewhere will cause null reference errors. Only use this
        /// method if you are personally managing the specific improvement memory lifetime.
        /// </remarks>
        public void DeleteImprovement(Improvement i)
        {
            improvements.Remove(i);
            internalImprovements = improvements.ToArray();
            UpdateImprovementNames();
        }

        /// <summary>
        /// Attempts to return an improvement from a provided name.
        /// </summary>
        /// <param name="name">The name of the improvement to look for</param>
        /// <returns>The improvement with the name provided; null if not found</returns>
        /// <example>
        /// The following example adds an improvement, then retrieves it by it's name. Using <see cref="AddImprovement(Improvement)"/> is
        /// not encouraged. Add improvements in the inspector.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///    ImprovementManager improvementManager;
        ///
        ///    void Start()
        ///    {
        ///        improvementManager = GameObject.FindObjectOfType&lt;ImprovementManager&gt;();
        ///
        ///        //this method is not encouraged, used as a specific example and not best practice. Add improvements in the
        ///        //inspector instead.
        ///        improvementManager.AddImprovement(new Improvement("Test", null, null, false, new HexRule(null, null)));
        ///
        ///        Improvement improvement = improvementManager.TryGetImprovement("Test");
        ///
        ///        Debug.Log(improvement.name);
        ///    }
        /// }
        ///
        /// //Output:
        /// //"Test"
        /// </code>
        /// </example>
        public Improvement TryGetImprovement(string name)
        {
            //cycle through all improvements
            foreach(Improvement i in internalImprovements)
            {
                //if the improvement shares the name; return it
                if(i.name == name)
                {
                    return i;
                }
            }
            //not found; return null
            return null;
        }

        /// <summary>
        /// Creates or updates an array of the improvement names. <see cref="improvementNames"/>
        /// </summary>
        public void UpdateImprovementNames()
        {
            //only update if there are improvements
            if (internalImprovements != null && internalImprovements.Length > 0)
            {
                //instatiate improvement names array
                improvementNames = new string[internalImprovements.Length];

                //loop through all improvements
                for (int i = 0; i < internalImprovements.Length; i++)
                {
                    //assign each name into the array
                    improvementNames[i] = internalImprovements[i].name;
                }
            }
        }

        /// <summary>
        /// Adds improvement to specified hex if it meets the rule requirements; slower than passing in an inprovement index.
        /// </summary>
        /// <param name="hex">Hex to attempt to add the improvement upon</param>
        /// <param name="improvementName">"Improvement to attempt to add</param>
        /// <example>
        /// The following code tries to add a "Farm" improvement to every tile.
        /// <code>
        /// /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///    WorldManager worldManager;
        ///    ImprovementManager improvementManager;
        ///
        ///    void Start()
        ///    {
        ///        worldManager = GameObject.FindObjectOfType&lt;WorldManager&gt;();
        ///        improvementManager = GameObject.FindObjectOfType&lt;ImprovementManager&gt;();
        ///
        ///        //check again for each hex if it should spawn a resource
        ///        foreach (HexChunk chunk in worldManager.hexChunks)
        ///        {
        ///            foreach (HexInfo hex in chunk.hexArray)
        ///            {
        ///                improvementManager.TestedAddImprovementToTile(hex, "Farm");
        ///            }
        ///        }
        ///    }
        /// }
        /// </code>
        /// </example>
        public void TestedAddImprovementToTile(Hex hex, string improvementName)
        {
            //if it's possible to spawn the improvement according to it's rules
            bool possible = false;

            //gets improvement from it's name
            Improvement improvement = TryGetImprovement(improvementName);

            //runs through the tests and if any return false, we can not spawn the improvement
            if (RuleTest.Test(hex, improvement.rule, tileManager))
            {
                possible = true;
            }
            else
            {
                possible = false;
            }
            
            //spawn the improvement on the tile
            if (possible)
            {
                hex.currentImprovement = improvement;
                hex.parentChunk.worldManager.improvementManager.InitiateImprovementsOnHexs(hex, improvement);
            }
        }

        /// <summary>
        /// Adds improvement to specified hex if it meets the rule requirements.
        /// </summary>
        /// <param name="hex">Hex to attempt to add the improvement upon</param>
        /// <param name="improvement">"Improvement to attempt to add</param>
        /// <example>
        /// The following code makes a new improvement at runtime and then tests it's rules against all tiles.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///   WorldManager worldManager;
        ///    ImprovementManager improvementManager;
        ///
        ///    void Start()
        ///    {
        ///        worldManager = GameObject.FindObjectOfType&lt;WorldManager&gt;();
        ///        improvementManager = GameObject.FindObjectOfType&lt;ImprovementManager&gt;();
        ///
        ///        Improvement improvement = new Improvement("NewImprovement", null, null, true, new HexRule(new int[]{4,5}, new Feature[]{Feature.Flat}));
        ///        improvementManager.AddImprovement(improvement);
        ///
        ///        //check again for each hex if it should spawn a resource
        ///        foreach (HexChunk chunk in worldManager.hexChunks)
        ///        {
        ///           foreach (HexInfo hex in chunk.hexArray)
        ///           {
        ///               improvementManager.TestedAddImprovementToTile(hex, improvement);
        ///           }
        ///        }
        ///    }
        /// }
        /// </code>
        /// </example>
        [System.Obsolete("Use improvementIndex overload; otherwise retrieve [index+1] to return correct improvement")]
        public void TestedAddImprovementToTile(Hex hex, Improvement improvement)
        {
            //if it's possible to spawn the improvement according to it's rules
            bool possible = false;

            //runs through the tests and if any return false, we can not spawn the improvement
            if (RuleTest.Test(hex, improvement.rule, tileManager))
            {
                possible = true;
            }
            else
            {
                possible = false;
            }

            //spawn the improvement on the tile
            if (possible)
            {
                hex.currentImprovement = improvement;
                hex.parentChunk.worldManager.improvementManager.InitiateImprovementsOnHexs(hex, improvement);
            }
        }

        /// <summary>
        /// Adds improvement to specified hex if it meets the rule requirements.
        /// </summary>
        /// <param name="hex">Hex to attempt to add the improvement upon</param>
        /// <param name="improvementIndex">Index of the improvement within the improvement manager to attemp to add</param>
        /// <example>
        /// The following code attempts to add the first improvement to every tile.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///    WorldManager worldManager;
        ///    ImprovementManager improvementManager;
        ///
        ///    void Start()
        ///    {
        ///        worldManager = GameObject.FindObjectOfType&lt;WorldManager&gt;();
        ///        improvementManager = GameObject.FindObjectOfType&lt;ImprovementManager&gt;();
        ///
        ///        //check again for each hex if it should spawn a resource
        ///        foreach (HexChunk chunk in worldManager.hexChunks)
        ///        {
        ///            foreach (HexInfo hex in chunk.hexArray)
        ///            {
        ///                //tries to add first improvement
        ///                improvementManager.TestedAddImprovementToTile(hex, 0);
        ///            }
        ///        }
        ///    }
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// The index system should be based off of the inspector indexes at startup. The automatically generated "None" improvement
        /// is not included in the index numbering.
        /// </remarks>
        public void TestedAddImprovementToTile(Hex hex, int improvementIndex)
        {
            //if it's possible to spawn the improvement according to it's rules
            bool possible = false;

            //gets improvement from it's index
            Improvement improvement = improvements[improvementIndex+1];

            //runs through the tests and if any return false, we can not spawn the improvement
            if (RuleTest.Test(hex, improvement.rule, tileManager))
            {
                possible = true;
            }
            else
            {
                possible = false;
            }

            //spawn the improvement on the tile
            if (possible)
            {
                hex.currentImprovement = improvement;
                hex.parentChunk.worldManager.improvementManager.InitiateImprovementsOnHexs(hex, improvement);
            }
        }

        /// <summary>
        /// Removes the improvement from the specified hex and restores its past state.
        /// </summary>
        /// <param name="hex">Hex to remove all improvements from</param>
        /// <example>
        /// The following code removes all improvements from the map.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///    WorldManager worldManager;
        ///    ImprovementManager improvementManager;
        ///
        ///    void Start()
        ///    {
        ///        worldManager = GameObject.FindObjectOfType&lt;WorldManager&gt;();
        ///        improvementManager = GameObject.FindObjectOfType&lt;ImprovementManager&gt;();
        ///
        ///        //check again for each hex if it should spawn a resource
        ///        foreach (HexChunk chunk in worldManager.hexChunks)
        ///        {
        ///            foreach (HexInfo hex in chunk.hexArray)
        ///            {
        ///                //tries to add first improvement
        ///                improvementManager.RemoveImprovementFromTile(hex);
        ///            }
        ///        }
        ///    }
        /// }
        /// </code>
        /// </example>
        public void RemoveImprovementFromTile(Hex hex)
        {
            if (hex.currentImprovement != null)
            {
                if (hex.currentImprovement.name != "None")
                {
                    //change texture of this hexagon back to its original if it has a resource it will be corrected below
                    hex.ChangeTextureToNormalTile();


                    //destory improvement children
                    Destroy(hex.iObject);

                    //respawn resource model
                    if (hex.currentResource.name != "None")
                    {
                        hex.resourceManager.SpawnResource(hex, hex.currentResource, true);
                    }
                }
            }

            //return "None"
            hex.currentImprovement = improvements[0];
        }
    }

    /// <summary>
    /// Improvement class that contains all values for the base improvement.
    /// </summary>
    [System.Serializable]
    public class Improvement
    {
        /// <summary>
        /// Name of the improvement
        /// </summary>
        public string name;
        /// <summary>
        /// The rules of where the improvement can spawn
        /// </summary>
        public HexRule rule;

        /// <summary>
        /// The improvement mesh to spawn
        /// </summary>
        public Mesh meshToSpawn;
        /// <summary>
        /// The texture for the spawned mesh
        /// </summary>
        public Texture2D meshTexture;
        /// <summary>
        /// Decides if the ground texture is replaced with the improvement specific one in the texture atlas
        /// </summary>
        public bool replaceGroundTexture;

        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="name">Name of the improvement</param>
        /// <param name="meshToSpawn">The improvement mesh to spawn</param>
        /// <param name="meshTexture">The texture for the spawned mesh</param>
        /// <param name="replaceGroundTexture">Decides if the ground texture is replaced with the improvement specific one in the texture atlas</param>
        /// <param name="rule">The rules of where the improvement can spawn</param>
        public Improvement(string name, Mesh meshToSpawn, Texture2D meshTexture, bool replaceGroundTexture, HexRule rule)
        {
            this.name = name;
            this.meshTexture = meshTexture;
            this.meshToSpawn = meshToSpawn;
            this.replaceGroundTexture = replaceGroundTexture;
            this.rule = rule;
        }

        /// <summary>
        /// Blank constrcutor for this class.
        /// </summary>
        public Improvement() { }
    }
}