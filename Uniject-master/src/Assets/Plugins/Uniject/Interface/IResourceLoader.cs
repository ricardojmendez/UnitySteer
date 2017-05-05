using System;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace Uniject {
    public interface IResourceLoader {
        AudioClip loadClip(string path);
        Material loadMaterial(string path);
		XDocument loadDoc(string path);
        TestableGameObject instantiate(string path);
        T loadResource<T>(string path) where T : UnityEngine.Object;
    }
}

