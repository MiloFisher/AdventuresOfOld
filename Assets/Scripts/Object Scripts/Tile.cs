using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class Tile : MonoBehaviour
{
    public Vector3Int position;
    public bool activated;
    private List<Tile> neighbors;

    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        Deactivate();
        FindNeighbors();
        if (position.x + position.y + position.z != 0)
            Debug.LogError(gameObject.name + " has an invalid position!");
    }

    public void Activate(int range)
    {
        activated = true;
        GetComponent<Image>().enabled = true;
        GetComponent<Button>().interactable = true;

        if(range - 1 > 0)
        {
            foreach (Tile t in neighbors)
            {
                if (!t.activated)
                {
                    t.Activate(range - 1);
                }
            }
        }
    }

    public void Deactivate()
    {
        activated = false;
        GetComponent<Image>().enabled = false;
        GetComponent<Button>().interactable = false;
    }

    private void FindNeighbors()
    {
        neighbors = new List<Tile>();
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject g in tiles)
        {
            Tile t = g.GetComponent<Tile>();
            if(t.position != position && ((t.position.x == position.x && Mathf.Abs(t.position.y - position.y) == 1 && Mathf.Abs(t.position.z - position.z) == 1) || (Mathf.Abs(t.position.x - position.x) == 1 && t.position.y == position.y && Mathf.Abs(t.position.z - position.z) == 1) || (Mathf.Abs(t.position.x - position.x) == 1 && Mathf.Abs(t.position.y - position.y) == 1 && t.position.z == position.z)))
            {
                neighbors.Add(t);
            }
        }
    }
}
