mergeInto(LibraryManager.library, {

  SendInputToWeb: function (content) {
    getUnityInput(UTF8ToString(content))
  },

  jslib_inside_triangle: function(c_triangle_json_str, c_pixel_json_str){
    RustInsideTriangle(UTF8ToString(c_triangle_json_str), UTF8ToString(c_pixel_json_str))
    return return_c_str
  },

  jslib_bounding_box: function(c_triangle_json_str){
    RustboundingBox(UTF8ToString(c_triangle_json_str))
    return return_c_str
  },

});
