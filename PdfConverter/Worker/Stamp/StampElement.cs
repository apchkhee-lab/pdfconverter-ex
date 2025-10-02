using PdfConverter.Worker.Stamp.Abstract;
using System;

namespace PdfConverter.Worker.Stamp
{
    public class StampElement : StampElementAbstract, IDisposable
    {
        private bool disposedValue;

        private static string pictureResource = @"Data\\stamp.png";

        protected float _pageWidth = 0;



        public StampElement(float pageWidth)
        {
            _pageWidth = pageWidth;
        }

        public override Tuple<float, float> Position => new Tuple<float, float>(_pageWidth - width_mm - margin_right_mm, margin_bottom_mm);

        public Tuple<float, float> Dimensions => new Tuple<float, float>(width_mm, height_mm);

        public static string PictureResource { get => pictureResource; }

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



        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}