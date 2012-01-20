/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />

//#region ICswMobileWebControls

function ICswMobileWebControls(controlDef, $parent) {
    "use strict";
    /// <summary>
    ///   Faux interface class for Csw Client Web Controls.
    /// </summary>
    /// <param name="controlDef" type="Object">Control definitional data.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <returns type="ICswMobileWebControls">Instance of itself. Must instance with 'new' keyword.</returns>
    
    //#region private 

    var _$control = $();
    var _classes = [];
    var _styles = [];
    var _cssClass = '';
    var _controlName = '';
    var _enabled = true;
    var _visible = true;
    var _eventHandlers = { };
    _eventHandlers[CswDomElementEvent.click] = { methods: [] };
    
    //#endregion private 
    
    //#region public, priveleged

    this.controlName = _controlName;
    this.$control = _$control;

    this.styles = _styles;
    this.classes = _classes;
    this.cssClass = _cssClass;
    
    this.isEnabled = _enabled;
    this.enable = function(enable) {
        /// <summary>Enables or disables the control.</summary>
        /// <param name="enable" type="Boolean">True to enable the control.</param>
        /// <returns type="jQuery">Returns the control.</returns>
        _enabled = isTrue(enable);
        _$control = this.$control;
        
        this.isEnabled = _enabled;
        if( _enabled ) {
            _$control.removeAttr('disabled');
        } else {
            _$control.CswAttrNonDom('disabled','disabled');
        }
        return _$control;
    };
    this.isVisible = _visible;
    this.visible = function(visible) {
        /// <summary>Shows or hides the control.</summary>
        /// <param name="keepVisible" type="Boolean">True to show the control.</param>
        /// <returns type="jQuery">Returns the control.</returns>
        _visible = isTrue(visible);
        _$control = this.$control;
        
        this.isVisible = _visible;
        if( _visible ) {
            _$control.css('display', '').show();
        } else {
            _$control.css('display', 'none').hide();    
        }
        return _$control;
    };
    this.addCssStyle = function(style,value) {
        /// <summary>Add a CSS style attribute to the control</summary>
        /// <param name="style" type="Object or String">JSON object {style: value} or style name as string.</param>
        /// <param name="value" type="String">Style value.</param>
        /// <returns type="jQuery">Returns the control.</returns>
        _$control = this.$control;
        
        if( arguments.length == 2 && typeof style === 'string') {
            _styles[style] = value;
        } else if (style) {
            $.extend(_styles, style);
        }
        _$control.css(_styles);
        return _$control;
    };
    this.removeCssStyle = function(style) {
        /// <summary>Removes a style attribute from the control.</summary>
        /// <param name="style" type="String">Style to remove.</param>
        /// <returns type="jQuery">Returns the control.</returns>
        _$control = this.$control;
        
        if(style) {
            delete _styles[style];
            _$control.removeAttr(style);
        }
        return _$control;
    };
    this.addCssClass = function(cssclass) {
        /// <summary>Add a CSS class to the control</summary>
        /// <param name="cssclass" type="String">Class name</param>
        /// <returns type="jQuery">Returns the control.</returns>
        _$control = this.$control;
        
        if (cssclass && this.classes.indexOf(cssclass) === -1 ) {
            _classes.push(cssclass);
            _cssClass += cssclass;
            _$control.addClass(cssclass);
        }
        return _$control;
    };
    this.removeCssClass = function(cssclass) {
        /// <summary>Remove a CSS class from the control</summary>
        /// <param name="cssclass" type="String">Class name</param>
        /// <returns type="jQuery">Returns the control.</returns>
        _$control = this.$control;
        
        if (cssclass && _classes.indexOf(cssclass) !== -1 ) {
            _classes.pop(cssclass);
            _cssClass.replace(cssclass, '');
            _$control.removeClass(cssclass);
        }
        return _$control;
    };

    this.bindEvents = function(event) {
        /// <summary>Binds defined events to the control</summary>
        /// <param name="event" type="CswDomElementEvent">CswDomElementEvent Event enum</param>
        /// <returns type="void"></returns>
        _$control = this.$control;
        
        var thisEvent = _eventHandlers[event.name];
        if( !isNullOrEmpty(thisEvent) ) {
            var methods = thisEvent.methods;
            for (var method in methods) {
                _$control.bind(event.name, methods[method]);
            }
        }
    };
    
    this.unbindEvents = function(event) {
        /// <summary>Unbinds all defined events from the control</summary>
        /// <param name="event" type="CswDomElementEvent">CswDomElementEvent Event enum</param>
        /// <returns type="void"></returns>
        _$control = this.$control;
        
        _$control.unbind(event.name);
    };
    
    this.setEvent = function(event, method) {
        /// <summary>Defines events available for binding to the control</summary>
        /// <param name="event" type="CswDomElementEvent">CswDomElementEvent event enum</param>
        /// <param name="method" type="Function">Event method</param>
        /// <returns type="void"></returns>
        _$control = this.$control;
        
        var thisEvent = _eventHandlers[event.name];
        if( isNullOrEmpty(_eventHandlers[event.name] )) {
            _eventHandlers[event.name] = { methods: [method] };
        } else {
            var methods = thisEvent.methods;
            methods.push(method);
        }
    };
    
    //#endregion public, priveleged
}

//#endregion ICswMobileWebControls