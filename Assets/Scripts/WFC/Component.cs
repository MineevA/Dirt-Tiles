using System;

namespace WFC
{
    public class Component<T> : IComparable<Component<T>>
    {
        public ComponentState<T> state;
        public ComponentState<T>[] possibleStates;
        public readonly int x;
        public readonly int y;
        public int priorityWeight;

        private Random random = new Random();

        public Component(int x, int y, ComponentState<T>[] possibleStates)
        {
            this.x = x;
            this.y = y;
            this.possibleStates = possibleStates;
        }

        public ComponentState<T> ChooseRandomState()
        {
            var summaryWeight = 0;
            foreach (var possibleState in possibleStates)
                summaryWeight += possibleState.weight;

            var randomResult = random.Next(1, summaryWeight + 1);
            
            foreach (var possibleState in possibleStates)
            {
                randomResult -= possibleState.weight;
                if (randomResult <= 0)
                {
                    SetState(possibleState);
                    return state;
                }
            }

            SetState(possibleStates[possibleStates.Length - 1]);
            return state;
        }

        private void SetState(ComponentState<T> state)
        {
            this.state = state;
            DeleteState(state);
        }

        public void DeleteState(ComponentState<T> state)
        {
            var newArrayLength = possibleStates.Length;

            foreach (var possibleState in possibleStates)
                if (possibleState.Equals(state))
                    newArrayLength--;

            var newPossibleStates = new ComponentState<T>[newArrayLength];
            var i = 0;
            foreach (var possibleState in possibleStates)
                if (!possibleState.Equals(state))
                {
                    newPossibleStates[i] = possibleState;
                    i++;
                }

            possibleStates = newPossibleStates;
        }

        public int CompareTo(Component<T> other)
        {
            var diff = other.possibleStates.Length - possibleStates.Length;
            
            if (diff == 0)
                diff = priorityWeight - other.priorityWeight;

            return diff;
        }

        public Component<T> Copy()
        {
            var copy = new Component<T>(x, y, possibleStates);
            copy.state = state;
            return copy;
        }
    }
}

