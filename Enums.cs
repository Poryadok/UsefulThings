using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
    public enum Sides2D
    {
        Right,
        Top,
        Left,
        Bottom
    }

    public enum Directions2D
    {
        Right,
        Up,
        Left,
        Down
    }


    public enum NonableDirections2D
    {
        None = 0,
        Right = 1,
        Up = 2,
        Left = 4,
        Down = 8
    }

	public enum WindowCloseModes
	{
		CloseNothing,
		CloseNonSolid,
		CloseEverything
	}

	public enum Resolutions
	{
		Custom = 0, // default
		r1920x1080 = 1, // fullHD // 16:9
		r1280x720 = 2,  // HD // 16:9
		r1920x1200 = 3, // 16:10
		r1440x900 = 4,	// 16:10
		r1024x768 = 5, // 4:3
		r2435x1125 = 6, //iphone X
	}

	public enum TextureQualities
	{
		Bad,
		Usual,
		Same
	}
}