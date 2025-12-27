using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GDS
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Animator : MonoBehaviour
    {
        [Serializable]
        private struct SpriteSheetSize
        {
            public SpriteSheetSize(int rows, int columns)
            {
                Rows = rows;
                Columns = columns;
            }

            [SerializeField, Min(1)] public int Rows;
            [SerializeField, Min(1)] public int Columns;
        }

        [SerializeField] private Sprite m_DefaultSpriteSheet;
        [SerializeField] private SpriteSheetSize m_SpriteSheetSize = new(1, 1);
        [SerializeField] private Animation[] m_Animations;
        [SerializeField] private bool m_Play;

        private SpriteRenderer m_SpriteRenderer;
        private List<Sprite> m_Frames = new();

        private Animation m_CurrentAnimation;

        private Dictionary<string, Animation> m_AnimationDictionary = new();

#if UNITY_EDITOR

        private float m_LastTime;
        private bool m_IsSpriteSheetDirty = false;

        private void OnEnable()
        {
            EditorApplication.update += EditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
        }

        private void EditorUpdate()
        {
            if (Application.isPlaying) return;

            if (m_IsSpriteSheetDirty && m_DefaultSpriteSheet != null)
            {
                SetSpriteSheet(m_DefaultSpriteSheet);
                m_IsSpriteSheetDirty = false;
            }

            if (!m_Play) return;

            UpdateFrame((float)EditorApplication.timeSinceStartup - m_LastTime);

            SceneView.RepaintAll();
            m_LastTime = (float)EditorApplication.timeSinceStartup;
        }
#endif

        private void Awake()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();

            if (m_Animations.Length > 0)
                m_CurrentAnimation = m_Animations[0];

            foreach (var animation in m_Animations)
            {
                if (!m_AnimationDictionary.ContainsKey(animation.Name))
                    m_AnimationDictionary.Add(animation.Name, animation);
            }
        }

        private void Start()
        {
            if (m_DefaultSpriteSheet != null)
                SetSpriteSheet(m_DefaultSpriteSheet);
        }

        public void SetSpriteSheet(Sprite sheet)
        {
            m_Frames.Clear();

            int rows = m_SpriteSheetSize.Rows;
            int columns = m_SpriteSheetSize.Columns;

            float cellWidth = sheet.textureRect.width / columns;
            float cellHeight = sheet.textureRect.height / rows;
            for (int i = rows - 1; i >= 0; --i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    m_Frames.Add(
                        Sprite.Create(sheet.texture, new Rect(cellWidth * j, cellHeight * i, cellWidth, cellHeight), new Vector2(0.5f, 0.0f), sheet.pixelsPerUnit)
                    );
                }
            }
            m_SpriteRenderer.sprite = m_Frames[0];
        }

        public void Play(string animationName)
        {
            if (!m_AnimationDictionary.ContainsKey(animationName))
            {
                Debug.LogWarning($"Animation '{animationName}' not found.");
                return;
            }
            if (animationName == m_CurrentAnimation.Name) return;
            m_CurrentAnimation = m_AnimationDictionary[animationName];
            m_CurrentAnimation.Start();
        }

        public void PlayOneShot(string animationName, Action onComplete)
        {
            if (!m_AnimationDictionary.ContainsKey(animationName))
            {
                Debug.Log("Animation not found.");
                return;
            }
            Animation lastAnim = m_CurrentAnimation;
            if (animationName == m_CurrentAnimation.Name) return;
            m_CurrentAnimation = m_AnimationDictionary[animationName];
            bool wasLooped = m_CurrentAnimation.ShouldLoop;
            m_CurrentAnimation.ShouldLoop = false;
            m_CurrentAnimation.OnAnimationEnd += () => {
                m_CurrentAnimation = lastAnim;
                m_CurrentAnimation.ShouldLoop = wasLooped;
                onComplete?.Invoke();
            };
            m_CurrentAnimation.Start();
        }

        private void OnValidate()
        {
            if (m_Animations.Length > 0)
                m_CurrentAnimation = m_Animations[0];

            foreach (var animation in m_Animations)
            {
                if (!m_AnimationDictionary.ContainsKey(animation.Name))
                    m_AnimationDictionary.Add(animation.Name, animation);
            }

#if UNITY_EDITOR
            m_IsSpriteSheetDirty = true;
#endif
        }

        private void Update()
        {
            if (!Application.isPlaying) return;

            UpdateFrame(Time.deltaTime);
        }

        private void UpdateFrame(float deltaTime)
        {
            if (m_Frames.Count <= 0) return;

            m_SpriteRenderer.sprite =
                m_Animations.Length <= 0
                ? m_Frames[0]
                : m_Frames[m_CurrentAnimation.GetNextFrame(deltaTime)];
        }
    }
}
