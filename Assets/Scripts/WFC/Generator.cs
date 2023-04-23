using System.Collections.Generic;

namespace WFC
{
    public delegate void StateChangeHandler<T>(Component<T> component, ref Component<T>[,] map);

    public class Generator<T>
    {
        private readonly ComponentState<T>[] possibleStates;
        private readonly StateChangeHandler<T> stateChangeHandler;
        
        private ComponentMap<T> componentMap;
        private Stack<ComponentMapMemento<T>> mapHistory;

        public Generator(T[] possibleStates, StateChangeHandler<T> stateChangeHandler)
        {
            this.stateChangeHandler = stateChangeHandler;
            this.possibleStates = new ComponentState<T>[possibleStates.Length];

            for (int i = 0; i < possibleStates.Length; i++)
                this.possibleStates[i] = new ComponentState<T>(possibleStates[i], 1);
        }

        public T[,] Generate(int width, int height)
        {
            componentMap = new ComponentMap<T>(width, height, possibleStates);
            mapHistory = new Stack<ComponentMapMemento<T>>(width * height);

            FillComponents();

            return componentMap.GetData();
        }

        private void FillComponents()
        {
            while (componentMap.TryGetPriorityComponent(out Component<T> component))
            {
                var mapMemento = componentMap.GetMapState(component);
                var state = component.ChooseRandomState();
                
                mapMemento.settedState = state;
                mapHistory.Push(mapMemento);

                stateChangeHandler.Invoke(component, ref componentMap.map);

                while (!componentMap.Solvable())
                    componentMap.RestoreState(mapHistory.Pop());
            }
        }
    }
}

