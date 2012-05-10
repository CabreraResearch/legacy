// supports ScriptService attribute
using Newtonsoft.Json.Linq;



namespace ChemSW.Nbt.WebServices
{
    public abstract class CswNbtServiceLogic
    {


        public abstract JObject doLogic( CswNbtServiceLogicResources CswNbtServiceLogicResources );



    } // class CswNbtServiceLogic

} // namespace ChemSW.Nbt.WebServices
