using TMPro;
using UnityEngine;

namespace GDS
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VersionUIDisplay : MonoBehaviour
    {
        private TextMeshProUGUI m_Text;

        private void Awake()
        {
            m_Text = GetComponent<TextMeshProUGUI>();
            m_Text.text = Application.version;
        }
    }
}
