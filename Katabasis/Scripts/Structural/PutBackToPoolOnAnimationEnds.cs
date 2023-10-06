using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityAsync;

public class PutBackToPoolOnAnimationEnds : MonoBehaviour
{
    public void OnEnable()
    {
        
        wait();
    }

    public async Task wait()
    {
        await Task.Delay(GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length * 1000);
        Pooler.PutObjectBackToPool(gameObject);
    }
}
