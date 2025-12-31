using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Course", menuName = "University/Course")]
public class CourseData : ScriptableObject
{
    public string courseName;
    public string shortName;
    public float ects = 5f;
    public Color courseColor = Color.blue;
    public List<Session> sessions;
}

[System.Serializable]
public class Session
{
    // 0 = Monday, 4 = Friday
    [Range(0, 4)] public int dayIndex;

    // 0 = Week 1, 1 = Week 2..
    [Range(0, 2)] public int weekIndex;

    // 8 = 08:00, 14.5 = 14:30
    [Range(8, 20)] public float startTime;

    // Duration in hours
    public float duration;
}