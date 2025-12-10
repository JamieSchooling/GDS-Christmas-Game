using System.Linq;
using UnityEngine;

public class Tasks : MonoBehaviour
{
    [SerializeField] Item[] Items;
    private float nextActionTime = 0.0f;
    public float frequency = 5f;

    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += frequency;
            GenItem();
        }
    }

    private void GenItem()
    {

        int Generated = Random.Range(0, Items.Count());
        int comNum = Items[Generated].Components.Count();
        Debug.Log(Items[Generated].name);
        foreach (int index in Enumerable.Range(0, comNum))
        {
            Debug.Log(Items[Generated].Components[index].name);
        }
        

    }
}
