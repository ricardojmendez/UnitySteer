using System;
using System.Collections.Generic;
using Uniject;
using UnityEngine;
using System.IO;
using System.Xml.Linq;
using Ninject;
using Moq;

namespace Tests {

    /// <summary>
    /// Mock resource loader.
    /// TODO: fix the ludicrous, broken file existence checking.
    /// </summary>
    public class MockResourceLoader : IResourceLoader {

        private string resourcesPath = Path.GetFullPath("../../../../Assets/resources");
        private IKernel kernel;
        private static List<string> knownExtensions = new List<string>() { ".ogg", ".wav", ".mat", ".mp3", ".physicMaterial" };

        public MockResourceLoader(IKernel kernel) {
            this.kernel = kernel;
        }

        public Material loadMaterial(string path) {
            string filepath = Path.Combine(resourcesPath, path);
            if (exists(filepath)) {
                return null;
            }

            throw new FileNotFoundException (path);
        }

        public AudioClip loadClip(string path) {

            string filepath = Path.Combine(resourcesPath, path);

            if (exists(filepath)) {
                return new AudioClip ();
            }

            throw new FileNotFoundException (path);
        }

        public XDocument loadDoc(string path) {
            path += ".xml";
            return XDocument.Load(Path.Combine(resourcesPath, path));
        }

        public TestableGameObject instantiate(string path) {
            path += ".prefab";
            string filepath = Path.Combine(resourcesPath, path);
            FileInfo file = new FileInfo (filepath);
            if (file.Exists) {
                return kernel.Get<TestableGameObject>();
            }

            throw new FileNotFoundException (path);
        }

        public T loadResource<T>(string path) where T : UnityEngine.Object {
            string filepath = Path.Combine(resourcesPath, path);
            if (exists(filepath)) {
                return kernel.Get<T>();
            }

            throw new FileNotFoundException(filepath);
        }

        private static bool exists(string filepath) {
            foreach (string extension in knownExtensions) {
                FileInfo file = new FileInfo (filepath + extension);
                if (file.Exists) {
                    return true;
                }
            }

            return false;
        }
    }
}

