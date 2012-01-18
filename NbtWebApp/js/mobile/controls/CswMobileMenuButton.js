/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="ICswMobileWebControls.js" />

//#region CswMobileMenuButton

CswMobileMenuButton.inheritsFrom(ICswMobileWebControls);

function CswMobileMenuButton(buttonDef, $parent) {
    "use strict";
    /// <summary>
    ///   Menu button class. Responsible for creating Mobile menu buttons suitable for consumption by a header/footer.
    ///   Menu buttons must be tied to static pages to wire their events properly, with the exception of 'Back'.
    /// </summary>
    /// <param name="buttonDef" type="Object">Button definitional data.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <returns type="CswMobileMenuButton">Instance of itself. Must instance with 'new' keyword.</returns>
    
    //#region private 

    ICswMobileWebControls.call(this);
    
    var _o, _$control, _classes;
    
    function Constructor() {
        var o = {
            ID: '',
            text: '',
            enabled: true,
            visible: true,
            cssStyles: { 'display': '' },
            cssClass: '',
            dataTransition: 'pop',
            dataRelationship: 'page',
            dataIcon: '',
            href: 'javascript:void(0)',
            rel: '',
            onClick: '' // function() {}
        };

        if (buttonDef) $.extend(o, buttonDef);

        var classes = o.cssClass.split(' ');

        var buttonId = o.ID + '_menubutton';

        var buttonAttr = {
            'data-identity': o.ID,
            'data-url': o.ID,
            'data-transition': o.dataTransition,
            'data-rel': o.dataRelationship,
            'data-icon': o.dataIcon,
            'data-theme': o.dataTheme
        };
        var buttonProp = {
            href: o.href,
            rel: o.rel,
            ID: buttonId,
            value: o.text,
            cssclass: o.cssClass
        };
        
        var $button = $parent.find('#' + buttonId);

        if (isNullOrEmpty($button) || $button.length === 0) {
            if (!isNullOrEmpty($parent)) {
                $button = $parent.CswLink('init', buttonProp);
            }

        } else {
            $button.CswAttrDom(buttonProp);
        }

        $button.CswAttrNonDom(buttonAttr)
            .css(o.cssStyles);

        _o = o;
        _classes = classes;
        _$control = $button;
    }

    Constructor();
    //#endregion private 
    
    //#region public, priveleged

    this.$control = _$control;
    this.classes = _classes;
   
    if( !isNullOrEmpty(_o.onClick) ) {
        this.unbindEvents(CswDomElementEvent.click);
        this.setEvent(CswDomElementEvent.click, _o.onClick);
        this.bindEvents(CswDomElementEvent.click);
    }    
    
    //#region ICswMobileWebControls Overrides
    this.enable = function(enable) {
        /// <summary>Enables or disables the control.</summary>
        /// <param name="enable" type="Boolean">True to enable the control.</param>
        /// <returns type="jQuery">Returns the control.</returns>
        _enabled = isTrue(enable);
        this.isEnabled = _enabled;
        if( _enabled ) {
            _$control.button('enable');
        } else {
            _$control.button('disable');
        }
        return _$control;
    };
    
    //#endregion ICswMobileWebControls Overrides
    
    this.setDataTransition = function(transition) {
        /// <summary>Defines page transition to use on button click.</summary>
        /// <param name="keepVisible" type="Boolean">True to show the button.</param>
        /// <returns type="jQuery">Returns the button.</returns>
        _o.dataTransition = tryParseString(transition, 'pop');
        _$control.CswAttrNonDom('data-transition', _o.dataTransition);
        return _$control;
    };
    this.setDataRelationship = function(relationship) {
        /// <summary>Shows or hides the button.</summary>
        /// <param name="keepVisible" type="Boolean">True to show the button.</param>
        /// <returns type="jQuery">Returns the button.</returns>
        _o.dataRelationship = tryParseString(relationship, 'page');
        _$control.CswAttrNonDom('data-rel', _o.dataRelationship);
        return _$control;
    };
    this.setDataIcon = function(icon) {
        /// <summary>Shows or hides the button.</summary>
        /// <param name="keepVisible" type="Boolean">True to show the button.</param>
        /// <returns type="jQuery">Returns the button.</returns>
        _o.dataIcon = tryParseString(icon, '');
        _$control.CswAttrNonDom('data-icon', _o.dataIcon);
        return _$control;
    };
    
    //#endregion public, priveleged
}

//#endregion CswMobileMenuButton