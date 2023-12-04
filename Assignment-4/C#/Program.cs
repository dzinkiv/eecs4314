using System.Text;
using TACompare;

namespace FileProcessing
{
    class Program {

        const string understandTA = "C:\\Users\\shenx\\source\\repos\\TACompare\\TACompare\\understandTA.raw.ta";
        const string jdepTA = "C:\\Users\\shenx\\source\\repos\\TACompare\\TACompare\\jdepTA.raw.ta";
        const string importTA = "C:\\Users\\shenx\\source\\repos\\TACompare\\TACompare\\Import.raw.ta";
        const string srcMLTA = "C:\\Users\\shenx\\source\\repos\\TACompare\\TACompare\\srcML.raw.ta";
        const string QRESULT = "C:\\Users\\shenx\\source\\repos\\TACompare\\TACompare\\QRESULT.txt";
        const string SRESULT = "C:\\Users\\shenx\\source\\repos\\TACompare\\TACompare\\SRESULT.txt";
        const bool KEY_ANALYSIS = true;

        static void Main(string[] args)
        {
            GetImportDependencies.GenerateImportDependencies();

            Dictionary<string, List<string>> understand = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> jdep = new Dictionary<string, List<string>>(); 
            Dictionary<string, List<string>> srcML = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> import = new Dictionary<string, List<string>>();


            //Loading understand TA file.
            foreach (var line in File.ReadLines(understandTA))
            {
                if (line.StartsWith("$INSTANCE"))
                {
                    string trimmed = line.Trim();

                    int start = trimmed.LastIndexOf('/');
                    int end = trimmed.IndexOf("cFile");

                    if(start >=0 && end >= 0)
                    {
                        string key = trimmed.Substring(start + 1, end - start - 1).Trim();
                        if(!understand.ContainsKey(key))
                            understand.Add(key, new List<string>());
                    }
                }

                else if (line.StartsWith("cLinks"))
                {
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string key = parts[1].Substring(parts[1].LastIndexOf('/')+1).Trim();
                    string value = parts[2].Substring(parts[2].LastIndexOf('/')+1).Trim();

                    if (understand.ContainsKey(key))
                    {
                        understand[key].Add(value);
                    }
                }
            }

            //Loading jdep TA file.
            foreach (var line in File.ReadLines(jdepTA))
            {
                if (line.StartsWith("$INSTANCE"))
                {
                    string trimmed = line.Trim();

                    int start = trimmed.LastIndexOf('.');
                    int end = trimmed.IndexOf("cFile");

                    if (start >= 0)
                    {
                        string key = trimmed.Substring(start + 1, end - start - 1).Trim();
                        key = trimDollar(key);
                        key += ".java";
                        if (!jdep.ContainsKey(key))
                            jdep.Add(key, new List<string>());
                    }
                }

                else if (line.StartsWith("cLinks"))
                {
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string key = parts[1].Substring(parts[1].LastIndexOf('.') + 1).Trim();
                    key = trimDollar(key);
                    key += ".java";
                    string value = parts[2].Substring(parts[2].LastIndexOf('.') + 1).Trim();
                    value = trimDollar(value);
                    value += ".java";
                    if (jdep.ContainsKey(key))
                    {
                        jdep[key].Add(value);
                    }
                }
            }

            //Loading srcML TA file.
            foreach (var line in File.ReadLines(srcMLTA))
            {
                if (line.StartsWith("$INSTANCE"))
                {
                    string trimmed = line.Trim();

                    int start = trimmed.LastIndexOf('/');
                    int end = trimmed.IndexOf("cFile");

                    if (start >= 0 && end >= 0)
                    {
                        string key = trimmed.Substring(start + 1, end - start - 1).Trim();
                        if (!srcML.ContainsKey(key))
                            srcML.Add(key, new List<string>());
                    }
                }

                else if (line.StartsWith("cLinks"))
                {
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string key = parts[1].Substring(parts[1].LastIndexOf('/') + 1).Trim();
                    string value = parts[2].Substring(parts[2].LastIndexOf('/') + 1).Trim();

                    if (srcML.ContainsKey(key))
                    {
                        srcML[key].Add(value);
                    }
                }
            }

            //Loading import TA file.
            foreach (var line in File.ReadLines(importTA))
            {
                if (line.StartsWith("$INSTANCE"))
                {
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string key = parts[1].Trim();
                    if (!import.ContainsKey(key))
                            import.Add(key, new List<string>());
                }

                else if (line.StartsWith("cLinks"))
                {
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string key = parts[1].Trim();
                    string value = parts[2].Trim();

                    if (import.ContainsKey(key))
                    {
                        import[key].Add(value);
                    }
                }
            }

            var totalKeys = understand.Keys.Union(jdep.Keys).Union(srcML.Keys).Union(import.Keys).ToList();
            string qResult = "Total unique entity count: " + totalKeys.Count() + "\r\n";
            Random rnd = new Random();
            string sResult = "";

            //Key Analysis
            if (KEY_ANALYSIS)
            {
                var commonKeys = understand.Keys.Intersect(jdep.Keys).Intersect(srcML.Keys).Intersect(import.Keys).ToList();
                qResult += "Common entity count: " + commonKeys.Count() + "\r\n";

                var uniqueUKeys = understand.Keys.Except(jdep.Keys).Except(srcML.Keys).Except(import.Keys).ToList();
                var uniqueJKeys = jdep.Keys.Except(understand.Keys).Except(srcML.Keys).Except(import.Keys).ToList();
                var uniqueSKeys = srcML.Keys.Except(understand.Keys).Except(jdep.Keys).Except(import.Keys).ToList();
                var uniqueIKeys = import.Keys.Except(understand.Keys).Except(jdep.Keys).Except(srcML.Keys).ToList();

                qResult += "Unique Understand Entity count: " + uniqueUKeys.Count() + "\r\n";
                qResult += "Unique jdep Entity count: " + uniqueJKeys.Count() + "\r\n";
                qResult += "Unique srcML Entity count: " + uniqueSKeys.Count() + "\r\n";
                qResult += "Unique import Entity count: " + uniqueIKeys.Count() + "\r\n";

                //Jdeps- unique : ALL
                sResult += "jdep unique Entities(all): ";
                sResult += string.Join("\r\n", uniqueJKeys);
                sResult += ("\r\n\r\n");
            }

            int uTotalLink = understand.SelectMany(kvp => kvp.Value).Count();
            int jTotalLink = jdep.SelectMany(kvp => kvp.Value).Count();
            int sTotalLink = srcML.SelectMany(kvp => kvp.Value).Count();
            int iTotalLink = import.SelectMany(kvp => kvp.Value).Count();

            qResult += "Understand dependencies Count: " + uTotalLink + "\r\n";
            qResult += "jdep dependencies Count: " + jTotalLink + "\r\n";
            qResult += "srcML dependencies Count: " + sTotalLink + "\r\n";
            qResult += "import dependencies Count: " + iTotalLink + "\r\n";

            int ulinkCount, slinkCount, ilinkCount,commonlinkCount, uiCount, usCount, isCount;
            ulinkCount = slinkCount = ilinkCount = commonlinkCount = uiCount = usCount = isCount = 0;
            var uLinks = new Dictionary<string, List<string>>();
            var sLinks = new Dictionary<string, List<string>>();
            var iLinks = new Dictionary<string, List<string>>();
            var comLinks = new Dictionary<string, List<string>>();
            var uiLinks = new Dictionary<string, List<string>>(); 
            var usLinks = new Dictionary<string, List<string>>(); 
            var isLinks = new Dictionary<string, List<string>>(); 

            int totalLinks = 0;
            foreach (var key in totalKeys)
            {
                var value1 = understand.ContainsKey(key) ? understand[key] : new List<string>();
                var value3 = srcML.ContainsKey(key) ? srcML[key] : new List<string>();
                var value4 = import.ContainsKey(key) ? import[key] : new List<string>();

                totalLinks += value1.Union(value3).Union(value4).Count();
                var comLink = value1.Intersect(value3).Intersect(value4).ToList();
                var uLink = value1.Except(value4).Except(value3).ToList();
                var sLink = value3.Except(value1).Except(value4).ToList(); 
                var iLink = value4.Except(value1).Except(value3).ToList();
                var uiLink = value1.Intersect(value4).Except(value3).ToList();
                var usLink = value1.Intersect(value3).Except(value4).ToList();
                var isLink = value3.Intersect(value4).Except(value1).ToList();

                if(comLink.Count > 0)
                {
                    comLinks.Add(key, comLink);
                    commonlinkCount += comLink.Count;
                }
                if (uLink.Count > 0)
                {
                    uLinks.Add(key, uLink);
                    ulinkCount += uLink.Count;
                }

                if (iLink.Count > 0)
                {
                    iLinks.Add(key, iLink);
                    ilinkCount += iLink.Count;
                }

                if (sLink.Count > 0)
                {
                    sLinks.Add(key, sLink);
                    slinkCount += sLink.Count;
                }

                if (uiLink.Count > 0)
                {
                    uiLinks.Add(key, uiLink);
                    uiCount += uiLink.Count;
                }
                if (usLink.Count > 0)
                {
                    usLinks.Add(key, usLink);
                    usCount += usLink.Count;
                }
                if (isLink.Count > 0)
                {
                    isLinks.Add(key, isLink);
                    isCount += isLink.Count;
                }

            }
            qResult += "Total unique dependencies Count: " + totalLinks + "\r\n";
            qResult += "Total common dependencies Count: " + commonlinkCount + "\r\n";

            qResult += "Understand uniquely extracted " + ulinkCount + " dependencies over " + uLinks.Count() + " entities.\r\n";
            qResult += "srcML uniquely extracted " + slinkCount + " dependencies over " + sLinks.Count() + " entities.\r\n";
            qResult += "import uniquely extracted " + ilinkCount + " dependencies over " + iLinks.Count() + " entities.\r\n";

            qResult += "Understand AND srcML excluding Common " + usCount +"\r\n";
            qResult += "Understand AND import excluding Common  " + uiCount + "\r\n";
            qResult += "import AND srcML excluding Common " + isCount + "\r\n";


            //Links
            //UNIQUES SAMPLING
            //U 96
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("U unique dependencies: ");
            var sample = uLinks.OrderBy(x => rnd.Next()).Take(96);
            foreach (KeyValuePair<string, List<string>> kvp in sample)
            {
                sb.AppendLine($"Key: {kvp.Key}");
                sb.AppendLine("Values:");
                foreach (string value in kvp.Value)
                {
                    sb.AppendLine(value);
                }
            }
            sb.AppendLine("\r\n\r\n\r\n\r\n");

            //I 93
            sb.AppendLine("I unique dependencies: ");
            sample = iLinks.OrderBy(x => rnd.Next()).Take(93);
            foreach (KeyValuePair<string, List<string>> kvp in sample)
            {
                sb.AppendLine($"Key: {kvp.Key}");
                sb.AppendLine("Values:");
                foreach (string value in kvp.Value)
                {
                    sb.AppendLine(value);
                }
            }
            sb.AppendLine("\r\n\r\n\r\n\r\n");

            //S 95
            sb.AppendLine("S unique dependencies: ");
            sample = sLinks.OrderBy(x => rnd.Next()).Take(95);
            foreach (KeyValuePair<string, List<string>> kvp in sample)
            {
                sb.AppendLine($"Key: {kvp.Key}");
                sb.AppendLine("Values:");
                foreach (string value in kvp.Value)
                {
                    sb.AppendLine(value);
                }
            }
            sb.AppendLine("\r\n\r\n\r\n\r\n");

            //Intersections
            //Commons 96
            sb.AppendLine("Common dependencies: ");
            sample = comLinks.OrderBy(x => rnd.Next()).Take(96);
            foreach (KeyValuePair<string, List<string>> kvp in sample)
            {
                sb.AppendLine($"Key: {kvp.Key}");
                sb.AppendLine("Values:");
                foreach (string value in kvp.Value)
                {
                    sb.AppendLine(value);
                }
            }
            sb.AppendLine("\r\n\r\n\r\n\r\n");

            //U and S 94
            sb.AppendLine("U AND S common dependencies: ");
            sample = usLinks.OrderBy(x => rnd.Next()).Take(94);
            foreach (KeyValuePair<string, List<string>> kvp in sample)
            {
                sb.AppendLine($"Key: {kvp.Key}");
                sb.AppendLine("Values:");
                foreach (string value in kvp.Value)
                {
                    sb.AppendLine(value);
                }
            }
            sb.AppendLine("\r\n\r\n\r\n\r\n");

            //I and S 93
            sb.AppendLine("I AND S common dependencies: ");
            sample = isLinks.OrderBy(x => rnd.Next()).Take(93);
            foreach (KeyValuePair<string, List<string>> kvp in sample)
            {
                sb.AppendLine($"Key: {kvp.Key}");
                sb.AppendLine("Values:");
                foreach (string value in kvp.Value)
                {
                    sb.AppendLine(value);
                }
            }
            sb.AppendLine("\r\n\r\n\r\n\r\n");

            //U and I 96
            sb.AppendLine("U AND S common dependencies: ");
            sample = uiLinks.OrderBy(x => rnd.Next()).Take(96);
            foreach (KeyValuePair<string, List<string>> kvp in sample)
            {
                sb.AppendLine($"Key: {kvp.Key}");
                sb.AppendLine("Values:");
                foreach (string value in kvp.Value)
                {
                    sb.AppendLine(value);
                }
            }       

            string sbRes = sb.ToString();
            sResult += sbRes;
            File.WriteAllText(SRESULT,sResult);
            File.WriteAllText(QRESULT, qResult);

        }

        private static string trimDollar(string str)
        {
            int dollarIndex = str.IndexOf('$');
            if (dollarIndex >= 0)
            {
                str = str.Remove(dollarIndex).Trim();
            }
            return str;
        }
    }
}