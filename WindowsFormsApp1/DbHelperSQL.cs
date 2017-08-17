/*
 * 2017年7月27日 10:45:45 郑少宝 修改命名规范，函数体签名
 */
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ZR.Base
{
	/// <summary>
	/// 数据访问抽象基础类
	/// </summary>
	public abstract class DbHelperSQL
	{
		//数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.		
		public static string connectionString = @"server=192.168.156.187\SQLSERVER2005;database=ZrTBM;uid=sa;pwd=sa";
		//public static string connectionString = @"server=WIN7-1606240932;database=ZrTBM;uid=sa;pwd=sa";

		public DbHelperSQL()
		{

		}

		#region 查询
		/// <summary>
		/// 执行查询语句，返回DataSet
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns>DataSet</returns>
		public static DataSet Query(string sql)
		{
			return Query(sql, 30);
		}

		/// <summary>
		/// 执行查询语句，返回DataSet
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <param name="timeOut">超时时间(默认30秒)</param>
		/// <returns>DataSet</returns>
		public static DataSet Query(string sql, int timeOut)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet ds = new DataSet();
				try
				{
					connection.Open();
					SqlDataAdapter command = new SqlDataAdapter(sql, connection);
					command.SelectCommand.CommandTimeout = timeOut;
					command.Fill(ds, "ds");
				}
				catch (System.Data.SqlClient.SqlException ex)
				{
					throw new Exception(ex.Message);
				}
				return ds;
			}
		}

		/// <summary>
		/// 执行查询语句，返回 DataSet
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <param name="parameters">参数集合</param>
		/// <returns>DataSet</returns>
		public static DataSet Query(string sql, params SqlParameter[] parameters)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand();
				PrepareCommand(cmd, connection, null, sql, parameters);
				using (SqlDataAdapter da = new SqlDataAdapter(cmd))
				{
					DataSet ds = new DataSet();
					try
					{
						da.Fill(ds, "ds");
						cmd.Parameters.Clear();
					}
					catch (System.Data.SqlClient.SqlException ex)
					{
						// ErrorLog.WriteError.Recode(ex);
						throw new Exception(ex.Message);
					}
					return ds;
				}
			}
		}

		/// <summary>
		/// 执行查询语句，返回 DataTable
		/// 2017年5月27日 15:02:40 郑少宝 修改
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static DataTable QueryTab(string sql)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				DataTable dt = new DataTable();
				try
				{
					conn.Open();
					new SqlDataAdapter(new SqlCommand(sql, conn)).Fill(dt);
					return dt;
				}
				catch (SqlException ex)
				{
					throw ex;
				}
			}
		}

		/// <summary>
		/// 执行查询语句，返回 DataTable
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <param name="parameters">参数集合</param>
		/// <returns>DataTable</returns>
		public static DataTable QueryTab(string sql, params SqlParameter[] parameters)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand();
				PrepareCommand(cmd, connection, null, sql, parameters);
				using (SqlDataAdapter da = new SqlDataAdapter(cmd))
				{
					DataTable dt = new DataTable();
					try
					{
						da.Fill(dt);
						cmd.Parameters.Clear();
					}
					catch (System.Data.SqlClient.SqlException ex)
					{
						// ErrorLog.WriteError.Recode(ex);
						throw new Exception(ex.Message);
					}
					return dt;
				}
			}
		}


		#endregion

		#region GetSingle
		/// <summary>
		/// 查询 返回第一行第一列
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns></returns>
		public static object GetSingle(string sql)
		{
			return GetSingle(sql, 30);
		}

		/// <summary>
		/// 查询 返回第一行第一列
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <param name="timeOut">超时时间(默认30秒)</param>
		/// <returns></returns>
		public static object GetSingle(string sql, int timeOut)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(sql, connection))
				{
					try
					{
						connection.Open();
						cmd.CommandTimeout = timeOut;
						object obj = cmd.ExecuteScalar();
						if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
						{
							return null;
						}
						else
						{
							return obj;
						}
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
		/// 执行一条计算查询结果语句，返回查询结果（object）。
		/// </summary>
		/// <param name="sql">计算查询结果语句</param>
		/// <param name="parameters">参数集合</param>
		/// <returns></returns>
		public static object GetSingle(string sql, params SqlParameter[] parameters)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						PrepareCommand(cmd, connection, null, sql, parameters);
						object obj = cmd.ExecuteScalar();
						cmd.Parameters.Clear();
						if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
						{
							return null;
						}
						else
						{
							return obj;
						}
					}
					catch (System.Data.SqlClient.SqlException e)
					{
						//  ErrorLog.WriteError.Recode(e);
						throw e;
					}
				}
			}
		}
		#endregion

		#region 事务
		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="listSql">多条SQL语句</param>		
		public static int ExecuteSqlTran(List<String> listSql)
		{
			return ExecuteSqlTran(listSql, IsolationLevel.ReadCommitted);
		}

		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="listSql">多条SQL语句</param>		
		public static int ExecuteSqlTran(List<String> listSql, IsolationLevel level)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				SqlTransaction tx = conn.BeginTransaction(level);
				cmd.Transaction = tx;
				try
				{
					int count = 0;
					for (int n = 0; n < listSql.Count; n++)
					{
						string strsql = listSql[n];
						if (strsql.Trim().Length > 1)
						{
							cmd.CommandText = strsql;
							int add = cmd.ExecuteNonQuery();

							count = count + (add < 0 ? 0 : add);
						}
					}
					tx.Commit();
					return count;
				}
				catch (Exception ex)
				{
					tx.Rollback();
					throw ex;
				}
			}
		}

		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// 2017年7月27日 10:31:33 郑少宝
		/// </summary>
		/// <param name="listSql">多条SQL语句</param>
		/// <param name="listParam">参数集合</param>	
		public static int ExecuteSqlTran(List<String> listSql, List<List<SqlParameter>> listParam)
		{
			return ExecuteSqlTran(listSql, listParam, IsolationLevel.ReadCommitted);
		}

		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// 2017年7月27日 10:31:33 郑少宝
		/// </summary>
		/// <param name="listSql">多条SQL语句</param>
		/// <param name="listParam">参数集合</param>	
		/// <param name="level">事务隔离级别</param>	
		public static int ExecuteSqlTran(List<String> listSql, List<List<SqlParameter>> listParam, IsolationLevel level)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				SqlTransaction tx = conn.BeginTransaction(level);
				cmd.Transaction = tx;
				//try
				//{
				int count = 0;
				for (int n = 0; n < listSql.Count; n++)
				{
					string cmdText = listSql[n];
					SqlParameter[] cmdParms = listParam[n].ToArray();
					PrepareCommand(cmd, conn, tx, cmdText, cmdParms);
					int val = cmd.ExecuteNonQuery();
					count += val;
					cmd.Parameters.Clear();
				}
				tx.Commit();
				return count;
				//}
				//catch
				//{
				//    tx.Rollback();
				//    return 0;
				//}
			}
		}

		/// <summary>
		/// 执行相应数据库的SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="listSql">多条SQL语句</param>		
		public static int ExecuteDataBaseSqlTran(String sqlCon, List<String> listSql)
		{
			using (SqlConnection conn = new SqlConnection(sqlCon))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				SqlTransaction tx = conn.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					int count = 0;
					for (int n = 0; n < listSql.Count; n++)
					{
						string strsql = listSql[n];
						if (strsql.Trim().Length > 1)
						{
							cmd.CommandText = strsql;
							count += cmd.ExecuteNonQuery();
						}
					}
					tx.Commit();
					return count;
				}
				catch
				{
					tx.Rollback();
					return 0;
				}
			}
		}

		/// <summary>
		/// 执行多条SQL语句，实现数据库事务,SQL语句允许相同。 update by hxg 2016-5-17
		/// </summary>
		/// <param name="SQLStringList">
		/// SQL语句的哈希表（key为GUID，value是该语句的SQL语句）；
		/// 参数的哈希表（key为GUID，value是该语句的SqlParameter[]）；</param>
		public static void ExecuteSqlTran(Dictionary<string, string> SQLStringList, Dictionary<string, SqlParameter[]> ParmsList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						//循环
						foreach (string myDE in SQLStringList.Keys)
						{
							string cmdText = SQLStringList[myDE];
							SqlParameter[] cmdParms = (SqlParameter[])ParmsList[myDE];
							PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							cmd.Parameters.Clear();
						}
						trans.Commit();
					}
					catch (SqlException ex)
					{
						trans.Rollback();
						// ErrorLog.WriteError.Recode(ex);
						throw;
					}
				}
			}
		}
		#endregion

		#region  执行SQL语句

		/// <summary>
		/// 执行SQL语句，返回影响的记录数
		/// </summary>
		/// <param name="exSql">SQL语句</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSql(string exSql)
		{
			return ExecuteSqlByTime(exSql, 30);
		}

		/// <summary>
		/// 执行SQL语句，返回影响的记录数
		/// </summary>
		/// <param name="exSql">SQL语句</param>
		/// <param name="timeOut">超时时间(默认30秒)</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSqlByTime(string exSql, int timeOut)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(exSql, connection))
				{
					try
					{
						connection.Open();
						cmd.CommandTimeout = timeOut;
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

		///// <summary>
		///// 执行Sql和Oracle滴混合事务
		///// </summary>
		///// <param name="list">SQL命令行列表</param>
		///// <param name="oracleCmdSqlList">Oracle命令行列表</param>
		///// <returns>执行结果 0-由于SQL造成事务失败 -1 由于Oracle造成事务失败 1-整体事务执行成功</returns>
		//public static int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		//{
		//    using (SqlConnection conn = new SqlConnection(connectionString))
		//    {
		//        conn.Open();
		//        SqlCommand cmd = new SqlCommand();
		//        cmd.Connection = conn;
		//        SqlTransaction tx = conn.BeginTransaction();
		//        cmd.Transaction = tx;
		//        try
		//        {
		//            foreach (CommandInfo myDE in list)
		//            {
		//                string cmdText = myDE.CommandText;
		//                SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
		//                PrepareCommand(cmd, conn, tx, cmdText, cmdParms);
		//                if (myDE.EffentNextType == EffentNextType.SolicitationEvent)
		//                {
		//                    if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
		//                    {
		//                        tx.Rollback();
		//                        throw new Exception("违背要求"+myDE.CommandText+"必须符合select count(..的格式");
		//                        //return 0;
		//                    }

		//                    object obj = cmd.ExecuteScalar();
		//                    bool isHave = false;
		//                    if (obj == null && obj == DBNull.Value)
		//                    {
		//                        isHave = false;
		//                    }
		//                    isHave = Convert.ToInt32(obj) > 0;
		//                    if (isHave)
		//                    {
		//                        //引发事件
		//                        myDE.OnSolicitationEvent();
		//                    }
		//                }
		//                if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
		//                {
		//                    if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
		//                    {
		//                        tx.Rollback();
		//                        throw new Exception("SQL:违背要求" + myDE.CommandText + "必须符合select count(..的格式");
		//                        //return 0;
		//                    }

		//                    object obj = cmd.ExecuteScalar();
		//                    bool isHave = false;
		//                    if (obj == null && obj == DBNull.Value)
		//                    {
		//                        isHave = false;
		//                    }
		//                    isHave = Convert.ToInt32(obj) > 0;

		//                    if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
		//                    {
		//                        tx.Rollback();
		//                        throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须大于0");
		//                        //return 0;
		//                    }
		//                    if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
		//                    {
		//                        tx.Rollback();
		//                        throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须等于0");
		//                        //return 0;
		//                    }
		//                    continue;
		//                }
		//                int val = cmd.ExecuteNonQuery();
		//                if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
		//                {
		//                    tx.Rollback();
		//                    throw new Exception("SQL:违背要求" + myDE.CommandText + "必须有影响行");
		//                    //return 0;
		//                }
		//                cmd.Parameters.Clear();
		//            }
		//            string oraConnectionString = PubConstant.GetConnectionString("ConnectionStringPPC");
		//            bool res = OracleHelper.ExecuteSqlTran(oraConnectionString, oracleCmdSqlList);
		//            if (!res)
		//            {
		//                tx.Rollback();
		//                throw new Exception("Oracle执行失败");
		//                // return -1;
		//            }
		//            tx.Commit();
		//            return 1;
		//        }
		//        catch (System.Data.SqlClient.SqlException e)
		//        {
		//            tx.Rollback();
		//            throw e;
		//        }
		//        catch (Exception e)
		//        {
		//            tx.Rollback();
		//            throw e;
		//        }
		//    }
		//}                               

		/// <summary>
		/// 执行带一个存储过程参数的的SQL语句。
		/// </summary>
		/// <param name="exSql">SQL语句</param>
		/// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSql(string exSql, string content)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(exSql, connection);
				System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
				myParameter.Value = content;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					int rows = cmd.ExecuteNonQuery();
					return rows;
				}
				catch (System.Data.SqlClient.SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
		}

		/// <summary>
		/// 执行带一个存储过程参数的的SQL语句。
		/// </summary>
		/// <param name="exSql">SQL语句</param>
		/// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
		/// <returns>影响的记录数</returns>
		public static object ExecuteSqlGet(string exSql, string content)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(exSql, connection);
				System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
				myParameter.Value = content;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					object obj = cmd.ExecuteScalar();
					if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
					{
						return null;
					}
					else
					{
						return obj;
					}
				}
				catch (System.Data.SqlClient.SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
		}

		/// <summary>
		/// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
		/// </summary>
		/// <param name="strSQL">SQL语句</param>
		/// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(strSQL, connection);
				System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@fs", SqlDbType.Image);
				myParameter.Value = fs;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					int rows = cmd.ExecuteNonQuery();
					return rows;
				}
				catch (System.Data.SqlClient.SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
		}

		/// <summary>
		/// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="strSQL">查询语句</param>
		/// <returns>SqlDataReader</returns>
		public static SqlDataReader ExecuteReader(string strSQL)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand(strSQL, connection);
			try
			{
				connection.Open();
				SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				return myReader;
			}
			catch (System.Data.SqlClient.SqlException e)
			{
				throw e;
			}

		}

		#endregion

		#region 执行带参数的SQL语句

		/// <summary>
		/// 执行SQL语句，返回影响的记录数
		/// </summary>
		/// <param name="exSql">SQL语句</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSql(string exSql, params SqlParameter[] cmdParms)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						PrepareCommand(cmd, connection, null, exSql, cmdParms);

						int rows = cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();
						return rows;
					}
					catch (System.Data.SqlClient.SqlException e)
					{
						//  ErrorLog.WriteError.Recode(e);
						throw e;
					}
				}
			}
		}


		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		public static void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						//循环
						foreach (DictionaryEntry myDE in SQLStringList)
						{
							string cmdText = myDE.Key.ToString();
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
							PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							cmd.Parameters.Clear();
						}
						trans.Commit();
					}
					catch (SqlException ex)
					{
						trans.Rollback();
						// ErrorLog.WriteError.Recode(ex);
						throw;
					}
				}
			}
		}


		///// <summary>
		///// 执行多条SQL语句，实现数据库事务。
		///// </summary>
		///// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		//public static int ExecuteSqlTran(System.Collections.Generic.List<CommandInfo> cmdList)
		//{
		//    using (SqlConnection conn = new SqlConnection(connectionString))
		//    {
		//        conn.Open();
		//        using (SqlTransaction trans = conn.BeginTransaction())
		//        {
		//            SqlCommand cmd = new SqlCommand();
		//            try
		//            { int count = 0;
		//                //循环
		//                foreach (CommandInfo myDE in cmdList)
		//                {
		//                    string cmdText = myDE.CommandText;
		//                    SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
		//                    PrepareCommand(cmd, conn, trans, cmdText, cmdParms);

		//                    if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
		//                    {
		//                        if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
		//                        {
		//                            trans.Rollback();
		//                            return 0;
		//                        }

		//                        object obj = cmd.ExecuteScalar();
		//                        bool isHave = false;
		//                        if (obj == null && obj == DBNull.Value)
		//                        {
		//                            isHave = false;
		//                        }
		//                        isHave = Convert.ToInt32(obj) > 0;

		//                        if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
		//                        {
		//                            trans.Rollback();
		//                            return 0;
		//                        }
		//                        if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
		//                        {
		//                            trans.Rollback();
		//                            return 0;
		//                        }
		//                        continue;
		//                    }
		//                    int val = cmd.ExecuteNonQuery();
		//                    count += val;
		//                    if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
		//                    {
		//                        trans.Rollback();
		//                        return 0;
		//                    }
		//                    cmd.Parameters.Clear();
		//                }
		//                trans.Commit();
		//                return count;
		//            }
		//            catch
		//            {
		//                trans.Rollback();
		//                throw;
		//            }
		//        }
		//    }
		//}
		///// <summary>
		///// 执行多条SQL语句，实现数据库事务。
		///// </summary>
		///// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		//public static void ExecuteSqlTranWithIndentity(System.Collections.Generic.List<CommandInfo> SQLStringList)
		//{
		//    using (SqlConnection conn = new SqlConnection(connectionString))
		//    {
		//        conn.Open();
		//        using (SqlTransaction trans = conn.BeginTransaction())
		//        {
		//            SqlCommand cmd = new SqlCommand();
		//            try
		//            {
		//                int indentity = 0;
		//                //循环
		//                foreach (CommandInfo myDE in SQLStringList)
		//                {
		//                    string cmdText = myDE.CommandText;
		//                    SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
		//                    foreach (SqlParameter q in cmdParms)
		//                    {
		//                        if (q.Direction == ParameterDirection.InputOutput)
		//                        {
		//                            q.Value = indentity;
		//                        }
		//                    }
		//                    PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
		//                    int val = cmd.ExecuteNonQuery();
		//                    foreach (SqlParameter q in cmdParms)
		//                    {
		//                        if (q.Direction == ParameterDirection.Output)
		//                        {
		//                            indentity = Convert.ToInt32(q.Value);
		//                        }
		//                    }
		//                    cmd.Parameters.Clear();
		//                }
		//                trans.Commit();
		//            }
		//            catch
		//            {
		//                trans.Rollback();
		//                throw;
		//            }
		//        }
		//    }
		//}


		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						int indentity = 0;
						//循环
						foreach (DictionaryEntry myDE in SQLStringList)
						{
							string cmdText = myDE.Key.ToString();
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.InputOutput)
								{
									q.Value = indentity;
								}
							}
							PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.Output)
								{
									indentity = Convert.ToInt32(q.Value);
								}
							}
							cmd.Parameters.Clear();
						}
						trans.Commit();
					}
					catch (SqlException ex)
					{
						trans.Rollback();
						// ErrorLog.WriteError.Recode(ex);
						throw;
					}
				}
			}
		}



		/// <summary>
		/// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="strSQL">查询语句</param>
		/// <returns>SqlDataReader</returns>
		public static SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmdParms)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand();
			try
			{
				PrepareCommand(cmd, connection, null, SQLString, cmdParms);
				SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				cmd.Parameters.Clear();
				return myReader;
			}
			catch (System.Data.SqlClient.SqlException e)
			{
				//  ErrorLog.WriteError.Recode(e);
				throw e;
			}
			//			finally
			//			{
			//				cmd.Dispose();
			//				connection.Close();
			//			}	

		}




		private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
		{
			if (conn.State != ConnectionState.Open)
				conn.Open();
			cmd.Connection = conn;
			cmd.CommandText = cmdText;
			if (trans != null)
				cmd.Transaction = trans;
			cmd.CommandType = CommandType.Text;//cmdType;
			if (cmdParms != null)
			{


				foreach (SqlParameter parameter in cmdParms)
				{
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
						(parameter.Value == null))
					{
						parameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(parameter);
				}
			}
		}

		#endregion

		#region 其他方法
		/// <summary>
		/// 判断是否存在某表的某个字段
		/// </summary>
		/// <param name="tableName">表名称</param>
		/// <param name="columnName">列名称</param>
		/// <returns>是否存在</returns>
		public static bool ColumnExists(string tableName, string columnName)
		{
			string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
			object res = GetSingle(sql);
			if (res == null)
			{
				return false;
			}
			return Convert.ToInt32(res) > 0;
		}
		/// <summary>
		/// 获取最大的编号
		/// </summary>
		/// <param name="fieldName">字段名称</param>
		/// <param name="tableName">表名</param>
		/// <returns></returns>
		public static int GetMaxID(string fieldName, string tableName)
		{
			string strsql = "select max(" + fieldName + ")+1 from " + tableName;
			object obj = GetSingle(strsql);
			if (obj == null)
			{
				return 1;
			}
			else
			{
				return int.Parse(obj.ToString());
			}
		}
		/// <summary>
		/// 查询是否有值（第一行第一列）
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <returns></returns>
		public static bool Exists(string sql)
		{
			object obj = GetSingle(sql);
			return toBool(obj);
		}
		/// <summary>
		/// 表是否存在
		/// </summary>
		/// <param name="tableName">表名</param>
		/// <returns></returns>
		public static bool TableExists(string tableName)
		{
			string strsql = "select count(*) from sysobjects where id = object_id(N'[" + tableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
			//string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
			object obj = GetSingle(strsql);
			return toBool(obj);
		}
		/// <summary>
		/// 查询是否有值（第一行第一列）
		/// </summary>
		/// <param name="sql">查询语句</param>
		/// <param name="parameters">参数集合</param>
		/// <returns></returns>
		public static bool Exists(string sql, params SqlParameter[] parameters)
		{
			object obj = GetSingle(sql, parameters);
			return toBool(obj);
		}
		/// <summary>
		/// 获取服务器时间
		/// </summary>
		/// <returns>服务器时间</returns>
		public static DateTime GetServerDateTime()
		{
			object obj = GetSingle("SELECT getdate()");
			return Convert.ToDateTime(obj);
		}
		/// <summary>
		/// 判断表是否含有一个递增主键字段
		/// </summary>
		/// <param name="tableName">表名</param>
		/// <returns></returns>
		public static bool HavePK(string tableName)
		{
			string sql = string.Format("SELECT name FROM syscolumns WHERE id = object_id(N'{0}') AND COLUMNPROPERTY(id,name,'IsIdentity')=1", tableName);
			return GetSingle(sql) == null ? false : true;
		}
		#endregion

		#region 存储过程操作

		/// <summary>
		/// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlDataReader</returns>
		public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlDataReader returnReader;
			connection.Open();
			SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
			command.CommandType = CommandType.StoredProcedure;
			returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
			return returnReader;

		}


		/// <summary>
		/// 执行存储过程
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <param name="tableName">DataSet结果中的表名</param>
		/// <returns>DataSet</returns>
		public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet dataSet = new DataSet();
				connection.Open();
				SqlDataAdapter sqlDA = new SqlDataAdapter();
				sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
				sqlDA.Fill(dataSet, tableName);
				connection.Close();
				return dataSet;
			}
		}
		public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet dataSet = new DataSet();
				connection.Open();
				SqlDataAdapter sqlDA = new SqlDataAdapter();
				sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
				sqlDA.SelectCommand.CommandTimeout = Times;
				sqlDA.Fill(dataSet, tableName);
				connection.Close();
				return dataSet;
			}
		}


		/// <summary>
		/// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
		/// </summary>
		/// <param name="connection">数据库连接</param>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlCommand</returns>
		private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = new SqlCommand(storedProcName, connection);
			command.CommandType = CommandType.StoredProcedure;
			foreach (SqlParameter parameter in parameters)
			{
				if (parameter != null)
				{
					// 检查未分配值的输出参数,将其分配以DBNull.Value.
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
						(parameter.Value == null))
					{
						parameter.Value = DBNull.Value;
					}
					command.Parameters.Add(parameter);
				}
			}

			return command;
		}

		/// <summary>
		/// 执行存储过程，返回影响的行数		
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <param name="rowsAffected">影响的行数</param>
		/// <returns></returns>
		public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				int result;
				connection.Open();
				SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
				rowsAffected = command.ExecuteNonQuery();
				result = (int)command.Parameters["ReturnValue"].Value;
				//Connection.Close();
				return result;
			}
		}

		/// <summary>
		/// 创建 SqlCommand 对象实例(用来返回一个整数值)	
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlCommand 对象实例</returns>
		private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
			command.Parameters.Add(new SqlParameter("ReturnValue",
				SqlDbType.Int, 4, ParameterDirection.ReturnValue,
				false, 0, 0, string.Empty, DataRowVersion.Default, null));
			return command;
		}
		#endregion

		#region 私有函数 2017年7月27日 11:08:08 郑少宝
		/// <summary>
		/// 将一个查询结果对象转为 bool 类型，针对本类
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private static bool toBool(object obj)
		{
			if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
			{
				return false;
			}
			else
			{
				return obj.ToString() == "0" ? false : true;
			}
		}

		#endregion

	}

}
