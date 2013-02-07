using System;
using System.Collections;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.Actions
{
    /// <summary>
    /// Collection of NBT Actions
    /// </summary>
    public class CswNbtActionCollection : IEnumerable
    {
        private SortedList _ActionSL;
        private Hashtable _ActionHash;
        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtActionCollection( CswNbtResources Resources, bool ExcludeDisabledActions )
        {
            _CswNbtResources = Resources;

            _ActionSL = new SortedList();
            _ActionHash = new Hashtable();

            // Actions
            DataTable ActionsTable = null;
            if( ExcludeDisabledActions )
            {
                CswStaticSelect ActionsSelect = _CswNbtResources.makeCswStaticSelect( "CswNbtActionCollection.ActionsSelect", "getActiveActions" );
                ActionsTable = ActionsSelect.getTable();
            }
            else
            {
                CswTableSelect ActionsSelect = _CswNbtResources.makeCswTableSelect( "CswNbtActionCollection.AllActionsSelect", "actions" );
                ActionsTable = ActionsSelect.getTable();
            }

            foreach( DataRow ActionRow in ActionsTable.Rows )
            {
                try
                {
                    CswNbtActionName CurrentActionName = CswNbtAction.ActionNameStringToEnum( ActionRow["actionname"].ToString() );
                    if( CurrentActionName != CswNbtActionName.Unknown )
                    {
                        Int32 ActionId = CswConvert.ToInt32( ActionRow["actionid"] );
                        CswNbtAction Action = new CswNbtAction( _CswNbtResources,
                                                                ActionId,
                                                                ActionRow["url"].ToString(),
                                                                CurrentActionName,
                                                                CswConvert.ToBoolean( ActionRow["showinlist"] ),
                                                                ActionRow["category"].ToString(),
                                                                CswConvert.ToString( ActionRow["iconfilename"] ) );
                        string ActionNameAsString = CswNbtAction.ActionNameEnumToString( CurrentActionName );
                        if( false == _ActionSL.ContainsKey( ActionNameAsString ) )
                        {
                            _ActionSL.Add( ActionNameAsString, Action );
                        }
                        if( false == _ActionHash.ContainsKey( ActionId ) )
                        {
                            _ActionHash.Add( ActionId, Action );
                        }
                    }
                }
                catch( Exception ex )
                {
                    // Log the error but keep going
                    _CswNbtResources.logError( new CswDniException( ErrorType.Error, "System Error", "Encountered an invalid Action: " + ActionRow["actionname"] + " (" + ActionRow["actionid"] + ")", ex ) );
                }
            }

        }

        /// <summary>
        /// Find an action by primary key
        /// </summary>
        /// <param name="ActionId">Primary key of Action</param>
        public CswNbtAction this[Int32 ActionId]
        {
            get { return (CswNbtAction) _ActionHash[ActionId]; }
        }

        /// <summary>
        /// Find an action by ActionName
        /// </summary>
        /// <param name="ActionName">CswNbtActionName value for action</param>
        public CswNbtAction this[CswNbtActionName ActionName]
        {
            get { return (CswNbtAction) _ActionSL[CswNbtAction.ActionNameEnumToString( ActionName )]; }
        }

        /// <summary>
        /// Returns an alphabetically sorted enumerator
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return _ActionSL.Values.GetEnumerator();
        }


    }
}
