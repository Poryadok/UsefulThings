using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace PM.UsefulThings.UIBinding
{
    public class ListElementDataFactory
    {
        [Inject] private IObjectResolver container;
        
        public T Create<T>() where T : BaseListElementData
        {
            var result = Activator.CreateInstance<T>();
            container.Inject(result);
            return result;
        }
    }
}
