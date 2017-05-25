/*
 * 2017年5月25日 13:48:29 郑少宝
 * 
 * 在这个什么都善变的人间
 * 我想和你
 * 看一看永远
 * 
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace zsbApps.DBHelper
{
	public class MsSql : DBHelper
	{
		/// <summary>
		/// 连接字符串
		/// </summary>
		private string _connStr;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="connStr">连接字符串</param>
		public MsSql(string connStr)
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
			using (SqlConnection connection = new SqlConnection(this._connStr))
			{
				using (SqlCommand cmd = new SqlCommand(exsql, connection))
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

		/// <summary>
		/// 批量执行 sql 语句
		/// </summary>
		/// <param name="listSql">执行的 sql 语句集合</param>
		/// <returns>影响行数</returns>
		public override int ExecuteSql(List<string> listSql)
		{
			using (SqlConnection connection = new SqlConnection(this._connStr))
			{
				try
				{ 
					connection.Open();
					SqlTransaction tran = connection.BeginTransaction();
					SqlCommand cmd = new SqlCommand();
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
				catch (SqlException e)
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

			using (SqlConnection connection = new SqlConnection(this._connStr))
			{
				try
				{
					connection.Open();
					new SqlDataAdapter(sql, connection).Fill(result);
					return result;
				}
				catch (SqlException ex)
				{
					throw new Exception(ex.Message);
				}
			}
		}
		#endregion
	}
}
