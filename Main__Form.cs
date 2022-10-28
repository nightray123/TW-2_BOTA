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
using System.Configuration;
using System.IO.Ports;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;//chartarea
using test_database__10__27;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace 传感_电机_电机控制程序
{
    public  partial class Main__Form : Form
    {

        //定义一个中间串口名称，用于拔插实时更新
        String serialPortName1;
        String serialPortName2;
        String serialPortName3;

        public Main__Form()
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
            timer2.Start();
        }

        #region  坐标转换
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
        #endregion
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            compute();
    }
        private void button1_Click(object sender, EventArgs e)
        {
            serialPort2.Close();
            serialPort3.Close();
            button9.Enabled = true;
            textBox6.Text = "0";
            textBox7.Text = "0";
            textBox8.Text = "0";
            textBox15.Text = "0";
        }
        #region 初始化图表
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
            Series series4 = new Series("拉力");
            series1.ChartArea = "C1";
            series2.ChartArea = "C1";
            series3.ChartArea = "C1";
            series4.ChartArea = "C1";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Series.Add(series4);
            //设置图表显示样式
            //this.chart1.ChartAreas[0].AxisY.Minimum = -10;
            //this.chart1.ChartAreas[0].AxisY.Maximum = 10;
            this.chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;
            this.chart1.ChartAreas[0].AxisX.IsStartedFromZero = false;
            this.chart1.ChartAreas[0].AxisX.Interval = 50;
            this.chart1.ChartAreas[0].AxisY.Interval = 5;
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            //网格线设置为虚线
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
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

            this.chart1.Series[3].Color = Color.Black;
            this.chart1.Series[3].ChartType = SeriesChartType.Line;
            this.chart1.Series[3].Points.Clear();
        }
        

        //鼠标悬停显示曲线点的值
        void chart_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                int i = e.HitTestResult.PointIndex;
                DataPoint dp = e.HitTestResult.Series.Points[i];
                e.Text = string.Format("时间:{0};力值:{1:F5}", dp.XValue, dp.YValues[0]);
            }
        }
        #endregion
        #region 对图表数据进行更新 形成流动效果
        private Queue<double> dataQueue1 = new Queue<double>(2000);
        private Queue<double> dataQueue2 = new Queue<double>(2000);
        private Queue<double> dataQueue3 = new Queue<double>(2000);
        private Queue<double> dataQueue4 = new Queue<double>(2000);
        private int num = 5;//每次删除增加几个点
        private void UpdateQueueValue()//更新数据 通过队列出列进行流动显示
        {
            if (dataQueue1.Count > 2000)
            {
                //先出列
                for (int i = 0; i < num; i++)
                {
                    dataQueue1.Dequeue();
                    dataQueue2.Dequeue();
                    dataQueue3.Dequeue();
                    dataQueue4.Dequeue();

                }
            }
            for (int i = 0; i < num; i++)
                {
                    double Fx = double.Parse(textBox6.Text);
                    double Fy = double.Parse(textBox7.Text);
                    double Fz = double.Parse(textBox8.Text);
                    double A3 = double.Parse(textBox15.Text);
                    dataQueue1.Enqueue(Fx);
                    dataQueue2.Enqueue(Fy);
                    dataQueue3.Enqueue(Fz);
                    dataQueue4.Enqueue(A3);

                }

            
        }
        #endregion
        private void chart1_Click(object sender, EventArgs e)
        {

        }
        #region 窗体初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            //设置默认下拉框
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            //comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
            comboBox9.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox10.SelectedIndex = 0;
            comboBox11.SelectedIndex = 0;
            comboBox12.SelectedIndex = 0;
            comboBox16.SelectedIndex = 0;
            comboBox15.SelectedIndex = 0;
            InitChart();
            //chart_trace_GetToolTipTex();
            chart1.GetToolTipText += new EventHandler<ToolTipEventArgs>(chart_GetToolTipText);
            //chart控件的鼠标悬停显示时间

            //获取电脑上可用串口号
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            //给comboBox1添加数据
            comboBox1.Items.AddRange(ports);
            //如果里面有数据，显示第0个
            comboBox1.SelectedIndex = comboBox1.Items.Count > 0 ? 0 : -1;
            //给comboBox13添加数据
            comboBox13.Items.AddRange(ports);
            //如果里面有数据，显示第0个
            comboBox13.SelectedIndex = comboBox13.Items.Count > 0 ? 0 : -1;
            //给comboBox14添加数据
            comboBox14.Items.AddRange(ports);
            //如果里面有数据，显示第0个
            comboBox14.SelectedIndex = comboBox14.Items.Count > 0 ? 0 : -1;
            //此时把所有的控件合法性线程检查全部都给禁止掉了
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            button1.Enabled = false;
            button17.Enabled = false;
            button13.Enabled = false;

        }
        #endregion
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
                speed = 1;
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
                speed = 2;
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
                serialPort1.Write("8mm");
                speed = 8;
            }
        }

        public DataTable createDatatable() {
            DataTable dt = new DataTable();
            //新的datatable 用来存储解算后的力
            DataColumn Time = new DataColumn("时间", typeof(DateTime));
            DataColumn Oder = new DataColumn("序号", typeof(int));
            DataColumn Fx1 = new DataColumn("Fx1", typeof(double));
            DataColumn Fy1 = new DataColumn("Fy1", typeof(double));
            DataColumn Fz1 = new DataColumn("Fz1", typeof(double));
            DataColumn AIN3 = new DataColumn("AIN3", typeof(double));
            dt.Columns.Add(Time);
            dt.Columns.Add(Oder);
            dt.Columns.Add(Fx1);
            dt.Columns.Add(Fy1);
            dt.Columns.Add(Fz1);
            dt.Columns.Add(AIN3);
            return dt;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if ((comboBox2.Text != "") && (comboBox3.Text != "") && (comboBox4.Text != "")  && (comboBox8.Text != "") && (comboBox9.Text != "") && (comboBox6.Text != "") && (comboBox10.Text != ""))
            {
                saveFileDialog1.Filter = "CSV文件|*.CSV";
                saveFileDialog1.FileName = comboBox2.Text + "_" + comboBox3.Text + "_" + comboBox4.Text + "_" + comboBox8.Text + "_" + comboBox16.Text + "_"  + comboBox10.Text + comboBox6.Text + comboBox11.Text + "_" + comboBox15.Text + "_" + "_" + comboBox9.Text + "_" + "_" + DateTime.Now.ToString("yyyy年MM月dd HH时mm分ss秒");
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    string fileName = saveFileDialog1.FileName;

                    SaveCsv(dt, fileName);
                }
            }
            else {


                MessageBox.Show("请选择工况！");
            }
            timer1.Enabled = false;
            count_csv = 0;
        }
        #region     保存CSV文件

        public static void SaveCsv(DataTable dt, string filePath)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(filePath + dt.TableName + ".csv", FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.Default);
                var data = string.Empty;
                //写出列名称
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    data += dt.Columns[i].ColumnName;
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                //写出各行数据
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    data = string.Empty;
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        data += dt.Rows[i][j].ToString();
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }

                    }
                    sw.WriteLine(data);
                }

            }
            catch (IOException ex)
            {

                throw new IOException(ex.Message, ex);
            }

            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }
        #endregion

        int count2 = 0;//用来进行自动停止计数；
        int distance = 0;//计算路程
        int speed = 0;//记录当前速度
        private void timer2_Tick(object sender, EventArgs e)
        {
            count2++;
            //textBox2.Text = count2.ToString();
            distance = count2 * speed;
            //textBox1.Text = distance.ToString();
            if (distance >= 25) {
                serialPort1.Write("stop");
                timer2.Stop();
                // speed = 0;
                distance = 0;
                count2 = 0;
            }
        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {


        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
        }
        #region 处理热拔插

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0219)
            {//设备改变
                if (m.WParam.ToInt32() == 0x8004)
                {//usb串口拔出
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames();//重新获取串口
                    comboBox13.Items.Clear();//清除comboBox里面的数据
                    comboBox14.Items.AddRange(ports);//给comboBox1添加数据
                    comboBox14.Items.Clear();//清除comboBox里面的数据
                    comboBox14.Items.AddRange(ports);//给comboBox1添加数据
                    if (serial_open_flag == true)
                    {//用户打开过串口
                        if (!serialPort2.IsOpen&&!serialPort3.IsOpen)
                        {//用户打开的串口被关闭:说明热插拔是用户打开的串口
                            serial_open_flag = false;
                            serialPort3.Dispose();//释放掉原先的串口资源
                            comboBox14.SelectedIndex = comboBox13.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                            serialPort2.Dispose();//释放掉原先的串口资源
                            comboBox13.SelectedIndex = comboBox13.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                        }
                        else
                        {

                            comboBox13.Text = serialPortName2;//显示用户打开的那个串口号
                            comboBox14.Text = serialPortName2;//显示用户打开的那个串口号
                        }
                    }
                    else
                    {//用户没有打开过串口
                        comboBox14.SelectedIndex = comboBox13.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                        comboBox13.SelectedIndex = comboBox13.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                    }
                }
                else if (m.WParam.ToInt32() == 0x8000)
                {//usb串口连接上
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames();//重新获取串口
                    comboBox13.Items.Clear();
                    comboBox13.Items.AddRange(ports);
                    comboBox14.Items.Clear();
                    comboBox14.Items.AddRange(ports);
                    if (serial_open_flag == true)
                    {//用户打开过一个串口

                        comboBox13.Text = serialPortName2;//显示用户打开的那个串口号
                        comboBox14.Text = serialPortName3;//显示用户打开的那个串口号
                    }
                    else
                    {
                        comboBox13.SelectedIndex = comboBox13.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                        comboBox14.SelectedIndex = comboBox14.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                    }
                }
            }
            if (m.Msg == 0x0219)
            {//设备改变
                if (m.WParam.ToInt32() == 0x8004)
                {//usb串口拔出
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames();//重新获取串口
                    comboBox1.Items.Clear();//清除comboBox里面的数据
                    comboBox1.Items.AddRange(ports);//给comboBox1添加数据
                    if (serial_open_flag2 == true)
                    {//用户打开过串口
                        if (!serialPort1.IsOpen)
                        {//用户打开的串口被关闭:说明热插拔是用户打开的串口
                            serial_open_flag2 = false;
                            serialPort1.Dispose();//释放掉原先的串口资源
                            comboBox1.SelectedIndex = comboBox1.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                        }
                        else
                        {
                            comboBox1.Text = serialPortName3;//显示用户打开的那个串口号
                        }
                    }
                    else
                    {//用户没有打开过串口
                        comboBox1.SelectedIndex = comboBox1.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                    }
                }
                else if (m.WParam.ToInt32() == 0x8000)
                {//usb串口连接上
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames();//重新获取串口
                    comboBox1.Items.Clear();
                    comboBox1.Items.AddRange(ports);
                    if (serial_open_flag2 == true)
                    {//用户打开过一个串口
                        comboBox1.Text = serialPortName3;//显示用户打开的那个串口号
                    }
                    else
                    {
                        comboBox1.SelectedIndex = comboBox1.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                    }
                }
            }
            base.WndProc(ref m);
        }

        #endregion





        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }
        double t0 = 0, t1 = 0, t2 = 0, t3 = 0;

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
            //f1 实时拉力 f2目标拉力
            double f1, f2;
            f1 = Convert.ToDouble(textBox15.Text);
            f2 = Convert.ToDouble(comboBox12.Text);

            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("串口未打开，请检查!");
                return;
            }
            else
            {
                if (f1 <= (f2 - 0.3))
                {
                    serialPort1.Write("127");
                    timer4.Enabled = true;
                }
                else {
                    serialPort1.Write("stop");
                }


            }

        }

        private void button15_Click(object sender, EventArgs e)
        {
            //f1 实时拉力 目标拉力0
            double f1;
            f1 = Convert.ToDouble(textBox15.Text);
            //f2 = Convert.ToDouble(comboBox12.Text);
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("串口未打开，请检查!");
                return;
            }
            else
            {
                if (f1 > 0)
                {
                    //serialPort1.Write("left");
                    serialPort1.Write("127");
                    timer5.Enabled = true;
                }
                else
                {
                    serialPort1.Write("stop");
                }

            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            double f1, f2;
            f1 = Convert.ToDouble(textBox15.Text);
            f2 = Convert.ToDouble(comboBox12.Text);
            if (f1 > f2)
            {
                serialPort1.Write("stop");
                timer4.Enabled = false;
            }


        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            double f1;
            f1 = Convert.ToDouble(textBox15.Text);
            //f2 = Convert.ToDouble(comboBox12.Text);
            if (f1 <= 0.01)
            {
                serialPort1.Write("stop");
                timer5.Enabled = false;
            }
        }
        //全局变量 用来存储每次试验的传感器数据

        DataTable dt = new DataTable();
        private void button17_Click(object sender, EventArgs e)
        {
            //开始Timer1
            timer1.Enabled = true;
            dt = createDatatable();
            button13.Enabled = true;
        }
        public string rockkind = "";
        public string rockstatus = "";
        public string rockattitude = "";
        public string toestatus = "";
        public string toeattitude = "";
        public string flexiblesheetinstallstatus ="";
        public string flexiblesheetangle = "";
        public string flexiblesheetstatus = "";
        public string flexiblesheetfixedstatus = "";
        public string material = "";
        private void button16_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            rockkind = comboBox2.Text;
            rockstatus = comboBox3.Text;
            rockattitude = comboBox4.Text;
            toeattitude = comboBox8.Text;
            toestatus = comboBox16.Text; 
            flexiblesheetinstallstatus = comboBox10.Text;
            flexiblesheetangle = comboBox6.Text;
            flexiblesheetfixedstatus = comboBox11.Text;
            flexiblesheetstatus = comboBox15.Text;
            material = comboBox9.Text;
            form.rockkind = this.rockkind;
            form.rockstatus = this.rockstatus;
            form.rockattitude = this.rockattitude;
            form.toestatus = this.toestatus;
            form.toeattitude= this.toeattitude;
            form.flexiblesheetinstallstatus = this.flexiblesheetinstallstatus;
            form.flexiblesheetangle = this.flexiblesheetangle;
            form.flexiblesheetfixedstatus= this.flexiblesheetfixedstatus;
            form.flexiblesheetstatus= this.flexiblesheetstatus;
            form.material = this.material;
            form.Show();
            //this.Hide();
        }

        #region 控件截图

        private Bitmap ScreenshotControl(Control control)
        {
            Bitmap bmp = new Bitmap(control.Width, control.Height);
            control.DrawToBitmap(bmp, new Rectangle(0, 0, control.Width, control.Height));
            return bmp;
        }
 
#endregion

        private void button18_Click(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            Bitmap bitmap = ScreenshotControl(chart1);
            saveFileDialog3.Filter = "图片文件(*.jpg,*.gif,*.png)|*.jpg,*.gif,*.png"; 
            saveFileDialog3.FileName = "波形图"+ comboBox2.Text + "_" + comboBox3.Text + "_" + comboBox4.Text + "_" + comboBox8.Text + "_" + comboBox16.Text + "_" + comboBox10.Text + comboBox6.Text + comboBox11.Text + "_" + comboBox15.Text + "_" + "_" + comboBox9.Text + "_" + time.ToString("yyyy年MM月dd HH时mm分ss秒");
            if (saveFileDialog3.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                string fileName = saveFileDialog3.FileName;

                bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            
            
        }

        private void saveFileDialog3_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            UpdateQueueValue();

            this.chart1.Series[0].Points.Clear();
            this.chart1.Series[1].Points.Clear();
            this.chart1.Series[2].Points.Clear();
            this.chart1.Series[3].Points.Clear();

            for (int i = 0; i < dataQueue1.Count; i++)
            {
                this.chart1.Series[0].Points.AddXY((i + 1), dataQueue1.ElementAt(i));
                this.chart1.Series[1].Points.AddXY((i + 1), dataQueue2.ElementAt(i));
                this.chart1.Series[2].Points.AddXY((i + 1), dataQueue3.ElementAt(i));
                this.chart1.Series[3].Points.AddXY((i + 1), dataQueue4.ElementAt(i));
            }
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            if (serialPort3.IsOpen) {
                String Str = "FE040000000125C5";
                byte[] byt = strToHexByte(Str);
                serialPort3.Write(byt, 0, byt.Length);
            }
            
        }
        #region <字符串转16进制格式,不够自动前面补零>
        /// <字符串转16进制格式,不够自动前面补零>
        /// 
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] strToHexByte(String hexString)
        {
            int i;
            hexString = hexString.Replace(" ", "");//清除空格
            if ((hexString.Length % 2) != 0)//奇数个
            {
                byte[] returnBytes = new byte[(hexString.Length + 1) / 2];
                try
                {
                    for (i = 0; i < (hexString.Length - 1) / 2; i++)
                    {
                        returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    }
                    returnBytes[returnBytes.Length - 1] = Convert.ToByte(hexString.Substring(hexString.Length - 1, 1).PadLeft(2, '0'), 16);
                }
                catch
                {
                    MessageBox.Show("含有非16进制字符", "提示");
                    return null;
                }
                return returnBytes;
            }
            else
            {
                byte[] returnBytes = new byte[(hexString.Length) / 2];
                try
                {
                    for (i = 0; i < returnBytes.Length; i++)
                    {
                        returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    }
                }
                catch
                {
                    MessageBox.Show("含有非16进制字符", "提示");
                    return null;
                }
                return returnBytes;
            }
        }
        #endregion
        int z;
        private void serialPort3_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buff = new byte[7];//创建缓存数据数组
            serialPort3.Read(buff, 0, 7);
            if (buff[0]== 0xFE && buff[1] == 0x04 && buff[2] == 0x02)//&&
            {
                byte[] buff2 = new byte[2];
                buff2[0] = buff[3];
                buff2[1] = buff[4];
                //字节转换为10进制有符号数
                string y = byteToHexStr(buff2);
                z = Convert.ToInt16(y, 16);
                textBox15.Text = (t3 - (z * 0.01)).ToString("0.00");
            }
           
        }
        #region 16进制格式转字符串
        /// <字节数组转16进制字符串>
        /// <param name="bytes"></param>
        /// <returns> String 16进制显示形式</returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            try
            {
                if (bytes != null)
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        returnStr += bytes[i].ToString("X2");
                    }
                }
                //添加自动换行
                return returnStr;
            }
            catch (Exception)
            {
                return returnStr;
            }
        }
        #endregion
        byte[] Fx1 = new byte[4];
        byte[] Fy1 = new byte[4];
        byte[] Fz1 = new byte[4];
        private void serialPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort2.IsOpen) {
                byte[] Tx = new byte[4];
                byte[] Ty = new byte[4];
                byte[] Tz = new byte[4];
                byte[] TimeStamp = new byte[4];
                byte[] Temprature = new byte[4];
                byte[] CRC16 = new byte[2];

                int len = serialPort2.BytesToRead;//获取可以读取的字节数          
                byte[] buff = new byte[len];//创建缓存数据数组           
                serialPort2.Read(buff, 0, len);

                //获取帧            
                if (len >= 37) { 
                  if (buff[0] == 0xAA && buff[1] == 0x00 && buff[2] == 0x00)
                    {
                        //Fx
                        Fx1[0] = buff[3];
                        Fx1[1] = buff[4];
                        Fx1[2] = buff[5];
                        Fx1[3] = buff[6];
                        //Fy
                        Fy1[0] = buff[7];
                        Fy1[1] = buff[8];
                        Fy1[2] = buff[9];
                        Fy1[3] = buff[10];
                        //Fz
                        Fz1[0] = buff[11];
                        Fz1[1] = buff[12];
                        Fz1[2] = buff[13];
                        Fz1[3] = buff[14];
                        ////Tx
                        //Tx[0] = buff[15];
                        //Tx[1] = buff[16];
                        //Tx[2] = buff[17];
                        //Tx[3] = buff[18];
                        ////Ty
                        //Ty[0] = buff[9];
                        //Ty[1] = buff[20];
                        //Ty[2] = buff[21];
                        //Ty[3] = buff[22];
                        ////Tz
                        //Tz[0] = buff[23];
                        //Tz[1] = buff[24];
                        //Tz[2] = buff[25];
                        //Tz[3] = buff[26];
                        ////timeStamp
                        //TimeStamp[0] = buff[27];
                        //TimeStamp[1] = buff[28];
                        //TimeStamp[2] = buff[29];
                        //TimeStamp[3] = buff[30];
                        ////tempetura
                        //Temprature[0] = buff[31];
                        //Temprature[1] = buff[32];
                        //Temprature[2] = buff[33];
                        //Temprature[3] = buff[4];
                        ////CRC
                        //CRC16[0] = buff[35];
                        //CRC16[1] = buff[36];
                        textBox6.Text = (BitConverter.ToSingle(Fx1, 0) - t0).ToString("0.00");
                        textBox7.Text = (BitConverter.ToSingle(Fy1, 0) - t1).ToString("0.00");
                        textBox8.Text = (t2-BitConverter.ToSingle(Fz1, 0)).ToString("0.00");
                        //textBox4.Text = BitConverter.ToSingle(Tx, 0).ToString();
                        //textBox5.Text = BitConverter.ToSingle(Ty, 0).ToString();
                        //textBox6.Text = BitConverter.ToSingle(Tz, 0).ToString();
                        //textBox7.Text = BitConverter.ToSingle(TimeStamp, 0).ToString();
                        //textBox9.Text = BitConverter.ToSingle(Temprature, 0).ToString();
                    }
                }
            }
        }

        //记录CSV文件的行数
        int count_csv = 0;

        private void button19_Click(object sender, EventArgs e)
        {
           
        }

        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //timer1用来输出CSV文件 interval 100  0.1s采集一次
        private void timer1_Tick(object sender, EventArgs e)
        {
            count_csv++;
            DateTime time = DateTime.Now;
            //更新datatable           
            dt.Rows.Add(time, count_csv.ToString(), textBox12.Text, textBox13.Text, textBox14.Text, textBox15.Text);
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            
            t0 = BitConverter.ToSingle(Fx1, 0);
            t1 = BitConverter.ToSingle(Fy1, 0);
            t2 = BitConverter.ToSingle(Fz1, 0);
            t3 = (z * 0.01);

            this.textBox1.Text = "0";
            this.textBox2.Text = "0";
            this.textBox3.Text = "0";
            this.textBox4.Text = "0";
            this.textBox5.Text = "0";
            this.textBox9.Text = null;
            this.textBox10.Text = null;
            this.textBox11.Text = null;
            this.textBox12.Text = "0";
            this.textBox13.Text = "0";
            this.textBox14.Text = "0";

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

            distance = 0;
            count2 = 0;
            timer2.Stop();
        }
        bool serial_open_flag2 = false;
        private void button7_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                MessageBox.Show("串口已打开!");
            }
            else if (serialPort1.PortName == null) {
                MessageBox.Show("请正确选择串口号");
            }
            else {
                serialPort1.PortName = comboBox1.Text.Trim();//串口名给了串口类
                serialPortName1 = comboBox1.Text;   
                serialPort1.BaudRate = 115200;
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;
                serialPort1.DataBits = 8;
                serialPort1.Encoding = Encoding.UTF8;
                serialPort1.Open();
                
                
                serial_open_flag2 = true;
            }



        }

        private void button8_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            serial_open_flag2 = false;
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

        bool serial_open_flag = false;

        public void button9_Click(object sender, EventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                MessageBox.Show("串口已打开!");
            }
            else if (serialPort2.PortName == null)
            {
                MessageBox.Show("请正确选择串口号");
            }
            else
            {
                serialPort2.PortName = comboBox13.Text;//串口名给了串口类
                serialPortName2 = comboBox13.Text;  
                serialPort2.BaudRate = 115200;
                serialPort2.Parity = Parity.None;
                serialPort2.StopBits = StopBits.One;
                serialPort2.DataBits = 8;
                serialPort2.Open();
                serialPort2.ReceivedBytesThreshold = 37;
                serialPort2.DataReceived += new SerialDataReceivedEventHandler(serialPort2_DataReceived);
            }
            if (serialPort3.IsOpen)
            {
                MessageBox.Show("串口已打开!");
            }
            else if (serialPort3.PortName == null)
            {
                MessageBox.Show("请正确选择串口号");
            }
            else
            {
                serialPort3.PortName = comboBox14.Text;//串口名给了串口类
                serialPortName3 = comboBox14.Text;
                serialPort3.BaudRate = 9600;
                serialPort3.Parity = Parity.None;
                serialPort3.StopBits = StopBits.One;
                serialPort3.DataBits = 8;
                serialPort3.Open();
                //serialPort3.ReceivedBytesThreshold = 7;
                //serialPort3.DataReceived += new SerialDataReceivedEventHandler(serialPort3_DataReceived);
            }
            timer3.Start();
            timer6.Start();
            button9.Enabled = false;
            button17.Enabled = true;
            button1.Enabled = true;
            //串口2,3打开标志位
            serial_open_flag = true;
            //Running = 1;
        }
    }

}
