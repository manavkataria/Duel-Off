/** \file
 * This file contains the changelog for the A* Pathfinding Project.
 */

/** \page changelog Changelog
 
- 3.1.4 (iOS fixes)
	- Fixes
		- More fixes for the iOS platform.
		- The "JsonFx.Json.dll" file is now correctly named as such.
	- Changes
		- Removed unused code from DotNetZip which reduced the size of it with about 20 KB
		
- 3.1.3 (free version only)
	- Fixes
		- Some of the fixes which were said to have been made in 3.1.2 were actually not included in the free version of the project. Sorry about that.
		- Also includes a new JsonFx and Ionic.Zip dll. This should make it possible to build with the .Net 2.0 Subset again see:
			http://www.arongranberg.com/forums/topic/ios-problem/page/1/
			
- 3.1.2 (small bugfix release)
	- Fixes
		- Fixed a bug which caused builds for iPhone to fail.
		- Fixed a bug which caused runtime errors on the iPhone platform.
		- Fixed a bug which caused huge lag in the editor for some users when using grid graphs.
		- ListGraphs are now correctly loaded as PointGraphs when loading data from older versions of the system.
	- Changes
		- Moved JsonFx into the namespace Pathfinding.Serialization.JsonFx to avoid conflicts with users own JsonFx libraries (if they used JsonFx).
		
- 3.1.1 (small bugfix release)
	- Fixes
		- Fixed a bug which would cause Pathfinding.GraphUpdateUtilities.UpdateGraphsNoBlock to throw an exception when using multithreading
		- Fixed a bug which caused an error to be logged and no pathfinding working when not using multithreading in the free version of the project
		- Fixed some example scene bugs due to downgrading the project from Unity 3.5 to Unity 3.4
		
- 3.1
	- Fixed bug which caused LayerMask fields (GridGraph inspector for example) to behave weirdly for custom layers on Unity 3.5 and up.
	- The color setting "Node Connection" now actually sets the colors of the node connections when no other information should be shown using the connection colors or when no data is available.
	- Put the Int3 class in a separate file.
	- Casting between Int3 and Vector3 is no longer implicit. This follows the rule of "if information might be lost: use explicit casts".
	- Renamed ListGraph to PointGraph. "ListGraph" has previously been used for historical reasons. PointGraph is a more suitable name.
	- Graph can now have names in the editor (just click the name in the graph list)
	- Graph Gizmos can now be selectively shown or hidden per graph (small "eye" icon to the right of the graph's name)
	- Added GraphUpdateUtilities with many useful functions for updating graphs.
	- Erosion for grid graphs can now use tags instead of walkability
	- Fixed a bug where using One Way links could in some cases result in a NullReferenceException being thrown.
	- Vector3 fields in the graph editors now look a bit better in Unity 3.5+. EditorGUILayout.Vector3Field didn't show the XYZ labels in a good way (no idea why)
	- GridGraph.useRaycastNormal is now enabled only if the Max Slope is less than 90 degrees. Previously it was a manual setting.
	- The keyboard shortcut to scan all graphs does now also work even when the graphs are not deserialized yet (which happens a lot in the editor)
	- Added NodeLink script, which can be attached to GameObjects to add manual links. This system will eventually replace the links system in the A* editor.
	- Added keyboard shortcuts for adding and removing links. See Menubar -> Edit -> Pathfinding
	\note Some features are restricted to Unity 3.5 and newer because of technical limitations in earlier versions (especially multi-object editing related features).
	\note Dropped Unity 3.3 support.
	
- 3.1 beta (version number 3.0.9.9 in Unity due to technical limitations of the System.Versions class)
	- Multithreading is now enabled in the free version of the A* Pathfinding Project!
	- Better support for graph updates called during e.g OnPostScan.
	- PathID is now used as a short everywhere in the project
	- G,H and penalty is now used as unsigned integers everywhere in the project instead of signed integers.
	- There is now only one tag per node (if not the #define ConfigureTagsAsMultiple is set).
	- Fixed a bug which could make connections between graphs invalid when loading from file (would also log annoying error messages).
	- Erosion (GridGraph) can now be used even when updating the graph during runtime.
	- Fixed a bug where the GridGraph could return null from it's GetNearestForce calls which ended up later throwing a NullReferenceException.
	- FunnelModifier no longer warns if any graph in the path does not implement the IFunnelGraph interface (i.e have no support for the funnel algorithm)
	and instead falls back to add node positions to the path.
	- Added a new graph type : LayerGridGraph which works like a GridGraph, but has support for multiple layers of nodes (e.g multiple floors in a building).
	- ScanOnStartup is now exposed in the editor.
	- Separated temporary path data and connectivity data.
	- Rewritten multithreading. You can now run any number of threads in parallel.
	- To avoid possible infinite loops, paths are no longer returned with just an error when requested at times they should not (e.g right when destroying the pathfinding object)
	- Cleaned up code in AstarPath.cs, members are now structured and many obsolete members have been removed.
	- Rewritten serialization. Now uses Json for settings along with a small part hardcoded binary data (for performance and memory).
		This is a lot more stable and will be more forwards and backwards compatible.
		Data is now saved as zip files(in memory, but can be saved to file) which means you can actually edit them by hand if you want!
	- Added dependency JsonFx (modified for smaller code size and better compatibility).
	- Added dependency DotNetZip (reduced version and a bit modified) for zip compression.
	- Graph types wanting to serialize members must add the JsonOptIn attribute to the class and JsonMember to any members to serialize (in the JsonFx.Json namespace)
	- Graph types wanting to serialize a bit more data (custom), will have to override some new functions from the NavGraph class to do that instead of the old serialization functions.
	- Changed from using System.Guid to a custom written Guid implementation placed in Pathfinding.Util.Guid. This was done to improve compabitility with iOS and other platforms.
	Previously it could crash when trying to create one because System.Guid was not included in the runtime.
	- Renamed callback AstarPath::OnSafeNodeUpdate to AstarPath::OnSafeCallback (also added AstarPath::OnThreadSafeCallback)
	- MultiTargetPath would throw NullReferenceException if no valid start node was found, fixed now.
	- Binary heaps are now automatically expanded if needed, no annoying warning messages.
	- Fixed a bug where grid graphs would not update the correct area (using GraphUpdateObject) if it was rotated.
	- Node position precision increased from 100 steps per world unit to 1000 steps per world unit (if 1 world unit = 1m, that is mm precision).
		This also means that all costs and penalties in graphs will need to be multiplied by 10 to match the new scale.
		It also means the max range of node positions is reduced a bit... but it is still quite large (about 2 150 000 world units in either direction, that should be enough).
	- If Unity 3.5 is used, the EditorGUIUtility.isProSkin field is used to toggle between light and dark skin.
	- Added LayeredGridGraph which works almost the same as grid graphs, but support multiple layers of nodes.
	- \note Dropped Unity 3.3 support.
	
	 <b>Known Bugs:</b> The C++ version of Recast does not work on Windows
	 
- Documentation Update
	- Changed from FixedUpdate to Update in the Get Started Guide. CharacterController.SimpleMove should not be called more than once per frame,
			so this might have lowered performance when using many agents, sorry about this typo.
- 3.0.9
	- The List Graph's "raycast" variable is now serialized correctly, so it will be saved.
	- List graphs do not generate connections from nodes to themselves anymore (yielding slightly faster searches)
	- List graphs previously calculated cost values for connections which were very low (they should have been 100 times larger),
		this can have caused searches which were not very accurate on small scales since the values were rounded to the nearest integer.
	- Added Pathfinding::Path::recalcStartEndCosts to specify if the start and end nodes connection costs should be recalculated when searching to reflect
		small differences between the node's position and the actual used start point. It is on by default but if you change node connection costs you might want to switch it off to get more accurate paths.
	- Fixed a compile time warning in the free version from referecing obsolete variables in the project.
	- Added AstarPath::threadTimeoutFrames which specifies how long the pathfinding thread will wait for new work to turn up before aborting (due to request). This variable is not exposed in the inspector yet.
	- Fixed typo, either there are eight (8) or four (4) max connections per node in a GridGraph, never six (6).
	- AlternativePath will no longer cause errors when using multithreading!
	- Added Pathfinding::ConstantPath, a path type which finds all nodes in a specific distance (cost) from a start node.
	- Added Pathfinding::FloodPath and Pathfinding::FloodPathTracer as an extreamly fast way to generate paths to a single point in for example TD games.
	- Fixed a bug in MultiTargetPath which could make it extreamly slow to process. It would not use much CPU power, but it could take half a second for it to complete due to excessive yielding
	- Fixed a bug in FleePath, it now returns the correct path. It had previously sometimes returned the last node searched, but which was not necessarily the best end node (though it was often close)
	- Using #defines, the pathfinder can now be better profiled (see Optimizations tab -> Profile Astar)
	- Added example scene Path Types (mainly useful for A* Pro users, so I have only included it for them)
	- Added many more tooltips in the editor
	- Fixed a bug which would double the Y coordinate of nodes in grid graphs when loading from saved data (or caching startup)
	- Graph saving to file will now work better for users of the Free version, I had forgot to include a segment of code for Grid Graphs (sorry about that)
	- Some other bugfixes
- 3.0.8.2
	- Fixed a critical bug which could render the A* inspector unusable on Windows due to problems with backslashes and forward slashes in paths.
- 3.0.8.1
	- Fixed critical crash bug. When building, a preprocessor-directive had messed up serialization so the game would probably crash from an OutOfMemoryException.
- 3.0.8
	- Graph saving to file is now exposed for users of the Free version
	- Fixed a bug where penalties added using a GraphUpdateObject would be overriden if updatePhysics was turned on in the GraphUpdateObject
	- Fixed a bug where list graphs could ignore some children nodes, especially common if the hierarchy was deep
	- Fixed the case where empty messages would spam the log (instead of spamming somewhat meaningful messages) when path logging was set to Only Errors
	- Changed the NNConstraint used as default when calling NavGraph::GetNearest from NNConstraint::Default to NNConstraint::None, this is now the same as the default for AstarPath::GetNearest.
	- You can now set the size of the red cubes shown in place of unwalkable nodes (Settings-->Show Unwalkable Nodes-->Size)
	- Dynamic search of where the EditorAssets folder is, so now you can place it anywhere in the project.
	- Minor A* inspector enhancements.
	- Fixed a very rare bug which could, when using multithreading cause the pathfinding thread not to start after it has been terminated due to a long delay
	- Modifiers can now be enabled or disabled in the editor
	- Added custom inspector for the Simple Smooth Modifier. Hopefully it will now be easier to use (or at least get the hang on which fields you should change).
	- Added AIFollow::canSearch to disable or enable searching for paths due to popular request.
	- Added AIFollow::canMove to disable or enable moving due to popular request.
	- Changed behaviour of AIFollow::Stop, it will now set AIFollow::ccanSearch and AIFollow::ccanMove to false thus making it completely stop and stop searching for paths.
	- Removed Path::customData since it is a much better solution to create a new path class which inherits from Path.
	- Seeker::StartPath is now implemented with overloads instead of optional parameters to simplify usage for Javascript users
	- Added Curved Nonuniform spline as a smoothing option for the Simple Smooth modifier.
	- Added Pathfinding::WillBlockPath as function for checking if a GraphUpdateObject would block pathfinding between two nodes (useful in TD games).
	- Unity References (GameObject's, Transforms and similar) are now serialized in another way, hopefully this will make it more stable as people have been having problems with the previous one, especially on the iPhone.
	- Added shortcuts to specific types of graphs, AstarData::navmesh, AstarData::gridGraph, AstarData::listGraph
	- <b>Known Bugs:</b> The C++ version of Recast does not work on Windows
- 3.0.7
	- Grid Graphs can now be scaled to allow non-square nodes, good for isometric games.
	- Added more options for custom links. For example individual nodes or connections can be either enabled or disabled. And penalty can be added to individual nodes
	- Placed the Scan keyboard shortcut code in a different place, hopefully it will work more often now
	- Disabled GUILayout in the AstarPath script for a possible small speed boost
	- Some debug variables (such as AstarPath::PathsCompleted) are now only updated if the ProfileAstar define is enabled
	- DynamicGridObstacle will now update nodes correctly when the object is destroyed
	- Unwalkable nodes no longer shows when Show Graphs is not toggled
	- Removed Path.multithreaded since it was not used
	- Removed Path.preCallback since it was obsolate
	- Added Pathfinding::XPath as a more customizable path
	- Added example of how to use MultiTargetPaths to the documentation as it was seriously lacking info on that area
	- The viewing mesh scaling for recast graphs is now correct also for the C# version
	- The StartEndModifier now changes the path length to 2 for correct applying if a path length of 1 was passed.
	- The progressbar is now removed even if an exception was thrown during scanning
	- Two new example scenes have been added, one for list graphs which includes sample links, and another one for recast graphs
	- Reverted back to manually setting the dark skin option, since it didn't work in all cases, however if a dark skin is detected, the user will be asked if he/she wants to enable the dark skin
	- Added gizmos for the AIFollow script which shows the current waypoint and a circle around it illustrating the distance required for it to be considered "reached".
	- The C# version of Recast does now use Character Radius instead of Erosion Radius (world units instead of voxels)
	- Fixed an IndexOutOfRange exception which could ocurr when saving a graph with no nodes to file
	- <b>Known Bugs:</b> The C++ version of Recast does not work on Windows
- 3.0.6
	- Added support for a C++ version of Recast which means faster scanning times and more features (though almost no are available at the moment since I haven't added support for them yet).
	- Removed the overload AstarData.AddGraph (string type, NavGraph graph) since it was obsolete. AstarData::AddGraph (Pathfinding::NavGraph) should be used now.
	- Fixed a few bugs in the FunnelModifier which could cause it to return invalid paths
	- A reference image can now be generated for the Use Texture option for Grid Graphs
	- Fixed an editor bug with graphs which had no editors
	- Graphs with no editors now show up in the Add New Graph list to show that they have been found, but they cannot be used
	- Deleted the \a graphIndex parameter in the Pathfinding::NavGraph::Scan function. If you need to use it in your graph's Scan function, get it using Pathfinding::AstarData::GetGraphIndex
	- Javascript support! At last you can use Js code with the A* Pathfinding Project! Go to A* Inspector-->Settings-->Editor-->Enable Js Support to enable it
	- The Dark Skin is now automatically used if the rest of Unity uses the dark skin(hopefully)
	- Fixed a bug which could cause Unity to crash when using multithreading and creating a new AstarPath object during runtime
- 3.0.5
	- \link Pathfinding::ListGraph List Graphs\endlink now support UpdateGraphs. This means that they for example can be used with the DynamicObstacle script.
	- List Graphs can now gather nodes based on GameObject tags instead of all nodes as childs of a specific GameObject.
	- List Graphs can now search recursively for childs to the 'root' GameObject instead of just searching through the top-level children.
	- Added custom area colors which can be edited in the inspector (A* inspector --> Settings --> Color Settings --> Custom Area Colors)
	- Fixed a NullReference bug which could ocurr when loading a Unity Reference with the AstarSerializer.
	- Fixed some bugs with the FleePath and RandomPath which could cause the StartEndModifier to assign the wrong endpoint to the path.
	- Documentation is now more clear on what is A* Pathfinding Project Pro only features.
	- Pathfinding::NNConstraint now has a variable to constrain which graphs to search (A* Pro only).\n
	  This is also available for Pathfinding::GraphUpdateObject which now have a field for an NNConstraint where it can constrain which graphs to update.
	- StartPath calls on the Seeker can now take a parameter specifying which graphs to search for close nodes on (A* Pro only)
	- Added the delegate AstarPath::OnAwakeSettings which is called as the first thing in the Awake function, can be used to set up settings.
	- Pathfinding::UserConnection::doOverrideCost is now serialized correctly. This represents the toggle to the right of the "Cost" field when editing a link.
	- Fixed some bugs with the RecastGraph when spans were partially out-of-bounds, this could generate seemingly random holes in the mesh
- 3.0.4 (only pro version affected)
	- Added a Dark Skin for Unity Pro users (though it is available to Unity Free users too, even though it doesn't look very good).
	  It can be enabled through A* Inspector --> Settings --> Editor Settings --> Use Dark Skin
	- Added option to include or not include out of bounds voxels (Y axis below the graph only) for Recast graphs.
- 3.0.3 (only pro version affected)
	- Fixed a NullReferenceException caused by Voxelize.cs which could surface if there were MeshFilters with no Renderers on GameObjects (Only Pro version affected)
- 3.0.2
	- Textures can now be used to add penalty, height or change walkability of a Grid Graph (A* Pro only)
	- Slope can now be used to add penalty to nodes
	- Height (Y position) can now be usd to add penalty to nodes
	- Prioritized graphs can be used to enable prioritizing some graphs before others when they are overlapping
	- Several bug fixes
	- Included a new DynamicGridObstacle.cs script which can be attached to any obstacle with a collider and it will update grids around it to account for changed position
- 3.0.1
	- Fixed Unity 3.3 compability
- 3.0
	- Rewrote the system from scratch
	- Funnel modifier
	- Easier to extend the system
	

- x. releases are major rewrites or updates to the system.
- .x releases are quite big feature updates
- ..x releases are the most common updates, fix bugs, add some features etc.
- ...x releases are quickfixes, most common when there was a really bad bug which needed fixing ASAP.

 */