using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class UiManager : MonoBehaviour
{
    private Canvas canvas;
    private Vector2 canvasSizeDelta;
    private Vector2 canvasOffset;
    private readonly Vector2 uiOffset = new Vector2(100, 100);

    private Dictionary<Airplane, TextMeshProUGUI> airplaneTextMap;
    private Dictionary<Station, TextMeshProUGUI> stationTextMap; // Does not support deleting stations

    private void Awake()
    {
        canvas = Prototypes.CreateCanvas();
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        
        canvasSizeDelta = canvasRectTransform.sizeDelta;
        
        canvasOffset = new Vector2(canvasSizeDelta.x * 0.5f, canvasSizeDelta.y * 0.5f);
        
        airplaneTextMap = new Dictionary<Airplane, TextMeshProUGUI>();
        stationTextMap = new Dictionary<Station, TextMeshProUGUI>();
        
        GameMaster.Instance.OnAirplaneCreate += OnAirplaneCreate;
        GameMaster.Instance.OnAirplaneDelete += OnAirplaneDelete;
        GameMaster.Instance.OnStationCreate += OnStationCreate;
    }

    private void Update()
    {
        UpdateAirplaneUi();
        UpdateStationUi();
    }


    private void UpdateAirplaneUi()
    {
        foreach (Airplane airplane in GameMaster.Instance.Airplanes)
        {
            if ( ! airplaneTextMap.ContainsKey(airplane))
            {
                Debug.LogError("UI text for airplane not found!");
                continue;
            }

            TextMeshProUGUI textObject = airplaneTextMap[airplane];

            SetPassengerCountString(textObject, airplane.Passengers, Airplane.Capacity);
            
            textObject.GetComponent<RectTransform>().localPosition = 
                CalculateViewportPosition(airplane.transform) + uiOffset;
        }
    }

    private void UpdateStationUi()
    {
        foreach (Station station in GameMaster.Instance.Stations)
        {
            if ( ! stationTextMap.ContainsKey(station))
            {
                Debug.LogError("UI text for station not found");
                continue;
            }

            TextMeshProUGUI textObject = stationTextMap[station];

            SetPassengerCountString(textObject, station.queue, Station.Capacity);

            textObject.GetComponent<RectTransform>().localPosition =
                CalculateViewportPosition(station.transform) + uiOffset;
        }
    }


    private static void SetPassengerCountString(TMP_Text textObject, IReadOnlyCollection<int> passengers, int capacity)
    {
        Dictionary<int, int> count = new Dictionary<int, int>();
        int total = 0;
        foreach (int passengerType in new HashSet<int>(passengers))
        {
            count[passengerType] = 0;
        }

        foreach (int passenger in passengers)
        {
            count[passenger]++;
            total++;
        }

        string newText = count.Aggregate("", (current, valuePair) => current + $"\"{valuePair.Key}\" = {valuePair.Value}\n");

        newText += $"{total}/{capacity}";
        
        textObject.text = newText;

        if (capacity <= total) { textObject.color = Color.red; }
        else if (textObject.color != Color.black) { textObject.color = Color.black; }
    }
    
    
    private Vector2 CalculateViewportPosition(Transform objectTransform)
    {
        Vector2 viewportPoint = GameMaster.Instance.MainCamera.WorldToViewportPoint(objectTransform.position);

        viewportPoint = new Vector2(viewportPoint.x * canvasSizeDelta.x, viewportPoint.y * canvasSizeDelta.y);

        return viewportPoint - canvasOffset;
    }


    private void OnAirplaneCreate(Airplane airplane)
    {
        airplaneTextMap[airplane] = Prototypes.CreateText(canvas.transform, Vector3.zero, "yo");
    }
    
    private void OnAirplaneDelete(Airplane airplane)
    {
        airplaneTextMap.Remove(airplane);
    }

    private void OnStationCreate(Station station)
    {
        stationTextMap[station] = Prototypes.CreateText(canvas.transform, Vector3.zero, "yo");
    }
}