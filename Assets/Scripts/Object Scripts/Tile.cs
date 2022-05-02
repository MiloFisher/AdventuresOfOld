using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class Tile : MonoBehaviour
{
    public Vector3Int position;
    public bool activated;
    public List<Tile> neighbors;
    public int cost;
    public Tile parent;
    public int g;
    public int h;
    [SerializeField] private GameObject treasureToken;

    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        Deactivate();
        DisableTreasureToken();
        if (position.x + position.y + position.z != 0)
            Debug.LogError(gameObject.name + " has an invalid position!");
    }

    public void Activate(int range)
    {
        activated = true;
        GetComponent<Image>().enabled = true;
        GetComponent<Button>().interactable = true;
        cost = range;

        if(range > 0)
        {
            foreach (Tile t in neighbors)
            {
                if(range - 1 > t.cost)
                    t.Activate(range - 1);
            }
        }
    }

    public void Deactivate()
    {
        activated = false;
        GetComponent<Image>().enabled = false;
        GetComponent<Button>().interactable = false;
        cost = -1;
    }

    public void EnableTreasureToken()
    {
        treasureToken.SetActive(true);
    }

    public void DisableTreasureToken()
    {
        treasureToken.SetActive(false);
    }

    public bool TreasureTokenIsEnabled()
    {
        return treasureToken.activeInHierarchy;
    }

    public void MovePlayer()
    {
        PlayManager.Instance.MoveToTile(position);
    }

    public int DistanceToTile(Vector3Int pos)
    {
        List<Tile> openPathTiles = new List<Tile>();
        List<Tile> closedPathTiles = new List<Tile>();

        // Prepare the start tile.
        Tile startPoint = this;
        Tile endPoint = PlayManager.Instance.gameboard[pos];
        Tile currentTile = startPoint;

        currentTile.parent = null;
        currentTile.g = 0;
        currentTile.h = GetH(startPoint.position, endPoint.position);

        // Add the start tile to the open list.
        openPathTiles.Add(currentTile);

        while (openPathTiles.Count != 0)
        {
            // Sorting the open list to get the tile with the lowest F.
            openPathTiles.Sort((a, b) => (a.g + a.h) - (b.g + b.h));
            currentTile = openPathTiles[0];

            // Removing the current tile from the open list and adding it to the closed list.
            openPathTiles.Remove(currentTile);
            closedPathTiles.Add(currentTile);

            int g1 = currentTile.g + 1;

            // If there is a target tile in the closed list, we have found a path.
            if (closedPathTiles.Contains(endPoint))
            {
                break;
            }

            foreach(Tile t in currentTile.neighbors)
            {
                // Check if tile is valid
                if (!closedPathTiles.Contains(t))
                {
                    if (!openPathTiles.Contains(t))
                    {
                        t.g = g1;
                        t.h = GetH(t.position, endPoint.position);
                        t.parent = currentTile;
                        openPathTiles.Add(t);
                    }
                    // Otherwise check if using current G we can get a lower value of F, if so update it's value.
                    else if (t.g + t.h > g1 + t.h)
                    {
                        t.g = g1;
                        t.parent = currentTile;
                    }
                }
            }
        }

        // Backtracking - get final distance
        int counter = -1;
        if (closedPathTiles.Contains(endPoint))
        {
            Tile current = endPoint;
            while (current != null && counter < 500)
            {
                current = current.parent;
                counter++;
            }
        }

        return counter;
    }

    private int GetH(Vector3Int t1, Vector3Int t2)
    {
        return Mathf.Max(Mathf.Abs(t1.x - t2.x), Mathf.Abs(t1.y - t2.y), Mathf.Abs(t1.z - t2.z));
    }
}
