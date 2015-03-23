using UnityEngine;
using System.Collections;
using CivGrid;

namespace CivGrid
{
    /// <summary>
    /// Operates a camera, or cameras if wrapping is enabled, to function similar to a locked real time game.
    /// </summary>
    public class CivGridCamera : MonoBehaviour
    {
        //Base camera Settings
        /// <summary>
        /// Should the terrain wrap around when a camera moves too far off in the horizontal side?
        /// This adds a small overhead from running two cameras and a depth buffer.
        /// </summary>
        public bool enableWrapping;
        private Camera cam1;
        private Transform cam1T;
        /// <summary>
        /// The height of the camera from the terrain.
        /// </summary>
        public float cameraHeight = 3f;
        /// <summary>
        /// The angle of the camera in degrees. Default is 65*.
        /// </summary>
        public float cameraAngle = 65f;
        /// <summary>
        /// The speed at which the camera moves in units per second.
        /// </summary>
        public float cameraSpeed = 2f;

        //Base wrapping settings
        private Vector2 camOffset;
        private Camera cam2;
        private Transform cam2T;
        private bool cam1Lead;

        //Mouse data
        private Vector3 moveVector;
        private Vector3 pos;

        //cache the WorldManager
        private WorldManager worldManager;

        /// <summary>
        /// Helper method to retrieve a camera from the camera system, containing camera(s).
        /// Index 0 will return the first camera and if present index 1 will return the second camera.
        /// </summary>
        /// <param name="index">The camera index to retrieve</param>
        /// <returns>The retrieved camera</returns>
        /// <remarks>
        /// If an invalid index is supplied, for example if index 1 is given when enableWrapping is false, null
        /// will be returned. This also applies for any other index then 0 or 1.
        /// </remarks>
        public Camera GetCamera(int index)
        {
            if (index == 0) { return cam1; }
            if (index == 1 && enableWrapping == true) { return cam2; }
            else { return null; }
        }

        /// <summary>
        /// Sets up the cameras to position themselves correctly depending on the user settings.
        /// Spawns the second follow camera if wrapping is enabled.
        /// Caches transforms for the cameras for speed.
        /// </summary>
        /// <remarks>
        /// The WorldManager script must be within the scene for this method to execute.
        /// </remarks>
        public void SetupCameras()
        {
            //cache the lead camera and world manager
            cam1 = gameObject.GetComponent<Camera>();
            worldManager = GameObject.FindObjectOfType<WorldManager>();

            //set the lead camera to the origin
            cam1.transform.position = new Vector3(0, 0, 0);

            //setup wrapping camera if needed
            if (enableWrapping == true)
            {
                //spawn the follow camera and setup it's camera setting to match the lead camera
                cam2 = new GameObject("Cam2").gameObject.AddComponent<Camera>();
                cam2.gameObject.AddComponent<GUILayer>();
                cam2.fieldOfView = cam1.fieldOfView;
                cam2.clearFlags = CameraClearFlags.Depth;
                cam2.depth = cam1.depth + 1;

                //calculate the offset between the two cameras to make a wrapping world
                camOffset = new Vector2((this.worldManager.mapSize.x * this.worldManager.hexSize.x), 0);
            }

            //cache the transform on the lead camera and assign its position and rotation
            cam1T = cam1.transform;
            cam1T.localEulerAngles = new Vector3(cameraAngle, cam1T.localEulerAngles.y, cam1T.localEulerAngles.z);
            cam1T.position = new Vector3(camOffset.x * .5f, cameraHeight, (worldManager.hexExt.y * worldManager.mapSize.y) * .5f);

            //set the follow camera to the lead cameras values until they are correctly calculated in the update methods
            if (enableWrapping == true)
            {
                cam2T = cam2.transform;
                cam2T.position = cam1T.position;
                cam2T.rotation = cam1T.rotation;
            }
        }

        /// <summary>
        /// Checks if the camera is recieving zoom input.
        /// Calls the correct update method on the camera depending if the camera has wrapping enabled.
        /// </summary>
        void Update()
        {
            //check for input
            CheckInput();
            if (enableWrapping)
            {
                //call wrapping update method
                UpdateCameraW();
            }
            else
            {
                //call regular update method
                UpdateCamera();
            }
        }

        /// <summary>
        /// Assigns moveVector in the vetical axis depending on zoom input.
        /// </summary>
        private void CheckInput()
        {
            //zoom in/out depending on input or not at all
            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus)) { moveVector.y = -1; }
            else if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)) { moveVector.y = 1; }
            else { moveVector.y = 0; }
        }

        /// <summary>
        /// Movement update for a camera system with wrapping disabled.
        /// Assigns the direction in which the camera needs to move and then translates it.
        /// Directions are described in screen space.
        /// </summary>
        private void UpdateCamera()
        {
            try
            {
                //gets the mouse position in viewport cords
                pos = cam1.ScreenToViewportPoint(worldManager.mousePos);

                //mouse is in the far right of the screen
                if (pos.x >= 0.8f)
                {
                    //move right
                    moveVector.x = 1;
                }
                //mouse is in the far left of the screen
                else if (pos.x <= 0.2f)
                {
                    //move left
                    moveVector.x = -1;
                }
                //mouse is in the middle of the screen in the horizontal axis
                else
                {
                    //dont move in the horizontal axis
                    moveVector.x = 0;
                }

                //mouse is in the far top of the screen
                if (pos.y >= 0.8f)
                {
                    //move up
                    moveVector.z = 1;
                }
                //mouse is in the far bottom of the screen
                else if (pos.y <= 0.2f)
                {
                    //move down
                    moveVector.z = -1;
                }
                //mouse is in the middle of the screen in the vertical axis
                else
                {
                    //dont move in the vertical axis
                    moveVector.z = 0;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Please Remove the CivGridCamera Script if you do not wish to use it; otherwise enable it in the WorldManager" + "/n" + e);
            }

            //move the camera in the assigned direction by the camera speed each second
            cam1T.Translate(moveVector * (cameraSpeed * Time.deltaTime), Space.World);
        }

        /// <summary>
        /// Movement update for a camera system with wrapping enabled.
        /// Assigns the direction in which the camera needs to move and then translates it.
        /// Directions are described in screen space.
        /// </summary>
        private void UpdateCameraW()
        {
            try
            {
                //gets the mouse position in viewport cords
                pos = cam1.ScreenToViewportPoint(worldManager.mousePos);

                //mouse is in the far right of the screen
                if (pos.x >= 0.8)
                {
                    //move right
                    MoveRightW();
                }
                //mouse is in the far left of the screen
                else if (pos.x <= 0.2)
                {
                    //move left
                    MoveLeftW();
                }
                //mouse is in the middle of the screen in the horizontal axis
                else
                {
                    //dont move in the horizontal axis
                    moveVector.x = 0;
                }

                //mouse is in the far top of the screen
                if (pos.y >= 0.8)
                {
                    //move up
                    moveVector.z = 1;
                }
                //mouse is in the far bottom of the screen
                else if (pos.y <= 0.2)
                {
                    //move down
                    moveVector.z = -1;
                }
                //mouse is in the middle of the screen in the vertical axis
                else
                {
                    //dont move in the vertical axis
                    moveVector.z = 0;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Please Remove the CivGridCamera Script if you do not wish to use it; otherwise enable it in the WorldManager" + "/n" + e);
            }

            //move the camera in the assigned direction by the camera speed each second
            cam1T.Translate(moveVector * (cameraSpeed * Time.deltaTime), Space.World);
            //move the follow camera
            cam2T.position = new Vector3(cam2T.position.x, cam1T.position.y, cam1T.position.z);
        }

        /// <summary>
        /// Moves the camera system to the right when wrapping is enabled.
        /// Assigns the horizontal direction in which the camera needs to move.
        /// Wraps the cameras around the map when moving.
        /// Directions are described in screen space.
        /// </summary>
        private void MoveRightW()
        {
            //move right
            moveVector.x = 1;
            
            //if camera1 is to the right of camera2
            if (cam1T.position.x > cam2T.position.x)
            {
                //camera1 is the lead camera
                cam1Lead = true;
            }
            //camera2 is to the right of camera1
            else
            {
                //camera2 is now the lead camera
                cam1Lead = false;
            }


            //camera1 is so far off the map to the right that it is going to reach the wrapping map edge soon
            if (cam1T.position.x >= (worldManager.mapSize.x * worldManager.hexExt.x) * 2.98f)
            {
                //loop it's position back over to beginning and camera2 is now lead
                cam1T.position = new Vector3(cam2T.position.x - camOffset.x, cam1T.position.y, cam1T.position.z);
                cam1Lead = false;
            }
            //camera2 is so far off the map to the right that it is going to reach the wrapping map edge soon
            if (cam2T.position.x >= (worldManager.mapSize.x * worldManager.hexExt.x) * 2.98f)
            {
                //loop it's position back over to the beginning and camera1 is now lead
                cam2T.position = new Vector3(cam1T.position.x - camOffset.x, cam2T.position.y, cam2T.position.z);
                cam1Lead = true;
            }

            //displace the follow camera by the camera offset
            if (cam1Lead)
            {
                cam2T.position = new Vector3(cam1T.position.x - camOffset.x + 0.1f, cam2T.position.y, cam2T.position.z);
            }
            else
            {
                cam2T.position = new Vector3(cam1T.position.x + camOffset.x, cam2T.position.y, cam2T.position.z);
            }
        }

        /// <summary>
        /// Moves the camera system to the left when wrapping is enabled.
        /// Assigns the horizontal direction in which the camera needs to move.
        /// Wraps the cameras around the map when moving.
        /// Directions are described in screen space.
        /// </summary>
        private void MoveLeftW()
        {
            //move left
            moveVector.x = -1;

            //if camera1 is to the left of camera2
            if (cam1T.position.x < cam2T.position.x)
            {
                //camera1 is the lead camera
                cam1Lead = true;
            }
            //camera2 is to the left of camera1
            else
            {
                //camera2 is now the lead camera
                cam1Lead = false;
            }

            //camera1 is so far off the map to the left that it is going to reach the wrapping map edge soon
            if (cam1T.position.x <= -((worldManager.mapSize.x * worldManager.hexExt.x) * .98))
            {
                //loop it's position back over to beginning and camera2 is now lead
                cam1T.position = new Vector3(cam2T.position.x + camOffset.x, cam1T.position.y, cam1T.position.z);
                cam1Lead = false;
            }
            //camera2 is so far off the map to the left that it is going to reach the wrapping map edge soon
            if (cam2T.position.x <= -((worldManager.mapSize.x * worldManager.hexExt.x) * .98))
            {
                //loop it's position back over to beginning and camera1 is now lead
                cam2T.position = new Vector3(cam1T.position.x + camOffset.x, cam2T.position.y, cam2T.position.z);
                cam1Lead = true;
            }

            //displace the follow camera by the camera offset
            if (cam1Lead)
            {
                cam2T.position = new Vector3(cam1T.position.x + camOffset.x - 0.1f, cam2T.position.y, cam2T.position.z);
            }
            else
            {
                cam2T.position = new Vector3(cam1T.position.x - camOffset.x, cam2T.position.y, cam2T.position.z);
            }
        }
    }
}