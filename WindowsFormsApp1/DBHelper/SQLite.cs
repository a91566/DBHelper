﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zsbApps.DBHelper
{
	public class SQLite : DBHelper
	{
		/// <summary>
		/// 连接字符串
		/// </summary>
		private string _connStr;

		public SQLite(string connstr)
		{
			this._connStr = connstr;
		}

		/// <summary>
		/// Shortcut to ExecuteNonQuery with SqlStatement and object[] param values
		/// </summary>
		/// <param name="exsql">exsql</param>
		/// <returns></returns>
		public override int ExecuteSql(string exsql)
		{
			using (SQLiteConnection connection = new SQLiteConnection(this._connStr))
			{
				using (SQLiteCommand cmd = new SQLiteCommand(exsql, connection))
				{
					try
					{
						connection.Open();
						int rows = cmd.ExecuteNonQuery();
						return rows;
					}
					catch (System.Data.SqlClient.SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
		}
	}
}