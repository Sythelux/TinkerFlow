@tool
extends InspectorGadgetBase
class_name GadgetResource

var custom_gadget_metadata := {}

func _init(in_node_path: NodePath = NodePath(), in_subnames: String = ""):
	super._init(in_node_path, in_subnames)
