# RNGalaxy 🌍🌑🌟🌞🚀
A procedural tectonic planet simulator.

![image](https://user-images.githubusercontent.com/33265853/141483949-84b5aa3d-1aeb-41bc-85ae-2d1ae64a611a.png)


# Getting Started

* Have **Unity** installed
* Open the TectonicPlanetGenerator scene in the _scenes folder.
* Run the scene, the settings will appear in play mode.

![planet-gif](https://user-images.githubusercontent.com/33265853/141481842-aa399580-8f63-459e-95ed-253a944a118c.gif)

**Enjoy!**

# Libraries and Assets used
* Fibonacci sampling from: (https://github.com/OcarinhaOfTime/SpherePlacement)
* KD-Tree from (https://github.com/viliwonka/KDTree)
* Sci-fi GUI from (https://assetstore.unity.com/packages/2d/gui/sci-fi-gui-skin-15606)
* Perlin noise from (https://github.com/keijiro/PerlinNoise)
* Computational Geometry (https://github.com/Habrador/Computational-geometry)
* MIConvexHull (https://designengrlab.github.io/MIConvexHull/)

I created a custom HalfEdgeMapper class that converts the convex hull from MIConvexHull and prepares it for the Delaunay3DToVoronoi algorithm from Habrador's Computational Geometry library. I also altered the Delaunay3DToVoronoi algorithm from that same package in order to link voronoi cells to their neighbors. The code for that can be found in the Delaunay3DToVoronoiTile function. This resulted in a significant increase in performance.

![image](https://user-images.githubusercontent.com/33265853/141485638-4f5c0614-fbba-4f2d-98af-371dc07ea1bf.png)


# Inspired by
* https://www.redblobgames.com/x/1843-planet-generation/
