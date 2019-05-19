using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing.Drawing2D;
using CustomForm;

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
            BackColorOld = BackColor;
            BackColor = ControlPaint.Light(BackColor);
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e) // not needed really, but nice to have
        {
            BackColor = BackColorOld;
            base.OnMouseLeave(e);
        }

    }

    public class RForm : GaussForm
    {
        private int wHeight;
        private int outHeight;
        private readonly int outWidth=700;
        private int mWidth;

        private TextBox[,] matrixinput;
        Panel panel1 = new Panel();
        Panel panel2 = new Panel();
        private ShapedButton runBtn = new ShapedButton();
        private Button bufferBtn = new Button();
        private Label[] labeleq;
        private Label labelsz=new Label();
        private RichTextBox output = new RichTextBox();
        private NumericUpDown msize = new NumericUpDown();
        ContextMenu cMenu;
        ContextMenu mMenu;

        int rowsnum= 3;

        private  Gaussnew GV;


        
        public RForm()
        {
            this.InitializeComponent();
           

        }

        private void InitializeComponent()
        {
            msize.Size = new System.Drawing.Size(45, 30);
            msize.Location = new System.Drawing.Point(50, 50);
            msize.Value = rowsnum;
            msize.Maximum = 10;
            msize.Minimum = 2;

            labelsz.Location = new System.Drawing.Point(50, 30);
            labelsz.Text = "Size:";

            runBtn.Size = new System.Drawing.Size(160, 100);
            runBtn.Location = new System.Drawing.Point(30, 90);
            runBtn.Text = "Run Gauß run!";
            runBtn.Font = new Font(runBtn.Font.FontFamily, 10);
            runBtn.BackColor = Color.Green;
            
            bufferBtn.Size = new System.Drawing.Size(45, 48);
            bufferBtn.BackColor = Color.Transparent;
            bufferBtn.FlatStyle = FlatStyle.Flat;
            bufferBtn.FlatAppearance.BorderSize = 0;
            bufferBtn.Image = Resource1.clipboard;

            cMenu = new ContextMenu();
            var copyItem = cMenu.MenuItems.Add("Copy to clipboard");
            copyItem.Click += new EventHandler(cMenu_ItemClicked);

            mMenu = new ContextMenu();
            var randomItem = mMenu.MenuItems.Add("Random fill");
            var clearItem = mMenu.MenuItems.Add("Clear matrix");
            var zeroItem = mMenu.MenuItems.Add("Set to zero");
            randomItem.Click += new EventHandler(random_ItemClicked);
            clearItem.Click += new EventHandler(clear_ItemClicked);
            zeroItem.Click += new EventHandler(zero_ItemClicked);


            panel1.BackColor = Color.Transparent;
            panel2.BackColor = Color.GhostWhite;

            addInputMask(rowsnum);
            Add(msize);

            //add Controls for a default form;
            panel1.Location = new Point(200, 10);
            output.Multiline = true;
            output.Font = new Font("Courier New", 10, FontStyle.Bold);
            output.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
            output.WordWrap = false;
            output.Text = "";
            output.ReadOnly = true;
            output.BackColor = Color.GhostWhite;
            output.BorderStyle = BorderStyle.None;
            updateWindowSize();
            output.Location = new Point(50, 0);
            panel2.Controls.Add(output);
            bufferBtn.Location = new Point(3, 3);
            panel2.Controls.Add(bufferBtn);
            Add(labelsz);
            Add(runBtn);
            Add(panel1);
            Add(panel2);
            msize.ValueChanged += new System.EventHandler(msize_ValueChanged);

            runBtn.Click += new System.EventHandler(runBtn_click);
            bufferBtn.Click += new System.EventHandler(bufferBtn_click);
            copyItem.Click += new System.EventHandler(cMenu_ItemClicked);

            Text = "Gaußsches Verfahren";
            BackColor = Color.Teal;
            output.MouseClick += new MouseEventHandler(output_onMouse_click);
            panel2.MouseClick += new MouseEventHandler(panel2_onMouse_click);
            panel1.MouseClick += new MouseEventHandler(panel_onMouse_click);
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

        private void updateWindowSize()
        {
            wHeight= Screen.PrimaryScreen.WorkingArea.Height;
            outHeight = wHeight - 150;
            mWidth = 50 * (rowsnum + 1);
            panel2.Size = new System.Drawing.Size(outWidth+47, outHeight);
            output.Size = new System.Drawing.Size(outWidth, outHeight);
            panel1.Size = new Size(mWidth, mWidth - 50);
            output.Location= new System.Drawing.Point(50,0); ;
            panel2.Location = new System.Drawing.Point(panel1.Location.X + mWidth, 20);
            //bufferBtn.Location = new System.Drawing.Point(panel1.Location.X + mWidth - 10, 0);
            Size = new Size(panel1.Location.X + mWidth + outWidth + 90, wHeight);
        }


        

        private void panel_onMouse_click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

               mMenu.Show(panel1, new System.Drawing.Point(e.X, e.Y));
            }
        }

        private void panel2_onMouse_click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                cMenu.Show(panel2, new System.Drawing.Point(e.X, e.Y));
            }
        }

        private void output_onMouse_click(object sender, MouseEventArgs e)
        {
            panel2_onMouse_click(panel2,e);
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
                updateWindowSize();
                Add(panel2);
                             
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