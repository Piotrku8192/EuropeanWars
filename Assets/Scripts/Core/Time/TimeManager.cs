using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.Core.Time {
    public class TimeManager {
        public static readonly int[] daysInMonths = new int[12] {
            31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
        };

        public delegate void OnDayElapsed();
        public delegate void OnMonthElapsed();
        public delegate void OnYearElapsed();

        public static OnDayElapsed onDayElapsed;
        public static OnDayElapsed lateOnDayElapsed;
        public static OnMonthElapsed onMonthElapsed;
        public static OnMonthElapsed lateOnMonthElapsed;
        public static OnYearElapsed onYearElapsed;
        public static OnYearElapsed lateOnYearElapsed;

        public static int year = 1648;
        public static int month = 1;
        public static int day = 1;

        public static void CountDay() {
            day++;

            onDayElapsed?.Invoke();
            lateOnDayElapsed?.Invoke();

            if (day > daysInMonths[month - 1] + (month == 2
                && ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0) ? 1 : 0)) {
                month++;
                day = 1;
                onMonthElapsed?.Invoke();
                lateOnMonthElapsed?.Invoke();
            }

            if (month == 13) {
                month = 1;
                year++;
                onYearElapsed?.Invoke();
                lateOnYearElapsed?.Invoke();
            }

            Timer.Singleton.date.text = ((day < 10) ? "0" : "") + day.ToString() 
                + "." + ((month < 10) ? "0" : "") + month.ToString() 
                + "." + year.ToString();
        }
    }
}
