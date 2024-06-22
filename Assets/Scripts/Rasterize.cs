using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

public class rasterize : MonoBehaviour
{
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
    }

    // Rust dylib
    [DllImport("color_houkan")]
    private static extern bool inside_triangle(float x, float y, Vector3f[] v);
    [DllImport("color_houkan")]
    private static extern void bounding_box(Vector3f[] v, out float min_x, out float min_y, out float max_x, out float max_y);


    // 3维向量的数组, 用于存储顶点.
    private Vector3[] localPos;  // 定义顶点本地坐标数组
    List<Vector3> worldPosList = new List<Vector3>();
    private Vector3[] worldPos;     // 定义顶点世界坐标数组
    private Vector3f[] pixelTriangle;
    // 像素父物体
    public Transform parentObject;
    public GameObject metaTriangle;

    // jslib
    [DllImport("__Internal")]
    private static extern void SendInputToWeb(string content);

    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        // SendInputToWeb("这段话来自C#, 需要显示在网页上");


        // 获取每个顶点本地坐标
        localPos = metaTriangle.GetComponent<MeshFilter>().sharedMesh.vertices;
        // 获取每个顶点世界坐标

        foreach (var localVertical in localPos)
        {
            var worldVertical = metaTriangle.transform.TransformPoint(localVertical);
            worldPosList.Add(worldVertical);
        }
        // 把Vector3的列表转为Vector3的数组!
        worldPos = worldPosList.ToArray();

        // 传入顶点坐标
        pixelTriangle = new Vector3f[]
        {
                new Vector3f(worldPos[0].x, worldPos[0].y, worldPos[0].z * 0),
                new Vector3f(worldPos[1].x, worldPos[1].y, worldPos[1].z * 0),
                new Vector3f(worldPos[2].x, worldPos[2].y, worldPos[2].z * 0),
        };
        // 获取正方形外框
        bounding_box(pixelTriangle, out float min_x, out float min_y, out float max_x, out float max_y);

        foreach (Transform child in parentObject)
        {
            // 获取显示器每一个像素的世界坐标
            Vector3 position = child.position;
            // 判断bounding_box
            if (position.x >= min_x && position.x <= max_x && position.y >= min_y && position.y <= max_y)
            {
                // 检查子物体的坐标是否在三角形内
                if (inside_triangle(position.x, position.y, pixelTriangle))
                {
                    Renderer rendererChild = child.GetComponent<Renderer>();
                    if (rendererChild != null)
                    {
                        // 投影的颜色设置为绿色
                        rendererChild.material.SetColor("_BaseColor", new Color(0f, 0.5f, 0.2f, 1f));
                    }
                }
            }
        }
    }
}

