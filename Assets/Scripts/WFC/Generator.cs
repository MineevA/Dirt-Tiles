using System.Collections.Generic;

namespace WFC
{
    public delegate void StateChangeHandler<T>(Component<T> component, ref Component<T>[,] map);
    public delegate void BeforeFillMap<T>(ref Component<T>[,] map);

    public class Generator<T>
    {
        private readonly ComponentState<T>[] possibleStates;
        private readonly StateChangeHandler<T> stateChangeHandler;
        private readonly BeforeFillMap<T> beforeFillMap;
        
        private ComponentMap<T> componentMap;
        private Stack<ComponentMapMemento<T>> mapHistory;

        public Generator(T[] possibleStates)
        {
            this.possibleStates = StatesFromData(possibleStates);
        }

        public Generator(T[] possibleStates, StateChangeHandler<T> stateChangeHandler)
        {
            this.stateChangeHandler = stateChangeHandler;
            this.possibleStates = StatesFromData(possibleStates);
        }

        public Generator(T[] possibleStates, StateChangeHandler<T> stateChangeHandler, BeforeFillMap<T> beforeFillMap)
        {
            this.possibleStates = StatesFromData(possibleStates);
            this.stateChangeHandler = stateChangeHandler;
            this.beforeFillMap = beforeFillMap;
        }

        public Generator(T[] possibleStates, BeforeFillMap<T> beforeFillMap)
        {
            this.possibleStates = StatesFromData(possibleStates);
            this.beforeFillMap = beforeFillMap;
        }

        private ComponentState<T>[] StatesFromData(T[] possibleStatesData)
        {
            var states = new ComponentState<T>[possibleStatesData.Length];
            for (int i = 0; i < possibleStatesData.Length; i++)
                states[i] = new ComponentState<T>(possibleStatesData[i], 1);

            return states;
        } 

        public T[,] Generate(int width, int height)
        {
            componentMap = new ComponentMap<T>(width, height, possibleStates);
            mapHistory = new Stack<ComponentMapMemento<T>>(width * height);

            beforeFillMap?.Invoke(ref componentMap.map);

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

