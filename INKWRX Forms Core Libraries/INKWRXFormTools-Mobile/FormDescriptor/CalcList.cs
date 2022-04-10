using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormTools.FormDescriptor
{
    public class CalcList
    {
        public string FieldName { get; set; }
        public List<string> Inputs { get; set; }
        public ISOFieldDescriptor Descriptor { get; set; }
        public object FieldView { get; set; }

        public static List<string> FieldList(string input)
        {
            var ret = new List<string>();
            var pattern = @"#(\w[\w0-9]*)(?::(\w+))?#";
            var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            var matches = rgx.Matches(input);
            foreach (Match match in matches)
            {
                ret.Add(match.Groups[1].ToString());
            }
            return ret;
        }

        private static Tuple<List<CalcList>, int> reorderListWithLastItem(List<CalcList> original, int insertedIndex)
        {
            if (original.Count < 2)
            {
                return Tuple.Create(original, 0);
            }
            var newList = original.ToList();
            var listItem = original[insertedIndex];
            var change = 0;
            for (var i = insertedIndex - 1; i >= 0; i--)
            {
                if (i + change < 0) break;
                var thisItem = newList[i + change];
                if (thisItem.Inputs.Where(t => t==listItem.FieldName).Any())
                {
                    newList.RemoveAt(i + change);
                    newList.Insert(insertedIndex + change, thisItem);
                    var result = reorderListWithLastItem(newList, insertedIndex + change);
                    change += result.Item2 - 1;
                }
            }
            return Tuple.Create(newList, change);
        }

        public static List<CalcList> GetOrderedList(List<CalcList> original)
        {
            var newList = new List<CalcList>();
            foreach (var calc in original)
            {
                if (newList.Count == 0)
                {
                    newList.Add(calc);
                    continue;
                }
                var existing = newList.Select(x => x.FieldName).ToList();
                if (calc.Inputs.Any(f=>existing.Any(e=>e==f)))
                {
                    //exists - one of the inputs exists in the list already, insert this record AFTER the last one in the list...
                    var insert = newList.Count;
                    for (var i = newList.Count - 1; i >= 0; i--)
                    {
                        var thisItem = newList[i];
                        if (calc.Inputs.Any(input => input==thisItem.FieldName))
                        {
                            insert = i + 1;
                            break;
                        }
                    }
                    newList.Insert(insert, calc);
                    var res = reorderListWithLastItem(newList, insert);
                    newList = res.Item1;
                }
                else
                {
                    //doesn't exist...
                    newList.Insert(0, calc);
                }
            }
            return newList;
        }
    }
}
