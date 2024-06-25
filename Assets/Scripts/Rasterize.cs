using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using Newtonsoft.Json;


public struct BoundingBox
{
    public float min_x;
    public float min_y;
    public float max_x;
    public float max_y;
    public BoundingBox(float min_x, float min_y, float max_x, float max_y)
    {
        this.min_x = min_x;
        this.min_y = min_y;
        this.max_x = max_x;
        this.max_y = max_y;
    }
}


public struct PixelCoord
{
    public float x;
    public float y;

    public PixelCoord(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}


public struct Vector3f
{
    public float x;
    public float y;
    public float z;

    public Vector3f(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    // 计算向量的叉积
    public Vector3f Cross(Vector3f other)
    {
        return new Vector3f(
            this.y * other.z - this.z * other.y,
            this.z * other.x - this.x * other.z,
            this.x * other.y - this.y * other.x
        );
    }

    // 计算向量的点积
    public float Dot(Vector3f other)
    {
        return this.x * other.x + this.y * other.y + this.z * other.z;
    }

    // 向量相减
    public Vector3f Sub(Vector3f other)
    {
        return new Vector3f(
            this.x - other.x,
            this.y - other.y,
            this.z - other.z
        );
    }

}

public class Rasterize : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern string jslib_inside_triangle(string c_triangle_json_str, string c_pixel_json_str);
    [DllImport("__Internal")]
    private static extern string jslib_bounding_box(string c_triangle_json_str);


    public static Vector3f[] pixelTriangle;

    // 像素父物体(屏幕)
    public Transform parentObject;
    public GameObject metaTriangle;

    // 平面原始颜色
    private Color parentObjectColor;
    private BoundingBox boundingBox;
    private PixelCoord pixelCoord;


    // 定义顶点本地坐标数组
    private Vector3[] localPos;
    // 定义顶点世界坐标数组
    private Vector3[] worldPos;
    // 3维向量的数组, 用于存储顶点.
    private List<Vector3> worldPosList = new List<Vector3>();

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // 偶数帧执行
        if (Time.frameCount % 2 == 0)
        {
            // 获取顶点坐标
            getVerticalCoordinate();

            // 获取正方形外框
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                TriangleUtils.BoundingBox(pixelTriangle, out boundingBox);

            }
            else if (Application.platform == RuntimePlatform.Android)
            {

            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                TriangleUtils.BoundingBox(pixelTriangle, out boundingBox);

            }
            else
            {
                Debug.LogError("没有匹配到运行的平台");
            }

            foreach (Transform child in parentObject)
            {
                // 获取显示器每一个像素的世界坐标
                pixelCoord = new PixelCoord(child.position.x, child.position.y);

                // 判断bounding_box
                if (pixelCoord.x >= boundingBox.min_x && pixelCoord.x <= boundingBox.max_x && pixelCoord.y >= boundingBox.min_y && pixelCoord.y <= boundingBox.max_y)
                {
                    // 检查子物体的坐标是否在三角形内
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        if (TriangleUtils.InsideTriangle(pixelTriangle, pixelCoord))
                        {
                            Renderer rendererChild = child.GetComponent<Renderer>();
                            if (rendererChild != null)
                            {
                                // 投影的颜色设置为绿色
                                rendererChild.material.SetColor("_BaseColor", new Color(0.5f, 0.1f, 0.1f, 1f));
                            }
                        }

                    }
                    else if (Application.platform == RuntimePlatform.Android)
                    {

                    }
                    else if (Application.platform == RuntimePlatform.OSXEditor)
                    {
                        if (TriangleUtils.InsideTriangle(pixelTriangle, pixelCoord))
                        {
                            Renderer rendererChild = child.GetComponent<Renderer>();
                            if (rendererChild != null)
                            {
                                // 投影的颜色设置为绿色
                                rendererChild.material.SetColor("_BaseColor", new Color(0.5f, 0.1f, 0.1f, 1f));
                            }
                        }

                    }
                    else
                    {
                        Debug.LogError("没有匹配到运行的平台");
                    }

                }
            }
        }
        else
        {
            foreach (Transform child in parentObject)
            {
                // 检查子物体的坐标是否在三角形内
                Renderer rendererChild = child.GetComponent<Renderer>();
                if (rendererChild != null)
                {
                    // 投影的颜色设置为绿色
                    rendererChild.material.SetColor("_BaseColor", new Color(0.5451f, 0.7176f, 0.6941f, 1f));
                }
            }
        }
    }
    public void getVerticalCoordinate()
    {
        // 获取每个顶点本地坐标
        localPos = metaTriangle.GetComponent<MeshFilter>().mesh.vertices;
        // 获取每个顶点世界坐标

        foreach (var localVertical in localPos)
        {
            var worldVertical = metaTriangle.transform.TransformPoint(localVertical);
            worldPosList.Add(worldVertical);
        }
        // 把Vector3的列表转为Vector3的数组!
        worldPos = worldPosList.ToArray();
        worldPosList.Clear();

        // 传入顶点坐标
        pixelTriangle = new Vector3f[]
        {
                new Vector3f(worldPos[0].x, worldPos[0].y, worldPos[0].z * 0),
                new Vector3f(worldPos[1].x, worldPos[1].y, worldPos[1].z * 0),
                new Vector3f(worldPos[2].x, worldPos[2].y, worldPos[2].z * 0),
        };
    }

}



public class TriangleUtils
{

    // 计算三角形的边界矩形
    public static void BoundingBox(Vector3f[] v, out BoundingBox boundingBox)
    {
        if (v.Length != 3)
        {
            throw new System.ArgumentException("The array 'v' must contain exactly 3 elements.");
        }

        boundingBox.min_x = Mathf.Min(v[0].x, v[1].x, v[2].x);
        boundingBox.min_y = Mathf.Min(v[0].y, v[1].y, v[2].y);
        boundingBox.max_x = Mathf.Max(v[0].x, v[1].x, v[2].x);
        boundingBox.max_y = Mathf.Max(v[0].y, v[1].y, v[2].y);
    }

    // 判断点是否在三角形内
    public static bool InsideTriangle(Vector3f[] v, PixelCoord pixelCoord)
    {
        if (v.Length != 3)
        {
            throw new System.ArgumentException("The array 'v' must contain exactly 3 elements.");
        }

        Vector3f q = new Vector3f(pixelCoord.x, pixelCoord.y, 0.0f);
        Vector3f ab = v[1].Sub(v[0]);
        Vector3f bc = v[2].Sub(v[1]);
        Vector3f ca = v[0].Sub(v[2]);
        Vector3f aq = q.Sub(v[0]);
        Vector3f bq = q.Sub(v[1]);
        Vector3f cq = q.Sub(v[2]);

        return ab.Cross(aq).Dot(bc.Cross(bq)) > 0.0f &&
               ab.Cross(aq).Dot(ca.Cross(cq)) > 0.0f &&
               bc.Cross(bq).Dot(ca.Cross(cq)) > 0.0f;
    }
}

