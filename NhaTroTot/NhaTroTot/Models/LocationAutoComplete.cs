using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhaTroTot.Model
{
    public static class LocationAutoComplete
    {
        public class MatchedSubstring
        {
            public int length { get; set; }
            public int offset { get; set; }
        }

        public class Term
        {
            public int offset { get; set; }
            public string value { get; set; }
        }

        public class Prediction
        {
            private ICollection<Term> _terms = new ObservableCollection<Term>();
            private ICollection<string> _type = new ObservableCollection<string>();
            public string description { get; set; }
            public string id { get; set; }
            public List<MatchedSubstring> matched_substrings { get; set; }
            public string place_id { get; set; }
            public string reference { get; set; }
            public ICollection<Term> terms { get { return _terms; } set { _terms = value; } }
            public ICollection<string> types { get { return _type; } set { _type = value; } }
        }

        public class RootObject
        {
            private ICollection<Prediction> _predictions = new ObservableCollection<Prediction>();
            public ICollection<Prediction> predictions { get { return _predictions; } set { _predictions = value; } }
            public  string status { get; set; }
        }
    }
}
