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
}
