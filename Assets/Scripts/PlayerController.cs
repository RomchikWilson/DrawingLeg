using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References objects")]
    [SerializeField] private GameObject leftLine = default;
    [SerializeField] private GameObject rightLine = default;
    [SerializeField] private Transform cameraTarget = default;
    [SerializeField] private LineRenderer lineRenderer = default;

    [Header("Settings")]
    [SerializeField] private float lineThickness = default;
    [SerializeField] private float velocityPlayer = 1f;

    //Компоненты объекта
    private Animator animator;
    private Rigidbody rigidBody;

    [HideInInspector]
    public bool freezeGame = false;
    private Vector3 startingPosition;

    private Mesh meshLeft;
    private Mesh meshRight;

    public static Action<Vector3[], int> DrawLines = default;

    private void Awake()
    {
        //Получаем компоненты объекта
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        meshLeft = leftLine.GetComponent<MeshFilter>().mesh;
        meshRight = rightLine.GetComponent<MeshFilter>().mesh;

        //Запоминаем стартовые данные
        startingPosition = transform.position;
    }

    private void OnEnable()
    {
        DrawLines += CleanAndDrawLines;
        GameManager.FreezeGame += FreezeGame;
        GameManager.UnfreezeGame += UnfreezeGame;
    }

    private void OnDisable()
    {
        DrawLines -= CleanAndDrawLines;
        GameManager.FreezeGame -= FreezeGame;
        GameManager.UnfreezeGame -= UnfreezeGame;
    }

    private void Start()
    {
        CameraController.SetTargetAction?.Invoke(cameraTarget);
        animator.SetInteger("type_of_movement", 1);
    }

    private void FixedUpdate()
    {
        if (!freezeGame)
        {
            transform.Translate(Vector3.forward * velocityPlayer * Time.deltaTime);
        }
    }

    private void FreezeGame()
    {
        freezeGame = true;
        rigidBody.velocity = Vector3.zero;
        rigidBody.isKinematic = true;
        animator.speed = 0;
    }

    private void UnfreezeGame()
    {
        freezeGame = false;
        rigidBody.isKinematic = false;
        animator.speed = 1;
    }

    private void CleanAndDrawLines(Vector3[] pointsPosition, int countPoints)
    {
        //Узнаем самую высокую и низкую точки LineRenderer
        Vector3 maxY = new Vector3(0f, -100f, 0f);
        Vector3 minY = new Vector3(0f, 100f, 0f);

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = lineRenderer.GetPosition(i);

            if (pos.y > maxY.y)
            {
                maxY = pos;
            }
            
            if (pos.y < minY.y)
            {
                minY = pos;
            }
        }

        //Подготавливаем LineRenderer перед запеканием в Mesh
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = lineRenderer.GetPosition(i);

            lineRenderer.SetPosition(i, new Vector3(0f, pos.y - maxY.y, pos.z - maxY.z));
        }

        //Запекаем линию с LineRenderer в Mesh
        lineRenderer.BakeMesh(rightLine.GetComponent<MeshFilter>().mesh);

        //Получаем Mesh с линии правой ноги
        Mesh mesh_rightLine = rightLine.GetComponent<MeshFilter>().mesh;

        //Получаем вершины Mesh
        List<Vector3> vertices_rightLine_Left = new List<Vector3>();
        for (int i = 0; i < mesh_rightLine.vertices.Length; i++)
        {
            vertices_rightLine_Left.Add(mesh_rightLine.vertices[i]);
        }

        //Получаем треугольники Mesh
        List<int> triangles_rightLine_Left = new List<int>();
        for (int i = 0; i < mesh_rightLine.triangles.Length; i++)
        {
            triangles_rightLine_Left.Add(mesh_rightLine.triangles[i]);
        }

        //Создаем нужные нам локальные переменные для генерации меша
        int countVertices = 0;
        int countOldVertices = vertices_rightLine_Left.Count;
        bool firstCube = true;

        for (int i = 0; i <= countOldVertices; i++)
        {
            if ((firstCube ? i % 4 != 0 : i % 2 != 0) || i == 0) continue;

            //Генерация меша вначале
            if (firstCube)
            {
                vertices_rightLine_Left.Add(vertices_rightLine_Left[i - 4]);
                vertices_rightLine_Left.Add(vertices_rightLine_Left[i - 3]);
                vertices_rightLine_Left.Add(new Vector3(vertices_rightLine_Left[i - 3].x - 0.05f, vertices_rightLine_Left[i - 3].y, vertices_rightLine_Left[i - 3].z));
                vertices_rightLine_Left.Add(new Vector3(vertices_rightLine_Left[i - 4].x - 0.05f, vertices_rightLine_Left[i - 4].y, vertices_rightLine_Left[i - 4].z));

                countVertices = vertices_rightLine_Left.Count;

                triangles_rightLine_Left.Add(countVertices - 4);  //0
                triangles_rightLine_Left.Add(countVertices - 2);  //2
                triangles_rightLine_Left.Add(countVertices - 3);  //1

                triangles_rightLine_Left.Add(countVertices - 4);  //0
                triangles_rightLine_Left.Add(countVertices - 1);  //3
                triangles_rightLine_Left.Add(countVertices - 2);  //2
            }

            //Генерация меша сверху
            vertices_rightLine_Left.Add(vertices_rightLine_Left[i - 3]);
            vertices_rightLine_Left.Add(vertices_rightLine_Left[i - 1]);
            vertices_rightLine_Left.Add(new Vector3(vertices_rightLine_Left[i - 1].x - 0.05f, vertices_rightLine_Left[i - 1].y, vertices_rightLine_Left[i - 1].z));
            vertices_rightLine_Left.Add(new Vector3(vertices_rightLine_Left[i - 3].x - 0.05f, vertices_rightLine_Left[i - 3].y, vertices_rightLine_Left[i - 3].z));

            countVertices = vertices_rightLine_Left.Count;

            triangles_rightLine_Left.Add(countVertices - 4);  //0
            triangles_rightLine_Left.Add(countVertices - 2);  //2
            triangles_rightLine_Left.Add(countVertices - 3);  //1

            triangles_rightLine_Left.Add(countVertices - 4);  //0
            triangles_rightLine_Left.Add(countVertices - 1);  //3
            triangles_rightLine_Left.Add(countVertices - 2);  //2

            //Генерация меша снизу
            vertices_rightLine_Left.Add(vertices_rightLine_Left[i - 4]);
            vertices_rightLine_Left.Add(vertices_rightLine_Left[i - 2]);
            vertices_rightLine_Left.Add(new Vector3(vertices_rightLine_Left[i - 2].x - 0.05f, vertices_rightLine_Left[i - 2].y, vertices_rightLine_Left[i - 2].z));
            vertices_rightLine_Left.Add(new Vector3(vertices_rightLine_Left[i - 4].x - 0.05f, vertices_rightLine_Left[i - 4].y, vertices_rightLine_Left[i - 4].z));

            countVertices = vertices_rightLine_Left.Count;

            triangles_rightLine_Left.Add(countVertices - 4);  //0
            triangles_rightLine_Left.Add(countVertices - 2);  //2
            triangles_rightLine_Left.Add(countVertices - 3);  //1

            triangles_rightLine_Left.Add(countVertices - 4);  //0
            triangles_rightLine_Left.Add(countVertices - 1);  //3
            triangles_rightLine_Left.Add(countVertices - 2);  //2

            //Генерация меша в конце
            if (i == countOldVertices)
            {
                vertices_rightLine_Left.Add(vertices_rightLine_Left[i - 2]);
                vertices_rightLine_Left.Add(vertices_rightLine_Left[i - 1]);
                vertices_rightLine_Left.Add(new Vector3(vertices_rightLine_Left[i - 1].x - 0.05f, vertices_rightLine_Left[i - 1].y, vertices_rightLine_Left[i - 1].z));
                vertices_rightLine_Left.Add(new Vector3(vertices_rightLine_Left[i - 2].x - 0.05f, vertices_rightLine_Left[i - 2].y, vertices_rightLine_Left[i - 2].z));

                countVertices = vertices_rightLine_Left.Count;

                triangles_rightLine_Left.Add(countVertices - 4);  //0
                triangles_rightLine_Left.Add(countVertices - 2);  //2
                triangles_rightLine_Left.Add(countVertices - 3);  //1

                triangles_rightLine_Left.Add(countVertices - 4);  //0
                triangles_rightLine_Left.Add(countVertices - 1);  //3
                triangles_rightLine_Left.Add(countVertices - 2);  //2
            }

            firstCube = false;
        }

        //Конвертируем List в массив
        Vector3[] vertices = new Vector3[vertices_rightLine_Left.Count];
        for (int i = 0; i < vertices_rightLine_Left.Count; i++)
        {
            vertices[i] = vertices_rightLine_Left[i];
        }

        int[] triangles = new int[triangles_rightLine_Left.Count];
        for (int i = 0; i < triangles_rightLine_Left.Count; i++)
        {
            triangles[i] = triangles_rightLine_Left[i];
        }

        ////Чистим и заполняем новыми вершинами и треугольниками MeshFilters
        meshLeft.Clear();
        meshRight.Clear();

        meshLeft.vertices = vertices;
        meshRight.vertices = vertices;

        meshLeft.triangles = triangles;
        meshRight.triangles = triangles;

        meshLeft.Optimize();
        meshRight.Optimize();

        meshLeft.RecalculateNormals();
        meshRight.RecalculateNormals();

        //Заполняем Colliders
        leftLine.GetComponent<MeshCollider>().sharedMesh = meshLeft;
        rightLine.GetComponent<MeshCollider>().sharedMesh = meshRight;

        //Телепортируем персонажа вверх
        transform.position = transform.position + (Vector3.up * Math.Abs(maxY.y - minY.y));
    }

    void OnTriggerEnter(Collider collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Finish":
                transform.position = startingPosition;

                leftLine.GetComponent<MeshCollider>().sharedMesh = null;
                rightLine.GetComponent<MeshCollider>().sharedMesh = null;

                meshLeft.Clear();
                meshRight.Clear();

                break;

            default:
                break;
        }
    }
}

