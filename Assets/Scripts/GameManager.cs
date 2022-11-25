using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float boundHeight = 10;
    public float boundWidth = 18;

    [SerializeField]private GameObject fruitPrefab, fishPrefab;

    [SerializeField] private Transform fruitParent, fishParent;

    public Hunter hunter;

    public List<Transform> fruits = new List<Transform>();

    public List<Fish> fishs = new List<Fish>();

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        StartCoroutine(SpawnManager());
    }

    public Vector3 SetObjectBoundPosition(Vector3 pos)
    {
        float z = boundHeight / 2;
        float x = boundWidth / 2;
        if (pos.z > z) pos.z = -z;
        if (pos.z < -z) pos.z = z;
        if (pos.x < -x) pos.x = x;
        if (pos.x > x) pos.x = -x;

        return pos;
    }
    public void DestroyFish(Vector3 pos) {
        foreach (Fish item in fishs)
        {
            if (item.transform.position == pos)
            {
                fishs.Remove(item);
                Destroy(item.gameObject);
                return;
            }
        }
    }
    public void DestroyFruits(Vector3 pos)
    {
        foreach (Transform fruit in fruits)
        {
            if (fruit.position == pos)
            {
                fruits.Remove(fruit);
                Destroy(fruit.gameObject);
                break;
            }
        }
        foreach (Fish fish in fishs)
        {
            fish.currentFruit = null;
        }
    }

    IEnumerator SpawnManager()
    {
        float z = boundHeight / 2;
        float x = boundWidth / 2;

        while (true)
        {
            var fruit = Instantiate(fruitPrefab,
            Random.Range(-x, x) * Vector3.right + Random.Range(-z, z) * Vector3.forward,
                Quaternion.Euler(Vector3.zero), fruitParent);

            fruits.Add(fruit.transform);

            yield return new WaitForSeconds(1.5f);
        }        
    }

    private void OnDrawGizmos()
    {
        float x = boundWidth / 2;
        float z = boundHeight / 2;
        Vector3 topLeft = new Vector3(-x, 0, z);
        Vector3 topRight = new Vector3(x, 0, z);
        Vector3 botRight = new Vector3(x, 0, -z);
        Vector3 botLeft = new Vector3(-x, 0, -z);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botRight, botLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }

}
