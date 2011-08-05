/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../controls/CswMobileListView.js" />

//#region CswMobileObjectClassFactory

function CswMobileObjectClassFactory(ocDef, $div, mobileStorage) {
	/// <summary>
	///   Object class factory. Responsible for generating nodes according to Object Class rules.
	/// </summary>
    /// <param name="ocDef" type="Object">Object Class definitional data.</param>
	/// <param name="$div" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobileObjectClassFactory">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var $content;
    
    if (isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    
    //ctor
    (function () {
        
        var p = {
            objectClass: CswObjectClasses.GenericClass
        };
        if (ocDef) $.extend(p, ocDef);

        switch (p.objectClass) {
            case CswObjectClasses.InspectionDesignClass:
                break;
            default:
                break;
        }
        
        $content = '';
    })(); //ctor

    function getContent() {
        
    }
    
    function makeOcContent(json) {
		var ret = {
		    isLink: true,
		    $html: ''
		};
		var html = '';
		var nodeId = makeSafeId({ ID: json['id'] });
		var nodeSpecies = json['value']['nodespecies'];
		var nodeName = json['value']['node_name'];
		var icon = '';
		if (!isNullOrEmpty(json['value']['iconfilename'])) {
			icon = 'images/icons/' + json['value']['iconfilename'];
		}
		var objectClass = json['value']['objectclass'];

		if (nodeSpecies !== 'More')
		{
			if (!isNullOrEmpty(icon)) {
			    html += '<img src="' + icon + '" class="ui-li-icon"/>';
			}
		    
		    switch (objectClass) {
			case "InspectionDesignClass":
				var dueDate = tryParseString(json['value']['duedate'],'' );
				var location = tryParseString(json['value']['location'],'' );
				var mountPoint = tryParseString(json['value']['target'],'' );
				var status = tryParseString(json['value']['status'],'' );

				html += '<h2>' + nodeName + '</h2>';
				html += '<p>' + location + '</p>';
				html += '<p>' + mountPoint + '</p>';
				html += '<p>';
				if (!isNullOrEmpty(status)) html += status + ', ';
				html += 'Due: ' + dueDate + '</p>';
				break;
			}
		} //if( nodeSpecies !== 'More' )
		else {
			html += '<h2 id="' + nodeId + '">' + nodeName + '</h2>';
		    ret.isLink = false;
		}
        ret.$html = $(html);
			
		return ret;
    }
    
	//#endregion private
    
    //#region public, priveleged

    this.getContent = getContent;
    
    //#endregion public, priveleged
}

//#endregion CswMobileObjectClassFactory