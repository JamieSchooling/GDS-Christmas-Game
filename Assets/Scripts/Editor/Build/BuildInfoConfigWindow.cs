using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildInfoConfigWindow : EditorWindow
{
    private TextField m_VersionField;
    private TextField m_BuildPathField;

    [MenuItem("Build/Configure")]
    public static void OpenWindow()
    {
        BuildInfoConfigWindow wnd = GetWindow<BuildInfoConfigWindow>();
        wnd.titleContent = new GUIContent("Configure Build Info");
        wnd.saveChangesMessage = "This window has unsaved changes. Would you like to save?";
    }

    public void CreateGUI()
    {
        BuildInfo buildInfo = BuildInfo.GetCurrent();

        VisualElement root = rootVisualElement;

        m_VersionField = new TextField("Version");
        m_VersionField.SetValueWithoutNotify(buildInfo.Version);
        m_VersionField.RegisterValueChangedCallback((version) => hasUnsavedChanges = true);
        root.Add(m_VersionField);

        m_BuildPathField = new TextField("Build Path");
        m_BuildPathField.SetValueWithoutNotify(buildInfo.BuildPath);
        m_BuildPathField.RegisterValueChangedCallback((buildPath) => hasUnsavedChanges = true);
        root.Add(m_BuildPathField);

        Button button = new Button(() => SaveChanges());
        button.text = "Save Changes";
        root.Add(button);
    }

    public override void SaveChanges()
    {
        BuildInfo buildInfo = BuildInfo.GetCurrent();
        buildInfo.Version = m_VersionField.value;
        buildInfo.BuildPath = m_BuildPathField.value;
        BuildInfo.Update(buildInfo);

        hasUnsavedChanges = false;

        base.SaveChanges();
    }
}
