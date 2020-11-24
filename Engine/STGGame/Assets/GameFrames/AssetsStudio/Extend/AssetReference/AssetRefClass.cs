using UnityEngine;

namespace ASGame
{
    [System.Serializable] public class AssetRefGameObject : AssetBaseRef<GameObject> { }
    [System.Serializable] public class AssetRefTextAsset : AssetBaseRef<TextAsset> { }
    [System.Serializable] public class AssetRefTexture : AssetBaseRef<Texture> { }
    [System.Serializable] public class AssetRefMesh : AssetBaseRef<Mesh> { }
    [System.Serializable] public class AssetRefAnimationClip : AssetBaseRef<AnimationClip> { }
    [System.Serializable] public class AssetRefAudioClip : AssetBaseRef<AudioClip> { }
}
