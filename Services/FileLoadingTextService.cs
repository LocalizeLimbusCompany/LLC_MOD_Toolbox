namespace LLC_MOD_Toolbox.Services
{
    public class FileLoadingTextService(IList<string> strings, Random random) : ILoadingTextService
    {
        public Task<string> GetLoadingTextAsync() =>
            Task.FromResult(strings[random.Next(strings.Count - 1)]);
    }
}
