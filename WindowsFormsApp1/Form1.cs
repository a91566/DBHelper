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
			string connstr = $@"Data Source={ Application.StartupPath }\temp.sqlite3;Version=3;";
			this.addDataList(DBClassify.SQLite, connstr);
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

		private void addDataList(DBClassify db, string connstr)
		{
			DBHelper a = DBHelper.Instance(db, connstr);
			List<string> list = new List<string>();
			for (int i = 0; i < 10; i++)
			{
				list.Add($"INSERT INTO a(a)VALUES('{System.DateTime.Now.AddHours(i).ToString("yyyy-MM-dd HH:mm:ss")}')");
			}
			//这句可以执行成功，但是被阶段，前20（数据库长度设置）
			list.Add($"INSERT INTO a(a)VALUES('111111{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ddd")}')");
			//这一句异常，程序抛出错误
			//list.Add($"INSERT INTO a(a,b)VALUES('{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
			int x = a.ExecuteSql(list);
			MessageBox.Show($"影响行数:{x}");
		}

		private void button3_Click(object sender, EventArgs e)
		{
			string connstr = @"host=localhost;database=test170525;uid=root;pwd=123456;Port=3306";
			connstr = @"host=localhost;database=test170525;uid=root;pwd=a91566;Port=3306";
			this.addDataList(DBClassify.MySql, connstr);
		}
	}
}
