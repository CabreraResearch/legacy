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
        bool SkipEvents { get; set; }
        bool AllowAuditing { get; set; }
        bool Creating { get; set; }

        void postChanges( CswNbtNode Node );
    }
}
