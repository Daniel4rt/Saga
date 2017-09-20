using System;
using System.Data;
using System.Data.SqlClient;	// ��װSQL Sevrer �ķ��ʷ����������ռ�
using SagaLib;

namespace SagaDB
{
    public class MSSQLOperator
    {
        /// <summary>
		/// ���ݿ�����
		/// </summary>
		private SqlConnection _conn;
		/// <summary>
		/// ��������
		/// </summary>
		private SqlTransaction _trans;
		/// <summary>
		/// ��ȡ��ǰ�Ƿ����������У�Ĭ��ֵfalse
		/// </summary>
		private bool isTransaction = false;
		
		public MSSQLOperator(string strConnection)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this._conn = new SqlConnection(strConnection);
		}

		/// <summary>
		/// ��ȡ��ǰSQL Server����
		/// </summary>
		public IDbConnection Connection
		{
			get
			{
				return this._conn;
			}
		}

        public ConnectionState State
        {
            get
            {
                if (this._conn == null)
                    return ConnectionState.Closed;
                return this._conn.State;
            }
        }


		/// <summary>
		/// ��SQL Server����
		/// </summary>
		public void Open()
		{
			if (_conn.State != ConnectionState.Open)
			{
				try
				{
					_conn.Open();                    
				}
				catch(Exception ex)
				{
                    Logger.ShowSQL(ex, null);
				}
			}
		}

		/// <summary>
		/// �ر�SQL Server����
		/// </summary>
		public void Close()
		{
			if (_conn.State == ConnectionState.Open)
			{
				try
				{
					_conn.Close();
				}
				catch(Exception ex)
				{
                    Logger.ShowSQL(ex, null);
				}
			}
		}

		/// <summary>
		/// ��ʼһ��SQL Server����
		/// </summary>
		public void BeginTrans()
		{
			_trans = _conn.BeginTransaction();
			isTransaction = true;
		}

		/// <summary>
		/// �ύһ��SQL Server����
		/// </summary>
		public void CommitTrans()
		{
			_trans.Commit();
			isTransaction = false;
		}

		/// <summary>
		/// �ع�һ��SQL Server����
		/// </summary>
		public void RollbackTrans()
		{
			_trans.Rollback();
			isTransaction = false;
		}

		/// <summary>
		/// ִ��һ��SQL���(UPDATE,INSERT)
		/// </summary>
		/// <param name="sql">SQL���</param>
		public void ExeSql(string sql)
		{
			// ��
            bool criticalarea = ClientManager.enteredcriarea;
            if (criticalarea)
                ClientManager.LeaveCriticalArea();
            DatabaseWaitress.EnterCriticalArea();
            this.Open();

			SqlCommand cmd = new SqlCommand();
			cmd.Connection = this._conn;
			if (isTransaction == true)
			{
				cmd.Transaction = this._trans;
			}
			cmd.CommandText = sql;
			try
			{
				cmd.ExecuteNonQuery();                
			}
			catch(Exception ex)
			{
                Logger.ShowSQL("Error on query:" + sql, null);
                Logger.ShowSQL(ex, null);
			}
            DatabaseWaitress.LeaveCriticalArea();
            if (criticalarea)
                ClientManager.EnterCriticalArea();
			// �ͷ�
			//this.Close();
		}

		/// <summary>
		/// ִ��һ��SQL���(INSERT)���ص�ǰID
		/// </summary>
		/// <param name="sql">SQL���</param>
		/// <param name="a">��ʱ����</param>
		/// <returns>��ǰID</returns>
		public int ExeSql(string sql, int a)
		{
			int identity = -1;
            bool criticalarea = ClientManager.enteredcriarea;
            if (criticalarea)
                ClientManager.LeaveCriticalArea();
            DatabaseWaitress.EnterCriticalArea();
            
			// ��
			this.Open();

			SqlCommand cmd = new SqlCommand();
			cmd.Connection = this._conn;
			if (isTransaction == true)
			{
				cmd.Transaction = this._trans;
			}
			cmd.CommandText = sql + " select @@identity as 'identity'";
			try
			{
				// ��һ�е�һ�е�ֵΪ��ǰID
				SqlDataReader dr = cmd.ExecuteReader();
				
				if (dr.Read())
				{
					identity = int.Parse(dr[0].ToString());
				}

				dr.Close();
			}
            catch (Exception ex)
            {
                Logger.ShowSQL("Error on query:" + sql, null);
                Logger.ShowSQL(ex, null);
            }
            DatabaseWaitress.LeaveCriticalArea();
            if (criticalarea)
                ClientManager.EnterCriticalArea();
			// �ͷ�
			//this.Close();

			return identity;
		}

		/// <summary>
		/// ִ��SQL��䷵�ص�һ�е�һ�е�ֵ
		/// </summary>
		/// <param name="sql">SQL���</param>
		/// <returns>��һ�е�һ�е�ֵ</returns>
		public string ExeSqlScalar(string sql)
		{
			DataTable dt = null;
			try
			{
				dt = this.GetDataTable( sql);
				if (dt.Rows.Count > 0)
				{
					string v_Value = dt.Rows[0][0].ToString();
					dt.Dispose();
					return v_Value;
				}
				else
				{
					return "";
				}
			}
            catch (Exception ex)
            {
                Logger.ShowSQL("Error on query:" + sql, null);
                Logger.ShowSQL(ex, null);
                return "";
            }
		}


		/// <summary>
		/// ִ��SQL��䷵��Ӱ������
		/// </summary>
		/// <param name="sql">SQL���</param>
		/// <returns>Ӱ������</returns>
		public int ExeSqlRows(string sql)
		{
			DataTable dt = null;
			try
			{
				dt = this.GetDataTable( sql);
				int v_RowsCount = dt.Rows.Count;
				dt.Dispose();
				return v_RowsCount;
			}
            catch (Exception ex)
            {
                Logger.ShowSQL("Error on query:" + sql, null);
                Logger.ShowSQL(ex, null);
                return -1;
            }
		}

		/// <summary>
		/// ��ȡDataSet
		/// </summary>
		/// <param name="sql">SQL���</param>
		/// <returns>DataSet</returns>
		public DataSet GetDataSet(string sql)
		{
            bool criticalarea = ClientManager.enteredcriarea;
            if (criticalarea)
                ClientManager.LeaveCriticalArea();
            DatabaseWaitress.EnterCriticalArea();
            // ��
			this.Open();

			SqlCommand cmd = new SqlCommand();
			cmd.Connection = this._conn;
			if (isTransaction == true)
			{
				cmd.Transaction = this._trans;
			}
			DataSet ds = new DataSet();
			SqlDataAdapter da = new SqlDataAdapter();
			cmd.CommandText = sql;
			da.SelectCommand = cmd;
			try
			{
				da.Fill(ds);
			}
			catch(Exception ex)
			{
                Logger.ShowSQL("Error on query:" + sql, null);
                Logger.ShowSQL(ex, null);
			}			
			// �ͷ�
			//this.Close();
            DatabaseWaitress.LeaveCriticalArea();
            if (criticalarea)
                ClientManager.EnterCriticalArea();
			return ds;
		}

		/// <summary>
		/// ��ȡDataTable
		/// </summary>
		/// <param name="sql">SQL���</param>
		/// <returns>DataTable</returns>
		public DataTable GetDataTable(string sql)
		{
            bool criticalarea = ClientManager.enteredcriarea;
            if (criticalarea)
                ClientManager.LeaveCriticalArea();
            DatabaseWaitress.EnterCriticalArea();
            // ��
			this.Open();

			SqlCommand cmd = new SqlCommand();
			cmd.Connection = this._conn;
			if (isTransaction == true)
			{
				cmd.Transaction = this._trans;
			}
			DataTable dt = new DataTable();
			SqlDataAdapter da = new SqlDataAdapter();
			cmd.CommandText = sql;
			da.SelectCommand = cmd;
			try
			{
				da.Fill(dt);
			}
            catch (Exception ex)
            {
                Logger.ShowSQL("Error on query:" + sql, null);
                Logger.ShowSQL(ex, null);
            }

			// �ͷ�
			//this.Close();
            DatabaseWaitress.LeaveCriticalArea();
            if (criticalarea)
                ClientManager.EnterCriticalArea();
			return dt;
		}

		/// <summary>
		/// ִ�д洢����
		/// </summary>
		/// <param name="p_ProcedureName">�洢������</param>
		/// <param name="p_SqlParameterArray">�洢���̲���</param>
		public void ExeProcedure(string p_ProcedureName, SqlParameter[] p_SqlParameterArray)
		{
			// ��
			this.Open();
			SqlCommand cmd = new SqlCommand();
			cmd.CommandText = p_ProcedureName;
			cmd.Connection = this._conn;
			cmd.CommandType = CommandType.StoredProcedure;
			foreach (SqlParameter Sq in p_SqlParameterArray)
			{
				cmd.Parameters.Add( Sq);
			}
			cmd.ExecuteNonQuery();
			// �ͷ�
			//this.Close();
		}

		/// <summary>
		/// ִ�д洢����
		/// </summary>
		/// <param name="p_ProcedureName">�洢������</param>
		/// <param name="p_SqlParameterArray">�洢���̲���</param>
		/// <param name="p_TableIndex">�������������ʱ��</param>
		/// <returns>DataSet</returns>
		public DataSet ExeProcedure(string p_ProcedureName, SqlParameter[] p_SqlParameterArray, int p_TableIndex)
		{
			DataSet ds = new DataSet();
			SqlDataAdapter da = new SqlDataAdapter(p_ProcedureName, this._conn);
			da.SelectCommand.CommandType = CommandType.StoredProcedure;
			foreach(SqlParameter Sq in p_SqlParameterArray)
			{
				da.SelectCommand.Parameters.Add( Sq);
			}
			da.Fill(ds);
			return ds;
		}
    }
}
