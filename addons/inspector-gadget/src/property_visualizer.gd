@tool
extends Control

@export var bool_value: bool = true :
	set = set_bool_value
@export var int_value: int = 1 :
	set = set_int_value
@export var float_value: float = PI :
	set = set_float_value
@export var string_value: String = "Foo Bar Baz Decafisbad" :
	set = set_string_value
@export var vector2_value: Vector2 = Vector2(0.0, 0.0) :
	set = set_vector2_value
@export var rect2_value: Rect2 = Rect2(Vector2(0.0, 0.0), Vector2(40.0, 20.0)) :
	set = set_rect2_value
@export var vector3_value: Vector3 = Vector3(0.0, 0.0, 0.0) :
	set = set_vector3_value
@export var transform2d_value: Transform2D = Transform2D(Vector2(1.0, 0.0), Vector2(0.0, 1.0), Vector2(0.0, 0.0)) :
	set = set_transform2d_value
@export var plane_value: Plane = Plane(0.0, 1.0, 0.0, 0.0) :
	set = set_plane_value
@export var quaternion_value: Quaternion = Quaternion(PI, 0.0, 0.0, 1.0) :
	set = set_quaternion_value
@export var color_value: Color = Color.ORANGE_RED :
	set = set_color_value
@export var aabb_value: AABB = AABB(Vector3(-7.5, -7.5, -7.5), Vector3(15.0, 15.0, 15.0)) :
	set = set_aabb_value
@export var basis_value: Basis = Basis(Vector3.RIGHT, Vector3.UP, Vector3.FORWARD) :
	set = set_basis_value
@export var transform3d_value: Transform3D = Transform3D(Basis(Vector3.RIGHT, Vector3.UP, Vector3.FORWARD), Vector3(5.0, 5.0, 5.0)) :
	set = set_transform3d_value
@export var rid_value: RID = RID() :
	set = set_rid_value

@export var array_value: Array = [
	true,
	12,
	140.5,
	"Hello",
	[1,2,3],
	["one", "two", "three"]
] :
	set = set_array_value

@export var dictionary_value: Dictionary = {
	"foo": "foo string",
	"bar": "bar string",
	"baz": "baz string",
} :
	set = set_dictionary_value

@export var packed_byte_array_value: PackedByteArray = PackedByteArray([0, 1, 2, 3, 7, 15, 31, 63, 127, 255]) :
	set = set_packed_byte_array_value
@export var packed_int32_array_value: PackedInt32Array = PackedInt32Array([0, 1, 2, 3, 4, 5, 6, 7]) :
	set = set_packed_int32_array_value
@export var packed_float32_array_value: PackedFloat32Array = PackedFloat32Array([0.0, 0.3, 0.6, 0.9, 1.2, 1.5, 1.7, 2.0, 2.3, 2.7, 3.0]) :
	set = set_packed_float32_array_value
@export var packed_string_array_value: PackedStringArray = PackedStringArray(["foo", "bar", "baz"]) :
	set = set_packed_string_array_value
@export var packed_vector2_array_value: PackedVector2Array = PackedVector2Array([Vector2.ZERO, Vector2.UP * 5, Vector2.RIGHT * 5, Vector2.ONE * 5]) :
	set = set_packed_vector2_array_value
@export var packed_vector3_array_value: PackedVector3Array = PackedVector3Array([Vector3.ZERO, Vector3.UP * 10, Vector3.RIGHT * 10, Vector3.BACK * 10]) :
	set = set_packed_vector3_array_value
@export var packed_color_array_value: PackedColorArray = PackedColorArray([Color.RED, Color.GREEN, Color.BLUE, Color.BLACK, Color.WHITE]) :
	set = set_packed_color_array_value

@export var blacklisted_property: String = "You'll never find me, Gadget! Wahahahaha!"

func _init() -> void:
	quaternion_value = Quaternion(Vector3(deg_to_rad(-30), deg_to_rad(-30), deg_to_rad(-30)).normalized(), 0)
	basis_value = Basis.IDENTITY
	basis_value = basis_value.rotated(Vector3.UP, deg_to_rad(45))
	basis_value = basis_value.rotated(Vector3.RIGHT, deg_to_rad(45))
	basis_value = basis_value.rotated(Vector3.FORWARD, deg_to_rad(45))

func set_bool_value(new_bool_value: bool) -> void:
	if bool_value != new_bool_value:
		bool_value = new_bool_value
	print("set bool value: ", bool_value)
	queue_redraw()

func set_int_value(new_int_value: int) -> void:
	if int_value != new_int_value:
		int_value = new_int_value
	print("set int value: ", int_value)
	queue_redraw()

func set_float_value(new_float_value: float) -> void:
	if float_value != new_float_value:
		float_value = new_float_value
	print("set float value: ", float_value)
	queue_redraw()

func set_string_value(new_string_value: String) -> void:
	if string_value != new_string_value:
		string_value = new_string_value
	print("set string value: ", string_value)
	queue_redraw()

func set_vector2_value(new_vector2_value: Vector2) -> void:
	if vector2_value != new_vector2_value:
		vector2_value = new_vector2_value
	print("set vector2 value: ", vector2_value)
	queue_redraw()

func set_rect2_value(new_rect2_value: Rect2) -> void:
	if rect2_value != new_rect2_value:
		rect2_value = new_rect2_value
	print("set rect2 value: ", rect2_value)
	queue_redraw()

func set_vector3_value(new_vector3_value: Vector3) -> void:
	if vector3_value != new_vector3_value:
		vector3_value = new_vector3_value
	print("set vector3 value: ", vector3_value)
	queue_redraw()

func set_transform2d_value(new_transform2d_value: Transform2D) -> void:
	if transform2d_value != new_transform2d_value:
		transform2d_value = new_transform2d_value
	print("set transform2d value: ", transform2d_value)
	queue_redraw()

func set_plane_value(new_plane_value: Plane) -> void:
	if plane_value != new_plane_value:
		plane_value = new_plane_value
	print("set plane value: ", plane_value)
	queue_redraw()

func set_quaternion_value(new_quaternion_value: Quaternion) -> void:
	if quaternion_value != new_quaternion_value:
		quaternion_value = new_quaternion_value
	print("set quaternion value: ", quaternion_value)
	queue_redraw()

func set_color_value(new_color_value: Color) -> void:
	if color_value != new_color_value:
		color_value = new_color_value
	print("set color value: ", color_value)
	queue_redraw()

func set_aabb_value(new_aabb_value: AABB) -> void:
	if aabb_value != new_aabb_value:
		aabb_value = new_aabb_value
	print("set aabb value: ", aabb_value)
	queue_redraw()

func set_basis_value(new_basis_value: Basis) -> void:
	if basis_value != new_basis_value:
		basis_value = new_basis_value
	print("set basis value: ", basis_value)
	queue_redraw()

func set_transform3d_value(new_transform3d_value: Transform3D) -> void:
	if transform3d_value != new_transform3d_value:
		transform3d_value = new_transform3d_value
	print("set transform value: ", transform3d_value)
	queue_redraw()

func set_rid_value(new_rid_value: RID) -> void:
	if rid_value != new_rid_value:
		rid_value = new_rid_value
	print("set rid value: ", rid_value)
	queue_redraw()

func set_array_value(new_array_value: Array) -> void:
	if array_value != new_array_value:
		array_value = new_array_value
	print("set array value: ", array_value)
	queue_redraw()

func set_dictionary_value(new_dictionary_value: Dictionary) -> void:
	if dictionary_value != new_dictionary_value:
		dictionary_value = new_dictionary_value
	print("set dictionary value: ", dictionary_value)
	queue_redraw()

func set_packed_byte_array_value(new_packed_byte_array_value: PackedByteArray) -> void:
	if packed_byte_array_value != new_packed_byte_array_value:
		packed_byte_array_value = new_packed_byte_array_value
	print("set pool byte array value: ", packed_byte_array_value)
	queue_redraw()

func set_packed_int32_array_value(new_packed_int32_array_value: PackedInt32Array) -> void:
	if packed_int32_array_value != new_packed_int32_array_value:
		packed_int32_array_value = new_packed_int32_array_value
	print("set pool int array value: ", packed_int32_array_value)
	queue_redraw()

func set_packed_float32_array_value(new_packed_float32_array_value: PackedFloat32Array) -> void:
	if packed_float32_array_value != new_packed_float32_array_value:
		packed_float32_array_value = new_packed_float32_array_value
	print("set pool real array value: ", packed_float32_array_value)
	queue_redraw()

func set_packed_string_array_value(new_packed_string_array_value: PackedStringArray) -> void:
	if packed_string_array_value != new_packed_string_array_value:
		packed_string_array_value = new_packed_string_array_value
	print("set pool string array value: ", packed_string_array_value)
	queue_redraw()

func set_packed_vector2_array_value(new_packed_vector2_array_value: PackedVector2Array) -> void:
	if packed_vector2_array_value != new_packed_vector2_array_value:
		packed_vector2_array_value = new_packed_vector2_array_value
	print("set pool vector2 array value: ", packed_vector2_array_value)
	queue_redraw()

func set_packed_vector3_array_value(new_packed_vector3_array_value: PackedVector3Array) -> void:
	if packed_vector3_array_value != new_packed_vector3_array_value:
		packed_vector3_array_value = new_packed_vector3_array_value
	print("set pool vector3 array value: ", packed_vector3_array_value)
	queue_redraw()

func set_packed_color_array_value(new_packed_color_array_value: PackedColorArray) -> void:
	if packed_color_array_value != new_packed_color_array_value:
		packed_color_array_value = new_packed_color_array_value
	print("set pool color array value: ", packed_color_array_value)
	queue_redraw()

func _draw() -> void:
	var origin = Vector2(10, 10)
	origin += draw_heading(origin, "Bool")
	origin += visualize_bool(origin, bool_value)
	origin += draw_heading(origin, "Int")
	origin += visualize_int(origin, int_value)
	origin += draw_heading(origin, "Float")
	origin += visualize_float(origin, float_value)
	origin += draw_heading(origin, "String")
	origin += visualize_string(origin, string_value)
	origin += draw_heading(origin, "Vector2")
	origin += visualize_vector2(origin, vector2_value)
	origin += draw_heading(origin, "Rect2")
	origin += visualize_rect2(origin, rect2_value)
	origin += draw_heading(origin, "Vector3")
	origin += visualize_vector3(origin, vector3_value)
	origin += draw_heading(origin, "Transform2D")
	origin += visualize_transform2d(origin, transform2d_value)
	origin += draw_heading(origin, "Plane")
	origin += visualize_plane(origin, plane_value)
	origin += draw_heading(origin, "Quat")
	origin += visualize_quaternion(origin, quaternion_value)
	origin += draw_heading(origin, "Color")
	origin += visualize_color(origin, color_value)
	origin += draw_heading(origin, "AABB")
	origin += visualize_aabb(origin, aabb_value)
	origin += draw_heading(origin, "Basis")
	origin += visualize_basis(origin, basis_value)
	origin += draw_heading(origin, "Transform")
	origin += visualize_transform(origin, transform3d_value)
	origin += draw_heading(origin, "Array")
	origin += visualize_array(origin, array_value)
	origin += draw_heading(origin, "PackedByteArray")
	origin += visualize_array(origin, packed_byte_array_value)
	origin += draw_heading(origin, "PackedInt32Array")
	origin += visualize_array(origin, packed_int32_array_value)
	origin += draw_heading(origin, "PackedFloat32Array")
	origin += visualize_array(origin, packed_float32_array_value)
	origin += draw_heading(origin, "PackedStringArray")
	origin += visualize_array(origin, packed_string_array_value)
	origin += draw_heading(origin, "PackedVector2Array")
	origin += visualize_array(origin, packed_vector2_array_value)
	origin += draw_heading(origin, "PackedVector3Array")
	origin += visualize_array(origin, packed_vector3_array_value)
	origin += draw_heading(origin, "PackedColorArray")
	origin += visualize_array(origin, packed_color_array_value)
	
	custom_minimum_size.y = origin.y

func draw_heading(origin: Vector2, text: String) -> Vector2:
	draw_string(get_theme_font('font'), origin + Vector2(0, 10), text)
	return Vector2(0, 20)

func visualize_bool(origin: Vector2, bool_value: bool) -> Vector2:
	draw_rect(Rect2(origin, Vector2(10, 10)), Color.RED if bool_value else Color.BLUE)
	return Vector2(0, 30)

func visualize_int(origin: Vector2, int_value: int) -> Vector2:
	draw_rect(Rect2(origin.x * int_value, origin.y, 10, 10), Color.GREEN)
	return Vector2(0, 30)

func visualize_float(origin: Vector2, float_value: float) -> Vector2:
	draw_arc(origin + Vector2(10, 10), 10, 0, float_value, 32, Color.WHITE)
	return Vector2(0, 40)

func visualize_string(origin: Vector2, string_value: String) -> Vector2:
	draw_string(get_theme_font('font'), origin + Vector2(0, 10), string_value)
	return Vector2(0, 30)

func visualize_vector2(origin: Vector2, vector2_value: Vector2) -> Vector2:
	var center = origin + Vector2(20, 20)
	draw_rect(Rect2(center - Vector2(10, 10), Vector2(20, 20)), Color.WHITE, false)
	draw_circle(center + vector2_value, 2, Color.WHITE)
	return Vector2(0, 40)

func visualize_rect2(origin: Vector2, rect2_value: Rect2) -> Vector2:
	draw_rect(Rect2(origin + rect2_value.position, rect2_value.size), Color.WHITE)
	return Vector2(0, 40)

func visualize_vector3(origin: Vector2, vector3_value: Vector3) -> Vector2:
	var center = origin + Vector2(20, 20)
	draw_axes_3d(center, Vector3(20, 20, 20))
	draw_circle(center + project_2d(vector3_value), 2.5, Color.WHITE)
	return Vector2(0, 60)

func visualize_transform2d(origin: Vector2, transform2d_value) -> Vector2:
	var t2d_origin = origin + Vector2(20, 20) + transform2d_value.origin
	draw_line(t2d_origin, t2d_origin + transform2d_value.x * 10, Color.RED)
	draw_line(t2d_origin, t2d_origin + transform2d_value.y * 10, Color.GREEN)
	return Vector2(0, 50)

func visualize_plane(origin: Vector2, plane_value: Plane) -> Vector2:
	var center = origin + Vector2(20, 20)
	draw_axes_3d(center, Vector3(20, 20, 20))

	var plane_corners := PackedVector2Array([
		center + project_2d(plane_value.project(Vector3(-1.0, 0.0, -1.0) * 10)),
		center + project_2d(plane_value.project(Vector3(1.0, 0.0, -1.0) * 10)),
		center + project_2d(plane_value.project(Vector3(1.0, 0.0, 1.0) * 10)),
		center + project_2d(plane_value.project(Vector3(-1.0, 0.0, 1.0) * 10)),
		center + project_2d(plane_value.project(Vector3(-1.0, 0.0, -1.0) * 10))
	])

	draw_polyline(plane_corners, Color.WHITE)

	return Vector2(0, 60)

func visualize_quaternion(origin: Vector2, quaternion_value: Quaternion) -> Vector2:
	var center = origin + Vector2(20, 20)
	draw_axes_3d(center, Vector3(20, 20, 20))
	
	draw_line(center, center + project_2d(quaternion_value.normalized() * (Vector3.UP * -15)), Color.WHITE)

	return Vector2(0, 60)

func visualize_color(origin: Vector2, color_value: Color) -> Vector2:
	draw_circle(origin + Vector2(20, 15), 10, color_value)
	return Vector2(0, 50)

func visualize_aabb(origin: Vector2, aabb_value: AABB) -> Vector2:
	var center = origin + Vector2(20, 20)
	draw_axes_3d(center, Vector3(20, 20, 20))

	var aabb_verts := []
	for i in range(0, 8):
		aabb_verts.append(aabb_value.get_endpoint(i))

	draw_line(center + project_2d(aabb_verts[0]), center + project_2d(aabb_verts[1]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[0]), center + project_2d(aabb_verts[2]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[0]), center + project_2d(aabb_verts[4]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[1]), center + project_2d(aabb_verts[3]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[1]), center + project_2d(aabb_verts[5]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[2]), center + project_2d(aabb_verts[3]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[2]), center + project_2d(aabb_verts[6]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[3]), center + project_2d(aabb_verts[7]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[4]), center + project_2d(aabb_verts[5]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[4]), center + project_2d(aabb_verts[6]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[5]), center + project_2d(aabb_verts[7]), Color.WHITE)
	draw_line(center + project_2d(aabb_verts[6]), center + project_2d(aabb_verts[7]), Color.WHITE)

	return Vector2(0, 60)

func visualize_basis(origin: Vector2, basis_value: Basis) -> Vector2:
	var center = origin + Vector2(20, 20)
	draw_axes_3d(center, Vector3(20, 20, 20))

	draw_line(center, center + project_2d(basis_value.x * 15.0), Color.DARK_GRAY, 3.0)
	draw_line(center, center + project_2d(basis_value.y * 15.0), Color.DARK_GRAY, 3.0)
	draw_line(center, center + project_2d(basis_value.z * 15.0), Color.DARK_GRAY, 3.0)

	draw_line(center, center + project_2d(basis_value.x * 15.0), Color.RED, 1.0)
	draw_line(center, center + project_2d(basis_value.y * 15.0), Color.GREEN, 1.0)
	draw_line(center, center + project_2d(basis_value.z * 15.0), Color.LIGHT_BLUE, 1.0)

	return Vector2(0, 60)

func visualize_transform(origin: Vector2, transform3d_value: Transform3D) -> Vector2:
	var center = origin + Vector2(20, 20)
	draw_axes_3d(center, Vector3(20, 20, 20))

	draw_line(center + project_2d(transform3d_value.origin), center + project_2d(transform3d_value.origin + transform3d_value.basis.x * 15.0), Color.DARK_GRAY, 3.0)
	draw_line(center + project_2d(transform3d_value.origin), center + project_2d(transform3d_value.origin + transform3d_value.basis.y * 15.0), Color.DARK_GRAY, 3.0)
	draw_line(center + project_2d(transform3d_value.origin), center + project_2d(transform3d_value.origin + transform3d_value.basis.z * 15.0), Color.DARK_GRAY, 3.0)

	draw_line(center + project_2d(transform3d_value.origin), center + project_2d(transform3d_value.origin + transform3d_value.basis.x * 15.0), Color.RED, 1.0)
	draw_line(center + project_2d(transform3d_value.origin), center + project_2d(transform3d_value.origin + transform3d_value.basis.y * 15.0), Color.GREEN, 1.0)
	draw_line(center + project_2d(transform3d_value.origin), center + project_2d(transform3d_value.origin + transform3d_value.basis.z * 15.0), Color.BLUE, 1.0)

	return Vector2(0, 60)

func visualize_array(origin: Vector2, array) -> Vector2:
	var local_origin = Vector2.ZERO

	for i in range(0, array.size()):
		var value = array[i]
		local_origin += draw_heading(origin + local_origin, str(i))
		if value is bool:
			local_origin += visualize_bool(origin + local_origin, value)
		elif value is int:
			local_origin += visualize_int(origin + local_origin, value)
		elif value is float:
			local_origin += visualize_float(origin + local_origin, value)
		elif value is String:
			local_origin += visualize_string(origin + local_origin, value)
		elif value is Vector2:
			local_origin += visualize_vector2(origin + local_origin, value)
		elif value is Rect2:
			local_origin += visualize_rect2(origin + local_origin, value)
		elif value is Vector3:
			local_origin += visualize_vector3(origin + local_origin, value)
		elif value is Transform2D:
			local_origin += visualize_transform2d(origin + local_origin, value)
		elif value is Plane:
			local_origin += visualize_plane(origin + local_origin, value)
		elif value is Quaternion:
			local_origin += visualize_quaternion(origin + local_origin, value)
		elif value is Color:
			local_origin += visualize_color(origin + local_origin, value)
		elif value is AABB:
			local_origin += visualize_aabb(origin + local_origin, value)
		elif value is Basis:
			local_origin += visualize_basis(origin + local_origin, value)
		elif value is Transform3D:
			local_origin += visualize_transform(origin + local_origin, value)
		elif value is Array:
			local_origin += visualize_array(origin + local_origin, value)

	return local_origin

func draw_axes_3d(center: Vector2, extents: Vector3) -> void:
	draw_line(center + project_2d(Vector3.LEFT) * extents.x, center + project_2d(Vector3.RIGHT) * extents.x, Color.RED)
	draw_line(center + project_2d(Vector3.UP) * extents.y, center + project_2d(Vector3.DOWN) * extents.y, Color.GREEN)
	draw_line(center + project_2d(Vector3.FORWARD) * extents.z, center + project_2d(Vector3.BACK) * extents.z, Color.LIGHT_BLUE)

func project_2d(v: Vector3) -> Vector2:
	return Vector2(v.x + v.z * 0.5, v.y - v.z * 0.5)
