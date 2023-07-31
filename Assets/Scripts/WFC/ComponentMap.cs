namespace WFC
{
    public class ComponentMap<T>
    {
        public Component<T>[,] map;

        public ComponentMap(int width, int height, ComponentState<T>[] possibleStates) 
        {
            map = new Component<T>[width, height];
            
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    map[i, j] = new Component<T>(i, j, possibleStates);
        }
        
        public void SetPossibleStatesToUndefinedComponents(ComponentState<T>[] possibleStates)
        {
            foreach(var component in map)
                if (component.state == null)
                    component.possibleStates = possibleStates;
        }
        
        public void SetStateToUndefinedComponents(ComponentState<T> state)
        {
            foreach (var component in map)
                if (component.state == null)
                    component.state = state;
        }

        public bool TryGetPriorityComponent(out Component<T> priorityComponent)
        {
            priorityComponent = null;

            foreach(var component in map)
            {
                if (component.state != null)
                    continue;

                if (priorityComponent == null || priorityComponent.CompareTo(component) > 0)
                    priorityComponent = component;
            }

            return priorityComponent != null;
        }

        public T[,] GetData()
        {
            var result = new T[map.GetUpperBound(0) + 1, map.GetUpperBound(1) + 1];

            for (int i = 0; i <= map.GetUpperBound(0); i++)
                for (int j = 0; j <= map.GetUpperBound(1); j++)
                    result[i, j] = map[i, j].state.data;

            return result;
        }

        public bool Solvable()
        {
            foreach (var component in map)
                if (component.state == null && component.possibleStates.Length == 0)
                    return false;

            return true;
        }

        public ComponentMapMemento<T> GetMapState(Component<T> changedComponent)
        {
            return new ComponentMapMemento<T>(map, changedComponent);
        }

        public void RestoreState(ComponentMapMemento<T> memento)
        {
            map = memento.mapBeforeChange;
            map[memento.componentX, memento.componentY].DeleteState(memento.settedState);
        }
    }
}

