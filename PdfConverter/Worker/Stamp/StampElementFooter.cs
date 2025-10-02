using PdfConverter.Worker.Stamp.Abstract;
using System;

namespace PdfConverter.Worker.Stamp
{
    public class StampElementFooter : StampElementText
    {
        public StampElementFooter(Tuple<float, float> basePosition, string txt) : base(basePosition, txt)
        {
        }

        public override Tuple<float, float> Position => new Tuple<float, float>(stamp_x, stamp_y);

    }
}