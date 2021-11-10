using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RNGalaxy
{
    public class ContinentFloodFillDemo : MonoBehaviour
    {
        public Planet planet;

        // A very simple demo using the planet class functionality.
        // Has an editor script that renders the planet's tectonic plates as polygonal tiles.
        void OnValidate()
        {
            planet.GenerateTectonicPlates();
        }
    }
}
