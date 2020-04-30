using System.Collections;
using System.Collections.Generic;
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

    private List<Collider2D> GetObjectsUnderMouse()
    {
        List<Collider2D> results = new List<Collider2D>();
        Physics2D.OverlapCircle(mousePosition, 1, new ContactFilter2D(), results);
        return results;
    }
    
    private Airplane GetAirplaneUnderMouse()
    {
        List<Collider2D> hits = GetObjectsUnderMouse();
        foreach (Collider2D hit in hits)
        {
            if (!Equals(hit, null) && hit.gameObject.CompareTag("Airplane"))
            {
                return hit.gameObject.GetComponent<Airplane>();
            }
        }

        return null;
    }

    private Station GetStationUnderMouse()
    {
        List<Collider2D> hits = GetObjectsUnderMouse();
        foreach (Collider2D hit in hits)
        {
            if (!Equals(hit, null) && hit.gameObject.CompareTag("Station"))
            {
                return hit.gameObject.GetComponent<Station>();
            }
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
