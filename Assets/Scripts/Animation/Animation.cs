using System;
using UnityEngine;

namespace GDS
{
    [Serializable]
    public class Animation
    {
        [SerializeField] private string m_Name;
        [SerializeField] private int[] m_Frames;
        [SerializeField] private int m_FrameRate = 8;
        [SerializeField] private bool m_ShouldLoop = false;

        public event Action OnAnimationEnd;
        
        public string Name => m_Name;
        public bool ShouldLoop { get => m_ShouldLoop; set => m_ShouldLoop = value; }

        private int m_CurrentFrameIndex = 0;
        private float m_TimeBetweenFrames;
        private float m_TimeSinceLastFrame = 0.0f;

        public void Start()
        {
            m_CurrentFrameIndex = 0;
            m_TimeSinceLastFrame = 0.0f;
        }

        public int GetNextFrame(float deltaTime)
        {
            m_TimeBetweenFrames = 1.0f / m_FrameRate;

            m_TimeSinceLastFrame += deltaTime;
            if (m_TimeSinceLastFrame >= m_TimeBetweenFrames)
            {
                m_CurrentFrameIndex++;
                if (m_CurrentFrameIndex >= m_Frames.Length)
                {
                    if (m_ShouldLoop)
                    {
                        m_CurrentFrameIndex = 0;
                    }
                    else
                    {
                        m_CurrentFrameIndex = m_Frames.Length - 1;
                        OnAnimationEnd?.Invoke();
                    }
                }
                m_TimeSinceLastFrame = 0;
            }
            return m_Frames[m_CurrentFrameIndex];
        }
    }
}