using System.Data;

namespace zsbApps.DBHelper
{
	public abstract class DBHelper
	{
		/// <summary>
		/// 创建一个数据访问实例
		/// </summary>
		/// <param name="dbclassify">数据库类型</param>
		/// <param name="connstr">连接字符串</param>
		/// <returns></returns>
		public static DBHelper Instance(DBClassify dbclassify, string connstr)
		{
			switch (dbclassify)
			{
				case DBClassify.MsSql:
					return new MsSql(connstr);
				case DBClassify.SQLite:
					return new SQLite(connstr);
				default:
					return null;
			}
		}

		/// <summary>
		/// 执行简单的 sql 语句
		/// </summary>
		/// <param name="exsql">执行的 sql 语句</param>
		/// <returns>影响行数</returns>
		public abstract int ExecuteSql(string exsql);

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns>DataSet</returns>
		public abstract DataSet Query(string sql);

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns>DataTable</returns>
		public abstract DataTable QueryTable(string sql);
	}
}
