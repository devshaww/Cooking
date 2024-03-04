using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    // 如：SoundManager注册了static event，发生SceneChange后SoundManager被销毁但static event的invocation list依然存在
    // 那么invoke就会导致来自已经被销毁SoundManager的invocation被调用 如果invoation里牵涉到那个SoundManager自身的代码比如transform
    // 会有NullReference Error
    private void Awake()
    {
        CuttingCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
        Player.ResetStaticData();
    }
}
