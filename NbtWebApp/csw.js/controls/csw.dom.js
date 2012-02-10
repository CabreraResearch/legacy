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

        external.makeId = function (options, prefix, suffix, delimiter) {
            /// <summary>
            ///   Generates an ID for DOM assignment
            /// </summary>
            /// <param name="options" type="Object">
            ///     A JSON Object
            ///     &#10;1 - options.ID: Base ID string
            ///     &#10;2 - options.prefix: String prefix to prepend
            ///     &#10;3 - options.suffix: String suffix to append
            ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
            /// </param>
            /// <returns type="String">A concatenated string of provided values</returns>
            var elementId;
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
            if (false === Csw.isNullOrEmpty(o.prefix) && false === Csw.isNullOrEmpty(elementId)) {
                elementId = o.prefix + o.Delimiter + elementId;
            }
            if (false === Csw.isNullOrEmpty(o.suffix) && false === Csw.isNullOrEmpty(elementId)) {
                elementId += o.Delimiter + o.suffix;
            }
            return elementId;
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
            /// <returns type="String">A br object</returns>
            var _internal = {
                styles: {}
            };
            var _external = {};

            _external.add = function (value, key) {
                _internal.styles[key] = value;
            };

            _external.get = function () {
                var htmlStyle = '', ret = '';

                function buildStyle(key, val) {
                    htmlStyle += key + ': ' + val + ';';
                }

                Csw.each(_internal.styles, buildStyle);

                if (htmlStyle.length > 0) {
                    ret = ' style="' + htmlStyle + '"';
                }
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

        return external;
    } ());
    Csw.controls.register('dom', dom);
    Csw.controls.dom = Csw.controls.dom || dom;

    function domExtend($element, options) {
        /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
        /// <param name="$element" type="jQuery">An element to bind to.</param>
        /// <param name="options" type="Object">An options collection to extend.</param>
        /// <returns type="Object">The options object with DOM methods attached.</returns> 
        var internal = {};
        options = options || {};
        options.$ = $element;
        internal.id = Csw.string($element.prop('id'));

        //#region Csw DOM classes

        options.getId = function () {
            /// <summary>Get the DOM Element ID of this object.</summary>
            /// <returns type="String">Element ID.</returns> 
            return internal.id;
        };

        options.length = function () {
            /// <summary>Get the length of this element.</summary>
            /// <returns type="Number">Number of elements at the current level of the tree.</returns> 
            return Csw.number($element.length);
        };

        options.propDom = function (name, value) {
            /// <summary>Gets or sets a DOM property</summary>
            /// <param name="name" type="String">The name of the attribute</param>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
            return Csw.controls.dom.propDom($element, name, value);
        };
        options.propNonDom = function (name, value) {
            /// <summary> Gets or sets an Non-Dom attribute</summary>
            /// <param name="name" type="String">The name of the attribute</param>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
            return Csw.controls.dom.propNonDom($element, name, value);
        };

        options.table = function (opts) {
            /// <summary> Creates a Csw.table on this element</summary>
            /// <param name="tableOpts" type="Object">Options to define the table.</param>
            /// <returns type="Object">A Csw.table</returns> 
            opts = opts || {};
            opts.ID = opts.ID || Csw.controls.dom.makeId(internal.id, 'subtbl');
            opts.$parent = $element;
            return Csw.controls.table(opts);
        };

        options.div = function (opts) {
            /// <summary> Creates a Csw.div on this element</summary>
            /// <param name="divOpts" type="Object">Options to define the div.</param>
            /// <returns type="Object">A Csw.div</returns> 
            opts = opts || {};
            opts.ID = opts.ID || Csw.controls.dom.makeId(internal.id, 'subdiv');
            opts.$parent = $element;
            return Csw.controls.div(opts);
        };

        options.br = function (opts) {
            /// <summary> Creates a Csw.br on this element</summary>
            /// <param name="options" type="Object">Options to define the br.</param>
            /// <returns type="Object">A Csw.br</returns> 
            opts.$parent = $element;
            return Csw.controls.br(opts);
        };

        options.span = function (opts) {
            /// <summary> Creates a Csw.span on this element</summary>
            /// <param name="spanOpts" type="Object">Options to define the span.</param>
            /// <returns type="Object">A Csw.span</returns> 
            opts.ID = opts.ID || Csw.controls.dom.makeId(internal.id, 'subspan');
            opts.$parent = $element;
            return Csw.controls.span(opts);
        };

        options.input = function (opts) {
            /// <summary> Creates a Csw.input on this element</summary>
            /// <param name="inputOpts" type="Object">Options to define the input.</param>
            /// <returns type="Object">A Csw.input</returns> 
            opts = opts || {};
            opts.ID = opts.ID || Csw.controls.dom.makeId(internal.id, 'subinput');
            opts.$parent = $element;
            return Csw.controls.input(opts);
        };

        options.button = function (opts) {
            /// <summary> Creates a Csw.button on this element</summary>
            /// <param name="buttonOpts" type="Object">Options to define the button.</param>
            /// <returns type="Object">A Csw.button</returns> 
            opts = opts || {};
            opts.ID = opts.ID || Csw.controls.dom.makeId(internal.id, 'subbutton');
            opts.$parent = $element;
            return Csw.controls.button(opts);
        };

        options.form = function (opts) {
            /// <summary> Creates a Csw.form on this element</summary>
            /// <param name="formOpts" type="Object">Options to define the form.</param>
            /// <returns type="Object">A Csw.form</returns> 
            opts = opts || {};
            opts.ID = opts.ID || Csw.controls.dom.makeId(internal.id, 'subform');
            opts.$parent = $element;
            return Csw.controls.form(opts);
        };

        options.jquery = function ($jqElement) {
            /// <summary> Extend a jQuery object with Csw methods.</summary>
            /// <param name="$element" type="jQuery">Element to extend.</param>
            /// <returns type="jquery">A Csw.jquery object</returns>
            return domExtend($jqElement, {});
        };

        //#endregion Csw DOM classes

        //#region Csw "jQuery" classes

        options.addClass = function (name) {
            /// <summary>Add a CSS class to an element.</summary>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.addClass($element, name);
            return options;
        };

        options.removeClass = function (name) {
            /// <summary>Remove a CSS class to an element.</summary>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.removeClass($element, name);
            return options;
        };

        options.css = function (values) {
            /// <summary>Add css styles to an element.</summary>
            /// <param name="values" type="Object">Styles to apply</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.css(values);
            return options;
        };

        options.bind = function (eventName, event) {
            /// <summary>Bind an action to a jQuery element's event.</summary>
            /// <param name="eventName" type="String">The name of the event</param>
            /// <param name="event" type="Function">A function to execute when the event fires</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.bind($element, eventName, event);
            return options;
        };
        options.trigger = function (eventName, eventOpts) {
            /// <summary>Trigger an event bound to a jQuery element.</summary>
            /// <param name="eventName" type="String">The name of the event</param>
            /// <param name="eventOpts" type="Object">Options collection to pass to the event handler.</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.trigger($element, eventName, eventOpts);
            return options;
        };

        options.children = function (searchTerm, selector) {
            /// <summary>Find the child elements of this DOM element represented by this object</summary>
            /// <param name="searchTerm" type="String">(Optional) Some search term to limit child results</param>
            /// <param name="selector" type="String">(Optional) A selector</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.children(Csw.string(searchTerm), Csw.string(selector));
            var ret = options.jquery(_$element);
            return ret;
        };

        options.find = function (selector) {
            /// <summary>Find the child elements of this DOM element represented by this object</summary>
            /// <param name="selector" type="String">A selector, id or jQuery object to find.</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.find(Csw.string(selector));
            var ret = options.jquery(_$element);
            return ret;
        };

        options.append = function (object) {
            /// <summary>Attach an object to this element.</summary>
            /// <param name="object" type="Object">Raw HTML, a jQuery object or text.</param>
            /// <returns type="Object">The appended Csw object (for chaining)</returns> 
            var _$element = $(object);
            if (false === Csw.isNullOrEmpty(object) && _$element.length === 0) {
                /* This handles plain text */
                $element.append(object);
            } else {
                $element.append(_$element); 
            }
            var ret = domExtend(_$element, {});
            return ret;
        };

        options.val = function (value) {
            /// <summary>Get the value of the element.</summary>
            /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
            if (arguments.length === 1 && false === Csw.isNullOrUndefined(value)) {
                $element.val(value);
                return options;
            } else {
                return Csw.string($element.val());
            }
        };

        options.text = function (text) {
            /// <summary>Get the value of the element.</summary>
            /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
            if (arguments.length === 1 && false === Csw.isNullOrUndefined(text)) {
                $element.text(text);
                return options;
            } else {
                return Csw.string($element.text());
            }
        };

        options.show = function () {
            /// <summary>Make the element visible.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.show();
            return options;
        };

        options.hide = function () {
            /// <summary>Make the element invisible.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.hide();
            return options;
        };

        options.empty = function () {
            /// <summary>Empty the element.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.empty();
            return options;
        };

        options.clickOnEnter = function (cswControl) {
            /// <summary>Bind an event to the enter key, when pressed in this control.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.clickOnEnter(cswControl.$);
            return options;
        };

        //#endregion Csw "jQuery" classes

        return options;
    }
    Csw.controls.register('domExtend', domExtend);
    Csw.controls.domExtend = Csw.controls.domExtend || domExtend;

} ());


