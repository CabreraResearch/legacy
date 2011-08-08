/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="CswMobileInspectionDesignClass.js" />
/// <reference path="CswMobileGenericClass.js" />

//#region CswMobileNodesFactory

function CswMobileNodesFactory(ocDef) {
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
        if(nodePk.hasOwnProperty(1)) {
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
                    if (!isNullOrEmpty(p.iconfilename)) {
                        icon = 'images/icons/' + p.iconfilename;
                    }
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