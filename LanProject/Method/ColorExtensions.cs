using System;
using System.Windows;
using System.Windows.Media;

namespace LanProject.Method
{
    public static class ColorExtensions
    {
        public static string ToHexColor()
        {
            var result = "#" + Guid.NewGuid().ToString().Substring(0, 6);
            return result;
        }
        public static LinearGradientBrush linearGradientForeground()
        {
            ColorConverter cc = new ColorConverter();
            LinearGradientBrush MetallicBlue = new LinearGradientBrush();
            GradientStopCollection BlueG = new GradientStopCollection();
            GradientStop BGS1 = new GradientStop();
            GradientStop BGS2 = new GradientStop();
            GradientStop BGS3 = new GradientStop();
            GradientStop BGS4 = new GradientStop();

            BGS1.Color = (Color)cc.ConvertFrom(ColorExtensions.ToHexColor());
            BGS1.Offset = 0.244;
            BGS2.Color = (Color)cc.ConvertFrom(ColorExtensions.ToHexColor());
            BGS2.Offset = 0.9;
            BGS3.Color = (Color)cc.ConvertFrom(ColorExtensions.ToHexColor());
            BGS3.Offset = 0.5;
            BGS4.Color = (Color)cc.ConvertFrom(ColorExtensions.ToHexColor());
            BGS4.Offset = 0.076;
            BlueG.Add(BGS1);
            BlueG.Add(BGS2);
            BlueG.Add(BGS3);
            BlueG.Add(BGS4);

            MetallicBlue.StartPoint = new Point(0.5, 0);
            MetallicBlue.EndPoint = new Point(0.5, 1);
            MetallicBlue.GradientStops = BlueG;
            return MetallicBlue;
        }
    }
}
