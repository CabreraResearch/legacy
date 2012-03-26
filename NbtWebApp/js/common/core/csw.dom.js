/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
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

            var internal = {};
            var external = options || {
                parentId: ''
            };

            if (Csw.isJQuery(element)) {
                external.$ = element;
                internal.id = Csw.string(element.prop('id'));
                external.isValid = true;
            } else if (false === Csw.isNullOrEmpty(element) && Csw.isJQuery(element.$)) {
                /*This is already a Csw dom object*/
                return element;
            } else {
                internal.id = '';
                external.$ = {};
            }

            internal.makeControlForChain = function ($child, method) {
                var ret,
                    _options = {
                        parent: function () { return external; },
                        first: function () { return external; },
                        root: external.root,
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
                        ret = external.jquery($child, _options);
                    }
                } else {
                    ret = _options;
                }
                return ret;
            };

            internal.prepControl = function (opts, controlName) {
                var id = opts.id || controlName;
                opts = opts || {};
                internal.id = internal.id || Csw.makeId(external.parentId, 'sub', id, '_', false);
                //opts.ID = opts.ID || internal.makeId(external.parentId, 'sub', id, '_', false);
                opts.$ = external.$;
                opts.root = external.root;

                return opts;
            };

            external.addClass = function (name) {
                /// <summary>Add a CSS class to an element.</summary>
                /// <param name="$element" type="jQuery">An element to add class to.</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Classy jQuery element (for chaining)</returns> 
                external.$.addClass(name);
                return external;
            };

            external.append = function (object) {
                /// <summary>Append an object to this element.</summary>
                /// <param name="object" type="Object">Raw HTML, a jQuery object or text.</param>
                /// <returns type="Object">The parent Csw object (for chaining)</returns> 
                try {
                    external.$.append(object);
                } catch (e) {
                    Csw.log('Warning: append() failed, text() was used instead.', true);
                    if (Csw.isString(object)) {
                        external.$.text(object);
                    }
                }
                return external;
            };

            external.attach = function (object) {
                /// <summary>Attach an object to this element.</summary>
                /// <param name="object" type="Object">Raw HTML. Warning: Do not pass a selector to this method!</param>
                /// <returns type="Object">The new Csw object (for chaining)</returns> 
                var $child = null, ret;
                try {
                    $child = $(object);
                    if (false === Csw.isNullOrEmpty($child)) {
                        external.$.append($child);
                    }
                } catch (e) {
                    /* One day we'll implement client-side error handling */
                }
                ret = internal.makeControlForChain($child);
                return ret;
            };

            external.bind = function (eventName, event) {
                /// <summary>Bind an action to a jQuery element's event.</summary>
                /// <param name="$element" type="jQuery">A jQuery element</param>
                /// <param name="eventName" type="String">The name of the event</param>
                /// <param name="event" type="Function">A function to execute when the event fires</param>
                /// <returns type="Object">The jQuery element (for chaining)</returns> 
                external.$.bind(eventName, event);
                return external;
            };

            external.children = function (searchTerm, selector) {
                /// <summary>Find the child elements of this DOM element represented by this object</summary>
                /// <param name="searchTerm" type="String">(Optional) Some search term to limit child results</param>
                /// <param name="selector" type="String">(Optional) A selector</param>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = external.$.children(Csw.string(searchTerm), Csw.string(selector)),
                        ret = internal.makeControlForChain(_$element);
                return ret;
            };

            external.clickOnEnter = function (cswControl) {
                /// <summary>Bind an event to the enter key, when pressed in this control.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                external.$.clickOnEnter(cswControl.$);
                return external;
            };

            external.css = function(param1, param2) {
                /// <param name="param1" type="Object">Either a single JSON object with CSS to apply, or a single CSS name</param>
                /// <param name="param2" type="string">single CSS value</param>
                if (arguments.length === 1) {
                    external.$.css(param1);
                } else {
                    external.$.css(param1, param2);
                }
                return external;
            };

            external.data = function (prop, val) {
                /// <summary>Store property data on the control.</summary>
                /// <returns type="Object">All properties, a single property, or the control if defining a property (for chaining).</returns> 
                var ret = '',
                        _internal = Csw.clientDb.getItem('control_data_' + internal.id) || {};
                switch (arguments.length) {
                    case 0:
                        ret = _internal || external.$.data();
                        break;
                    case 1:
                        ret = _internal[prop] || external.$.data(prop);
                        break;
                    case 2:
                        _internal[prop] = val;
                        external.$.data(prop, val);
                        Csw.clientDb.setItem('control_data_' + internal.id, _internal);
                        ret = external;
                        break;
                }
                return ret;

            };

            external.empty = function () {
                /// <summary>Empty the element.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                external.$.empty();
                return external;
            };

            external.filter = function (selector) {
                /// <summary>Filter the child elements of this DOM element according to this selector</summary>
                /// <param name="selector" type="String">A filter string.</param>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = external.$.filter(selector),
                        ret = internal.makeControlForChain(_$element);
                return ret;
            };

            external.find = function (selector) {
                /// <summary>Find the child elements of this DOM element represented by this object</summary>
                /// <param name="selector" type="String">A selector, id or jQuery object to find.</param>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = external.$.find(Csw.string(selector)),
                        ret = internal.makeControlForChain(_$element);
                return ret;
            };

            external.first = function () {
                /// <summary>Find the first child element of this DOM element represented by this object</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = external.$.first(),
                        ret = internal.makeControlForChain(_$element);
                return ret;
            };

            external.getId = function () {
                /// <summary>Get the DOM Element ID of this object.</summary>
                /// <returns type="String">Element ID.</returns> 
                return internal.id;
            };

            external.hide = function () {
                /// <summary>Make the element invisible.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                external.$.hide();
                return external;
            };

            external.jquery = function ($jqElement, opts) {
                /// <summary> Extend a jQuery object with Csw methods.</summary>
                /// <param name="$element" type="jQuery">Element to extend.</param>
                /// <returns type="jquery">A Csw.jquery object</returns>
                opts = internal.controlPreProcessing(opts, 'jquery');
                return Csw.controls.factory($jqElement, opts);
            };

            external.length = function () {
                /// <summary>Get the length of this element.</summary>
                /// <returns type="Number">Number of elements at the current level of the tree.</returns> 
                return Csw.number(external.$.length);
            };

            external.parent = external.parent || function () {
                /// <summary>Get the parent of this control</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = external.$.parent(),
                    ret;

                if (false === Csw.isNullOrEmpty(_$element, true)) {
                    ret = external.jquery(_$element);
                } else {
                    ret = {};
                }
                return ret;
            };

            external.propDom = function (name, value) {
                /// <summary>
                ///   Gets or sets a DOM property
                /// </summary>
                /// <param name="name" type="String">The name of the attribute</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
                var ret = external,
                        prop;

                try {
                    if (typeof name === "object") {
                        for (prop in name) {
                            doProp(external.$, prop, name[prop]);
                        }
                    } else {
                        ret = doProp(external.$, name, value);
                    }
                } catch (e) {
                    //We're in IE hell. Do nothing.
                }
                if (arguments.length === 2 || Csw.isPlainObject(name)) {
                    ret = external;
                }
                return ret;
            };

            external.propNonDom = function (name, value) {
                /// <summary>
                ///   Gets or sets an Non-Dom attribute
                /// </summary>
                /// <param name="name" type="String">The name of the attribute</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
                var ret = external,
                        prop;
                try {
                    if (typeof name === "object") {
                        for (prop in name) {
                            doAttr(external.$, prop, name[prop]);
                        }
                    } else {
                        ret = doAttr(external.$, name, value);
                    }
                    // For proper chaining support
                } catch (e) {
                    //We're in IE hell. Do nothing.
                }
                if (arguments.length === 2 || Csw.isPlainObject(name)) {
                    ret = external;
                }
                return ret;
            };

            external.remove = function () {
                /// <summary>Remove the element and delete the object.</summary>
                /// <returns type="null"></returns> 
                external.$.remove();
                external = null;
                return external;
            };

            external.removeClass = function (name) {
                /// <summary>Remove a CSS class to an element.</summary>
                /// <param name="$element" type="jQuery">An element to remove class from.</param>
                /// <param name="value" type="String">The value of the attribute</param>
                /// <returns type="Object">Classless jQuery element (for chaining)</returns> 
                external.$.removeClass(name);
                return external;
            };

            external.root = external.root || function () {
                /// <summary>Get the root (great, great, great grandparent) of this control</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                var _$element = external.$.parent(),
                    ret;
                while (false === Csw.isNullOrEmpty(_$element.parent(), true)) {
                    _$element = _$element.parent();
                }
                if (false === Csw.isNullOrEmpty(_$element, true)) {
                    ret = external.jquery(_$element);
                } else {
                    ret = {};
                }
                return ret;
            };

            external.show = function () {
                /// <summary>Make the element visible.</summary>
                /// <returns type="Object">The Csw object (for chaining)</returns> 
                external.$.show();
                return external;
            };

            external.text = function (text) {
                /// <summary>Get the value of the element.</summary>
                /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
                if (arguments.length === 1 && false === Csw.isNullOrUndefined(text)) {
                    external.$.text(text);
                    return external;
                } else {
                    return Csw.string(external.$.text());
                }
            };

            external.trigger = function (eventName, eventOpts) {
                /// <summary>Trigger an event bound to a jQuery element.</summary>
                /// <param name="$element" type="jQuery">A jQuery element</param>
                /// <param name="eventName" type="String">The name of the event</param>
                /// <param name="eventOpts" type="Object">Options collection to pass to the event handler.</param>
                /// <returns type="Object">The jQuery element (for chaining)</returns> 
                external.$.trigger(eventName, eventOpts);
                return external;
            };

            external.unbind = function (eventName, event) {
                /// <summary>Unbind an action from a jQuery element's event.</summary>
                /// <param name="$element" type="jQuery">A jQuery element</param>
                /// <param name="eventName" type="String">The name of the event</param>
                /// <returns type="Object">The jQuery element (for chaining)</returns> 
                external.$.unbind(eventName, event);
                return external;
            };

            external.val = external.val || function (value) {
                /// <summary>Get the value of the element.</summary>
                /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
                if (arguments.length === 1 && false === Csw.isNullOrUndefined(value)) {
                    external.$.val(value);
                    return external;
                } else {
                    return Csw.string(external.$.val());
                }
            };

            external.valueOf = function () {
                return external;
            };

            return external;
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
            var internal = {
                idCount: 1 + Csw.number(Csw.getGlobalProp('uniqueIdCount'), 0),
                prefix: '',
                id: id,
                suffix: suffix,
                Delimiter: delimiter
            };
            var elementId = [];

            if (Csw.isPlainObject(options)) {
                $.extend(internal, options);
            } else {
                internal.prefix = options;
            }
            internal.Delimiter = Csw.string(internal.Delimiter, '_');

            if (false === Csw.isNullOrEmpty(internal.prefix)) {
                elementId.push(Csw.string(internal.prefix));
            }
            if (false === Csw.isNullOrEmpty(internal.id)) {
                elementId.push(internal.id);
            }
            if (false === Csw.isNullOrEmpty(internal.suffix)) {
                elementId.push(internal.suffix);
            }
            if (Csw.bool(isUnique, true)) {
                Csw.setGlobalProp('uniqueIdCount', internal.idCount);
                elementId.push(internal.idCount);
            }
            return elementId.join(internal.Delimiter);
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
                $.extend(o, options);
            } else {
                o.ID = Csw.string(options);
            }

            elementId = o.ID;
            toReplace = [/'/gi, / /gi, /\//g];
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
            var internal = {
                attributes: {}
            };
            var external = {};

            external.add = function (key, value) {
                if (false === Csw.isNullOrEmpty(key)) {
                    internal.attributes[Csw.string(key)] = Csw.string(value);
                }
            };

            external.get = function () {
                var ret = '';

                function buildStyle(val, key) {
                    if (false === Csw.isNullOrEmpty(val) &&
                        false === Csw.isNullOrEmpty(key)) {
                        ret += ' ' + key + '="' + val + '" ';
                    }
                }

                Csw.each(internal.attributes, buildStyle);

                return ret;
            };

            return external;
        });

    Csw.makeStyle = Csw.makeStyle ||
        Csw.register('makeStyle', function () {
            /// <summary> Build an HTML element style string </summary>
            /// <returns type="String">A string of style key/value pairs</returns>
            var internal = {
                styles: {}
            };
            var external = {};

            external.add = function (key, value) {
                internal.styles[key] = value;
            };

            external.set = function (stylesObj) {
                internal.styles = stylesObj;
            };

            external.get = function () {
                var htmlStyle = '', ret = '';

                function buildStyle(val, key) {
                    if (false === Csw.isNullOrEmpty(key) &&
                        false === Csw.isNullOrEmpty(val)) {
                        htmlStyle += key + ': ' + val + ';';
                    }
                }

                Csw.each(internal.styles, buildStyle);

                if (htmlStyle.length > 0) {
                    ret = ' style="' + htmlStyle + '"';
                }
                return ret;
            };

            return external;
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


