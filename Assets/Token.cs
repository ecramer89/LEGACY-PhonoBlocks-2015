using UnityEngine;
using System.Collections;

public class Token 
{
    public int session;
    public int difficulty;
    public string type;

    public Token(string session, string difficulty, string type)
    {
        this.session = System.Convert.ToInt32(session);
        this.difficulty = System.Convert.ToInt32(difficulty);
        this.type = type;
    }
}
