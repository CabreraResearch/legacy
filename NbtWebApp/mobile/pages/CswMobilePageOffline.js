/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../../main/tools/CswCookie.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../sync/CswMobileSync.js" />
/// <reference path="../sync/CswMobileBackgroundTask.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />

//#region CswMobilePageOffline

function CswMobilePageOffline(offlineDef, $parent, mobileStorage, $contentRole) {
    /// <summary>
    ///   Offline Page class. Responsible for generating a Mobile offline page.
    /// </summary>
    /// <param name="offlineDef" type="Object">Offline definitional data.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageOffline">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var pageDef = { };
    var id, title, contentDivId, $content,
        divSuffix = '_offline';
    
    //ctor
    (function () {
        pageDef = {
            level: -1,
            DivId: '',
            title: '',
            theme: CswMobileGlobal_Config.theme,
            buttons: [CswMobileFooterButtons.fullsite, CswMobileFooterButtons.help]
        };
        if (offlineDef) {
            $.extend(pageDef, offlineDef);
        }

        id = tryParseString(pageDef.DivId, CswMobilePage_Type.offline.id);
        contentDivId = id + divSuffix;
        title = tryParseString(pageDef.title, CswMobilePage_Type.offline.title);
        $content = ensureContent($contentRole, contentDivId);

        getContent();
    })();
    
    function getContent() {
        $content = ensureContent($contentRole, contentDivId);
        $content.append($('<p>You must have internet connectivity to login.</p>'));
    }
    
    //#endregion private
    
    //#region public, priveleged

    return {
        $pageDiv: $parent,
        $contentRole: $contentRole,
        $content: $content,
        contentDivId: contentDivId,
        pageDef: pageDef,
        id: id,
        title: title,
        getContent: getContent
    };
    //#endregion public, priveleged
}

//#endregion CswMobilePageOffline