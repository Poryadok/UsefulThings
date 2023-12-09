using PM.UsefulThings.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.UIBinding
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class IgnoreBinding : Attribute
	{
		
	}
}