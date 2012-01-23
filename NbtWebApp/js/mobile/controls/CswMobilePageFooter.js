/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="CswMobileMenuButton.js" />
/// <reference path="ICswMobileWebControls.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswPrototypeExtensions.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />

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

    var id, $footer, buttonNames;
    
    //ctor
    (function() {
        var o = {
            buttons: { button1: { ID: '', text: '', 'data-icon': '', cssclass: '' } },
            ID: '',
            dataId: 'csw_footer',
            dataTheme: 'b'
        };

        if (footerDef) $.extend(o, footerDef);

        var $footerCtn;
        buttonNames = [];
        id = o.ID + '_footer';

        $footer = $('#' + id);
        if (isNullOrEmpty($footer) || $footer.length === 0)
        {
            $footer = $parent.CswDiv('init', { ID: id })
                .CswAttrNonDom({
                        'data-role': 'footer',
                        'data-position': 'fixed',
                        'data-id': o.dataId,
                        'data-theme': o.dataTheme
                    });
            var $footerNav = $('<div data-role="navbar">').appendTo($footer);
            $footerCtn = $('<ul class="csw_fieldctn"></ul>').appendTo($footerNav);
            makeButtons($footerCtn, o.buttons);
            //$footer.trigger('create');
        } else {
            $footerCtn = $footer.find('.csw_fieldctn');
        }
    })();
    //#endregion private
    
    //#region sheol
    
    //let's make these buttons accessible by name
    function makeButtons($footerCtn, buttons) {
        var buttonCnt = 0;
        for (var buttonName in buttons) {
            if(buttons.hasOwnProperty(buttonName)) {
                buttonCnt++;

                var thisButton = buttons[buttonName];
                var $buttonWrp = $footerCtn.find('#' + thisButton.ID + '_li');
                if (isNullOrEmpty($buttonWrp) || $buttonWrp.length === 0) {
                    $buttonWrp = $('<li id="' + thisButton.ID + '_li"></li>');
                }

                if (buttonCnt <= 4) {
                    var button = new CswMobileMenuButton(thisButton, $buttonWrp.appendTo($footerCtn));
                    buttonNames.push(buttonName);
                    this[buttonName] = button;
                }
            }
        }
    }
    //#endregion sheol
    
    //#region public, priveleged
    
    this.ID = id;
    this.$content = $footer;
    this.buttonNames = buttonNames;
    
    //#endregion public, priveleged
    
}

//#endregion CswMobilePageFooter