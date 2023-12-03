@tool
extends InspectorGadgetBase
class_name GadgetStringEdit

@export var placeholder_text: String :
	set = set_placeholder_text

func set_placeholder_text(new_placeholder_text: String) -> void:
	if placeholder_text != new_placeholder_text:
		placeholder_text = new_placeholder_text

		if has_controls():
			get_controls()[0].placeholder_text = placeholder_text

func _init(in_node_path: NodePath = NodePath(), in_subnames: String = ""):
	super._init(in_node_path, in_subnames)

static func supports_type(value) -> bool:
	if value is String:
		return true
	return false

func has_controls() -> bool:
	return has_node("LineEdit")

func get_controls() -> Array:
	return [$LineEdit]

func populate_controls() -> void:
	var line_edit = LineEdit.new()
	line_edit.name = "LineEdit"
	line_edit.placeholder_text = placeholder_text
	line_edit.set_anchors_and_offsets_preset(PRESET_FULL_RECT)
	line_edit.text_submitted.connect(set_node_value)
	add_child(line_edit)

func populate_value(value) -> void:
	var line_edit = get_controls()[0]
	line_edit.set_block_signals(true)
	line_edit.text = value
	line_edit.set_block_signals(false)
	line_edit.editable = editable

func depopulate_value() -> void:
	var line_edit = get_controls()[0]
	line_edit.set_block_signals(true)
	line_edit.text = ""
	line_edit.set_block_signals(false)
	line_edit.editable = false
