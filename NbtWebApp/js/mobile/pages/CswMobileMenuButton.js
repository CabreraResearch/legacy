/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../_Global.js" />
/// <reference path="../../_CswPrototypeExtensions.js" />
/// <reference path="../../jquery/common/CswAttr.js" />

//#region CswMobileMenuButton

function CswMobileMenuButton(buttonDef, $parent) {
    /// <summary>
    ///   Menu button class. Responsible for creating Mobile menu buttons suitable for consumption by a header/footer.
    ///   Menu buttons must be tied to static pages to wire their events properly, with the exception of 'Back'.
    /// </summary>
    /// <param name="buttonDef" type="Object">Button definitional data.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <returns type="CswMobileMenuButton">Instance of itself. Must instance with 'new' keyword.</returns>
    
    //#region private 

    var o = {
        ID: '',
        text: '',
        enabled: true,
        visible: true,
        cssStyles: {'display': '' },
        cssClass: '',
        dataTransition: 'pop',
        dataRelationship: 'page',
        dataIcon: '',
        href: 'javascript:void(0)',
        rel: '',
        onClick: function() {}
    };
    
    if(buttonDef) $.extend(o, buttonDef);

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
    
    var $li = $parent.find('#' + buttonId + '_li');
    var $button;
    if( isNullOrEmpty($li) || $li.length === 0 ) {
        $li = $('<li id="' + buttonId + '_li"></li>');
        if (!isNullOrEmpty($parent)) {
            $parent.append($li);
        }
        $button = $li.CswLink('init', buttonProp);
    } else {
        $button = $li.find('#' + buttonId).CswAttrDom(buttonProp);
    }
    
    $button.CswAttrXml(buttonAttr)
           .css(o.cssStyles);
    //#endregion private 
    
    //#region public, priveleged
    
    this.buttonName = o.ID + '_menubutton';
    this.$button = $li;
    
    this.isEnabled = o.enabled;
    this.enable = function(enableButton) {
        /// <summary>Enables or disables the button.</summary>
        /// <param name="enableButton" type="Boolean">True to enable the button.</param>
        /// <returns type="jQuery">Returns the button.</returns>
        o.enabled = isTrue(enableButton);
        if( o.enabled ) {
            $li.button('enable');
        } else {
            $li.button('disable');
        }
        return $li;
    };
    this.isVisible = o.visible;
    this.visible = function(keepVisible) {
        /// <summary>Shows or hides the button.</summary>
        /// <param name="keepVisible" type="Boolean">True to show the button.</param>
        /// <returns type="jQuery">Returns the button.</returns>
        o.visible = isTrue(keepVisible);
        if( o.visible ) {
            $li.css('display', '').show();
        } else {
            $li.css('display', 'none').hide();    
        }
        return $li;
    };
    this.addCssStyle = function(style,value) {
        /// <summary>Add a CSS style attribute to the button</summary>
        /// <param name="style" type="Object or String">JSON object {style: value} or style name as string.</param>
        /// <param name="value" type="String">Style value.</param>
        /// <returns type="jQuery">Returns the button.</returns>
        if( arguments.length == 2 && typeof style === 'string') {
            o.cssStyles[style] = value;
        } else if (style) {
            $.extend(o.cssStyles, style);
        }
        $li.css(o.cssStyles);
        return $li;
    };
    this.removeCssStyle = function(style) {
        /// <summary>Removes a style attribute from the button.</summary>
        /// <param name="style" type="String">Style to remove.</param>
        /// <returns type="jQuery">Returns the button.</returns>
        if(style) {
            delete o.cssStyles[style];
            $li.removeAttr(style);
        }
        return $li;
    };
    this.addCssClass = function(cssclass) {
        /// <summary>Add a CSS class to the button</summary>
        /// <param name="cssclass" type="String">Class name</param>
        /// <returns type="jQuery">Returns the button.</returns>
        if (cssclass && classes.indexOf(cssclass) === -1 ) {
            classes.push(cssclass);
            o.cssClass += cssclass;
        }
        $li.addClass(cssclass);
        return $li;
    };
    this.removeCssClass = function(cssclass) {
        /// <summary>Remove a CSS class from the button</summary>
        /// <param name="cssclass" type="String">Class name</param>
        /// <returns type="jQuery">Returns the button.</returns>
        if (cssclass && classes.indexOf(cssclass) === -1 ) {
            classes.pop(cssclass);
            o.cssClass.replace(cssclass, '');
        }
        $li.removeClass(cssclass);
        return $li;
    };
    this.setDataTransition = function(transition) {
        /// <summary>Defines page transition to use on button click.</summary>
        /// <param name="keepVisible" type="Boolean">True to show the button.</param>
        /// <returns type="jQuery">Returns the button.</returns>
        o.dataTransition = tryParseString(transition, 'pop');
        $li.CswAttrXml('data-transition', o.dataTransition);
        return $li;
    };
    this.setDataRelationship = function(relationship) {
        /// <summary>Shows or hides the button.</summary>
        /// <param name="keepVisible" type="Boolean">True to show the button.</param>
        /// <returns type="jQuery">Returns the button.</returns>
        o.dataRelationship = tryParseString(relationship, 'page');
        $li.CswAttrXml('data-rel', o.dataRelationship);
        return $li;
    };
    this.setDataIcon = function(icon) {
        /// <summary>Shows or hides the button.</summary>
        /// <param name="keepVisible" type="Boolean">True to show the button.</param>
        /// <returns type="jQuery">Returns the button.</returns>
        o.dataIcon = tryParseString(icon, '');
        $li.CswAttrXml('data-icon', o.dataIcon);
        return $li;
    };
    
    //#endregion public, priveleged
}

//#endregion CswMobileMenuButton