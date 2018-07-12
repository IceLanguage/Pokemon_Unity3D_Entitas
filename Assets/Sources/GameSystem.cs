using Entitas;
public sealed class GameSystem : Feature
{
    public GameSystem(Contexts contexts)
    {
        Add(new GrassLandSystemFeature(contexts, ResourceController.Instance.glassMaterial));
    }
}
   
