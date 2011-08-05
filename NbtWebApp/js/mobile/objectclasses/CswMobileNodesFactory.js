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

    var $content, nodeId, nodeSpecies, nodeName, objectClass;
    
    //ctor
    (function () {
        
        var p = {
            nodeId: '',           
            objectClass: CswObjectClasses.GenericClass,
            json: {},
            onClick: null // function () {}
        };
        if (ocDef) $.extend(p, ocDef);

		nodeId = makeSafeId({ ID: p.nodeId });
		nodeName = ['node_name'];
        
        var icon = '';
        var species = tryParseString(json.nodespecies,'');
        switch (species) {
            case CswNodeSpecies.More.name:
                nodeSpecies = CswNodeSpecies.More;
                break;
            default:
                nodeSpecies = CswNodeSpecies.Plain;
		        if (!isNullOrEmpty(p.json.iconfilename)) {
			        icon = 'images/icons/' + p.json.iconfilename;
		        }
                break;
        }

        var class = p.json.objectclass;
        var node;		
        switch (class) {
            case CswObjectClasses.InspectionDesignClass.name:
                objectClass = CswObjectClasses.InspectionDesignClass;
                node = new Cs
                break;
            default:
                objectClass = CswObjectClasses.GenericClass;
                break;
        }

        $content = '';
    })(); //ctor

    function getContent() {
        
    }
    
//    function makeOcContent(json) {
//		
//		    
//		    switch (objectClass) {
//			case "InspectionDesignClass":
//				var dueDate = tryParseString(json['value']['duedate'],'' );
//				var location = tryParseString(json['value']['location'],'' );
//				var mountPoint = tryParseString(json['value']['target'],'' );
//				var status = tryParseString(json['value']['status'],'' );

//				html += '<h2>' + nodeName + '</h2>';
//				html += '<p>' + location + '</p>';
//				html += '<p>' + mountPoint + '</p>';
//				html += '<p>';
//				if (!isNullOrEmpty(status)) html += status + ', ';
//				html += 'Due: ' + dueDate + '</p>';
//				break;
//			}
//		} //if( nodeSpecies !== 'More' )
//		else {
//			html += '<h2 id="' + nodeId + '">' + nodeName + '</h2>';
//		    ret.isLink = false;
//		}
//        ret.$html = $(html);
//			
//		return ret;
//    }
    
	//#endregion private
    
    //#region public, priveleged

    this.getContent = getContent;
    this.$content = $content;
    this.nodeId = nodeId;
    this.nodeSpecies = nodeSpecies;
    this.nodeName = nodeName;
    this.objectClass = objectClass;
    //#endregion public, priveleged
}

//#endregion CswMobileNodesFactory