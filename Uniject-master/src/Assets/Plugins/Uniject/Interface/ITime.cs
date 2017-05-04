using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniject {
  public interface ITime {
    float DeltaTime {
      get;
    }

    float realtimeSinceStartup { get; }
  }
}
