using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

//Plays a sound when 5 minutes has passed, plays again every 30 seconds if not reset
namespace WindowsFormsApplication1
{
    public partial class NMZTimer : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //Var declarations
        SoundPlayer alert, reset;
        Timer time;

        float timeLeft, maxTime;
        string defaultTime;

        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        public NMZTimer()
        {
            InitializeComponent();

            int id = 0;
            RegisterHotKey(this.Handle, id, (int)KeyModifier.Control, Keys.D1.GetHashCode());
        }

        //Initializes variables and centers UI objects
        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            time = new Timer();
            alert = new SoundPlayer(Properties.Resources.alert);
            reset = new SoundPlayer(Properties.Resources.reset);
            time.Tick += new EventHandler(TimerTick);

            panel2.Visible = false;
            maxTime = 300;
            time.Interval = 1000;
            defaultTime = "5:00";

            lblWarning.Location = new Point((this.Width / 2) - (lblWarning.Size.Width / 2), lblWarning.Location.Y);
            button1.Location = new Point((this.Width / 2) - (button1.Size.Width / 2), button1.Location.Y);
            btnReset.Location = new Point((this.Width / 2) - (btnReset.Size.Width / 2) - 85, btnReset.Location.Y);
            btnStop.Location = new Point((this.Width / 2) - (btnStop.Size.Width / 2) + 85, btnStop.Location.Y);

            CenterTime();
        }

        //Called once per 1000ms, updates timer and performs functions when time has passed 5 minutes
        void TimerTick(object sender, EventArgs e)
        {
            timeLeft -= 1;

            TimeSpan t = TimeSpan.FromMinutes(Convert.ToDouble(timeLeft));
            string timeFormatted = t.ToString(@"hh\:mm");

            if (timeLeft == 0 || timeLeft == -2 || timeLeft == -30 || timeLeft == -32)
            {
                alert.Play();
            }

            if (timeLeft <= 0)
            {
                lblWarning.Visible = !lblWarning.Visible;
                timeFormatted = "- " + timeFormatted;
            }

            lblTime.Text = timeFormatted;
            CenterTime();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312 && panel2.Visible)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                 * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if 
                 * you want to know which key/modifier was pressed for some particular reason. */

                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                //Resets timer
                ResetTimer();
                reset.Play();
            }
        }

        //Start button, swaps visible UI and resets time
        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;

            timeLeft = maxTime;
            time.Enabled = true;
        }

        //Called when reset button is clicked
        private void btnReset_Click(object sender, EventArgs e)
        {
            timeLeft = maxTime;
            lblTime.Text = defaultTime;
            lblWarning.Visible = false;
            CenterTime();
        }

        //Stops timer and returns to main menu
        private void btnStop_Click(object sender, EventArgs e)
        {
            timeLeft = maxTime;
            lblTime.Text = defaultTime;

            time.Enabled = false;
            panel1.Visible = true;
            panel2.Visible = false;
            lblWarning.Visible = false;

            CenterTime();
        }

        //Calculates center position, as the size of the label changes as the clock ticks down
        void CenterTime()
        {
            lblTime.Location = new Point((this.Width / 2) - (lblTime.Size.Width / 2), lblTime.Location.Y);
        }

        //Performs same action as clicking reset button
        void ResetTimer()
        {
            timeLeft = maxTime;
            lblTime.Text = defaultTime;
            lblWarning.Visible = false;
            CenterTime();
        }
    }
}
