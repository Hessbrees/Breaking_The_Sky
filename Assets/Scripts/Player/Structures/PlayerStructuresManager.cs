using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class PlayerStructuresManager : MonoBehaviour
{
    [HideInInspector] private List<Vector2> structuresTiles = new List<Vector2>();

    [SerializeField] private Tilemap groundTilemaps; 
    [SerializeField] private GameObject roofTilemap;

    [Inject(Id = "Player")]
    Player player;

    public List<Vector2> GetPlayerStructuresPositions()
    {
        foreach (Vector2Int tilemap in groundTilemaps.cellBounds.allPositionsWithin)
        {
            structuresTiles.Add(tilemap);
        }

        return structuresTiles;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Names.playerTag) 
        {
            roofTilemap.SetActive(false);
        
            player.playerStatusEffectsControl.DeactivateRadiationSickness();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Names.playerTag)
        {
            roofTilemap.SetActive(true);

            player.playerStatusEffectsControl.ActivateRadiationSickness();
        }
    }
}
