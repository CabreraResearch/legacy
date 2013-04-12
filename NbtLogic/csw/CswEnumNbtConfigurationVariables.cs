using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Audit;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Log;
using ChemSW.Mail;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.Security;
using ChemSW.RscAdo;
using ChemSW.Security;
using ChemSW.Session;
using ChemSW.StructureSearch;
using ChemSW.TblDn;

namespace ChemSW.Nbt
{
    public enum CswEnumNbtConfigurationVariables
    {
        unknown,

        /// <summary>
        /// 1 = auditing is on; 0 = auditing is off
        /// </summary>
        auditing,

        /// <summary>
        /// Image to display on every page
        /// </summary>
        brand_pageicon,

        /// <summary>
        /// Title to display on every page
        /// </summary>
        brand_pagetitle,

        /// <summary>
        /// Records the last time Nbt Meta Data was altered
        /// </summary>
        cache_lastupdated,

        /// <summary>
        /// If set to 1, users can define their own barcodes on new containers.
        /// </summary>
        custom_barcodes,

        /// <summary>
        /// Format of database (oracle, mysql, mssql)
        /// </summary>
        dbformat,

        /// <summary>
        /// If 1, display error messages in the web interface.
        /// </summary>
        displayerrorsinui,

        /// <summary>
        /// Number of failed login attempts before a user's account is locked.
        /// </summary>
        failedloginlimit,

        /// <summary>
        /// Number of Generators to process in each scheduler cycle
        /// </summary>
        generatorlimit,

        /// <summary>
        /// Number of Targets to generate from a Generator in each scheduler cycle
        /// </summary>
        generatortargetlimit,

        /// <summary>
        /// If 1, Schema is in Demo mode
        /// </summary>
        is_demo,

        /// <summary>
        /// Enforce license agreement on all users
        /// </summary>
        license_type,

        /// <summary>
        /// Maximum depth of location controls
        /// </summary>
        loc_max_depth,

        /// <summary>
        /// If 1, use image-based location controls
        /// </summary>
        loc_use_images,

        /// <summary>
        /// Number of results to display for views on Mobile
        /// </summary>
        mobileview_resultlim,

        /// <summary>
        /// When set to 1, total quantity to deduct in DispenseContainer cannot exceed container netquantity.
        /// </summary>
        netquantity_enforced,

        /// <summary>
        /// Number of days before a password expires
        /// </summary>
        passwordexpiry_days,

        /// <summary>
        /// User password complexity level (0 - none; 1 - letters, numbers; 2 - letters, numbers, and symbols)
        /// </summary>
        password_complexity,

        /// <summary>
        /// User password minimum length (between 0 and 20)
        /// </summary>
        password_length,

        /// <summary>
        /// Unique identifier for the schema structure
        /// </summary>
        schemaid,

        /// <summary>
        /// Version of this Schema
        /// </summary>
        schemaversion,

        /// <summary>
        /// Show the Loading box on postback
        /// </summary>
        showloadbox,

        /// <summary>
        /// Maximum number of results per tree level
        /// </summary>
        treeview_resultlimit,

        /// <summary>
        /// Limit at which relationship values must be searched for
        /// </summary>
        relationshipoptionlimit,

        /// <summary>
        /// Limit the number of containers allowed to receive in a single operation
        /// </summary>
        container_receipt_limit,

        /// <summary>
        /// The maximum number of lines in comments fields
        /// </summary>
        total_comments_lines,

        /// <summary>
        /// The name of the root level item on location views
        /// </summary>
        LocationViewRootName
    };

}