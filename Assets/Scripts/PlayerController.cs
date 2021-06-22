using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References objects")]
    [SerializeField] private GameObject leftLine = default;
    [SerializeField] private GameObject rightLine = default;
    [SerializeField] private Transform cameraTarget = default;

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
        //Создаем локальные переменные
        Vector3 maxY = new Vector3(0f, -100f, 0f);
        Vector3 minY = new Vector3(0f, 100f, 0f);

        Vector3[] vertices = new Vector3[(countPoints - 1) * 8];
        int[] triangls = new int[(countPoints - 1) * 36];

        int indexPointsVertices = 0;
        int indexPointsTriangls = 0;
        int index = 0;
        int toAdd;
        Vector3 previousPosition = Vector3.zero;

        //Найдём самую высокую точку
        foreach (Vector3 _position in pointsPosition)
        {
            if (_position.y > maxY.y)
            {
                maxY = _position;
            } else if (_position.y < minY.y)
            {
                minY = _position;
            }        
        }

        //Создаем точки вершин и треугольников для MeshFilters
        foreach (Vector3 _position in pointsPosition)
        {
            if (previousPosition == Vector3.zero)
            {
                previousPosition = _position;
                continue;
            }

            //if (_position.y - previousPosition.y > _position.x - previousPosition.x)
            //{
                vertices[indexPointsVertices == 0 ? 0 : ++indexPointsVertices] = new Vector3(previousPosition.z - lineThickness - maxY.z, previousPosition.y + lineThickness - maxY.y, 0 - lineThickness);
                vertices[++indexPointsVertices] = new Vector3(previousPosition.z - lineThickness - maxY.z, previousPosition.y + lineThickness - maxY.y, 0 + lineThickness);
                vertices[++indexPointsVertices] = new Vector3(_position.z - lineThickness - maxY.z, _position.y + lineThickness - maxY.y, 0 + lineThickness);
                vertices[++indexPointsVertices] = new Vector3(_position.z - lineThickness - maxY.z, _position.y + lineThickness - maxY.y, 0 - lineThickness);
                vertices[++indexPointsVertices] = new Vector3(_position.z + lineThickness - maxY.z, _position.y + lineThickness - maxY.y, 0 - lineThickness);
                vertices[++indexPointsVertices] = new Vector3(_position.z + lineThickness - maxY.z, _position.y + lineThickness - maxY.y, 0 + lineThickness);
                vertices[++indexPointsVertices] = new Vector3(previousPosition.z + lineThickness - maxY.z, previousPosition.y + lineThickness - maxY.y, 0 + lineThickness);
                vertices[++indexPointsVertices] = new Vector3(previousPosition.z + lineThickness - maxY.z, previousPosition.y + lineThickness - maxY.y, 0 - lineThickness);
            //}
            //else if (_position.y - previousPosition.y < _position.x - previousPosition.x)
            //{
            //    vertices[indexPointsVertices == 0 ? 0 : ++indexPointsVertices] = new Vector3(0 - lineThickness, previousPosition.y - lineThickness, previousPosition.z + lineThickness);
            //    vertices[++indexPointsVertices] = new Vector3(0 + lineThickness, previousPosition.y - lineThickness, previousPosition.z + lineThickness);
            //    vertices[++indexPointsVertices] = new Vector3(0 + lineThickness, previousPosition.y + lineThickness, previousPosition.z + lineThickness);
            //    vertices[++indexPointsVertices] = new Vector3(0 - lineThickness, previousPosition.y + lineThickness, previousPosition.z + lineThickness);
            //    vertices[++indexPointsVertices] = new Vector3(0 - lineThickness, _position.y + lineThickness, _position.z + lineThickness);
            //    vertices[++indexPointsVertices] = new Vector3(0 + lineThickness, _position.y + lineThickness, _position.z + lineThickness);
            //    vertices[++indexPointsVertices] = new Vector3(0 + lineThickness, _position.y - lineThickness, _position.z + lineThickness);
            //    vertices[++indexPointsVertices] = new Vector3(0 - lineThickness, _position.y - lineThickness, _position.z + lineThickness);
            //}

            toAdd = (index * 8);

            triangls[indexPointsTriangls == 0 ? 0 : ++indexPointsTriangls] = 0 + toAdd;
            triangls[++indexPointsTriangls] = 2 + toAdd;
            triangls[++indexPointsTriangls] = 1 + toAdd;
            triangls[++indexPointsTriangls] = 0 + toAdd;
            triangls[++indexPointsTriangls] = 3 + toAdd;
            triangls[++indexPointsTriangls] = 2 + toAdd;
            triangls[++indexPointsTriangls] = 2 + toAdd;
            triangls[++indexPointsTriangls] = 3 + toAdd;
            triangls[++indexPointsTriangls] = 4 + toAdd;
            triangls[++indexPointsTriangls] = 2 + toAdd;
            triangls[++indexPointsTriangls] = 4 + toAdd;
            triangls[++indexPointsTriangls] = 5 + toAdd;
            triangls[++indexPointsTriangls] = 1 + toAdd;
            triangls[++indexPointsTriangls] = 2 + toAdd;
            triangls[++indexPointsTriangls] = 5 + toAdd;
            triangls[++indexPointsTriangls] = 1 + toAdd;
            triangls[++indexPointsTriangls] = 5 + toAdd;
            triangls[++indexPointsTriangls] = 6 + toAdd;
            triangls[++indexPointsTriangls] = 0 + toAdd;
            triangls[++indexPointsTriangls] = 7 + toAdd;
            triangls[++indexPointsTriangls] = 4 + toAdd;
            triangls[++indexPointsTriangls] = 0 + toAdd;
            triangls[++indexPointsTriangls] = 4 + toAdd;
            triangls[++indexPointsTriangls] = 3 + toAdd;
            triangls[++indexPointsTriangls] = 5 + toAdd;
            triangls[++indexPointsTriangls] = 4 + toAdd;
            triangls[++indexPointsTriangls] = 7 + toAdd;
            triangls[++indexPointsTriangls] = 5 + toAdd;
            triangls[++indexPointsTriangls] = 7 + toAdd;
            triangls[++indexPointsTriangls] = 6 + toAdd;
            triangls[++indexPointsTriangls] = 0 + toAdd;
            triangls[++indexPointsTriangls] = 6 + toAdd;
            triangls[++indexPointsTriangls] = 7 + toAdd;
            triangls[++indexPointsTriangls] = 0 + toAdd;
            triangls[++indexPointsTriangls] = 1 + toAdd;
            triangls[++indexPointsTriangls] = 6 + toAdd;

            previousPosition = _position;
            index++;
        }

        //Получаем MeshFilters
        meshLeft = leftLine.GetComponent<MeshFilter>().mesh;
        meshRight = rightLine.GetComponent<MeshFilter>().mesh;

        //Чистим и заполняем новыми вершинами и треугольниками MeshFilters
        meshLeft.Clear();
        meshRight.Clear();

        meshLeft.vertices = vertices;
        meshRight.vertices = vertices;

        meshLeft.triangles = triangls;
        meshRight.triangles = triangls;

        meshLeft.Optimize();
        meshRight.Optimize();

        meshLeft.RecalculateNormals();
        meshRight.RecalculateNormals();

        //Заполняем Colliders
        leftLine.GetComponent<MeshCollider>().sharedMesh = meshLeft;
        rightLine.GetComponent<MeshCollider>().sharedMesh = meshRight;

        //transform.position = new Vector3(transform.position.x, transform.position.y + Math.Abs(maxY.y - minY.y), transform.position.z);
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
