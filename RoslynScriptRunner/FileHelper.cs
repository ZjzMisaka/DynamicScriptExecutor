namespace RoslynScriptRunner
{
    public static class FileHelper
    {
        public static FileSystemInfo[] GetDllInfos(string path)
        {
            string folderPath = path;
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileSystemInfo[] dllInfos = null;
            if (dir.Exists)
            {
                DirectoryInfo dirD = dir as DirectoryInfo;
                dllInfos = dirD.GetFileSystemInfos();
            }

            return dllInfos;
        }
    }
}
