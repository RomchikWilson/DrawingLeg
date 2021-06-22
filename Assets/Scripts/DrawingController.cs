using UnityEngine;
using UnityEngine.EventSystems;

public class DrawingController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform playerTransfrom = default;

    //Компоненты объекта
    private EventTrigger trigger = default;
    private LineRenderer lineRenderer;

    private Vector3[] pointsPosition;
    private bool press = false;

    void Awake()
    {
        //Получаем нужные нам компоненты объектов
        lineRenderer = GetComponent<LineRenderer>();
        trigger = GetComponent<EventTrigger>();

        //Заполняем компонент ивентами EventTrigger
        EventTrigger.Entry _pointerDown = new EventTrigger.Entry();
        _pointerDown.eventID = EventTriggerType.PointerDown;
        _pointerDown.callback.AddListener((data) =>
        {
            OnPointerDown((PointerEventData)data);
        });

        EventTrigger.Entry _pointerDrag = new EventTrigger.Entry();
        _pointerDrag.eventID = EventTriggerType.Drag;
        _pointerDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });

        EventTrigger.Entry _pointerUp = new EventTrigger.Entry();
        _pointerUp.eventID = EventTriggerType.PointerUp;
        _pointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });

        EventTrigger.Entry _pointerExit = new EventTrigger.Entry();
        _pointerExit.eventID = EventTriggerType.PointerExit;
        _pointerExit.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });

        trigger.triggers.Add(_pointerDown);
        trigger.triggers.Add(_pointerDrag);
        trigger.triggers.Add(_pointerUp);
        trigger.triggers.Add(_pointerExit);
    }

    public void OnPointerDown(PointerEventData data)
    {
        press = true;

        //Задаём первую точку для отрисовки на LineRenderer
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(0, cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z + 4f)));

        //Останавливаем игру пока рисуем линию
        GameManager.FreezeGame?.Invoke();
    }

    public void OnDrag(PointerEventData data)
    {
        if (!press) return;
        //Задаём следующую точку для отрисовки на LineRenderer
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z + 4f)));
    }

    public void OnPointerUp(PointerEventData data)
    {
        _endDrag();
    }

    public void OnPointerExit(PointerEventData data)
    {
        _endDrag();
    }

    private void _endDrag()
    {
        if (!press) return;

        //Создаем и заполняем массив точками позиций
        pointsPosition = new Vector3[lineRenderer.positionCount];

        float deltaPosition = 0f;

        if (lineRenderer.positionCount > 0) deltaPosition = lineRenderer.GetPosition(0).z;

        for (int i = 0; i<lineRenderer.positionCount; i++)
        {
            pointsPosition[i] = lineRenderer.GetPosition(i);
            pointsPosition[i].z = pointsPosition[i].z - deltaPosition;
        }

        //Отрисовываем линии в MeshLines
        PlayerController.DrawLines?.Invoke(pointsPosition, lineRenderer.positionCount);

        //Очищаем точки отрисовки LineRenderer
        lineRenderer.positionCount = 0;

        //Продолжаем игру когда линия уже нарисована
        GameManager.UnfreezeGame?.Invoke();

        press = false;
    }
}