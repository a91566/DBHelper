/*
 * 2017年5月25日 13:49:24 郑少宝
 * 
 * 我们一起量量
 * 一辈子有多长
 * 好不好
 */
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

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
					catch (SQLiteException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
		}

		/// <summary>
		/// 批量执行 sql 语句
		/// </summary>
		/// <param name="listSql">执行的 sql 语句集合</param>
		/// <returns>影响行数</returns>
		public override int ExecuteSql(List<string> listSql)
		{
			using (SQLiteConnection connection = new SQLiteConnection(this._connStr))
			{
				try
				{
					connection.Open();
					SQLiteTransaction tran = connection.BeginTransaction();
					SQLiteCommand cmd = new SQLiteCommand();
					cmd.Connection = connection;
					int result = 0;
					foreach (var item in listSql)
					{
						cmd.CommandText = item;
						int x = cmd.ExecuteNonQuery();
						result += x >= 0 ? x : 0;
					}
					tran.Commit();
					return result;
				}
				catch (SQLiteException e)
				{
					connection.Close();
					throw e;
				}
			}
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns>DataSet</returns>
		public override DataSet Query(string sql)
		{
			return null;
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns>DataTable</returns>
		public override DataTable QueryTable(string sql)
		{
			return null;
		}
	}
}
