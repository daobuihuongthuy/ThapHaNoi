using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace DOANCTDL
{
    public partial class Form1 : Form
    {
        TimeSpan time;
        int moveCount;
        PictureBox[] disks;
        Stack<PictureBox> disksA, disksB, disksC, firstClickDisks, secondClickedDisks;
        const int FIRSTY = 750; // hang so mac dinh vi tri toa do y
        const int DISKHEIGHT = 40; // DO CAO DIA
        const int DISTXFROMRODTODISK = 11;// KHOANG CACH COC TOI DIA
        public Form1()
        {
            InitializeComponent();
            disks = new PictureBox[] { p1, p2, p3, p4, p5, p6, p7, p8 };
            picRodA.Tag = disksA = new Stack<PictureBox>();
            picRodB.Tag = disksB = new Stack<PictureBox>();
            picRodC.Tag = disksC = new Stack<PictureBox>();

        }


        private void MoveDisk(Point point)
        {
            PictureBox firstTopDisk = firstClickDisks.Pop();
            firstTopDisk.Location = point;
            secondClickedDisks.Push(firstTopDisk); ++moveCount;
            lblMoveCount.Text = string.Format("Số lần di chuyển: {0} lần", moveCount);

            firstClickDisks = secondClickedDisks = null;
            picRodA.BorderStyle = picRodB.BorderStyle = picRodC.BorderStyle = BorderStyle.None;
            if (disksC.Count == nudLevel.Value)
            {

                MessageBox.Show("Surprise, you have completed the game.Congratulation!!!");
                btnGiveIn.Enabled = false;
                btnPause.Enabled = false;
            }
        }

        private void btnShowRule_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Luật Chơi: \n- Mỗi lần chỉ được di chuyển 1 đĩa trên cùng của cọc\n- Đĩa nằm trên cùng phải nhỏ hơn đĩa nằm dưới",
                "Luật Chơi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tmrCountTime_Tick(object sender, EventArgs e)
        {
            time = time.Add(new TimeSpan(0, 0, 1));
            lblTime.Text = string.Format("Thời gian: {0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
        }

        private void btnGiveIn_Click(object sender, EventArgs e)
        {
            tmrCountTime.Stop();
            nudLevel.Enabled = true;
            /*nut bo cuoc*/
            btnGiveIn.Enabled = false;
            btnPause.Enabled = false;
            btnPlay.Text = "Chơi Lại";


            MessageBox.Show("Game Over :))");
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            /*nut dung*/
            //tmrCountTime.Stop();
            /*ko chac buoc nay nua, cai timer phai la time k?
             hoac la doi time thanh CountTime*/
            if (tmrCountTime.Enabled)
            {
                tmrCountTime.Enabled = false;
                btnPause.Text = "choi tiep";

                /* cả 3 cột*/
                picRodA.Enabled = false;
                picRodB.Enabled = false;
                picRodC.Enabled = false;
            }

            else
            {
                tmrCountTime.Enabled = true;
                picRodA.Enabled = true;
                picRodB.Enabled = true;
                picRodC.Enabled = true;
                btnPause.Text = "tam dung";

            }


        }

        

        private void btnPlay_Click(object sender, EventArgs e)
        {
            //reset- giong choi lai: dat tat ca ve mac dinh 11 dong
            tmrCountTime.Stop();
            foreach (PictureBox disk in disks)
                disk.Visible = false;
            time = new TimeSpan(0, 0, 0);
            moveCount = 0;
            lblTime.Text = "Thời gian: 00:00:00";
            lblMoveCount.Text = "Số lần di chuyên: 0 lần";
            disksA.Clear();
            disksB.Clear();
            disksC.Clear();
            picRodA.BorderStyle = picRodA.BorderStyle = picRodA.BorderStyle = BorderStyle.None;
            firstClickDisks = secondClickedDisks = null;

            //intialze
            nudLevel.Enabled = false; // khong cho thay doi so luong dia
            btnGiveIn.Enabled = true;  // cho bam nut chiu thua

            btnPause.Visible = true; /*ban dau la false*/

            // btnGiveIn.Enable= true;  // cho bam nut chiu thua
            btnPlay.Text = "Chơi Lại"; // chuyen doi nut choi thanh choi lai
            // hoac khong cho ban nut choi (thuy) // btnPlay.Enabled= False;
            // them cac dia
            int x = picRodA.Location.X, y = FIRSTY;
            for (int i = (int)nudLevel.Value - 1; i >= 0; --i)
            {
                disks[i].Location = new Point(x, y);
                disks[i].Visible = true;
                disksA.Push(disks[i]);
                y = y - DISKHEIGHT;
            }
            tmrCountTime.Start();
        }

        private void ProcessMovingDisk(PictureBox clickedRod)
        {
            if (secondClickedDisks.Count == 0)
                MoveDisk(new Point(clickedRod.Location.X, FIRSTY));
            else
            {
                PictureBox firstTopDisk = firstClickDisks.Peek();
                PictureBox secondTopDisk = secondClickedDisks.Peek();
                if (int.Parse(firstTopDisk.Tag.ToString()) < int.Parse(secondTopDisk.Tag.ToString()))
                    MoveDisk(new Point(secondTopDisk.Location.X, secondTopDisk.Location.Y - DISKHEIGHT));
                else
                    secondClickedDisks = null;
            }
        }
        private void picRod_click(object sender, EventArgs e)// bam vao cot de choi
        {
            {
                if (nudLevel.Enabled) return;// ko choi
                PictureBox clickedRod = (PictureBox)sender;
                Stack<PictureBox> diskOfClickRod = (Stack<PictureBox>)clickedRod.Tag;

                if (firstClickDisks == null)
                {
                    if (diskOfClickRod.Count == 0) return;
                    firstClickDisks = diskOfClickRod;
                    clickedRod.BorderStyle = BorderStyle.FixedSingle;

                }
                else if (secondClickedDisks == null)
                {
                    if (diskOfClickRod == firstClickDisks)
                    {
                        firstClickDisks = null;
                        clickedRod.BorderStyle = BorderStyle.None;
                        return;
                    }
                    secondClickedDisks = diskOfClickRod;
                    ProcessMovingDisk(clickedRod);

                }
            }
        }


    }
}



