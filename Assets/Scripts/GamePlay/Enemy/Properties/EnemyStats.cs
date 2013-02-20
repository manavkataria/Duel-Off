using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/** EnemyStats Properties Object
 *  -- Properties for EnemyBase to be passed into main EnemyBase Script
 *  -- These values are populated by sqlite DB from stored values
 */
public class EnemyStats {
	
	#region Enemy Properties

    private int _level;
	private float _health;
	private float _hitRate;
    private float _hideRate;
    private float _minHideRate;
    private float _maxHideRate;
	private float _runSpeed;
    private float _minShotRate;
    private float _maxShotRate;
    private float _minStillRate;
    private float _maxStillRate;
	private float _damage;
	private string _enemyGunModel;
	private bool _isTwoHanded;
	
	#endregion
	
	#region Enemy Accessors

    public int level
    {
        get { return _level; }
        set { _level = value; }
    }

    public float health
    {
        get { return _health; }
        set 
		{ 
			if( value > 100 )
				_health = 100;
			else if( value < 0 )
				_health = 0;
			else
			_health = value; 
		}
    }

    public float hitRate
    {
        get { return _hitRate; }
        set { _hitRate = value; }
    }

    public float hideRate
    {
        get { return _hideRate; }
        set { _hideRate = value; }
    }

    public float minHideRate
    {
        get { return _minHideRate; }
        set { _minHideRate = value; }
    }

    public float maxHideRate
    {
        get { return _maxHideRate; }
        set { _maxHideRate = value; }
    }

    public float runSpeed
    {
        get { return _runSpeed; }
        set { _runSpeed = value; }
    }

    public float minShotRate
    {
        get { return _minShotRate; }
        set { _minShotRate = value; }
    }

    public float maxShotRate
    {
        get { return _maxShotRate; }
        set { _maxShotRate = value; }
    }

    public float minStillRate
    {
        get { return _minStillRate; }
        set { _minStillRate = value; }
    }

    public float maxStillRate
    {
        get { return _maxStillRate; }
        set { _maxStillRate = value; }
    }
	
	public float damage
	{
		get { return _damage; }
		set { _damage = value; }
	}
	
	public string enemyGunModel
	{
		get { return _enemyGunModel; }
		set { _enemyGunModel = value; }
	}
	
	public bool isTwoHanded
	{
		get { return _isTwoHanded; }
		set { _isTwoHanded = value; }
	}

	#endregion

    #region Constructor

    public EnemyStats(System.Object[] objArray)
    {
		level = Convert.ToInt32(objArray[0]);
		health = 100f;
		hitRate = Convert.ToSingle(objArray[1]);
		hideRate = Convert.ToSingle(objArray[2]);
		minHideRate = Convert.ToSingle(objArray[3]);
		maxHideRate = Convert.ToSingle(objArray[4]);
		runSpeed = Convert.ToSingle(objArray[5]);
		minShotRate = Convert.ToSingle(objArray[6]);
		maxShotRate = Convert.ToSingle(objArray[7]);
		minStillRate = Convert.ToSingle(objArray[8]);
		maxStillRate = Convert.ToSingle(objArray[9]);
		damage = Convert.ToSingle(objArray[10]);
		
		DBAccess.instance.isDbInit = true;
    }
	
	public EnemyStats() { }

    #endregion

}
