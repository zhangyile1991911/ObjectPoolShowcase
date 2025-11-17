using UnityEngine;
using GameSystem.ObjectPool;
public class TestObjectPool : MonoBehaviour
{
    public GameObject TestPrefab;
    GameObjectPool _pool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pool = FindFirstObjectByType<GameObjectPool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {//通常な使い方
            var handler = _pool.Get();
            handler.Instance.transform.SetParent(transform);
            handler.Instance.SetActive(true);
            PrimeTween.Tween.Delay(this, duration: 2.0f, onComplete: () => handler.Release());
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            //借りたオブジェクト、親ノードを設定されない場合
            var handler = _pool.Get();
            handler.Instance.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            //手動で親ノードを破棄することで　動作を確認する
            //想定は返却されないHandlerが警告を出すとなります
            for (int i = 0; i < 5; i++)
            {
                var h = _pool.Get();
                h.Instance.SetActive(true);
                h.Instance.transform.SetParent(transform);
            }
            PrimeTween.Tween.Delay(this, duration: 2.0f, onComplete: () => Destroy(this.gameObject));
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            //借りたオブジェクト、無闇に破棄する
            var handler = _pool.Get();
            Destroy(handler.Instance);
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            //借りたオブジェクト、複数回にreleaseする
            var handler = _pool.Get();
            handler.Release();
            handler.Release();
            handler.Dispose();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            //返却した後に使い続ける
            var handler = _pool.Get();
            handler.Release();
            //すぐエラーが出る
            handler.Instance.SetActive(true);
            handler.Instance.transform.SetParent(transform);
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            _pool.PrintDebug();
        }
        

        if (Input.GetKeyDown(KeyCode.C))
        {
            using (var handler = _pool.Get())
            {//Disposeを活用して自動で返却される
                handler.Instance.gameObject.SetActive(true);
                handler.Instance.transform.SetParent(this.transform);
                Debug.Log("test automatically release");
            }
        }
    }
}
