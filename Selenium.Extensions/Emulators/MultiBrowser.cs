using System;
using System.Collections.Generic;

namespace Selenium.Extensions.Emulators
{
    public class MultiBrowser
    {
        public static DeviceDetails GetMultiBrowserEmulators(Emulator emulator)
        {
            switch (emulator)
            {
                case Emulator.iPhone4:
                    var appleiPhone4 = new DeviceDetails
                    {
                        DeviceName = "Apple iPhone 4",
                        DeviceWidth = 320,
                        DeviceHeight = 480,
                        DevicePixelRatio = 2,
                        DeviceUserAgent = "Mozilla/5.0 (iPhone; U; CPU iPhone OS 4_2_1 like Mac OS X; en-us) AppleWebKit/533.17.9 (KHTML, like Gecko) Version/5.0.2 Mobile/8C148 Safari/6533.18.5",
                        EmulatorArgument = "iphone4"
                    };
                    return appleiPhone4;
                case Emulator.iPhone5:
                    var appleiPhone5 = new DeviceDetails
                    {
                        DeviceName = "Apple iPhone 5",
                        DeviceWidth = 320,
                        DeviceHeight = 568,
                        DevicePixelRatio = 2,
                        DeviceUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 7_0 like Mac OS X; en-us) AppleWebKit/537.51.1 (KHTML, like Gecko) Version/7.0 Mobile/11A465 Safari/9537.53",
                        EmulatorArgument = "iphone5"
                    };
                    return appleiPhone5;
                case Emulator.iPhone6:
                    var appleiPhone6 = new DeviceDetails
                    {
                        DeviceName = "Apple iPhone 6",
                        DeviceWidth = 375,
                        DeviceHeight = 667,
                        DevicePixelRatio = 2,
                        DeviceUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4",
                        EmulatorArgument = "iphone6"
                    };
                    return appleiPhone6;
                case Emulator.iPhone6Plus:
                    var appleiPhone6Plus = new DeviceDetails
                    {
                        DeviceName = "Apple iPhone 6 Plus",
                        DeviceWidth = 414,
                        DeviceHeight = 736,
                        DevicePixelRatio = 3,
                        DeviceUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4",
                        EmulatorArgument = "iphone6plus"
                    };
                    return appleiPhone6Plus;
                case Emulator.iPad:
                    var appleiPad = new DeviceDetails
                    {
                        DeviceName = "Apple iPad",
                        DeviceWidth = 1024,
                        DeviceHeight = 768,
                        DevicePixelRatio = 2,
                        DeviceUserAgent = "Mozilla/5.0 (iPad; CPU OS 7_0 like Mac OS X) AppleWebKit/537.51.1 (KHTML, like Gecko) Version/7.0 Mobile/11A465 Safari/9537.53",
                        EmulatorArgument = "ipad"
                    };
                    return appleiPad;
                case Emulator.SamsungGalaxyTab:
                    var samsungGalaxyTab = new DeviceDetails
                    {
                        DeviceName = "Samsung Galaxy Tab",
                        DeviceWidth = 800,
                        DeviceHeight = 1280,
                        DevicePixelRatio = 1,
                        DeviceUserAgent = "Mozilla/5.0 (Linux; Android 4.4.2; SM-T232 Build/KOT49H) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.517 Safari/537.36",
                        EmulatorArgument = "samsunggalaxytab"
                    };
                    return samsungGalaxyTab;
                case Emulator.SamsungGalaxyS5:
                    var samsungGalaxyS5 = new DeviceDetails
                    {
                        DeviceName = "Samsung Galaxy S5",
                        DeviceWidth = 360,
                        DeviceHeight = 640,
                        DevicePixelRatio = 3,
                        DeviceUserAgent = "Mozilla/5.0 (Linux; Android 4.4.2; GT-I9505 Build/JDQ39) AppleWebKit/537.36 (KHTML, like Gecko) Version/1.5 Chrome/28.0.1500.94 Mobile Safari/537.36",
                        EmulatorArgument = "samsunggalaxys5"
                    };
                    return samsungGalaxyS5;
                case Emulator.SamsungGalaxyS6:
                    var samsungGalaxyS6 = new DeviceDetails
                    {
                        DeviceName = "Samsung Galaxy S6",
                        DeviceWidth = 360,
                        DeviceHeight = 640,
                        DevicePixelRatio = 4,
                        DeviceUserAgent = "Mozilla/5.0 (Linux; Android 5.0.2; SM-G920I Build/LRX22G) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.111 Mobile Safari/537.36",
                        EmulatorArgument = "samsunggalaxys6"
                    };
                    return samsungGalaxyS6;
                case Emulator.SonyXperiaZ3:
                    var sonyXperiaZ3 = new DeviceDetails
                    {
                        DeviceName = "Sony Xperia Z3",
                        DeviceWidth = 360,
                        DeviceHeight = 640,
                        DevicePixelRatio = 3,
                        DeviceUserAgent = "Mozilla/5.0 (Linux; Android 4.4.4; D5833 Build/23.0.1.A.5.77) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.109 Mobile Safari/537.36",
                        EmulatorArgument = "sonyxperiaz3"
                    };
                    return sonyXperiaZ3;
            }
            return null;
        }

        public class DeviceDetails
        {
            public string DeviceName { get; set; }
            public int DeviceWidth { get; set; }
            public int DeviceHeight { get; set; }
            public double DevicePixelRatio { get; set; }
            public string DeviceUserAgent { get; set; }
            public string EmulatorArgument { get; set; }
        }
    }
}
