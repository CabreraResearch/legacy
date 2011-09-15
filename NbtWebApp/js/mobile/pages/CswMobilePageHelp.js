/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../../jquery/common/CswCookie.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../sync/CswMobileSync.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../sync/CswMobileBackgroundTask.js" />

//#region CswMobilePageHelp

function CswMobilePageHelp(helpDef,$parent,mobileStorage) {
    /// <summary>
    ///   Help Page class. Responsible for generating a Mobile help page.
    /// </summary>
    /// <param name="helpDef" type="Object">Help definitional data.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageHelp">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var $content = '';
    var pageDef = { };
    var id = CswMobilePage_Type.help.id;
    var title = CswMobilePage_Type.help.title;
    var divSuffix = '_help';
    var contentDivId;
    
    //ctor
    (function() {
        if(isNullOrEmpty(mobileStorage)) {
            mobileStorage = new CswMobileClientDbResources();
        }
    
        var p = {
            level: -1,
            DivId: '',
            title: '',
            theme: CswMobileGlobal_Config.theme,
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
            onOnlineClick: function () {},
            onRefreshClick: function () {}
        };
        if (helpDef) $.extend(p, helpDef);
        
        if(!isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }

        contentDivId = id + divSuffix;
        
        if( !isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
        
        var buttons = { };
        buttons[CswMobileFooterButtons.online.name] = p.onOnlineClick;
        buttons[CswMobileFooterButtons.refresh.name] = p.onRefreshClick;
        buttons[CswMobileFooterButtons.fullsite.name] = '';
        buttons[CswMobileHeaderButtons.back.name] = '';

        pageDef = makeMenuButtonDef(p, id, buttons, mobileStorage);

        getContent();
    })();
    
    function getContent() {
        $content = ensureContent($content, contentDivId);
        
        var $help = $('<p>Help</p>').appendTo($content);

        if (debugOn()) //this is set onLoad based on the includes variable 'debug'
        {
            $help.append('</br></br></br>');
            var $logLevelDiv = $help.CswDiv('init')
                                    .CswAttrXml({ 'data-role': 'fieldcontain' });
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
                            .CswAttrXml({ 'data-role': 'slider' });

        }
    }
    
    //#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.contentDivId = contentDivId;
    this.pageDef = pageDef;
    this.id = id;
    this.title = title;
    this.getContent = getContent;
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageHelp