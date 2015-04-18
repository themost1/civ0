using UnityEngine;
using System.Collections;
using CivGrid;
using CivGrid.SampleResources;

namespace CivGrid.SampleResources
{

    /// <summary>
    /// This is the base script for ALL units in the game, combat and speical abilities is handled in the child class (Melee, Range Attack, Build, etc)
    /// This handles movement, health, death, selection, and basic statistics.
    /// It is not recamended to add to this class instead use this as a base class and extend methods from it(see Melee for more information on how to do this)
    /// </summary>
    public abstract class Unit : MonoBehaviour
    {

        public int health;
        public bool selected = false;
        public Hex currentLocation;
        bool moveMode;

        public int unitMoves;
        public int movesLeft;

        private Ray ray;
        private RaycastHit hit;

        void Start()
        {
            GameManager.nextTurn += NextTurn;
        }

        void NextTurn()
        {
            movesLeft = unitMoves;
        }

        public void Update()
        {
            //selected = currentLocation.isSelected;
            if (currentLocation != null)
            {
                currentLocation.currentUnit = this;
            }

            //if we are selected
            if (selected)
            {
                //pressed "m" while we are selected to move?
                if (Input.GetButtonDown("Move"))
                {
                    //toggle move mode
                    if (moveMode == false) { moveMode = true; }
                    else { moveMode = true; }
                }
                //if we are in move mode
                if (moveMode)
                {
                    //create ray from camera
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    //cast raycast to find disired movement grid
                    if (Physics.Raycast(ray, out hit, 100f))
                    {
                        //get obj reference to hit obj
                        GameObject obj = hit.collider.gameObject;
                        //is it a tile?
                        if (obj.CompareTag("Tile"))
                        {

                            //have we click on this tile to move our unit?
                            if (Input.GetMouseButtonDown(0))
                            {
                                //since its a tile get the Hex script
                                //TODO: FIX
                                Hex hex = new Hex();//obj.GetComponent<HexInfo>();

                                //check to see if tile is occupied
                                if (hex.currentUnit == null)
                                {
                                    //unlink from current obj
                                    moveMode = false;
                                    selected = false;

                                    int movesNeeded = MovesToCompleteAction();

                                    if (movesNeeded <= movesLeft)
                                    {
                                        print("Moving to tile: " + hex.AxialCoordinates);
                                        //actually move the unit
                                        Move(currentLocation, hex, true);
                                        movesLeft -= movesNeeded;
                                    }
                                    else { print("too far to move"); }
                                }
                                else
                                {
                                    //add combat or unit switching here(in the child class)
                                    //make sure we dont attack ourselves!!!
                                    if (hex.currentUnit != (Melee)this)
                                    {
                                        //unlink from current obj
                                        moveMode = false;
                                        selected = false;

                                        print("attacking");
                                        Attack((Melee)hex.currentUnit);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (health <= 0)
            {
                GameManager.worldEvent.Invoke("Death", "Unit " + gameObject.name + " has been killed!");
                currentLocation.currentUnit = null;
                Destroy(this.gameObject);
            }
        }


        //TODO impliment this method
        public int MovesToCompleteAction()
        {
            return 1;
        }



        public void Move(Hex fromTile, Hex toTile, bool quickMove)
        {
            if (quickMove)
            {
                //find our obj reference to the needed grid
                GameObject hex = GameObject.Find("HexTile " + toTile.AxialCoordinates);
                //tell old tile we are no longer on it
                fromTile.currentUnit = null;
                //move unit
                gameObject.transform.position = new Vector3(hex.transform.position.x, (hex.transform.position.y + hex.GetComponent<Collider>().bounds.extents.y), hex.transform.position.z);
                //set current location
                currentLocation = toTile;
            }
            else
            {
                //CalculateFastestPath(fromTile.gridPosition, toTile.gridPosition);
            }
        }

        private void CalculateFastestPath(Vector3 from, Vector3 to)
        {
            //used for pathfinding
        }
        //method to be overode by child class
        public virtual void Attack(Melee unitToAttack)
        {
            //see indivdiual scripts for attack behavior(Melee, Range, etc)
            print("ahhh The Unit version of attack was called: OH NO! Something went hugely wrong");
        }
        //method to be overode by child class
        public virtual void Attack(Range unitToAttack)
        {
            //see indivdiual scripts for attack behavior(Melee, Range, etc)
            print("ahhh The Unit version of attack was called: OH NO! Something went hugely wrong");
        }
    }
}