namespace CleanArchitecture.Common.Patterns
{
    public interface IBuilder<out T>
    {
        T Build();
    }
}
