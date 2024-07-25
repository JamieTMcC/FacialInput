using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    /// <summary>
    /// Gives a unique directory for saving files so that the data is not overwritten, uses a time stamp
    /// </summary>

    private string path = "";
    

    public void GenerateNewIteration()
    {
        path = Application.persistentDataPath + "/experimentdata/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path += "ParticipantTimeStamp_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "/";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        Debug.Log("Path Generated At: " + path);
    }

    public string GetPath()
    {
        return path;
    }

}
