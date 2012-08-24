/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="CswMobileInspectionDesignClass.js" />
/// <reference path="CswMobileGenericClass.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

//#region CswMobileNodesFactory

function CswMobileNodesFactory(ocDef) {
    "use strict";
    /// <summary>
    ///   Object class factory. Responsible for generating nodes according to Object Class rules.
    /// </summary>
    /// <param name="ocDef" type="Object">Object Class definitional data.</param>
    /// <returns type="CswMobileNodesFactory">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var $content, nodeKey, nodeId, nodeSpecies, nodeName, objectClass, icon;
    
    //ctor
    (function () {
        
        var p = {
            nodeKey: '',
            'node_name': '',
            nodespecies: '',
            iconfilename: ''
            //ocprops follow
        };
        if (ocDef) $.extend(p, ocDef);

        nodeKey = makeSafeId({ ID: p.nodeKey });
        nodeName = 'No Results';
        icon = '';
        
        var nodePk = nodeKey.split('_');
        if(contains(nodePk, 1)) {
            nodeId = nodeKey[1];
        }
        if(Int32MinVal !== nodeId && 'No Results' !== p) {

            nodeName = p['node_name'];
            var species = tryParseString(p.nodespecies, '');
            switch (species) {
                case CswNodeSpecies.More.name:
                    nodeSpecies = CswNodeSpecies.More;
                    break;
                default:
                    nodeSpecies = CswNodeSpecies.Plain;
                    icon = tryParseString(p.iconfilename);
                    break;
            }

            var oClass = p.objectclass;
            var node;
  
            switch (oClass) {
                case CswObjectClasses.InspectionDesignClass.name:
                    objectClass = CswObjectClasses.InspectionDesignClass;
                    node = new CswMobileInspectionDesignClass(p);
                    break;
                default:
                    objectClass = CswObjectClasses.GenericClass;
                    node = new CswMobileGenericClass(p);
                    break;
            }

            $content = node.$content;
        }
    })(); //ctor

    //#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.nodeId = nodeId;
    this.nodeKey = nodeKey;
    this.nodeSpecies = nodeSpecies;
    this.nodeName = nodeName;
    this.objectClass = objectClass;
    this.icon = icon;
    //#endregion public, priveleged
}

//#endregion CswMobileNodesFactory