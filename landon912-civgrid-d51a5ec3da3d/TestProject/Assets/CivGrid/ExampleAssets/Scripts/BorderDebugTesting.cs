using UnityEngine;
using System.Collections;
using CivGrid;

public class BorderDebugTesting : MonoBehaviour
{

    [SerializeField]
    public WorldManager worldManager;

    void Start()
    {
        WorldManager.onHexClick += OnHexClick;
    }

    void OnDisable()
    {
        WorldManager.onHexClick -= OnHexClick;
    }

    void OnHexClick(Hex hex, int mouseButton)
    {
        if (mouseButton == 0)
        {
			//worldManager.ShowGrid = true;
           	hex.BorderID = 0;
        }
        if (mouseButton == 1)
        {
			//worldManager.ShowGrid = false;
            hex.BorderID = 1;
        }
    }
}