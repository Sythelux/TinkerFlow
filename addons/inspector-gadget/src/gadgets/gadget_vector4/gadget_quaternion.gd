@tool
extends GadgetVector4
class_name GadgetQuaternion

static func supports_type(value) -> bool:
	return value is Quaternion
