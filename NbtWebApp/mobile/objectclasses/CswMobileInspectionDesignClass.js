/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../controls/CswMobileListView.js" />

//#region CswMobileInspectionDesignClass

function CswMobileInspectionDesignClass(ocDef) {
    /// <summary>
    ///   Inspection Design. Responsible for generating nodes according to Object Class rules.
    /// </summary>
    /// <param name="ocDef" type="Object">Object Class definitional data.</param>
    /// <returns type="CswMobileInspectionDesignClass">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var $content;
    var divSuffix = '_nodeitem';
    var contentDivId;
    //ctor
    (function () {
        
        var p = {
            nodekey: '',
            nodeName: '',
            location: '',
            target: '',
            status: '',
            duedate: ''            
        };
        if (ocDef) $.extend(p, ocDef);
        contentDivId = p.nodekey + divSuffix;
        $content = ensureContent($content, contentDivId);
        if (!isNullOrEmpty(p['node_name'])) {
            $content.append('<h2>' + p['node_name'] + '</h2>');
        }
        if (!isNullOrEmpty(p.location)) {
            $content.append('<p>' + p.location + '</p>');
        }
        if (!isNullOrEmpty(p.target)) {
            $content.append('<p>' + p.target + '</p>');
        }
        $content.append(makeStatusDate(p.status,p.duedate));
    })(); //ctor
    
    function makeStatusDate(status,date) {
        var ret = '';
        if (!isNullOrEmpty(status)) {
            ret = status;
        }
        if (!isNullOrEmpty(ret) && !isNullOrEmpty(date)) {
            ret += ', Due: ' + date;
        } 
        else if (!isNullOrEmpty(date)) {
            ret = 'Due: ' + date;
        }
        if (!isNullOrEmpty(ret)) {
            ret = '<p>' + ret + '</p>'; 
        }
        return ret;
    }
    //#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.contentDivId = contentDivId;
    
    //#endregion public, priveleged
}

//#endregion CswMobileInspectionDesignClass