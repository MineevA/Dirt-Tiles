namespace WFC
{
    public class ComponentState<T>
    {
        public readonly T data;
        public int weight;

        public ComponentState(T data, int weight)
        {
            this.data = data;
            this.weight = weight;
        }
    }
}
