using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameMaster : MonoBehaviour
{
    private Dictionary<int, RuntimeAnimatorController> shapeGraphicMap;
    
    private List<Station> stations;
    private List<Airplane> airplanes;

    private InputManager inputManager;
    private UiManager uiManager;
    
    
    
    public static GameMaster Instance;

    public bool gameOverEnabled = true;
    
    public LineFactory LinePool{ get; private set; }

    public Camera MainCamera { get; private set; }

    public event Action<Airplane> OnAirplaneCreate;
    public event Action<Airplane> OnAirplaneDelete;

    public event Action<Station> OnStationCreate;

    public ReadOnlyCollection<Airplane> Airplanes => new ReadOnlyCollection<Airplane>(airplanes);
    public ReadOnlyCollection<Station> Stations => new ReadOnlyCollection<Station>(stations);

    private int availableShapes = 2;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Duplicate GameMasters!");
            Destroy(Instance);
        }
        Instance = this;

        ConstructEmptyWorld();
        SetNewGameState();
        
        StartCoroutine(SlowUpdate());
    }

    
    /// <summary>
    /// Sets the GameMaster up with only essentials. No game data involved.
    /// </summary>
    private void ConstructEmptyWorld()
    {
        uiManager = gameObject.AddComponent<UiManager>();
        inputManager = gameObject.AddComponent<InputManager>();
        
        LinePool = Prototypes.CreateLineFactory();
        MainCamera = Prototypes.CreateMainCamera();
        
        stations = new List<Station>();
        airplanes = new List<Airplane>();
        
        shapeGraphicMap = new Dictionary<int, RuntimeAnimatorController>()
        {
            {0, Resources.Load <RuntimeAnimatorController> ("Animations/box_anim")},
            {1, Resources.Load <RuntimeAnimatorController> ("Animations/cross_anim")}
        };
    }

    
    /// <summary>
    /// Creates the objects for a new game
    /// </summary>
    private void SetNewGameState()
    {
        
        SpawnNewStation(0,new Vector3(-3, -3));
        SpawnNewStation(1, new Vector3(3, 3));
        SpawnNewStation(0, new Vector3(3, -1));

        SpawnNewAirplane(1, Vector3.zero);

        airplanes[0].NewDestination(stations[0]);
    }

    
    /// <summary>
    /// Calculate the camera size for a given time
    /// </summary>
    /// <param name="timePassed">Time passed since start of game in seconds</param>
    /// <returns>Orthographic size for a camera</returns>
    private static float CalculateCameraZoom(double timePassed)
    {
        // Logistic Equation
        // https://youtu.be/NU1v-8VRirU
        
        
        return (float)(80 / (16 * Math.Pow(Math.E, -0.0018 * timePassed) + 4));
    }

    /// <summary>
    /// Get an available shape
    /// </summary>
    /// <returns>A shape index</returns>
    public int GetShape()
    {
        return Random.Range(0, availableShapes);
    }

    
    private IEnumerator SlowUpdate()
    {
        while (true)
        {
            MainCamera.orthographicSize = CalculateCameraZoom(Time.time);
            CalculateSpawning(Time.time);
            yield return new WaitForSeconds(1);
        }
    }

    
    /// <summary>
    /// Calculates spawn chances and spawns new objects.
    /// </summary>
    /// <param name="timePassed">Time since start of game</param>
    private void CalculateSpawning(float timePassed)
    {
        timePassed /= 60;
        float pressure = CalculatePlayerPressure();
        
        if (Random.value < 1 / (30 * Math.Abs(timePassed - stations.Count * 3 + 6)) * (1 - pressure))
        {
            SpawnNewStation(GetShape(), PickStationLocation());
        }
        else if (Random.value < 1 / (30 * Math.Abs(timePassed - airplanes.Count * 3)) * pressure && airplanes.Count < stations.Count - 1)
        {
            SpawnNewAirplane(1, Vector3.zero);
        }
    }

    
    /// <summary>
    /// Randomly tries positions until it finds one that's suitable.
    /// Suitable positions are inside the view of the camera,
    /// and at least 2 world units away from the nearest stations.
    /// </summary>
    /// <returns>A position suitable for a new station</returns>
    private Vector2 PickStationLocation()
    {
        int attempts = 100;
        while (0 < attempts)
        {
            bool positionFound = true;
            attempts--;
            
            Vector2 position = Random.insideUnitCircle * MainCamera.orthographicSize;
            foreach (Station station in stations)
            {
                if (Vector2.Distance(position, station.transform.position) > 2f) continue;
                positionFound = false;
                break;
            }
            if (positionFound)
            {
                return position;
            }
        }
        Debug.LogError("Could not find good position for new station!");
        return Vector2.zero;
    }
    
    /// <summary>
    /// Calculate the amount of passengers in the game compared to the total passenger capacity.
    /// This should provide a good measurement of how the player is doing.
    /// </summary>
    /// <returns>Passengers as a percentage of capacity</returns>
    private float CalculatePlayerPressure()
    {
        int totalPassengers = 0;
        int totalCapacity = 0;
        
        foreach (Station station in stations)
        {
            totalPassengers += station.passengers.Count;
            totalCapacity += Station.Capacity;
        }

        foreach (Airplane airplane in airplanes)
        {
            totalPassengers += airplane.Passengers.Count;
            totalCapacity += Airplane.Capacity;
        }

        return totalPassengers / (float)totalCapacity;
    }

    /// <summary>
    /// Perform all the actions needed to spawn a new airplane
    /// </summary>
    /// <param name="shape">Shape index</param>
    /// <param name="position">World position</param>
    private void SpawnNewAirplane(int shape, Vector3 position)
    {
        Airplane airplane = Prototypes.CreateAirplane(position, shapeGraphicMap[shape]);
        
        airplanes.Add(airplane);

        OnAirplaneCreate?.Invoke(airplane);
    }

    
    /// <summary>
    /// Perform all the actions needed to spawn a new station
    /// </summary>
    /// <param name="shape">Shape index</param>
    /// <param name="position">World position</param>
    private void SpawnNewStation(int shape, Vector3 position)
    {
        Station station = Prototypes.CreateStation(shape, position, shapeGraphicMap[shape]);
        
        stations.Add(station);
        
        OnStationCreate?.Invoke(station);
    }


    public void GameOver()
    {
        if (gameOverEnabled)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
