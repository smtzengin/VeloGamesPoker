public class GoodAI : AIClass
{
    private void Start()
    {
        Aggression = 0.8f;
        Caution = 0.8f;
        Stupidity = -0.2f;
        Randomness = 0.4f;
        RaiseThreshold = 2.55f;
        CallThreshold = 1.1f; //OLD 0.34
    }
}