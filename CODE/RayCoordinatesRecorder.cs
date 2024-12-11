using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;
using TMPro;

public class RayCoordinatesRecorder : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public Transform planeTransform;
    public TextMeshPro coordinateText;
    public bool debugMode = true; // For debugging

    private string filePath = @"C:/Users/cadur/Videos/Unity Pruebas/Data/coordinates.txt";
    private bool isRecording = false;
    private float timeSinceLastSave = 0f; // Time since last coordinates save
    private float saveInterval = 5f; // Save every 5 seconds

    void Start()
    {
        if (rayInteractor == null || planeTransform == null)
        {
            Debug.LogError("Components not assigned!");
            enabled = false;
            return;
        }

        // Create directory if it doesn't exist
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            if (debugMode) Debug.Log($"Directory created: {directory}");
        }

        // Write header if the file doesn't exist
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Coordinates (X, Y)\n");
            if (debugMode) Debug.Log($"File created: {filePath}");
        }
    }

    void Update()
    {
        timeSinceLastSave += Time.deltaTime; // Update the timer

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.transform == planeTransform)
            {
                Vector3 localPos = planeTransform.InverseTransformPoint(hit.point);
                float x = localPos.x / (planeTransform.localScale.x * -5f);
                float y = localPos.z / (planeTransform.localScale.z * -5f);

                // Update text if it exists
                if (coordinateText != null)
                {
                    coordinateText.text = $"X: {x:F2}, Y: {y:F2}";
                }

                // Save coordinates every 5 seconds
                if (timeSinceLastSave >= saveInterval)
                {
                    SaveCoordinates(x, y);
                    timeSinceLastSave = 0f; // Reset timer
                }
            }
        }
    }

    // Method to save coordinates to the file
    private void SaveCoordinates(float x, float y)
    {
        string coordText = $"X: {x:F2}, Y: {y:F2}\n";
        try
        {
            File.AppendAllText(filePath, coordText);
            if (debugMode) Debug.Log($"Saved: {coordText.Trim()} to {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving: {e.Message}");
        }
    }

    // Show file path in GUI for debugging
    void OnGUI()
    {
        if (debugMode)
        {
            GUI.Label(new Rect(10, 10, 1000, 20), $"File path: {filePath}");
        }
    }
}