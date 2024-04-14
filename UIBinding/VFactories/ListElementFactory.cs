using System.Collections;
using System.Collections.Generic;
using PM.UsefulThings.UIBinding;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PM.UsefulThings.UIBinding.Elements
{
    public class ListElementFactory
    {
        private IObjectResolver container;

        public ListElementFactory(IObjectResolver container)
        {
            this.container = container;
        }
        
        public T Create<T>(T prefab, Transform parent) where T : BaseListElement
        {
            var result = container.Instantiate(prefab, parent, false);
            
            return result;
        }
    }
}
