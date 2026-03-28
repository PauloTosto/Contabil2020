using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace PrjApiParceiro_C.Services
{
    public  static class FindDiretorio
    {
        public static IEnumerable<FileInfo> TraverseDirectory(string rootPath, Func<FileInfo, bool> Pattern)
        {
            var directoryStack = new Stack<DirectoryInfo>();
            directoryStack.Push(new DirectoryInfo(rootPath));
            while (directoryStack.Count > 0)
            {
                var dir = directoryStack.Pop();
                try
                {
                    foreach (var i in dir.GetDirectories())
                        directoryStack.Push(i);
                }
                catch (UnauthorizedAccessException)
                {
                    continue; // We don't have access to this directory, so skip it
                }
                foreach (var f in dir.GetFiles().Where(Pattern)) // "Pattern" is a function
                    yield return f;
            }
        }

    }

}
