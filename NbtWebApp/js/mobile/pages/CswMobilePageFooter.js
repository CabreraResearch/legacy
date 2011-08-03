/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../_Global.js" />
/// <reference path="../../_CswPrototypeExtensions.js" />
/// <reference path="CswMobileMenuButton.js" />


//#region CswMobilePageFooter

function CswMobilePageFooter(footerDef, $parent) {
    /// <summary>
    ///   Footer class. Responsible for generating a Mobile page footer.
    /// </summary>
    /// <param name="footerDef" type="Object">JSON definition of buttons to display</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <returns type="CswMobilePageFooter">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private
    
    var o = {
        buttons: { button1: { ID: '', text: '', 'data-icon': '', cssclass: '' } },
        ID: '',
        dataId: 'csw_footer',
        dataTheme: 'b'
    };
    
    if(footerDef) $.extend(o, footerDef);

    var button1, button2, button3, button4, $footerCtn;
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
    for( var button in o.buttons) {
        buttonCnt++;
        switch(buttonCnt) {
            case 1:
                button1 = new CswMobileMenuButton(o.buttons[button], $footerCtn);
                this[button] = button1;
                break;
            case 2:
                button2 = new CswMobileMenuButton(o.buttons[button], $footerCtn);
                this[button] = button2;
                break;
            case 3:
                button3 = new CswMobileMenuButton(o.buttons[button], $footerCtn);
                this[button] = button3;
                break;
            case 4:
                button4 = new CswMobileMenuButton(o.buttons[button], $footerCtn);
                this[button] = button4;
                break;
        } 
    }
    
    //#endregion sheol
    
    //#region public, priveleged
    this.ID = o.ID;
    this.footer = $footer;
    
    //in case we don't know the button names, we can fetch and query
    this.button1 = button1;
    this.button2 = button2;
    this.button3 = button3;
    this.button4 = button4;
    //#endregion public, priveleged
    
}

//#endregion CswMobilePageFooter