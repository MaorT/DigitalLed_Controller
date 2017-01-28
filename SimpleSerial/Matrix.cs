using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using System.Xml;

namespace SimpleSerial
{
    class Matrix
    {
        private int _numOfRows;
        private int _numOfColumns;
        private int _totalPixels;


        private char[] _sendBuffer;
        private char[] _stripBuffer;

        private Pixel[,] _seqTempMatrix;

      //  private enum EffectName { bar,barFill };

        public enum EffectName : int
        {
            bar,
            barFill,
            randomRect,
            dancingMan
 
        }

        private EffectName currentEffect = EffectName.barFill;

        private int sequenceEffectStep = 0;

        private SerialPort _serial;
        private Random rand;

        private int _currentBrightness;
        
        private bool serialReadyFlag = true;



        public Matrix(int numOfRows, int numOfColumns,SerialPort serialPort)
        {
            _numOfRows = numOfRows;
            _numOfColumns = numOfColumns;
            _totalPixels = numOfRows*numOfColumns;

            int numOfBytes = _totalPixels * 3; // todo: fix the last pixel

            _stripBuffer = new char[numOfBytes];
            _sendBuffer  = new char[numOfBytes];

            _seqTempMatrix = new Pixel[_numOfRows,_numOfColumns];

          //  _stripBuffer[numOfBytes - 1] = (char)1; // Set the 'end char' (data end symbol)

        //    _pixelsArray = new Pixel[_totalPixels];

            _serial = serialPort;
            //_endChar = new char[1];
            //_endChar[0] = (char) 1;

            rand = new Random();

            _currentBrightness = 20;
          

            CleanArr();

        }


        private void SetPixel(int x, int y,int red,int green,int blue)
        {
            int pixelNum = Pixel_Get_Index(x, y);


            //red = BrightConvertor(red);
            //green = BrightConvertor(green);
            //blue = BrightConvertor(blue);


            char redC = (char) red;
            char greenC = (char)green;
            char blueC = (char)blue;

            //if (redC == 1 || greenC == 1 || blueC == 1)
            //{
            //    int xx = 0;
            //}

            _stripBuffer[3 * pixelNum] = greenC;
            _stripBuffer[3 * pixelNum + 1] = redC;
            _stripBuffer[3 * pixelNum + 2] = blueC;


        }

        private void SetPixel(int x, int y, Color color)
        {
            int pixelNum = Pixel_Get_Index(x, y);

            char redC = (char)((color.R));
            char greenC = (char)(color.G);
            char blueC = (char)(color.B) ;

            //if (redC == 1 || greenC == 1 || blueC == 1)
            //{
            //    int xx = 0;
            //}

            _stripBuffer[3 * pixelNum] = redC;
            _stripBuffer[3 * pixelNum + 1] = greenC;
            _stripBuffer[3 * pixelNum + 2] = blueC;
        }


        

        




        private int Pixel_Get_Index(int x, int y)
        {
            // Convert an [X,Y] coordinates to the right pixel index,  [x=1,y=1] is the axis start point
            // Designed for horizontal lines order , strated from buttom right.
            // Example:                        [11][10][09][08]
            //                                 [07][06][05][04]
            //                                 [03][02][01][00]

            int result = -1;

            if (x < 0 || x >= _numOfColumns)
                ErrorNofity();
            else if (y < 0 || y >= _numOfRows)
                ErrorNofity();
            else
                result = (_numOfColumns - x - 1) + (y) * _numOfColumns;

            return result; 
        }


        public void Show(bool sequencedEffect)
        {
          int numOfBytes = _totalPixels * 3;


         //   char[] temp = new char[1];

            if (sequencedEffect)
                UpdateFromSequenceMatrix();
          
            for (int i = 0; i < numOfBytes; i++)
            {
                int currentVal = _stripBuffer[i];
                int val = (int)(currentVal * (1.0 * _currentBrightness / 100));
           //     temp[0] = (char) val;
                _sendBuffer[i] = (char)val;
               // _serial.Write(temp, 0, 1);
            //    Sleep(5);

            }

            ShowThread(_sendBuffer, 0, numOfBytes);

          //  _serial.Write(_stripBuffer, 0, numOfBytes);

          //  Sleep(15);
          ////  temp[0] = (char)1;
         //   _serial.Write(temp, 0, 1); 
        }



        void Sleep(int ms)
        {
            try
            {
                Thread.Sleep(ms);

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void ErrorNofity()
        {
            //todo: generate error
        }


        public void RandomSolidColor()
        {
            int red = RandomWithout1();
            int green = RandomWithout1();
            int blue = RandomWithout1();
            SolidColor(red, green, blue);
        }

        public void SolidColor(int red,int green,int blue)
        {
        //    CleanArr();
            for (int i = 0; i < _totalPixels * 3; i += 3)
            {
                _stripBuffer[i] = (char) green;
                _stripBuffer[i + 1] = (char)red;
                _stripBuffer[i + 2] = (char)blue;
            }
            Show(false);
        }

        //public void SolidColor(Color color)
        //{
        //    //for (int i = 0; i < _numOfColumns; i++)
        //    //    for (int j = 0; j < _numOfRows; j++)
        //    //    {
        //    //        SetPixel(i, j, color);
        //    //        if (i == 0 && j== 9)
        //    //            return;
        //    //    }

        //    CleanArr();

        //    for (int i = 0; i < _totalPixels*3; i+=3)
        //    {

        //        _stripBuffer[i] = (char)color.G;
        //        _stripBuffer[i + 1] = (char)color.R;
        //        _stripBuffer[i + 2] = (char)color.B;
        //    }


        //  //  SinglePixel(0, 9, (char)color.R, (char)color.B, (char)color.B,true);
                
                    
        //    Show();
        //}


        public void ClearStrip()
        {
            CleanArr();
            Show(false);
        }

        public void CleanArr() // todo: check it
        {
            for (int i = 0; i < _numOfColumns; i++)
                for (int j = 0; j < _numOfRows; j++)
                    SetPixel(i, j, 0, 0, 0);
        }


        private int RandomWithout1()
        {
            int temp = rand.Next(0, 256);

            while (temp == 1)
                temp = rand.Next(0, 256);
            return temp;
        }


        private Color GetRandomColor()
        {
            Random randomGen = new Random();
            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[randomGen.Next(names.Length)];
            Color randomColor = Color.FromKnownColor(randomColorName);
            return randomColor;
        }

        public void SetBrightness(int newBrightness)
        {
            if (newBrightness == _currentBrightness)
                return;

           _currentBrightness = newBrightness;

            Show(false);
        }

        private int BrightConvertorXX(int currentVal)
        {
            //return (int)(currentVal*1.0*_currentBrightness/100);
          //  int newVal = (currentVal * _currentBrightness) >> 8;
            int newVal = (int)(currentVal * 1.0 * _currentBrightness / 100);
            return newVal;
        }

        public void SinglePixel(int x, int y, int r, int g, int b, bool cleanBefore)
        {
            if(cleanBefore)
                CleanArr();
            SetPixel(x,y,r,g,b);
            Show(false);
        }


        public void Runner(int r, int g, int b, int delay)
        {
            CleanArr();

            for (int i = 0; i < _totalPixels*3; i += 3)
            {
                _stripBuffer[i] = (char) g;
                _stripBuffer[i + 1] = (char) r;
                _stripBuffer[i + 2] = (char) b;
                
                Sleep(delay);
                Show(false);

            }
        }

       public  void DrawRect(int x1,int y1,int x2,int y2,int red,int green,int blue,bool cleanBeforeDraw)
        {
          if(Pixel_Get_Index(x1,y1) == -1 || Pixel_Get_Index(x2,y2) == -1)
            return;
          if( x1 > x2  || y1 > y2)
            return;
    
          if(cleanBeforeDraw)
            CleanArr();
      
          for(int i = x1 ; i <= x2 ; i++)
            for ( int j = y1 ; j <= y2 ; j++)
              SetPixel(i,j,red,green,blue);

          Show(false);    

        }

       public void DrawRect_Random(int rectSize, int red, int green, int blue, bool cleanBeforeDraw)
       {
           int posX = rand.Next(1, _numOfColumns - rectSize);
           int posY = rand.Next(1, _numOfRows - rectSize);
           DrawRect(posX, posY, posX + rectSize, posY + rectSize, red,green,blue, cleanBeforeDraw);
           if(sequenceEffectStep++ >20)
               NextEffect();
       }



        public void ShowThread(char[] buff,int offset,int size)
        {
            if (!serialReadyFlag || !_serial.IsOpen)
                return;

                var thread = new Thread(() =>
                {
                    char[] temp = new char[1];
                    temp[0] = (char) 1;

                    //while (!serialReadyFlag)
                    //{
                    //    Thread.Sleep(2);
                    //}
                    serialReadyFlag = false;
                    if (_serial.IsOpen)
                    {
                        _serial.Write(buff, offset, size);
                        _serial.Write(temp, 0, 1);    
                    }


                    serialReadyFlag = true;
                });


            thread.Start();

        }


        public void SequenceEffectPlayer()
        {
            switch (currentEffect)
            {
                case EffectName.bar:
                    Bars_MiddleOut(false);
                    break;
                case EffectName.barFill:
                    Bars_MiddleOut(true);
                    break;
                case EffectName.randomRect:
                    DrawRect_Random(rand.Next(2, 4), RandomWithout1(), RandomWithout1(), RandomWithout1(),true);
                    break;
                case EffectName.dancingMan:
                    DancingMan(0,255,0);
                    break;

            }
        }


        public void Bars_MiddleOut(bool fill)
        {
             bool isEven = (_numOfColumns%2 == 0);

            int center = _numOfColumns/2;

            int lastStep = center-1;

            switch (sequenceEffectStep)
            {
                case 0:
                    DrawLine(center, 0, center, _numOfRows, 255, 0, 0, true);
                    if(isEven)
                        DrawLine(center-1, 0, center-1, _numOfRows, 255, 0, 0, false);

                    sequenceEffectStep++;
                    break;

                default:

                    int left = center - sequenceEffectStep;
                    if (isEven)
                        left--;
                    int right = center + sequenceEffectStep;

                    if(fill)
                        DrawLine(left, 0, left, _numOfRows, 255, 0, 0, false);
                    else
                        DrawLine(left, 0, left, _numOfRows, 255, 0, 0, true);

                    DrawLine(right, 0, right, _numOfRows, 255, 0, 0, false);

                    if (sequenceEffectStep == lastStep)
                    {
                        sequenceEffectStep = 0;
                        CleanArr();
                        NextEffect();
                    }              
                    else
                        sequenceEffectStep++;
                    break;
            }
            
            


        }




        private void UpdateFromSequenceMatrix()
        {
            for(int col= 0 ; col < _numOfColumns ; col++)
                for (int row = 0; row < _numOfRows; row++)
                {
                    Pixel pixel = _seqTempMatrix[row, col];
                    SetPixel(row,col,pixel.Red,pixel.Green,pixel.Blue);
                }
        }

        private void NextEffect()
        {
            

        //    Array values = Enum.GetValues(typeof(EffectName));
           
        //    int index = rand.Next(0, values.Length);

        //    int effectName = (int)values.GetValue(index);
        ////    currentEffect = EffectName.TryParse(index);


        //    Enum.GetName(EffectName, effectName);

        //    (EffectName)(rand.Next(0, Enum.GetNames(typeof(EffectName)).Length));

            var enumValues = Enum.GetValues(typeof(EffectName));
            var random = new Random();
            var randomValue = (EffectName)enumValues.GetValue(random.Next(enumValues.Length));

            currentEffect = randomValue;

            sequenceEffectStep = 0;

            // currentEffect = rand.Next(0,2)

        }


        public void DrawLine(int x1,int y1,int x2,int y2,int red,int green,int blue,bool cleanBefore)
        {
            double m = 0;

            if (cleanBefore)
                CleanArr();

            if (y2 != y1) // Vertical line
            {
                for (int y = y1; y < y2; y++)
                    SetPixel(x1, y, red, green, blue);
         
                    Show(false);  
            }
            else if (x2 != x1) // Horizontal line
            {
                for (int x = x1; x < x2; x++)
                    SetPixel(x, y1, red, green, blue);
              
                    Show(false);
            }


            //    m = 1.0* (x2 - x1)/(y2 - y1);

            //if (m != 0 && m != 1 && m != -1)
            //    return;

        }

        public void Flash(int r,int g,int b)
        {
            SolidColor(r,g,b);

         //   ClearStrip();

            //var thread = new Thread(() =>
            //{
            //    Thread.Sleep(10);
            //    ClearStrip();
            //});
            //thread.Start();
            
        }

        
        

        public void DrawIndexDemo()
        {
            int[] indexs = {1, 3, 5, 6, 7, 8};

            DrawFromIndexs(indexs,255,0,0,true);
        }


        public void DrawFromIndexs(int[] indexs,int r,int g,int b,bool cleanBeforeDraw)
        {
            if(cleanBeforeDraw)
                CleanArr();

            int size = indexs.Length;
            
            for (int i = 0; i < size; i ++)
            {
                int pixel = indexs[i]*3;
                _stripBuffer[pixel] = (char)g;
                _stripBuffer[pixel + 1] = (char)r;
                _stripBuffer[pixel + 2] = (char)b;
            }

            Show(false);
 

        }

        public void DancingMan2(int r, int g, int b)
        {

            
            switch (sequenceEffectStep)
            {
                case 0:
                    int[] values0 = { 9, 11, 23, 29, 31, 37, 43, 50, 57, 64, 70, 76, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 110, 129, 130, 131, 147, 148, 152, 153, 167, 169, 171, 173, 187, 188, 189, 190, 191, 192, 193 };
                    DrawFromIndexs(values0, r, g, b,true);
                    sequenceEffectStep++;
                    break;
                case 1:
                    int[] values1 = { 8, 12, 29, 31, 50, 70, 86, 87, 88, 89, 90, 91, 92, 93, 105, 110, 114, 121, 122, 123, 124, 129, 130, 131, 135, 136, 137, 138, 147, 148, 152, 153, 167, 169, 171, 173, 187, 188, 189, 190, 191, 192, 193 };
                    DrawFromIndexs(values1, r, g, b, true);
                    sequenceEffectStep++;
                    break;
                case 2:
                    int[] values2 = { 8, 12, 29, 31, 50, 70, 87, 88, 89, 90, 91, 92, 93, 106, 110, 114, 125, 129, 130, 131, 135, 144, 147, 148, 152, 153, 156, 161, 162, 163, 167, 169, 171, 173, 177, 178, 179, 187, 188, 189, 190, 191, 192, 193 };
                    DrawFromIndexs(values2, r, g, b, true);
                    sequenceEffectStep ++;
                    break;
                case 3:
                    int[] values3 = { 8, 12, 29, 31, 50, 70, 87, 88, 89, 90, 91, 92, 93, 106, 110, 114, 125, 129, 130, 131, 135, 144, 147, 148, 152, 153, 156, 163, 164, 165, 167, 169, 171, 173, 175, 176, 177, 187, 188, 189, 190, 191, 192, 193 };
                    DrawFromIndexs(values3, r, g, b, true);
                    sequenceEffectStep++;
                    break;
                case 4:
                    int[] values4 = { 8, 12, 29, 31, 50, 70, 87, 88, 89, 90, 91, 92, 93, 106, 110, 114, 125, 129, 130, 131, 135, 144, 147, 148, 152, 153, 156, 163, 167, 169, 171, 173, 177, 184, 185, 187, 188, 189, 190, 191, 192, 193, 195, 196 };
                    DrawFromIndexs(values4, r, g, b, true);
                    sequenceEffectStep++;
                    break;
                case 5:
                    int[] values5 = { 8, 12, 29, 31, 50, 70, 87, 88, 89, 90, 91, 92, 93, 102, 106, 110, 114, 122, 125, 129, 130, 131, 135, 143, 144, 147, 148, 152, 153, 156, 167, 169, 171, 173, 177, 187, 188, 189, 190, 191, 192, 193, 195, 196 };
                    DrawFromIndexs(values5, r, g, b, true);
                    sequenceEffectStep ++;
                    break;
                case 6:
                    int[] values6 = { 8, 12, 29, 31, 50, 70, 87, 88, 89, 90, 91, 92, 93, 106, 110, 114, 118, 125, 129, 130, 131, 135, 138, 144, 147, 148, 152, 153, 156, 157, 163, 167, 169, 171, 173, 184, 185, 187, 188, 189, 190, 191, 192, 193 };
                    DrawFromIndexs(values6, r, g, b, true);
                    sequenceEffectStep ++;
                    break;
                case 7:
                    int[] values7 = { 8, 12, 29, 31, 50, 70, 87, 88, 89, 90, 91, 92, 93, 102, 106, 110, 114, 118, 122, 125, 129, 130, 131, 135, 138, 143, 144, 147, 148, 152, 153, 156, 157, 167, 169, 171, 173, 187, 188, 189, 190, 191, 192, 193 };
                    DrawFromIndexs(values7, r, g, b, true);
                    sequenceEffectStep = 0;
                    break;
            }
        }


        public void DancingMan(int r, int g, int b)
        {
            int[][] effectMatrix2 = 
            {
                new int[] {8,12,29,31,47,50,53,68,70,72,89,90,91,110,129,130,131,147,148,152,153,167,169,171,173,187,188,189,190,191,192,193},
                new int[] {8,12,29,31,50,70,86,87,88,89,90,91,92,93,94,110,129,130,131,147,148,152,153,167,169,171,173,187,188,189,190,191,192,193},
                new int[] {8,12,29,31,50,70,88,89,90,91,92,107,110,113,126,129,130,131,134,147,148,152,153,167,169,171,173,187,188,189,190,191,192,193},
                new int[] {9,11,29,31,50,70,88,89,90,91,92,107,110,113,126,129,130,131,134,147,148,152,153,167,169,171,173,187,188,189,190,191,192,193},
                new int[] {8,11,29,31,50,54,70,73,88,89,90,91,92,107,110,126,129,130,131,147,148,152,153,167,169,171,173,187,188,189,190,191,192,193},
                new int[] {9,12,29,31,50,70,88,89,90,91,92,107,110,113,126,129,130,131,134,147,148,152,153,167,169,171,173,187,188,189,190,191,192,193},
                new int[] {8,12,29,31,50,51,70,72,88,89,90,91,92,93,107,110,126,129,130,131,147,148,152,153,167,169,173,187,188,189,190,191,192,193},
                new int[] {8,12,29,31,48,50,51,67,70,87,88,89,90,91,92,93,110,114,129,130,131,135,147,148,152,153,167,171,173,187,188,189,190,191,192,193}

            };

            int[][] effectMatrix1 = 
            {
                new int[] {9,11,29,31,50,70,87,88,89,90,91,92,93,107,110,113,126,127,129,130,131,133,134,148,152,167,169,171,173,188,189,190,191,192},
                new int[] {9,11,29,31,50,70,88,89,90,91,92,107,110,113,125,126,129,130,131,134,135,136,148,152,167,169,171,173,188,189,190,191,192},
                new int[] {9,11,29,31,50,70,88,89,90,91,92,107,110,113,126,129,130,131,134,145,148,152,155,165,167,169,171,173,175,188,189,190,191,192}
            };

            int[][] effectMatrix = 
            {
                new int[] {9,11,29,31,50,70,86,87,88,89,90,91,92,93,94,105,110,115,125,129,130,131,135,148,152,167,169,171,173,188,189,190,191,192},
                new int[] {9,11,29,31,50,70,87,88,89,90,91,92,93,106,110,114,125,129,130,131,135,145,148,152,155,167,169,171,173,188,189,190,191,192},
                new int[] {9,11,29,31,50,70,88,89,90,91,92,107,110,113,126,129,130,131,134,145,148,152,155,165,167,169,171,173,175,188,189,190,191,192},
                new int[] {9,11,29,31,50,70,88,89,90,91,92,105,106,107,110,113,114,115,125,129,130,131,135,145,148,152,155,167,169,171,173,188,189,190,191,192},
                new int[] {9,11,29,31,50,70,88,89,90,91,92,107,110,113,126,129,130,131,134,145,148,152,155,165,167,169,171,173,175,188,189,190,191,192},
                new int[] {9,11,29,31,50,70,88,89,90,91,92,105,106,107,110,113,114,115,125,129,130,131,135,145,148,152,155,167,169,171,173,188,189,190,191,192},
                new int[] {9,11,29,31,50,70,88,89,90,91,92,107,110,113,126,129,130,131,134,145,148,152,155,165,167,169,171,173,175,188,189,190,191,192}
            };


            DrawFromIndexs(effectMatrix[sequenceEffectStep++ % effectMatrix.Count()], r, g, b, true);

            int x = effectMatrix.Count();

            if (sequenceEffectStep == effectMatrix.Count()*20)
            {
                sequenceEffectStep = 0;
                NextEffect();
            }

            
        }




}










    
}
