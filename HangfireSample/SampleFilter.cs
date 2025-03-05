using Hangfire.Client;

namespace HangfireSample;

internal class SampleFilter : IClientFilter
{
    public void OnCreating(CreatingContext context)
    {
        context.Parameters["Anonymous"] = new { π = 3.14m };
    }

    public void OnCreated(CreatedContext context)
    {
    }
}