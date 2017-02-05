using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using NAudio.Codecs;
using NAudio.CoreAudioApi;


namespace SimpleSerial
{
    public partial class Form1 : Form
    {
        // serial communication vars
        //string RxString;

        private int sleepBetweenCommands = 20; // Wait before next command to prevent arduino buffer overflow

        // Sound objects and vars
        private MMDeviceEnumerator de;
        private MMDevice mediaDevice ;
        int diff = 10, bigPeakVal = 15;
        private int maxPeak = 15; // Store the maximum peak for last X seconds (can be different for each song)
        private int peaksPerQuantom = 0;

        // effect configuration vars
        private int EFFECT_REPLAY_COUNT = 5; // how many times to repeat each normal effect (not for sequence effects).
        private int FLASH_MILLIS = 50;
        private int currentEffect = 0; // represent the current effect number.
        private int currentEffectStep = 0;

        private int numOfActivatedEffects ;


        // --- For window draging (from top panel) ---- //
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        // --- For window minimize (from taskbar) ---- //
        const int WS_MINIMIZEBOX = 0x20000;
        const int CS_DBLCLKS = 0x8;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }

        


 


        private Matrix _matrix;

      //  private bool runEffect = false;


        

        public Form1()
        {
            InitializeComponent();
            sensivityScrollBar.Value = 57;
            de = new MMDeviceEnumerator();
            mediaDevice = de.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            _matrix = new Matrix(10,20);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            DefaultSetup_Load();

            List<string> effectsList = _matrix.GetEffectsNamesList();
            numOfActivatedEffects = effectsList.Count;
            TableLayoutPanel table = new TableLayoutPanel();
            table.RowCount = 1;
            table.AutoSize = true;
            

            foreach (var effectName in effectsList)
            {
                CheckBox cBox = new CheckBox();
                cBox.Checked = true;
                cBox.CheckedChanged += delegate(object senderArg, EventArgs eArg)
                {

                    _matrix.SetEffectActiveState(cBox.Text, cBox.Checked);

                    if (cBox.Checked)
                        numOfActivatedEffects++;
                    else
                        numOfActivatedEffects--;

                    if (numOfActivatedEffects == 0)
                        MessageBox.Show("Can't disable all effect!\nThe last check effect will remain active until another one selected");

                };
                cBox.Text = effectName;
                table.Controls.Add(cBox);

            }


            panelEffectsPanel.Controls.Add(table);

            panelEffectsPanel.AutoScroll = true;

            // ComReceiveTxt.ScrollToCaret();
        }




        private void buttonStart_Click(object sender, EventArgs e)
        {
            StartConnection(); 
        }



        private void StartConnection()
        {
     

        }

        private void StopConnection()
        {
         
        }




        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
               _matrix.ClearStrip();
               _matrix.ClearStrip();
            }
            catch (Exception ex)
            {
                
                
            }
            
        }


        private void DefaultSetup_Load()
        {
            LoadConfiguration();
        }


        private void LoadConfiguration()
        {
            try
            {
                using (StreamReader file = new StreamReader("MT_Serial_Configuation.txt", Encoding.GetEncoding("windows-1255")))
                {
                    string line;

                    while ((line = file.ReadLine()) != null)
                    {
                        //if (line.Contains("Baud:"))
                        //    BaudComboSetup(line.Remove(0, 5));
                        //else if (line.Contains("Com:"))
                        //    ComComboSetup(line.Remove(0, 4));
                    }
                }
            }

            catch (Exception msg)
            {
                return;
            }

        }

        private void SaveConfiguration()
        {
            try
            {
                using (StreamWriter file = new StreamWriter("MT_Serial_Configuation.txt", false))
                {
                  //  file.WriteLine("Baud:"+BaudcomboBox.Text);
                  //  file.WriteLine("Com:" + ComPortsCombo.Text);
                }
            }

            catch (Exception msg)
            {
                return;
            }

        }


        private void musicTimer_Tick(object sender, EventArgs e)
        {

            float volumeLeft = (float)mediaDevice.AudioMeterInformation.PeakValues[0] * 100;
            float volumeRight = (float)mediaDevice.AudioMeterInformation.PeakValues[1] * 100;

            int mastervolume = (int)Math.Max(volumeRight, volumeLeft);

            if (mastervolume > maxPeak)
            {
                maxPeak = mastervolume;
                maxPeakLabel.Text = maxPeak.ToString();
            }
                

            musicProgBarLeft.Value = (int)volumeLeft;
            musicProgBarRight.Value = (int)volumeRight;

            if (!musicControllChkBox.Checked)
                return;

            MusicControll_Function(mastervolume, volumeLeft, volumeRight);
        }


        private void MusicControll_Function(float mastervolume, float volumeLeft, float volumeRight)
        {
            if (mastervolume < sensivityScrollBar.Value)
                return;

            peaksPerQuantom++;
            lblPeakPerQuantum.Text = peaksPerQuantom.ToString();

            if (volumeLeft - diff > volumeRight ) //left is louder
            {
                EffectsLeft();
            }
            else if (volumeRight - diff > volumeLeft) //right is louder
            {
                EffectsRight();
            }
            else if (peaksPerQuantom > 15)
            {
                Led_Effects_High_Peak();

            }
            else // normal effect
            {
                Led_Effects_RegualrBeat();         
            }
        }


        private void EffectsLeft()
        {
           // todo : add left effects
            TestFunction();


        }

        private void EffectsRight()
        {
            // todo : add right effects
            TestFunction();


        }

        private void Led_Effects_High_Peak()
        {
         //   ColorRefresh();
         //   //Quarter(myRandom(1, 4),true);
         //   Octet(myRandom(1,8),true);
         ////   serialPort1.Write("a");
            TestFunction();

          //  _matrix.Flash(0,0,255);

        }

        private void Led_Effects_RegualrBeat()
        {
            TestFunction();
        }







        private void sesivityScroll_Change(object sender, EventArgs e)
        {
            sensivityLabel.Text = sensivityScrollBar.Value.ToString();

        }

        private void saveSetupBtn_Click(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        private void customColorBtn_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            // Keeps the user from selecting a custom color.
            MyDialog.AllowFullOpen = true;
            // Allows the user to get help. (The default is false.)
            MyDialog.ShowHelp = true;
            // Sets the initial color select to the current text color.
            //MyDialog.Color = textBox1.ForeColor;
            Color c = Color.Red;
            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                c = MyDialog.Color;
                _matrix.SolidColor(c.R,c.G,c.B);
            }

        }


  

       
        //int MyRandom(int first, int last)
        //{
        //    Random rnd = new Random();
        //    int num = rnd.Next(first, last + 1);
        //    while (num == lastRandom)
        //        num = rnd.Next(first, last + 1);

        //    lastRandom = num;
        //    return num;
        //}

        //int MyRandom_Effect(int first, int last)
        //{
        //    Random rnd = new Random();
        //    int num = rnd.Next(first, last + 1);

        //    while (num == lastEffect)
        //        num = rnd.Next(first, last + 1);

        //    lastEffect = num;
        //    return num;
        //}

        private void autoSensivityTimer_Tick(object sender, EventArgs e)
        {

            if (!checkBoxAutoSensivity.Checked ||  maxPeak == 5)
                return;
            int newSensivity = maxPeak - 4;
            sensivityScrollBar.Value = newSensivity;
            sensivityLabel.Text = newSensivity.ToString();
            maxPeak = 5;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //serialPort1.Write
           // TestFunction();
          //  testStripWrite();

           _matrix.RandomSolidColor();
          // _matrix.Runner(0, 255, 120, 10);
        }

        //public void testStripWrite()
        //{
        //    int numOfPixel = 200;
        //    int bufferSize = 200;
        //    int sleepTime = 10;

        //    int numOfBytes = numOfPixel * 3 + 1;

        //    char[] fullBuff =  new char[numOfBytes];

        //    Random rand = new Random();
            
        
        //    for (int i =0 ; i< numOfBytes-1;  i++)
        //    {
        //        int num = rand.Next(3, 256);
        //        char ch = (char) num;
        //        fullBuff[i] = ch;
        //    }

        //    fullBuff[numOfBytes - 1] = (char) 1;

 

        //    int numOfPackets = numOfBytes/bufferSize;

        //    int ptr = 0;


        //    for (int i = 0; i < numOfPackets; i++)
        //    {
        //        serialPort1.Write(fullBuff, ptr,bufferSize);
        //        ptr += bufferSize;
        //     //   Sleep(sleepTime);       
        //    }

        //    serialPort1.Write(fullBuff, ptr, 1);


            
        //}


        private void Sleep(int sleepTime)
        {
            try
            {
                Thread.Sleep(sleepTime);

            }
            catch (Exception)
            {

                throw;
            }
        }



        //public void ColorRefresh()
        //{
        //    Random rand = new Random();
        //    currentRed = rand.Next(1, 256);
        //    currentGreen = rand.Next(1, 256);
        //    currentBlue = rand.Next(1, 256);
        //}




        //public void BarsFromMiddle(Boolean colorFull)
        //{
        //    if (colorFull)
        //        ColorRefresh();

        //    switch (currentEffectStep)
        //    {
        //        case 0:
        //            Octet(4, true);
        //            Thread.Sleep(sleepBetweenCommands);
        //            Octet(5, false);
        //            currentEffectStep++;
        //            break;
        //        case 1:
        //            Octet(6, false);
        //            Thread.Sleep(sleepBetweenCommands);
        //            Octet(3, false);
        //            currentEffectStep++;
        //            break;
        //        case 2:
        //            Octet(2, false);
        //            Thread.Sleep(sleepBetweenCommands);
        //            Octet(7, false);
        //            currentEffectStep++;
        //            break;
        //        case 3:
        //            Octet(1, false);
        //            Thread.Sleep(sleepBetweenCommands);
        //            Octet(8, false);
        //            currentEffectStep++;
        //            break;
        //        case 4:
        //            EndOfEffect();
        //            break;
        //    }
           
        //}


        //private void OctetTwoPlaces(int pos1, int pos2, bool cleanStrip)
        //{
        //    if(cleanStrip)
        //        ClearStrip();
        //    Octet(pos1, false);
        //    Thread.Sleep(sleepBetweenCommands);
        //    Octet(pos2, false);  
        //}


        //public void BarsMiddleFlash(Boolean colorFull)
        //{
        //    if (colorFull)
        //        ColorRefresh();

        //    switch (currentEffectStep)
        //    {
        //        case 0:
        //            OctetTwoPlaces(4, 5,true);
        //            currentEffectStep++;
        //            break;
        //        case 1:
        //            OctetTwoPlaces(3, 6,true);
        //            currentEffectStep++;
        //            break;
        //        case 2:
        //            OctetTwoPlaces(2, 7, true);
        //            currentEffectStep++;
        //            break;
        //        case 3:
        //            OctetTwoPlaces(1, 8, true);
        //            currentEffectStep++;
        //            break;
        //        case 4:
        //            OctetTwoPlaces(2, 7, true);
        //            currentEffectStep++;
        //            break;
        //        case 5:
        //            OctetTwoPlaces(3, 6, true);
        //            currentEffectStep++;
        //            break;
        //        case 6:             
        //            OctetTwoPlaces(4, 5, true);
        //            currentEffectStep++;
        //            break;
        //        case 7:
        //            EndOfEffect();
        //            break;
        //    }

        //    Thread.Sleep(sleepBetweenCommands);            
        //}


        //public void EndOfEffect() // called when the current effect finished
        //{
        //    ClearStrip();
        //    currentEffectStep = 0;
        //    currentEffect = 0;
            
        //}

        //public void SmartBarsFlash(Boolean returnToCenter, int divSize, Boolean colorFull)
        //{      
        //    if (colorFull)
        //        ColorRefresh();

        //    int right = divSize / 2 -  currentEffectStep;
        //    int left = divSize / 2 + 1 + currentEffectStep;

        //    if (currentEffectStep == divSize / 2)
        //    {
        //        EndOfEffect();
        //        return;   
        //    }

        //    if(currentEffectStep == 0)
        //        ClearStrip();

        //    SmartBar(divSize,left,true);
        //    Thread.Sleep(sleepBetweenCommands);
        //    SmartBar(divSize, right, false);
        //    Thread.Sleep(sleepBetweenCommands);

        //    currentEffectStep++;

        //}


        //public void SmartBar(int divSize, int position, Boolean clean)
        //{
        //    switch (divSize)
        //    {
        //        case 16:
        //            Nibble(position, clean);
        //            break;
        //        case 8:
        //            Octet(position,clean);
        //            break;
        //        case 4:
        //            Quarter(position,clean);
        //            break;
        //    }
        //}




        private void timerPeaksCounterReset_Tick(object sender, EventArgs e)
        {
            peaksPerQuantom = 0;
        }

        private void btnTest2_Click(object sender, EventArgs e)
        {
           // _matrix.SinglePixel(0, 9, 255, 0, 0, true);
            //_matrix.Runner(255, 0, 0,10);
          //  _matrix.DrawRect(0,0,5,5,255,0,120,true);
          //  _matrix.DrawRect_Random(4,255,0,0,true);
            _matrix.DancingMan(0,255,0);
        }

        private void hScrollBarBrgihtness_ValueChanged(object sender, EventArgs e)
        {
            labelBrightness.Text = hScrollBarBrgihtness.Value.ToString();
            _matrix.SetBrightness(int.Parse(labelBrightness.Text));

        }

        public void TestFunction() // todo: remove this function and add to the callers
        {
            //  BarsFromMiddle(true);
            //  BarsMiddleFlash(true);
            //    SmartBarsFlash(true, 16, true);
            //  ShowSelector();
            // _matrix.SinglePixel(0, 9, 255, 0, 0, true);
            //_matrix.Runner(255, 0, 0,10);
            //  _matrix.DrawRect(0,0,5,5,255,0,120,true);
            // _matrix.DrawRect_Random(4, 255, 0, 0, true);

           // _matrix.DancingMan(0,255,0);

            _matrix.SequenceEffectPlayer();
        }

        private void tempTimer_Tick(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress.Parse(txtDeviceIP.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter a valid IP");
                return;
            }
            try
            {

                _matrix.Connect(txtDeviceIP.Text);
                txtDeviceIP.Enabled = false;
                btnConnect.Enabled = false;
         

            }
            catch (Exception)
            {

                MessageBox.Show("Can't connect!\nPlease check the IP and try again");
            }

        }

        private void btnTurnOffLight_Click(object sender, EventArgs e)
        {
            _matrix.ClearStrip();
            _matrix.ClearStrip();
        }

        private void pictureBoxExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxMinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panelTopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

    }
}


