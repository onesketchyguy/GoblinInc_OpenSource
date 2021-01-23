namespace LowEngine
{
    /// <summary>
    /// We should have contracts be a public item so that modders may fuck with them
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// How much money do we get paid for this contract?
        /// </summary>
        public double money;

        /// <summary>
        /// What is this contract?
        /// </summary>
        public string description;

        /// <summary>
        /// Who is supplying this contract?
        /// </summary>
        public string hiring;

        /// <summary>
        /// How much effor does the user need to put in?
        /// This is also kind of the difficulty
        /// </summary>
        public float workCost;

        private double completion;

        public void PutInWork(float amount)
        {
            completion += amount * workCost;
        }

        public bool IsCompleted()
        {
            return completion >= 1.0f;
        }
    }
}