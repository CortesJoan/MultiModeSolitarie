using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class LayoutController : MonoBehaviour
{
    // The space between rows when in portrait mode
    public float verticalSpaceBetweenRows;

    // The space between rows when in landscape mode
    public float horizontalSpaceBetweenRows;

    // The current orientation of the screen
    private ScreenOrientation currentOrientation;

    // The original positions of the slots in the layout
    private List<Vector3> originalPositions;
    public List<GameObject> layoutInstantiatedSlots;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Initialize current orientation to landscape left
        currentOrientation = ScreenOrientation.LandscapeLeft;

        // Initialize original positions to an empty list
        originalPositions = new List<Vector3>();

        // Loop through layout instantiated slots and add their positions to original positions
        foreach (GameObject slot in layoutInstantiatedSlots)
        {
            originalPositions.Add(slot.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        // Check if screen width is greater than screen height
        if (Screen.width > Screen.height)
        {
            // Screen is in landscape mode
            // Check if current orientation is not landscape left or right
            if (currentOrientation != ScreenOrientation.LandscapeLeft && currentOrientation != ScreenOrientation.LandscapeRight)
            {
                // Update current orientation to match screen orientation
                currentOrientation = Screen.orientation;

                // Adjust layout according to new orientation
                AdjustLayout(currentOrientation);
            }
        }
        else
        {
            // Screen is in portrait mode
            // Check if current orientation is not portrait
            if (currentOrientation != ScreenOrientation.Portrait)
            {
                // Update current orientation to portrait
                currentOrientation = ScreenOrientation.Portrait;

                // Adjust layout for portrait mode
                AdjustLayoutForPortrait();
            }
        }
    }

    // Adjusts layout according to screen orientation
    void AdjustLayout(ScreenOrientation orientation)
    {
        // Check if orientation is portrait, landscape left, or landscape right
        if (orientation == ScreenOrientation.Portrait)
        {
            // Adjust layout for portrait mode
            AdjustLayoutForPortrait();
        }
        else if (orientation == ScreenOrientation.LandscapeLeft)
        {
            // Adjust layout for landscape left mode
            AdjustLayoutForLandscapeLeft();
        }
        else if (orientation == ScreenOrientation.LandscapeRight)
        {
            // Adjust layout for landscape right mode
            AdjustLayoutForLandscapeRight();
        }
    }

    // Adjusts layout for portrait mode
    void AdjustLayoutForPortrait()
    {
        // Calculate the horizontal space between slots based on the number of slots and the screen width
        float horizontalSpaceBetweenSlots = Screen.width / (layoutInstantiatedSlots.Count + 1);

        // Loop through layout instantiated slots and set their positions for portrait mode
        for (int i = 0; i < layoutInstantiatedSlots.Count; i++)
        {
            GameObject slot = layoutInstantiatedSlots[i];
            Vector3 originalPosition = originalPositions[i];

            // Set slot position to a new vector with these values:
            // x: Horizontal space between slots multiplied by (index + 1)
            // y: Original y position minus (index multiplied by vertical space between rows)
            // z: Original z position
            slot.transform.position = new Vector3(
                horizontalSpaceBetweenSlots * (i + 1),
                originalPosition.y - (i * verticalSpaceBetweenRows),
                originalPosition.z
            );
        }
    }

    // Adjusts layout for landscape left mode
    void AdjustLayoutForLandscapeLeft()
    {
        // Calculate the vertical space between slots based on the number of slots and the screen height
        float verticalSpaceBetweenSlots = Screen.height / (layoutInstantiatedSlots.Count + 1);

        // Loop through layout instantiated slots and set their positions for landscape left mode
        for (int i = 0; i < layoutInstantiatedSlots.Count; i++)
        {
            GameObject slot = layoutInstantiatedSlots[i];
            Vector3 originalPosition = originalPositions[i];

            // Set slot position to a new vector with these values:
            // x: Original x position plus (index multiplied by horizontal space between rows)
            // y: Vertical space between slots multiplied by (index + 1)
            // z: Original z position
            slot.transform.position = new Vector3(
                originalPosition.x + (i * horizontalSpaceBetweenRows),
                verticalSpaceBetweenSlots * (i + 1),
                originalPosition.z
            );
        }
    }

    // Adjusts layout for landscape right mode
    void AdjustLayoutForLandscapeRight()
    {
        // Calculate the vertical space between slots based on the number of slots and the screen height
        float verticalSpaceBetweenSlots = Screen.height / (layoutInstantiatedSlots.Count + 1);

        // Loop through layout instantiated slots and set their positions for landscape right mode
        for (int i = 0; i < layoutInstantiatedSlots.Count; i++)
        {
            GameObject slot = layoutInstantiatedSlots[i];
            Vector3 originalPosition = originalPositions[i];

            // Set slot position to a new vector with these values:
            // x: Original x position minus (index multiplied by horizontal space between rows)
            // y: Vertical space between slots multiplied by (index + 1)
            // z: Original z position
            slot.transform.position = new Vector3(
                originalPosition.x - (i * horizontalSpaceBetweenRows),
                verticalSpaceBetweenSlots * (i + 1),
                originalPosition.z
            );
        }
    }
} 