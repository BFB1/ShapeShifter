using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    private Dictionary<int, RuntimeAnimatorController> shapeGraphicMap;
    
    private List<Station> stations;
    private List<Airplane> airplanes;

    private InputManager inputManager;
    private UiManager uiManager;
    
    
    
    public static GameMaster Instance;
    
    public LineFactory LinePool{ get; private set; }

    public Camera MainCamera { get; private set; }

    public event Action<Airplane> OnAirplaneCreate;
    public event Action<Airplane> OnAirplaneDelete;

    public event Action<Station> OnStationCreate;

    public ReadOnlyCollection<Airplane> Airplanes => new ReadOnlyCollection<Airplane>(airplanes);
    public ReadOnlyCollection<Station> Stations => new ReadOnlyCollection<Station>(stations);

    public int AvailableShapes { get; private set; } = 1 + 1;


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

    
    private IEnumerator SlowUpdate()
    {
        while (true)
        {
            MainCamera.orthographicSize = CalculateCameraZoom(Time.time);
            yield return new WaitForSeconds(1);
        }
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
