using UnityEditor;
using UnityEngine;
using System.IO;

public class CustomAudioImporter : EditorWindow
{
    [MenuItem("Tools/2 - MÜZİK VE SESLERİ ÖZELLEŞTİR", false, 2)]
    public static void ShowWindow()
    {
        GetWindow<CustomAudioImporter>("Ses Özelleştirici");
    }

    void OnGUI()
    {
        GUILayout.Label("Kendi Müziklerini Ekle!", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Telif hakları sebebiyle internetten otomatik müzik indiremiyoruz. Ama kendi bilgisayarındaki herhangi bir şarkıyı (mp3, wav, ogg) oyuna saniyeler içinde ekleyebilirsin!", MessageType.Info);

        if (GUILayout.Button("1. Ana Tema Müziğini Seç (Epik Anime Müziği)"))
        {
            ImportAndReplace("Ana Tema Müziği", "Music.wav");
        }

        if (GUILayout.Button("2. Zıplama Sesini Seç"))
        {
            ImportAndReplace("Zıplama Sesi", "jump.wav");
        }

        if (GUILayout.Button("3. Ninja Ölüm Sesini Seç"))
        {
            ImportAndReplace("Ninja Ölüm Sesi", "Death.wav");
        }
    }

    private void ImportAndReplace(string title, string targetFileName)
    {
        string path = EditorUtility.OpenFilePanel(title + " Seç", "", "wav,mp3,ogg");
        if (!string.IsNullOrEmpty(path))
        {
            string destPath = "Assets/Audio/" + targetFileName;
            
            // Eğer dosya varsa üzerine yaz
            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }

            File.Copy(path, destPath);
            AssetDatabase.ImportAsset(destPath, ImportAssetOptions.ForceUpdate);
            
            Debug.Log($"[BAŞARILI] {title} başarıyla güncellendi! Yeni dosya: " + destPath);
            EditorUtility.DisplayDialog("Başarılı", title + " oyuna eklendi!\n\nŞimdi Play tuşuna basıp yeni müziğinin tadını çıkarabilirsin.", "Tamam");
        }
    }
}
