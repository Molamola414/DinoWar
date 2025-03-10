using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using DG.Tweening;

[RequireComponent(typeof(Creature))]
public class DinoAI : MonoBehaviour
{
    public enum BulletDodgeType
    {
        None, // No dodging
        RunAway, // Simply run away from danger
        Dodge // Dodge sideway
    }

    public float walkSpeedRatio = 1;

    // Need a reference of bullet rader if it can dodge bullet
    public BulletDetector bulletDetector;
    protected Creature _creature;
    protected Creature _target;

    public float generatePathInterval = 0.5f;
    private float _pathFindingTime = 0;
    protected Path path;
    protected int currentWaypoint = 0;
    protected bool reachedEndOfPath = false;
    protected float nextWayPointDist = 3;
    protected Seeker seeker;

    protected Vector3 defaultScale;

    public virtual void Start()
    {
        seeker = GetComponent<Seeker>();
    }

    public virtual void Awake()
    {
        _creature = GetComponent<Creature>();

        defaultScale = transform.localScale;
    }

    public virtual void OnEnable()
    {
        if (transform.localScale != defaultScale) {
            transform.localScale = new Vector3(defaultScale.x*1.2f, defaultScale.y/10f, defaultScale.z*1.2f);
        }

        walkSpeedRatio = 0;

        DOTween.Sequence()
            .Append(transform.DOScale(defaultScale, 0.6f).SetEase(Ease.OutElastic))
            .AppendCallback(()=> {
                walkSpeedRatio = 1;
            });
    }

    // private void OnDrawGizmos() {
    //     if (path == null) return;

    //     Gizmos.color = Color.blue;
    //     for (int i = currentWaypoint; i < path.vectorPath.Count - 1; i++) {
    //         Gizmos.DrawLine(path.vectorPath[i], path.vectorPath[i + 1]);
    //     }
    // }

    public virtual void OnPathComplete(Path p)
    {
        _pathFindingTime = Time.time;
        if(!p.error) {
            path = p;
            if(path.vectorPath.Count > 10) {
                // Fix the issue of turning backward for 1 frame after refreshing the path
                currentWaypoint = 10;
            }else {
                currentWaypoint = 0;
            }
        }else {
            path = null;
        }
    }

    public virtual void GeneratePath(Vector3 destination) {
        if (path != null && Vector3.Distance(_creature.transform.position, destination) < nextWayPointDist)
            return; // Avoid recalculating paths when close to destination

        if((seeker != null && Time.time - _pathFindingTime > generatePathInterval) && seeker.IsDone()) {
            seeker.StartPath(_creature.transform.position, destination, OnPathComplete);
        }
    }

    public virtual void PathFindingMove() {
        if(path == null) {
            return;
        }

        if(currentWaypoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            return;
        }

        reachedEndOfPath = false;

        Vector3 currentPos = _creature.transform.position;
        Vector3 targetWaypoint = path.vectorPath[currentWaypoint];
        Vector3 dir = targetWaypoint - currentPos;

        _creature.controls.Direction = (new Vector2(dir.x, dir.z)).normalized * walkSpeedRatio;

        if(Vector3.Distance(currentPos, targetWaypoint) < nextWayPointDist) {
            currentWaypoint++;
        }
    }
}
