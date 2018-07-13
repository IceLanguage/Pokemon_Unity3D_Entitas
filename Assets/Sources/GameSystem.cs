public sealed class GameSystem : Feature
{
    public GameSystem(Contexts contexts)
    {
        Add(new DataSaveAndLoadFeature(contexts));
        Add(new GrassLandSystemFeature(contexts, ResourceController.Instance.grassMaterial));
        Add(new BattleSystemFeature(contexts));
    }
}
   
