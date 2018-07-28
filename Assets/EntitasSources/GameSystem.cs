public sealed class GameSystem : Feature
{
    public GameSystem(Contexts contexts)
    {
        Add(new DestroyEntitySystem(contexts));
        Add(new DataSaveAndLoadFeature(contexts));
        Add(new GrassLandSystemFeature(contexts));
        Add(new BattleSystemFeature(contexts));
    }
}
   
