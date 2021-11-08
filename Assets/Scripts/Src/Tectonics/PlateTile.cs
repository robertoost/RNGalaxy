using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy
{
    public class PlateTile
    {
        TectonicPlate plate;
        Vector3 position;
        List<PlateTile> neighbors = new List<PlateTile>();

        public PlateTile(Vector3 position, TectonicPlate plate)
        {
            this.plate = plate;
        }
    }
}