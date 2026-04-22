using System.Collections.Frozen;

namespace DenonRemote.Constants;

public record InputDefinition(string Label, string Sub, string Icon);

public enum SoundFamily { Stereo, Surround }

public record SoundModeDefinition(string Label, SoundFamily Family, string Description);

public static class Commands
{
    public static class Power
    {
        public const string On = "PWON";
        public const string Standby = "PWSTANDBY";
    }

    public static class Volume
    {
        public const string Up = "MVUP";
        public const string Down = "MVDOWN";

        public static string Set(int level) => $"MV{level:D2}";
    }

    public static class Mute
    {
        public const string On = "MUON";
        public const string Off = "MUOFF";
    }

    public static class Input
    {
        public const string Hdmi1 = "SIHDMI1";
        public const string Bd = "SIBD";
        public const string Dvd = "SIDVD";
        public const string Cd = "SICD";
        public const string Game = "SIGAME";
        public const string Tv = "SITV";
        public const string Bluetooth = "SIBT";
        public const string Usb = "SIUSB";

        public static string Select(string input) => $"{input}";

        public static readonly FrozenDictionary<string, InputDefinition> All =
            new Dictionary<string, InputDefinition>
            {
                [Hdmi1] = new("HDMI 1", "TV / Apple TV", "📺"),
                [Bd] = new("BD", "Blu-ray", "💿"),
                [Dvd] = new("DVD", "DVD Player", "📀"),
                [Cd] = new("CD", "CD Player", "🎶"),
                [Game] = new("GAME", "Console", "🎮"),
                [Tv] = new("TV", "Tuner", "📡"),
                [Bluetooth] = new("BT", "Bluetooth", "🔵"),
                [Usb] = new("USB", "Front USB", "💾"),
            }.ToFrozenDictionary();
    }

    public static class SoundMode
    {
        // Suffixes — the part after "MS" in Telnet commands and state strings.
        public const string Auto = "AUTO";
        public const string Stereo = "STEREO";
        public const string Direct = "DIRECT";
        public const string PureDirect = "PURE DIRECT";
        public const string DolbyDigital = "DOLBY DIGITAL";
        public const string DolbyAtmos = "DOLBY ATMOS";
        public const string DolbyTrueHD = "DOLBY TRUEHD";
        public const string DtsSurround = "DTS SURROUND";
        public const string DtsHdMstr = "DTS-HD MSTR";
        public const string DtsX = "DTS:X";
        public const string MultiChIn = "MULTI CH IN";

        public static readonly FrozenDictionary<string, SoundModeDefinition> All =
            new Dictionary<string, SoundModeDefinition>
            {
                [Stereo] = new("Stereo", SoundFamily.Stereo, "Two-channel downmix. Clean and direct, no surround processing."),
                [Direct] = new("Direct", SoundFamily.Stereo, "Stereo with optimised amp circuits. Source-faithful playback."),
                [PureDirect] = new("Pure Direct", SoundFamily.Stereo, "Shortest signal path; display powers down for maximum purity."),
                [DolbyDigital] = new("Dolby D", SoundFamily.Surround, "Classic Dolby 5.1 discrete surround. Most common for DVDs."),
                [DolbyAtmos] = new("Atmos", SoundFamily.Surround, "Object-based 3D audio with overhead height channels."),
                [DolbyTrueHD] = new("TrueHD", SoundFamily.Surround, "Lossless Dolby codec found on Blu-ray discs."),
                [DtsSurround] = new("DTS", SoundFamily.Surround, "Classic DTS 5.1 discrete surround, alternative to Dolby."),
                [DtsHdMstr] = new("DTS-HD", SoundFamily.Surround, "Lossless DTS Master Audio found on Blu-ray discs."),
                [DtsX] = new("DTS:X", SoundFamily.Surround, "Object-based DTS surround with height channel support."),
                [MultiChIn] = new("Multi-Ch", SoundFamily.Surround, "Passes multi-channel PCM input directly to the amplifier."),
            }.ToFrozenDictionary();
    }

    public static class Zone2
    {
        public const string On = "Z2ON";
        public const string Off = "Z2OFF";
        public const string Up = "Z2UP";
        public const string Down = "Z2DOWN";
        public static string Set(int level)         => $"Z2{level:D2}";
        public static string SetInput(string siKey) => "Z2" + siKey[2..]; // "SIBD" → "Z2BD"

        // HDMI inputs are not routable to Zone 2 on most AVRs.
        public static readonly FrozenSet<string> SafeInputs =
            new HashSet<string> { Input.Bd, Input.Dvd, Input.Cd, Input.Game, Input.Tv, Input.Bluetooth, Input.Usb }
            .ToFrozenSet();
    }
}
