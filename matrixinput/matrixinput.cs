using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing.Drawing2D;

namespace matrixinput
{

    public class ShapedButton : Button
    {
        Color BackColorOld;

        public ShapedButton(): base()
        {
            BackColorOld = BackColor;

        }   
        protected override void OnResize(EventArgs e)
        {

            GraphicsPath gp = new GraphicsPath();
            Rectangle R = new Rectangle(2,2, this.Width-6, this.Height-6);
            gp.AddEllipse(R);
            this.Region = new Region(gp);
            base.OnResize(e);
            R.Inflate(1, 1);
        }

        protected override void OnMouseEnter(EventArgs e) // not needed really, but nice to have
        {
            /*Graphics g = CreateGraphics();
            Rectangle R = new Rectangle(2, 2, this.Width - 6, this.Height - 6);
            Pen pen = new Pen(Color.Red, 1);
            SolidBrush brush = new SolidBrush(Color.Red);
            g.FillEllipse(brush,R);*/
            BackColorOld = BackColor;
            BackColor = ControlPaint.Light(BackColor);

        
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e) // not needed really, but nice to have
        {
            /*Graphics g = CreateGraphics();
            Rectangle R = new Rectangle(2, 2, this.Width - 6, this.Height - 6);
            Pen pen = new Pen(Color.Red, 1);
            SolidBrush brush = new SolidBrush(Color.Red);
            g.FillEllipse(brush,R);*/
            BackColor = BackColorOld;
            base.OnMouseLeave(e);
        }

    }

    public class RForm : Form
    {
        private TextBox[,] matrixinput;
        Panel panel1 = new Panel();
        private ShapedButton runBtn = new ShapedButton();
        private Button bufferBtn = new Button();
        private Label[] labeleq;
        private Label labelsz=new Label();
        private RichTextBox output = new RichTextBox();
        private NumericUpDown msize = new NumericUpDown();
        ContextMenu cMenu;
        ContextMenu mMenu;

        int rowsnum= 3;

        //private Label timelabel = new System.Windows.Forms.Label();
       // double[,] G = new double[3, 4] { { -5, 1, 7, 3 }, { 1, 2, -1, 2 }, { 3, -1, 11, 13 } };
        private  Gaussnew GV;


        //private System.Windows.Forms.Label label1;

        public RForm()
        {
            this.InitializeComponent();
           

        }

        [System.STAThreadAttribute()]
        public static void Main()
        {
            System.Windows.Forms.Application.Run(new RForm());
        }


        private void clearInputMask(int m)
        {
            for (int i = 0; i < m; i++)
                for (int j = 0; j < m+1; j++)
                    panel1.Controls.Remove(matrixinput[i, j]);

            for (int i = 0; i < m; i++)
                panel1.Controls.Remove(labeleq[i]);


        }

        private void addInputMask(int m)
        {
            matrixinput = new TextBox[m, m+1];
            for (int i = 0; i < m; i++)
                for (int j = 0; j < m+1; j++)
                {
                    matrixinput[i, j] = new TextBox();
                    matrixinput[i, j].Location = new System.Drawing.Point(50 * j, 50 * i);
                    matrixinput[i, j].Size = new System.Drawing.Size(30, 20);
                    matrixinput[i, j].Leave += new EventHandler(input_Leave);

                }
            labeleq = new Label[m];
            for (int i = 0; i < m; i++)
            {
                labeleq[i] = new Label();
                labeleq[i].Text = "=";
                labeleq[i].Location = new System.Drawing.Point(35 +50*(m-1),  50 * i);

            }

            for (int i = 0; i < m; i++)
                for (int j = 0; j < m + 1; j++)
                    panel1.Controls.Add(matrixinput[i, j]);

            for (int i = 0; i < m; i++)
                panel1.Controls.AddRange(labeleq);


        }

        private void input_Leave(object sender, System.EventArgs e)
        {
            if (!Double.TryParse(((TextBox)sender).Text, out double res))
                ((TextBox)sender).BackColor = Color.LightPink;
            else
                ((TextBox)sender).BackColor = Color.White;
        }


        private void InitializeComponent()
        {


            output.Size = new System.Drawing.Size(700, 300);
            output.Location = new System.Drawing.Point(50, 490);
            output.Multiline = true;
            output.Font = new Font("Courier New", 10, FontStyle.Bold);
            output.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
            output.WordWrap= false;
            output.Text = "";
            output.ReadOnly = true;
            output.BackColor = Color.GhostWhite;

            msize.Size = new System.Drawing.Size(45, 30);
            msize.Location = new System.Drawing.Point(50, 50);
            msize.Value = rowsnum;
            msize.Maximum = 10;
            msize.Minimum = 2;

            labelsz.Location = new System.Drawing.Point(50, 30);
            labelsz.Text = "Size:";

            runBtn.Size = new System.Drawing.Size(160, 100);
            runBtn.Location = new System.Drawing.Point(30, 90);
            runBtn.Text ="Run Gauß run!";
            runBtn.BackColor = Color.Green;

            bufferBtn.Size = new System.Drawing.Size(35, 40);
            bufferBtn.Location = new System.Drawing.Point(750, 440);
            bufferBtn.BackColor = Color.White;
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("matrixinput.icon");
            Icon myIcon = new Icon(myStream);
            bufferBtn.Image = myIcon.ToBitmap();

            cMenu = new ContextMenu();
            var copyItem = cMenu.MenuItems.Add("copy");
            copyItem.Click += new EventHandler(cMenu_ItemClicked);

            mMenu = new ContextMenu();
            var randomItem = mMenu.MenuItems.Add("Random fill");
            var clearItem = mMenu.MenuItems.Add("Clear matrix");
            var zeroItem = mMenu.MenuItems.Add("Set to zero");
            randomItem.Click += new EventHandler(random_ItemClicked);
            clearItem.Click += new EventHandler(clear_ItemClicked);
            zeroItem.Click += new EventHandler(zero_ItemClicked);

            panel1.Location = new Point(200, 10);
            panel1.Size = new Size(550, 500);
            panel1.BackColor = Color.Transparent;

            addInputMask(rowsnum);
           
            ClientSize = new System.Drawing.Size(800, 800);
                      
            //Controls.AddRange(labeleq);
            Controls.Add(output);
            Controls.Add(msize);
            Controls.Add(labelsz);
            Controls.Add(runBtn);
            Controls.Add(bufferBtn);
            Controls.Add(panel1);
            msize.ValueChanged += new System.EventHandler(msize_ValueChanged);

            runBtn.Click+= new System.EventHandler(runBtn_click);
            bufferBtn.Click += new System.EventHandler(bufferBtn_click);
            copyItem.Click += new System.EventHandler(cMenu_ItemClicked);

            Text = "Gaußsches Verfahren";
            BackColor = Color.Teal;
            output.MouseClick += new MouseEventHandler(output_onMouse_click);
            panel1.MouseClick += new MouseEventHandler(panel_onMouse_click);
            //  string[] resNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            // foreach (string resName in resNames)
            //   writeLine(resName);

        }

        private void panel_onMouse_click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

               mMenu.Show(panel1, new System.Drawing.Point(e.X, e.Y));
            }
        }

        private void clear_ItemClicked(object sender, EventArgs e)
        {
            for (int i = 0; i < rowsnum; i++)
                for (int j = 0; j < rowsnum + 1; j++)
                {
                    matrixinput[i, j].BackColor = Color.White;
                    matrixinput[i, j].Text = "";
                }
        }

        private void zero_ItemClicked(object sender, EventArgs e)
        {
            for (int i = 0; i < rowsnum; i++)
                for (int j = 0; j < rowsnum + 1; j++)
                {
                    matrixinput[i, j].BackColor = Color.White;
                    matrixinput[i, j].Text = "0";
                }
        }

        private void random_ItemClicked(object sender, EventArgs e)
        {
            Random rnd = new Random();
            for (int i = 0; i < rowsnum; i++)
                for (int j = 0; j < rowsnum + 1; j++)
                {
                    matrixinput[i, j].BackColor = Color.White;
                    matrixinput[i, j].Text = rnd.Next(-9,10).ToString();
                }

        }

        private void msize_ValueChanged(Object sender, EventArgs e)
        {
            if ((msize.Value>1)&&(msize.Value<=10))
            {
                clearInputMask(rowsnum);
                addInputMask((int)msize.Value);
                rowsnum = (int)msize.Value;

            }
            }


        Boolean inputData(out double[,] GM)
        {
            Boolean f = true;
            GM = new double[rowsnum, rowsnum + 1];
           

            for(int i=0; i<rowsnum; i++)
                for (int j = 0; j < rowsnum+1; j++)
                   if  (!Double.TryParse(matrixinput[i,j].Text,out double res))
                    {
                        matrixinput[i, j].BackColor = Color.LightPink;
                        f=false;

                    }
                    else
                    {   
                        GM[i, j] = res;
                        matrixinput[i, j].BackColor = Color.White;
                    }
            return (f);
        }

        public void clearOutput()
        {
            output.Text = "";
           
        }

        public void writeLine(String str)
        {
            output.AppendText(str);
            output.AppendText(Environment.NewLine);
        }

        public void writeLine(String str, Color color)
        {

            output.SelectionColor = color;
            output.AppendText(str);
            output.AppendText(Environment.NewLine); 
            output.SelectionColor = output.ForeColor;
        }

        void cMenu_ItemClicked(object sender, EventArgs e)
        {
            clipboard_copy();
        }

        private void clipboard_copy()
        {

            if (output.Text == "") return;

            if (output.SelectedText == "")
                Clipboard.SetText(output.Text);
            else
                Clipboard.SetText(output.SelectedText);

        }
        private void bufferBtn_click(object sender, System.EventArgs e)
        {
            clipboard_copy();
        }
        private void output_onMouse_click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                cMenu.Show(output, new System.Drawing.Point(e.X, e.Y));
            }
        }

            private void runBtn_click(object sender, System.EventArgs e)
        {
            double[,] GM;
            if (inputData(out GM))
            {
                clearOutput();
                GV = new Gaussnew(GM);
                writeLine("Ursprüngliche Matrix (in letzter Spalte stehen Zahlen rechts von = )", Color.Blue);
                printMatrix();
                String logStr="";
                for (int k = 0; k < GV.r; k++)
                {
                    if (!GV.NormRow(k, out logStr))
                    {
                        writeLine("Das Gleichungssystem besitzt keine oder keine eindeutige Lösung",Color.Red);
                        return;
                    }
                    else
                    {
                        writeLine("Normieren der " + (k + 1).ToString() + "-ten Zeile", Color.Blue);
                        writeLine(logStr);

                    }
                    printMatrix();

                    if (k != GV.r - 1)
                    {                       
                        GV.Elimination(k,out logStr);
                        if (logStr != "")
                        {
                            writeLine("Eliminieren von X" + (k + 1).ToString(), Color.Blue);
                            writeLine(logStr);
                            printMatrix();
                        }

                    }
                }
                    for (int k = GV.r - 1; k >= 1; k--)
                    {
                       
                        GV.BackSubstitution(k, out logStr);
                     if (logStr != "")
                     {
                            writeLine("Rücksubstition von X" + (k + 1).ToString(), Color.Blue);
                            writeLine(logStr);
                            printMatrix();
                    }
                                     
                        
                    }

                    writeLine("Die Lösung", Color.Blue);
                    printSolution();
                    output.AppendText(new String('_', 85));

                }

            }


        public void printMatrix()
        {

            string[] tempArray = output.Lines;
            int r =GV.r;
            int c = r+1;


            for (int i = 0; i < r; i++)
            {

                for (int j = 0; j < c; j++)
                {
                    output.AppendText(String.Format("{0:0.#####} ", GV[i,j]).PadRight(12));

                    if (j == r-1)
                        output.AppendText("|");
                    //;
                }
                output.AppendText(Environment.NewLine);
                }
            output.AppendText(Environment.NewLine);
        }


        public void printSolution()
        {
            double[] x = GV.GetSolution();

            for (int j = 0; j < GV.r; j++)
            {
                output.AppendText("x" + (j + 1).ToString() + " =" + String.Format("{0:0.#####} ", x[j]).PadRight(9));
                output.AppendText(Environment.NewLine);
            }

        }


    }
}