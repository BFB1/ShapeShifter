using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Prototypes
{

    #region GameplayPrototypes

    public static Station CreateStation(int shape, Vector3 position, RuntimeAnimatorController animationController)
    {
        GameObject newStation = new GameObject {name = "Station", tag = "Station"};

        Station stationComponent = newStation.AddComponent<Station>();
        stationComponent.Shape = shape;

        SpriteRenderer sr = newStation.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Stations";

        Animator animatorComponent = newStation.AddComponent<Animator>(); 

        BoxCollider2D boxCollider2D = newStation.AddComponent<BoxCollider2D>();
        boxCollider2D.size = new Vector2(0.1f, 0.1f);

        newStation.transform.position = position;
        animatorComponent.runtimeAnimatorController = animationController;

        return stationComponent;
    }

    public static Airplane CreateAirplane(Vector3 position, RuntimeAnimatorController animationController)
    {
        GameObject newAirplane = new GameObject {name = "Airplane", tag = "Airplane"};

        Airplane airplaneComponent = newAirplane.AddComponent<Airplane>();
        
        SpriteRenderer sr = newAirplane.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Airplanes";

        Animator animatorComponent = newAirplane.AddComponent<Animator>(); 
        
        BoxCollider2D boxCollider2D = newAirplane.AddComponent<BoxCollider2D>();
        boxCollider2D.size = new Vector2(0.1f, 0.1f);
        
        newAirplane.transform.position = position;
        animatorComponent.runtimeAnimatorController = animationController;

        return airplaneComponent;
    }
    
    #endregion

    #region UtilityPrototypes

    public static Camera CreateMainCamera()
    {
        GameObject newCamera = new GameObject {name= "Main Camera", tag = "MainCamera"};
        newCamera.transform.position = new Vector3(0, 0, -10);

        Camera cameraComponent = newCamera.AddComponent<Camera>();
        cameraComponent.clearFlags = CameraClearFlags.SolidColor;
        cameraComponent.backgroundColor = Color.white;
        cameraComponent.orthographic = true;
        cameraComponent.nearClipPlane = 0.3f;
        cameraComponent.farClipPlane = 1000f;
        cameraComponent.orthographicSize = 4;

        cameraComponent.depth = -1;
        cameraComponent.useOcclusionCulling = false;
        cameraComponent.allowMSAA = false;

        newCamera.AddComponent<AudioListener>();

        return cameraComponent;
    }
    
    public static LineFactory CreateLineFactory()
    {
        GameObject newLinePool = new GameObject {name="LinePool"};
        LineFactory lineFactory = newLinePool.AddComponent<LineFactory>();
        lineFactory.linePrefab = Resources.Load<GameObject>("Prefabs/line");
        return lineFactory;
    }

    #endregion

    #region UICanvasPrototypes

    public static Canvas CreateCanvas()
    {
        // Create a Canvas GameObject with all it's components
        GameObject newCanvas = new GameObject {name = "Canvas"};
        Canvas canvasComponent = newCanvas.AddComponent<Canvas>();
        CanvasScaler canvasScalerComponent = newCanvas.AddComponent<CanvasScaler>();
        GraphicRaycaster graphicRaycasterComponent = newCanvas.AddComponent<GraphicRaycaster>();
        
        // Then create an EventSystem GameObject and it's components
        GameObject newEventSystem = new GameObject {name = "EventSystem"};
        EventSystem eventSystemComponent = newEventSystem.AddComponent<EventSystem>();
        StandaloneInputModule standaloneInputModuleComponent = newEventSystem.AddComponent<StandaloneInputModule>();
        BaseInput baseInputComponent = newEventSystem.AddComponent<BaseInput>();
        
        // Then set up the components
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;

        return canvasComponent;
    }

    public static TextMeshProUGUI CreateText(Transform parent, Vector3 position, string text, Color? color = null)
    {
        GameObject newText = new GameObject {name = text};
        newText.transform.parent = parent;

        TextMeshProUGUI textMeshProUguiComponent = newText.AddComponent<TextMeshProUGUI>();
        textMeshProUguiComponent.text = text;
        textMeshProUguiComponent.color = color ?? Color.black;
        
        newText.GetComponent<RectTransform>().localPosition = position;

        textMeshProUguiComponent.alignment = TextAlignmentOptions.Center;

        return textMeshProUguiComponent;
    }

    public static TextMeshProUGUI CreateText(GameObject parent, Vector3 position, string text)
    {
        return CreateText(parent.transform, position, text);
    }

    #endregion
}
