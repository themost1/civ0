using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CivGrid;

namespace CivGrid
{

    /// <summary>
    /// Contains all possible resources.
    /// Handles the addition and removal of these resources upon hexagons.
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        /// <summary>
        /// Possible resources to spawn.
        /// </summary>
        public List<Resource> resources;
        /// <summary>
        /// Names of the possible resources to spawn.
        /// </summary>
        /// <remarks>
        /// The index is the same for the respective resource.
        /// </remarks>
        public string[] resourceNames;

        //internal array for speed
        private Resource[] internalResources;

        //cached managers
        internal WorldManager worldManager;
        internal TileManager tileManager;

        /// <summary>
        /// Sets up the resource manager.
        /// Caches all needed values.
        /// </summary>
        internal void SetUp()
        {
            //insert default "None" resource into the resource array
            resources.Insert(0, new Resource("None", 0, 0, null, null, false, null));

            //set internal array
            internalResources = resources.ToArray();

            //cache managers
            tileManager = GetComponent<TileManager>();
            worldManager = GetComponent<WorldManager>();

            //instatiate the improvement name array
            if (resourceNames == null)
            {
                UpdateResourceNames();
            }
        }

        public void DeleteDependencies(int index)
        {
            foreach (Resource r in resources)
            {
				List<int> list = r.rule.possibleTiles.ToList<int>();
                list.Remove(index);
				
				r.rule.possibleTiles = list.ToArray();
            }
        }

        /// <summary>
        /// Called on start-up to make sure all hexs with resources are changed to use their resource texture.
        /// </summary>
        internal void InitiateResourceTexturesOnHexs()
        {
            //loop through all hexs
            foreach (Chunk chunk in worldManager.hexChunks)
            {
                foreach (Hex hex in chunk.hexArray)
                {
                    //has a resource?
                    if (hex.currentResource.name != "None")
                    {
                        //change texture to resource
                        hex.ChangeTextureToResource();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a resource to the resource array.
        /// </summary>
        /// <param name="r">Resource to add</param>
        /// <remarks>
        /// This method should only be used before world generation as it will 
        /// not have an effect unless you re-check for resources on each hex.
        /// </remarks>
        public void AddResource(Resource r)
        {
            resources.Add(r);
            internalResources = resources.ToArray();
            UpdateResourceNames();
        }

        /// <summary>
        /// Adds a resource to the resource array at the provided index.
        /// </summary>
        /// <param name="r">Resource to add</param>
        /// <param name="index">Index in which to add the resource</param>
        /// <remarks>
        /// This method should only be used before world generation as it will 
        /// not have an effect unless you re-check for resources on each hex.
        /// </remarks>
        public void AddResourceAtIndex(Resource r, int index)
        {
            resources.Insert(index, r);
            internalResources = resources.ToArray();
            UpdateResourceNames();
        }

        /// <summary>
        /// Removes a resource from the resource array.
        /// </summary>
        /// <param name="r">Resource to remove</param>
        /// <remarks>
        /// Removing a resource that is referenced elsewhere will cause null reference errors. Only use this
        /// method if you are personally managing the specific resources memory lifetime.
        /// </remarks>
        public void DeleteResource(Resource r)
        {
            resources.Remove(r);
            internalResources = resources.ToArray();
            UpdateResourceNames();
        }

        /// <summary>
        /// Attempts to return a resource from a provided name.
        /// </summary>
        /// <param name="name">The name of the resource to look for</param>
        /// <returns>The improvement with the name provided; null if not found</returns>
        /// <example>
        /// The following example adds a resource, then retrieves it by it's name. Using <see cref="AddResource(Resource)"/> is
        /// not encouraged. Add resources in the inspector.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///    ResourceManager resourceManager;
        ///
        ///    void Start()
        ///    {
        ///        resourceManager = GameObject.FindObjectOfType&lt;ResourceManager&gt;();
        ///
        ///        //this method is not encouraged, used as a specific example and not best practice. Add resources in the
        ///        //inspector instead.
        ///        resourceManager.AddResource(new Resource("Test", 0, 0, null, null, false, new HexRule(null, null)));
        ///
        ///        Resource resource = resourceManager.TryGetResource("Test");
        ///
        ///        Debug.Log(resource.name);
        ///    }
        /// }
        ///
        /// //Output:
        /// //"Test"
        /// </code>
        /// </example>
        public Resource TryGetResource(string name)
        {
            //cycle through all resources
            foreach(Resource r in internalResources)
            {
                //if the resource shares the name; return it
                if(r.name == name)
                {
                    return r;
                }
            }
            //not found; return null
            return null;
        }

        /// <summary>
        /// Creates an array of the resource names. <see cref="resourceNames"/>
        /// </summary>
        public void UpdateResourceNames()
        {
            //only update if there are resources
            if (internalResources != null && internalResources.Length > 0)
            {
                //instatiate resource names array
                resourceNames = new string[internalResources.Length];

                //loop through all resources
                for (int i = 0; i < internalResources.Length; i++)
                {
                    //assign each name into the array
                    resourceNames[i] = internalResources[i].name;
                }
            }
        }

        /// <summary>
        /// Checks if a resource should be spawned on a hexagon.
        /// </summary>
        /// <param name="hex">The hexagon to check</param>
        /// <example>
        /// The following code changes the possible resources and then re-checks each hex for the resource.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///    WorldManager worldManager;
        ///    ResourceManager resourceManager;
        ///
        ///    void Start()
        ///    {
        ///        worldManager = GameObject.FindObjectOfType&lt;WorldManager&gt;();
        ///        resourceManager = GameObject.FindObjectOfType&lt;ResourceManager&gt;();
        ///
        ///        //creates a new resource to possibly spawn
        ///        resourceManager.AddResource(new Resource("SpecialNewResource", 15, 1, null, null, false, new HexRule(new int[] { 3, 4 }, new Feature[] { Feature.Flat })));
        ///
        ///        //check again for each hex if it should spawn a resource
        ///        foreach (HexChunk chunk in worldManager.hexChunks)
        ///        {
        ///            foreach (HexInfo hex in chunk.hexArray)
        ///            {
        ///                resourceManager.CheckForResource(hex); 
        ///            }
        ///        }
        ///    }
        /// }
        /// </code>
        /// </example>
        public void CheckForResource(Hex hex)
        {

            //loop through all resources
            for (int i = 0; i < internalResources.Length; i++)
            {
                //get each resource and check if we can spawn them
                Resource r = internalResources[i];
                if (r.rule != null)
                {
                    //runs through the tests and if any return false, we can not spawn this resource; check the next
                    if (RuleTest.Test(hex, r.rule, tileManager))
                    {
                        //we can spawn it, but should we?
                        int number = (int)Random.Range(0, r.rarity);
                        if (number == 0)
                        {
                            //spawn resource
                            hex.currentResource = r;
                            SpawnResource(hex, hex.currentResource, false);
                            return;
                        }
                    }
                }
            }

            //no resource spawned; return "None"
            hex.currentResource = internalResources[0];
        }

        /// <summary>
        /// Spawns the provided resource on the tile.
        /// Optional to regenerate the chunk.
        /// </summary>
        /// <remarks>
        /// This can be used to force a resource to spawn, even if against it's rules.
        /// </remarks>
        /// <param name="hex">Hex to spawn the resource on</param>
        /// <param name="r">Resource to spawn</param>
        /// <param name="regenerateChunk">If the parent chunk should be regenerated</param>
        public void SpawnResource(Hex hex, Resource r, bool regenerateChunk)
        {
            //reset resource locations
            hex.resourceLocations.Clear();
            
            //destroy previous resource objects
            if (hex.rObject != null)
            {
                Destroy(hex.rObject);
            }

            //if the resource has a mesh to spawn
            if (r.meshToSpawn != null)
            {
                //calculate y position to spawn the resources
                float y;
                if (hex.localMesh == null)
                {
                    y = (worldManager.hexExt.y); if (y == 0) { y -= ((hex.worldPosition.y + worldManager.hexExt.y) / Random.Range(4, 8)); } else { y = hex.worldPosition.y + worldManager.hexExt.y + hex.currentResource.meshToSpawn.bounds.extents.y; }
                }
                else
                {
                    y = (hex.localMesh.bounds.extents.y); if (y == 0) { y -= ((hex.worldPosition.y + hex.localMesh.bounds.extents.y) / Random.Range(4, 8)); } else { y = hex.worldPosition.y + hex.localMesh.bounds.extents.y + hex.currentResource.meshToSpawn.bounds.extents.y; }
                }

                //spawn a resource for each spawn amount
                for (int i = 0; i < r.meshSpawnAmount; i++)
                {
                    //position setting
                    float x = (worldManager.hexCenter.x + Random.Range(-0.2f, 0.2f));
                    float z = (worldManager.hexCenter.z + Random.Range(-0.2f, 0.2f));
                    hex.resourceLocations.Add(new Vector3(x, y, z));
                }

                //number of resources
                int size = hex.resourceLocations.Count;

                //number of resources to combine
                if (size > 0)
                {
                    //combine instances
                    CombineInstance[] combine = new CombineInstance[size];
                    Matrix4x4 matrix = new Matrix4x4();
                    matrix.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);

                    //skip first combine instance due to presetting
                    for (int k = 0; k < size; k++)
                    {
                        combine[k].mesh = hex.currentResource.meshToSpawn;
                        matrix.SetTRS(hex.resourceLocations[k], Quaternion.identity, Vector3.one);
                        combine[k].transform = matrix;
                    }

                    //create gameobject to hold the resource meshes 
                    GameObject holder = new GameObject(r.name + " at " + hex.AxialCoordinates, typeof(MeshFilter), typeof(MeshRenderer));

                    //set the gameobject position to the hex position
                    holder.transform.position = hex.worldPosition;
                    holder.transform.parent = hex.parentChunk.transform;

                    //set the resource mesh texture
                    holder.GetComponent<Renderer>().material.mainTexture = r.meshTexture;

                    //assign the combined mesh to the resource holder gameobject
                    MeshFilter filter = holder.GetComponent<MeshFilter>();
                    filter.mesh = new Mesh();
                    filter.mesh.CombineMeshes(combine);

                    //set the hex's resource object to the resource holder
                    hex.rObject = holder;

                    //UV mapping
                    Rect rectArea;
                    worldManager.textureAtlas.resourceLocations.TryGetValue(r, out rectArea);

                    //temp UV data
                    Vector2[] uv;
                    uv = new Vector2[filter.mesh.vertexCount];

                    //calculate the combined UV data
                    for (int i = 0; i < filter.mesh.vertexCount; i++)
                    {
                        uv[i] = new Vector2(filter.mesh.uv[i].x * rectArea.width + rectArea.x, filter.mesh.uv[i].y * rectArea.height + rectArea.y);
                    }

                    //assign the resource holder's UV data
                    filter.mesh.uv = uv;
                }
            }

            //if needed; regenerate the chunk and it's UV data
            if (regenerateChunk)
            {
                hex.ChangeTextureToResource();
                hex.parentChunk.RegenerateMesh();
            }
        }
    }


    /// <summary>
    /// Resource class that contains all the values for the base resource.
    /// </summary>
    [System.Serializable]
    public class Resource
    {
        /// <summary>
        /// Name of the resource
        /// </summary>
        public string name;
        /// <summary>
        /// The rules of where the resource can spawn
        /// </summary>
        public HexRule rule;
        /// <summary>
        /// The rarity of the resource
        /// </summary>
        /// <remarks>
        /// This is a calculated probabilty of 1/<see cref="rarity"/>.
        /// </remarks>
        public float rarity;
        /// <summary>
        /// Amount of mesh to spawn
        /// </summary>
        /// <remarks>
        /// Only has an effect if <see cref="meshToSpawn"/> is present.
        /// </remarks>
        public int meshSpawnAmount;

        /// <summary>
        /// The resource mesh to spawn
        /// </summary>
        public Mesh meshToSpawn;
        /// <summary>
        /// The texture for the spawned mesh
        /// </summary>
        public Texture2D meshTexture;
        /// <summary>
        /// Decides if the ground texture is replaced with the resource specific one in the texture atlas
        /// </summary>
        public bool replaceGroundTexture;

        /// <summary>
        /// Constructor of this class.
        /// </summary>
        /// <param name="name">Name of the resource</param>
        /// <param name="rarity">This is a calculated probabilty of 1/<see cref="rarity"/>.</param>
        /// <param name="meshSpawnAmount">Amount of mesh to spawn</param>
        /// <param name="meshToSpawn">The resource mesh to spawn</param>
        /// <param name="meshTexture">The texture for the spawned mesh</param>
        /// <param name="replaceGroundTexture">Decides if the ground texture is replaced with the resource specific one in the texture atlas</param>
        /// <param name="rule">The rules of where the resource can spawn</param>
        public Resource(string name, float rarity, int meshSpawnAmount, Mesh meshToSpawn, Texture2D meshTexture, bool replaceGroundTexture, HexRule rule)
        {
            this.name = name;
            this.rarity = rarity;
            this.meshToSpawn = meshToSpawn;
            this.meshTexture = meshTexture;
            this.replaceGroundTexture = replaceGroundTexture;
            this.rule = rule;
        }
    }
}