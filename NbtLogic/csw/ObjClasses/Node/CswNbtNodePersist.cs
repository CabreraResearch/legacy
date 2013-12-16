namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodePersist
    {
        private ICswNbtNodePersistStrategy _PersistStrategy;

        public CswNbtNodePersist( ICswNbtNodePersistStrategy PersistStrategy )
        {
            _PersistStrategy = PersistStrategy;
        }      

        /// <summary>
        /// Calls the concrete strategy's postChanges function
        /// </summary>
        public void postChanges( CswNbtNode Node )
        {
            _PersistStrategy.postChanges( Node );
        }

    }
}
