
using AnotherFileBrowser.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FileBrowserUpdate : MonoBehaviour
{
    public RawImage rawImage;
    [SerializeField] ProfilePanelScript profile;
    public void OpenFileBrowser()
    {
        var bp = new BrowserProperties();
        bp.filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            //Load image from local path with UWR
            // StartCoroutine(LoadImageLocal(path));
            ImageUploader.Instance.UploadImage(path, OnImageUploaded);
        });
    }

    IEnumerator LoadImageLocal(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            Debug.Log(path);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                rawImage.texture = uwrTexture;
            }
        }
    }

    private void OnImageUploaded(string imageUrl)
     {
         if (string.IsNullOrEmpty(imageUrl))
         {
             Debug.LogError("Image upload failed.");
         }
         else
         {
             Debug.Log($"Received Image URL: {imageUrl}");
             StartCoroutine(LoadImage(imageUrl));
         }
     }

     public IEnumerator LoadImage(string url)
     {
         using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
         {

            profile.avatarLink = url;
             yield return www.SendWebRequest();

             if (www.result != UnityWebRequest.Result.Success)
             {
                 Debug.LogError($"Error loading image: {www.error}");
             }
             else
             {
                 Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                 rawImage.texture = texture; // Set the texture to the RawImage
             }
         }
     }
}
