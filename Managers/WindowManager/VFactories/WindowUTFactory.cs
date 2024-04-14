using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PM.UsefulThings
{
    public class WindowUTFactory
    {
        [Inject]
        private IObjectResolver container;
        
        public IWindowUT Create(IWindowUT prefab, Transform parent) 
        {
            var result = container.Instantiate((Component)prefab, parent);

            foreach (var monobeh in result.GetComponentsInChildren<MonoBehaviour>())
            {
                container.Inject(monobeh);
            }

            return (IWindowUT)result;
        }
    }
}
