using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GDS
{
    public class Timer : MonoBehaviour
    {
        [SerializeField, Min(0.0f)] private float m_Duration;
        [SerializeField] private bool m_StartOnLoad = false;

        public UnityEvent OnTimerComplete;

        private void Start()
        {
            if (m_StartOnLoad) StartCoroutine(TimerCoroutine(m_Duration));
        }

        public void StartTimer()
        {
            StartCoroutine(TimerCoroutine(m_Duration));
        }

        public void StartTimer(float duration)
        {
            StartCoroutine(TimerCoroutine(duration));
        }

        private IEnumerator TimerCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);

            OnTimerComplete?.Invoke();
        }
    }
}
