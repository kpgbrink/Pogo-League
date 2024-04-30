using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts._Game.Objects
{
    public interface IPooledObject
    {
        public ObjectPooler ObjectPooler { get; set;  }
    }
}
