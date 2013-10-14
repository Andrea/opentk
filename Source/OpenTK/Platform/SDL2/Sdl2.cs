#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2013 Stefanos Apostolopoulos for the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
using System.Runtime.InteropServices;


#endregion

using System;

namespace OpenTK.Platform.Android
{
    partial class SDL
    {
#if ANDROID
        internal const string nativeLibName = "libSDL2.so";
#elif IPHONE
        internal const string nativeLibName = "__Internal";
#else
        internal const string nativeLibName = "SDL2.dll";
#endif

        public readonly static object Sync = new object();
		public readonly static SDL_version Version;

		static SDL()
		{
			SDL.SDL_GetVersion(out Version);
		}

        public enum EventType : uint
        {
            /* Touch events */
            FingerDown      = 0x700,
            FingerUp,
            FingerMotion,

            /* Gesture events */
            DollarGesture   = 0x800,
            DollarRecord,
            MultiGesture,
        }

        public const uint TouchMouseID = 0xffffffff;

        public static class GL
        {
            [DllImport(SDL.nativeLibName, EntryPoint = "SDL_GL_GetCurrentContext")]
            public static extern IntPtr GetCurrentContext();
        }
    }
}

