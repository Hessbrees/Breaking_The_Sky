using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class MapGeneratorManager : MonoBehaviour
{
    [SerializeField] List<Tile> backgroundTileset = new List<Tile>();
    [SerializeField] Tilemap backgroundTilemap;

    [Inject(Id = "Factors")]
    FactorsManager factorsManager;

    [Inject(Id = "TimeManager")]
    private TimeChangeEvent timeChangeEvent;

    private Tile currentBackgroundTile;
    private bool isCurrentBackgroundGenerateEnded = true;

    //background tile choose randomizer
    Randomizer randomizer;
    private void Awake()
    {
        randomizer = new Randomizer(-15, 15, -16, 14);

    }

    private void OnEnable()
    {
        timeChangeEvent.OnTimeChange += UpdateMap_OnTimeChange;

    }
    private void OnDisable()
    {
        timeChangeEvent.OnTimeChange -= UpdateMap_OnTimeChange;
    }

    private void UpdateMap_OnTimeChange(TimeChangeEvent timeChangeEvent, TimeChangeArg timeChangeArg)
    {

        UpdateBackground();
    }

    private void UpdateBackground()
    {
        if (isCurrentBackgroundGenerateEnded)
        {
            Tile newTile;

            if (factorsManager.currentPolution > 20)
            {
                newTile = backgroundTileset[1];
            }
            else if (factorsManager.currentTemperature > 40)
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
}
