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

//#region CswMobilePageHelp

function CswMobilePageHelp(helpDef, $parent, mobileStorage, $contentRole) {
    /// <summary>
    ///   Help Page class. Responsible for generating a Mobile help page.
    /// </summary>
    /// <param name="helpDef" type="Object">Help definitional data.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageHelp">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var pageDef = { };
    var id, title, contentDivId, $content,
        divSuffix = '_help';
    
    //ctor
    (function () {
        pageDef = {
            level: -1,
            DivId: '',
            buttons: [CswMobileFooterButtons.online, CswMobileFooterButtons.fullsite, CswMobileFooterButtons.refresh, CswMobileFooterButtons.help, CswMobileHeaderButtons.back ],
            title: '',
            theme: CswMobileGlobal_Config.theme
        };
        if (helpDef) {
            $.extend(pageDef, helpDef);
        }

        id = tryParseString(pageDef.DivId, CswMobilePage_Type.help.id);
        contentDivId = id + divSuffix;
        title = tryParseString(pageDef.title, CswMobilePage_Type.help.title);
        $content = ensureContent($contentRole, contentDivId);

        getContent();
    })();

    function getContent(onSuccess) {
        ///<summary></summary>
        ///<param name="refreshPageContent" type="Function"></param>
        ///<param name="onSuccess" type="Function"></param>
        $content = ensureContent($contentRole, contentDivId);
        
        var $help = $('<p>Help</p>').appendTo($content);

        if (debugOn()) { //this is set onLoad based on the includes variable 'debug'
            $help.append('</br></br></br>');
            var $logLevelDiv = $help.CswDiv('init')
                                    .CswAttrNonDom({ 'data-role': 'fieldcontain' });
            $('<label for="mobile_log_level">Logging</label>')
                                    .appendTo($logLevelDiv);

            $logLevelDiv.CswSelect('init', {
                                ID: 'mobile_log_level',
                                selected: debugOn() ? 'on' : 'off',
                                values: [{ value: 'off', display: 'Logging Disabled' },
                                    { value: 'on', display: 'Logging Enabled' }],
                                onChange: function($select) {
                                    if ($select.val() === 'on') {
                                        debugOn(true);
                                        $('.debug').css('display', '').show();
                                    } else {
                                        debugOn(false);
                                        $('.debug').css('diplay', 'none').hide();
                                    }
                                }
                            })
                            .CswAttrNonDom({ 'data-role': 'slider' });
            doSuccess(onSuccess, $contentRole);
        }
        $contentRole.append($content);
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

//#endregion CswMobilePageHelp