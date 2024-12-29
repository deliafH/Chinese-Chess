using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq; // Ensure you have Newtonsoft.Json

public class ImageUploader : Singleton<ImageUploader>
{
    [SerializeField]private const string ApiKey = "60dbf67a4c6cc199df7084acf57dc5c9"; // Replace with your API key
    private const string ApiUrl = "https://api.imgbb.com/1/upload";

    public void UploadImage(string imagePath, Action<string> onUploadComplete)
    {
        StartCoroutine(Upload(imagePath, onUploadComplete));
    }

    private IEnumerator Upload(string imagePath, Action<string> onUploadComplete)
    {
        byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
        WWWForm form = new WWWForm();
        form.AddField("key", ApiKey);
        form.AddBinaryData("image", imageData, "image.png", "image/png");

        using (UnityWebRequest www = UnityWebRequest.Post(ApiUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error uploading image: {www.error}");
                onUploadComplete?.Invoke(null); // Invoke callback with null on error
            }
            else
            {
                Debug.Log("Image uploaded successfully!");
                string jsonResponse = www.downloadHandler.text;
                Debug.Log(jsonResponse); // Print the raw JSON response
                string imageUrl = ExtractImageUrl(jsonResponse);
                Debug.Log($"Image URL: {imageUrl}");
                onUploadComplete?.Invoke(imageUrl); // Invoke callback with the image URL
            }
        }
    }

    private string ExtractImageUrl(string jsonResponse)
    {
        var json = JObject.Parse(jsonResponse);
        return json["data"]["url"].ToString(); // Get the direct URL of the uploaded image
    }
}