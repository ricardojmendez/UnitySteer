using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Uniject.Impl {
    public class UnityTime : ITime {

        public float DeltaTime {
            get { return Time.deltaTime; }
        }
       
        public float realtimeSinceStartup {
            get { return Time.realtimeSinceStartup; }
        }
    }
}
