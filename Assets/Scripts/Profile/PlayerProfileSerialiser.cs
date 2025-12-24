using System;
using System.IO;
using System.Text;
using UnityEngine;

[Serializable]
public class PlayerProfileData
{
    public string DisplayName;
}

public static class PlayerProfileSerialiser
{
    public static void SaveProfileData(PlayerProfileData profileData)
    {
        using (var stream = File.Open(Application.persistentDataPath + "/.profile", FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(profileData.DisplayName);
            }
        }
    }

    public static bool TryLoadProfileData(out PlayerProfileData profileData)
    {
        profileData = new();

        if (File.Exists(Application.persistentDataPath + "/.profile"))
        {
            using (var stream = File.Open(Application.persistentDataPath + "/.profile", FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    profileData.DisplayName = reader.ReadString();
                }
            }
            return true;
        }

        return false;
    }
}
