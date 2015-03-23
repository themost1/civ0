using UnityEngine;
using System.Collections;
using CivGrid;
using CivGrid.SampleResources;

namespace CivGrid.SampleResources
{

    public delegate void WorldEvent(string type, string message);
    public delegate void NextTurn();

    public class GameManager : MonoBehaviour
    {

        public GameObject Barbarian;

        public static WorldEvent worldEvent;
        public static NextTurn nextTurn;

        WorldManager worldManager;

        void Awake()
        {
            GameManager.worldEvent += EventManager;
            GameManager.nextTurn += NextTurn;
            WorldManager.onHexClick += OnHexClick;
            worldManager = GameObject.FindObjectOfType<WorldManager>();
        }

        void OnHexClick(Hex hex, int mouseButton)
        {
            if(mouseButton == 0)
            {
				Debug.Log( "nya" );
                worldManager.improvementManager.TestedAddImprovementToTile(hex, 0);
            }
            if(mouseButton == 1)
            {
                worldManager.improvementManager.RemoveImprovementFromTile(hex);
            }
        }

        void OnDisable()
        {
            GameManager.worldEvent -= EventManager;
            GameManager.nextTurn -= NextTurn;
        }

        void StartSpawning()
        {
            SpawnUnit(Barbarian, new Vector2(10,12));
            SpawnUnit(Barbarian, new Vector2(10, 13));
        }

        void SpawnUnit(GameObject obj, Vector2 hexLoc)
        {
            Hex hex = worldManager.GetHexFromAxialCoordinates(hexLoc);

            GameObject unit = (GameObject)Instantiate(obj, hex.worldPosition, Quaternion.identity);

            unit.GetComponent<Unit>().currentLocation = hex;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                nextTurn.Invoke();
            }
        }

        void NextTurn()
        {
            Debug.Log("going to next turn on GameManager");
        }

        void EventManager(string type, string message)
        {
            if (type == "Death")
            {
                print(message);
            }
        }

    }
}