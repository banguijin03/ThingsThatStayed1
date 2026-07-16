using System.Collections;
using UnityEngine;
using TMPro;

public class CheckDate : MonoBehaviour
{
    public TextMeshProUGUI Hour;       // 시간
    public TextMeshProUGUI Minutes;    // 분
    public TextMeshProUGUI TimeOfDay;  // AM / PM

    private int hour = 6;
    private int minute = 0;

    void Start()
    {
        StartCoroutine(TimeRoutine());
        UpdateTimeUI();
    }

    IEnumerator TimeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 10초마다 10분 증가

            minute += 10;

            if (minute >= 60)
            {
                minute = 0;
                hour++;

                // 12시간제 유지
                if (hour > 12)
                {
                    hour -=12;
                }
            }

            UpdateTimeUI();
        }
    }

    void UpdateTimeUI()
    {
        // AM / PM 구분
        if (hour >= 6 && hour < 12)
        {
            TimeOfDay.text = "AM";
        }
        else
        {
            TimeOfDay.text = "PM";
        }

        // 두 자리수 표시
        Hour.text = hour.ToString("00");
        Minutes.text = minute.ToString("00");
    }
}