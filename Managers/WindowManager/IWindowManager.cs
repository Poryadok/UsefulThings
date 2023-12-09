using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public interface IWindowManager
	{
		IWindowUT ActiveFrame { get; }
		IWindowUT FrameWithFocus { get; }
		IWindowUT OpenNewFrame(IWindowUT newWindow, WindowCloseMode mode = WindowCloseMode.CloseNonSolid, bool isSolid = false);
		IWindowUT AddChildToActiveFrame(IWindowUT newChild);
	}
}