using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class SaveLoad : MonoBehaviour
{
    string fileDirectory => $"{Application.persistentDataPath}/SavedGrids/";

    private void Start()
    {
        DrawGrid drawGrid = GetComponent<DrawGrid>();

        try
        {
            string json = "";
            var directory = new DirectoryInfo(fileDirectory);
            string fileName = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First().Name;

            json = File.ReadAllText($"{fileDirectory}{fileName}");
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            drawGrid.LoadColors(saveData);
            Debug.Log($"Loaded file: {fileName}");
        }
        catch (Exception e)
        {
            drawGrid.SetColors();
            Debug.LogAssertion("File not found or couldn't be loaded: Loading fresh grid");
        }
    }

    public void Save()
    {
        DrawGrid drawGrid = GetComponent<DrawGrid>();
        SaveData saveData = new SaveData();

        saveData.dimensions = drawGrid.GetDimensions();
        saveData.colorsList = new List<Vector3>();
        Color[,] colors = drawGrid.GetColors();

        foreach (var color in colors)
            saveData.colorsList.Add((Vector4)color);

        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            new DirectoryInfo(fileDirectory).Create();

            string fileName = $"[{saveData.dimensions.x} x {saveData.dimensions.y}].json";
            File.WriteAllText($"{fileDirectory}{fileName}", json);
            Debug.Log($"Saved file: {fileName}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving file");
        }
    }
}

[Serializable]
public class SaveData
{
    public Vector2Int dimensions;
    public List<Vector3> colorsList;
}
