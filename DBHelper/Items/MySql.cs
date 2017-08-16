/*
 * 2017年5月25日 13:48:59 郑少宝
 * 
 * 比起天黑天塌
 * 我更害怕
 * 你皱眉
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;


namespace zsbApps.DBHelper
{
	public class MySql : DBHelper, IDBClassify
	{
		/// <summary>
		/// 连接字符串
		/// </summary>
		private string _connStr;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="connStr">连接字符串</param>
		public MySql(string connStr)
		{
			this._connStr = connStr;
		}

		/// <summary>
		/// 执行简单的 sql 语句
		/// </summary>
		/// <param name="exsql">执行的 sql 语句</param>
		/// <returns>影响行数</returns>
		public override int ExecuteSql(string exsql)
		{
			using (MySqlConnection connection = new MySqlConnection(this._connStr))
			{
				using (MySqlCommand cmd = new MySqlCommand(exsql, connection))
				{
					try
					{
						connection.Open();
						int rows = cmd.ExecuteNonQuery();
						return rows;
					}
					catch (MySqlException e)
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
		public override (int count, string error) ExecuteSql(List<string> listSql)
		{
			using (MySqlConnection connection = new MySqlConnection(this._connStr))
			{
				try
				{
					connection.Open();
					MySqlTransaction tran = connection.BeginTransaction();
					MySqlCommand cmd = new MySqlCommand();
					cmd.Connection = connection;
					int result = 0;
					foreach (var item in listSql)
					{
						cmd.CommandText = item;
						int x = cmd.ExecuteNonQuery();
						result += x >= 0 ? x : 0;
					}
					tran.Commit();
					return (count: result, error: null);
				}
				catch (MySqlException e)
				{
					connection.Close();
					throw e;
				}
			}
		}

		/// <summary>
		/// 事务执行 sql 语句
		/// </summary>
		/// <param name="listSql">执行的 sql 语句集合</param>
		/// <returns>影响行数</returns>
		public override int ExecuteTran(List<string> listSql)
		{
			throw new Exception("Not Implemented");
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns>DataSet</returns>
		public override DataSet Query(string sql)
		{
			return fill(sql, 0);
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns>DataTable</returns>
		public override DataTable QueryTable(string sql)
		{
			return fill(sql, 1);
		}

		/// <summary>
		/// 返回第一行第一列 返回类型为 object
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns>返回第一行第一列</returns>
		public override object GetSingle(string sql)
		{
			using (MySqlConnection connection = new MySqlConnection(this._connStr))
			{
				using (MySqlCommand cmd = new MySqlCommand(sql, connection))
				{
					try
					{
						connection.Open();
						return cmd.ExecuteScalar();
					}
					catch (MySqlException ex)
					{
						connection.Close();
						throw ex;
					}
				}
			}
		}

		#region 私有函数
		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <param name="modal">查询模式（0 DataSet，1 DataTable）</param>
		/// <returns></returns>
		private dynamic fill(string sql, int modal)
		{
			dynamic result;
			switch (modal)
			{
				case 0:
					result = new DataSet();
					break;
				case 1:
					result = new DataTable();
					break;
				default:
					return null;
			}
			using (MySqlConnection connection = new MySqlConnection(this._connStr))
			{
				try
				{
					connection.Open();
					new MySqlDataAdapter(sql, connection).Fill(result);
					return result;
				}
				catch (MySqlException ex)
				{
					throw new Exception(ex.Message);
				}
			}			
		}
		#endregion

		#region IDBClassify 接口实现
		/// <summary>
		/// 获取类型
		/// </summary>
		/// <returns>数据库类型</returns>
		DBClassify IDBClassify.GetClassify()
		{
			return DBClassify.MySql;
		}
		#endregion
	}
}
