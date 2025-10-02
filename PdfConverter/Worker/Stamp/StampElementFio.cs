using PdfConverter.Worker.Stamp.Abstract;
using System;

namespace PdfConverter.Worker.Stamp
{
    public class StampElementFio : StampElementText
    {
        public StampElementFio(Tuple<float, float> basePosition, string txt) : base(basePosition, txt)
        {
            x = 158;
            y = 197;
        }
    }
}