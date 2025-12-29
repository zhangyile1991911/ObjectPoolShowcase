using System;
using GameSystem.ObjectPool;
using PrimeTween;
using UnityEngine;

public class PoolActorDeadInCollider : MonoBehaviour
{
    public Action OnFinish;
    private Transform StartPoint;
    private Transform EndPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPoint = GameObject.Find("StartPoint").transform;
        EndPoint = GameObject.Find("EndPoint").transform;

        this.transform.position = StartPoint.position;

        //Collider範囲内へ移動する
        Sequence.Create()
        .Chain(Tween.Position(this.transform,EndPoint.position,5.0f))
        .ChainCallback(() =>
        {//Collider内でいるうちにプールに返却する
            OnFinish();
        });
    }
}
