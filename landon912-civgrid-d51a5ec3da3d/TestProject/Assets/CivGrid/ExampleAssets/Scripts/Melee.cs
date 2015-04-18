using UnityEngine;
using System.Collections;
using CivGrid;
using CivGrid.SampleResources;

namespace CivGrid.SampleResources
{

    public class Melee : Unit
    {

        public int strength;

        public override void Attack(Melee unitToAttack)
        {
            //determine health(NEED TO BE OVERHAULED)
            int r = this.strength / unitToAttack.strength;
            unitToAttack.health -= ((20 * (3 * (r) + 1)) / (3 + r));
            this.health -= ((20 * (3 + r)) / ((3 * r) + 1));
            print(this.health);
        }

        public override void Attack(Range unitToAttack)
        {
            //determine health(NEED TO BE OVERHAULED)
            int r = this.strength / unitToAttack.strength;
            unitToAttack.health -= ((20 * (3 * (r) + 1)) / (3 + r));
            this.health -= ((20 * (3 + r)) / ((3 * r) + 1));
            print(this.health);
        }
    }
}