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
* Habrador's Computational Geometry (HCG) (https://github.com/Habrador/Computational-geometry)
* MIConvexHull (MICH) (https://designengrlab.github.io/MIConvexHull/)

I created a custom class in [HalfEdgeMapper.cs](HalfEdgeMapper.cs) that converts the convex hull from MICH and prepares it for the Delaunay3DToVoronoi algorithm from HCG. I also altered the Delaunay3DToVoronoi algorithm from that same package in order to link voronoi cells to their neighbors. The code for that can be found in the [Delaunay3DToVoronoiTile.cs](Delaunay3DToVoronoiTile.cs) file. This resulted in a significant increase in performance. The following screenshot is for the generation of a 10.000 point convex hull from before the changes, and after.

![image](https://user-images.githubusercontent.com/33265853/141485638-4f5c0614-fbba-4f2d-98af-371dc07ea1bf.png)

# Planet Generation
First, to get a uniform distribution of vertices for our sphere mesh, we generate a number of fibonacci points.

![image](https://user-images.githubusercontent.com/33265853/141595473-6f5f758c-0090-4cfc-a0c5-a5eb17441d63.png)

Then we find the convex hull of those points using MIConvexHull, and turn it into a sphere mesh using a method from HCG. I used a simple editor script to save a sphere with 30.000 fibonacci points so this step wouldn't have to be repeated during runtime.

![image](https://user-images.githubusercontent.com/33265853/141596450-6774a554-80b4-4c71-8f9b-01ac82da7596.png)

In order to determine how we want to deform the mesh, we need to figure out where all the tectonic plates are. Again, we start of with a (smaller) number of fibonacci points.

![image](https://user-images.githubusercontent.com/33265853/141595963-ec8d632d-1ef2-4971-868f-c96d6c985fba.png)

And we follow this up by another convex hull

![image](https://user-images.githubusercontent.com/33265853/141595515-b306bf00-8098-49dc-8582-347d4d284749.png)

That convex hull is then used as input for the (altered) Voronoi Diagram algorithm from Habrador's Computational Geometry.

![image](https://user-images.githubusercontent.com/33265853/141595615-7c1bc707-b333-47ff-8d6a-e7b41302998a.png)

Our altered implementation allows us to find the neighbors of each tile, so we run a basic flood-fill to divide the Voronoi cells over the tectonic plates.

![image](https://user-images.githubusercontent.com/33265853/141595704-39285476-fdbb-4238-ab86-e9e4bf6a30ac.png)

Finally, we give tiles at colliding plate boundaries a higher elevation value. For now, plate collisions are just randomly determined, with a low chance of oceanic-continental collisions and a higher chance of inter-continental ones. A KD-tree then lets us assign every vertex of the sphere mesh to a tile in the Voronoi diagram, after which we can raise or lower vertices according to their tectonic plate type. Vertices on oceanic tiles get lowered, while those on continental tiles get raised. a small amount of Perlin noise is then applied to vertices on every continental tile, after which another layer of Perlin noise gets applied across colliding plate boundary tiles. This gives us mountain ranges in places where you'd expect them to be.

![image](https://user-images.githubusercontent.com/33265853/141597527-98d8c67b-9cb4-49ea-8404-045c893004db.png)

# Inspired by
* https://www.redblobgames.com/x/1843-planet-generation/
