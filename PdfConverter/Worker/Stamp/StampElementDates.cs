using PdfConverter.Worker.Stamp.Abstract;
using System;

namespace PdfConverter.Worker.Stamp
{
    public class StampElementDates : StampElementText
    {
        public StampElementDates(Tuple<float, float> basePosition, string txt) : base(basePosition, txt)
        {
            x = 195;
            y = 222;
        }
    }
}