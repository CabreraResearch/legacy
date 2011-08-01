/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../_Global.js" />
/// <reference path="../../_CswPrototypeExtensions.js" />
/// <reference path="CswMobileMenuButton.js" />
/// <reference path="ICswMobileWebControls.js" />

//#region CswMobilePageFooter

CswMobilePageFooter.inheritsFrom(ICswMobileWebControls);

function CswMobilePageFooter(footerDef, $parent) {
    /// <summary>
    ///   Footer class. Responsible for generating a Mobile page footer.
    /// </summary>
    /// <param name="footerDef" type="Object">JSON definition of buttons to display</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <returns type="CswMobilePageFooter">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    ICswMobileWebControls.call(this);
    
    var o = {
        buttons: { button1: { ID: '', text: '', 'data-icon': '', cssclass: '' } },
        ID: '',
        dataId: 'csw_footer',
        dataTheme: 'b'
    };
    
    if(footerDef) $.extend(o, footerDef);

    var $footerCtn;
    var buttonNames = [];
    var footerId = o.ID + '_footer';

    var $footer = $parent.find('div:jqmData(role="footer")');

    if( isNullOrEmpty($footer) || $footer.length === 0 )
    {
        $footer = $parent.CswDiv('init', { ID: footerId })
        .CswAttrXml({
                'data-role': 'footer',
                'data-position': 'fixed',
                'data-id': o.dataId,
                'data-theme': o.dataTheme
            });
        var $footerNav = $('<div data-role="navbar">').appendTo($footer);
        $footerCtn = $('<ul class="csw_fieldctn"></ul>').appendTo($footerNav);
    } else {
        $footerCtn = $footer.find('.csw_fieldctn');  
    }
    
    //#endregion private
    
    //#region sheol
    
    //let's make these buttons accessible by name
    var buttonCnt = 0;
    for( var buttonName in o.buttons) {
        buttonCnt++;

        var thisButton = o.buttons[buttonName];
        var $buttonWrp = $footerCtn.find('#' + thisButton.ID + '_li');
        if( isNullOrEmpty($buttonWrp) || $buttonWrp.length === 0) {
            $buttonWrp = $('<li id="' +  thisButton.ID + '_li"></li>');
        }
        
        if(buttonCnt <= 4) {
            var button = new CswMobileMenuButton(thisButton, $buttonWrp.appendTo($footerCtn));
            buttonNames.push(buttonName);
            this[buttonName] = button;
        } 
    }
    
    //#endregion sheol
    
    //#region public, priveleged
    this.ID = o.ID;
    this.$content = $footer;
    
    //in case we don't know the button names, we can fetch and query
    this.buttonNames = buttonNames;
    //#endregion public, priveleged
    
}

//#endregion CswMobilePageFooter