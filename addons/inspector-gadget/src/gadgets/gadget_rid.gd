@tool
extends GadgetStringEdit
class_name GadgetRID

func _init(in_node_path: NodePath = NodePath(), in_subnames: String = ""):
	super._init(in_node_path, in_subnames)

static func supports_type(value) -> bool:
	if value is RID:
		return true
	return false

func populate_controls() -> void:
	var line_edit = LineEdit.new()
	line_edit.name = "LineEdit"
	line_edit.editable = false
	line_edit.set_anchors_and_offsets_preset(PRESET_FULL_RECT)
	add_child(line_edit)

func populate_value(value) -> void:
	var line_edit = get_controls()[0]
	line_edit.text = str(value)

func depopulate_value() -> void:
	var line_edit = get_controls()[0]
	line_edit.text = ""
