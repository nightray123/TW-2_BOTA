using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot;
using System.IO.Ports;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;//chartarea

namespace 传感_电机_电机控制程序
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("串口未打开，请检查!");
                return;
            }
            else
            {
                serialPort1.Write("127");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        //开始计算按钮
        double fx, fy, fz;
        //实际测量力
        int o1, o2;
        //角度输入
        double gx, gy, gz;
        //重力
        double Fx, Fy, Fz;
        //f-g
        double x1, y1, z1;
        public void compute()
        {
            //计算过的力
            //try {
            o1 = int.Parse(textBox1.Text);
            o2 = int.Parse(textBox2.Text);
            gx = float.Parse(textBox3.Text);
            gy = float.Parse(textBox4.Text);
            gz = float.Parse(textBox5.Text);
            fx = double.Parse(textBox6.Text);
            fy = double.Parse(textBox7.Text);
            fz = double.Parse(textBox8.Text);
            Fx = fx - gx;
            Fy = fy - gy;
            Fz = fz - gz;
            x1 = Fx * (Math.Cos(o2)) - Fz * (Math.Sin(o2));
            y1 = Fy * (Math.Cos(o1)) - Fz * (Math.Cos(o2)) * (Math.Sin(o1)) + Fx * (Math.Sin(o1)) * (Math.Sin(o2));
            z1 = Fy * (Math.Sin(o1)) + Fz * (Math.Cos(o1)) * (Math.Cos(o2)) - Fx * (Math.Cos(o1)) * (Math.Sin(o2));
            this.textBox9.Text = Fx.ToString();
            this.textBox10.Text = Fy.ToString();
            this.textBox11.Text = Fz.ToString();
            this.textBox12.Text = x1.ToString();
            this.textBox13.Text = y1.ToString();
            this.textBox14.Text = z1.ToString();


        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            compute();

    }
        private void button1_Click(object sender, EventArgs e)
        {
            load_CSV();
            timer1.Start();
        }
        int timer_count = 0;
        int j = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            UpdateQueueValue();
            this.chart1.Series[0].Points.Clear();
            this.chart1.Series[1].Points.Clear();
            this.chart1.Series[2].Points.Clear();
            for (int i = 0; i < dataQueue1.Count; i++)
            {
                this.chart1.Series[0].Points.AddXY((i + 1), dataQueue1.ElementAt(i));
                this.chart1.Series[1].Points.AddXY((i + 1), dataQueue2.ElementAt(i));
                this.chart1.Series[2].Points.AddXY((i + 1), dataQueue3.ElementAt(i));
            }
            //展示到页面
            if(j < dt.Rows.Count) {
                textBox6.Text = dt.Rows[j][0].ToString();
                textBox7.Text = dt.Rows[j][1].ToString();
                textBox8.Text = dt.Rows[j][2].ToString();
            }
            j++;
            timer_count++;

        }
       
        string path;
        private void InitChart()//设置图表
        {
            //定义图表区域
            this.chart1.ChartAreas.Clear();
            ChartArea chartArea1 = new ChartArea("C1");
            this.chart1.ChartAreas.Add(chartArea1);
            //定义存储和显示点的容器
            this.chart1.Series.Clear();
            Series series1 = new Series("Fx");
            Series series2 = new Series("Fy");
            Series series3 = new Series("Fz");
            series1.ChartArea = "C1";
            series2.ChartArea = "C1";
            series3.ChartArea = "C1";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            //设置图表显示样式
            //this.chart1.ChartAreas[0].AxisY.Minimum = -10;
            //this.chart1.ChartAreas[0].AxisY.Maximum = 10;
            this.chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;
            this.chart1.ChartAreas[0].AxisX.IsStartedFromZero = false;
            this.chart1.ChartAreas[0].AxisX.Interval = 10;
            this.chart1.ChartAreas[0].AxisY.Interval = 0.1;
            //this.chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false; //去掉表的背景线
            //this.chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false; //去掉表的背景线

            //this.chart1.ChartAreas[0].AxisX.ScaleView.Zoom(2, 3);
            //this.chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            //this.chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            //this.chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            ////将滚动内嵌到坐标轴中
            //this.chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            //// 设置滚动条的大小
            //this.chart1.ChartAreas[0].AxisX.ScrollBar.Size = 10;
            //// 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
            //this.chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
            //this.chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = double.NaN;
            //this.chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = 2;


            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            //设置标题
            this.chart1.Titles.Clear();
            this.chart1.Titles.Add("Fx");
            this.chart1.Titles[0].Text = "波形图";
            this.chart1.Titles[0].ForeColor = Color.Black;
            this.chart1.Titles[0].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            //设置图表显示样式
            this.chart1.Series[0].Color = Color.Blue;
            this.chart1.Series[0].ChartType = SeriesChartType.Line;
            this.chart1.Series[0].Points.Clear();

            this.chart1.Series[1].Color = Color.Red;
            this.chart1.Series[1].ChartType = SeriesChartType.Line;
            this.chart1.Series[1].Points.Clear();

            this.chart1.Series[2].Color = Color.Green;
            this.chart1.Series[2].ChartType = SeriesChartType.Line;
            this.chart1.Series[2].Points.Clear();
        }
        private Queue<double> dataQueue1 = new Queue<double>(500);
        private Queue<double> dataQueue2 = new Queue<double>(500);
        private Queue<double> dataQueue3 = new Queue<double>(500);
        private int num = 1;//每次删除增加几个点

        private void UpdateQueueValue()//更新数据
        {
            if (dataQueue1.Count > 500)
            {
                //先出列
                for (int i = 0; i < num; i++)
                {
                    dataQueue1.Dequeue();
                    dataQueue2.Dequeue();
                    dataQueue3.Dequeue();

                }
            }
                for (int i = 0; i < num; i++)
                {
                double Fx = double.Parse(dt.Rows[j][0].ToString());
                double Fy = double.Parse(dt.Rows[j][1].ToString());
                double Fz = double.Parse(dt.Rows[j][2].ToString());
                dataQueue1.Enqueue(Fx);
                dataQueue2.Enqueue(Fy);
                dataQueue3.Enqueue(Fz);
            }
        }
        private void chart1_Click(object sender, EventArgs e)
        {

        }
        DataTable dt = new DataTable();

        private void Form1_Load(object sender, EventArgs e)
        {
            InitChart();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("串口未打开，请检查!");
                return;
            }
            else
            {
                //设置1mm/s
                serialPort1.Write("1mm");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("串口未打开，请检查!");
                return;
            }
            else
            {
                //设置2mm/s
                serialPort1.Write("2mm");
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("串口未打开，请检查!");
                return;
            }
            else
            {
                //设置4mm/s
                serialPort1.Write("4mm");
            }
        }

        public void  load_CSV()
        {
            //文件流读取
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, Encoding.Default);

            string tempText = "";
            bool isFirst = true;
            while ((tempText = sr.ReadLine()) != null)
            {
                string[] arr = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                //一般第一行为标题，所以取出来作为标头
                if (isFirst)
                {
                    foreach (string str in arr)
                    {
                        dt.Columns.Add(str);
                    }
                    isFirst = false;
                }
                else
                {
                    //从第二行开始添加到datatable数据行
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dr[i] = i < arr.Length ? arr[i] : "";
                    }
                    dt.Rows.Add(dr);
                }

            }           
                sr.Close();
                fs.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "0";
            this.textBox2.Text = "0";
            this.textBox3.Text = "0";
            this.textBox4.Text = "0";
            this.textBox7.Text = "0";
            this.textBox6.Text = "0";
            this.textBox5.Text = "0";
            this.textBox8.Text = "0";
            this.textBox9.Text = null;
            this.textBox10.Text = null;
            this.textBox11.Text = null;
            this.textBox12.Text = null;
            this.textBox13.Text = null;
            this.textBox14.Text = null;
            timer1.Stop();
            timer_count = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("串口未打开，请检查!");
                return;
            }
            else
            {
                serialPort1.Write("stop");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                MessageBox.Show("串口已打开!");
            }
            else {
                serialPort1.PortName = comboBox1.Text.Trim();//串口名给了串口类
                serialPort1.BaudRate = 115200;
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;
                serialPort1.DataBits = 8;
                serialPort1.Encoding = Encoding.UTF8;
                serialPort1.Open();
            }
            

            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("串口未打开，请检查!");
                return;
            }
            else
            {
                serialPort1.Write("left");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("串口未打开，请检查!");
                return;
            }
            else
            {
                serialPort1.Write("right");
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            path = ofd.FileName;
        }
    }
}
