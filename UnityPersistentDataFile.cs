using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using UnityEngine;

public class UnityPersistentDataFile<T>
{
    private string folderName;
    private string fileName;

    private string folderPath => Path.Combine(Application.persistentDataPath, folderName);
    private string filePath => Path.Combine(folderPath, fileName);

    private void EnsureDirectoryExists()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"The directory was created successfully, path: {folderPath}");
        }
    }

    public void Delete()
    {
        if (!File.Exists(filePath))
            return;

        File.Delete(filePath);
    }

    public T Load()
    {
        if (!File.Exists(filePath))
            return default;

        string text = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<T>(text);
    }

    public async UniTask<T> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            return default;

        string text = await File.ReadAllTextAsync(filePath, cancellationToken);
        return JsonConvert.DeserializeObject<T>(text);
    }

    public void Save(T data)
    {
        EnsureDirectoryExists();

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public async UniTask SaveAsync(T data, CancellationToken cancellationToken = default)
    {
        EnsureDirectoryExists();

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    public UnityPersistentDataFile(string folderName, string fileName)
    {
        this.folderName = folderName;
        this.fileName = fileName;
    }
}
