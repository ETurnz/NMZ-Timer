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

namespace WindowsFormsApplication1
{
    //Plays a sound when 5 minutes has passed, plays again every 30 seconds if not reset
    public partial class NMZTimer : Form
    {
        //Var declarations
        SoundPlayer alert;
        Timer time;
        float timeLeft;
        float maxTime;
        string defaultTime;

        public NMZTimer()
        {
            InitializeComponent();
        }

        //Initializes variables and centers UI objects
        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            time = new Timer();
            alert = new SoundPlayer(Properties.Resources.alert);
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

            if (timeLeft == 0 || timeLeft == -2 || timeLeft == -4 || timeLeft == -30 || timeLeft == -32 || timeLeft == -34)
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

        //Start button, swaps visible UI and resets time
        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;

            timeLeft = maxTime;
            time.Enabled = true;
        }

        //Resets timer
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
    }
}
