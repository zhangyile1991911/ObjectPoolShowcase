# 通常な使い方
```csharp
    //オブジェクトを取得する
    var handler = _pool.Get();
    handler.instance.transform.SetParent(transform);
    handler.instance.SetActive(true);
    //2秒後返却する
    PrimeTween.Tween.Delay(
        this, 
        duration: 2.0f, 
        onComplete: () => handler.Release());
```
# 借りたオブジェクト、親ノードにアタッチしてない場合
 - テストSceneは`TestObjectPool`です
 - 下記のコード借りたオブジェクトを親ノードにアタッチしてないという場合ではオブジェクトが逸脱したがプールが適切に回収できる
```csharp
    var handler = _pool.Get();
    handler.instance.SetActive(true);
```

# 意外と廃棄された場合
親ノードが廃棄された場合
```csharp
    //プールから取得する
    for (int i = 0; i < 5; i++)
    {
        var h = _pool.Get();
        h.instance.SetActive(true);
        h.instance.transform.SetParent(transform);
    }
    //2秒後勝手に借りたオブジェクトを廃棄する
    PrimeTween.Tween.Delay(
        this, 
        duration: 2.0f, 
        onComplete: () => Destroy(this.gameObject));
```

借りたオブジェクトが無闇に破棄された時
```csharp
    var handler = _pool.Get();
    Destroy(handler.instance);
```

エラーがすぐ出てきました
<img width="100%" src=".git/graphics/errorlog.jpg">

# 借りたオブジェクトを複数回にreleaseされる

```csharp
    var handler = _pool.Get();
    handler.Release();
    handler.Release();
    handler.Dispose();
```

# 返却した後に使い続ける
```csharp
    var handler = _pool.Get();
    handler.Release();
    //すぐエラーが出る
    handler.instance.SetActive(true);
    handler.instance.transform.SetParent(transform);
```

# 自動で滑らかに拡大したり縮小したりする
- テストSceneは`TestPoolShrinkOrExpand`です
```csharp
public class GameObjectPool
{
    public ObjectHandler Get()
    {
        //貸し出し際に 貸出回数を記録する
        //一定時間内で最大貸出数を記録する
        _currentBorrowed++;
        _maxBorrowed = Mathf.Max(_currentBorrowed, _maxBorrowed);
    }
    public void Release(ObjectHandler handler)
    {
        _currentBorrowed--;
    }

    public void LateUpdate()
    {
        //設定時間が経ったら
        _smoothedPeak = alpha * _maxBorrowed + (1.0f - alpha) * _smoothedPeak;
        int targetPoolNum = Mathf.CeilToInt(_smoothedPeak * 1.2f);
        ShrinkOrExpandTo(targetPoolNum);
    }

    private void ShrinkOrExpandTo(int targetPoolNum)
    {
        int current = _pool.Count;
        
        if (current == targetPoolNum)
            return;
        
        float changePercent = Mathf.Abs(targetPoolNum - current) / (float)current;
        //頻繁に操作を避けるため
        if (changePercent < 0.25f)
            return;

        if (targetPoolNum < current)
        {
            // shrink
            int toRemove = current - targetPoolNum;
            for (int i = 0; i < toRemove; i++)
            {
                var obj = _pool.Dequeue();
                Destroy(obj);
            }
            Debug.Log($"{name} shrink out of {toRemove}.");
        }
        else
        {
            // expand
            int toAdd = targetPoolNum - current;
            MakeMore(toAdd);
            Debug.Log($"{name} expand {toAdd}.");
        }
    }
}
```

# PoolObjectがColliderに入っているうちに回収された時の処理
- テストSceneは`TestWithCollider`です
---

### 前提
Colliderは当たり範囲に入っているObjectを記録する変数を持っている

```
public class ColliderListener : MonoBehaviour
{
    //key=InstanceId
    protected Dictionary<int,Transform> Actors;

    void Update()
    {
        foreach(var one in Actors)
        {//仮にフレームごとにActorにダメージを与える
            ApplyDamage(one);
        }
    }

    void OnTriggerEnter(Collider other)
    {//Objectが入った
        Actors.Add(other.GetInstanceID(),other.transform);
    }

    void OnTriggerExit(Collider other)
    {//Objectが離れた
        Actors.Remove(other.GetInstanceID());
    }
}
```

### 問題
仮にColliderにいるObjectがとある理由で破棄されたりプールに返却されたりしたら、Colliderが持っているActors変数には無効の参照先がそのままに残ってしまいました。


### 問題の対策

- Objectが廃棄された後、OnDestroy関数を呼び出されるという仕組みがあります
- ObjectがPoolに返却された時に、親オブジェクトが変わるという仕組みがあるので、OnTransformParentChanged関数を呼び出される

上記の2つ仕組みを活用して問題を解決する方法ができます
新規Componentを作ります
```
public class ColliderWatchDog : MonoBehaviour
{
    public ColliderListener Owner;

    void OnTransformParentChanged()
    {
        Owner.RemoveObject(this.transform);
    }

    void OnDestroy()
    {
        Owner.RemoveObject(this.transform);
    }
}
```

ColliderListenerにRemoveObject関数を追加する
```
public class ColliderListener : MonoBehaviour
{
    //key=InstanceId
    protected Dictionary<int,Transform> Actors;

    void OnTriggerEnter(Collider other)
    {//Objectが入った
        ColliderWatchDog colliderWatchDog = other.gameObject.AddComponent<ColliderWatchDog>();
        colliderWatchDog.Owner = this;
        Actors.Add(other.GetInstanceID(),other.transform);
    }

    void OnTriggerExit(Collider other)
    {//Objectが離れた
        ColliderWatchDog colliderWatchDog = other.gameObject.GetComponent<ColliderWatchDog>();
        if(colliderWatchDog != null)
        {
            colliderWatchDog.Owner = null;
            Destroy(colliderWatchDog);
        }
        Actors.Remove(other.GetInstanceID());
    }

    void RemoveObject(Transform other)
    {
        Actors.Remove(other.GetInstanceID());
    }
}
```



# 便利なツール
<img width="100%" src=".git/graphics/tool.jpg">

# アイコン
<img width="25%" height="25%" src=".git/graphics/objectpoolicon.jpg">
