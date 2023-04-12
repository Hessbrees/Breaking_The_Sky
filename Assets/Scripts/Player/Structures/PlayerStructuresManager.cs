using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStructuresManager : MonoBehaviour
{
    public List<Vector2> structuresTiles;

    [SerializeField] private GameObject roofTilemap;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Names.playerTag) 
        {
            roofTilemap.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Names.playerTag)
        {
            roofTilemap.SetActive(true);
        }
    }
}
