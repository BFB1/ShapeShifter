using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawLine : MonoBehaviour {
    
    [SerializeField]
    private GameObject LineGeneratorPrefab;
    
    private void Start(){
        SpawnLineGenerator();
    }

    private void SpawnLineGenerator(){
        GameObject newLineGen = Instantiate(LineGeneratorPrefab);
        LineRenderer lineRenderer = newLineGen.GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, new Vector3(0, -2, 0));
        lineRenderer.SetPosition(1, new Vector3(0, 2, 0));

        //Destroy(newLineGen, 5);
    }
}
