using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutOutObject : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private LayerMask wallMask;
    private Camera mainCamera;
    private List<GameObject> objs = new List<GameObject>();

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        
        for (int i = 0; i < objs.Count; i++)
        {
            Material[] materials = objs[i].transform.GetComponent<Renderer>().materials;
            for (int m = 0; m < materials.Length; m++)
            {
                materials[m].SetFloat("_Cutout_Size", 0f);
            }
        }
        objs.Clear();
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 5f, offset, offset.magnitude, wallMask);
        foreach (RaycastHit hit in hits)
        {
            objs.Add(hit.transform.gameObject);
            Material[] materials = hit.transform.GetComponent<Renderer>().materials;
            
            for (int m = 0; m < materials.Length; m++)
            {
                materials[m].SetFloat("_Cutout_Size", 0.2f);
                materials[m].SetFloat("_Falloff_Size", 0.05f);
            }
        }

        /*RaycastHit[] hitObjects = Physics.SphereCastAll(transform.position, 5f, offset, offset.magnitude, wallMask);
        for (int i  = 0; i < hitObjects.Length; i++)
        {
            Material[] materials =  hitObjects[i].transform.GetComponent<Renderer>().materials;
            Debug.Log(hitObjects[i].transform);

            for(int m = 0; m < materials.Length; m++)
            {
                //materials[m].SetVector("_Cutout_Position", cutoutPos);
                materials[m].SetFloat("_Cutout_Size", 0.2f);
                materials[m].SetFloat("_Falloff_Size", 0.05f);
            }

        }*/
    }
}
