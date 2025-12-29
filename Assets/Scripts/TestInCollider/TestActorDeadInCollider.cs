using PrimeTween;
using UnityEngine;

public class TestActorDeadInCollider : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.transform.position = StartPoint.position;

        //1 Collider範囲内へ移動する
        //2 廃棄する
        Sequence.Create()
        .Chain(Tween.Position(this.transform,EndPoint.position,5.0f))
        .ChainCallback(()=>{Destroy(this.gameObject);});
    }

    void OnDestroy()
    {
        Debug.Log($"{name} has been destroyed");
    }
}
