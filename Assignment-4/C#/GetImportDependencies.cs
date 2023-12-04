using System.Text.RegularExpressions;

namespace TACompare
{
    internal static class GetImportDependencies
    {
        const string targetFolder = @"C:\flink";
        const string outputFile = @"C:\Users\shenx\source\repos\TACompare\TACompare\Import.raw.ta";
        public static void GenerateImportDependencies()
        {
            string[] javaFiles = Directory.GetFiles(targetFolder, "*.java", SearchOption.AllDirectories);

            Regex importRegex = new Regex(@"import\s+(org\.apache\.flink\.[\w\.]+);");

            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                foreach (string javaFile in javaFiles)
                {
                    string[] lines = File.ReadAllLines(javaFile);
                    string fileName = Path.GetFileName(javaFile);

                    writer.WriteLine("$INSTANCE " + fileName + " cFile");
                    foreach (string line in lines)
                    {
                        MatchCollection matches = importRegex.Matches(line);

                        if (matches.Count > 0)
                        {
                            foreach (Match match in matches)
                            {
                                string[] dependency = match.Groups[1].Value.Split(".");
                                string className = dependency[dependency.Length - 1];
                                writer.WriteLine("cLinks "+ fileName + " " + className +".java");
                            }
                        }
                    }
                }
            }
        }

    }
}
