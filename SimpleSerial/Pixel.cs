using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSerial
{
    class Pixel
    {
        private int _red;

        public int Red
        {
            get { return _red; }
        }

        public int Green
        {
            get { return _green; }
        }

        public int Blue
        {
            get { return _blue; }
        }

        private int _green;
        private int _blue;

        public Pixel()
        {
            _red = 255;
            _blue = 255;
            _green = 255;
        }

        public void SetColor(int red, int green, int blue)
        {
            if (red > 255 || green > 255 || blue > 255)
                return;
            if (red < 0 || green < 0 || blue < 0)
                return;

            _red = red;
            _green = green;
            _blue = blue;
        }

        

    }
}
