--------------------
How to run the PhotonViewSyncTester scene
--------------------
1: publish the scene PhotonViewSyncTester and run it
2: run the scene inside Unity Editor
3: Open the Grapher editor Window
3: Select the GameObject Script to trigger the plot for the various solutions ( if you don't select this gameobject, Grapher will not be setup)
4: make sure you have each graph vertical range set to be identical ( second icon with the two diagoanl arrow must be green)

--------------------
How to read the graph
--------------------
- The  White graph is the raw position coming from the network stream
- All other  curve are smoothing out and aplklying algorythm to the raw position

the y axis represent the distance to the world origin.
It can not be the local position of the source itself since it's remote, but for the sake of anylsing the best smoothing technic, this is fine, it's a common referencial for all curves

If you switch the raw position to local, you will then have a relative curve output ( as in the SyncTester scenem, where the source position is known and can be use as a referencial)

--------------------
To Add new test cubes:
--------------------
1: create a regular scene Network GameObject
2: observe your smoothing component with the PhotonView
3: add the GrapherProxy Component, define a unique name and color
4: in the Scripts GameObject, add this new GameObject to both the DragAndMove Component Cubes list and the GrapherEditor Instances List

