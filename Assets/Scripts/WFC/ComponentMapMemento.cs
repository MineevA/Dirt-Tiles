using System;

namespace WFC 
{
    public class ComponentMapMemento<T>
    {
        public readonly Component<T>[,] mapBeforeChange;
        public readonly int componentX;
        public readonly int componentY;
        public ComponentState<T> settedState;

        public ComponentMapMemento(Component<T>[,] map, Component<T> changedComponent)
        {
            mapBeforeChange = new Component<T>[map.GetLength(0), map.GetLength(1)];
            foreach(var component in map)
                mapBeforeChange[component.x, component.y] = component.Copy();

            componentX = changedComponent.x;
            componentY = changedComponent.y;
        }
    }
}