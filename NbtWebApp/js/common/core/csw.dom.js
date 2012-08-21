/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
        Csw.register('dom', function (options, element) {
            ///<summary>Extend an object with Csw DOM methods and properties</summary>
            ///<param name="options" type="Object">Object defining paramaters for dom construction.</param>
            ///<param name="element" type="Object">Object to extend</param>
            ///<returns type="Csw.dom">Object representing a Csw.dom</returns>
            'use strict';

            var cswPrivate = {};
            var cswPublic = options || {
                parentId: ''
            };

            if (Csw.isJQuery(element)) {
                cswPublic.$ = element;
                cswPrivate.id = Csw.string(element.prop('id'));
                cswPublic.isValid = true;
            } else if (false === Csw.isNullOrEmpty(element) && Csw.isJQuery(element.$)) {
                /*This is already a Csw dom object*/
                return element;
            } else {
                cswPrivate.id = '';
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
                var id = opts.id || controlName;
                opts = opts || {};
                cswPrivate.id = cswPrivate.id || Csw.makeId(cswPublic.parentId, 'sub', id, '_', false);
                //opts.ID = opts.ID || cswPrivate.makeId(cswPublic.parentId, 'sub', id, '_', false);
                opts.$ = cswPublic.$;
                opts.root = cswPublic.root;

                return opts;
            };

            cswPublic.addClass = function (name) {
                /// <summary>Add a CSS class to an element.</summary>
                /// <param name="$element" type="jQuery">An element to add class to.</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Classy jQuery element (for chaining)</returns> 
                cswPublic.$.addClass(name);
                return cswPublic;
            };

            cswPublic.append = function (object) {
                /// <summary>Append an object to this element.</summary>
                /// <param name="object" type="Object">Raw HTML, a jQuery object or text.</param>
                /// <returns type="Object">The parent Csw object (for chaining)</returns> 
                try {
                    cswPublic.$.append(object);
                } catch (e) {
                    Csw.debug.log('Warning: append() failed, text() was used instead.', true);
                    if (Csw.isString(object)) {
                        cswPublic.$.text(object);
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
                if (Csw.bool(isRequired) && !Csw.bool(isReadOnly)) {
                    propName = Csw.makeRequiredName(propName);
                }
                cswPublic.append(propName);
                return cswPublic;
            };

            Csw.makeRequiredName = Csw.makeRequiredName ||
                Csw.register('makeRequiredName', function (propName) {
                    /// <summary>Returns the property name with the required symbol next to it</summary>
                    /// <param name="propName" type="Object">the property name to display</param>
                    /// <returns type="string">The label name for a required property</returns> 
                    return propName + "*";
                });

            cswPublic.attach = function (object) {
                /// <summary>Attach an object to this element.</summary>
                /// <param name="object" type="Object">Raw HTML. Warning: Do not pass a selector to this method!</param>
                /// <returns type="Object">The new Csw object (for chaining)</returns> 
                var $child = null, ret;
                try {
                    $child = $(object);
                    if (false === Csw.isNullOrEmpty($child)) {
                        cswPublic.$.append($child);
                    }
                } catch (e) {
                    /* One day we'll implement client-side error handling */
                }
                ret = cswPrivate.makeControlForChain($child);
                return ret;
            };

            cswPublic.bind = function (eventName, event) {
                /// <summary>Bind an action to a jQuery element's event.</summary>
                /// <param name="$element" type="jQuery">A jQuery element</param>
                /// <param name="eventName" type="String">The name of the event</param>
                /// <param name="event" type="Function">A function to execute when the event fires</param>
                /// <returns type="Object">The jQuery element (for chaining)</returns> 
                cswPublic.$.bind(eventName, event);
                return cswPublic;
            };

            cswPublic.children = function (searchTerm, selector) {
                /// <summary>Find the child elements of this DOM element represented by this object</summary>
                /// <param name="searchTerm" type="String">(Optional) Some search term to limit child results</param>
                /// <param name="selector" type="String">(Optional) A selector</param>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = cswPublic.$.children(Csw.string(searchTerm), Csw.string(selector)),
                        ret = cswPrivate.makeControlForChain(_$element);
                return ret;
            };

            cswPublic.clickOnEnter = function (cswControl) {
                /// <summary>Bind an event to the enter key, when pressed in this control.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                cswPublic.$.clickOnEnter(cswControl.$);
                return cswPublic;
            };

            cswPublic.css = function (param1, param2) {
                /// <param name="param1" type="Object">Either a single JSON object with CSS to apply, or a single CSS name</param>
                /// <param name="param2" type="string">single CSS value</param>
                if (arguments.length === 1) {
                    cswPublic.$.css(param1);
                } else {
                    cswPublic.$.css(param1, param2);
                }
                return cswPublic;
            };

            cswPublic.data = function (prop, val) {
                /// <summary>Store property data on the control.</summary>
                /// <returns type="Object">All properties, a single property, or the control if defining a property (for chaining).</returns> 
                var ret = '',
                        _internal = Csw.clientDb.getItem('control_data_' + cswPrivate.id) || {};
                switch (arguments.length) {
                    case 0:
                        ret = _internal || cswPublic.$.data();
                        break;
                    case 1:
                        ret = _internal[prop] || cswPublic.$.data(prop);
                        break;
                    case 2:
                        _internal[prop] = val;
                        cswPublic.$.data(prop, val);
                        Csw.clientDb.setItem('control_data_' + cswPrivate.id, _internal);
                        ret = cswPublic;
                        break;
                }
                return ret;

            };

            cswPublic.empty = function () {
                /// <summary>Empty the element.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                cswPublic.$.empty();
                return cswPublic;
            };

            cswPublic.filter = function (selector) {
                /// <summary>Filter the child elements of this DOM element according to this selector</summary>
                /// <param name="selector" type="String">A filter string.</param>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = cswPublic.$.filter(selector),
                        ret = cswPrivate.makeControlForChain(_$element);
                return ret;
            };

            cswPublic.find = function (selector) {
                /// <summary>Find the child elements of this DOM element represented by this object</summary>
                /// <param name="selector" type="String">A selector, id or jQuery object to find.</param>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = cswPublic.$.find(Csw.string(selector)),
                        ret = cswPrivate.makeControlForChain(_$element);
                return ret;
            };

            cswPublic.first = function () {
                /// <summary>Find the first child element of this DOM element represented by this object</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = cswPublic.$.first(),
                        ret = cswPrivate.makeControlForChain(_$element);
                return ret;
            };

            cswPublic.getId = function () {
                /// <summary>Get the DOM Element ID of this object.</summary>
                /// <returns type="String">Element ID.</returns> 
                return cswPrivate.id;
            };

            cswPublic.hide = function () {
                /// <summary>Make the element invisible.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                cswPublic.$.hide();
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
                return Csw.number(cswPublic.$.length);
            };

            cswPublic.parent = cswPublic.parent || function () {
                /// <summary>Get the parent of this control</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = cswPublic.$.parent(),
                    ret;

                if (false === Csw.isNullOrEmpty(_$element, true)) {
                    ret = cswPublic.jquery(_$element);
                } else {
                    ret = {};
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
                var ret = cswPublic,
                        prop;

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
                return ret;
            };

            cswPublic.propNonDom = function (name, value) {
                /// <summary>
                ///   Gets or sets an Non-Dom attribute
                /// </summary>
                /// <param name="name" type="String">The name of the attribute</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
                var ret = cswPublic,
                        prop;
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
                return ret;
            };

            cswPublic.remove = function () {
                /// <summary>Remove the element and delete the object.</summary>
                /// <returns type="null"></returns> 
                cswPublic.$.remove();
                cswPublic = null;
                return cswPublic;
            };

            cswPublic.removeClass = function (name) {
                /// <summary>Remove a CSS class to an element.</summary>
                /// <param name="$element" type="jQuery">An element to remove class from.</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Classless jQuery element (for chaining)</returns> 
                cswPublic.$.removeClass(name);
                return cswPublic;
            };

            cswPublic.root = cswPublic.root || function () {
                /// <summary>Get the root (great, great, great grandparent) of this control</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = cswPublic.$.parent(),
                    ret;
                while (false === Csw.isNullOrEmpty(_$element.parent(), true)) {
                    _$element = _$element.parent();
                }
                if (false === Csw.isNullOrEmpty(_$element, true)) {
                    ret = cswPublic.jquery(_$element);
                } else {
                    ret = {};
                }
                return ret;
            };

            cswPublic.show = function () {
                /// <summary>Make the element visible.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                cswPublic.$.show();
                return cswPublic;
            };

            cswPublic.text = function (text) {
                /// <summary>Get the value of the element.</summary>
                /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
                if (arguments.length === 1 && false === Csw.isNullOrUndefined(text)) {
                    cswPublic.$.text(text);
                    return cswPublic;
                } else {
                    return Csw.string(cswPublic.$.text());
                }
            };

            cswPublic.trigger = function (eventName, eventOpts) {
                /// <summary>Trigger an event bound to a jQuery element.</summary>
                /// <param name="$element" type="jQuery">A jQuery element</param>
                /// <param name="eventName" type="String">The name of the event</param>
                /// <param name="eventOpts" type="Object">Options collection to pass to the event handler.</param>
                /// <returns type="Object">The jQuery element (for chaining)</returns> 
                cswPublic.$.trigger(eventName, eventOpts);
                return cswPublic;
            };

            cswPublic.unbind = function (eventName, event) {
                /// <summary>Unbind an action from a jQuery element's event.</summary>
                /// <param name="$element" type="jQuery">A jQuery element</param>
                /// <param name="eventName" type="String">The name of the event</param>
                /// <returns type="Object">The jQuery element (for chaining)</returns> 
                cswPublic.$.unbind(eventName, event);
                return cswPublic;
            };

            cswPublic.val = cswPublic.val || function (value) {
                /// <summary>Get the value of the element.</summary>
                /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
                if (arguments.length === 1 && false === Csw.isNullOrUndefined(value)) {
                    cswPublic.$.val(value);
                    return cswPublic;
                } else {
                    return Csw.string(cswPublic.$.val());
                }
            };

            cswPublic.valueOf = function () {
                return cswPublic;
            };

            return cswPublic;
        });

    Csw.makeId = Csw.makeId ||
        Csw.register('makeId', function (options, id, suffix, delimiter, isUnique) {
            /// <summary>
            ///   Generates an ID for DOM assignment
            /// </summary>
            /// <param name="options" type="Object">
            ///     A JSON Object or a prefix as string
            ///     &#10;1 - options.id: Base ID string
            ///     &#10;2 - options.prefix: String prefix to prepend
            ///     &#10;3 - options.suffix: String suffix to append
            ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
            /// </param>
            /// <param name="ID" type="Object"></param>
            /// <param name="suffix" type="Object"></param>
            /// <param name="delimiter" type="Object"></param>
            ///	<returns type="String">A concatenated string of provided values</returns>
            var cswPrivate = {
                idCount: 1 + Csw.number(Csw.getGlobalProp('uniqueIdCount'), 0),
                prefix: '',
                id: id,
                suffix: suffix,
                Delimiter: delimiter
            };
            var elementId = [];

            if (Csw.isPlainObject(options)) {
                Csw.extend(cswPrivate, options);
            } else {
                cswPrivate.prefix = options;
            }
            cswPrivate.Delimiter = Csw.string(cswPrivate.Delimiter, '_');

            if (false === Csw.isNullOrEmpty(cswPrivate.prefix)) {
                elementId.push(Csw.string(cswPrivate.prefix));
            }
            if (false === Csw.isNullOrEmpty(cswPrivate.id)) {
                elementId.push(cswPrivate.id);
            }

            if (false === Csw.isNullOrEmpty(cswPrivate.suffix)) {
                elementId.push(cswPrivate.suffix);
            }
            //            if (Csw.bool(isUnique, true)) {
            //                Csw.setGlobalProp('uniqueIdCount', cswPrivate.idCount);
            //                elementId.push(cswPrivate.idCount);
            //            }
            return elementId.join(cswPrivate.Delimiter);
        });

    Csw.makeSafeId = Csw.makeSafeId ||
        Csw.register('makeSafeId', function (options, prefix, suffix, delimiter) {
            /// <summary>   Generates a "safe" ID for DOM assignment </summary>
            /// <param name="options" type="Object">
            ///     A JSON Object
            ///     &#10;1 - options.ID: Base ID string
            ///     &#10;2 - options.prefix: String prefix to prepend
            ///     &#10;3 - options.suffix: String suffix to append
            ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
            /// </param>
            /// <returns type="String">A concatenated string of provided values</returns>
            var elementId, i, toReplace;
            var o = {
                ID: '',
                prefix: Csw.string(prefix),
                suffix: Csw.string(suffix),
                Delimiter: Csw.string(delimiter, '_')
            };
            if (Csw.isPlainObject(options)) {
                Csw.extend(o, options);
            } else {
                o.ID = Csw.string(options);
            }

            elementId = o.ID;
            //toReplace = [/'/gi, / /gi, /\//g];
            toReplace = [/\(/g, /\)/g, /'/gi, / /gi, /\//g];
            if (false === Csw.isNullOrEmpty(o.prefix) && false === Csw.isNullOrEmpty(elementId)) {
                elementId = o.prefix + o.Delimiter + elementId;
            }
            if (false === Csw.isNullOrEmpty(o.suffix) && false === Csw.isNullOrEmpty(elementId)) {
                elementId += o.Delimiter + o.suffix;
            }
            for (i = 0; i < toReplace.length; i += 1) {
                if (Csw.contains(toReplace, i)) {
                    if (false === Csw.isNullOrEmpty(elementId)) {
                        elementId = elementId.replace(toReplace[i], '');
                    }
                }
            }
            return elementId;
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

    Csw.tryParseElement = Csw.tryParseElement ||
        Csw.register('tryParseElement', function (elementId, $context) {
            /// <summary>Attempts to fetch an element from the DOM first through jQuery, then through JavaScript.</summary>
            /// <param name="elementId" type="String"> ElementId to find </param>
            /// <param name="$context" type="jQuery"> Optional context to limit the search </param>
            /// <returns type="jQuery">jQuery object, empty if no match found.</returns>
            var $ret = $('');
            var document = Csw.getGlobalProp('document');
            if (false === Csw.isNullOrEmpty(elementId)) {
                if (arguments.length === 2 && false === Csw.isNullOrEmpty($context)) {
                    $ret = $('#' + elementId, $context);
                } else {
                    $ret = $('#' + elementId);
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

} ());


