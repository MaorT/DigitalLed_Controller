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


        private byte[] _sendBuffer;
        private byte[] _stripBuffer;

        private Pixel[,] _seqTempMatrix;

        private List<string> activeEffectsList;

      //  private enum EffectName { bar,barFill };

        public enum EffectName : int
        {
            bar,
            barFill,
            randomRect,
            flash,
            EatSleepRaveRepeat,
            DancingMan,
            Smiley
            
 
        }

        private EffectName currentEffect = EffectName.barFill;

        private int sequenceEffectStep = 0;

    //    private SerialPort _serial;

        private ArtNetEngine artNetEngine;

        private Random rand;

        private int _currentBrightness;
        
        private bool serialReadyFlag = true;

        private bool timedEffectOnProgress = false;



        public Matrix(int numOfRows, int numOfColumns)
        {
            _numOfRows = numOfRows;
            _numOfColumns = numOfColumns;
            _totalPixels = numOfRows*numOfColumns;

            int numOfBytes = _totalPixels * 3; // todo: fix the last pixel

            _stripBuffer = new byte[numOfBytes];
            _sendBuffer = new byte[numOfBytes];

            _seqTempMatrix = new Pixel[_numOfRows,_numOfColumns];

            activeEffectsList = GetEffectsNamesList();

            

          //  _stripBuffer[numOfBytes - 1] = (char)1; // Set the 'end char' (data end symbol)

        //    _pixelsArray = new Pixel[_totalPixels];

           // _serial = serialPort;



            //_endChar = new char[1];
            //_endChar[0] = (char) 1;

            rand = new Random();

            _currentBrightness = 20;
          

            CleanArr();

        }

        public List<string> GetEffectsNamesList()
        {
            List<string> list = new List<string>();

            foreach (var effect in Enum.GetValues(typeof(EffectName)))
            {
                list.Add(effect.ToString());   
            }
            return list;

        }

        public void SetEffectActiveState(string effectName, bool isActive)
        {
            if(isActive)
                activeEffectsList.Add(effectName);
            else
                activeEffectsList.Remove(effectName); 
        }


        public void Connect(string ipAddress)
        {
            artNetEngine = new ArtNetEngine("MyArtNet", ipAddress);
            artNetEngine.Start();  
        }

        private void SetPixel(int x, int y,int red,int green,int blue)
        {
            int pixelNum = Pixel_Get_Index(x, y);


            //red = BrightConvertor(red);
            //green = BrightConvertor(green);
            //blue = BrightConvertor(blue);


            byte redC = (byte) red;
            byte greenC = (byte)green;
            byte blueC = (byte)blue;

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

            byte redC = (byte)((color.R));
            byte greenC = (byte)(color.G);
            byte blueC = (byte)(color.B);

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
                _sendBuffer[i] = (byte)val;
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
                _stripBuffer[i] = (byte)green;
                _stripBuffer[i + 1] = (byte)red;
                _stripBuffer[i + 2] = (byte)blue;
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
                _stripBuffer[i] = (byte)g;
                _stripBuffer[i + 1] = (byte)r;
                _stripBuffer[i + 2] = (byte)b;
                
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



        public void ShowThread(byte[] buff,int offset,int size)
        {
            if (!serialReadyFlag || artNetEngine == null)
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

                 //   artNetEngine.SendDMX(buff, offset, size);
                    artNetEngine.SendDMX(0, buff, _totalPixels*3);
                   // _serial.Write(temp, 0, 1);    
               


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
                case EffectName.flash:
                    Flash(0,0,255);
                    break;
                case EffectName.DancingMan:
                    DancingMan_RandomColor();
                    break;
                case EffectName.EatSleepRaveRepeat:
                    EatSleepRaveRepeat();
                    break;
                case EffectName.Smiley:
                    Smiley();
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
            string effectName = "";

            if (activeEffectsList.Count == 0) return;

            do
            {
                var randomValue = (EffectName) enumValues.GetValue(random.Next(enumValues.Length));
                currentEffect = randomValue;
                effectName = randomValue.ToString();

            } while (!activeEffectsList.Contains(effectName));

            

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

            if(sequenceEffectStep++ > 5)
                NextEffect();

         //   ClearStrip();

            var thread = new Thread(() =>
            {
                Thread.Sleep(10);
                ClearStrip();
            });
            thread.Start();
            
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
                _stripBuffer[pixel] = (byte)g;
                _stripBuffer[pixel + 1] = (byte)r;
                _stripBuffer[pixel + 2] = (byte)b;
            }

            Show(false);
 

        }

        public void DrawFromIndexs_RandomColor(int[] indexs, bool cleanBeforeDraw)
        {
            if (cleanBeforeDraw)
                CleanArr();

            int size = indexs.Length;

            int r = RandomWithout1();
            int g = RandomWithout1();
            int b = RandomWithout1();

            for (int i = 0; i < size; i++)
            {
                int pixel = indexs[i] * 3;
                _stripBuffer[pixel] = (byte)g;
                _stripBuffer[pixel + 1] = (byte)r;
                _stripBuffer[pixel + 2] = (byte)b;
            }

            Show(false);


        }

     

        public void DancingMan(int r, int g, int b)
        {
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

            WaitBeforeNextShow(30);

            if (sequenceEffectStep == effectMatrix.Count()*5)
            {
                sequenceEffectStep = 0;
                NextEffect();
            }

            
        }


        public void DancingMan_RandomColor()
        {
            int r = RandomWithout1();
            int g = RandomWithout1();
            int b = RandomWithout1();
            DancingMan(r,g,b);
            
        }


        //public void EatSleepRaveRepeat_Timed()
        //{
        //    if (timedEffectOnProgress)
        //        return;

        //    timedEffectOnProgress = true;

        //    TimedEffectFunction(EffectName.EatSleepRaveRepeat, 250);
        //}



        public void EatSleepRaveRepeat()
        {
            if (!serialReadyFlag)
                return;
 
            int[][] effectMatrix = 
            {
                new int[] {24,28,32,34,35,36,37,44,48,52,57,64,68,72,77,84,88,92,97,104,108,109,110,111,112,114,115,116,117,124,128,132,137,144,148,152,157,162,163,164,165,166,168,169,170,171,172,174,175,176,177},
                new int[] {23,25,26,27,29,30,31,33,34,35,37,38,39,43,47,51,55,57,63,67,71,75,77,83,87,91,95,97,100,101,102,103,105,106,107,109,110,111,115,117,118,119,120,123,127,131,135,139,140,143,147,151,155,159,160,161,162,163,165,166,167,169,170,171,175,177,178,179},
                new int[] {20,21,22,26,30,33,35,39,42,45,47,50,53,56,59,62,64,68,70,73,77,79,82,84,88,90,93,98,99,100,101,102,104,108,110,111,112,113,116,117,118,119,122,124,128,130,133,135,139,142,144,148,150,153,155,159,160,161,162,164,168,170,171,172,173,176,177,178,179},
                new int[] {21,24,26,28,29,32,34,35,37,39,41,44,46,49,52,55,58,59,61,64,66,69,72,75,79,81,84,86,89,92,95,99,101,104,105,106,108,109,112,114,115,118,119,121,124,126,129,131,132,135,137,139,141,144,146,149,150,152,155,157,159,160,161,162,164,165,166,168,169,170,171,172,174,175,178,179}
            };

            DrawFromIndexs_RandomColor(effectMatrix[sequenceEffectStep++ % effectMatrix.Count()], true);

            
            WaitBeforeNextShow(20);
            
            if (sequenceEffectStep == effectMatrix.Count() * 2)
            {
                sequenceEffectStep = 0;
                NextEffect();
            }

        }


        public void Smiley()
        {
            if (!serialReadyFlag)
                return;

            int[][] effectMatrix = 
            {
                new int[] {8,9,10,11,12,27,33,44,45,46,47,48,49,50,51,52,53,54,55,56,90,105,106,107,110,113,114,115,124,128,132,136,143,146,149,151,154,157,164,168,172,176,185,186,187,193,194,195},

            };

            DrawFromIndexs_RandomColor(effectMatrix[sequenceEffectStep++ % effectMatrix.Count()], true);


            WaitBeforeNextShow(100);

            if (sequenceEffectStep == effectMatrix.Count() * 2)
            {
                sequenceEffectStep = 0;
                NextEffect();
            }

        }


        



        private void TimedEffectFunction(EffectName effectName,int speedMs)
        {
            int x = 5;
        

            var thread = new Thread(() =>
            {
                switch (effectName)
                {
                    case EffectName.EatSleepRaveRepeat:
                        while (currentEffect == EffectName.EatSleepRaveRepeat)
                        {
                            EatSleepRaveRepeat();
                            Thread.Sleep(speedMs);
                        }

                        break;
                }

                timedEffectOnProgress = false;

            });

            
            thread.Start();
        }



        public void WaitBeforeNextShow(int ms)
        {
            Thread.Sleep(10);

            serialReadyFlag = false;

            var thread = new Thread(() =>
            {
                Thread.Sleep(ms);
                serialReadyFlag = true;
            });
            thread.Start();

        }

}










    
}
