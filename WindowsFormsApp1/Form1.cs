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
		public struct DB
		{
			public DBHelper MsSql;
			public DBHelper MySql;
			public DBHelper Sqlite;
		}
		private DB db;

		public Form1()
		{
			InitializeComponent();
			this.db = new DB();

			string connstr = @"server=localhost;database=test170525;uid=sa;pwd=1234567";
			db.MsSql = DBHelper.Instance(DBClassify.MsSql, connstr);

			connstr = @"host=localhost;database=test170525;uid=root;pwd=a91566;Port=3306";
			connstr = @"host=localhost;database=test170525;uid=root;pwd=123456;Port=3306";
			db.MySql = DBHelper.Instance(DBClassify.MySql, connstr);

			connstr = $@"Data Source={ Application.StartupPath }\temp.sqlite3;Version=3;";
			db.Sqlite = DBHelper.Instance(DBClassify.SQLite, connstr);
		}

		public DBHelper getInstanceIndex()
		{
			if (radioButton1.Checked)
			{
				return db.MsSql;
			}
			else if (radioButton2.Checked)
			{
				return db.Sqlite;
			}
			else if (radioButton3.Checked)
			{
				return db.MySql;
			}
			else
			{
				return null;
			}
		}

		private void addOneData(DBHelper a)
		{
			this.Text = a.ToString();
			string exsql = $"INSERT INTO a(a)VALUES('{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
			int x = a.ExecuteSql(exsql);
			if (x == 1)
			{
				MessageBox.Show($"{a.ToString()},OK");
			}
			else
			{
				MessageBox.Show($"{a.ToString()},false");
			}
		}

		private void addDataList(DBHelper a, bool isTran)
		{
			this.Text = a.ToString();
			List<string> list = new List<string>();
			for (int i = 0; i < 10; i++)
			{
				list.Add($"INSERT INTO a(a)VALUES('{System.DateTime.Now.AddHours(i).ToString("yyyy-MM-dd HH:mm:ss")}')");
			}
			//这句可以执行成功，但是被阶段，前20（数据库长度设置）
			list.Add($"INSERT INTO a(a)VALUES('111111{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ddd")}')");
			if (isTran)
			{
				int x = a.ExecuteTran(list);
				MessageBox.Show($"{a.ToString()},影响行数:{x}");
			}
			else
			{
				var x = a.ExecuteSql(list);
				MessageBox.Show($"{a.ToString()},影响行数:{x.count},错误信息：{x.error}");
			}
		}

		private void getSingle(DBHelper a)
		{
			this.Text = a.ToString();
			string sql;
			if ((a as IDBClassify).GetClassify() == DBClassify.MsSql)
			{
				sql = "SELECT TOP 1 a FROM a ORDER BY id DESC";
			}
			else
			{
				sql = "SELECT a FROM a ORDER BY id DESC limit 0,1;";
			}
			object x = a.GetSingle(sql);
			if (x == null)
			{
				MessageBox.Show($"{a.ToString()},is null");
			}
			else
			{
				MessageBox.Show($"{a.ToString()},{x.ToString()}");
			}
		}

		private void queryTable(DBHelper a)
		{
			this.Text = a.ToString();
			string sql = $"SELECT * FROM a ORDER BY CreateDateTime DESC";
			this.dataGridView1.DataSource = a.QueryTable(sql);
		}

		private void button1_Click(object sender, EventArgs e)
		{			
			addOneData(getInstanceIndex());
		}

		

		private void button4_Click(object sender, EventArgs e)
		{
			addDataList(getInstanceIndex(), false);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			getSingle(getInstanceIndex());
		}

		private void button6_Click(object sender, EventArgs e)
		{
			queryTable(getInstanceIndex());
		}

		private void button2_Click(object sender, EventArgs e)
		{
			addDataList(getInstanceIndex(), true);
		}
	}
}
