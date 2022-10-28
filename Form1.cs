using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 传感_电机_电机控制程序;

namespace test_database__10__27
{
    public partial class Form1 : Form
    {
        
        private OleDbConnection thisConnection;
        private OleDbDataAdapter thisAdapter;
        private DataSet thisDataSet;
        private DataTable dt;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DateToAccess(); 
           
        }
        
        public void DateToAccess()
        {
            //【1】连接数据库
            string connect_str = "provider=Microsoft.Jet.OLEDB.4.0;Jet OLEDB:DataBase Password='';User Id='admin';Data source=C:\\Users\\admin_Jason\\Desktop\\test.mdb";
            thisConnection = new OleDbConnection(connect_str);
            //【2】编写SQL指令，星号（*）是选取所有列的快捷方式。
            string sql = "select * from test";
            //OleDbDataAdapter是 DataSet 和数据源之间的桥梁，用于检索和保存数据。
            thisAdapter = new OleDbDataAdapter(sql, thisConnection);
            //DataSet可以理解成在应用程序中的数据库
             thisDataSet = new System.Data.DataSet();
            //使用 Fill 将数据从数据源加载到 DataSet 中
            thisAdapter.Fill(thisDataSet, "test");
            //DataTable可以理解成DataSet的一个表格；将table中的表格内容添加到datatable
             dt = thisDataSet.Tables["test"];
            //将数据表和dataGridView1进行绑定
            dataGridView1.DataSource = dt;
            //关闭连接
            thisConnection.Close();



        }
        //添加行号
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //自动编号，与数据无关
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
             e.RowBounds.Location.Y,
                   dataGridView1.RowHeadersWidth - 4,
                e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics,
                  (e.RowIndex + 1).ToString(),
                   dataGridView1.RowHeadersDefaultCellStyle.Font,
                   rectangle,
                   dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                   TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        public string rockkind;
        public string rockstatus;
        public string rockattitude;
        public string toestatus;
        public string toeattitude;
        public string flexiblesheetinstallstatus;
        public string flexiblesheetangle;
        public string flexiblesheetstatus;
        public string flexiblesheetfixedstatus;
        public string material;
        //新行生成默认值
        private  void dataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            //获取当前时间的日期部分
            DateTime dateTime = DateTime.Now.Date;
            string time = DateTime.Now.ToString("hh:mm:ss");
            e.Row.Cells["日期"].Value = dateTime;
            e.Row.Cells["时间"].Value = time;
            e.Row.Cells["岩石种类"].Value = rockkind;
            e.Row.Cells["岩石状态"].Value = rockstatus;
            e.Row.Cells["岩石姿态"].Value = rockattitude;
            e.Row.Cells["足趾姿态"].Value = toeattitude;
            e.Row.Cells["足趾状态"].Value = toestatus;
            e.Row.Cells["柔性片安装状态"].Value = flexiblesheetinstallstatus;
            e.Row.Cells["柔性片安装角度"].Value = flexiblesheetangle;
            e.Row.Cells["柔性片状态"].Value = flexiblesheetstatus;
            e.Row.Cells["柔性片固定状态"].Value = flexiblesheetfixedstatus;
            e.Row.Cells["材料"].Value = material;   
        }
        #region 右键删除
        private int index = 0;
        
        private void dataGridView1_CellMouseUp_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.dataGridView1.Rows[e.RowIndex].Selected = true;//是否选中当前行
                index = e.RowIndex;
                this.dataGridView1.CurrentCell = this.dataGridView1.Rows[e.RowIndex].Cells[0];

                //每次选中行都刷新到datagridview中的活动单元格
                this.contextMenuStrip1.Show(this.dataGridView1, e.Location);
                //指定控件（DataGridView），指定位置（鼠标指定位置）
                this.contextMenuStrip1.Show(Cursor.Position);//锁定右键列表出现的位置
            }
        }
        private void 删除行ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (!this.dataGridView1.Rows[index].IsNewRow)//判断是否为新行
            {
                
                thisDataSet.Tables[0].Rows[dataGridView1.CurrentRow.Index].Delete();
                OleDbCommandBuilder cmdb = new OleDbCommandBuilder(thisAdapter);
                thisAdapter.Update(thisDataSet.Tables[0].GetChanges());
                MessageBox.Show("数据删除成功!");
                //Datatable 接受更改 以便下一次更改做准备
                thisDataSet.Tables[0].AcceptChanges();
                //this.dataGridView1.Rows.RemoveAt(index);//从集合中移除指定的行
                //MessageBox.Show("删除成功" + dele);
                //必须数据库先删除后在刷新Ui界面的数据，因为CurrentCell会因为UI界面刷新删除数据后发生单元格的变动， 
                //跳到下一行数据，从而导致数据库删除失败。
                //对于只删除一行的数据库，个人比较推荐Command

            }

        }
        

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            var dt = (DataTable)dataGridView1.DataSource;
            OleDbCommandBuilder cmdb = new OleDbCommandBuilder(thisAdapter);
            thisAdapter.Update(dt);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Main__Form form = new Main__Form();
            //form.Show();
            //this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Main__Form form = new Main__Form();
            //form.Show();
            //this.Close();
        }
    }
}
