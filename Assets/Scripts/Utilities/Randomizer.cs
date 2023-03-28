using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.BoundsInt;

public class Randomizer
{
    private Queue<Vector3Int> currentQueue;
    private int currentQueueIndex = 0;
    private Queue<Vector3Int>[] randomQueueArray;

    int minValueX;
    int minValueY;
    int maxValueX;
    int maxValueY;

    public bool queueIsEmpty;
    public Randomizer(int minValueX, int maxValueX, int minValueY, int maxValueY)
    {
        this.minValueX = minValueX;
        this.minValueY = minValueY;
        this.maxValueX = maxValueX;
        this.maxValueY = maxValueY;

        currentQueue = new Queue<Vector3Int>();

        GetRandomValuesFromNestedLoop();

        currentQueue = randomQueueArray[currentQueueIndex];
    }
    public Vector3Int GetNextValue()
    {
        if (currentQueue.Count == 0)
        {
            while (randomQueueArray[currentQueueIndex].Count == 0)
            {
                currentQueueIndex++;

                if (currentQueueIndex >= randomQueueArray.Length)
                {
                    queueIsEmpty = true;
                    GetRandomValuesFromNestedLoop();
                    currentQueueIndex = 0;

                    return new Vector3Int(minValueX, minValueY);

                }
                    
            }

            if (currentQueueIndex < randomQueueArray.Length && randomQueueArray[currentQueueIndex].Count != 0)
            {
                currentQueue = randomQueueArray[currentQueueIndex];
            }
        }

        return currentQueue.Dequeue();

    }
    public void QueueIsEmpty(bool isFalse)
    {
        queueIsEmpty = isFalse;
    }
    public void GetRandomValuesFromNestedLoop()
    {
        Queue<Vector3Int>[] queue = new Queue<Vector3Int>[10];

        for(int i  = 0; i < queue.Length; i++)
            queue[i] = new Queue<Vector3Int>();

        for (int i = minValueX; i < maxValueX; i++)
        {
            for (int j = minValueY; j < maxValueY; j++)
            {
                int randomIndex = Random.Range(0, 10);

                queue[randomIndex].Enqueue(new Vector3Int(i, j));

            }
        }
        randomQueueArray = queue;
    }
}
