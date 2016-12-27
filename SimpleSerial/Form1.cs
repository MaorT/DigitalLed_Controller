using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using NAudio.CoreAudioApi;


namespace SimpleSerial
{
    public partial class Form1 : Form
    {
        // serial communication vars
        string RxString;
        private char endChar = '~';
        private string CurrentCom = "";
        private string BaudRate = "9600";
        private int sleepBetweenCommands = 20; // Wait before next command to prevent arduino buffer overflow

        // Sound objects and vars
        private MMDeviceEnumerator de;
        private MMDevice mediaDevice ;
        int diff = 10, bigPeakVal = 15;
        private int maxPeak = 15; // Store the maximum peak for last X seconds (can be different for each song)
        private int peaksPerQuantom = 0;


        /**  Effects\Colors variables  **/
        //current color vars
        private int currentRed = 255;
        private int currentGreen = 255;
        private int currentBlue = 255;

        // random functions vars
        private int lastRandom = 0;
        private int lastEffect = 0;



        // effect configuration vars
        private int EFFECT_REPLAY_COUNT = 5; // how many times to repeat each normal effect (not for sequence effects).
        private int FLASH_MILLIS = 50;
        private int currentEffect = 0; // represent the current effect number.
        private int currentEffectStep = 0;


        

        public Form1()
        {
            InitializeComponent();
            sensivityScrollBar.Value = 57;
            de = new MMDeviceEnumerator();
            mediaDevice = de.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshComPortsCombo();
            BaudComboSetup();
            DefaultSetup_Load();
            ResetBtn.Enabled = false;

            // ComReceiveTxt.ScrollToCaret();
        }


        private void buttonStart_Click(object sender, EventArgs e)
        {
            StartConnection(); 
        }



        private void StartConnection()
        {
            CurrentCom = ComPortsCombo.Text;

            if (CurrentCom == "")
            {
                MessageBox.Show("Cant Connect ,Please select COM port");
                return;
            }

            try
            {
                serialPort1.PortName = ComPortsCombo.Text;
                serialPort1.BaudRate = Convert.ToInt32(BaudRate);

                serialPort1.Open();
                if (serialPort1.IsOpen)
                {
                    buttonStart.Enabled = false;
                    buttonStop.Enabled = true;
                   richTextBox1.ReadOnly = false;
                    BaudcomboBox.Enabled = false;
                    serialPort1.DtrEnable = true;
                    ResetBtn.Enabled = true;
                }  
            }
            catch (Exception )
            {
                MessageBox.Show(" Can't Connect \"" + ComPortsCombo.Text + "\"\nPlease check the name and if it used by other terminal");
            }
                
        

        }

        private void StopConnection()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
                richTextBox1.ReadOnly = true;
                BaudcomboBox.Enabled = true;
                ResetBtn.Enabled = false;
            }            
        }


        private void buttonStop_Click(object sender, EventArgs e)
        {
            StopConnection();

        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
        }


        private void DisplayText(object sender, EventArgs e)
        {
            richTextBox1.AppendText(RxString);
            if (AutoScrollcheckBox.Checked)
            {           
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }

                
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            RxString = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(DisplayText));
        }


        public List<string> GetAllPorts()
        {
            List<String> allPorts = new List<String>();
            foreach (String portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                allPorts.Add(portName);
            }
            return allPorts;
        }



        private String fixComName(String str)
        {
            Match m = Regex.Match(str, "[a-zA-Z]+[0-9]+");
            return m.Value;
        }

        private void BaudComboSetup()
        {
            BaudcomboBox.Items.Add("9600");
            BaudcomboBox.Items.Add("14400");
            BaudcomboBox.Items.Add("19200");
            BaudcomboBox.Items.Add("38400");
            BaudcomboBox.Items.Add("57600");
            BaudcomboBox.Items.Add("115200");
            BaudcomboBox.Items.Add("230400");
            BaudcomboBox.Items.Add("460800");
            BaudcomboBox.Items.Add("921600");   
        }

        private void DefaultSetup_Load()
        {
            BaudcomboBox.SelectedIndex = 0;
            ComPortsCombo.SelectedIndex = 0;
            LoadConfiguration();
        }

        private void RefreshComPortsCombo()
        {
            ComPortsCombo.Items.Clear();
            foreach (var port in GetAllPorts())
            {
                String FixedName = fixComName(port);
                ComPortsCombo.Items.Add(FixedName);
            }            
        }

        private void ComPortsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
                richTextBox1.ReadOnly = true;
            }
       //     CurrentCom = ComPortsCombo.SelectedItem.ToString();


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
                        if (line.Contains("Baud:"))
                            BaudComboSetup(line.Remove(0, 5));
                        else if (line.Contains("Com:"))
                            ComComboSetup(line.Remove(0, 4));
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
                    file.WriteLine("Baud:"+BaudcomboBox.Text);
                    file.WriteLine("Com:" + ComPortsCombo.Text);
                }
            }

            catch (Exception msg)
            {
                return;
            }

        }


        private void BaudComboSetup(string baud)
        {
            int index = -1;

            index = BaudcomboBox.FindString(baud);
            if (index != -1)
                BaudcomboBox.SelectedIndex = index;
        }

        private void ComComboSetup(string comPort)
        {
            int index = -1;

            //index = ComPortsCombo.FindString(comPort);
            index = ComPortsCombo.Items.IndexOf(comPort);
 
            if (index != -1)
                ComPortsCombo.SelectedIndex = index;
        }



        private void BaudcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BaudRate = BaudcomboBox.SelectedItem.ToString();

        }


        private void BaudcomboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            if (sendComTxt.Text.Length == 0)
                return;

            if (serialPort1.IsOpen)
            {
                serialPort1.Write(sendComTxt.Text);
                if (clearTxtChecbox.Checked)
                    sendComTxt.Text = "";
            }
            else
            {
                MessageBox.Show("NOT CONNECTED , Can't send DATA !");
            }

        }

        private void sendComTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendBtn_Click(sender,e);
            }
        }

        private void RecTextClearBtn_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
                StopConnection();
                richTextBox1.Text = "";
                serialPort1.DtrEnable = true;
                StartConnection();         
        }

        private void ComRefreshBtn_Click(object sender, EventArgs e)
        {
            RefreshComPortsCombo();
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
            if (mastervolume < sensivityScrollBar.Value || !serialPort1.IsOpen)
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
            Quarter(4,true);

        }

        private void EffectsRight()
        {
            // todo : add right effects
            Quarter(1, true);

        }

        private void Led_Effects_High_Peak()
        {
         //   ColorRefresh();
         //   //Quarter(myRandom(1, 4),true);
         //   Octet(myRandom(1,8),true);
         ////   serialPort1.Write("a");
      //   TestFunction();
            Flash();

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
                SolidColor(c);
            }

        }


        public void SolidColor(Color c)
        {
            char red = (char)(c.R/2);
            char green = (char)(c.G/2);
            char blue = (char)(c.B/2);
             

            serialPort1.Write("s" + red + green + blue + endChar);   
        }

       
        int MyRandom(int first, int last)
        {
            Random rnd = new Random();
            int num = rnd.Next(first, last + 1);
            while (num == lastRandom)
                num = rnd.Next(first, last + 1);

            lastRandom = num;
            return num;
        }

        int MyRandom_Effect(int first, int last)
        {
            Random rnd = new Random();
            int num = rnd.Next(first, last + 1);

            while (num == lastEffect)
                num = rnd.Next(first, last + 1);

            lastEffect = num;
            return num;
        }

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
            TestFunction();
        }

        public void TestFunction() // todo: remove this function and add to the callers
        {
          //  BarsFromMiddle(true);
          //  BarsMiddleFlash(true);
        //    SmartBarsFlash(true, 16, true);
              ShowSelector();
        }


        public void Quarter(int quarter,bool cleanStrip) // Turn on 1/4 of the leds 
        {
            if (quarter > 4)
                return;

            if (cleanStrip)
                ClearStrip();
            char[] cmd = { 'q', (char)quarter, (char)currentRed, (char)currentGreen, (char)currentBlue, endChar };
            serialPort1.Write(cmd, 0, 5);
        }


        public void Octet(int octet, bool cleanStrip) // Turn on 1/8 of the leds 
        {
            if (octet > 8)
                return;

            if (cleanStrip)
                ClearStrip();
            char[] cmd = { 'o', (char)octet, (char)currentRed, (char)currentGreen, (char)currentBlue, endChar };
            serialPort1.Write(cmd, 0, 5);
        }

        public void Nibble(int position, bool cleanStrip) // Turn on 1/16 of the leds 
        {
            if (position > 16 || position<1)
                return;

            if (cleanStrip)
                ClearStrip();
            char[] cmd = { 'n', (char)position, (char)currentRed, (char)currentGreen, (char)currentBlue, endChar };
            serialPort1.Write(cmd, 0, 5);
        }

        public void ColorRefresh()
        {
            Random rand = new Random();
            currentRed = rand.Next(1, 256);
            currentGreen = rand.Next(1, 256);
            currentBlue = rand.Next(1, 256);
        }




        public void BarsFromMiddle(Boolean colorFull)
        {
            if (colorFull)
                ColorRefresh();

            switch (currentEffectStep)
            {
                case 0:
                    Octet(4, true);
                    Thread.Sleep(sleepBetweenCommands);
                    Octet(5, false);
                    currentEffectStep++;
                    break;
                case 1:
                    Octet(6, false);
                    Thread.Sleep(sleepBetweenCommands);
                    Octet(3, false);
                    currentEffectStep++;
                    break;
                case 2:
                    Octet(2, false);
                    Thread.Sleep(sleepBetweenCommands);
                    Octet(7, false);
                    currentEffectStep++;
                    break;
                case 3:
                    Octet(1, false);
                    Thread.Sleep(sleepBetweenCommands);
                    Octet(8, false);
                    currentEffectStep++;
                    break;
                case 4:
                    EndOfEffect();
                    break;
            }
           
        }


        private void OctetTwoPlaces(int pos1, int pos2, bool cleanStrip)
        {
            if(cleanStrip)
                ClearStrip();
            Octet(pos1, false);
            Thread.Sleep(sleepBetweenCommands);
            Octet(pos2, false);  
        }


        public void BarsMiddleFlash(Boolean colorFull)
        {
            if (colorFull)
                ColorRefresh();

            switch (currentEffectStep)
            {
                case 0:
                    OctetTwoPlaces(4, 5,true);
                    currentEffectStep++;
                    break;
                case 1:
                    OctetTwoPlaces(3, 6,true);
                    currentEffectStep++;
                    break;
                case 2:
                    OctetTwoPlaces(2, 7, true);
                    currentEffectStep++;
                    break;
                case 3:
                    OctetTwoPlaces(1, 8, true);
                    currentEffectStep++;
                    break;
                case 4:
                    OctetTwoPlaces(2, 7, true);
                    currentEffectStep++;
                    break;
                case 5:
                    OctetTwoPlaces(3, 6, true);
                    currentEffectStep++;
                    break;
                case 6:             
                    OctetTwoPlaces(4, 5, true);
                    currentEffectStep++;
                    break;
                case 7:
                    EndOfEffect();
                    break;
            }

            Thread.Sleep(sleepBetweenCommands);            
        }


        public void EndOfEffect() // called when the current effect finished
        {
            ClearStrip();
            currentEffectStep = 0;
            currentEffect = 0;
            
        }

        public void SmartBarsFlash(Boolean returnToCenter, int divSize, Boolean colorFull)
        {      
            if (colorFull)
                ColorRefresh();

            int right = divSize / 2 -  currentEffectStep;
            int left = divSize / 2 + 1 + currentEffectStep;

            if (currentEffectStep == divSize / 2)
            {
                EndOfEffect();
                return;   
            }

            if(currentEffectStep == 0)
                ClearStrip();

            SmartBar(divSize,left,true);
            Thread.Sleep(sleepBetweenCommands);
            SmartBar(divSize, right, false);
            Thread.Sleep(sleepBetweenCommands);

            currentEffectStep++;

        }


        public void SmartBar(int divSize, int position, Boolean clean)
        {
            switch (divSize)
            {
                case 16:
                    Nibble(position, clean);
                    break;
                case 8:
                    Octet(position,clean);
                    break;
                case 4:
                    Quarter(position,clean);
                    break;
            }
        }


        public void ClearStrip()
        {
            serialPort1.Write("c" + endChar);
        }


        public void RandomPartOn()
        {
            serialPort1.Write("rp"+endChar);
            currentEffectStep++;
            if(currentEffectStep == EFFECT_REPLAY_COUNT)
                EndOfEffect();
        }


        public void Flash()
        {
            ColorRefresh();
            serialPort1.Write("s" + (char)currentRed + (char)currentBlue + (char)currentGreen + endChar);
            //if (currentEffectStep++ == EFFECT_REPLAY_COUNT)
            //    EndOfEffect();
            Thread.Sleep(FLASH_MILLIS);
            ClearStrip();    
        }

        public void SolidColor()
        {
            ColorRefresh();
            serialPort1.Write("s" + (char)currentRed + (char)currentBlue + (char)currentGreen + endChar);
            if (currentEffectStep++ == EFFECT_REPLAY_COUNT)
                EndOfEffect();
        }



        public void ShowSelector()
        {
            if (currentEffect == 0)
                currentEffect = MyRandom_Effect(1, 4);
            switch (currentEffect)
            {
                case 1:
                    BarsMiddleFlash(true);
                    break;
                case 2:
                    BarsFromMiddle(true);
                    break;
                case 3:
                   SmartBarsFlash(false, 16, true);
                    break;
                case 4:
                    RandomPartOn();
                    break;
            }

            Thread.Sleep(sleepBetweenCommands);
        }

        private void timerPeaksCounterReset_Tick(object sender, EventArgs e)
        {
            peaksPerQuantom = 0;
        }
    }
}


