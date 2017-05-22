using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using zsbApps.DBHelper;

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string connstr = @"server=cc;database=ZrTBM;uid=sa;pwd=123456";
			this.addOneData(DBClassify.MsSql, connstr);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string connstr = @"Data Source=D:\Work\DB\DBFile\SQLite\temp.db;Version=3;";
			this.addOneData(DBClassify.SQLite, connstr);
		}

		private void addOneData(DBClassify db, string connstr)
		{
			DBHelper a = DBHelper.Instance(db, connstr);
			string exsql = $"INSERT INTO a(a)VALUES('{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
			int x = a.ExecuteSql(exsql);
			if (x == 1)
			{
				MessageBox.Show("OK");
			}
		}
	}
}
