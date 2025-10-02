using System;

namespace PdfConverter.Worker.Stamp.Abstract
{
    public abstract class StampElementAbstract
    {
        //Настройки изоображения штампа:

        protected const float margin_right_mm = 10;
        protected const float margin_bottom_mm = 10;

        protected const float width_px = 611;
        protected const float height_px = 297;
        protected const float width_mm = 70;
        protected const float scale_x = width_mm / width_px;
        protected const float height_mm = height_px * scale_x;

        public abstract Tuple<float, float> Position { get; }


    }
}