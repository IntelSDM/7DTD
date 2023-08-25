using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat
{
    class QualityOfLife : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(EnableGC());
            StartCoroutine(DisableGC());
        }
        IEnumerator EnableGC()
        {
            for (; ; )
            {
                
                    UnityEngine.Scripting.GarbageCollector.GCMode = UnityEngine.Scripting.GarbageCollector.Mode.Enabled;
                //   GC.CollectionFrequency
                yield return new WaitForSeconds(15f);
            }
        }

        IEnumerator DisableGC()
        {
            for (; ; )
            {
                if (Globals.Config.Debug.GarbageCollection)
                    UnityEngine.Scripting.GarbageCollector.GCMode = UnityEngine.Scripting.GarbageCollector.Mode.Disabled;
                yield return new WaitForSeconds(7f);
            }
        }
    }
}
