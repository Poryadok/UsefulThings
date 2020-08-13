using PM.UsefulThings.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public class StateMachine<T>
	{
		public event Action<T, T> OnStateFinish;
		public event Action<T, T> OnStateStart;

		public T CurrentState { get; private set; }

		public StateMachine(T firstState)
		{
			CurrentState = firstState;
			OnStateStart.SafeCall(default(T), CurrentState);
		}

		public bool SwitchState(T newState)
		{
			var oldState = CurrentState;
			OnStateFinish?.Invoke(oldState, newState);
			CurrentState = newState;
			OnStateStart?.Invoke(oldState, newState);
			return true;
		}
	}
}