using System;
using PrimeTween;
using UnityEngine;

public class PoolActorNotDeadInCollider : MonoBehaviour
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
        
        Sequence.Create()
        .Chain(Tween.Position(this.transform,EndPoint.position,5.0f))
        .Chain(Tween.Position(this.transform,StartPoint.position,5.0f))
        .ChainCallback(() =>
        {//Colliderから離れた後プールに返却する
            OnFinish();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
