using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormTools.FormDescriptor
{
    public class DynamicPanelDescriptor
    {
        public RectElement RectArea { get; set; }
        public Dictionary<string, bool> panelTriggers = new Dictionary<string, bool>();
        public bool InitiallyVisible { get; set; }
        public List<object> FieldsBelowPanel = new List<object>();
        public List<DynamicPanelDescriptor> PanelsBelowPanel = new List<DynamicPanelDescriptor>();
        public bool RepeatingPanel { get; set; }
        public string FieldId { get; set; }
        public Dictionary<string, bool> TriggersNegated = new Dictionary<string, bool>();
        public bool ShouldMoveFieldsBelow { get; set; }
        public List<object> Children = new List<object>();
        public bool AndTriggers;


        public DynamicPanelDescriptor ()
        {
            ShouldMoveFieldsBelow = true;
            InitiallyVisible = false;
            RepeatingPanel = false;
            AndTriggers = false;
        }

        public bool FieldIsChild(string fldId)
        {
            var fldIndex = "" + fldId; //quick copy - prob not necessary....?
            while(fldIndex.Contains("_"))
            {
                fldIndex = fldIndex.Substring(fldIndex.IndexOf('_') + 1);
            }
            int intInd;
            if (Int32.TryParse(fldIndex, out intInd))
            {
                fldId = fldId.Replace(string.Format("_{0}", intInd), "");
            }

            foreach (var o in Children)
            {
                if (o is DynamicPanelDescriptor)
                {
                    var chi = (DynamicPanelDescriptor)o;
                    if (chi.FieldIsChild(fldId)) return true;
                }
                if (o is FieldDescriptor)
                {
                    var fd = (FieldDescriptor)o;
                    if (fd.FieldId == fldId) return true;
                }
                if (o is string)
                {
                    if (((string)o) == fldId) return true;
                }
            }
            return false;
        }

        public bool FieldVisible(string fldId)
        {
            foreach (var o in Children)
            {
                if (o is DynamicPanelDescriptor)
                {
                    var chi = (DynamicPanelDescriptor)o;
                    if (chi.FieldVisible(fldId)) return true;
                }
                if (o is FieldDescriptor)
                {
                    var fd = (FieldDescriptor)o;
                    if (fd.FieldId == fldId)  return ShouldShowPanel();
                }
                if (o is string)
                {
                    if (((string)o) == fldId) return ShouldShowPanel();
                }
            }
            return false;
        }

        public void SetFieldNegated(string key, bool negated)
        {
            if (TriggersNegated.ContainsKey(key))
            {
                TriggersNegated[key] = negated;
            }
        }

        public bool ShouldShowPanel()
        {
            if (panelTriggers.Count == 0)
            {
                return InitiallyVisible;
            }

            #region 'And' Triggers
            if (AndTriggers)
            {
                foreach (var s in panelTriggers.Keys.ToList())
                {
                    var b = panelTriggers[s];
                    var negate = TriggersNegated[s];
                    if (negate) return false;
                    if ((!b && !s.Contains("!")) 
                        || (b && s.Contains("!")))
                        return false;
                    
                }
                return true;
            }
            #endregion

            #region 'Or' Triggers
            var show = false;
            foreach (var s in panelTriggers.Keys.ToList())
            {
                var b = panelTriggers[s];
                var negate = TriggersNegated[s];
                if ((b && !s.Contains("!") && !negate) || (!b && s.Contains("!") && !negate)) show = true;
            }
            return show;
            #endregion
        }
    }
}
