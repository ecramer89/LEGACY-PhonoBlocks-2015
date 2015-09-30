using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class DataManager : MonoBehaviour
{   
    static Dictionary<string, User> users = new Dictionary<string, User>();
    public static Dictionary<string, Texture2D> userImages = new Dictionary<string, Texture2D>();
    public static string currentUserID = "";
    static string today;
    public static bool tempTeacherFlag = false;

    public static string imageDirectoryPath;

    static int imageWidth = 200;
    static int imageHeight = 200;

    public static Texture2D currentUserImage
    {
        get
        {
            return userImages[currentUserID];
        }
    }

    // Use this for initialization
    void Start()
    {
        today = DateTime.Now.ToString("ddMMyy");
        //PlayerPrefs.DeleteAll();

        // Handle images
        imageDirectoryPath = Application.persistentDataPath + "/Images/";
        Directory.CreateDirectory(imageDirectoryPath);

		//I think here, what Perry is doing is caching all of the images that are already saved on the disk
		//into this game object so that they can be displayed in the ui.
        foreach (string filepath in Directory.GetFiles(DataManager.imageDirectoryPath))
        {
            string filename = Path.GetFileNameWithoutExtension(filepath);
            byte[] rawImage = File.ReadAllBytes(filepath);
            Texture2D tex2d = new Texture2D(1, 1);
            tex2d.LoadImage(rawImage);
            TextureScale.Bilinear(tex2d, imageWidth, imageHeight);
            userImages[filename] = tex2d;
        }
		//sets the value of the preference identified by key.
        PlayerPrefs.SetInt("ChartTime", 10 * 60);

        // likewise he then caches all of the saved user data (the users tokens and the users sessions)
		//into these list and dictionary objects.
        foreach (String userID in userImages.Keys)
        {   //there is a list of 'session' associated with each user string
            List<Session> sessions = new List<Session>();
			//and for each user there is a dictionary which stores a collection of "tokens" for each of an int.
            Dictionary<int, List<Token>> tokens = new Dictionary<int, List<Token>>();

			//if this key exists in the preferences (userID_tokens)
            if (PlayerPrefs.HasKey(userID + "_tokens"))
            { //then... we get the string value that is assocuated with this key in the preferences file
				//"returns the value corresonding to this key in the preferences file"
                string[] tokensString = PlayerPrefs.GetString(userID + "_tokens").Split(';');
                foreach (string token in tokensString)
					//tokenstring[] containts an array of tokens (the string associated with tokens is split by ; to mark out distinct toekn objets
					//and within these, an _ marks out distinct values (3) for each token
                {
                    string[] tokenValues = token.Split('_');
                    Token t = new Token(tokenValues[0], tokenValues[1], tokenValues[2]);
					//"if we don't have a token for the key t.session [an int] in the dicrtionary associated with this user
                    if (!tokens.ContainsKey(t.session))
                        tokens[t.session] = new List<Token>(); //...then create one
                    tokens[t.session].Add(t); //AND then add this token to the list of tokens associated with this token's session
                }
            }

            if (PlayerPrefs.HasKey(userID + "_sessions"))
            {
                string[] sessionsString = PlayerPrefs.GetString(userID + "_sessions").Split(';');
                int i = 0;
                foreach (string sessionString in sessionsString)
                {
                    Session s;
                    if (tokens.ContainsKey(i))
                        s = new Session(sessionString, tokens[i]);
                    else
                        s = new Session(sessionString);
                    sessions.Add(s);
                    i++;
                }
            }

            if (PlayerPrefs.HasKey(userID))
                users[userID] = new User(PlayerPrefs.GetString(userID), sessions);
            else
                users[userID] = new User();
        }
        //currentUserID = "123";
        //users[currentUserID] = new User();
        //NewSession();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {

        }
    }

    static List<Session> CreateUserSessions(string sessionString, string tokenString)
    {
        List<Session> sessions = new List<Session>();
        Dictionary<int, List<Token>> tokens = new Dictionary<int, List<Token>>();

        string[] tokensString = tokenString.Split(';');
        foreach (string token in tokensString)
        {
            string[] tokenValues = token.Split('_');
            Token t = new Token(tokenValues[0], tokenValues[1], tokenValues[2]);
            if (!tokens.ContainsKey(t.session))
                tokens[t.session] = new List<Token>();
            tokens[t.session].Add(t);
        }


        string[] sessionsString = sessionString.Split(';');
        int i = 0;
        foreach (string session in sessionsString)
        {
            Session s;
            if (tokens.ContainsKey(i))
                s = new Session(session, tokens[i]);
            else
                s = new Session(session);
            sessions.Add(s);
            i++;
        }
        return sessions;
    }

    public static string ExportCurrentUser()
    {
        if(users[currentUserID].sessions.Count > 0)
            return PlayerPrefs.GetString(currentUserID + "_sessions")+"|"+PlayerPrefs.GetString(currentUserID+"_tokens");
        return "";
    }
    
	//saves the data of every user
    public static void Save()
    {
        foreach (KeyValuePair<string, User> user in users)
        {
			//re-saves the data for each user in the player prefs (... presumably, this would be called when we "save" button is hit, and I suppose
			//that possibly multiple users could have played in the course of one game.
			//(...he has to load all the images and all of the user data for quick loading in start, because the kids tap on their picture to log in)
			//to retrieve all the data that is saved with that user
            User u = user.Value;
            string userString = "";
            string userKey = user.Key;
            string sessionKey = user.Key + "_sessions";
            string sessionString = "";
            string tokenKey = user.Key + "_tokens";
            string tokenString = "";

            foreach (Session session in user.Value.sessions)
            {
                foreach (Token token in session.tokens)
                {
                    if (tokenString != "")
                        tokenString += ";";
                    tokenString += (token.session + "_" + token.difficulty + "_" + token.type);
                }
                if (sessionString != "")
                    sessionString += ";";
                sessionString += (session.time + "_" + session.index + "_" +
                    session.pinwheelActiveTime + "_" + session.paragliderActiveTime + "_" + session.rockActiveTime + "_" + session.otherActiveTime + "_" + session.headsetActiveTime);
            }
            if (userString != "")
                userString += ";";
            userString += u.pinwheelSeen + "_" + u.paragliderSeen + "_" + u.rockSeen + "_" +
                u.pinwheelTime + "_" + u.paragliderTime + "_" + u.rockTime + "_" +
                u.pinwheelThreshold + "_" + u.paragliderThreshold + "_" + u.rockThreshold + "_" +
                u.pinwheelLevel + "_" + u.paragliderLevel + "_" + u.rockLevel;
            if (user.Key == currentUserID)
            {
                PlayerPrefs.SetString(userKey, userString);
                PlayerPrefs.SetString(sessionKey, sessionString);
                PlayerPrefs.SetString(tokenKey, tokenString);
                PlayerPrefs.Save();
            }
        }
    }

    public static void NewSession()
    {
        users[currentUserID].AddNewSession();
    }

    public static string GetMostRecentPlay(string user)
    {       
        return users[user].GetLastSessionCreationTime();
    }
    
    public static void AddUserImage(Texture2D tex2d, string userID)
    {
        currentUserID = userID;
        userImages[userID] = tex2d;
        users[userID] = new User();
    }

    public static void ReplaceUserImage(Texture2D tex2d)
    {
        Texture2D tex2dCopy = Instantiate(tex2d) as Texture2D;
        TextureScale.Bilinear(tex2dCopy, imageWidth, imageHeight);
        userImages[currentUserID] = tex2dCopy;
    }

    public static void RemoveCurrentUser()
    {
        userImages.Remove(currentUserID);
        File.Delete(Path.Combine(DataManager.imageDirectoryPath, currentUserID + ".png"));
        UnityEngine.Object.DestroyImmediate(GameObject.Find(currentUserID));
        GameObject.Find("Controllers").GetComponent<UserGridController>().RefreshGrid();
    }
    
}
