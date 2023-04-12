using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class PlayerStructuresManager : MonoBehaviour
{
    public List<Vector2> structuresTiles;

    [SerializeField] private GameObject roofTilemap;

    [Inject(Id = "Player")]
    Player player;

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
