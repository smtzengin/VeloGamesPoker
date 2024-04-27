public class BadAI : AIClass
{
    private void Start()
    {
        Aggression = 1.5f;
        Caution = 0.5f;
        Stupidity = 1f;
        Randomness = 0.6f;
        RaiseThreshold = 1.8f;
        CallThreshold = 1.5f;
    }
}
