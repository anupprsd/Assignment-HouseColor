using System;
using UnityEditor;
using UnityEngine;

// Handles selecting walls by raycasting and passing them to the ColourPicker
public class WallColorController : MonoBehaviour
{
    [SerializeField] private ColourPicker picker; 
    [SerializeField] private LayerMask wallLayer;   // Layer mask to filter raycast hits to walls only

    // Called from CameraController when user taps/clicks on screen
    public void SelectWall(Vector2 touchPosition)
    {
        Camera cam = Camera.main;
        if (cam == null) return; // No camera found

        // Cast a ray from screen position into the 3D world
        Ray ray = cam.ScreenPointToRay(touchPosition);

        RaycastHit hit;
        // Check if ray hits a wall within 20 units, restricted by wallLayer
        if (Physics.Raycast(ray, out hit, 20, wallLayer))
        {
            GameObject currentSelectedWall = hit.collider.gameObject;

            // Pass the selected wall to the ColourPicker
            picker.ChangeColorOfObject(currentSelectedWall);
        }
    }
}
