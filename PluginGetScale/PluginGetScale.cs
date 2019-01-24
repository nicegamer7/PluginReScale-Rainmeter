/*
  Copyright (C) 2014 Birunthan Mohanathas

  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU General Public License
  as published by the Free Software Foundation; either version 2
  of the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using Rainmeter;

// Overview: This example demonstrates the basic concept of Rainmeter C# plugins.

// Sample skin:
/*
    [Rainmeter]
    Update=1000
    BackgroundMode=2
    SolidColor=000000

    [mString]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=String

    [mMajor]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=Major

    [mMinor]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=Minor

    [mNumber]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=Number

    [Text1]
    Meter=STRING
    MeasureName=mString
    MeasureName2=mMajor
    MeasureName3=mMinor
    MeasureName4=mNumber
    X=5
    Y=5
    W=300
    H=70
    FontColor=FFFFFF
    Text="String: %1#CRLF#Major: %2#CRLF#Minor: %3#CRLF#Number: %4#CRLF#"

    [Text2]
    Meter=STRING
    MeasureName=mString
    MeasureName2=mMajor
    MeasureName3=mMinor
    MeasureName4=mNumber
    NumOfDecimals=1
    X=5
    Y=5R
    W=300
    H=70
    FontColor=FFFFFF
    Text="String: %1#CRLF#Major: %2#CRLF#Minor: %3#CRLF#Number: %4#CRLF#"
*/

namespace PluginGetScale
{
    internal class Measure
    {
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }

        internal Measure()
        {
        }

        internal void Reload(Rainmeter.API api, ref double maxValue)
        {
        }

        internal double Update()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            double ScreenScalingFactor = (double)PhysicalScreenHeight / (double)LogicalScreenHeight;

            return ScreenScalingFactor;

            // https://stackoverflow.com/a/21450169
        }
    }

    public static class Plugin
    {
        static IntPtr StringBuffer = IntPtr.Zero;

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            GCHandle.FromIntPtr(data).Free();
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            return measure.Update();
        }
    }
}