using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Vector3 mousePosition;
    private bool leftMouseUpThisFrame;

    private void Update()
    {
        leftMouseUpThisFrame = Input.GetKeyUp(KeyCode.Mouse0);
        mousePosition = GameMaster.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Airplane subject = GetAirplaneUnderMouse();
            if (!Equals(subject, null))
            { 
                StartCoroutine(HandleDrag(subject));
            }
        }
    }

    private Collider2D GetObjectUnderMouse()
    {
        Collider2D hit = Physics2D.OverlapArea(mousePosition - Vector3.one, mousePosition + Vector3.one);
        return hit;
    }
    
    private Airplane GetAirplaneUnderMouse()
    {
        Collider2D hit = GetObjectUnderMouse();
        if (!Equals(hit, null) && hit.gameObject.CompareTag("Airplane"))
        {
            return hit.gameObject.GetComponent<Airplane>();
        }

        return null;
    }

    private Station GetStationUnderMouse()
    {
        Collider2D hit = GetObjectUnderMouse();
        if (!Equals(hit, null) && hit.gameObject.CompareTag("Station"))
        {
            return hit.gameObject.GetComponent<Station>();
        }

        return null;
    }

    private IEnumerator HandleDrag(Airplane airplane)
    {
        Line drawnLine = GameMaster.Instance.LinePool.GetLine(airplane.transform.position, mousePosition, 0.03f, Color.black);
        while (true)
        {
            drawnLine.end = mousePosition;
            drawnLine.start = airplane.transform.position;
            if (leftMouseUpThisFrame) {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        drawnLine.gameObject.SetActive(false);

        Station station = GetStationUnderMouse();
        if (!Equals(station, null))
        {
            airplane.NewDestination(station);
        }
    }
}
