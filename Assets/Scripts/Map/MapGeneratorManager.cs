using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

[RequireComponent(typeof(MapObjectManager))]
[DisallowMultipleComponent]
public class MapGeneratorManager : MonoBehaviour
{
    [SerializeField] List<Tile> backgroundTileset = new List<Tile>();
    [SerializeField] Tilemap backgroundTilemap;

    private int[] tileMapBorder = new int[] { -15, 15, -16, 14 }; 

    [Inject(Id = "Factors")]
    FactorsManager factorsManager;

    [Inject(Id = "TimeManager")]
    private TimeChangeEvent timeChangeEvent;

    private Tile currentBackgroundTile;
    private bool isCurrentBackgroundGenerateEnded = true;
    private MapObjectManager mapObjectManager;

    //background tile choose randomizer
    Randomizer randomizer;
    private void Awake()
    {
        mapObjectManager = GetComponent<MapObjectManager>();
        randomizer = new Randomizer(tileMapBorder[0], tileMapBorder[1], tileMapBorder[2], tileMapBorder[3]);

        mapObjectManager.FillAvailablePositionList(tileMapBorder);
    }

    private void OnEnable()
    {
        timeChangeEvent.OnTimeChange += UpdateMap_OnTimeChange;

    }
    private void OnDisable()
    {
        timeChangeEvent.OnTimeChange -= UpdateMap_OnTimeChange;
    }
    /// <summary>
    /// Update map background and object every time change
    /// </summary>
    /// <param name="timeChangeEvent"></param>
    /// <param name="timeChangeArg"></param>
    private void UpdateMap_OnTimeChange(TimeChangeEvent timeChangeEvent, TimeChangeArg timeChangeArg)
    {

        UpdateBackground();

        mapObjectManager.UpdateObjectInMap(factorsManager.currentFactors);

    }
    #region Background
    private void UpdateBackground()
    {
        if (isCurrentBackgroundGenerateEnded)
        {
            Tile newTile;

            if (factorsManager.currentFactors.polution > 20)
            {
                newTile = backgroundTileset[1];
            }
            else if (factorsManager.currentFactors.temperature > 40)
            {
                newTile = backgroundTileset[2];
            }
            else
            {
                newTile = backgroundTileset[0];
            }

            if (newTile == currentBackgroundTile) return;

            currentBackgroundTile = newTile;

            randomizer.QueueIsEmpty(false);

            isCurrentBackgroundGenerateEnded = false;
        }        

        for (int i = 0; i < 100; i++)
        {
            if (randomizer.queueIsEmpty)
            {
                isCurrentBackgroundGenerateEnded = true;
                break;
            }

            backgroundTilemap.SetTile(randomizer.GetNextValue(), currentBackgroundTile);
        }

    }

    #endregion Background

}
