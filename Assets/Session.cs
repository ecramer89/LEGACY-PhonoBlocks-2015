using UnityEngine;
using System.Collections.Generic;


public class Session
{
    public long time;
    public int index;
    public float pinwheelActiveTime;
    public float paragliderActiveTime;
    public float rockActiveTime;
    public float otherActiveTime;
    public float headsetActiveTime;
    public List<Token> tokens;

    public int pinwheelTokens;
    public int paragliderTokens;
    public int rockTokens;

    public Session(string sessionString, List<Token> tokens)
    {
        string[] sessionValues = sessionString.Split('_');
        time = System.Convert.ToInt64(sessionValues[0]);
        index = System.Convert.ToInt32(sessionValues[1]);
        pinwheelActiveTime = System.Convert.ToSingle(sessionValues[2]);
        paragliderActiveTime = System.Convert.ToSingle(sessionValues[3]);
        rockActiveTime = System.Convert.ToSingle(sessionValues[4]);
        otherActiveTime = System.Convert.ToSingle(sessionValues[5]);
        headsetActiveTime = System.Convert.ToSingle(sessionValues[6]);
        this.tokens = tokens;
        UpdateTokenCount();
    }

    public Session(string sessionString)
    {
        string[] sessionValues = sessionString.Split('_');
        time = System.Convert.ToInt64(sessionValues[0]);
        index = System.Convert.ToInt32(sessionValues[1]);
        pinwheelActiveTime = System.Convert.ToSingle(sessionValues[2]);
        paragliderActiveTime = System.Convert.ToSingle(sessionValues[3]);
        rockActiveTime = System.Convert.ToSingle(sessionValues[4]);
        otherActiveTime = System.Convert.ToSingle(sessionValues[5]);
        headsetActiveTime = System.Convert.ToSingle(sessionValues[6]);
        this.tokens = new List<Token>();
        UpdateTokenCount();
    }

    public Session(long time, int index)
    {
        this.time = time;
        this.index = index;
        this.pinwheelActiveTime = 0f;
        this.paragliderActiveTime = 0f;
        this.rockActiveTime = 0f;
        this.otherActiveTime = 0f;
        this.headsetActiveTime = 0f;
        this.tokens = new List<Token>();
        UpdateTokenCount();
    }

    public void UpdateTokenCount()
    {
        pinwheelTokens = 0;
        paragliderTokens = 0;
        rockTokens = 0;
        foreach (Token t in tokens)
        {
            if (t.type == "pinwheel")
                pinwheelTokens++;
            else if (t.type == "paraglider")
                paragliderTokens++;
            else if (t.type == "rock")
                rockTokens++;
        }
    }

    public void ReindexTokens()
    {
        foreach (Token t in tokens)
        {
            t.session = index;
        }
    }
}
