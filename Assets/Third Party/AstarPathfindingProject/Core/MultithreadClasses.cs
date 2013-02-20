//#define SingleCoreOptimize

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

namespace Pathfinding {
	public class NodeRunData {
		public Path path;
		public ushort pathID;
		public NodeRun[] nodes;
		
		/** The open list.
		  * A binary heap holds and sorts the open list for the pathfinding. 
	 	  * Binary Heaps are extreamly fast in providing a priority queue for the node with the lowest F score.*/
		public BinaryHeapM open;
		
		/** Set all node's pathIDs to 0.
		 * \see Pathfinding.NodeRun.pathID
		 */
		public void ClearPathIDs () {
			Debug.Log ("Clearing Path IDs");
			
			NavGraph[] graphs = AstarPath.active.astarData.graphs;
			for (int i=0;i<graphs.Length;i++) {
				Node[] nodes = graphs[i].nodes;
				if (nodes != null) {
					for (int j=0;j<nodes.Length;j++) {
						if (nodes[j] != null) nodes[j].pathID = 0;
					}
				}
			}
		}
	}
	
	public class NodeRun {
		public uint g;
		public uint h;
		
		public Node node {
			get {
				return (Node)this;
			}
			set {}
		}
		
		public ushort pathID;
		public uint cost;
		
		public NodeRun parent;
		
		/** F score. The F score is the #g score + #h score, that is the cost it taken to move to this node from the start + the estimated cost to move to the end node.\n
		 * Nodes are sorted by their F score, nodes with lower F scores are opened first */
		public uint f {
			get {
				return g+h;
			}
		}
		
		/** Links this NodeRun with the specified node.
		 * \param node Node to link to
		 * \param index Index of this NodeRun in the nodeRuns array in Pathfinding::AstarData
		 */
		public void Link (Node node, int index) {
			if (index != node.GetNodeIndex ())
				throw new System.Exception ("Node indices out of sync when creating NodeRun data (node index != specified index)");
		}
	}
}