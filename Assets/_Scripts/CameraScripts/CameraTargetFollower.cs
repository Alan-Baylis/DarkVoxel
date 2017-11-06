using System;
using UnityEngine;

public abstract class CameraTargetFollower : MonoBehaviour
{
    public enum UpdateType
    {
        FixedUpdate,
        LateUpdate,
        ManualUpdate
    }

    [SerializeField] protected Transform _target;

    [SerializeField] private bool _autoTargetPlayer = true;

    [SerializeField] private UpdateType _updateType;

    protected Rigidbody _targetRigidbody;

    protected virtual void Start()
    {
        // if auto targeting is used, find the object tagged "Player"
        // any class inheriting from this should call base.Start() to perform this action!
        if(_autoTargetPlayer)
        {
            FindAndTargetPlayer ();
        }

        if(_target == null)
        {
            return;
        }

        _targetRigidbody = _target.GetComponentInParent<Rigidbody> ();
    }

    private void FixedUpdate ( )
    {
        // we update from here if updatetype is set to Fixed, or in auto mode,
        // if the target has a rigidbody, and isn't kinematic.
        if(_autoTargetPlayer && (_target == null || !_target.gameObject.activeSelf))
        {
            FindAndTargetPlayer ();
        }

        if(_updateType == UpdateType.FixedUpdate)
        {
            FollowTarget (Time.deltaTime);
        }
    }

    private void LateUpdate ( )
    {
        // we update from here if updatetype is set to Late, or in auto mode,
        // if the target does not have a rigidbody, or - does have a rigidbody but is set to kinematic.
        if (_autoTargetPlayer && (_target == null || !_target.gameObject.activeSelf))
        {
            FindAndTargetPlayer ();
        }

        if (_updateType == UpdateType.LateUpdate)
        {
            FollowTarget (Time.deltaTime);
        }
    }

    public void ManualUpdate()
    {
        // we update from here if updatetype is set to Late, or in auto mode,
        // if the target does not have a rigidbody, or - does have a rigidbody but is set to kinematic.
        if (_autoTargetPlayer && (_target == null || !_target.gameObject.activeSelf))
        {
            FindAndTargetPlayer ();
        }

        if (_updateType == UpdateType.ManualUpdate)
        {
            FollowTarget (Time.deltaTime);
        }
    }

    protected abstract void FollowTarget ( float deltaTime );

    public void FindAndTargetPlayer()
    {
        // auto target an object tagged player, if no target has been assigned
        var targetObject = GameObject.FindGameObjectWithTag ("Player");

        if(targetObject)
        {
            SetTarget (targetObject.transform);
        }
    }

    public virtual void SetTarget(Transform newTransform)
    {
        _target = newTransform;
    }

    public Transform Target
    {
        get { return _target; }
    }
}

