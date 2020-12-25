using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DrlTraceView
{
    class APICallLoader
    {
        public static List<APICall> LoadLog(string filename)
        {
            List<APICall> result = new List<APICall>();
            APICall lastApiCall = null;
            APICallParameter lastApiCallParam = null;

            string line;
            Regex r1 = new Regex(@"^~~(\d+)~~\s(.+)",RegexOptions.Compiled);
            Regex r2 = new Regex(@"\s{4}arg\s(\d+):\s(.*)", RegexOptions.Compiled);

            System.IO.StreamReader file = new System.IO.StreamReader(filename);

            int lineNumber = 0;

            while ((line = file.ReadLine()) != null)
            {
                lineNumber++;
                if (line.Length>0 && line[0]=='~')
                {
                    var m = r1.Match(line);
                    if (m.Success)
                    {
                        lastApiCall = new APICall() { Pid = int.Parse(m.Groups[1].Value), Name = m.Groups[2].Value };
                        lastApiCall.Parameters = new List<APICallParameter>();
                        result.Add(lastApiCall);
                    }
                    
                    else
                        MessageBox.Show("failed to parse line number "+ lineNumber+" :" + line);
                      
                }
                else if (line.StartsWith("    arg"))
                {
                    var m = r2.Match(line);
                    if (m.Success)
                    {
                        lastApiCallParam = new APICallParameter() { Pos = int.Parse(m.Groups[1].Value), Value = m.Groups[2].Value };
                        lastApiCall.Parameters.Add(lastApiCallParam);
                    }
                }
                else
                    lastApiCallParam.Value += line;
            }

            file.Close();

            return result;
        }
    }
}
