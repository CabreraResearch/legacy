/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    function doProp($Dom, name, value) {
        var ret = '';

        try {
            if (arguments.length === 2) {
                ret = $Dom.prop(name);
            } else {
                ret = $Dom.prop(name, value);
            }

            // special cases
            if (ret === undefined ||
                name === 'href' ||
                    name === 'cellpadding' ||
                        name === 'cellspacing' ||
                            name === 'rowspan' ||
                                name === 'colspan') {
                if (arguments.length === 2) {
                    ret = $Dom.attr(name);
                } else {
                    ret = $Dom.attr(name, value);
                }
            }
        } catch (e) {
            //We're in IE hell. Do nothing.
        }

        return ret;
    }

    function doAttr($this, name, value) {
        var ret = $this;

        try {
            switch (arguments.length) {
                case 2:
                    ret = $this.attr(name);
                    break;
                case 3:
                    ret = $this.attr(name, value);
                    break;
            }
        } catch (e) {
            //We're in IE hell. Do nothing.
        }
        // For proper chaining support
        return ret;
    }

    Csw.dom = Csw.dom ||
        Csw.register('dom', function (cswPublic, $element) {
            ///<summary>Extend an object with Csw DOM methods and properties</summary>
            ///<param name="options" type="Object">Object defining paramaters for dom construction.</param>
            ///<param name="element" type="Object">Object to extend</param>
            ///<returns type="Csw.dom">Object representing a Csw.dom</returns>
            'use strict';

            var cswPrivate = {
                data: {},
                enabled: true
            };
            cswPublic = cswPublic || {};

            if (cswPublic && cswPublic[0] && cswPublic.$) {
                cswPublic.isValid = true;
            }
            else if (Csw.isJQuery($element)) {
                cswPublic.$ = $element;
                cswPublic.isValid = true;
            }
            else if (false === Csw.isNullOrEmpty($element) && Csw.isJQuery($element.$)) {
                /*This is already a Csw dom object*/
                return $element;
            } else {
                cswPublic.$ = {};
            }

            cswPrivate.makeControlForChain = function ($child, method) {
                var ret,
                    _options = {
                        parent: function () { return cswPublic; },
                        first: function () { return cswPublic; },
                        root: cswPublic.root,
                        length: function () {
                            return 0;
                        },
                        isValid: false
                    };
                _options.children = function () {
                    return _options;
                };
                if (false === Csw.isNullOrEmpty($child, true)) {
                    if (Csw.isFunction(method)) {
                        ret = method($child, _options);
                    } else {
                        ret = cswPublic.jquery($child, _options);
                    }
                } else {
                    ret = _options;
                }
                return ret;
            };

            cswPrivate.prepControl = function (opts, controlName) {
                opts = opts || {};
                opts.controlName = controlName;
                opts.$ = cswPublic.$;
                opts.root = cswPublic.root;

                return opts;
            };

            cswPrivate.isControlStillValid = function () {
                var ret = false === Csw.isNullOrEmpty(cswPublic);
                if (false === ret) {
                    Csw.error.throwException('cswPublic is null. The control may have been garbage collected while event bindings were not.');
                }
                return ret;
            };

            cswPublic[0] = cswPublic.$[0];

            cswPublic.addClass = function (name) {
                /// <summary>Add a CSS class to an element.</summary>
                /// <param name="$element" type="jQuery">An element to add class to.</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Classy jQuery element (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.addClass(name);
                }
                return cswPublic;
            };

            cswPublic.append = function (object) {
                /// <summary>Append an object to this element.</summary>
                /// <param name="object" type="Object">Raw HTML, a jQuery object or text.</param>
                /// <returns type="Object">The parent Csw object (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    try {
                        cswPublic.$.append(object);
                    } catch (e) {
                        Csw.debug.log('Warning: append() failed, text() was used instead.', true);
                        if (Csw.isString(object)) {
                            cswPublic.$.text(object);
                        }
                    }
                }
                return cswPublic;
            };

            cswPublic.setLabelText = function (propName, isRequired, isReadOnly) {
                /// <summary>Append a property name to a dom element. Appends a '*' if it's a required property</summary>
                /// <param name="propName" type="Object">the property name to display</param>
                /// <param name="isRequired" type="Object">whether or not this property is required</param>
                /// <param name='isReadOnly" type="Object">whether or not this property is read only</param>
                /// <returns type="Object">The parent Csw object (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.append(propName).addClass('propertylabel').required(isRequired, false == isReadOnly);
                }
                return cswPublic;
            };

            cswPublic.attach = function (object) {
                /// <summary>Attach an object to this element.</summary>
                /// <param name="object" type="Object">Raw HTML. Warning: Do not pass a selector to this method!</param>
                /// <returns type="Object">The new Csw object (for chaining)</returns> 
                var $child = null, ret;
                if (cswPrivate.isControlStillValid()) {
                    try {
                        $child = $(object);
                        if (false === Csw.isNullOrEmpty($child)) {
                            cswPublic.$.append($child);
                        }
                    } catch (e) {
                        /* One day we'll implement client-side error handling */
                    }
                    ret = cswPrivate.makeControlForChain($child);
                }
                return ret;
            };

            cswPublic.bind = function (eventName, event) {
                /// <summary>Bind an action to a jQuery element's event.</summary>
                /// <param name="$element" type="jQuery">A jQuery element</param>
                /// <param name="eventName" type="String">The name of the event</param>
                /// <param name="event" type="Function">A function to execute when the event fires</param>
                /// <returns type="Object">The jQuery element (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.bind(eventName, event);
                }
                return cswPublic;
            };

            cswPublic.children = function (searchTerm, selector) {
                /// <summary>Find the child elements of this DOM element represented by this object</summary>
                /// <param name="searchTerm" type="String">(Optional) Some search term to limit child results</param>
                /// <param name="selector" type="String">(Optional) A selector</param>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var ret = null;
                if (cswPrivate.isControlStillValid()) {
                    var _$element = cswPublic.$.children(Csw.string(searchTerm), Csw.string(selector));
                    ret = cswPrivate.makeControlForChain(_$element);
                }
                return ret;
            };

            cswPublic.clickOnEnter = function (cswControl) {
                /// <summary>Bind an event to the enter key, when pressed in this control.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.clickOnEnter(cswControl.$);
                }
                return cswPublic;
            };

            cswPublic.css = function (param1, param2) {
                /// <param name="param1" type="Object">Either a single JSON object with CSS to apply, or a single CSS name</param>
                /// <param name="param2" type="string">single CSS value</param>
                if (cswPrivate.isControlStillValid()) {
                    if (arguments.length === 1) {
                        cswPublic.$.css(param1);
                    } else {
                        cswPublic.$.css(param1, param2);
                    }
                }
                return cswPublic;
            };

            //#region data methods

            cswPrivate.getData = function (propName) {
                /// <summary>
                /// Get the value of a data prop from an Element (first from the DOM, then from memory, then from clientDb)
                /// </summary>
                var ret = null;
                if (cswPrivate.isControlStillValid() &&
                    false === Csw.isNullOrEmpty(propName)) {

                    if (cswPublic[0] && cswPublic[0].dataset && cswPublic[0].dataset[propName]) {
                        ret = cswPublic[0].dataset.propName;
                    }
                    if (Csw.isNullOrEmpty(ret)) {
                        ret = cswPrivate.data[propName] || cswPublic.$.data(propName);
                    }
                    //We can probably come back and delete this
                    if (Csw.isNullOrEmpty(ret)) {
                        ret = Csw.clientDb.getItem(propName + '_control_data_' + cswPrivate.id);
                    }
                }
                return ret;
            };

            cswPrivate.setData = function (propName, value) {
                /// <summary>
                /// Set the value of a data prop from an Element (first to the DOM, then to memory, then to clientDb)
                /// </summary>
                var ret = null;
                if (cswPrivate.isControlStillValid() &&
                    false === Csw.isNullOrEmpty(propName)) {

                    ret = value;
                    if (cswPublic[0] && cswPublic[0].dataset) {
                        cswPublic[0].dataset[propName] = value;
                    }
                    cswPrivate.data[propName] = value;  // these are important when the value is actually empty
                    cswPublic.$.data(propName, value);  // these are important when the value is actually empty
                }
                return ret;
            };

            cswPrivate.setDataObj = function (obj) {
                Csw.each(obj, function (val, propName) {
                    cswPrivate.setData(propName, val);
                });
            };

            //#endregion data methods

            cswPublic.data = function (prop, val) {
                /// <summary>Store property data on the control.</summary>
                /// <returns type="Object">All properties, a single property, or the control if defining a property (for chaining).</returns> 
                var ret = '';
                if (cswPrivate.isControlStillValid()) {
                    if (Csw.isPlainObject(prop)) {
                        cswPrivate.setDataObj(prop);
                    } else {
                        switch (arguments.length) {
                            //this isn't a valid use case           
                            //case 0:           
                            //    ret = _internal || cswPublic.$.data();           
                            //    break;           
                            case 1:
                                ret = cswPrivate.getData(prop);
                                break;
                            case 2:
                                cswPrivate.setData(prop, val);
                                ret = cswPublic;
                                break;
                        }
                    }
                }
                return ret;

            };

            cswPublic.disable = function () {
                /// <summary>Disable the element.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPrivate.enabled = false;
                    cswPublic.propNonDom('disabled', 'disabled');
                }
                return cswPublic;
            };

            cswPublic.empty = function () {
                /// <summary>Empty the element.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.empty();
                }
                return cswPublic;
            };

            cswPublic.enable = function () {
                /// <summary>Enable the element.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPrivate.enabled = true;
                    cswPublic.removeAttr('disabled');
                }
                return cswPublic;
            };

            cswPublic.filter = function (selector) {
                /// <summary>Filter the child elements of this DOM element according to this selector</summary>
                /// <param name="selector" type="String">A filter string.</param>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var ret = null;
                if (cswPrivate.isControlStillValid()) {
                    var _$element = cswPublic.$.filter(selector);
                    ret = cswPrivate.makeControlForChain(_$element);
                }
                return ret;
            };

            cswPublic.find = function (selector) {
                /// <summary>Find the child elements of this DOM element represented by this object</summary>
                /// <param name="selector" type="String">A selector, id or jQuery object to find.</param>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var ret = null;
                if (cswPrivate.isControlStillValid()) {
                    var _$element = cswPublic.$.find(Csw.string(selector));
                    ret = cswPrivate.makeControlForChain(_$element);
                }
                return ret;
            };

            cswPublic.first = function () {
                /// <summary>Find the first child element of this DOM element represented by this object</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var ret = null;
                if (cswPrivate.isControlStillValid()) {
                    var _$element = cswPublic.$.first();
                    ret = cswPrivate.makeControlForChain(_$element);
                }
                return ret;
            };

            cswPublic.getId = function () {
                /// <summary>Get the DOM Element ID of this object.</summary>
                /// <returns type="String">Element ID.</returns> 
                var ret = null;
                if (cswPrivate.isControlStillValid()) {
                    ret = cswPublic[0].id;
                }
                return ret;
            };

            cswPublic.hide = function () {
                /// <summary>Make the element invisible.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.hide();
                }
                return cswPublic;
            };

            cswPublic.jquery = function ($jqElement, opts) {
                /// <summary> Extend a jQuery object with Csw methods.</summary>
                /// <param name="$element" type="jQuery">Element to extend.</param>
                /// <returns type="jquery">A Csw.jquery object</returns>
                opts = cswPrivate.controlPreProcessing(opts, 'jquery');
                return Csw.literals.factory($jqElement, opts);
            };

            cswPublic.length = function () {
                /// <summary>Get the length of this element.</summary>
                /// <returns type="Number">Number of elements at the current level of the tree.</returns> 
                var ret = 0;
                if (cswPrivate.isControlStillValid()) {
                    ret = Csw.number(cswPublic.$.length);
                }
                return ret;
            };

            cswPublic.parent = cswPublic.parent || function () {
                /// <summary>Get the parent of this control</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var ret = {};
                if (cswPrivate.isControlStillValid()) {
                    var _$element = cswPublic.$.parent();

                    if (false === Csw.isNullOrEmpty(_$element, true)) {
                        ret = cswPublic.jquery(_$element);
                    }
                }
                return ret;
            };

            cswPublic.propDom = function (name, value) {
                /// <summary>
                ///   Gets or sets a DOM property
                /// </summary>
                /// <param name="name" type="String">The name of the attribute</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
                var ret = null;
                if (cswPrivate.isControlStillValid()) {
                    ret = cswPublic;
                    var prop;

                    //TODO: this try..catch.. shouldn't be here.
                    try {
                        if (typeof name === "object") {
                            for (prop in name) {
                                doProp(cswPublic.$, prop, name[prop]);
                            }
                        } else {
                            ret = doProp(cswPublic.$, name, value);
                        }
                    } catch (e) {
                        //We're in IE hell. Do nothing.
                    }
                    if (arguments.length === 2 || Csw.isPlainObject(name)) {
                        ret = cswPublic;
                    }
                }
                return ret;
            };

            cswPublic.propNonDom = function (name, value) {
                /// <summary>
                ///   Gets or sets an Non-Dom attribute
                /// </summary>
                /// <param name="name" type="String">The name of the attribute</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
                var ret = null;
                if (cswPrivate.isControlStillValid()) {

                    ret = cswPublic;
                    var prop;
                    //TODO: this try..catch.. shouldn't be here.
                    try {
                        if (typeof name === "object") {
                            for (prop in name) {
                                doAttr(cswPublic.$, prop, name[prop]);
                            }
                        } else {
                            ret = doAttr(cswPublic.$, name, value);
                        }
                        // For proper chaining support
                    } catch (e) {
                        //We're in IE hell. Do nothing.
                    }
                    if (arguments.length === 2 || Csw.isPlainObject(name)) {
                        ret = cswPublic;
                    }
                }
                return ret;
            };

            cswPublic.remove = function () {
                /// <summary>Remove the element and delete the object.</summary>
                /// <returns type="null"></returns> 
                //if (cswPrivate.isControlStillValid()) {
                //if the control isn't valid, we don't need to throw on removing it.
                if(cswPublic && cswPublic.$) {
                    cswPublic.$.remove();
                    //Nice try, but this doesn't nuke outstanding references--only the assignment of the reference to the property on this object.
                    //Csw.each(cswPublic, function (name) {
                    //cswPublic[name] = null;
                    //delete cswPublic[name];
                    //});

                    //This is a value assignment, it doesn't affect the reference that brought you here
                    //However, it's useful inside the closure that defines cswPublic to know we removed this element from the DOM.
                    cswPublic = null;
                }
                return null;
            };

            cswPublic.removeClass = function (name) {
                /// <summary>Remove a CSS class to an element.</summary>
                /// <param name="$element" type="jQuery">An element to remove class from.</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Classless jQuery element (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.removeClass(name);
                }
                return cswPublic;
            };

            cswPublic.removeProp = function (name) {
                /// <summary>Remove a property from an element.</summary>
                /// <param name="name" type="String">The name of the attribute</param>
                /// <returns type="Object">CswDomObject (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.removeProp(name);
                }
                return cswPublic;
            };

            cswPublic.removeAttr = function (name) {
                /// <summary>Remove an attribute from an element.</summary>
                /// <param name="name" type="String">The name of the attribute</param>
                /// <returns type="Object">CswDomObject (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.removeAttr(name);
                }
                return cswPublic;
            };

            cswPublic.required = function (truthy, addLabel) {
                /// <summary>Mark the required status of the element.</summary>
                /// <returns type="Object">CswDomObject (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    switch (Csw.bool(truthy)) {
                        case true:
                            if (addLabel) {
                                cswPrivate.requiredspan = cswPublic.span({ text: "*" }).css('color', 'Red');
                            }
                            cswPublic.propDom('required', true);
                            cswPublic.addClass('required');
                            break;
                        case false:
                            if (cswPrivate.requiredspan) {
                                cswPrivate.requiredspan.remove();
                            }
                            cswPublic.removeProp('required');
                            cswPublic.removeClass('required');
                            break;
                    }
                }
                return cswPublic;
            };

            cswPublic.root = cswPublic.root || function () {
                /// <summary>Get the root (great, great, great grandparent) of this control</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var ret = null;
                if (cswPrivate.isControlStillValid()) {
                    var _$element = cswPublic.$.parent();
                    while (false === Csw.isNullOrEmpty(_$element.parent(), true)) {
                        _$element = _$element.parent();
                    }
                    if (false === Csw.isNullOrEmpty(_$element, true)) {
                        ret = cswPublic.jquery(_$element);
                    } else {
                        ret = {};
                    }
                }
                return ret;
            };

            cswPublic.show = function () {
                /// <summary>Make the element visible.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.show();
                }
                return cswPublic;
            };

            cswPublic.text = function (text) {
                /// <summary>Get the value of the element.</summary>
                /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
                if (cswPrivate.isControlStillValid()) {
                    if (arguments.length === 1 && false === Csw.isNullOrUndefined(text)) {
                        cswPublic.$.text(text);
                        return cswPublic;
                    } else {
                        return Csw.string(cswPublic.$.text());
                    }
                }
            };

            cswPublic.toggle = function () {
                /// <summary>Toggle the element's visibility.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.toggle();
                }
                return cswPublic;
            };

            cswPublic.toggleEnable = function () {
                /// <summary>Toggle the element's enabled state.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    if (cswPrivate.enabled) {
                        cswPublic.disable();
                    } else {
                        cswPublic.enable();
                    }
                }
                return cswPublic;
            };

            cswPublic.trigger = function (eventName, eventOpts) {
                /// <summary>Trigger an event bound to a jQuery element.</summary>
                /// <param name="$element" type="jQuery">A jQuery element</param>
                /// <param name="eventName" type="String">The name of the event</param>
                /// <param name="eventOpts" type="Object">Options collection to pass to the event handler.</param>
                /// <returns type="Object">The jQuery element (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.trigger(eventName, eventOpts);
                }
                return cswPublic;
            };

            cswPublic.unbind = function (eventName, event) {
                /// <summary>Unbind an action from a jQuery element's event.</summary>
                /// <param name="$element" type="jQuery">A jQuery element</param>
                /// <param name="eventName" type="String">The name of the event</param>
                /// <returns type="Object">The jQuery element (for chaining)</returns> 
                if (cswPrivate.isControlStillValid()) {
                    cswPublic.$.unbind(eventName, event);
                }
                return cswPublic;
            };

            cswPublic.val = cswPublic.val || function (value) {
                /// <summary>Get the value of the element.</summary>
                /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
                if (cswPrivate.isControlStillValid()) {
                    if (arguments.length === 1 && false === Csw.isNullOrUndefined(value)) {
                        cswPublic.$.val(value);
                        return cswPublic;
                    } else {
                        return Csw.string(cswPublic.$.val());
                    }
                }
            };

            cswPublic.valueOf = function () {
                return cswPublic;
            };

            return cswPublic;
        });

    Csw.makeAttr = Csw.makeAttr ||
        Csw.register('makeAttr', function () {
            /// <summary> Build an HTML element attribute string </summary>
            /// <returns type="String">A string of attribute key/value pairs</returns>
            var cswPrivate = {
                attributes: {}
            };
            var cswPublic = {};

            cswPublic.add = function (key, value) {
                if (false === Csw.isNullOrEmpty(key)) {
                    cswPrivate.attributes[Csw.string(key)] = Csw.string(value);
                }
            };

            cswPublic.get = function () {
                var ret = '';

                function buildStyle(val, key) {
                    if (false === Csw.isNullOrEmpty(val) &&
                        false === Csw.isNullOrEmpty(key)) {
                        ret += ' ' + key + '="' + val + '" ';
                    }
                }

                Csw.each(cswPrivate.attributes, buildStyle);

                return ret;
            };

            return cswPublic;
        });

    Csw.makeStyle = Csw.makeStyle ||
        Csw.register('makeStyle', function () {
            /// <summary> Build an HTML element style string </summary>
            /// <returns type="String">A string of style key/value pairs</returns>
            var cswPrivate = {
                styles: {}
            };
            var cswPublic = {};

            cswPublic.add = function (key, value) {
                cswPrivate.styles[key] = value;
            };

            cswPublic.set = function (stylesObj) {
                cswPrivate.styles = stylesObj;
            };

            cswPublic.get = function () {
                var htmlStyle = '', ret = '';

                function buildStyle(val, key) {
                    if (false === Csw.isNullOrEmpty(key) &&
                        false === Csw.isNullOrEmpty(val)) {
                        htmlStyle += key + ': ' + val + ';';
                    }
                }

                Csw.each(cswPrivate.styles, buildStyle);

                if (htmlStyle.length > 0) {
                    ret = ' style="' + htmlStyle + '"';
                }
                return ret;
            };

            return cswPublic;
        });

    Csw.isElementInDom = Csw.isElementInDom ||
        Csw.register('isElementInDom', function (elementId) {
            return false === Csw.isNullOrEmpty(document.getElementById(elementId));
        });

    Csw.tryParseElement = Csw.tryParseElement ||
        Csw.register('tryParseElement', function (elementId, $context) {
            /// <summary>Attempts to fetch an element from the DOM first through jQuery, then through JavaScript.</summary>
            /// <param name="elementId" type="String"> ElementId to find </param>
            /// <param name="$context" type="jQuery"> Optional context to limit the search </param>
            /// <returns type="jQuery">jQuery object, empty if no match found.</returns>
            var $ret = $('');
            var document = Csw.getGlobalProp('document');
            if (false === Csw.isNullOrEmpty(elementId)) {
                try {
                    if (arguments.length === 2 && false === Csw.isNullOrEmpty($context)) {
                        $ret = $('#' + elementId, $context);
                    } else {
                        $ret = $('#' + elementId);
                    }
                } catch (e) {
                    Csw.debug.error('Could not fetch element by ID using jQuery.');
                    Csw.debug.error(e);
                }

                if ($ret.length === 0) {
                    $ret = $(document.getElementById(elementId));
                }
                if ($ret.length === 0) {
                    $ret = $(document.getElementsByName(elementId));
                }
            }
            return $ret;
        });

    Csw.getIconUrlString = Csw.getIconUrlString || Csw.register('getIconUrlString', function (iconSize, iconType) {
        ///<summary>Given an icon enum, builds the url to the location of the icon image.</summary>
        ///<param name="">Size of the icon to find.</param>
        ///<param name="">Enum of the icon to find.</param>
        ///<returns type="">String URL of the icon image, empty string if no match found.</returns>
        var ret = '';

        var iconName = Csw.enums.getName(Csw.enums.iconType, iconType);

        if (false === Csw.isNullOrEmpty(iconSize) && false === Csw.isNullOrEmpty(iconName)) {

            ret = 'Images/newicons/' + iconSize + '/' + iconName + '.png';
        }

        return ret;

    });

} ());


