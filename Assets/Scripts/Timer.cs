using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GDS
{
    public class Timer : MonoBehaviour
    {
        [SerializeField, Min(0.0f)] private float m_DefaultDuration;
        [SerializeField] private bool m_StartOnLoad = false;

        public UnityEvent<float> OnTimerStart;
        public UnityEvent<float> OnTimerPause;
        public UnityEvent<float> OnTimerResume;
        public UnityEvent OnTimerComplete;

        public bool IsStarted => m_IsStarted;
        public bool IsPaused => m_IsPaused;

        private Coroutine m_TimerCoroutine = null;

        private float m_TimerStartTime = 0.0f;
        private float m_Duration = 0.0f;
        private bool m_IsStarted = false;
        private bool m_IsPaused = false;
        private Action m_OnTimerCompleteCallback = null;

        private void Start()
        {
            if (m_StartOnLoad) StartTimer();
        }

        public void StartTimer()
        {
            m_Duration = m_DefaultDuration;
            OnTimerStart?.Invoke(m_Duration);
            m_IsStarted = true;
            m_TimerCoroutine = StartCoroutine(TimerCoroutine(m_Duration));
        }

        public void StartTimer(float duration = 0.0f, Action onComplete = null)
        {
            m_Duration = duration <= 0.0f ? m_DefaultDuration : duration;
            OnTimerStart?.Invoke(m_Duration);
            m_OnTimerCompleteCallback = onComplete;
            m_TimerCoroutine = StartCoroutine(TimerCoroutine(m_Duration, m_OnTimerCompleteCallback));
        }
        public void StopTimer()
        {
            m_Duration = 0.0f;
            if (m_TimerCoroutine != null)
            {
                StopCoroutine(m_TimerCoroutine);
                m_TimerCoroutine = null;
                OnTimerComplete?.Invoke();
            }
            m_IsStarted = false;
        }

        public void PauseTimer()
        {
            m_Duration = m_Duration - (Time.time - m_TimerStartTime);
            OnTimerPause?.Invoke(m_Duration);
            m_IsPaused = true;
            if (m_TimerCoroutine != null)
            {
                StopCoroutine(m_TimerCoroutine);
                m_TimerCoroutine = null;
            }
        }

        public void ResumeTimer()
        {
            if (!m_IsPaused) return;
            m_IsPaused = false;
            OnTimerResume?.Invoke(m_Duration);
            m_TimerCoroutine = StartCoroutine(TimerCoroutine(m_Duration, m_OnTimerCompleteCallback));
        }

        private IEnumerator TimerCoroutine(float duration, Action onComplete = null)
        {
            m_TimerStartTime = Time.time;

            yield return new WaitForSeconds(duration);

            onComplete?.Invoke();
            OnTimerComplete?.Invoke();
            m_Duration = 0.0f;
            m_IsStarted = false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Timer))]
    public class TimerEditor : Editor
    {
        private Timer m_Timer;

        private void OnEnable()
        {
            m_Timer = target as Timer;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = !m_Timer.IsStarted;
            if (GUILayout.Button("Start"))
            {
                m_Timer.StartTimer();
            }
            GUI.enabled = m_Timer.IsStarted;
            if (GUILayout.Button("Stop"))
            {
                m_Timer.StopTimer();
            }
            GUI.enabled = m_Timer.IsStarted && !m_Timer.IsPaused;
            if (GUILayout.Button("Pause"))
            {
                m_Timer.PauseTimer();
            }
            GUI.enabled = m_Timer.IsStarted && m_Timer.IsPaused;
            if (GUILayout.Button("Resume"))
            {
                m_Timer.ResumeTimer();
            }
            GUI.enabled = true;
        }
    }

#endif
}
