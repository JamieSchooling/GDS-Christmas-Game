using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GDS
{
    public enum TimerDisplayType
    {
        ProgressBar,
        Digits,
    }

    [RequireComponent(typeof(Timer))]
    public class TimerDisplay : NetworkBehaviour
    {
        [SerializeField] private TimerDisplayType m_DisplayType;
        [SerializeField] private bool m_HideDisplayOnComplete = true;

        [SerializeField, HideInInspector] private Slider m_ProgressBar;
        [SerializeField, HideInInspector] private TextMeshProUGUI m_DisplayText;
        [SerializeField, HideInInspector] private bool m_AlwaysShowMins = false;

        private Timer m_Timer;

        private System.Action<float> InitialiseDisplay = null;
        private System.Action<float> UpdateDisplay = null;
        private GameObject m_DisplayObject = null;

        private Coroutine m_TickCoroutine = null;

        private void Awake()
        {
            m_Timer = GetComponent<Timer>();
            switch (m_DisplayType)
            {
                case TimerDisplayType.ProgressBar:
                    InitialiseDisplay = InitialiseProgressBar;
                    UpdateDisplay = UpdateProgressBar;
                    m_DisplayObject = m_ProgressBar.gameObject;
                    break;
                case TimerDisplayType.Digits:
                    InitialiseDisplay = InitialiseDigitText;
                    UpdateDisplay = UpdateDigitText;
                    m_DisplayObject = m_DisplayText.gameObject;
                    break;
                default:
                    throw new System.NotImplementedException("Given TimerDisplayType not implemented.");
            }

            m_Timer.OnTimerStart.AddListener((duration) =>
            {
                if (NetworkManager.Singleton != null) StartDisplayRpc(duration);
                else StartDisplay(duration);
            });
            m_Timer.OnTimerPause.AddListener((duration) => {
                if (NetworkManager.Singleton != null) PauseDisplayRpc(duration);
                else PauseDisplay(duration);
            });
            m_Timer.OnTimerResume.AddListener((duration) =>
            {
                if (NetworkManager.Singleton != null) ResumeDisplayRpc(duration);
                else ResumeDisplay(duration);
            });
            m_Timer.OnTimerComplete.AddListener(() => {
                if (NetworkManager.Singleton != null) StopDisplayRpc();
                else StopDisplay();
            });
        }

        private void StartDisplay(float duration)
        {
            m_TickCoroutine = StartCoroutine(TickTimerDisplay(duration));
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        private void StartDisplayRpc(float duration)
        {
            StartDisplay(duration);
        }

        private void PauseDisplay(float duration)
        {
            UpdateDisplay(duration);
            if (m_TickCoroutine != null)
            {
                StopCoroutine(m_TickCoroutine);
            }
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        private void PauseDisplayRpc(float duration)
        {
            PauseDisplay(duration);
        }

        private void ResumeDisplay(float duration)
        {
            StartDisplay(duration);
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        private void ResumeDisplayRpc(float duration)
        {
            ResumeDisplay(duration);
        }

        private void StopDisplay()
        {
            if (m_TickCoroutine != null)
            {
                StopCoroutine(m_TickCoroutine);
            }
            if (m_HideDisplayOnComplete) m_DisplayObject.SetActive(false);
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        private void StopDisplayRpc()
        {
            StopDisplay();
        }

        private IEnumerator TickTimerDisplay(float duration)
        {
            float time = duration; 
            m_DisplayObject.SetActive(true);
            InitialiseDisplay(time);

            while (time >= 0.0f)
            {
                UpdateDisplay(time);

                time -= Time.deltaTime;

                yield return null;
            }
        }

        private void InitialiseProgressBar(float duration)
        {
            m_ProgressBar.wholeNumbers = false;
            m_ProgressBar.maxValue = duration;
            m_ProgressBar.minValue = 0.0f;
            m_ProgressBar.value = duration;
        }

        private void InitialiseDigitText(float duration)
        {
            int mins = Mathf.FloorToInt(duration / 60.0f);
            int seconds = Mathf.FloorToInt(duration % 60);
            string displayString = mins > 0 || m_AlwaysShowMins ? 
                $"{mins}:{(seconds >= 10 ? seconds : $"0{seconds}")}" : $"{seconds}";
            m_DisplayText.text = displayString;
        }

        private void UpdateProgressBar(float currentTime)
        {
            m_ProgressBar.value = currentTime;
        }

        private void UpdateDigitText(float currentTime)
        {
            int mins = Mathf.FloorToInt(currentTime / 60.0f);
            int seconds = Mathf.FloorToInt(currentTime % 60.0f);
            string displayString = mins > 0 || m_AlwaysShowMins ? 
                $"{mins:00}:{seconds:00}" : seconds.ToString();
            m_DisplayText.text = displayString;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TimerDisplay))]
    public class TimerDisplayEditor : Editor
    {
        private SerializedProperty m_DisplayTypeProp;
        private SerializedProperty m_ProgressBarProp;
        private SerializedProperty m_DigitTextProp;
        private SerializedProperty m_AlwaysShowMinsProp;

        private void OnEnable()
        {
            m_DisplayTypeProp = serializedObject.FindProperty("m_DisplayType");
            m_ProgressBarProp = serializedObject.FindProperty("m_ProgressBar");
            m_DigitTextProp = serializedObject.FindProperty("m_DisplayText");
            m_AlwaysShowMinsProp = serializedObject.FindProperty("m_AlwaysShowMins");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            switch ((TimerDisplayType)m_DisplayTypeProp.enumValueIndex)
            {
                case TimerDisplayType.ProgressBar:
                    EditorGUILayout.PropertyField(m_ProgressBarProp);
                    serializedObject.ApplyModifiedProperties();
                    break;
                case TimerDisplayType.Digits:
                    EditorGUILayout.PropertyField(m_DigitTextProp);
                    EditorGUILayout.PropertyField(m_AlwaysShowMinsProp);
                    serializedObject.ApplyModifiedProperties();
                    break;
            }

        }
    }
#endif
}