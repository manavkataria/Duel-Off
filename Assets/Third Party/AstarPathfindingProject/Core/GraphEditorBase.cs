using Pathfinding.Serialization.JsonFx;

namespace Pathfinding {
	[JsonOptIn]
	/** Defined here only so non-editor classes can use the #target field */
	public class GraphEditorBase {
		
		public NavGraph target;
	}
}