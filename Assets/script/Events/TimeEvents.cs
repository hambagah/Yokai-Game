using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeEvents
{
    public event Action<int> onChangeTime;
    public void ChangeTime(int time)
    {
        if (onChangeTime != null)
        {
            onChangeTime(time);
        }
    }

    public event Action<int> onChangeProgress;
    public void ChangeProgress(int progression)
    {
        if (onChangeProgress != null)
        {
            onChangeProgress(progression);
        }
    }

    public event Action onReturnTime;
    public void ReturnTime()
    {
        if (onReturnTime != null)
        {
            onReturnTime();
        }
    }

    /*public event Action onSleepingEvent;
    public void SleepingEvent()
    {
        if (onSleepingEvent != null)
        {
            onSleepingEvent();
        }
    }*/
}
