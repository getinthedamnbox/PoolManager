using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : Manager<HUDManager>
{
    [Header("Frames Per Second (FPS)")]
    [SerializeField] private int framesForAverage = 60;
    private LinkedList<float> fpsList = new LinkedList<float>();
    private float fpsSum;
    [SerializeField] private Text fpsValue;

    [Header("Seconds Per Spawn (SPS)")]
    [SerializeField] private int spawnsForAverage = 10;
    private LinkedList<float> spsList = new LinkedList<float>();
    private float spsSum;
    [SerializeField] private Text spsValue;

    private void Update()
    {
        while (fpsList.Count >= framesForAverage)
        {
            float prevFps = fpsList.Last.Value;
            fpsList.RemoveLast();
            fpsSum -= prevFps;
        }

        float fps = 1 / Time.deltaTime;
        fpsList.AddFirst(fps);
        fpsSum += fps;

        float average = fpsSum / fpsList.Count;

        fpsValue.text = average.ToString("F0");
    }

    public void AddSPS(float sps)
    {
        while (spsList.Count >= spawnsForAverage)
        {
            float prevSps = spsList.Last.Value;
            spsList.RemoveLast();
            spsSum -= prevSps;
        }

        spsList.AddFirst(sps);
        spsSum += sps;

        float average = spsSum / spsList.Count;

        spsValue.text = average.ToString("F4");
    }
}
