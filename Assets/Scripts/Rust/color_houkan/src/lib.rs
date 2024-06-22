#[derive(Debug, Copy, Clone)]
pub struct Vector3f {
    x: f32,
    y: f32,
    z: f32,
}

impl Vector3f {
    // 创建新的Vector3f
    pub fn new(x: f32, y: f32, z: f32) -> Self {
        Vector3f { x, y, z }
    }
    
    // 计算向量的叉积
    fn cross(self, other: Vector3f) -> Vector3f {
        Vector3f {
            x: self.y * other.z - self.z * other.y,
            y: self.z * other.x - self.x * other.z,
            z: self.x * other.y - self.y * other.x,
        }
    }

    // 计算向量的点积
    fn dot(self, other: Vector3f) -> f32 {
        self.x * other.x + self.y * other.y + self.z * other.z
    }

    // 向量相减
    fn sub(self, other: Vector3f) -> Vector3f {
        Vector3f {
            x: self.x - other.x,
            y: self.y - other.y,
            z: self.z - other.z,
        }
    }
}

// 判断点是否在三角形内
#[no_mangle]
pub extern fn inside_triangle(x: f32, y: f32, v: *const Vector3f) -> bool {
    let v = unsafe { std::slice::from_raw_parts(v, 3) };
    let q = Vector3f::new(x, y, 0.0);
    let ab = v[1].sub(v[0]);
    let bc = v[2].sub(v[1]);
    let ca = v[0].sub(v[2]);
    let aq = q.sub(v[0]);
    let bq = q.sub(v[1]);
    let cq = q.sub(v[2]);
    ab.cross(aq).dot(bc.cross(bq)) > 0.0 && ab.cross(aq).dot(ca.cross(cq)) > 0.0 && bc.cross(bq).dot(ca.cross(cq)) > 0.0
}

// 计算三角形的边界矩形
#[no_mangle]
pub extern fn bounding_box(v: *const Vector3f, min_x: *mut f32, min_y: *mut f32, max_x: *mut f32, max_y: *mut f32) {
    let v = unsafe { std::slice::from_raw_parts(v, 3) };
    let (min_x_val, min_y_val, max_x_val, max_y_val) = (
        v.iter().map(|vec| vec.x).fold(f32::INFINITY, f32::min),
        v.iter().map(|vec| vec.y).fold(f32::INFINITY, f32::min),
        v.iter().map(|vec| vec.x).fold(f32::NEG_INFINITY, f32::max),
        v.iter().map(|vec| vec.y).fold(f32::NEG_INFINITY, f32::max),
    );
    unsafe {
        *min_x = min_x_val;
        *min_y = min_y_val;
        *max_x = max_x_val;
        *max_y = max_y_val;
    }
}
