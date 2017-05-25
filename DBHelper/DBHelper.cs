/*
 * 2017年5月25日 13:46:47 郑少宝
 * 
 * 
 * 备注
 * 1.长度不够的话会被截断，但是还是会执行成功。
 * 
 * 各个数据库间的区别，主要以 MsSql 为习惯来指出需要注意的
 * 1.MySql, Sqlite 没有 TOP 关键字用法，采用 limit 0,10 这样的用法
 * 
 * 你的口红
 * 以后我全包了
 * 记得每天
 * 还我一点
 */
using System.Collections.Generic;
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
				case DBClassify.MySql:
					return new MySql(connstr);
				case DBClassify.SQLite:
					return new SQLite(connstr);
				default:
					return null;
			}
		}

		#region 查询
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

		/// <summary>
		/// 返回第一行第一列 返回类型为 object
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns>返回第一行第一列</returns>
		public abstract object GetSingle(string sql);

		#endregion

		#region 执行

		/// <summary>
		/// 执行 sql 语句
		/// </summary>
		/// <param name="exsql">执行的 sql 语句</param>
		/// <returns>影响行数</returns>
		public abstract int ExecuteSql(string exsql);

		/// <summary>
		/// 批量执行 sql 语句
		/// </summary>
		/// <param name="listSql">执行的 sql 语句集合</param>
		/// <returns>影响行数</returns>
		public abstract int ExecuteSql(List<string> listSql);
		#endregion


		//#region #endregion



	}
}
