﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy
{
    public class TectonicPlate
    {
        public enum PlateType {
            Continental = 0,
            Oceanic = 1
        }
        
        public List<VoronoiTile> plateTiles = new List<VoronoiTile>();
        public PlateType plateType;
        public TectonicPlate(VoronoiTile startTile)
        {
            startTile.plate = this;
            plateTiles.Add(startTile);
        }
    }
}