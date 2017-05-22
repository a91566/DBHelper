using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
