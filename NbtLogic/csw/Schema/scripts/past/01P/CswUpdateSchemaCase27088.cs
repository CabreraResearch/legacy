using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27088
    /// </summary>
    public class CswUpdateSchemaCase27088 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Fix bugs introduced in Othello's CswUpdateSchemaCase25981.cs

            
            // Remove erroneous extra value
            string Sql = @"delete from jct_nodes_props 
                            where jctnodepropid in (select j.jctnodepropid
                                                      from jct_nodes_props j
                                                      join nodetype_props p on j.nodetypepropid = p.nodetypepropid
                                                      join nodes n on j.nodeid = n.nodeid
                                                      join nodetypes t1 on p.nodetypeid = t1.nodetypeid
                                                      join nodetypes t2 on n.nodetypeid = t2.nodetypeid
                                                     where t1.nodetypeid <> t2.nodetypeid)";
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( Sql );


            // Set up Lab 2 correctly
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            Collection<CswNbtNode> LocationNodes = LocationOC.getNodes( false, true );

            CswNbtObjClassLocation LocationNRL = null;
            foreach( CswNbtObjClassLocation LocationNode in LocationNodes )
            {
                if( LocationNode.Name.Text == "North Research Lab" )
                {
                    LocationNRL = LocationNode;
                }
            } // foreach( CswNbtObjClassLocation LocationNode in LocationNodes )

            if( LocationNRL != null )
            {
                foreach( CswNbtObjClassLocation LocationNode in LocationNodes )
                {
                    if( LocationNode.Name.Text == string.Empty && LocationNode.IsDemo )
                    {
                        LocationNode.Name.Text = "Lab 2";
                        LocationNode.Location.SelectedNodeId = LocationNRL.NodeId;
                        LocationNode.postChanges( true );
                    }
                } // foreach( CswNbtObjClassLocation LocationNode in LocationNodes )
            } // if( LocationNRL != null )

        }//Update()

    }//class CswUpdateSchemaCase27088

}//namespace ChemSW.Nbt.Schema