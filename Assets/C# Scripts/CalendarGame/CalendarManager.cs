using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CalendarManager : MonoBehaviour
{
    [Header("Configuration")]
    public List<CourseData> allCourses; // Drag all your ScriptableObjects here
    public float targetECTS = 30f;

    [Header("UI References")]
    public Transform sidebarContent; // The parent for the list items
    public Transform calendarGrid;   // The parent for the event blocks
    public CourseListItemUI listItemPrefab;
    public GameObject eventBlockPrefab;
    public TextMeshProUGUI ectsLabel;
    public TextMeshProUGUI weekLabel;

    [Header("Grid Settings")]
    public float rowHeight = 50f; // Pixels per hour
    public float colWidth = 100f; // Pixels per day
    // Offset if your grid doesn't start exactly at (0,0) of the container
    public Vector2 startOffset = new Vector2(50, -50);

    // State
    private List<CourseData> selectedCourses = new List<CourseData>();
    private int currentWeek = 0; // 0 to 2
    private float currentTotalECTS = 0;

    void Start()
    {
        GenerateSidebar();
        UpdateCalendar();
    }

    void GenerateSidebar()
    {
        foreach (var course in allCourses)
        {
            var item = Instantiate(listItemPrefab, sidebarContent);
            item.Setup(course, this);
            // Auto-hook the button click
            item.GetComponent<Button>().onClick.AddListener(item.OnClick);
        }
    }

    public void ToggleCourse(CourseData course, bool isSelected)
    {
        if (isSelected)
        {
            if (!selectedCourses.Contains(course))
                selectedCourses.Add(course);
        }
        else
        {
            if (selectedCourses.Contains(course))
                selectedCourses.Remove(course);
        }

        CalculateECTS();
        UpdateCalendar();
    }

    void CalculateECTS()
    {
        currentTotalECTS = 0;
        foreach (var c in selectedCourses) currentTotalECTS += c.ects;

        ectsLabel.text = $"ECTS: {currentTotalECTS} / {targetECTS}";

        CheckWinCondition();
    }

    public void ChangeWeek(int direction)
    {
        currentWeek += direction;
        // Clamp between 0 and 2 (3 weeks)
        currentWeek = Mathf.Clamp(currentWeek, 0, 2);

        weekLabel.text = $"Week {currentWeek + 1}";
        UpdateCalendar();
    }

    void UpdateCalendar()
    {
        // 1. Clear existing blocks
        foreach (Transform child in calendarGrid) Destroy(child.gameObject);

        // 2. Spawn new blocks for selected courses in current week
        List<GameObject> activeBlocks = new List<GameObject>();

        foreach (var course in selectedCourses)
        {
            foreach (var session in course.sessions)
            {
                if (session.weekIndex == currentWeek)
                {
                    CreateBlock(course, session, activeBlocks);
                }
            }
        }

        // 3. Check Overlaps
        CheckOverlaps(activeBlocks);
    }

    void CreateBlock(CourseData course, Session session, List<GameObject> activeBlocks)
    {
        GameObject block = Instantiate(eventBlockPrefab, calendarGrid);

        // Calculate Position
        // X = Day Index * Width
        // Y = (Start Time - 8am) * Height (Negative because Y goes down in UI usually)
        float xPos = startOffset.x + (session.dayIndex * colWidth);
        float yPos = startOffset.y - ((session.startTime - 8f) * rowHeight);

        RectTransform rect = block.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(xPos, yPos);

        // Calculate Size
        rect.sizeDelta = new Vector2(colWidth, session.duration * rowHeight);

        // Set Color and Text (if you have text on the block)
        block.GetComponent<Image>().color = course.courseColor;

        // Store data for overlap checking
        // We attach a temporary simple component or just use tag/name to identify
        CalendarBlock blockData = block.AddComponent<CalendarBlock>();
        blockData.courseName = course.courseName;
        blockData.rect = rect;
        blockData.img = block.GetComponent<Image>();
        blockData.normalColor = course.courseColor;

        activeBlocks.Add(block);
    }

    void CheckOverlaps(List<GameObject> blocks)
    {
        bool globalOverlapFound = false;

        // Reset all to normal color first
        foreach (var b in blocks)
        {
            var cb = b.GetComponent<CalendarBlock>();
            cb.img.color = cb.normalColor;
        }

        // Brute force check (N is small, so this is fine)
        for (int i = 0; i < blocks.Count; i++)
        {
            for (int j = i + 1; j < blocks.Count; j++)
            {
                CalendarBlock b1 = blocks[i].GetComponent<CalendarBlock>();
                CalendarBlock b2 = blocks[j].GetComponent<CalendarBlock>();

                if (IsOverlapping(b1.rect, b2.rect))
                {
                    b1.img.color = Color.red;
                    b2.img.color = Color.red;
                    globalOverlapFound = true;
                }
            }
        }
    }

    bool IsOverlapping(RectTransform r1, RectTransform r2)
    {
        // Simple AABB collision check for UI Rects
        // We use local positions relative to the grid container
        Rect rect1 = new Rect(r1.anchoredPosition.x, r1.anchoredPosition.y - r1.sizeDelta.y, r1.sizeDelta.x, r1.sizeDelta.y);
        Rect rect2 = new Rect(r2.anchoredPosition.x, r2.anchoredPosition.y - r2.sizeDelta.y, r2.sizeDelta.x, r2.sizeDelta.y);

        return rect1.Overlaps(rect2);
    }

    void CheckWinCondition()
    {
        // Logic to check if ECTS >= 30 and no blocks are red
        // You might need to track the "hasOverlap" state globally
    }
}

// Helper class for the blocks
public class CalendarBlock : MonoBehaviour
{
    public string courseName;
    public RectTransform rect;
    public Image img;
    public Color normalColor;
}