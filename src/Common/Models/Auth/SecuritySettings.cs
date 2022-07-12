namespace Common.Models.Auth;

public class SecuritySettings
{
    public SecuritySettings(bool restrictParallelSessions, int parallelSessionMaxCount)
    {
        RestrictParallelSessions = restrictParallelSessions;
        ParallelSessionMaxCount = parallelSessionMaxCount;
    }

    public bool RestrictParallelSessions { get; }
    public int ParallelSessionMaxCount { get; }
}