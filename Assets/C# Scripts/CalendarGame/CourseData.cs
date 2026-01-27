using UnityEngine;
using System.Collections.Generic;

/*
 * CourseData
 * ----------
 * ScriptableObject used to define a university course and its sessions.
 * Stores course name, short name, ECTS points, display color, and scheduled sessions.
 */
[CreateAssetMenu(fileName = "New Course", menuName = "University/Course")]
public class CourseData : ScriptableObject
{
    public string courseName;        // Full course name
    public string shortName;         // Optional short name for UI
    public float ects = 5f;          // Number of ECTS points for this course
    public Color courseColor = Color.blue; // Display color in calendar UI
    public List<Session> sessions;   // List of scheduled sessions for this course
}

/*
 * Session
 * -------
 * Represents a single scheduled session of a course.
 * Contains information about the day, week, start time, and duration.
 */
[System.Serializable]
public class Session
{
    [Range(0, 4)] public int dayIndex;   // 0 = Monday, 4 = Friday
    [Range(0, 2)] public int weekIndex;  // 0 = Week 1, 1 = Week 2, 2 = Week 3
    [Range(8, 20)] public float startTime; // Start time in 24-hour format (e.g., 14.5 = 14:30)
    public float duration;                // Duration in hours
}