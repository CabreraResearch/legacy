using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.DB;
using ChemSW.Core;
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
        public CswNbtActionCollection( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;

            _ActionSL = new SortedList();
            _ActionHash = new Hashtable();

            // Actions
            CswStaticSelect ActionsSelect = _CswNbtResources.makeCswStaticSelect( "CswNbtActionCollection.ActionsSelect", "getActiveActions" );
            DataTable ActionsTable = ActionsSelect.getTable();
            foreach( DataRow ActionRow in ActionsTable.Rows )
            {
                CswNbtActionName CurrentActionName;
                try
                {
                    CurrentActionName = CswNbtAction.ActionNameStringToEnum( ActionRow["actionname"].ToString() );
                    Int32 ActionId = CswConvert.ToInt32( ActionRow["actionid"] );
                    CswNbtAction Action = new CswNbtAction( _CswNbtResources, ActionId, ActionRow["url"].ToString(), CurrentActionName, ( ActionRow["showinlist"].ToString() == "1" ), ActionRow["category"].ToString() );
                    _ActionSL.Add( CswNbtAction.ActionNameEnumToString( CurrentActionName ), Action );
                    _ActionHash.Add( ActionId, Action );
                }
                catch( Exception ex )
                {
                    // Log the error but keep going
                    _CswNbtResources.logError( new CswDniException( "System Error", "Encountered an invalid Action: " + ActionRow["actionname"] + " (" + ActionRow["actionid"] + ")", ex ) );
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
            get { return (CswNbtAction) _ActionSL[CswNbtAction.ActionNameEnumToString(ActionName)]; }
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
