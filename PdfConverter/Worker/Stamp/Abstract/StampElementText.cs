using System;

namespace PdfConverter.Worker.Stamp.Abstract
{
    public abstract class StampElementText : StampElementAbstract, IDisposable
    {
        protected const float font_size = 6;
        protected float x = 0;
        protected float y = 0;
        private static string fontResource = "Data\\Roboto-Regular.ttf";
        private static string fontName = "Roboto";
        protected float stamp_x = 0;
        protected float stamp_y = 0;
        protected string text = string.Empty;
        private bool disposedValue;

        public static string FontResource { get => fontResource; }
        public static string FontName { get => fontName; }

        public string Text { get => text; }

        public StampElementText(Tuple<float, float> basePosition, string txt)
        {
            stamp_x = basePosition.Item1;
            stamp_y = basePosition.Item2;

            text = txt;
        }
        public override Tuple<float, float> Position => new Tuple<float, float>(stamp_x + x * scale_x, stamp_y + height_mm - y * scale_x);

        public static float FontSize => font_size;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StampElementText()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}