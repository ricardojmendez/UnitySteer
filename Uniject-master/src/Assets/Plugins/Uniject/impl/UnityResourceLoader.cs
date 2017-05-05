using System;
using UnityEngine;
using System.Xml.Linq;

namespace Uniject.Impl {
    public class UnityResourceLoader : Uniject.IResourceLoader {
        public AudioClip loadClip(string path) {
            AudioClip result = (AudioClip)Resources.Load(path);
            if (null == result) {
                throw new NullReferenceException();
            }

            return result;
        }

        public Material loadMaterial(string path) {
            return (Material) Resources.Load(path);
        }
		
		public XDocument loadDoc(string path) {
            TextAsset textAsset = (TextAsset) Resources.Load(path);
            return XDocument.Parse(textAsset.text);
        }

        public TestableGameObject instantiate(string path) {
            GameObject obj = (GameObject) GameObject.Instantiate(Resources.Load(path));
            return new UnityGameObject(obj);
        }

        public T loadResource<T>(string path) where T : UnityEngine.Object {
            return (T)UnityEngine.Resources.Load(path);
        }
    }
}

