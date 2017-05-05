using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Tests {
    /// <summary>
    /// Makes reference to Unity's TagMangager to verify tag names.
    /// TODO: Use a proper YAML parser.
    /// </summary>
    public class MockLayerMask : ILayerMask {

        private Dictionary<string, int> layerMap = new Dictionary<string, int> ();

        public MockLayerMask() {

            string[] lines = File.ReadAllLines("../../../../ProjectSettings/TagManager.asset");
            var filtered = from l in lines
                where l.Contains("Layer") select l.Split(':')[1];

            filtered = from x in filtered select x.TrimStart(new char[] { ' ' });

            int count = 0;
            foreach (string layer in filtered) {
                layerMap[layer] = count++;
            }
        }

        public int NameToLayer(string name) {
            return layerMap[name];
        }
    }
}

