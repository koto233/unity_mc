using Cysharp.Threading.Tasks;
using UnityEngine;

public class Loader : MonoBehaviour
{

    void Start()
    {
        StartGame().Forget();
    }

    async UniTaskVoid StartGame()
    {

    }
}
