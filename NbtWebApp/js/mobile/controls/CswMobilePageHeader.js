/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="CswMobileMenuButton.js" />
/// <reference path="ICswMobileWebControls.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//#region CswMobilePageHeader

CswMobilePageHeader.inheritsFrom(ICswMobileWebControls);

function CswMobilePageHeader(headerDef, $page) {
    /// <summary>
    ///   Header class. Responsible for generating a Mobile page header.
    /// </summary>
    /// <param name="headerDef" type="Object">JSON definition of header to display</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <returns type="CswMobilePageFooter">Instance of itself. Must instance with 'new' keyword.</returns>

    ICswMobileWebControls.call(this);

    var id, $header, buttonNames;
    
    //#region private
    //ctor
    (function() {
        var o = {
            buttons: { button1: { ID: '', text: '', 'data-icon': '', cssclass: '' } },
            ID: '',
            text: '',
            dataId: 'csw_header',
            dataTheme: 'b'
        };

        if (headerDef) $.extend(o, headerDef);

        buttonNames = [];
        id = o.ID + '_header';
        $header = $('#' + id);

        if (isNullOrEmpty($header) || $header.length === 0) {
            $header = $page.CswDiv('init', { ID: id });
            $header.CswAttrNonDom({
                'data-role': 'header',
                'data-position': 'fixed',
                'data-id': o.dataId,
                'data-theme': o.dataTheme
            });
            pageHeader(o.text);
            makeButtons(o.buttons);
        }
        
    })(); //ctor
    
    function pageHeader(text) {
        var ret;
        var $headerText = $header.find('#' + id + '_text');
        if( isNullOrEmpty($headerText) || $headerText.length === 0) {
            $headerText = $('<h1 style="white-space: normal;" id="' + id + '_text"></h1>')
                            .appendTo($header);
        }
        if(arguments.length === 1) {
            ret = $headerText.text(text);
        } else {
            ret = $headerText.text();
        }
        return ret;
    }
    
    //#endregion private
    
    //#region sheol
    
    function makeButtons(buttons) {
        var buttonCnt = 0;
        for (var buttonName in buttons) {
            buttonCnt++;

            var thisButton = buttons[buttonName];

            if (buttonCnt <= 2) {
                var button = new CswMobileMenuButton(thisButton, $header);
                buttonNames.push(buttonName);
                this[buttonName] = button;
            }
        }
    }
    //#endregion sheol
    
    //#region public, priveleged
    this.ID = id;
    this.$content = $header;
    this.buttonNames = buttonNames;
    this.pageHeader = pageHeader;
    this.addToHeader = function(text) {
        var $ret = $();
        if(text) {
            $ret = $('<p style="text-align: center; white-space: normal;">' + text + '</p>')
                .appendTo($header);
        }
        return $ret;
    };
    //#endregion public, priveleged
    
}

//#endregion CswMobilePageHeader