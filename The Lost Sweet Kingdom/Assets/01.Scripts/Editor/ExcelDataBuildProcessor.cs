#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ExcelDataBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        string source = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "ExcelData"));
        string buildDirectory = Path.GetDirectoryName(report.summary.outputPath);
        if (string.IsNullOrWhiteSpace(buildDirectory))
        {
            throw new BuildFailedException("빌드 출력 폴더를 확인할 수 없습니다.");
        }

        string destination = Path.Combine(buildDirectory, "ExcelData");
        if (Directory.Exists(destination))
        {
            Directory.Delete(destination, recursive: true);
        }
        CopyDirectory(source, destination);
        Debug.Log($"ExcelData 복사 완료: {destination}");
    }

    private static void CopyDirectory(string source, string destination)
    {
        if (!Directory.Exists(source))
        {
            throw new BuildFailedException($"ExcelData 폴더를 찾을 수 없습니다: {source}");
        }

        Directory.CreateDirectory(destination);
        foreach (string file in Directory.GetFiles(source))
        {
            string fileName = Path.GetFileName(file);
            if (fileName.StartsWith("~"))
            {
                continue;
            }
            File.Copy(file, Path.Combine(destination, fileName), overwrite: true);
        }

        foreach (string directory in Directory.GetDirectories(source))
        {
            CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
        }
    }
}
#endif
