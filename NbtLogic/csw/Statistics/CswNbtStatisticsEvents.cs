using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Configuration;
using ChemSW.Core;
using ChemSW.Encryption;
using ChemSW.Exceptions;
using ChemSW.Session;
using System.Text.RegularExpressions;
using ChemSW.Security;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;

namespace ChemSW.Nbt.Statistics
{
    //Sergei: This class is completely unimplemented because we need
    //to have a generic class called CswStatisticsRecorder that 
    //provides a mechanism for recording arbitrary statistics in a long
    //table, and that then uses ICswSessionStorage. I'm leaving
    //in the commented-out old implementation so we can remember 
    //what we used to do. 
    //--Dimitri
    public class CswNbtStatisticsEvents
    {
        public CswNbtStatisticsEntry CswNbtStatisticsEntry = null;

        private bool _RecordStatistics = false;


        public CswNbtStatisticsEvents( bool RecordStatistics )
        {
            _RecordStatistics = RecordStatistics;
        }


        public void OnAddNode( CswNbtNode Node )
        {
            if ( _RecordStatistics )
            {


                if ( Node != null )
                {
                    CswNbtStatisticsEntry.Stats_count_nodesadded++;
                    CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.NodeTypesAdded, Node.NodeTypeId.ToString() );
                }
            }
        }
        public void OnCopyNode( CswNbtNode OldNode, CswNbtNode NewNode )
        {
            if ( _RecordStatistics )
            {


                if ( NewNode != null )
                {
                    CswNbtStatisticsEntry.Stats_count_nodescopied++;
                    CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.NodeTypesCopied, NewNode.NodeTypeId.ToString() );
                }
            }
        }
        public void OnWriteNode( CswNbtNode Node, bool ForceSave, bool IsCopy )
        {
            if ( _RecordStatistics )
            {

                if ( Node != null )
                {
                    CswNbtStatisticsEntry.Stats_count_nodessaved++;
                    CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.NodeTypesSaved, Node.NodeTypeId.ToString() );
                }
            }
        }
        public void OnDeleteNode( CswNbtNode Node )
        {
            if ( _RecordStatistics )
            {
                if ( Node != null )
                {
                    CswNbtStatisticsEntry.Stats_count_nodesdeleted++;
                    CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.NodeTypesDeleted, Node.NodeTypeId.ToString() );
                }
            }
        }
        public void OnLoadView( CswNbtView View )
        {
            if ( _RecordStatistics )
            {
                if ( null != View )
                {
                    CswNbtStatisticsEntry.Stats_count_viewloads++;
                    if ( View.ViewId > 0 )
                        CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.ViewsLoaded, View.ViewId.ToString() );
                }
            }
        }
        public void OnLoadSearch( CswNbtViewProperty ViewProp )
        {
            if ( _RecordStatistics )
            {
                if ( null != ViewProp )
                {
                    CswNbtStatisticsEntry.Stats_count_searches++;
                    if ( ViewProp.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                        CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.NodeTypePropsSearched, ViewProp.NodeTypePropId.ToString() );
                    else
                        CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.ObjectClassPropsSearched, ViewProp.ObjectClassPropId.ToString() );
                }
            }
        }
        public void OnModifyViewFilters( CswNbtView OldView, CswNbtView NewView )
        {
            if ( _RecordStatistics )
            {
                if ( ( null != OldView ) && ( null != NewView ) )
                {
                    CswNbtStatisticsEntry.Stats_count_viewfiltermod++;
                    foreach ( CswNbtViewPropertyFilter OldFilter in OldView.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewPropertyFilter ) )
                    {
                        foreach ( CswNbtViewPropertyFilter NewFilter in NewView.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewPropertyFilter ) )
                        {
                            if ( OldFilter.ArbitraryId == NewFilter.ArbitraryId &&
                                OldFilter.Value != NewFilter.Value )
                            {
                                CswNbtViewProperty ParentProp = ( CswNbtViewProperty )NewFilter.Parent;
                                if ( ParentProp.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                                    CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.NodeTypePropsFilterMod, ParentProp.NodeTypePropId.ToString() );
                                else
                                    CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.ObjectClassPropsFilterMod, ParentProp.ObjectClassPropId.ToString() );
                            }
                        }
                    }
                }
            }
        }

        public void OnLoadReport( CswPrimaryKey ReportId )
        {
            if ( _RecordStatistics )
            {
                if ( Int32.MinValue != ReportId.PrimaryKey )
                {
                    CswNbtStatisticsEntry.Stats_count_reportruns++;
                    CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.ReportsLoaded, ReportId.ToString() );
                }
            }
        }

        public void OnLoadAction( Int32 ActionId )
        {
            if ( _RecordStatistics )
            {
                if ( Int32.MinValue != ActionId )
                {
                    CswNbtStatisticsEntry.Stats_count_actionloads++;
                    CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.ActionsLoaded, ActionId.ToString() );
                }
            }
        }
        public void OnMultiModeEnabled( CswNbtView View )
        {
            if ( _RecordStatistics )
            {
                if ( null != View )
                {
                    CswNbtStatisticsEntry.Stats_count_multiedit++;
                    if ( View.ViewId > 0 )
                        CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.ViewsMultiEdited, View.ViewId.ToString() );
                }
            }
        }
        public void OnFinishEditingView( CswNbtView View )
        {
            if ( _RecordStatistics )
            {
                if ( null != View )
                {
                    CswNbtStatisticsEntry.Stats_count_viewsedited++;
                    if ( View.ViewId > 0 )
                        CswNbtStatisticsEntry.IncrementHash( CswNbtStatisticsEntry.ViewsEdited, View.ViewId.ToString() );
                }
            }
        }
        public void OnEndOfPageLifeCycle( CswTimer Timer )
        {
            if ( _RecordStatistics )
            {
                if ( null != Timer )
                {
                    // Store numbers to determine average page lifecycle
                    double ElapsedTime = Timer.ElapsedDurationInMilliseconds;
                    CswNbtStatisticsEntry.Stats_servertime_total += ElapsedTime;
                    CswNbtStatisticsEntry.Stats_servertime_count++;
                }
            }
        }

        public void OnBeforeLogout( ICswSession Me )
        {
            // we don't do this here, since clear() will do this after
            //SaveStatistics(this.SessionId);
            if ( _RecordStatistics )
            {
                CswNbtStatisticsEntry.Stats_LoggedOut = true;
            }
        }

        public void OnError( Exception ex )
        {
            if ( _RecordStatistics )
            {
                CswNbtStatisticsEntry.Stats_errors++;
            }
        }


    }//CswStatisticsNbt

}//ChemSW.Nbt
