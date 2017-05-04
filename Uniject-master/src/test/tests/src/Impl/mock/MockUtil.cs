using System;
using UnityEngine;
using Uniject;

namespace Tests {
    public class MockUtil : IUtil {

        public MockUtil() {
            result = new object[0];
            currentTime = DateTime.Now;
        }

        public object[] result { get; set; }

        public T[] getAnyComponentsOfType<T>() where T : class {
            T[] r = new T[result.Length];
            int index = 0;
            foreach (object o in result) {
                r[index] = (T)result[index];
                index++;
            }

            return r;
        }

        public DateTime currentTime { get; set; }

        public string persistentDataPath {
            get { return "/tmp/outline"; }
        }

        public RuntimePlatform Platform {
            get { return RuntimePlatform.Android; }
        }

        public string levelName = "level1";

        public string loadedLevelName() {
            return levelName;
        }
    }
}

