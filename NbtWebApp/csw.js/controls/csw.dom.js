/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    var dom = (function _dom() {
        var internal = {};
        var external = {};

        internal.doProp = function ($Dom, name, value) {
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
        };

        internal.doAttr = function ($this, name, value) {
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
        };

        external.propDom = function ($Dom, name, value) {
            /// <summary>
            ///   Gets or sets a DOM property
            /// </summary>
            /// <param name="name" type="String">The name of the attribute</param>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
            var ret = $Dom,
                prop;

            try {
                if (typeof name === "object") {
                    for (prop in name) {
                        internal.doProp($Dom, prop, name[prop]);
                    }
                } else {
                    ret = internal.doProp($Dom, name, value);
                }
            } catch (e) {
                //We're in IE hell. Do nothing.
            }
            return ret;
        };

        external.propNonDom = function ($this, name, value) {
            /// <summary>
            ///   Gets or sets an Non-Dom attribute
            /// </summary>
            /// <param name="name" type="String">The name of the attribute</param>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
            var ret = $this,
                prop;
            try {
                if (typeof name === "object") {
                    for (prop in name) {
                        internal.doAttr($this, prop, name[prop]);
                    }
                } else {
                    ret = internal.doAttr($this, name, value);
                }
                // For proper chaining support
            } catch (e) {
                //We're in IE hell. Do nothing.
            }
            return ret;
        };

        external.makeId = function (options, ID, suffix, delimiter, isUnique) {
            /// <summary>
            ///   Generates an ID for DOM assignment
            /// </summary>
            /// <param name="options" type="Object">
            ///     A JSON Object or a prefix as string
            ///     &#10;1 - options.ID: Base ID string
            ///     &#10;2 - options.prefix: String prefix to prepend
            ///     &#10;3 - options.suffix: String suffix to append
            ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
            /// </param>
            /// <param name="ID" type="Object"></param>
            /// <param name="suffix" type="Object"></param>
            /// <param name="delimiter" type="Object"></param>
            ///	<returns type="String">A concatenated string of provided values</returns>
            var _internal = {
                idCount: 1 + Csw.number(Csw.getGlobalProp('uniqueIdCount'), 0),
                prefix: '',
                ID: ID,
                suffix: suffix,
                Delimiter: delimiter
            };
            var elementId = [];

            if (Csw.isPlainObject(options)) {
                $.extend(_internal, options);
            } else {
                _internal.prefix = options;
            }
            _internal.Delimiter = Csw.string(_internal.Delimiter, '_');

            if (false === Csw.isNullOrEmpty(_internal.prefix)) {
                elementId.push(Csw.string(_internal.prefix));
            }
            if (false === Csw.isNullOrEmpty(_internal.ID)) {
                elementId.push(_internal.ID);
            }
            if (false === Csw.isNullOrEmpty(_internal.suffix)) {
                elementId.push(_internal.suffix);
            }
            if (Csw.bool(isUnique, true)) {
                Csw.setGlobalProp('uniqueIdCount', _internal.idCount);
                elementId.push(_internal.idCount);
            }
            return elementId.join(_internal.Delimiter);
        };

        external.makeSafeId = function (options, prefix, suffix, delimiter) {
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
        };

        external.tryParseElement = function (elementId, $context) {
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
        };

        external.style = function () {
            /// <summary> Build an HTML element style string </summary>
            /// <returns type="String">A string of style key/value pairs</returns>
            var _internal = {
                styles: {}
            };
            var _external = {};

            _external.add = function (key, value) {
                _internal.styles[key] = value;
            };

            _external.set = function (stylesObj) {
                _internal.styles = stylesObj;
            };

            _external.get = function () {
                var htmlStyle = '', ret = '';

                function buildStyle(val, key) {
                    if (false === Csw.isNullOrEmpty(key) &&
                        false === Csw.isNullOrEmpty(val)) {
                        htmlStyle += key + ': ' + val + ';';
                    }
                }

                Csw.each(_internal.styles, buildStyle);

                if (htmlStyle.length > 0) {
                    ret = ' style="' + htmlStyle + '"';
                }
                return ret;
            };

            return _external;
        };

        external.attributes = function () {
            /// <summary> Build an HTML element attribute string </summary>
            /// <returns type="String">A string of attribute key/value pairs</returns>
            var _internal = {
                attributes: {}
            };
            var _external = {};

            _external.add = function (key, value) {
                if (false === Csw.isNullOrEmpty(key)) {
                    _internal.attributes[Csw.string(key)] = Csw.string(value);
                }
            };

            _external.get = function () {
                var ret = '';

                function buildStyle(val, key) {
                    if(false === Csw.isNullOrEmpty(val) &&
                        false === Csw.isNullOrEmpty(key) ) {
                        ret += ' ' + key + '="' + val + '" ';
                    }
                }

                Csw.each(_internal.attributes, buildStyle);

                return ret;
            };

            return _external;
        };

        external.addClass = function ($el, name) {
            /// <summary>Add a CSS class to an element.</summary>
            /// <param name="$el" type="jQuery">An element to add class to.</param>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">Classy jQuery element (for chaining)</returns> 
            return $el.addClass(name);
        };

        external.removeClass = function ($el, name) {
            /// <summary>Remove a CSS class to an element.</summary>
            /// <param name="$el" type="jQuery">An element to remove class from.</param>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">Classless jQuery element (for chaining)</returns> 
            return $el.removeClass(name);
        };

        external.bind = function ($el, eventName, event) {
            /// <summary>Bind an action to a jQuery element's event.</summary>
            /// <param name="$el" type="jQuery">A jQuery element</param>
            /// <param name="eventName" type="String">The name of the event</param>
            /// <param name="event" type="Function">A function to execute when the event fires</param>
            /// <returns type="Object">The jQuery element (for chaining)</returns> 
            return $el.bind(eventName, event);
        };

        external.unbind = function ($el, eventName, event) {
            /// <summary>Unbind an action from a jQuery element's event.</summary>
            /// <param name="$el" type="jQuery">A jQuery element</param>
            /// <param name="eventName" type="String">The name of the event</param>
            /// <returns type="Object">The jQuery element (for chaining)</returns> 
            return $el.unbind(eventName, event);
        };

        external.trigger = function ($el, eventName, eventOpts) {
            /// <summary>Trigger an event bound to a jQuery element.</summary>
            /// <param name="$el" type="jQuery">A jQuery element</param>
            /// <param name="eventName" type="String">The name of the event</param>
            /// <param name="eventOpts" type="Object">Options collection to pass to the event handler.</param>
            /// <returns type="Object">The jQuery element (for chaining)</returns> 
            return $el.trigger(eventName, eventOpts);
        };

        external.children = function ($el, searchTerm, selector) {
            /// <summary>Find the child elements of this DOM element represented by this object</summary>
            /// <param name="$el" type="jQuery">A jQuery element</param>
            /// <param name="searchTerm" type="String">(Optional) Some search term to limit child results</param>
            /// <param name="selector" type="String">(Optional) A selector</param>
            /// <returns type="Object">The jQuery element(s) (for chaining)</returns> 
            return $el.children(Csw.string(searchTerm), Csw.string(selector));
        };

        external.find = function ($el, selector) {
            /// <summary>Find the child elements of this DOM element represented by this object</summary>
            /// <param name="$el" type="jQuery">A jQuery element</param>
            /// <param name="selector" type="String">A selector, id or jQuery object to find.</param>
            /// <returns type="Object">The jQuery element(s) (for chaining)</returns> 
            return $el.find(Csw.string(selector));
        };

        external.first = function ($el) {
            /// <summary>Find the first child element of this DOM element represented by this object</summary>
            /// <param name="$el" type="jQuery">A jQuery element</param>
            /// <returns type="Object">The jQuery element(s) (for chaining)</returns> 
            return $el.first();
        };

        return external;
    } ());
    Csw.controls.register('dom', dom);
    Csw.controls.dom = Csw.controls.dom || dom;

} ());


