using System;

namespace WFC
{
    public class Component<T> : IComparable<Component<T>>
    {
        public ComponentState<T> state;
        public ComponentState<T>[] possibleStates;
        public int x;
        public int y;

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

            foreach (var possibleState in possibleStates)
            {
                int randomResult = random.Next(0, summaryWeight);
                if (randomResult <= possibleState.weight)
                {
                    SetState(possibleState);
                    return state;
                }
            }

            SetState(possibleStates[0]);
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
            return other.possibleStates.Length - possibleStates.Length;
        }

        public Component<T> Copy()
        {
            var copy = new Component<T>(x, y, possibleStates);
            copy.state = state;
            return copy;
        }
    }
}

