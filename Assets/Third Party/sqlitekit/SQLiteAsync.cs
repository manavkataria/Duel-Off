using UnityEngine;
using System;
using System.Threading;

public class SQLiteAsync
{
	
	#region Public
	
	public SQLiteAsync()
	{
	}
	
	public void Open(string filename)
	{
		ThreadQueue.QueueUserWorkItem(new ThreadQueue.WorkCallback(OpenDatabase), new WaitCallback(EmptyCallback), filename);
	}
	
	public void Close()
	{
		ThreadQueue.QueueUserWorkItem(new ThreadQueue.WorkCallback(CloseDatabase), new WaitCallback(EmptyCallback), null);
	}
	
	public delegate void QueryCallback(SQLiteQuery qr, object state);
	public void Query(string query, QueryCallback callback, object state)
	{
		ThreadQueue.QueueUserWorkItem(new ThreadQueue.WorkCallback(CreateQuery), new WaitCallback(CreateQueryComplete), new QueryState(query,callback,state));
	}
	
	public delegate void StepCallback(SQLiteQuery qr, bool rv, object state);
	public void Step(SQLiteQuery qr, StepCallback callback, object state)
	{
		ThreadQueue.QueueUserWorkItem(new ThreadQueue.WorkCallback(StepQuery), new WaitCallback(StepQueryComplete), new StepState(qr,callback,state));
	}
	
	public delegate void ReleaseCallback(object state);
	public void Release(SQLiteQuery qr, ReleaseCallback callback, object state)
	{
		ThreadQueue.QueueUserWorkItem(new ThreadQueue.WorkCallback(ReleaseQuery), new WaitCallback(ReleaseQueryComplete), new ReleaseState(qr,callback,state));
	}

	#endregion
	
	
	#region Implementation
	
	//
	// members
	//
	SQLiteDB db = null;
	
	
	//
	// internal classes
	//
	class QueryState 
	{
		string 			sql;
		QueryCallback	callback;
		object 			state;
		SQLiteQuery		query;
		
		public string 			Sql 		{ get { return sql; } }
		public SQLiteQuery 		Query 		{ get { return query; } set { query = value; } }
		public QueryCallback	Callback	{ get { return callback; } }
		public object 			State		{ get { return state; } }
		
		public QueryState(string sql, QueryCallback callback, object state){
			this.sql = sql; 
			this.callback = callback;
			this.state = state;
		}
	}

	class StepState 
	{
		SQLiteQuery		query;
		StepCallback	callback;
		object 			state;
		bool			step;
		
		public SQLiteQuery		Query 		{ get { return query; } }
		public StepCallback		Callback	{ get { return callback; } }
		public object 			State		{ get { return state; } }
		public bool 			Step		{ get { return step; } set { step = value; } }
		
		public StepState(SQLiteQuery query, StepCallback callback, object state){
			this.query = query; 
			this.callback = callback;
			this.state = state;
		}
	}

	class ReleaseState
	{
		SQLiteQuery		query;
		ReleaseCallback	callback;
		object 			state;
		
		public SQLiteQuery		Query 		{ get { return query; } }
		public ReleaseCallback	Callback	{ get { return callback; } }
		public object 			State		{ get { return state; } }
		
		public ReleaseState(SQLiteQuery query, ReleaseCallback callback, object state){
			this.query = query; 
			this.callback = callback;
			this.state = state;
		}
	}
	
	//
	// functions
	//
	private void OpenDatabase(ThreadQueue.TaskControl control, object obj)
    {
		string filename = obj as string;
		
        try
        {
			db = new SQLiteDB();
            db.Open(filename);
			
        }
        catch (Exception ex)
        {
            Debug.LogError("SQLiteAsync : OpenDatabase : Exception : " + ex.Message);
        }
    }
	
    private void CloseDatabase(ThreadQueue.TaskControl control, object state)
    {
        try
        {
			if( db != null )
			{
            	db.Close();
				db = null;
			}
			else
			{
				throw new Exception( "Database not ready!" );
			}
        }
        catch (Exception ex)
        {
			Debug.LogError("SQLiteAsync : Exception : " + ex.Message);
        }
    }

	private void CreateQuery(ThreadQueue.TaskControl control, object state)
	{
        try
        {
			if( db != null )
			{
				QueryState qrState = state as QueryState;
				qrState.Query = new SQLiteQuery(db,qrState.Sql);
			}
			else
			{
				throw new Exception( "Database not ready!" );
			}/**/
        }
        catch (Exception ex)
        {
			Debug.LogError("SQLiteAsync : CreateQuery : Exception : " + ex.Message);
        }
	}
	
	private void CreateQueryComplete(object state)
	{
		QueryState qrState = state as QueryState;
		qrState.Callback(qrState.Query, qrState.State);
	}
	
	private void StepQuery(ThreadQueue.TaskControl control, object state)
	{
        try
        {
			if( db != null )
			{
				StepState stState = state as StepState;
				stState.Step = stState.Query.Step();
			}
			else
			{
				throw new Exception( "Database not ready!" );
			}
        }
        catch (Exception ex)
        {
			Debug.LogError("SQLiteAsync : Exception : " + ex.Message);
        }
	}
	
	private void StepQueryComplete(object state)
	{
		StepState stState = state as StepState;
		stState.Callback(stState.Query,stState.Step,stState.State);
	}

	private void ReleaseQuery(ThreadQueue.TaskControl control, object state)
	{
        try
        {
			if( db != null )
			{
				ReleaseState rlState = state as ReleaseState;
				rlState.Query.Release();
			}
			else
			{
				throw new Exception( "Database not ready!" );
			}
        }
        catch (Exception ex)
        {
			Debug.LogError("SQLiteAsync : Exception : " + ex.Message);
        }
	}
	
	private void ReleaseQueryComplete(object state)
	{
		ReleaseState rlState = state as ReleaseState;
		rlState.Callback(rlState.State);
	}
	
	private void EmptyCallback(object obj)
	{
		// nothing to do here
	}

	
	#endregion
}
