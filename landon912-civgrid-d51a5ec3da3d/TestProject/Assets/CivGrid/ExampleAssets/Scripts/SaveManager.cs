using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CivGrid;
using CivGrid.SampleResources;

namespace CivGrid.SampleResources
{
    public class SaveManager : MonoBehaviour
    {

        List<string> savedGames = new List<string>();

        WorldManager worldManager;

        void Start()
        {
            worldManager = GameObject.FindObjectOfType<WorldManager>();
        }

        string saveName = "worldTest";
        bool show = false;
        void OnGUI()
        {
            if (GUI.Button(new Rect((Screen.width / 2) - 75f, (Screen.height / 2) - (75f), 150f, 50f), "New World"))
            {
                worldManager.RegenerateNewMap();
            }
            if (GUI.Button(new Rect((Screen.width / 2) - 75f, (Screen.height / 2) - (25f), 150f, 50f), "Menu")) { if (show == false) { show = true; } else { show = false; } }

            if (show)
            {

                GUI.Label(new Rect((Screen.width / 2) - 50f, (Screen.height / 2) + 25f, 100f, 50f), "Save Name:");
                saveName = GUI.TextField(new Rect((Screen.width / 2) - 100f, (Screen.height / 2) + 50f, 200f, 25f), saveName);

                if (GUI.Button(new Rect((Screen.width / 2) - 50f, (Screen.height / 2) + 75f, 100f, 50f), "Save Game"))
                {
                    FileUtility.SaveTerrain(saveName);
                    if (savedGames.Contains(saveName) == false)
                    {
                        savedGames.Add(saveName);
                    }
                }

                GUI.Label(new Rect((Screen.width / 2) - 50f, (Screen.height / 2) + 125f, 100f, 50f), "Load:");
                for (int i = 0; i < savedGames.Count; i++)
                {
                    string s = savedGames[i];
                    GUI.Button(new Rect((Screen.width / 2) - 50f, (Screen.height / 2) + 150f + (25f * i), 100f, 25f), s);
                }
            }
        }
    }
}