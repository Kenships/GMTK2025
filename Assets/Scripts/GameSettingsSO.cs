using Obvious.Soap;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingsSO", menuName = "Scriptable Objects/GameSettingsSO")]
public class GameSettingsSO : ScriptableObject
{
    public FloatVariable VolumeVar;
    public IntVariable QualityIndexVar;
    public ResolutionVariable ResolutionVar;
    public BoolVariable FullScreenVar;
    
    
    public float Volume => VolumeVar.Value;
    public int QualityIndex => QualityIndexVar.Value;
    
    public int ScreenWidth => ResolutionVar.Value.width;
    public int ScreenHeight => ResolutionVar.Value.height;
    public bool FullScreen => FullScreenVar.Value;
    
    
}
