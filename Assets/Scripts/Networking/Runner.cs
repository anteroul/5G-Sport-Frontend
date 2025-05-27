using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class ScriptRunner : MonoBehaviour
{
    private Process pythonProcess;

    void Awake()
    {
        string pythonExe = "python"; // or "python3" if needed on Unix
        string scriptPath = Path.Combine(Application.dataPath, "Resources/gen_data.py");

        // Start the Python script in a new thread
        Thread pythonThread = new Thread(() => RunPythonScript(pythonExe, scriptPath));
        pythonThread.IsBackground = true;
        pythonThread.Start();
    }

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
        }
    }

    void RunPythonScript(string pythonExe, string scriptPath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonExe,
            Arguments = $"\"{scriptPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        pythonProcess = new Process { StartInfo = startInfo };

        try
        {
            pythonProcess.Start();

            // Optional: read output
            string output = pythonProcess.StandardOutput.ReadToEnd();
            string error = pythonProcess.StandardError.ReadToEnd();

            UnityEngine.Debug.Log($"Python Output: {output}");
            if (!string.IsNullOrEmpty(error))
                UnityEngine.Debug.LogError($"Python Error: {error}");

            pythonProcess.WaitForExit();
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"Failed to run Python script: {ex.Message}");
        }
    }
}
