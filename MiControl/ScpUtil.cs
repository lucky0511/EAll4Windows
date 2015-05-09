﻿using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Reflection;
using System.Xml;
using System.Drawing;

using System.Security.Principal;
namespace MiWindows
{
    [Flags]
    public enum MiKeyType : byte { None = 0, ScanCode = 1, Toggle = 2, Unbound = 4, Macro = 8, HoldMacro = 16, RepeatMacro = 32 }; //Increment by exponents of 2*, starting at 2^0
    public enum Ds3PadId : byte { None = 0xFF, One = 0x00, Two = 0x01, Three = 0x02, Four = 0x03, All = 0x04 };
    //!public enum MiControls : byte { None, LXNeg, LXPos, LYNeg, LYPos, RXNeg, RXPos, RYNeg, RYPos, L1, L2, L3, R1, R2, R3, Square, Triangle, Circle, Cross, DpadUp, DpadRight, DpadDown, DpadLeft, PS, TouchLeft, TouchUpper, TouchMulti, TouchRight, Share, Options, GyroXPos, GyroXNeg, GyroZPos, GyroZNeg, SwipeLeft, SwipeRight, SwipeUp, SwipeDown };
    public enum MiControls : byte { None
        , LXNeg, LXPos, LYNeg, LYPos, RXNeg, RXPos, RYNeg, RYPos
        , L1, L2, LT
        , R1, R2, RT
        , A, B, X, Y
        , DpadUp, DpadRight, DpadDown, DpadLeft
        , Back, Menu
        , LS, RS
        , HomeSimulated};
    public enum X360Controls : byte { None, LXNeg, LXPos, LYNeg, LYPos, RXNeg, RXPos, RYNeg, RYPos, LB, LT, LS, RB, RT, RS, X, Y, B, A, DpadUp, DpadRight, DpadDown, DpadLeft, Guide, Back, Start, LeftMouse, RightMouse, MiddleMouse, FourthMouse, FifthMouse, WUP, WDOWN, MouseUp, MouseDown, MouseLeft, MouseRight, Unbound };

    public class DebugEventArgs : EventArgs
    {
        protected DateTime m_Time = DateTime.Now;
        protected String m_Data = String.Empty;
        protected bool warning = false;
        public DebugEventArgs(String Data, bool warn)
        {
            m_Data = Data;
            warning = warn;
        }

        public DateTime Time
        {
            get { return m_Time; }
        }

        public String Data
        {
            get { return m_Data; }
        }
        public bool Warning
        {
            get { return warning; }
        }
    }

    public class MappingDoneEventArgs : EventArgs
    {
        protected int deviceNum = -1;

        public MappingDoneEventArgs(int DeviceID)
        {
            deviceNum = DeviceID;
        }

        public int DeviceID
        {
            get { return deviceNum; }
        }
    }

    public class ReportEventArgs : EventArgs
    {
        protected Ds3PadId m_Pad = Ds3PadId.None;
        protected Byte[] m_Report = new Byte[64];

        public ReportEventArgs()
        {
        }

        public ReportEventArgs(Ds3PadId Pad)
        {
            m_Pad = Pad;
        }

        public Ds3PadId Pad
        {
            get { return m_Pad; }
            set { m_Pad = value; }
        }

        public Byte[] Report
        {
            get { return m_Report; }
        }
    }

    public class Global
    {
        protected static BackingStore m_Config = new BackingStore();
        protected static Int32 m_IdleTimeout = 600000;
        static string exepath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
        public static string appdatapath;
        public static string[] tempprofilename = new string[5] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

        public static void SaveWhere(string path)
        {
            appdatapath = path;
            m_Config.m_Profile = appdatapath + "\\Profiles.xml";
            m_Config.m_Actions = appdatapath + "\\Actions.xml";
        }
        /// <summary>
        /// Check if Admin Rights are needed to write in Appliplation Directory
        /// </summary>
        /// <returns></returns>
        public static bool AdminNeeded()
        {
            try
            {
                File.WriteAllText(exepath + "\\test.txt", "test");
                File.Delete(exepath + "\\test.txt");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static event EventHandler<EventArgs> ControllerStatusChange; // called when a controller is added/removed/battery or touchpad mode changes/etc.
        public static void ControllerStatusChanged(object sender)
        {
            if (ControllerStatusChange != null)
                ControllerStatusChange(sender, EventArgs.Empty);
        }

        //general values
        public static bool UseExclusiveMode
        {
            set { m_Config.useExclusiveMode = value; }
            get { return m_Config.useExclusiveMode; }
        }

        public static DateTime LastChecked
        {
            set { m_Config.lastChecked = value; }
            get { return m_Config.lastChecked; }
        }

        public static int CheckWhen
        {
            set { m_Config.CheckWhen = value; }
            get { return m_Config.CheckWhen; }
        }
        public static bool Notifications
        {
            set { m_Config.notifications = value; }
            get { return m_Config.notifications; }
        }
        public static bool DCBTatStop
        {
            set { m_Config.disconnectBTAtStop = value; }
            get { return m_Config.disconnectBTAtStop; }
        }
        public static bool SwipeProfiles
        {
            set { m_Config.swipeProfiles = value; }
            get { return m_Config.swipeProfiles; }
        }
        public static bool MiMapping
        {
            set { m_Config.miMapping = value; }
            get { return m_Config.miMapping; }
        }
        public static bool QuickCharge
        {
            set { m_Config.quickCharge = value; }
            get { return m_Config.quickCharge; }
        }
        public static int FirstXinputPort
        {
            set { m_Config.firstXinputPort = value; }
            get { return m_Config.firstXinputPort; }
        }
        public static bool CloseMini
        {
            set { m_Config.closeMini = value; }
            get { return m_Config.closeMini; }
        }
        public static bool StartMinimized
        {
            set { m_Config.startMinimized = value; }
            get { return m_Config.startMinimized; }
        }
        public static int FormWidth
        {
            set { m_Config.formWidth = value; }
            get { return m_Config.formWidth; }
        }
        public static int FormHeight
        {
            set { m_Config.formHeight = value; }
            get { return m_Config.formHeight; }
        }
        public static bool DownloadLang
        {
            set { m_Config.downloadLang = value; }
            get { return m_Config.downloadLang; }
        }
        public static bool FlashWhenLate
        {
            set { m_Config.flashWhenLate = value; }
            get { return m_Config.flashWhenLate; }
        }

        //controller/profile specfic values
        public static int[] ButtonMouseSensitivity
        {
            get { return m_Config.buttonMouseSensitivity; }
        }

        public static byte[] RumbleBoost
        {
            get { return m_Config.rumble; }
        }
        public static double[] Rainbow
        {
            get { return m_Config.rainbow; }
        }
        public static bool[] FlushHIDQueue
        {
            get { return m_Config.flushHIDQueue; }
        }
        public static int[] IdleDisconnectTimeout
        {
            get { return m_Config.idleDisconnectTimeout; }
        }
        public static byte[] TouchSensitivity
        {
            get { return m_Config.touchSensitivity; }
        }
        public static byte[] FlashType
        {
            get { return m_Config.flashType; }
        }
        public static int[] FlashAt
        {
            get { return m_Config.flashAt; }
        }
        public static bool[] LedAsBatteryIndicator
        {
            get { return m_Config.ledAsBattery; }
        }
        public static int[] ChargingType
        {
            get { return m_Config.chargingType; }
        }
        public static bool[] DinputOnly
        {
            get { return m_Config.dinputOnly; }
        }
        public static bool[] StartTouchpadOff
        {
            get { return m_Config.startTouchpadOff; }
        }
        public static bool[] UseTPforControls
        {
            get { return m_Config.useTPforControls; }
        }
        public static MiColor[] MainColor
        {
            get { return m_Config.m_Leds; }
        }
        public static MiColor[] LowColor
        {
            get { return m_Config.m_LowLeds; }
        }
        public static MiColor[] ChargingColor
        {
            get { return m_Config.m_ChargingLeds; }
        }

        public static MiColor[] FlashColor
        {
            get { return m_Config.m_FlashLeds; }
        }
        public static MiColor[] ShiftColor
        {
            get { return m_Config.m_ShiftLeds; }
        }
        public static bool[] ShiftColorOn
        {
            get { return m_Config.shiftColorOn; }
        }
        public static byte[] TapSensitivity
        {
            get { return m_Config.tapSensitivity; }
        }
        public static bool[] DoubleTap
        {
            get { return m_Config.doubleTap; }
        }
        public static int[] ScrollSensitivity
        {
            get { return m_Config.scrollSensitivity; }
        }
        public static bool[] LowerRCOn
        {
            get { return m_Config.lowerRCOn; }
        }
        public static bool[] TouchpadJitterCompensation
        {
            get { return m_Config.touchpadJitterCompensation; }
        }


        public static byte[] L2Deadzone
        {
            get { return m_Config.l2Deadzone; }
        }
        public static byte[] R2Deadzone
        {
            get { return m_Config.r2Deadzone; }
        }
        public static double[] SXDeadzone
        {
            get { return m_Config.SXDeadzone; }
        }
        public static double[] SZDeadzone
        {
            get { return m_Config.SZDeadzone; }
        }
        public static int[] LSDeadzone
        {
            get { return m_Config.LSDeadzone; }
        }
        public static int[] RSDeadzone
        {
            get { return m_Config.RSDeadzone; }
        }
        public static int[] LSCurve
        {
            get { return m_Config.lsCurve; }
        }
        public static int[] RSCurve
        {
            get { return m_Config.rsCurve; }
        }
        public static bool[] MouseAccel
        {
            get { return m_Config.mouseAccel; }
        }
        public static int[] ShiftModifier
        {
            get { return m_Config.shiftModifier; }
        }
        public static string[] LaunchProgram
        {
            get { return m_Config.launchProgram; }
        }
        public static string[] ProfilePath
        {
            get { return m_Config.profilePath; }
        }
        public static List<String>[] ProfileActions
        {
            get { return m_Config.profileActions; }
        }

        public static void SaveAction(string name, string controls, int mode, string details, bool edit, string extras = "")
        {
            m_Config.SaveAction(name, controls, mode, details, edit, extras);
        }

        public static void RemoveAction(string name)
        {
            m_Config.RemoveAction(name);
        }

        public static bool LoadActions()
        {
            return m_Config.LoadActions();
        }

        public static List<SpecialAction> GetActions()
        {
            return m_Config.actions;
        }

        public static int GetActionIndexOf(string name)
        {
            for (int i = 0; i < m_Config.actions.Count; i++)
                if (m_Config.actions[i].name == name)
                    return i;
            return -1;
        }

        public static SpecialAction GetAction(string name)
        {
            foreach (SpecialAction sA in m_Config.actions)
                if (sA.name == name)
                    return sA;
            return new SpecialAction("null", "null", "null", "null");
        }


        public static X360Controls getCustomButton(int device, MiControls controlName)
        {
            return m_Config.GetCustomButton(device, controlName);
        }
        public static ushort getCustomKey(int device, MiControls controlName)
        {
            return m_Config.GetCustomKey(device, controlName);
        }
        public static string getCustomMacro(int device, MiControls controlName)
        {
            return m_Config.GetCustomMacro(device, controlName);
        }
        public static string getCustomExtras(int device, MiControls controlName)
        {
            return m_Config.GetCustomExtras(device, controlName);
        }
        public static MiKeyType getCustomKeyType(int device, MiControls controlName)
        {
            return m_Config.GetCustomKeyType(device, controlName);
        }
        public static bool getHasCustomKeysorButtons(int device)
        {
            return m_Config.customMapButtons[device].Count > 0
                || m_Config.customMapKeys[device].Count > 0;
        }
        public static bool getHasCustomExtras(int device)
        {
            return m_Config.customMapExtras[device].Count > 0;
        }
        public static Dictionary<MiControls, X360Controls> getCustomButtons(int device)
        {
            return m_Config.customMapButtons[device];
        }
        public static Dictionary<MiControls, ushort> getCustomKeys(int device)
        {
            return m_Config.customMapKeys[device];
        }
        public static Dictionary<MiControls, string> getCustomMacros(int device)
        {
            return m_Config.customMapMacros[device];
        }
        public static Dictionary<MiControls, string> getCustomExtras(int device)
        {
            return m_Config.customMapExtras[device];
        }
        public static Dictionary<MiControls, MiKeyType> getCustomKeyTypes(int device)
        {
            return m_Config.customMapKeyTypes[device];
        }

        public static X360Controls getShiftCustomButton(int device, MiControls controlName)
        {
            return m_Config.GetShiftCustomButton(device, controlName);
        }
        public static ushort getShiftCustomKey(int device, MiControls controlName)
        {
            return m_Config.GetShiftCustomKey(device, controlName);
        }
        public static string getShiftCustomMacro(int device, MiControls controlName)
        {
            return m_Config.GetShiftCustomMacro(device, controlName);
        }
        public static string getShiftCustomExtras(int device, MiControls controlName)
        {
            return m_Config.GetShiftCustomExtras(device, controlName);
        }
        public static MiKeyType getShiftCustomKeyType(int device, MiControls controlName)
        {
            return m_Config.GetShiftCustomKeyType(device, controlName);
        }
        public static bool getHasShiftCustomKeysorButtons(int device)
        {
            return m_Config.shiftCustomMapButtons[device].Count > 0
                || m_Config.shiftCustomMapKeys[device].Count > 0;
        }
        public static bool getHasShiftCustomExtras(int device)
        {
            return m_Config.shiftCustomMapExtras[device].Count > 0;
        }
        public static Dictionary<MiControls, X360Controls> getShiftCustomButtons(int device)
        {
            return m_Config.shiftCustomMapButtons[device];
        }
        public static Dictionary<MiControls, ushort> getShiftCustomKeys(int device)
        {
            return m_Config.shiftCustomMapKeys[device];
        }
        public static Dictionary<MiControls, string> getShiftCustomMacros(int device)
        {
            return m_Config.shiftCustomMapMacros[device];
        }
        public static Dictionary<MiControls, string> getShiftCustomExtras(int device)
        {
            return m_Config.shiftCustomMapExtras[device];
        }
        public static Dictionary<MiControls, MiKeyType> getShiftCustomKeyTypes(int device)
        {
            return m_Config.shiftCustomMapKeyTypes[device];
        }
        public static bool Load()
        {
            return m_Config.Load();
        }
        public static void LoadProfile(int device, System.Windows.Forms.Control[] buttons, System.Windows.Forms.Control[] shiftbuttons, bool launchprogram, ControlService control)
        {
            m_Config.LoadProfile(device, buttons, shiftbuttons, launchprogram, control);
            tempprofilename[device] = string.Empty;
        }
        public static void LoadProfile(int device, bool launchprogram, ControlService control)
        {
            m_Config.LoadProfile(device, null, null, launchprogram, control);
            tempprofilename[device] = string.Empty;

        }
        public static void LoadTempProfile(int device, string name, bool launchprogram, ControlService control)
        {
            m_Config.LoadProfile(device, null, null, launchprogram, control, appdatapath + @"\Profiles\" + name + ".xml");
            tempprofilename[device] = name;
        }
        public static bool Save()
        {
            return m_Config.Save();
        }

        public static void SaveProfile(int device, string propath, System.Windows.Forms.Control[] buttons, System.Windows.Forms.Control[] shiftbuttons)
        {
            m_Config.SaveProfile(device, propath, buttons, shiftbuttons);
        }

        private static byte applyRatio(byte b1, byte b2, double r)
        {
            if (r > 100)
                r = 100;
            else if (r < 0)
                r = 0;
            r /= 100f;
            return (byte)Math.Round((b1 * (1 - r) + b2 * r), 0);
        }
        public static MiColor getTransitionedColor(MiColor c1, MiColor c2, double ratio)
        {//;
            //Color cs = Color.FromArgb(c1.red, c1.green, c1.blue);
            c1.red = applyRatio(c1.red, c2.red, ratio);
            c1.green = applyRatio(c1.green, c2.green, ratio);
            c1.blue = applyRatio(c1.blue, c2.blue, ratio);
            return c1;
        }

        private static Color applyRatio(Color c1, Color c2, uint r)
        {
            float ratio = r / 100f;
            float hue1 = c1.GetHue();
            float hue2 = c2.GetHue();
            float bri1 = c1.GetBrightness();
            float bri2 = c2.GetBrightness();
            float sat1 = c1.GetSaturation();
            float sat2 = c2.GetSaturation();
            float hr = hue2 - hue1;
            float br = bri2 - bri1;
            float sr = sat2 - sat1;
            Color csR;
            if (bri1 == 0)
                csR = HuetoRGB(hue2, sat2, bri2 - br * ratio);
            else
                csR = HuetoRGB(hue2 - hr * ratio, sat2 - sr * ratio, bri2 - br * ratio);
            return csR;
        }

        public static Color HuetoRGB(float hue, float sat, float bri)
        {
            float C = (1 - Math.Abs(2 * bri) - 1) * sat;
            float X = C * (1 - Math.Abs((hue / 60) % 2 - 1));
            float m = bri - C / 2;
            float R, G, B;
            if (0 <= hue && hue < 60)
            { R = C; G = X; B = 0; }
            else if (60 <= hue && hue < 120)
            { R = X; G = C; B = 0; }
            else if (120 <= hue && hue < 180)
            { R = 0; G = C; B = X; }
            else if (180 <= hue && hue < 240)
            { R = 0; G = X; B = C; }
            else if (240 <= hue && hue < 300)
            { R = X; G = 0; B = C; }
            else if (300 <= hue && hue < 360)
            { R = C; G = 0; B = X; }
            else
            { R = 255; G = 0; B = 0; }
            R += m; G += m; B += m;
            R *= 255; G *= 255; B *= 255;
            return Color.FromArgb((int)R, (int)G, (int)B);
        }

    }



    public class BackingStore
    {
        //public String m_Profile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MiTool" + "\\Profiles.xml";
        public String m_Profile = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "\\Profiles.xml";
        public String m_Actions = Global.appdatapath + "\\Actions.xml";

        protected XmlDocument m_Xdoc = new XmlDocument();
        //fifth value used to for options, not fifth controller
        public int[] buttonMouseSensitivity = { 25, 25, 25, 25, 25 };

        public bool[] flushHIDQueue = { true, true, true, true, true };
        public int[] idleDisconnectTimeout = { 0, 0, 0, 0, 0 };
        public Boolean[] touchpadJitterCompensation = { true, true, true, true, true };
        public Boolean[] lowerRCOn = { false, false, false, false, false };
        public Boolean[] ledAsBattery = { false, false, false, false, false };
        public Byte[] flashType = { 0, 0, 0, 0, 0 };
        public Byte[] l2Deadzone = { 0, 0, 0, 0, 0 }, r2Deadzone = { 0, 0, 0, 0, 0 };
        public String[] profilePath = { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty };
        public Byte[] rumble = { 100, 100, 100, 100, 100 };
        public Byte[] touchSensitivity = { 100, 100, 100, 100, 100 };
        public int[] LSDeadzone = { 0, 0, 0, 0, 0 }, RSDeadzone = { 0, 0, 0, 0, 0 };
        public double[] SXDeadzone = { 0.25, 0.25, 0.25, 0.25, 0.25 }, SZDeadzone = { 0.25, 0.25, 0.25, 0.25, 0.25 };
        public Byte[] tapSensitivity = { 0, 0, 0, 0, 0 };
        public bool[] doubleTap = { false, false, false, false, false };
        public int[] scrollSensitivity = { 0, 0, 0, 0, 0 };
        public double[] rainbow = { 0, 0, 0, 0, 0 };
        public int[] flashAt = { 0, 0, 0, 0, 0 };
        public int[] shiftModifier = { 0, 0, 0, 0, 0 };
        public bool[] mouseAccel = { true, true, true, true, true };
        public MiColor[] m_LowLeds = new MiColor[]
        {
             new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black)
        };
        public MiColor[] m_Leds = new MiColor[]
        {
            new MiColor(Color.Blue),
            new MiColor(Color.Red),
            new MiColor(Color.Green),
            new MiColor(Color.Pink),
            new MiColor(Color.White)
        };
        public MiColor[] m_ChargingLeds = new MiColor[] 
        {
             new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black)
        };
        public MiColor[] m_ShiftLeds = new MiColor[]
        {
            new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black)
        };
        public MiColor[] m_FlashLeds = new MiColor[] 
        {
             new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black),
            new MiColor(Color.Black)
        };
        public bool[] shiftColorOn = { false, false, false, false, false };
        public int[] chargingType = { 0, 0, 0, 0, 0 };
        public string[] launchProgram = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        public bool[] dinputOnly = { false, false, false, false, false };
        public bool[] startTouchpadOff = { false, false, false, false, false };
        public bool[] useTPforControls = { false, false, false, false, false };
        public int[] lsCurve = { 0, 0, 0, 0, 0 };
        public int[] rsCurve = { 0, 0, 0, 0, 0 };
        public Boolean useExclusiveMode = false;
        public Int32 formWidth = 782;
        public Int32 formHeight = 550;
        public Boolean startMinimized = false;
        public DateTime lastChecked;
        public int CheckWhen = 1;
        public bool notifications = true;
        public bool disconnectBTAtStop = false;
        public bool swipeProfiles = true;
        public bool miMapping = true;
        public bool quickCharge = false;
        public int firstXinputPort = 1;
        public bool closeMini = false;
        public List<SpecialAction> actions = new List<SpecialAction>();
        public Dictionary<MiControls, MiKeyType>[] customMapKeyTypes = { null, null, null, null, null };
        public Dictionary<MiControls, UInt16>[] customMapKeys = { null, null, null, null, null };
        public Dictionary<MiControls, String>[] customMapMacros = { null, null, null, null, null };
        public Dictionary<MiControls, X360Controls>[] customMapButtons = { null, null, null, null, null };
        public Dictionary<MiControls, String>[] customMapExtras = { null, null, null, null, null };

        public Dictionary<MiControls, MiKeyType>[] shiftCustomMapKeyTypes = { null, null, null, null, null };
        public Dictionary<MiControls, UInt16>[] shiftCustomMapKeys = { null, null, null, null, null };
        public Dictionary<MiControls, String>[] shiftCustomMapMacros = { null, null, null, null, null };
        public Dictionary<MiControls, X360Controls>[] shiftCustomMapButtons = { null, null, null, null, null };
        public Dictionary<MiControls, String>[] shiftCustomMapExtras = { null, null, null, null, null };
        public List<String>[] profileActions = { null, null, null, null, null };
        public bool downloadLang = true;
        public bool flashWhenLate = true;
        public BackingStore()
        {
            for (int i = 0; i < 5; i++)
            {
                customMapKeyTypes[i] = new Dictionary<MiControls, MiKeyType>();
                customMapKeys[i] = new Dictionary<MiControls, UInt16>();
                customMapMacros[i] = new Dictionary<MiControls, String>();
                customMapButtons[i] = new Dictionary<MiControls, X360Controls>();
                customMapExtras[i] = new Dictionary<MiControls, string>();

                shiftCustomMapKeyTypes[i] = new Dictionary<MiControls, MiKeyType>();
                shiftCustomMapKeys[i] = new Dictionary<MiControls, UInt16>();
                shiftCustomMapMacros[i] = new Dictionary<MiControls, String>();
                shiftCustomMapButtons[i] = new Dictionary<MiControls, X360Controls>();
                shiftCustomMapExtras[i] = new Dictionary<MiControls, string>();
                profileActions[i] = new List<string>();
                profileActions[i].Add("Disconnect Controller");
            }
        }

        public X360Controls GetCustomButton(int device, MiControls controlName)
        {
            if (customMapButtons[device].ContainsKey(controlName))
                return customMapButtons[device][controlName];
            else return X360Controls.None;
        }
        public UInt16 GetCustomKey(int device, MiControls controlName)
        {
            if (customMapKeys[device].ContainsKey(controlName))
                return customMapKeys[device][controlName];
            else return 0;
        }
        public string GetCustomMacro(int device, MiControls controlName)
        {
            if (customMapMacros[device].ContainsKey(controlName))
                return customMapMacros[device][controlName];
            else return "0";
        }
        public string GetCustomExtras(int device, MiControls controlName)
        {
            if (customMapExtras[device].ContainsKey(controlName))
                return customMapExtras[device][controlName];
            else return "0";
        }
        public MiKeyType GetCustomKeyType(int device, MiControls controlName)
        {
            try
            {
                if (customMapKeyTypes[device].ContainsKey(controlName))
                    return customMapKeyTypes[device][controlName];
                else return 0;
            }
            catch { return 0; }
        }

        public X360Controls GetShiftCustomButton(int device, MiControls controlName)
        {
            if (shiftCustomMapButtons[device].ContainsKey(controlName))
                return shiftCustomMapButtons[device][controlName];
            else return X360Controls.None;
        }
        public UInt16 GetShiftCustomKey(int device, MiControls controlName)
        {
            if (shiftCustomMapKeys[device].ContainsKey(controlName))
                return shiftCustomMapKeys[device][controlName];
            else return 0;
        }
        public string GetShiftCustomMacro(int device, MiControls controlName)
        {
            if (shiftCustomMapMacros[device].ContainsKey(controlName))
                return shiftCustomMapMacros[device][controlName];
            else return "0";
        }
        public string GetShiftCustomExtras(int device, MiControls controlName)
        {
            if (customMapExtras[device].ContainsKey(controlName))
                return customMapExtras[device][controlName];
            else return "0";
        }
        public MiKeyType GetShiftCustomKeyType(int device, MiControls controlName)
        {
            try
            {
                if (shiftCustomMapKeyTypes[device].ContainsKey(controlName))
                    return shiftCustomMapKeyTypes[device][controlName];
                else return 0;
            }
            catch { return 0; }
        }

        public Boolean SaveProfile(int device, String propath, System.Windows.Forms.Control[] buttons, System.Windows.Forms.Control[] shiftbuttons)
        {
            Boolean Saved = true;
            String path = Global.appdatapath + @"\Profiles\" + Path.GetFileNameWithoutExtension(propath) + ".xml";
            try
            {
                XmlNode Node;
                XmlNode xmlControls = m_Xdoc.SelectSingleNode("/MiWindows/Control");
                XmlNode xmlShiftControls = m_Xdoc.SelectSingleNode("/MiWindows/ShiftControl");
                m_Xdoc.RemoveAll();

                Node = m_Xdoc.CreateXmlDeclaration("1.0", "utf-8", String.Empty);
                m_Xdoc.AppendChild(Node);

                Node = m_Xdoc.CreateComment(String.Format(" MiWindows Configuration Data. {0} ", DateTime.Now));
                m_Xdoc.AppendChild(Node);

                Node = m_Xdoc.CreateWhitespace("\r\n");
                m_Xdoc.AppendChild(Node);

                Node = m_Xdoc.CreateNode(XmlNodeType.Element, "MiWindows", null);

                XmlNode xmlFlushHIDQueue = m_Xdoc.CreateNode(XmlNodeType.Element, "flushHIDQueue", null); xmlFlushHIDQueue.InnerText = flushHIDQueue[device].ToString(); Node.AppendChild(xmlFlushHIDQueue);
                XmlNode xmlIdleDisconnectTimeout = m_Xdoc.CreateNode(XmlNodeType.Element, "idleDisconnectTimeout", null); xmlIdleDisconnectTimeout.InnerText = idleDisconnectTimeout[device].ToString(); Node.AppendChild(xmlIdleDisconnectTimeout);
                XmlNode xmlColor = m_Xdoc.CreateNode(XmlNodeType.Element, "Color", null);
                xmlColor.InnerText = m_Leds[device].red.ToString() + "," + m_Leds[device].green.ToString() + "," + m_Leds[device].blue.ToString();
                Node.AppendChild(xmlColor);
                XmlNode xmlRumbleBoost = m_Xdoc.CreateNode(XmlNodeType.Element, "RumbleBoost", null); xmlRumbleBoost.InnerText = rumble[device].ToString(); Node.AppendChild(xmlRumbleBoost);
                XmlNode xmlLedAsBatteryIndicator = m_Xdoc.CreateNode(XmlNodeType.Element, "ledAsBatteryIndicator", null); xmlLedAsBatteryIndicator.InnerText = ledAsBattery[device].ToString(); Node.AppendChild(xmlLedAsBatteryIndicator);
                XmlNode xmlLowBatteryFlash = m_Xdoc.CreateNode(XmlNodeType.Element, "FlashType", null); xmlLowBatteryFlash.InnerText = flashType[device].ToString(); Node.AppendChild(xmlLowBatteryFlash);
                XmlNode xmlFlashBatterAt = m_Xdoc.CreateNode(XmlNodeType.Element, "flashBatteryAt", null); xmlFlashBatterAt.InnerText = flashAt[device].ToString(); Node.AppendChild(xmlFlashBatterAt);
                XmlNode xmlTouchSensitivity = m_Xdoc.CreateNode(XmlNodeType.Element, "touchSensitivity", null); xmlTouchSensitivity.InnerText = touchSensitivity[device].ToString(); Node.AppendChild(xmlTouchSensitivity);
                XmlNode xmlLowColor = m_Xdoc.CreateNode(XmlNodeType.Element, "LowColor", null);
                xmlLowColor.InnerText = m_LowLeds[device].red.ToString() + "," + m_LowLeds[device].green.ToString() + "," + m_LowLeds[device].blue.ToString();
                Node.AppendChild(xmlLowColor);
                XmlNode xmlChargingColor = m_Xdoc.CreateNode(XmlNodeType.Element, "ChargingColor", null);
                xmlChargingColor.InnerText = m_ChargingLeds[device].red.ToString() + "," + m_ChargingLeds[device].green.ToString() + "," + m_ChargingLeds[device].blue.ToString();
                Node.AppendChild(xmlChargingColor);
                XmlNode xmlShiftColor = m_Xdoc.CreateNode(XmlNodeType.Element, "ShiftColor", null);
                xmlShiftColor.InnerText = m_ShiftLeds[device].red.ToString() + "," + m_ShiftLeds[device].green.ToString() + "," + m_ShiftLeds[device].blue.ToString();
                Node.AppendChild(xmlShiftColor);
                XmlNode xmlShiftColorOn = m_Xdoc.CreateNode(XmlNodeType.Element, "ShiftColorOn", null); xmlShiftColorOn.InnerText = shiftColorOn[device].ToString(); Node.AppendChild(xmlShiftColorOn);
                XmlNode xmlFlashColor = m_Xdoc.CreateNode(XmlNodeType.Element, "FlashColor", null);
                xmlFlashColor.InnerText = m_FlashLeds[device].red.ToString() + "," + m_FlashLeds[device].green.ToString() + "," + m_FlashLeds[device].blue.ToString();
                Node.AppendChild(xmlFlashColor);
                XmlNode xmlTouchpadJitterCompensation = m_Xdoc.CreateNode(XmlNodeType.Element, "touchpadJitterCompensation", null); xmlTouchpadJitterCompensation.InnerText = touchpadJitterCompensation[device].ToString(); Node.AppendChild(xmlTouchpadJitterCompensation);
                XmlNode xmlLowerRCOn = m_Xdoc.CreateNode(XmlNodeType.Element, "lowerRCOn", null); xmlLowerRCOn.InnerText = lowerRCOn[device].ToString(); Node.AppendChild(xmlLowerRCOn);
                XmlNode xmlTapSensitivity = m_Xdoc.CreateNode(XmlNodeType.Element, "tapSensitivity", null); xmlTapSensitivity.InnerText = tapSensitivity[device].ToString(); Node.AppendChild(xmlTapSensitivity);
                XmlNode xmlDouble = m_Xdoc.CreateNode(XmlNodeType.Element, "doubleTap", null); xmlDouble.InnerText = doubleTap[device].ToString(); Node.AppendChild(xmlDouble);
                XmlNode xmlScrollSensitivity = m_Xdoc.CreateNode(XmlNodeType.Element, "scrollSensitivity", null); xmlScrollSensitivity.InnerText = scrollSensitivity[device].ToString(); Node.AppendChild(xmlScrollSensitivity);
                XmlNode xmlLeftTriggerMiddle = m_Xdoc.CreateNode(XmlNodeType.Element, "LeftTriggerMiddle", null); xmlLeftTriggerMiddle.InnerText = l2Deadzone[device].ToString(); Node.AppendChild(xmlLeftTriggerMiddle);
                XmlNode xmlRightTriggerMiddle = m_Xdoc.CreateNode(XmlNodeType.Element, "RightTriggerMiddle", null); xmlRightTriggerMiddle.InnerText = r2Deadzone[device].ToString(); Node.AppendChild(xmlRightTriggerMiddle);
                XmlNode xmlButtonMouseSensitivity = m_Xdoc.CreateNode(XmlNodeType.Element, "ButtonMouseSensitivity", null); xmlButtonMouseSensitivity.InnerText = buttonMouseSensitivity[device].ToString(); Node.AppendChild(xmlButtonMouseSensitivity);
                XmlNode xmlRainbow = m_Xdoc.CreateNode(XmlNodeType.Element, "Rainbow", null); xmlRainbow.InnerText = rainbow[device].ToString(); Node.AppendChild(xmlRainbow);
                XmlNode xmlLSD = m_Xdoc.CreateNode(XmlNodeType.Element, "LSDeadZone", null); xmlLSD.InnerText = LSDeadzone[device].ToString(); Node.AppendChild(xmlLSD);
                XmlNode xmlRSD = m_Xdoc.CreateNode(XmlNodeType.Element, "RSDeadZone", null); xmlRSD.InnerText = RSDeadzone[device].ToString(); Node.AppendChild(xmlRSD);
                XmlNode xmlSXD = m_Xdoc.CreateNode(XmlNodeType.Element, "SXDeadZone", null); xmlSXD.InnerText = SXDeadzone[device].ToString(); Node.AppendChild(xmlSXD);
                XmlNode xmlSZD = m_Xdoc.CreateNode(XmlNodeType.Element, "SZDeadZone", null); xmlSZD.InnerText = SZDeadzone[device].ToString(); Node.AppendChild(xmlSZD);
                XmlNode xmlChargingType = m_Xdoc.CreateNode(XmlNodeType.Element, "ChargingType", null); xmlChargingType.InnerText = chargingType[device].ToString(); Node.AppendChild(xmlChargingType);
                XmlNode xmlMouseAccel = m_Xdoc.CreateNode(XmlNodeType.Element, "MouseAcceleration", null); xmlMouseAccel.InnerText = mouseAccel[device].ToString(); Node.AppendChild(xmlMouseAccel);
                XmlNode xmlShiftMod = m_Xdoc.CreateNode(XmlNodeType.Element, "ShiftModifier", null); xmlShiftMod.InnerText = shiftModifier[device].ToString(); Node.AppendChild(xmlShiftMod);
                XmlNode xmlLaunchProgram = m_Xdoc.CreateNode(XmlNodeType.Element, "LaunchProgram", null); xmlLaunchProgram.InnerText = launchProgram[device].ToString(); Node.AppendChild(xmlLaunchProgram);
                XmlNode xmlDinput = m_Xdoc.CreateNode(XmlNodeType.Element, "DinputOnly", null); xmlDinput.InnerText = dinputOnly[device].ToString(); Node.AppendChild(xmlDinput);
                XmlNode xmlStartTouchpadOff = m_Xdoc.CreateNode(XmlNodeType.Element, "StartTouchpadOff", null); xmlStartTouchpadOff.InnerText = startTouchpadOff[device].ToString(); Node.AppendChild(xmlStartTouchpadOff);
                XmlNode xmlUseTPforControls = m_Xdoc.CreateNode(XmlNodeType.Element, "UseTPforControls", null); xmlUseTPforControls.InnerText = useTPforControls[device].ToString(); Node.AppendChild(xmlUseTPforControls);
                XmlNode xmlLSC = m_Xdoc.CreateNode(XmlNodeType.Element, "LSCurve", null); xmlLSC.InnerText = lsCurve[device].ToString(); Node.AppendChild(xmlLSC);
                XmlNode xmlRSC = m_Xdoc.CreateNode(XmlNodeType.Element, "RSCurve", null); xmlRSC.InnerText = rsCurve[device].ToString(); Node.AppendChild(xmlRSC);
                XmlNode xmlProfileActions = m_Xdoc.CreateNode(XmlNodeType.Element, "ProfileActions", null); xmlProfileActions.InnerText = string.Join("/", profileActions[device]); Node.AppendChild(xmlProfileActions);
                XmlNode NodeControl = m_Xdoc.CreateNode(XmlNodeType.Element, "Control", null);

                XmlNode Key = m_Xdoc.CreateNode(XmlNodeType.Element, "Key", null);
                XmlNode Macro = m_Xdoc.CreateNode(XmlNodeType.Element, "Macro", null);
                XmlNode KeyType = m_Xdoc.CreateNode(XmlNodeType.Element, "KeyType", null);
                XmlNode Button = m_Xdoc.CreateNode(XmlNodeType.Element, "Button", null);
                XmlNode Extras = m_Xdoc.CreateNode(XmlNodeType.Element, "Extras", null);
                if (buttons != null)
                {
                    foreach (var button in buttons)
                    {
                        // Save even if string (for xbox controller buttons)
                        if (button.Tag != null)
                        {
                            XmlNode buttonNode;
                            string keyType = String.Empty;

                            if (button.Tag is KeyValuePair<string, string>)
                                if (((KeyValuePair<string, string>)button.Tag).Key == "Unbound")
                                    keyType += MiKeyType.Unbound;

                            if (button.Font.Strikeout)
                                keyType += MiKeyType.HoldMacro;
                            if (button.Font.Underline)
                                keyType += MiKeyType.Macro;
                            if (button.Font.Italic)
                                keyType += MiKeyType.Toggle;
                            if (button.Font.Bold)
                                keyType += MiKeyType.ScanCode;
                            if (keyType != String.Empty)
                            {
                                buttonNode = m_Xdoc.CreateNode(XmlNodeType.Element, button.Name, null);
                                buttonNode.InnerText = keyType;
                                KeyType.AppendChild(buttonNode);
                            }

                            string[] extras;
                            buttonNode = m_Xdoc.CreateNode(XmlNodeType.Element, button.Name, null);
                            if (button.Tag is KeyValuePair<IEnumerable<int>, string> || button.Tag is KeyValuePair<Int32[], string> || button.Tag is KeyValuePair<UInt16[], string>)
                            {
                                KeyValuePair<Int32[], string> tag = (KeyValuePair<Int32[], string>)button.Tag;
                                int[] ii = tag.Key;
                                buttonNode.InnerText = string.Join("/", ii);
                                Macro.AppendChild(buttonNode);
                                extras = tag.Value.Split(',');
                            }
                            else if (button.Tag is KeyValuePair<Int32, string> || button.Tag is KeyValuePair<UInt16, string> || button.Tag is KeyValuePair<byte, string>)
                            {
                                KeyValuePair<int, string> tag = (KeyValuePair<int, string>)button.Tag;
                                buttonNode.InnerText = tag.Key.ToString();
                                Key.AppendChild(buttonNode);
                                extras = tag.Value.Split(',');
                            }
                            else if (button.Tag is KeyValuePair<string, string>)
                            {
                                KeyValuePair<string, string> tag = (KeyValuePair<string, string>)button.Tag;
                                buttonNode.InnerText = tag.Key;
                                Button.AppendChild(buttonNode);
                                extras = tag.Value.Split(',');
                            }
                            else
                            {
                                KeyValuePair<object, string> tag = (KeyValuePair<object, string>)button.Tag;
                                extras = tag.Value.Split(',');
                            }
                            bool hasvalue = false;
                            foreach (string s in extras)
                                if (s != "0")
                                {
                                    hasvalue = true;
                                    break;
                                }
                            if (hasvalue)
                            {
                                XmlNode extraNode = m_Xdoc.CreateNode(XmlNodeType.Element, button.Name, null);
                                extraNode.InnerText = String.Join(",", extras);
                                Extras.AppendChild(extraNode);
                            }
                        }
                    }
                    Node.AppendChild(NodeControl);
                    if (Button.HasChildNodes)
                        NodeControl.AppendChild(Button);
                    if (Macro.HasChildNodes)
                        NodeControl.AppendChild(Macro);
                    if (Key.HasChildNodes)
                        NodeControl.AppendChild(Key);
                    if (Extras.HasChildNodes)
                        NodeControl.AppendChild(Extras);
                    if (KeyType.HasChildNodes)
                        NodeControl.AppendChild(KeyType);
                }
                else if (xmlControls != null)
                    Node.AppendChild(xmlControls);
                if (shiftModifier[device] > 0)
                {
                    XmlNode NodeShiftControl = m_Xdoc.CreateNode(XmlNodeType.Element, "ShiftControl", null);

                    XmlNode ShiftKey = m_Xdoc.CreateNode(XmlNodeType.Element, "Key", null);
                    XmlNode ShiftMacro = m_Xdoc.CreateNode(XmlNodeType.Element, "Macro", null);
                    XmlNode ShiftKeyType = m_Xdoc.CreateNode(XmlNodeType.Element, "KeyType", null);
                    XmlNode ShiftButton = m_Xdoc.CreateNode(XmlNodeType.Element, "Button", null);
                    XmlNode ShiftExtras = m_Xdoc.CreateNode(XmlNodeType.Element, "Extras", null);
                    if (shiftbuttons != null)
                    {
                        foreach (var button in shiftbuttons)
                        {
                            // Save even if string (for xbox controller buttons)
                            if (button.Tag != null)
                            {
                                XmlNode buttonNode;
                                string keyType = String.Empty;
                                if (button.Tag is KeyValuePair<string, string>)
                                    if (((KeyValuePair<string, string>)button.Tag).Key == "Unbound")
                                        keyType += MiKeyType.Unbound;

                                if (button.Font.Strikeout)
                                    keyType += MiKeyType.HoldMacro;
                                if (button.Font.Underline)
                                    keyType += MiKeyType.Macro;
                                if (button.Font.Italic)
                                    keyType += MiKeyType.Toggle;
                                if (button.Font.Bold)
                                    keyType += MiKeyType.ScanCode;
                                if (keyType != String.Empty)
                                {
                                    buttonNode = m_Xdoc.CreateNode(XmlNodeType.Element, button.Name, null);
                                    buttonNode.InnerText = keyType;
                                    ShiftKeyType.AppendChild(buttonNode);
                                }

                                string[] extras;
                                buttonNode = m_Xdoc.CreateNode(XmlNodeType.Element, button.Name, null);
                                if (button.Tag is KeyValuePair<IEnumerable<int>, string> || button.Tag is KeyValuePair<Int32[], string> || button.Tag is KeyValuePair<UInt16[], string>)
                                {
                                    KeyValuePair<Int32[], string> tag = (KeyValuePair<Int32[], string>)button.Tag;
                                    int[] ii = tag.Key;
                                    buttonNode.InnerText = string.Join("/", ii);
                                    ShiftMacro.AppendChild(buttonNode);
                                    extras = tag.Value.Split(',');
                                }
                                else if (button.Tag is KeyValuePair<Int32, string> || button.Tag is KeyValuePair<UInt16, string> || button.Tag is KeyValuePair<byte, string>)
                                {
                                    KeyValuePair<int, string> tag = (KeyValuePair<int, string>)button.Tag;
                                    buttonNode.InnerText = tag.Key.ToString();
                                    ShiftKey.AppendChild(buttonNode);
                                    extras = tag.Value.Split(',');
                                }
                                else if (button.Tag is KeyValuePair<string, string>)
                                {
                                    KeyValuePair<string, string> tag = (KeyValuePair<string, string>)button.Tag;
                                    buttonNode.InnerText = tag.Key;
                                    ShiftButton.AppendChild(buttonNode);
                                    extras = tag.Value.Split(',');
                                }
                                else
                                {
                                    KeyValuePair<object, string> tag = (KeyValuePair<object, string>)button.Tag;
                                    extras = tag.Value.Split(',');
                                }
                                bool hasvalue = false;
                                foreach (string s in extras)
                                    if (s != "0")
                                    {
                                        hasvalue = true;
                                        break;
                                    }
                                if (hasvalue && !string.IsNullOrEmpty(String.Join(",", extras)))
                                {
                                    XmlNode extraNode = m_Xdoc.CreateNode(XmlNodeType.Element, button.Name, null);
                                    extraNode.InnerText = String.Join(",", extras);
                                    ShiftExtras.AppendChild(extraNode);
                                }
                            }
                        }
                        Node.AppendChild(NodeShiftControl);
                        if (ShiftButton.HasChildNodes)
                            NodeShiftControl.AppendChild(ShiftButton);
                        if (ShiftMacro.HasChildNodes)
                            NodeShiftControl.AppendChild(ShiftMacro);
                        if (ShiftKey.HasChildNodes)
                            NodeShiftControl.AppendChild(ShiftKey);
                        if (ShiftKeyType.HasChildNodes)
                            NodeShiftControl.AppendChild(ShiftKeyType);
                    }
                    else if (xmlShiftControls != null)
                        Node.AppendChild(xmlShiftControls);
                }
                m_Xdoc.AppendChild(Node);
                if (NodeControl.HasChildNodes)
                    Node.AppendChild(NodeControl);
                m_Xdoc.Save(path);
            }
            catch { Saved = false; }
            return Saved;
        }
        private MiControls getMiControlsByName(string key)
        {
            switch (key)
            {
                case "bnShare": return MiControls.Menu;
                case "bnL3": return MiControls.LS;
                case "bnR3": return MiControls.RS;
                case "bnOptions": return MiControls.Back;
                case "bnUp": return MiControls.DpadUp;
                case "bnRight": return MiControls.DpadRight;
                case "bnDown": return MiControls.DpadDown;
                case "bnLeft": return MiControls.DpadLeft;

                case "bnL1": return MiControls.L1;
                case "bnR1": return MiControls.R1;
                case "bnTriangle": return MiControls.Y;
                case "bnCircle": return MiControls.B;
                case "bnCross": return MiControls.A;
                case "bnSquare": return MiControls.X;

                case "bnPS": return MiControls.HomeSimulated;
                case "bnLSLeft": return MiControls.LXNeg;
                case "bnLSUp": return MiControls.LYNeg;
                case "bnRSLeft": return MiControls.RXNeg;
                case "bnRSUp": return MiControls.RYNeg;

                case "bnLSRight": return MiControls.LXPos;
                case "bnLSDown": return MiControls.LYPos;
                case "bnRSRight": return MiControls.RXPos;
                case "bnRSDown": return MiControls.RYPos;
                case "bnL2": return MiControls.LT;
                case "bnR2": return MiControls.RT;

                //case "bnTouchLeft": return MiControls.TouchLeft;
                //case "bnTouchMulti": return MiControls.TouchMulti;
                //case "bnTouchUpper": return MiControls.TouchUpper;
                //case "bnTouchRight": return MiControls.TouchRight;
                //case "bnGyroXP": return MiControls.GyroXPos;
                //case "bnGyroXN": return MiControls.GyroXNeg;
                //case "bnGyroZP": return MiControls.GyroZPos;
                //case "bnGyroZN": return MiControls.GyroZNeg;

                //case "bnSwipeUp": return MiControls.SwipeUp;
                //case "bnSwipeDown": return MiControls.SwipeDown;
                //case "bnSwipeLeft": return MiControls.SwipeLeft;
                //case "bnSwipeRight": return MiControls.SwipeRight;

                #region OldShiftname
                case "sbnShare": return MiControls.Menu;
                case "sbnL3": return MiControls.LS;
                case "sbnR3": return MiControls.RS;
                case "sbnOptions": return MiControls.Back;
                case "sbnUp": return MiControls.DpadUp;
                case "sbnRight": return MiControls.DpadRight;
                case "sbnDown": return MiControls.DpadDown;
                case "sbnLeft": return MiControls.DpadLeft;

                case "sbnL1": return MiControls.L1;
                case "sbnR1": return MiControls.R1;
                case "sbnTriangle": return MiControls.Y;
                case "sbnCircle": return MiControls.B;
                case "sbnCross": return MiControls.A;
                case "sbnSquare": return MiControls.X;

                case "sbnPS": return MiControls.HomeSimulated;
                case "sbnLSLeft": return MiControls.LXNeg;
                case "sbnLSUp": return MiControls.LYNeg;
                case "sbnRSLeft": return MiControls.RXNeg;
                case "sbnRSUp": return MiControls.RYNeg;

                case "sbnLSRight": return MiControls.LXPos;
                case "sbnLSDown": return MiControls.LYPos;
                case "sbnRSRight": return MiControls.RXPos;
                case "sbnRSDown": return MiControls.RYPos;
                case "sbnL2": return MiControls.LT;
                case "sbnR2": return MiControls.RT;

                //case "sbnTouchLeft": return MiControls.TouchLeft;
                //case "sbnTouchMulti": return MiControls.TouchMulti;
                //case "sbnTouchUpper": return MiControls.TouchUpper;
                //case "sbnTouchRight": return MiControls.TouchRight;
                //case "sbnGsyroXP": return MiControls.GyroXPos;
                //case "sbnGyroXN": return MiControls.GyroXNeg;
                //case "sbnGyroZP": return MiControls.GyroZPos;
                //case "sbnGyroZN": return MiControls.GyroZNeg;
                #endregion

                case "bnShiftShare": return MiControls.Menu;
                case "bnShiftL3": return MiControls.LS;
                case "bnShiftR3": return MiControls.RS;
                case "bnShiftOptions": return MiControls.Back;
                case "bnShiftUp": return MiControls.DpadUp;
                case "bnShiftRight": return MiControls.DpadRight;
                case "bnShiftDown": return MiControls.DpadDown;
                case "bnShiftLeft": return MiControls.DpadLeft;

                case "bnShiftL1": return MiControls.L1;
                case "bnShiftR1": return MiControls.R1;
                case "bnShiftTriangle": return MiControls.Y;
                case "bnShiftCircle": return MiControls.B;
                case "bnShiftCross": return MiControls.A;
                case "bnShiftSquare": return MiControls.X;

                case "bnShiftPS": return MiControls.HomeSimulated;
                case "bnShiftLSLeft": return MiControls.LXNeg;
                case "bnShiftLSUp": return MiControls.LYNeg;
                case "bnShiftRSLeft": return MiControls.RXNeg;
                case "bnShiftRSUp": return MiControls.RYNeg;

                case "bnShiftLSRight": return MiControls.LXPos;
                case "bnShiftLSDown": return MiControls.LYPos;
                case "bnShiftRSRight": return MiControls.RXPos;
                case "bnShiftRSDown": return MiControls.RYPos;
                case "bnShiftL2": return MiControls.LT;
                case "bnShiftR2": return MiControls.RT;

                //case "bnShiftTouchLeft": return MiControls.TouchLeft;
                //case "bnShiftTouchMulti": return MiControls.TouchMulti;
                //case "bnShiftTouchUpper": return MiControls.TouchUpper;
                //case "bnShiftTouchRight": return MiControls.TouchRight;
                //case "bnShiftGyroXP": return MiControls.GyroXPos;
                //case "bnShiftGyroXN": return MiControls.GyroXNeg;
                //case "bnShiftGyroZP": return MiControls.GyroZPos;
                //case "bnShiftGyroZN": return MiControls.GyroZNeg;

                //case "bnShiftSwipeUp": return MiControls.SwipeUp;
                //case "bnShiftSwipeDown": return MiControls.SwipeDown;
                //case "bnShiftSwipeLeft": return MiControls.SwipeLeft;
                //case "bnShiftSwipeRight": return MiControls.SwipeRight;
            }
            return 0;
        }

        private X360Controls getX360ControlsByName(string key)
        {
            switch (key)
            {
                case "Back": return X360Controls.Back;
                case "Left Stick": return X360Controls.LS;
                case "Right Stick": return X360Controls.RS;
                case "Start": return X360Controls.Start;
                case "Up Button": return X360Controls.DpadUp;
                case "Right Button": return X360Controls.DpadRight;
                case "Down Button": return X360Controls.DpadDown;
                case "Left Button": return X360Controls.DpadLeft;

                case "Left Bumper": return X360Controls.LB;
                case "Right Bumper": return X360Controls.RB;
                case "Y Button": return X360Controls.Y;
                case "B Button": return X360Controls.B;
                case "A Button": return X360Controls.A;
                case "X Button": return X360Controls.X;

                case "Guide": return X360Controls.Guide;
                case "Left X-Axis-": return X360Controls.LXNeg;
                case "Left Y-Axis-": return X360Controls.LYNeg;
                case "Right X-Axis-": return X360Controls.RXNeg;
                case "Right Y-Axis-": return X360Controls.RYNeg;

                case "Left X-Axis+": return X360Controls.LXPos;
                case "Left Y-Axis+": return X360Controls.LYPos;
                case "Right X-Axis+": return X360Controls.RXPos;
                case "Right Y-Axis+": return X360Controls.RYPos;
                case "Left Trigger": return X360Controls.LT;
                case "Right Trigger": return X360Controls.RT;

                case "Left Mouse Button": return X360Controls.LeftMouse;
                case "Right Mouse Button": return X360Controls.RightMouse;
                case "Middle Mouse Button": return X360Controls.MiddleMouse;
                case "4th Mouse Button": return X360Controls.FourthMouse;
                case "5th Mouse Button": return X360Controls.FifthMouse;
                case "Mouse Wheel Up": return X360Controls.WUP;
                case "Mouse Wheel Down": return X360Controls.WDOWN;
                case "Mouse Up": return X360Controls.MouseUp;
                case "Mouse Down": return X360Controls.MouseDown;
                case "Mouse Left": return X360Controls.MouseLeft;
                case "Mouse Right": return X360Controls.MouseRight;
                case "Unbound": return X360Controls.Unbound;

            }
            return X360Controls.Unbound;
        }

        public Boolean LoadProfile(int device, System.Windows.Forms.Control[] buttons, System.Windows.Forms.Control[] shiftbuttons, bool launchprogram, ControlService control, string propath = "")
        {
            Boolean Loaded = true;
            Dictionary<MiControls, MiKeyType> customMapKeyTypes = new Dictionary<MiControls, MiKeyType>();
            Dictionary<MiControls, UInt16> customMapKeys = new Dictionary<MiControls, UInt16>();
            Dictionary<MiControls, X360Controls> customMapButtons = new Dictionary<MiControls, X360Controls>();
            Dictionary<MiControls, String> customMapMacros = new Dictionary<MiControls, String>();
            Dictionary<MiControls, String> customMapExtras = new Dictionary<MiControls, String>();
            Dictionary<MiControls, MiKeyType> shiftCustomMapKeyTypes = new Dictionary<MiControls, MiKeyType>();
            Dictionary<MiControls, UInt16> shiftCustomMapKeys = new Dictionary<MiControls, UInt16>();
            Dictionary<MiControls, X360Controls> shiftCustomMapButtons = new Dictionary<MiControls, X360Controls>();
            Dictionary<MiControls, String> shiftCustomMapMacros = new Dictionary<MiControls, String>();
            Dictionary<MiControls, String> shiftCustomMapExtras = new Dictionary<MiControls, String>();
            string rootname = "MiWindows";
            Boolean missingSetting = false;
            string profilepath;
            if (propath == "")
                profilepath = Global.appdatapath + @"\Profiles\" + profilePath[device] + ".xml";
            else
                profilepath = propath;
            if (File.Exists(profilepath))
            {
                XmlNode Item;

                m_Xdoc.Load(profilepath);
                if (m_Xdoc.SelectSingleNode(rootname) == null)
                {
                    rootname = "ScpControl";
                    missingSetting = true;
                }
                //if (device < 4)
                //{
                //    MiLightBar.forcelight[device] = false;
                //    MiLightBar.forcedFlash[device] = 0;
                //}
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/flushHIDQueue"); Boolean.TryParse(Item.InnerText, out flushHIDQueue[device]); }
                catch { missingSetting = true; }//rootname = }

                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/idleDisconnectTimeout"); Int32.TryParse(Item.InnerText, out idleDisconnectTimeout[device]); }
                catch { missingSetting = true; }
                //New method for saving color
                try
                {
                    Item = m_Xdoc.SelectSingleNode("/" + rootname + "/Color");
                    string[] colors;
                    if (!string.IsNullOrEmpty(Item.InnerText))
                        colors = Item.InnerText.Split(',');
                    else
                        colors = new string[0];
                    m_Leds[device].red = byte.Parse(colors[0]);
                    m_Leds[device].green = byte.Parse(colors[1]);
                    m_Leds[device].blue = byte.Parse(colors[2]);
                }
                catch { missingSetting = true; }
                if (m_Xdoc.SelectSingleNode("/" + rootname + "/Color") == null)
                {
                    //Old method of color saving
                    try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/Red"); Byte.TryParse(Item.InnerText, out m_Leds[device].red); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/Green"); Byte.TryParse(Item.InnerText, out m_Leds[device].green); }
                    catch { missingSetting = true; }

                    try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/Blue"); Byte.TryParse(Item.InnerText, out m_Leds[device].blue); }
                    catch { missingSetting = true; }
                }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/RumbleBoost"); Byte.TryParse(Item.InnerText, out rumble[device]); }
                catch { missingSetting = true; }

                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ledAsBatteryIndicator"); Boolean.TryParse(Item.InnerText, out ledAsBattery[device]); }
                catch { missingSetting = true; }

                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/FlashType"); Byte.TryParse(Item.InnerText, out flashType[device]); }
                catch { missingSetting = true; }

                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/flashBatteryAt"); Int32.TryParse(Item.InnerText, out flashAt[device]); }
                catch { missingSetting = true; }

                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/touchSensitivity"); Byte.TryParse(Item.InnerText, out touchSensitivity[device]); }
                catch { missingSetting = true; }
                //New method for saving color
                try
                {
                    Item = m_Xdoc.SelectSingleNode("/" + rootname + "/LowColor");
                    string[] colors;
                    if (!string.IsNullOrEmpty(Item.InnerText))
                        colors = Item.InnerText.Split(',');
                    else
                        colors = new string[0];
                    m_LowLeds[device].red = byte.Parse(colors[0]);
                    m_LowLeds[device].green = byte.Parse(colors[1]);
                    m_LowLeds[device].blue = byte.Parse(colors[2]);
                }
                catch { missingSetting = true; }
                if (m_Xdoc.SelectSingleNode("/" + rootname + "/LowColor") == null)
                {
                    //Old method of color saving
                    try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/LowRed"); Byte.TryParse(Item.InnerText, out m_LowLeds[device].red); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/LowGreen"); Byte.TryParse(Item.InnerText, out m_LowLeds[device].green); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/LowBlue"); Byte.TryParse(Item.InnerText, out m_LowLeds[device].blue); }
                    catch { missingSetting = true; }
                }
                //New method for saving color
                try
                {
                    Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ChargingColor");
                    string[] colors;
                    if (!string.IsNullOrEmpty(Item.InnerText))
                        colors = Item.InnerText.Split(',');
                    else
                        colors = new string[0];

                    m_ChargingLeds[device].red = byte.Parse(colors[0]);
                    m_ChargingLeds[device].green = byte.Parse(colors[1]);
                    m_ChargingLeds[device].blue = byte.Parse(colors[2]);
                }
                catch { missingSetting = true; }
                if (m_Xdoc.SelectSingleNode("/" + rootname + "/ChargingColor") == null)
                {
                    try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ChargingRed"); Byte.TryParse(Item.InnerText, out m_ChargingLeds[device].red); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ChargingGreen"); Byte.TryParse(Item.InnerText, out m_ChargingLeds[device].green); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ChargingBlue"); Byte.TryParse(Item.InnerText, out m_ChargingLeds[device].blue); }
                    catch { missingSetting = true; }
                }
                try
                {
                    Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ShiftColor");
                    string[] colors;
                    if (!string.IsNullOrEmpty(Item.InnerText))
                        colors = Item.InnerText.Split(',');
                    else
                        colors = new string[0];
                    m_ShiftLeds[device].red = byte.Parse(colors[0]);
                    m_ShiftLeds[device].green = byte.Parse(colors[1]);
                    m_ShiftLeds[device].blue = byte.Parse(colors[2]);
                }
                catch { m_ShiftLeds[device] = m_Leds[device]; missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ShiftColorOn"); Boolean.TryParse(Item.InnerText, out shiftColorOn[device]); }
                catch { shiftColorOn[device] = false; missingSetting = true; }
                try
                {
                    Item = m_Xdoc.SelectSingleNode("/" + rootname + "/FlashColor");
                    string[] colors;
                    if (!string.IsNullOrEmpty(Item.InnerText))
                        colors = Item.InnerText.Split(',');
                    else
                        colors = new string[0];
                    m_FlashLeds[device].red = byte.Parse(colors[0]);
                    m_FlashLeds[device].green = byte.Parse(colors[1]);
                    m_FlashLeds[device].blue = byte.Parse(colors[2]);
                }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/touchpadJitterCompensation"); Boolean.TryParse(Item.InnerText, out touchpadJitterCompensation[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/lowerRCOn"); Boolean.TryParse(Item.InnerText, out lowerRCOn[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/tapSensitivity"); Byte.TryParse(Item.InnerText, out tapSensitivity[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/doubleTap"); Boolean.TryParse(Item.InnerText, out doubleTap[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/scrollSensitivity"); Int32.TryParse(Item.InnerText, out scrollSensitivity[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/LeftTriggerMiddle"); Byte.TryParse(Item.InnerText, out l2Deadzone[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/RightTriggerMiddle"); Byte.TryParse(Item.InnerText, out r2Deadzone[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ButtonMouseSensitivity"); Int32.TryParse(Item.InnerText, out buttonMouseSensitivity[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/Rainbow"); Double.TryParse(Item.InnerText, out rainbow[device]); }
                catch { rainbow[device] = 0; missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/LSDeadZone"); int.TryParse(Item.InnerText, out LSDeadzone[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/RSDeadZone"); int.TryParse(Item.InnerText, out RSDeadzone[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/SXDeadZone"); Double.TryParse(Item.InnerText, out SXDeadzone[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/SZDeadZone"); Double.TryParse(Item.InnerText, out SZDeadzone[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ChargingType"); Int32.TryParse(Item.InnerText, out chargingType[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/MouseAcceleration"); Boolean.TryParse(Item.InnerText, out mouseAccel[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ShiftModifier"); Int32.TryParse(Item.InnerText, out shiftModifier[device]); }
                catch { shiftModifier[device] = 0; missingSetting = true; }
                try
                {
                    Item = m_Xdoc.SelectSingleNode("/" + rootname + "/LaunchProgram");
                    launchProgram[device] = Item.InnerText;
                    if (launchprogram == true && launchProgram[device] != string.Empty) System.Diagnostics.Process.Start(launchProgram[device]);
                }
                catch { launchProgram[device] = string.Empty; missingSetting = true; }
                try
                {
                    Item = m_Xdoc.SelectSingleNode("/" + rootname + "/DinputOnly");
                    Boolean.TryParse(Item.InnerText, out dinputOnly[device]);
                    if (device < 4)
                    {
                        if (dinputOnly[device] == true) control.x360Bus.Unplug(device);
                        else if (control.MiControllers[device] != null && control.MiControllers[device].IsAlive()) control.x360Bus.Plugin(device);
                    }
                }
                catch { missingSetting = true; }
                try
                {
                    Item = m_Xdoc.SelectSingleNode("/" + rootname + "/StartTouchpadOff");
                    Boolean.TryParse(Item.InnerText, out startTouchpadOff[device]);
                    if (startTouchpadOff[device] == true) control.StartTPOff(device);
                }
                catch { startTouchpadOff[device] = false; missingSetting = true; }
                try
                { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/UseTPforControls"); Boolean.TryParse(Item.InnerText, out useTPforControls[device]); }
                catch { useTPforControls[device] = false; missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/LSCurve"); int.TryParse(Item.InnerText, out lsCurve[device]); }
                catch { missingSetting = true; }
                try { Item = m_Xdoc.SelectSingleNode("/" + rootname + "/RSCurve"); int.TryParse(Item.InnerText, out rsCurve[device]); }
                catch { missingSetting = true; }
                try
                {
                    Item = m_Xdoc.SelectSingleNode("/" + rootname + "/ProfileActions");
                    profileActions[device].Clear();
                    if (!string.IsNullOrEmpty(Item.InnerText))
                        profileActions[device].AddRange(Item.InnerText.Split('/'));
                }
                catch { profileActions[device].Clear(); missingSetting = true; }

                MiKeyType keyType;
                UInt16 wvk;
                if (buttons == null)
                {
                    XmlNode ParentItem = m_Xdoc.SelectSingleNode("/" + rootname + "/Control/Button");
                    if (ParentItem != null)
                        foreach (XmlNode item in ParentItem.ChildNodes)
                            customMapButtons.Add(getMiControlsByName(item.Name), getX360ControlsByName(item.InnerText));
                    ParentItem = m_Xdoc.SelectSingleNode("/" + rootname + "/Control/Macro");
                    if (ParentItem != null)
                        foreach (XmlNode item in ParentItem.ChildNodes)
                            customMapMacros.Add(getMiControlsByName(item.Name), item.InnerText);
                    ParentItem = m_Xdoc.SelectSingleNode("/" + rootname + "/Control/Key");
                    if (ParentItem != null)
                        foreach (XmlNode item in ParentItem.ChildNodes)
                            if (UInt16.TryParse(item.InnerText, out wvk))
                                customMapKeys.Add(getMiControlsByName(item.Name), wvk);
                    ParentItem = m_Xdoc.SelectSingleNode("/" + rootname + "/Control/Extras");
                    if (ParentItem != null)
                        foreach (XmlNode item in ParentItem.ChildNodes)
                            if (item.InnerText != string.Empty)
                                customMapExtras.Add(getMiControlsByName(item.Name), item.InnerText);
                            else
                                ParentItem.RemoveChild(item);
                    ParentItem = m_Xdoc.SelectSingleNode("/" + rootname + "/Control/KeyType");
                    if (ParentItem != null)
                        foreach (XmlNode item in ParentItem.ChildNodes)
                            if (item != null)
                            {
                                keyType = MiKeyType.None;
                                if (item.InnerText.Contains(MiKeyType.ScanCode.ToString()))
                                    keyType |= MiKeyType.ScanCode;
                                if (item.InnerText.Contains(MiKeyType.Toggle.ToString()))
                                    keyType |= MiKeyType.Toggle;
                                if (item.InnerText.Contains(MiKeyType.Macro.ToString()))
                                    keyType |= MiKeyType.Macro;
                                if (item.InnerText.Contains(MiKeyType.HoldMacro.ToString()))
                                    keyType |= MiKeyType.HoldMacro;
                                if (item.InnerText.Contains(MiKeyType.Unbound.ToString()))
                                    keyType |= MiKeyType.Unbound;
                                if (keyType != MiKeyType.None)
                                    customMapKeyTypes.Add(getMiControlsByName(item.Name), keyType);
                            }
                    if (shiftModifier[device] > 0)
                    {
                        ParentItem = m_Xdoc.SelectSingleNode("/" + rootname + "/ShiftControl/Button");
                        if (ParentItem != null)
                            foreach (XmlNode item in ParentItem.ChildNodes)
                                shiftCustomMapButtons.Add(getMiControlsByName(item.Name), getX360ControlsByName(item.InnerText));
                        ParentItem = m_Xdoc.SelectSingleNode("/" + rootname + "/ShiftControl/Macro");
                        if (ParentItem != null)
                            foreach (XmlNode item in ParentItem.ChildNodes)
                                shiftCustomMapMacros.Add(getMiControlsByName(item.Name), item.InnerText);
                        ParentItem = m_Xdoc.SelectSingleNode("/" + rootname + "/ShiftControl/Key");
                        if (ParentItem != null)
                            foreach (XmlNode item in ParentItem.ChildNodes)
                                if (UInt16.TryParse(item.InnerText, out wvk))
                                    shiftCustomMapKeys.Add(getMiControlsByName(item.Name), wvk);
                        ParentItem = m_Xdoc.SelectSingleNode("/" + rootname + "/ShiftControl/Extras");
                        if (ParentItem != null)
                            foreach (XmlNode item in ParentItem.ChildNodes)
                                shiftCustomMapExtras.Add(getMiControlsByName(item.Name), item.InnerText);
                        ParentItem = m_Xdoc.SelectSingleNode("/" + rootname + "/ShiftControl/KeyType");
                        if (ParentItem != null)
                            foreach (XmlNode item in ParentItem.ChildNodes)
                                if (item != null)
                                {
                                    keyType = MiKeyType.None;
                                    if (item.InnerText.Contains(MiKeyType.ScanCode.ToString()))
                                        keyType |= MiKeyType.ScanCode;
                                    if (item.InnerText.Contains(MiKeyType.Toggle.ToString()))
                                        keyType |= MiKeyType.Toggle;
                                    if (item.InnerText.Contains(MiKeyType.Macro.ToString()))
                                        keyType |= MiKeyType.Macro;
                                    if (item.InnerText.Contains(MiKeyType.HoldMacro.ToString()))
                                        keyType |= MiKeyType.HoldMacro;
                                    if (item.InnerText.Contains(MiKeyType.Unbound.ToString()))
                                        keyType |= MiKeyType.Unbound;
                                    if (keyType != MiKeyType.None)
                                        shiftCustomMapKeyTypes.Add(getMiControlsByName(item.Name), keyType);
                                }
                    }
                }
                else
                {
                    LoadButtons(buttons, "Control", customMapKeyTypes, customMapKeys, customMapButtons, customMapMacros, customMapExtras);
                    LoadButtons(shiftbuttons, "ShiftControl", shiftCustomMapKeyTypes, shiftCustomMapKeys, shiftCustomMapButtons, shiftCustomMapMacros, shiftCustomMapExtras);
                }
            }
            //catch { Loaded = false; }
            if (Loaded)
            {
                this.customMapButtons[device] = customMapButtons;
                this.customMapKeys[device] = customMapKeys;
                this.customMapKeyTypes[device] = customMapKeyTypes;
                this.customMapMacros[device] = customMapMacros;
                this.customMapExtras[device] = customMapExtras;

                this.shiftCustomMapButtons[device] = shiftCustomMapButtons;
                this.shiftCustomMapKeys[device] = shiftCustomMapKeys;
                this.shiftCustomMapKeyTypes[device] = shiftCustomMapKeyTypes;
                this.shiftCustomMapMacros[device] = shiftCustomMapMacros;
                this.shiftCustomMapExtras[device] = shiftCustomMapExtras;
            }
            // Only add missing settings if the actual load was graceful
            if (missingSetting && Loaded)// && buttons != null)
                SaveProfile(device, profilepath, buttons, shiftbuttons);

            return Loaded;
        }

        public void LoadButtons(System.Windows.Forms.Control[] buttons, string control, Dictionary<MiControls, MiKeyType> customMapKeyTypes,
           Dictionary<MiControls, UInt16> customMapKeys, Dictionary<MiControls, X360Controls> customMapButtons, Dictionary<MiControls, String> customMapMacros, Dictionary<MiControls, String> customMapExtras)
        {
            XmlNode Item;
            MiKeyType keyType;
            UInt16 wvk;
            string rootname = "MiWindows";
            foreach (var button in buttons)
                try
                {
                    if (m_Xdoc.SelectSingleNode(rootname) == null)
                    {
                        rootname = "ScpControl";
                    }
                    //bool foundBinding = false;
                    Item = m_Xdoc.SelectSingleNode(String.Format("/" + rootname + "/" + control + "/KeyType/{0}", button.Name));
                    if (Item != null)
                    {
                        //foundBinding = true;
                        keyType = MiKeyType.None;
                        if (Item.InnerText.Contains(MiKeyType.Unbound.ToString()))
                        {
                            keyType = MiKeyType.Unbound;
                            button.Tag = "Unbound";
                            button.Text = "Unbound";
                        }
                        else
                        {
                            bool SC = Item.InnerText.Contains(MiKeyType.ScanCode.ToString());
                            bool TG = Item.InnerText.Contains(MiKeyType.Toggle.ToString());
                            bool MC = Item.InnerText.Contains(MiKeyType.Macro.ToString());
                            bool MR = Item.InnerText.Contains(MiKeyType.HoldMacro.ToString());
                            button.Font = new Font(button.Font,
                                (SC ? FontStyle.Bold : FontStyle.Regular) | (TG ? FontStyle.Italic : FontStyle.Regular) |
                                (MC ? FontStyle.Underline : FontStyle.Regular) | (MR ? FontStyle.Strikeout : FontStyle.Regular));
                            if (Item.InnerText.Contains(MiKeyType.ScanCode.ToString()))
                                keyType |= MiKeyType.ScanCode;
                            if (Item.InnerText.Contains(MiKeyType.Toggle.ToString()))
                                keyType |= MiKeyType.Toggle;
                            if (Item.InnerText.Contains(MiKeyType.Macro.ToString()))
                                keyType |= MiKeyType.Macro;
                        }
                        if (keyType != MiKeyType.None)
                            customMapKeyTypes.Add(getMiControlsByName(Item.Name), keyType);
                    }
                    string extras;
                    Item = m_Xdoc.SelectSingleNode(String.Format("/" + rootname + "/" + control + "/Extras/{0}", button.Name));
                    if (Item != null)
                    {
                        if (Item.InnerText != string.Empty)
                        {
                            extras = Item.InnerText;
                            customMapExtras.Add(getMiControlsByName(button.Name), Item.InnerText);
                        }
                        else
                        {
                            m_Xdoc.RemoveChild(Item);
                            extras = "0,0,0,0,0,0,0,0";
                        }
                    }
                    else
                        extras = "0,0,0,0,0,0,0,0";
                    Item = m_Xdoc.SelectSingleNode(String.Format("/" + rootname + "/" + control + "/Macro/{0}", button.Name));
                    if (Item != null)
                    {
                        string[] splitter = Item.InnerText.Split('/');
                        int[] keys = new int[splitter.Length];
                        for (int i = 0; i < keys.Length; i++)
                        {
                            keys[i] = int.Parse(splitter[i]);
                            if (keys[i] < 255) splitter[i] = ((System.Windows.Forms.Keys)keys[i]).ToString();
                            else if (keys[i] == 256) splitter[i] = "Left Mouse Button";
                            else if (keys[i] == 257) splitter[i] = "Right Mouse Button";
                            else if (keys[i] == 258) splitter[i] = "Middle Mouse Button";
                            else if (keys[i] == 259) splitter[i] = "4th Mouse Button";
                            else if (keys[i] == 260) splitter[i] = "5th Mouse Button";
                            else if (keys[i] > 300) splitter[i] = "Wait " + (keys[i] - 300) + "ms";
                        }
                        button.Text = "Macro";
                        button.Tag = new KeyValuePair<int[], string>(keys, extras);
                        customMapMacros.Add(getMiControlsByName(button.Name), Item.InnerText);
                    }
                    else if (m_Xdoc.SelectSingleNode(String.Format("/" + rootname + "/" + control + "/Key/{0}", button.Name)) != null)
                    {
                        Item = m_Xdoc.SelectSingleNode(String.Format("/" + rootname + "/" + control + "/Key/{0}", button.Name));
                        if (UInt16.TryParse(Item.InnerText, out wvk))
                        {
                            //foundBinding = true;
                            customMapKeys.Add(getMiControlsByName(Item.Name), wvk);
                            button.Tag = new KeyValuePair<int, string>(wvk, extras);
                            button.Text = ((System.Windows.Forms.Keys)wvk).ToString();
                        }
                    }
                    else if (m_Xdoc.SelectSingleNode(String.Format("/" + rootname + "/" + control + "/Button/{0}", button.Name)) != null)
                    {
                        Item = m_Xdoc.SelectSingleNode(String.Format("/" + rootname + "/" + control + "/Button/{0}", button.Name));
                        //foundBinding = true;
                        button.Tag = new KeyValuePair<string, string>(Item.InnerText, extras);
                        button.Text = Item.InnerText;
                        customMapButtons.Add(getMiControlsByName(button.Name), getX360ControlsByName(Item.InnerText));
                    }
                    else
                    {
                        button.Tag = new KeyValuePair<object, string>(null, extras);
                    }
                }
                catch
                {

                }
        }
        public bool Load()
        {
            Boolean Loaded = true;
            Boolean missingSetting = false;

            try
            {
                if (File.Exists(m_Profile))
                {
                    XmlNode Item;

                    m_Xdoc.Load(m_Profile);

                    try { Item = m_Xdoc.SelectSingleNode("/Profile/useExclusiveMode"); Boolean.TryParse(Item.InnerText, out useExclusiveMode); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/startMinimized"); Boolean.TryParse(Item.InnerText, out startMinimized); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/formWidth"); Int32.TryParse(Item.InnerText, out formWidth); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/formHeight"); Int32.TryParse(Item.InnerText, out formHeight); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/Controller1"); profilePath[0] = Item.InnerText; }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/Controller2"); profilePath[1] = Item.InnerText; }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/Controller3"); profilePath[2] = Item.InnerText; }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/Controller4"); profilePath[3] = Item.InnerText; }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/LastChecked"); DateTime.TryParse(Item.InnerText, out lastChecked); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/CheckWhen"); Int32.TryParse(Item.InnerText, out CheckWhen); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/Notifications"); Boolean.TryParse(Item.InnerText, out notifications); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/DisconnectBTAtStop"); Boolean.TryParse(Item.InnerText, out disconnectBTAtStop); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/SwipeProfiles"); Boolean.TryParse(Item.InnerText, out swipeProfiles); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/UseMiForMapping"); Boolean.TryParse(Item.InnerText, out miMapping); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/QuickCharge"); Boolean.TryParse(Item.InnerText, out quickCharge); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/FirstXinputPort"); Int32.TryParse(Item.InnerText, out firstXinputPort); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/CloseMinimizes"); Boolean.TryParse(Item.InnerText, out closeMini); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/DownloadLang"); Boolean.TryParse(Item.InnerText, out downloadLang); }
                    catch { missingSetting = true; }
                    try { Item = m_Xdoc.SelectSingleNode("/Profile/FlashWhenLate"); Boolean.TryParse(Item.InnerText, out flashWhenLate); }
                    catch { missingSetting = true; }
                }
            }
            catch { }
            if (missingSetting)
                Save();
            return Loaded;
        }
        public bool Save()
        {
            Boolean Saved = true;

            XmlNode Node;

            m_Xdoc.RemoveAll();

            Node = m_Xdoc.CreateXmlDeclaration("1.0", "utf-8", String.Empty);
            m_Xdoc.AppendChild(Node);

            Node = m_Xdoc.CreateComment(String.Format(" Profile Configuration Data. {0} ", DateTime.Now));
            m_Xdoc.AppendChild(Node);

            Node = m_Xdoc.CreateWhitespace("\r\n");
            m_Xdoc.AppendChild(Node);

            Node = m_Xdoc.CreateNode(XmlNodeType.Element, "Profile", null);


            XmlNode xmlUseExclNode = m_Xdoc.CreateNode(XmlNodeType.Element, "useExclusiveMode", null); xmlUseExclNode.InnerText = useExclusiveMode.ToString(); Node.AppendChild(xmlUseExclNode);
            XmlNode xmlStartMinimized = m_Xdoc.CreateNode(XmlNodeType.Element, "startMinimized", null); xmlStartMinimized.InnerText = startMinimized.ToString(); Node.AppendChild(xmlStartMinimized);
            XmlNode xmlFormWidth = m_Xdoc.CreateNode(XmlNodeType.Element, "formWidth", null); xmlFormWidth.InnerText = formWidth.ToString(); Node.AppendChild(xmlFormWidth);
            XmlNode xmlFormHeight = m_Xdoc.CreateNode(XmlNodeType.Element, "formHeight", null); xmlFormHeight.InnerText = formHeight.ToString(); Node.AppendChild(xmlFormHeight);

            XmlNode xmlController1 = m_Xdoc.CreateNode(XmlNodeType.Element, "Controller1", null); xmlController1.InnerText = profilePath[0]; Node.AppendChild(xmlController1);
            XmlNode xmlController2 = m_Xdoc.CreateNode(XmlNodeType.Element, "Controller2", null); xmlController2.InnerText = profilePath[1]; Node.AppendChild(xmlController2);
            XmlNode xmlController3 = m_Xdoc.CreateNode(XmlNodeType.Element, "Controller3", null); xmlController3.InnerText = profilePath[2]; Node.AppendChild(xmlController3);
            XmlNode xmlController4 = m_Xdoc.CreateNode(XmlNodeType.Element, "Controller4", null); xmlController4.InnerText = profilePath[3]; Node.AppendChild(xmlController4);

            XmlNode xmlLastChecked = m_Xdoc.CreateNode(XmlNodeType.Element, "LastChecked", null); xmlLastChecked.InnerText = lastChecked.ToString(); Node.AppendChild(xmlLastChecked);
            XmlNode xmlCheckWhen = m_Xdoc.CreateNode(XmlNodeType.Element, "CheckWhen", null); xmlCheckWhen.InnerText = CheckWhen.ToString(); Node.AppendChild(xmlCheckWhen);
            XmlNode xmlNotifications = m_Xdoc.CreateNode(XmlNodeType.Element, "Notifications", null); xmlNotifications.InnerText = notifications.ToString(); Node.AppendChild(xmlNotifications);
            XmlNode xmlDisconnectBT = m_Xdoc.CreateNode(XmlNodeType.Element, "DisconnectBTAtStop", null); xmlDisconnectBT.InnerText = disconnectBTAtStop.ToString(); Node.AppendChild(xmlDisconnectBT);
            XmlNode xmlSwipeProfiles = m_Xdoc.CreateNode(XmlNodeType.Element, "SwipeProfiles", null); xmlSwipeProfiles.InnerText = swipeProfiles.ToString(); Node.AppendChild(xmlSwipeProfiles);
            XmlNode xmlMiMapping = m_Xdoc.CreateNode(XmlNodeType.Element, "UseMiForMapping", null); xmlMiMapping.InnerText = miMapping.ToString(); Node.AppendChild(xmlMiMapping);
            XmlNode xmlQuickCharge = m_Xdoc.CreateNode(XmlNodeType.Element, "QuickCharge", null); xmlQuickCharge.InnerText = quickCharge.ToString(); Node.AppendChild(xmlQuickCharge);
            XmlNode xmlFirstXinputPort = m_Xdoc.CreateNode(XmlNodeType.Element, "FirstXinputPort", null); xmlFirstXinputPort.InnerText = firstXinputPort.ToString(); Node.AppendChild(xmlFirstXinputPort);
            XmlNode xmlCloseMini = m_Xdoc.CreateNode(XmlNodeType.Element, "CloseMinimizes", null); xmlCloseMini.InnerText = closeMini.ToString(); Node.AppendChild(xmlCloseMini);
            XmlNode xmlDownloadLang = m_Xdoc.CreateNode(XmlNodeType.Element, "DownloadLang", null); xmlDownloadLang.InnerText = downloadLang.ToString(); Node.AppendChild(xmlDownloadLang);
            XmlNode xmlFlashWhenLate = m_Xdoc.CreateNode(XmlNodeType.Element, "FlashWhenLate", null); xmlFlashWhenLate.InnerText = flashWhenLate.ToString(); Node.AppendChild(xmlFlashWhenLate);

            m_Xdoc.AppendChild(Node);

            try { m_Xdoc.Save(m_Profile); }
            catch (UnauthorizedAccessException) { Saved = false; }
            return Saved;
        }



        private void CreateAction()
        {
            XmlDocument m_Xdoc = new XmlDocument();
            XmlNode Node;

            Node = m_Xdoc.CreateXmlDeclaration("1.0", "utf-8", String.Empty);
            m_Xdoc.AppendChild(Node);

            Node = m_Xdoc.CreateComment(String.Format(" Special Actions Configuration Data. {0} ", DateTime.Now));
            m_Xdoc.AppendChild(Node);

            Node = m_Xdoc.CreateWhitespace("\r\n");
            m_Xdoc.AppendChild(Node);

            Node = m_Xdoc.CreateNode(XmlNodeType.Element, "Actions", "");
            m_Xdoc.AppendChild(Node);
            m_Xdoc.Save(m_Actions);
        }

        public bool SaveAction(string name, string controls, int mode, string details, bool edit, string extras = "")
        {
            bool saved = true;
            if (!File.Exists(m_Actions))
                CreateAction();
            m_Xdoc.Load(m_Actions);
            XmlNode Node;

            Node = m_Xdoc.CreateComment(String.Format(" Special Actions Configuration Data. {0} ", DateTime.Now));
            foreach (XmlNode node in m_Xdoc.SelectNodes("//comment()"))
                node.ParentNode.ReplaceChild(Node, node);

            Node = m_Xdoc.SelectSingleNode("Actions");
            XmlElement el = m_Xdoc.CreateElement("Action");
            el.SetAttribute("Name", name);
            el.AppendChild(m_Xdoc.CreateElement("Trigger")).InnerText = controls;
            switch (mode)
            {
                case 1:
                    el.AppendChild(m_Xdoc.CreateElement("Type")).InnerText = "Macro";
                    el.AppendChild(m_Xdoc.CreateElement("Details")).InnerText = details;
                    if (extras != string.Empty)
                        el.AppendChild(m_Xdoc.CreateElement("Extras")).InnerText = extras;
                    break;
                case 2:
                    el.AppendChild(m_Xdoc.CreateElement("Type")).InnerText = "Program";
                    el.AppendChild(m_Xdoc.CreateElement("Details")).InnerText = details.Split('?')[0];
                    el.AppendChild(m_Xdoc.CreateElement("Arguements")).InnerText = extras;
                    el.AppendChild(m_Xdoc.CreateElement("Delay")).InnerText = details.Split('?')[1];
                    break;
                case 3:
                    el.AppendChild(m_Xdoc.CreateElement("Type")).InnerText = "Profile";
                    el.AppendChild(m_Xdoc.CreateElement("Details")).InnerText = details;
                    el.AppendChild(m_Xdoc.CreateElement("UnloadTrigger")).InnerText = extras;
                    break;
                case 4:
                    el.AppendChild(m_Xdoc.CreateElement("Type")).InnerText = "Key";
                    el.AppendChild(m_Xdoc.CreateElement("Details")).InnerText = details;
                    if (!String.IsNullOrEmpty(extras))
                    {
                        string[] exts = extras.Split('\n');
                        el.AppendChild(m_Xdoc.CreateElement("UnloadTrigger")).InnerText = exts[1];
                        el.AppendChild(m_Xdoc.CreateElement("UnloadStyle")).InnerText = exts[0];
                    }
                    break;
                case 5:
                    el.AppendChild(m_Xdoc.CreateElement("Type")).InnerText = "DisconnectBT";
                    el.AppendChild(m_Xdoc.CreateElement("Details")).InnerText = details;
                    break;
                case 6:
                    el.AppendChild(m_Xdoc.CreateElement("Type")).InnerText = "BatteryCheck";
                    el.AppendChild(m_Xdoc.CreateElement("Details")).InnerText = details;
                    break;
            }
            if (edit)
            {
                XmlNode oldxmlprocess = m_Xdoc.SelectSingleNode("/Actions/Action[@Name=\"" + name + "\"]");
                Node.ReplaceChild(el, oldxmlprocess);
            }
            else { Node.AppendChild(el); }
            m_Xdoc.AppendChild(Node);
            try { m_Xdoc.Save(m_Actions); }
            catch { saved = false; }
            LoadActions();
            return saved;
        }

        public void RemoveAction(string name)
        {
            m_Xdoc.Load(m_Actions);
            XmlNode Node = m_Xdoc.SelectSingleNode("Actions");
            XmlNode Item = m_Xdoc.SelectSingleNode("/Actions/Action[@Name=\"" + name + "\"]");
            if (Item != null)
                Node.RemoveChild(Item);
            m_Xdoc.AppendChild(Node);
            m_Xdoc.Save(m_Actions);
            LoadActions();
        }

        public bool LoadActions()
        {
            bool saved = true;
            if (!File.Exists(Global.appdatapath + "\\Actions.xml"))
            {
                SaveAction("Disconnect Controller", "PS/Options", 5, "0", false);
                saved = false;
            }
            try
            {
                actions.Clear();
                XmlDocument doc = new XmlDocument();
                doc.Load(Global.appdatapath + "\\Actions.xml");
                XmlNodeList actionslist = doc.SelectNodes("Actions/Action");
                string name, controls, type, details, extras, extras2;
                foreach (XmlNode x in actionslist)
                {
                    name = x.Attributes["Name"].Value;
                    controls = x.ChildNodes[0].InnerText;
                    type = x.ChildNodes[1].InnerText;
                    details = x.ChildNodes[2].InnerText;
                    if (type == "Profile")
                    {
                        extras = x.ChildNodes[3].InnerText;
                        actions.Add(new SpecialAction(name, controls, type, details, 0, extras));
                    }
                    else if (type == "Macro")
                    {
                        if (x.ChildNodes[3] != null) extras = x.ChildNodes[3].InnerText;
                        else extras = string.Empty;
                        actions.Add(new SpecialAction(name, controls, type, details, 0, extras));
                    }
                    else if (type == "Key")
                    {
                        if (x.ChildNodes[3] != null)
                        {
                            extras = x.ChildNodes[3].InnerText;
                            extras2 = x.ChildNodes[4].InnerText;
                        }
                        else
                        {
                            extras = string.Empty;
                            extras2 = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(extras))
                            actions.Add(new SpecialAction(name, controls, type, details, 0, extras2 + '\n' + extras));
                        else
                            actions.Add(new SpecialAction(name, controls, type, details));
                    }
                    else if (type == "DisconnectBT")
                    {
                        double doub;
                        if (double.TryParse(details, out doub))
                            actions.Add(new SpecialAction(name, controls, type, "", doub));
                        else
                            actions.Add(new SpecialAction(name, controls, type, ""));
                    }
                    else if (type == "BatteryCheck")
                    {
                        double doub;
                        if (double.TryParse(details.Split(',')[0], out doub))
                            actions.Add(new SpecialAction(name, controls, type, details, doub));
                        else
                            actions.Add(new SpecialAction(name, controls, type, details));
                    }
                    else if (type == "Program")
                    {
                        double doub;
                        if (x.ChildNodes[3] != null)
                        {
                            extras = x.ChildNodes[3].InnerText;
                            if (double.TryParse(x.ChildNodes[4].InnerText, out doub))
                                actions.Add(new SpecialAction(name, controls, type, details, doub, extras));
                            else
                                actions.Add(new SpecialAction(name, controls, type, details, 0, extras));
                        }
                        else
                        {
                            actions.Add(new SpecialAction(name, controls, type, details));
                        }
                    }
                }
            }
            catch { saved = false; }
            return saved;
        }
    }

    public class SpecialAction
    {
        public string name;
        public List<MiControls> trigger = new List<MiControls>();
        public string type;
        public string controls;
        public List<int> macro = new List<int>();
        public string details;
        public List<MiControls> uTrigger = new List<MiControls>();
        public string ucontrols;
        public double delayTime = 0;
        public string extra;
        public bool pressRelease = false;
        public MiKeyType keyType;
        public SpecialAction(string name, string controls, string type, string details, double delay = 0, string extras = "")
        {
            this.name = name;
            this.type = type;
            this.controls = controls;
            delayTime = delay;
            string[] ctrls = controls.Split('/');
            foreach (string s in ctrls)
                trigger.Add(getMiControlsByName(s));
            if (type == "Macro")
            {
                string[] macs = details.Split('/');
                foreach (string s in macs)
                {
                    int v;
                    if (int.TryParse(s, out v))
                        macro.Add(v);
                }
                if (extras.Contains("Scan Code"))
                    keyType |= MiKeyType.ScanCode;
            }
            else if (type == "Key")
            {
                this.details = details.Split(' ')[0];
                if (!string.IsNullOrEmpty(extras))
                {
                    string[] exts = extras.Split('\n');
                    pressRelease = exts[0] == "Release";
                    this.ucontrols = exts[1];
                    string[] uctrls = exts[1].Split('/');
                    foreach (string s in uctrls)
                        uTrigger.Add(getMiControlsByName(s));
                }
                if (details.Contains("Scan Code"))
                    keyType |= MiKeyType.ScanCode;
            }
            else if (type == "Program")
            {
                this.details = details;
                if (extras != string.Empty)
                    extra = extras;
            }
            else
                this.details = details;

            if (type != "Key" && !string.IsNullOrEmpty(extras))
            {
                this.ucontrols = extras;
                string[] uctrls = extras.Split('/');
                foreach (string s in uctrls)
                    uTrigger.Add(getMiControlsByName(s));
            }
        }

        private MiControls getMiControlsByName(string key)
        {
            switch (key)
            {
                case "Back": return MiControls.Back;
                case "LT": return MiControls.LT;
                case "RT": return MiControls.RT;
                case "Menu": return MiControls.Menu;
                case "Up": return MiControls.DpadUp;
                case "Right": return MiControls.DpadRight;
                case "Down": return MiControls.DpadDown;
                case "Left": return MiControls.DpadLeft;

                case "L1": return MiControls.L1;
                case "R1": return MiControls.R1;
                case "A": return MiControls.A;
                case "B": return MiControls.B;
                case "X": return MiControls.X;
                case "Y": return MiControls.Y;

                case "HomeSimulated": return MiControls.HomeSimulated;
                case "Left Stick Left": return MiControls.LXNeg;
                case "Left Stick Up": return MiControls.LYNeg;
                case "Right Stick Left": return MiControls.RXNeg;
                case "Right Stick Up": return MiControls.RYNeg;

                case "Left Stick Right": return MiControls.LXPos;
                case "Left Stick Down": return MiControls.LYPos;
                case "Right Stick Right": return MiControls.RXPos;
                case "Right Stick Down": return MiControls.RYPos;
                case "LS": return MiControls.LS;
                case "RS": return MiControls.RS;

                //case "Left Touch": return MiControls.TouchLeft;
                //case "Multitouch": return MiControls.TouchMulti;
                //case "Upper Touch": return MiControls.TouchUpper;
                //case "Right Touch": return MiControls.TouchRight;

                //case "Swipe Up": return MiControls.SwipeUp;
                //case "Swipe Down": return MiControls.SwipeDown;
                //case "Swipe Left": return MiControls.SwipeLeft;
                //case "Swipe Right": return MiControls.SwipeRight;

                //case "Tilt Up": return MiControls.GyroZNeg;
                //case "Tilt Down": return MiControls.GyroZPos;
                //case "Tilt Left": return MiControls.GyroXPos;
                //case "Tilt Right": return MiControls.GyroXNeg;
            }
            return 0;
        }
    }
}
