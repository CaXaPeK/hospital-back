using Hospital.Database.TableModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hospital.Database.Icd
{
    public class IcdCsvParser
    {
        public IcdCsvParser()
        {
            LoadedDiagnoses = new List<string>();
        }

        private List<string> LoadedDiagnoses;

        public void LoadAndReformatCsv(string filename)
        {
            List<string> lines = File.ReadAllLines(filename).ToList();

            Console.WriteLine("Reformatting CSV...");

            AddRootCodes(lines);
            ImproveRootElementCodes(lines);
            ReplaceIds(lines);
            RemoveUnactual(lines);
            lines.RemoveAt(0);

            LoadedDiagnoses = lines;
        }

        public List<Diagnosis> GetDiagnosesList()
        {
            List<Diagnosis> diagnoses = new List<Diagnosis>();
            foreach (string line in LoadedDiagnoses)
            {
                List<string> attributes = line.Split(';').ToList();

                bool actual = attributes[0] == "1" ? true : false;
                int? addlCode = attributes[1] == "" ? null : int.Parse(attributes[1]);
                DateOnly? date = attributes[2] == "" ? null : DateOnly.ParseExact(attributes[2].Trim('\"'), "dd.MM.yyyy", null);
                DateTime createDate = DateTime.UtcNow;
                Guid? parentId = attributes[3] == "" ? null : new Guid(attributes[3].Trim('\"'));
                string mkbCode = attributes[4].Trim('\"');
                string mkbName = attributes[5].Trim('\"');
                string recCode = attributes[6].Trim('\"');
                Guid id = new Guid(attributes[7].Trim('\"'));
                string rootCode = attributes[8].Trim('\"');

                diagnoses.Add(new Diagnosis(actual, addlCode, date, createDate, parentId, mkbCode, mkbName, recCode, id, rootCode));
            }

            return diagnoses;
        }

        private int ParentId(string line)
        {
            string value = line.Split(';')[3];
            return value == "" ? 0 : int.Parse(value);
        }

        private string MkbCode(string line)
        {
            return line.Split(';')[4];
        }

        private int Id(string line)
        {
            string value = line.Split(';')[7];
            return value == "" ? 0 : int.Parse(value);
        }

        private void ImproveRootElementCodes(List<string> lines)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string curGroup = "";
            string firstCode = "";
            string lastCode = "";

            for (int i = 1; i < lines.Count; i++)
            {
                if (ParentId(lines[i]) == 0)
                {
                    if (firstCode != "")
                    {
                        dict.Add(curGroup, '\"' + firstCode + '-' + lastCode + '\"');
                        Console.WriteLine(curGroup + ' ' + firstCode + '-' + lastCode);
                    }

                    curGroup = MkbCode(lines[i]);
                    firstCode = "";
                    lastCode = "";

                    continue;
                }

                if (firstCode == "" && !MkbCode(lines[i]).Contains('-') && !MkbCode(lines[i]).Contains('.'))
                {
                    firstCode = MkbCode(lines[i]).Trim('\"');
                }
                if (!MkbCode(lines[i]).Contains('-') && !MkbCode(lines[i]).Contains('.'))
                {
                    lastCode = MkbCode(lines[i]).Trim('\"');
                }
            }

            dict.Add(curGroup, firstCode + '-' + lastCode);
            Console.WriteLine(curGroup + ' ' + firstCode + '-' + lastCode);
            Console.ReadKey();

            for (int i = 1; i < lines.Count; i++)
            {
                string newLine = "";
                List<string> attributes = lines[i].Split(';').ToList();
                if (dict.ContainsKey(attributes[4]))
                {
                    attributes[4] = dict[attributes[4]];
                }
                if (dict.ContainsKey(attributes[8]))
                {
                    attributes[8] = dict[attributes[8]];
                }
                for (int j = 0; j < attributes.Count; j++)
                {
                    newLine += attributes[j];
                    if (j != attributes.Count - 1)
                    {
                        newLine += ";";
                    }
                }

                lines[i] = newLine;

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Improving root element codes... " + Math.Round(i / ((double)lines.Count - 1) * 100) + "%");
            }

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("Improving root element codes... DONE");
        }

        private void AddRootCodes(List<string> lines)
        {
            lines[0] += ";ROOT_CODE";

            for (int i = 1; i < lines.Count; i++)
            {
                int j = i;
                while (ParentId(lines[j]) != 0)
                {
                    int parent = ParentId(lines[j]);
                    for (j = 1; j < lines.Count; j++)
                    {
                        if (Id(lines[j]) == parent)
                        {
                            break;
                        }
                    }
                }

                lines[i] += ";" + MkbCode(lines[j]);

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Adding root codes... " + Math.Round(i / ((double)lines.Count - 1) * 100) + "%");
            }

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Adding root codes... DONE");
        }

        private void ReplaceIds(List<string> lines)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            for (int i = 1; i < lines.Count; i++)
            {
                dict.Add(lines[i].Split(';')[7], '\"' + Guid.NewGuid().ToString() + '\"');
            }

            for (int i = 1; i < lines.Count; i++)
            {
                string newLine = "";
                List<string> attributes = lines[i].Split(';').ToList();
                attributes[3] = attributes[3] != "" ? dict[attributes[3]] : "";
                attributes[7] = dict[attributes[7]];
                for (int j = 0; j < attributes.Count; j++)
                {
                    newLine += attributes[j];
                    if (j != attributes.Count - 1)
                    {
                        newLine += ";";
                    }
                }

                lines[i] = newLine;

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Replacing IDs... " + Math.Round(i / ((double)lines.Count - 1) * 100) + "%");
            }

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Replacing IDs... DONE");
        }

        private void RemoveUnactual(List<string> lines)
        {
            for (int i = 1; i < lines.Count; i++)
            {
                if (lines[i][0] == '0')
                {
                    lines.RemoveAt(i);
                    i--;

                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("Removing unactual diagnoses... " + Math.Round(i / ((double)lines.Count - 1) * 100) + "%");
                }
            }

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Removing unactual diagnoses... DONE");
        }
    }
}
