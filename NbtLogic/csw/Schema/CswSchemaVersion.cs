using System;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Represents a Schema Version tag for NBT
    /// </summary>
    public class CswSchemaVersion : IComparable<CswSchemaVersion>, IEquatable<CswSchemaVersion>
    {
        /// <summary>
        /// The release cycle iteration (how many times we've gone from A to Z).  This rarely changes.
        /// </summary>
        public Int32 CycleIteration;
        /// <summary>
        /// Single character release identifier (A-Z).  This changes for every release.
        /// </summary>
        public char ReleaseIdentifier;
        /// <summary>
        /// Iteration within this release.  This changes for every script.
        /// </summary>
        public Int32 ReleaseIteration;

        /// <summary>
        /// Constructor to build a Schema Version tag from parts
        /// </summary>
        public CswSchemaVersion( Int32 inCycleIteration, char inReleaseIdentifier, Int32 inReleaseIteration )
        {
            _init( inCycleIteration, inReleaseIdentifier, inReleaseIteration );
        }

        /// <summary>
        /// Constructor for single integer version numbers (for backwards compatibility)
        /// </summary>
        public CswSchemaVersion( Int32 SingleIntegerVersion )
        {
            _init( 0, 'X', SingleIntegerVersion );
        }
        /// <summary>
        /// Constructor for string version of Schema Version tag.  Detects single integers for backwards compatibility.
        /// </summary>
        public CswSchemaVersion( string SchemaVersionAsString )
        {
            if( SchemaVersionAsString.Length < 6 && CswTools.IsInteger( SchemaVersionAsString ) )
            {
                // Example: 74   (backwards compatibility)
                _init( 0, 'X', CswConvert.ToInt32( SchemaVersionAsString ) );
            }
            else
            {
                // Example: 01F-02

                Int32 CycleIteration = CswConvert.ToInt32( SchemaVersionAsString.Substring( 0, 2 ) );
                char ReleaseIdentifier = SchemaVersionAsString.Substring( 2, 1 )[0];

                //case 27448: variable length iteration segment (retro-handle 2-character segments and forward-handle 3-character segments)
                Int32 ReleaseIteration = CswConvert.ToInt32( SchemaVersionAsString.Substring( 4, SchemaVersionAsString.Length - 4 ) );

                _init( CycleIteration, ReleaseIdentifier, ReleaseIteration );
            }
        }

        private void _init( Int32 inCycleIteration, char inReleaseIdentifier, Int32 inReleaseIteration )
        {
            CycleIteration = inCycleIteration;
            ReleaseIdentifier = inReleaseIdentifier;
            ReleaseIteration = inReleaseIteration;

        }

        /// <summary>
        /// String version of Schema Version tag, e.g. 01F-02
        /// </summary>
        public override string ToString()
        {
            string ret = "";
            ret += CswTools.PadInt( CycleIteration, 2 );
            ret += ReleaseIdentifier.ToString();
            ret += "-";
            ret += CswTools.PadInt( ReleaseIteration, 3 );//27448: going forward, three character iteration segments
            return ret;
        }

        #region IComparable<CswSchemaVersion> Members

        public static bool operator <( CswSchemaVersion ver1, CswSchemaVersion ver2 )
        {
            return ver1.CompareTo( ver2 ) < 0;
        }
        public static bool operator >( CswSchemaVersion ver1, CswSchemaVersion ver2 )
        {
            return ver1.CompareTo( ver2 ) > 0;
        }

        public Int32 CompareTo( CswSchemaVersion other )
        {
            Int32 ret = 0;
            if( this.CycleIteration != other.CycleIteration )
                ret = this.CycleIteration.CompareTo( other.CycleIteration );
            else if( this.ReleaseIdentifier != other.ReleaseIdentifier )
                ret = this.ReleaseIdentifier.CompareTo( other.ReleaseIdentifier );
            else
                ret = this.ReleaseIteration.CompareTo( other.ReleaseIteration );
            return ret;
        }

        #endregion

        #region IEquatable<CswSchemaVersion> Members

        public static bool operator ==( CswSchemaVersion version1, CswSchemaVersion version2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( version1, version2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) version1 == null ) || ( (object) version2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( version1.CycleIteration == version2.CycleIteration &&
                version1.ReleaseIdentifier == version2.ReleaseIdentifier &&
                version1.ReleaseIteration == version2.ReleaseIteration )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=( CswSchemaVersion version1, CswSchemaVersion version2 )
        {
            return !( version1 == version2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswSchemaVersion ) )
                return false;
            return this == (CswSchemaVersion) obj;
        }

        public bool Equals( CswSchemaVersion obj )
        {
            return this == (CswSchemaVersion) obj;
        }

        public override int GetHashCode()
        {                                                            // For 01A-16:
            return this.CycleIteration * 100000 +                    // 100000
                   Convert.ToInt16( this.ReleaseIdentifier ) * 100 +   // 106500
                   this.ReleaseIteration;                            // 106516
        }

        #endregion

        #region SchemaDefinition

        public enum NbtTables
        {
            actions,
            configuration_variables,
            containers,
            data_dictionary,
            field_types,
            fkey_definitions,
            inventory_groups,
            jct_dd_ntp,
            jct_modules_actions,
            jct_modules_nodetypes,
            jct_modules_objectclass,
            jct_nodes_props,
            jct_nodes_props_audit,
            license,
            license_accept,
            locations,
            materials,
            materials_subclass,
            materials_synonyms,
            modules,
            nodes,
            nodes_audit,
            nodetypes,
            nodetypes_audit,
            nodetype_props,
            nodetype_props_audit,
            nodetype_tabset,
            node_views,
            object_class,
            object_class_props,
            packages,
            packdetail,
            schedule_items,
            sequences,
            sessionlist,
            static_sql_selects,
            statistics,
            statistics_actions,
            statistics_nodetypes,
            statistics_reports,
            statistics_searches,
            statistics_views,
            units_of_measure,
            update_history,
            users,
            vendors,
            welcome
        }

        public enum ActionsColumns
        {
            actionid,
            actionname,
            showinlist,
            url,
            category
        }

        public enum ConfigurationVariablesColumns
        {
            configurationvariableid,
            deleted,
            description,
            variablename,
            variablevalue,
            issystem
        }

        public enum ContainersColumns
        {
            containerid,
            nodetypeid,
            nodename,
            deleted,
            pendingupdate,
            auditflag,
            conc_mass_uomid,
            conc_vol_uomid,
            concentration,
            customstatus,
            esigflag,
            receiptlotid,
            reconcileddate,
            reconcileduserid,
            reconcilestate,
            reconciliationid,
            reservegroupid,
            reservetype,
            retestdate,
            serialno,
            specificactivity,
            specificactivity_mass_uom,
            storpress,
            stortemp,
            useridmovereq,
            usetype,
            materialsynonymid,
            synonymname,
            barcodeid,
            assetno,
            containerclass,
            parentid,
            parentname,
            originalparentid,
            originalparentname,
            receiveddate,
            openeddate,
            expirationdate,
            containerstatus,
            containergroupcode,
            ownerid,
            ownername,
            projectid,
            project,
            locationid,
            locationname,
            locationidmovereq,
            movereqlocationname,
            locationidhome,
            homelocationname,
            netquantity,
            unitofmeasurename,
            tarequantity,
            tareunitofmeasurename,
            packdetailid,
            packdetail,
            manufacturerlotno,
            manufactureddate,
            receivedcondition,
            notes,
            isdemo
        }

        public enum DataDictionaryColumns
        {
            allownonexistentfkref,
            columnname,
            columntype,
            datatypesize,
            dblprecision,
            defaultvalue,
            deleted,
            description,
            foreignkeycolumn,
            foreignkeytable,
            isview,
            logicaldelete,
            lowerrangevalue,
            lowerrangevalueinclusive,
            portabledatatype,
            readOnly,
            required,
            tablecolid,
            tablename,
            uniquetype,
            uperrangevalueinclusive,
            upperrangevalue
        }

        public enum FieldTypesColumns
        {
            auditflag,
            datatype,
            deleted,
            fieldprecision,
            fieldtype,
            fieldtypeid,
            mask
        }

        public enum FkeyDefinitionsColumns
        {
            deleted,
            fkeydefid,
            fkeyname,
            sql,
            pk_table,
            pk_column,
            ref_table,
            ref_column
        }

        public enum InventoryGroupsColumns
        {
            inventorygroupid,
            nodetypeid,
            nodename,
            deleted,
            pendingupdate,
            workunitid,
            inventorygroupname,
            inventorygroupcode,
            iscentralgroup,
            isdemo
        }

        public enum JctDdNtpColumns
        {
            jctddntpid,
            datadictionaryid,
            nodetypepropid,
            subfieldname
        }

        public enum JctModulesActionsColumns
        {
            jctmoduleactionid,
            actionid,
            moduleid,
            isdemo
        }

        public enum JctModulesNodeTypesColumns
        {
            jctmodulenodetypeid,
            nodetypeid,
            moduleid,
            isdemo
        }

        public enum JctModulesObjectClassColumns
        {
            deleted,
            jctmoduleobjectclassid,
            moduleid,
            objectclassid
        }

        public enum JctNodesPropsColumns
        {
            auditlevel,
            field1,
            field2,
            field3,
            jctnodepropid,
            nodeid,
            nodetypepropid,
            field1_fk,
            gestalt,
            field4,
            field5,
            blobdata,
            readOnly,
            pendingupdate,
            clobdata,
            nodeidtablename,
            field1_date,
            field1_numeric,
            field2_date,
            field2_numeric,
            objectclasspropid,
            isdemo
        }

        public enum JctNodesPropsAuditColumns
        {
            auditlevel,
            audittransactionid,
            deletedphysically,
            field1,
            field2,
            field3,
            jctnodepropauditid,
            jctnodepropid,
            nodeid,
            nodetypepropid,
            servermanaged,
            url,
            value,
            value_blob,
            value_fk,
            isdemo
        }

        public enum LicenseColumns
        {
            licenseid,
            licensetxt,
            activedate
        }

        public enum LicenseAcceptColumns
        {
            licenseacceptid,
            lastname,
            firstname,
            username,
            userid,
            acceptdate,
            licenseid,
            isdemo
        }

        public enum LocationsColumns
        {
            locationid,
            nodetypeid,
            nodename,
            deleted,
            pendingupdate,
            controlzoneid,
            locationlevel1id,
            locationlevel2id,
            locationlevel3id,
            locationlevel4id,
            locationlevel5id,
            descript,
            inventorygroupid,
            inventorygroup,
            ishomelocation,
            istransferlocation,
            locationcode,
            locationisinactive,
            pathname,
            reqdeliverylocation,
            selfservecode,
            isdemo
        }

        public enum MaterialsColumns
        {
            materialid,
            nodetypeid,
            nodename,
            deleted,
            pendingupdate,
            auditflag,
            biosafety,
            color,
            compressed_gas,
            const_chem_group,
            const_chem_react,
            const_coe_no,
            const_color_index,
            const_fema_no,
            const_ingred_class,
            const_mat_function,
            const_mat_origin,
            const_simple_name,
            const_uba_code,
            creationworkunitid,
            dot_code,
            goi,
            has_activity,
            istier2,
            keepatstatus,
            lastupdated,
            lob_type,
            manufacturer,
            material_finish,
            material_sizevol,
            material_type,
            material_use,
            model,
            productbrand,
            productcategory,
            producttype,
            refno,
            research_notes,
            reviewstatuschangedate,
            reviewstatusname,
            reviewstatustype,
            safety_notes,
            species,
            specific_code,
            specificcode,
            struct_pict,
            transgenic,
            type,
            variety,
            vectors,
            otherreferenceno,
            materialname,
            casno,
            einecs,
            inventoryrequired,
            keywords,
            creationdate,
            creationsiteid,
            creationsite,
            creationuserid,
            creationuser,
            expireinterval,
            expireintervalunits,
            openexpireinterval,
            openexpireintervalunits,
            nfpacode,
            healthcode,
            firecode,
            reactivecode,
            storage_conditions,
            ppe,
            exposure_limits,
            target_organs,
            hazards,
            disposal,
            formula,
            physical_description,
            physical_state,
            molecular_weight,
            specific_gravity,
            ph,
            boiling_point,
            melting_point,
            aqueous_solubility,
            flash_point,
            vapor_density,
            vapor_pressure,
            isdemo
        }

        public enum MaterialsSynonymsColumns
        {
            materialsynonymid,
            nodetypeid,
            nodename,
            deleted,
            pendingupdate,
            auditflag,
            charset,
            synonymname,
            materialid,
            materialname,
            sortorder,
            synonymclass,
            isdemo
        }

        public enum ModulesColumns
        {
            deleted,
            description,
            enabled,
            moduleid,
            name
        }

        public enum NodesColumns
        {
            auditlevel,
            nodeid,
            nodename,
            nodetypeid,
            pendingupdate,
            issystem,
            isdemo,
            hidden
        }

        public enum NodesAuditColumns
        {
            auditlevel,
            audittransactionid,
            deletedphysically,
            nodeauditid,
            nodeid,
            nodename,
            nodetypeid,
            isdemo
        }

        public enum NodeTypesColumns
        {
            auditlevel,
            iconfilename,
            nametemplate,
            nodetypeid,
            nodetypename,
            objectclassid,
            restrictchildren,
            tab_docs,
            tab_geo_detail,
            category,
            islocked,
            priorversionid,
            versionno,
            firstversionid,
            tablename,
            isdemo
        }

        public enum NodeTypesAuditColumns
        {
            auditlevel,
            audittransactionid,
            deletedphysically,
            iconfilename,
            locked,
            nametemplate,
            nodetypeauditid,
            nodetypeid,
            nodetypename,
            objectclassid,
            restrictchildren,
            tab_docs,
            tab_geo_detail,
            isdemo
        }

        public enum NodeTypePropsColumns
        {
            append,
            auditlevel,
            datetoday,
            display_col,
            display_row,
            fieldtypeid,
            fkvalue,
            isbatchentry,
            isfk,
            isrequired,
            isunique,
            length,
            nodetypeid,
            nodetypepropid,
            nodetypetabsetid,
            objectclasspropid,
            servermanaged,
            textareacols,
            textarearows,
            textlength,
            url,
            valuepropid,
            width,
            sequenceid,
            numberprecision,
            listoptions,
            compositetemplate,
            fktype,
            valueproptype,
            statictext,
            multi,
            nodeviewid,
            readOnly,
            display_col_add,
            display_row_add,
            setvalonadd,
            numberminvalue,
            numbermaxvalue,
            usenumbering,
            questionno,
            subquestionno,
            filter,
            filterpropid,
            firstpropversionid,
            priorpropversionid,
            valueoptions,
            defaultvalue,
            helptext,
            propname,
            defaultvalueid,
            isquicksearch,
            extended,
            hideinmobile,
            mobilesearch,
            isdemo
        }

        public enum NodeTypePropsAuditColumns
        {
            append,
            auditlevel,
            audittransactionid,
            colspan,
            deletedphysically,
            display_col,
            display_row,
            displayfieldid,
            fieldtypeid,
            filterclause,
            fkeydefid,
            grid_config,
            grid_sql,
            isbatchentry,
            isfk,
            isrequired,
            isset,
            isunique,
            length,
            nodetypeid,
            nodetypepropauditid,
            nodetypepropid,
            nodetypetabsetid,
            nodeviewid,
            objectclasspropid,
            pad,
            prepend,
            propname,
            rowspan,
            seqname,
            servermanaged,
            textareacols,
            textarearows,
            textlength,
            textvalue,
            url,
            valuefieldid,
            width,
            isdemo
        }

        public enum NodeTypeTabsetColumns
        {
            auditflag,
            nodetypeid,
            nodetypetabsetid,
            tabclass,
            tabname,
            taborder,
            includeinnodereport,
            isdemo
        }

        public enum NodeViewsColumns
        {
            auditflag,
            nodeviewid,
            roleid,
            userid,
            viewname,
            viewxml,
            visibility,
            category,
            formobile,
            isdemo
        }

        public enum ObjectClassColumns
        {
            auditlevel,
            objectclass,
            objectclassid,
            use_batch_entry,
            iconfilename
        }

        public enum ObjectClassPropsColumns
        {
            auditlevel,
            fieldtypeid,
            fkvalue,
            isbatchentry,
            isfk,
            isrequired,
            isunique,
            objectclassid,
            objectclasspropid,
            servermanaged,
            valuefieldid,
            numberprecision,
            listoptions,
            viewxml,
            fktype,
            multi,
            readOnly,
            display_col_add,
            display_row_add,
            setvalonadd,
            numberminvalue,
            numbermaxvalue,
            statictext,
            filter,
            filterpropid,
            valueoptions,
            propname,
            isglobalunique,
            extended,
            defaultvalueid
        }

        public enum PackagesColumns
        {
            packageid,
            nodetypeid,
            nodename,
            deleted,
            pendingupdate,
            materialid,
            materialname,
            manufacturerid,
            manufacturer,
            supplierid,
            supplier,
            productno,
            productdescription,
            obsolete,
            isdemo
        }

        public enum PackdetailColumns
        {
            packdetailid,
            nodetypeid,
            nodename,
            deleted,
            pendingupdate,
            qtypereach,
            uompereach,
            containertype,
            unitcount,
            packageid,
            packagename,
            capacity,
            catalogno,
            upc,
            unitofmeasureid,
            unitofmeasurename,
            packagedescription,
            dispenseonly,
            isdemo
        }

        public enum ScheduleItemsColumns
        {
            scheduleitemid,
            itemname,
            lastrun,
            isdemo
        }

        public enum SequencesColumns
        {
            sequenceid,
            sequencename,
            prep,
            post,
            pad,
            auditflag,
            isdemo
        }

        public enum SessionListColumns
        {
            sessionlistid,
            sessionid,
            accessid,
            userid,
            username,
            ipaddress,
            cswprimekey,
            roletimeoutminutes,
            timeoutdate,
            logindate,
            //ismobile, /* Case 26063 */
            isdemo
        }

        public enum StaticSqlSelectsColumns
        {
            datadictionaryid,
            dd_columnname,
            dd_tablename,
            deleted,
            queryid,
            querytext,
            staticsqlselectid
        }

        public enum StatisticsColumns
        {
            statisticsid,
            userid,
            logindate,
            logoutdate,
            count_lifecycles,
            count_multiedit,
            count_reportruns,
            count_viewloads,
            count_nodessaved,
            count_nodesadded,
            count_nodescopied,
            count_nodesdeleted,
            count_viewsedited,
            userloggedout,
            average_servertime,
            username,
            count_actionloads,
            count_errors,
            count_searches,
            count_viewfiltermods,
            isdemo
        }

        public enum StatisticsActionsColumns
        {
            statisticsactionid,
            statisticsid,
            actionname,
            hitcount,
            action,
            actionid,
            isdemo
        }

        public enum StatisticsNodeTypesColumns
        {
            statisticsnodetypeid,
            statisticsid,
            nodetypeid,
            hitcount,
            action,
            nodetypename,
            isdemo
        }

        public enum StatisticsReportsColumns
        {
            statisticsreportid,
            statisticsid,
            reportid,
            hitcount,
            action,
            reportname,
            isdemo
        }

        public enum StatisticsSearchesColumns
        {
            statisticssearchid,
            statisticsid,
            nodetypepropid,
            objectclasspropid,
            propname,
            hitcount,
            action,
            isdemo
        }

        public enum StatisticsViewsColumns
        {
            statisticsviewid,
            statisticsid,
            nodeviewid,
            hitcount,
            action,
            viewname,
            isdemo
        }

        public enum UnitsOfMeasureColumns
        {
            unitofmeasureid,
            nodetypeid,
            nodename,
            deleted,
            pendingupdate,
            unitofmeasurename,
            unittype,
            is_activity_type,
            convertfromeaches_base,
            convertfromeaches_exp,
            convertfromkgs_base,
            convertfromkgs_exp,
            convertfromliters_base,
            convertfromliters_exp,
            converttoeaches_base,
            converttoeaches_exp,
            converttokgs_base,
            converttokgs_exp,
            converttoliters_base,
            converttoliters_exp,
            isdemo
        }

        public enum UpdateHistoryColumns
        {
            updatehistoryid,
            updatedate,
            log,
            version,
            isdemo
        }

        public enum UsersColumns
        {
            userid,
            nodetypeid,
            nodename,
            deleted,
            pendingupdate,
            auditflag,
            defaultcategoryid,
            defaultprinterid,
            licenseagreementanddate,
            mystarturl,
            nodeviewid,
            password_old,
            welcomeredirect,
            workunitid,
            password_date,
            title,
            phone,
            email,
            supervisorid,
            supervisor,
            homeinventorygroupid,
            homeinventorygroup,
            defaultlocationid,
            defaultlocation,
            defaultlanguage,
            hidehints,
            issystemuser,
            disabled,
            navrows,
            username,
            password,
            roleid,
            namefirst,
            namelast,
            locked,
            failedlogincount,
            isdemo
        }

        public enum VendorsColumns
        {
            vendorid,
            nodetypeid,
            nodename,
            deleted,
            pendingupdate,
            vendorname,
            division,
            accountno,
            contactname,
            phone,
            fax,
            email,
            street1,
            street2,
            city,
            state,
            zip,
            country,
            isapprovedvendor,
            obsolete,
            isdemo
        }

        public enum WelcomeColumns
        {
            welcomeid,
            roleid,
            componenttype,
            display_row,
            display_col,
            nodeviewid,
            displaytext,
            nodetypeid,
            buttonicon,
            actionid,
            reportid,
            isdemo
        }

        #endregion
    }
}
