using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public abstract class CswUpdateSchemaTo
    {
        protected CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
        {
            set { _CswNbtSchemaModTrnsctn = value; }
        }

        //        public abstract CswSchemaVersion SchemaVersion { get; }
        //public abstract string Description { set; get; }

        private string _Description = string.Empty;
        public virtual string Description { set { _Description = value; } get { return ( _Description ); } }

        /// <summary>
        /// The logic to execute in each Schema Script
        /// </summary>
        public abstract void update();

        /// <summary>
        /// The author of the script
        /// </summary>
        public abstract CswDeveloper Author { get; }
    }
}
