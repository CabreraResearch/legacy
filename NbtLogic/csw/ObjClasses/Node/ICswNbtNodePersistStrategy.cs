namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// This interface defines ways to persist nodes based on state and context
    /// </summary>
    public interface ICswNbtNodePersistStrategy
    {
        bool ForceUpdate { get; set; }
        bool IsCopy { get; set; }
        bool OverrideUniqueValidation { get; set; }

        void postChanges( CswNbtNode Node );
    }
}
