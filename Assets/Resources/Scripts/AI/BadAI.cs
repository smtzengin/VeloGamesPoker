public class BadAI : AIClass
{
    private void Start()
    {
        Aggression = 1.5f;
        Caution = 0.5f;
        Stupidity = 1f;
        Randomness = 0.6f;
        RaiseThreshold = 2.7f;
        CallThreshold = 1.2f; //OLD 0.5f
    }
}
