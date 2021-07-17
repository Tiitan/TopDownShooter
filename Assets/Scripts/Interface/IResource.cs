namespace Interface
{
    public interface IResource
    {
        bool TryConsumeResource(string resourceName, int energyCost);
    }
}
