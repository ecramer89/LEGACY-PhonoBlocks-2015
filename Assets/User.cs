using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class User
{
    public bool pinwheelSeen;
    public bool paragliderSeen;
    public bool rockSeen;
    public int pinwheelTime;
    public int paragliderTime;
    public int rockTime;
    public int pinwheelThreshold;
    public int paragliderThreshold;
    public int rockThreshold;
    public int pinwheelLevel;
    public int paragliderLevel;
    public int rockLevel;
    public List<Session> sessions;
    public Session currentSession;

    public User(string userString, List<Session> sessions)
    {
        string[] userValues = userString.Split('_');
        this.pinwheelSeen = System.Convert.ToBoolean(userValues[0]);
        this.paragliderSeen = System.Convert.ToBoolean(userValues[1]);
        this.rockSeen = System.Convert.ToBoolean(userValues[2]);
        this.pinwheelTime = System.Convert.ToInt32(userValues[3]);
        this.paragliderTime = System.Convert.ToInt32(userValues[4]);
        this.rockTime = System.Convert.ToInt32(userValues[5]);
        this.pinwheelThreshold = System.Convert.ToInt32(userValues[6]);
        this.paragliderThreshold = System.Convert.ToInt32(userValues[7]);
        this.rockThreshold = System.Convert.ToInt32(userValues[8]);
        this.pinwheelLevel = System.Convert.ToInt32(userValues[9]);
        this.paragliderLevel = System.Convert.ToInt32(userValues[10]);
        this.rockLevel = System.Convert.ToInt32(userValues[11]);
        this.sessions = sessions;
        currentSession = sessions[sessions.Count - 1];
    }

    public User()
    {
        this.pinwheelSeen = false;
        this.paragliderSeen = false;
        this.rockSeen = false;
        this.pinwheelTime = 4;//4;
        this.paragliderTime = 11; //11;
        this.rockTime = 8; //8;
        this.pinwheelThreshold = 40; //40
        this.paragliderThreshold = 40; //40
        this.rockThreshold = 40; //40
        this.pinwheelLevel = 1;
        this.paragliderLevel = 1;
        this.rockLevel = 1;
        this.sessions = new List<Session>();
    }

    public void AddNewSession()
    {
        sessions.Add(new Session(System.Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds), sessions.Count));
        currentSession = sessions[sessions.Count - 1];
    }

    public string GetLastSessionCreationTime()
    {
        if (sessions.Count > 0)
            return sessions.Last().time.ToString();
        else
            return "0";
    }
}
