using PdfConverter.Worker.Stamp.Abstract;
using System;

namespace PdfConverter.Worker.Stamp
{
    public class StampElementCert : StampElementText
    {
        public StampElementCert(Tuple<float, float> basePosition, string txt) : base(basePosition, txt)
        {
            x = 179;
            y = 171;
        }
    }
}