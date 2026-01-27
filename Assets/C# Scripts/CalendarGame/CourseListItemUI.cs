using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 * CourseListItemUI
 * ----------------
 * Handles the UI representation of a single course in the sidebar.
 * - Shows course name and ECTS
 * - Handles selection/deselection
 * - Updates visual feedback when selected
 */
public class CourseListItemUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;  // Displays the course name
    public TextMeshProUGUI ectsText;  // Displays the ECTS points
    public Image background;          // Background image for selection highlight

    // Internal references
    private CourseData data;
    private CalendarManager manager;
    private bool isSelected = false;

    // Initializes the UI element with the course data and manager reference
    public void Setup(CourseData course, CalendarManager mainManager)
    {
        data = course;
        manager = mainManager;
        nameText.text = course.courseName;
        ectsText.text = $"{course.ects} ECTS";
        UpdateVisuals();
    }

    // Called when the UI item is clicked
    public void OnClick()
    {
        // Toggle selection state
        isSelected = !isSelected;

        // Inform manager of selection change
        manager.ToggleCourse(data, isSelected);

        // Update visual feedback
        UpdateVisuals();
    }

    // Updates background color based on selection state
    private void UpdateVisuals()
    {
        background.color = isSelected ? Color.green : Color.white;
    }
}