using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CourseListItemUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI ectsText;
    public Image background;

    private CourseData data;
    private CalendarManager manager;
    private bool isSelected = false;

    public void Setup(CourseData course, CalendarManager mainManager)
    {
        data = course;
        manager = mainManager;
        nameText.text = course.courseName;
        ectsText.text = $"{course.ects} ECTS";
        UpdateVisuals();
    }

    public void OnClick()
    {
        isSelected = !isSelected;
        manager.ToggleCourse(data, isSelected);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // Darken if selected, lighter if not (example logic)
        background.color = isSelected ? Color.green : Color.white;
    }
}