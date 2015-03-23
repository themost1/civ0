using UnityEngine;
using System.Collections;
using CivGrid;
using CivGrid.SampleResources;

namespace CivGrid
{
    public class Hex : InternalHex
    {
        public bool isSelected;
        internal Unit currentUnit;
    }
}