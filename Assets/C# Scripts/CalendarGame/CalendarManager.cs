using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/*
 * CalendarManager
 * ----------------
 * Handles the course selection and calendar display for the minigame.
 * - Generates sidebar list of courses
 * - Updates calendar grid each week
 * - Detects overlapping sessions
 * - Tracks total ECTS and triggers win condition
 * - Loads next scene and saves overworld position
 */
public class CalendarManager : MonoBehaviour
{
    [Header("Configuration")]
    public List<CourseData> allCourses;
    public float targetECTS = 30f;

    [Header("Next Scene")]
    public GameObject nextScene;
    public string nextSceneName;

    [Header("UI References")]
    public Transform sidebarContent;
    public Transform calendarGrid; 
    public CourseListItemUI listItemPrefab;
    public GameObject eventBlockPrefab;
    public TextMeshProUGUI ectsLabel;
    public TextMeshProUGUI weekLabel;
    public GameObject winPanel;
    
    // True if any sessions overlap in the current week
    public bool isOverlapping = false;

    [Header("Grid Settings")]
    public float rowHeight = 50f;
    public float colWidth = 100f; 
    public Vector2 startOffset = new Vector2(50, -50);

    [Header("Overworld Settings")]
    [Tooltip("Optional: set this to override the overworld spawn position")]
    public Vector3 overworldSpawnPosition;

    [Header("Level Settings")]
    public string levelName = "CalendarMinigame";

    // Internal state
    private List<CourseData> selectedCourses = new List<CourseData>();
    private int currentWeek = 0;
    private float currentTotalECTS = 0;

    void Start()
    {
        GenerateSidebar();
        UpdateCalendar();
    }

    // Creates the list of selectable courses in the sidebar
    void GenerateSidebar()
    {
        foreach (var course in allCourses)
        {
            var item = Instantiate(listItemPrefab, sidebarContent);
            item.Setup(course, this);
            item.GetComponent<Button>().onClick.AddListener(item.OnClick);
        }
    }

    // Called by UI items when a course is selected or deselected
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

    // Sums ECTS of selected courses and updates UI
    void CalculateECTS()
    {
        currentTotalECTS = 0;
        foreach (var c in selectedCourses) currentTotalECTS += c.ects;

        ectsLabel.text = $"ECTS: {currentTotalECTS} / {targetECTS}";

        CheckWinCondition();
    }

    // Move between weeks
    public void ChangeWeek(int direction)
    {
        currentWeek += direction;
        currentWeek = Mathf.Clamp(currentWeek, 0, 2);

        weekLabel.text = $"Week {currentWeek + 1}";
        UpdateCalendar();
    }

    // Clears and updates calendar grid for current week
    void UpdateCalendar()
    {
        foreach (Transform child in calendarGrid) Destroy(child.gameObject);

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

        CheckOverlaps(activeBlocks);
    }

    // Instantiates a UI block for a course session
    void CreateBlock(CourseData course, Session session, List<GameObject> activeBlocks)
    {
        GameObject block = Instantiate(eventBlockPrefab, calendarGrid);

        float xPos = startOffset.x + (session.dayIndex * colWidth);
        float yPos = startOffset.y - ((session.startTime - 8f) * rowHeight);

        RectTransform rect = block.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(xPos, yPos);
        rect.sizeDelta = new Vector2(colWidth, session.duration * rowHeight);

        block.GetComponent<Image>().color = course.courseColor;

        // Attach CalendarBlock component for tracking
        CalendarBlock blockData = block.AddComponent<CalendarBlock>();
        blockData.courseName = course.courseName;
        blockData.rect = rect;
        blockData.img = block.GetComponent<Image>();
        blockData.normalColor = course.courseColor;

        // Set texts inside the block (name and ECTS)
        var texts = block.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach (var t in texts)
        {
            if (t.name == "BlockNameText")
            {
                blockData.nameText = t;
                blockData.nameText.text = string.IsNullOrEmpty(course.shortName) ? course.courseName : course.shortName;
            }
            else if (t.name == "BlockECTSText")
            {
                blockData.ectsText = t;
                blockData.ectsText.text = course.ects + " ECTS";
            }
        }

        activeBlocks.Add(block);
    }

    // Highlights overlapping sessions in red
    void CheckOverlaps(List<GameObject> blocks)
    {
        isOverlapping = false;

        // Reset colors first
        foreach (var b in blocks)
        {
            var cb = b.GetComponent<CalendarBlock>();
            cb.img.color = cb.normalColor;
        }

        // Check for overlaps
        for (int i = 0; i < blocks.Count; i++)
        {
            for (int j = i + 1; j < blocks.Count; j++)
            {
                CalendarBlock b1 = blocks[i].GetComponent<CalendarBlock>();
                CalendarBlock b2 = blocks[j].GetComponent<CalendarBlock>();

                if (IsOverlapping(b1.rect, b2.rect))
                {
                    // Mark both blocks red
                    b1.img.color = new Color(1f, 0.3f, 0.3f);
                    b2.img.color = new Color(1f, 0.3f, 0.3f);
                    isOverlapping = true; 
                }
            }
        }

        CheckWinCondition();
    }

    // Returns true if two blocks visually overlap
    bool IsOverlapping(RectTransform r1, RectTransform r2)
    {
        Rect rect1 = new Rect(r1.anchoredPosition.x, r1.anchoredPosition.y - r1.sizeDelta.y, r1.sizeDelta.x, r1.sizeDelta.y);
        Rect rect2 = new Rect(r2.anchoredPosition.x, r2.anchoredPosition.y - r2.sizeDelta.y, r2.sizeDelta.x, r2.sizeDelta.y);

        return rect1.Overlaps(rect2);
    }

    // Checks if the player has enough ECTS and no overlaps to win
    void CheckWinCondition()
    {
        bool hasEnoughPoints = currentTotalECTS >= targetECTS;
        bool isSafe = !isOverlapping;

        if (hasEnoughPoints && isSafe)
        {
            if (nextScene != null) nextScene.SetActive(true);
            if(winPanel != null) winPanel.SetActive(true);
        }
        else
        {
            if(nextScene != null) nextScene.SetActive(false);
            if (winPanel != null) winPanel.SetActive(false);
        }
    }

    // Called by UI button to proceed to next scene
    public void LoadNextScene()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CompleteLevel(levelName);
            SaveManager.Instance.SaveOverworldPosition(overworldSpawnPosition);
        }

        SceneManager.LoadScene(nextSceneName);
    }
}

// Attached to each calendar block for tracking its UI and state
public class CalendarBlock : MonoBehaviour
{
    public string courseName;
    public RectTransform rect;
    public Image img;
    public Color normalColor;

    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI ectsText;
}