/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    function factory($element, options) {
        /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
        /// <param name="$element" type="jQuery">An element to bind to.</param>
        /// <param name="options" type="Object">An options collection to extend.</param>
        /// <returns type="Object">The options object with DOM methods attached.</returns> 
        var internal = {};
        options = options || {};
        options.$ = $element;
        internal.id = Csw.string($element.prop('id'));

        internal.prepControl = function (opts, controlName) {
            opts = opts || {};
            opts.ID = opts.ID || Csw.controls.dom.makeId(internal.id, 'sub', controlName);
            opts.$parent = $element;
            opts.parent = function () {
                return options;
            };
            return opts;
        };

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
            opts = internal.prepControl(opts, 'table');
            return Csw.controls.table(opts);
        };

        options.layoutTable = function (opts) {
            /// <summary> Creates a Csw.layoutTable on this element</summary>
            /// <param name="tableOpts" type="Object">Options to define the table.</param>
            /// <returns type="Object">A Csw.layoutTable</returns> 
            opts = internal.prepControl(opts, 'layoutTable');
            return Csw.controls.layoutTable(opts);
        };

        options.div = function (opts) {
            /// <summary> Creates a Csw.div on this element</summary>
            /// <param name="opts" type="Object">Options to define the div.</param>
            /// <returns type="Object">A Csw.div</returns> 
            opts = internal.prepControl(opts, 'div');
            return Csw.controls.div(opts);
        };

        options.tabDiv = function (opts) {
            /// <summary> Creates a Csw.tabDiv on this element</summary>
            /// <param name="opts" type="Object">Options to define the div.</param>
            /// <returns type="Object">A Csw.tabDiv</returns> 
            opts = internal.prepControl(opts, 'tabdiv');
            return Csw.controls.tabDiv(opts);
        };

        options.br = function (opts) {
            /// <summary> Creates a Csw.br on this element</summary>
            /// <param name="opts" type="Object">Options to define the br.</param>
            /// <returns type="Object">A Csw.br</returns> 
            opts = internal.prepControl(opts, 'br');
            return Csw.controls.br(opts);
        };

        options.ul = function (opts) {
            /// <summary> Creates a Csw.ul on this element</summary>
            /// <param name="options" type="Object">Options to define the ul.</param>
            /// <returns type="Object">A Csw.ul</returns> 
            opts = internal.prepControl(opts, 'ul');
            return Csw.controls.ul(opts);
        };

        options.li = function (opts) {
            /// <summary> Creates a Csw.li on this element</summary>
            /// <param name="options" type="Object">Options to define the li.</param>
            /// <returns type="Object">A Csw.li</returns> 
            opts = internal.prepControl(opts, 'li');
            return Csw.controls.li(opts);
        };

        options.span = function (opts) {
            /// <summary> Creates a Csw.span on this element</summary>
            /// <param name="spanOpts" type="Object">Options to define the span.</param>
            /// <returns type="Object">A Csw.span</returns> 
            opts = internal.prepControl(opts, 'span');
            return Csw.controls.span(opts);
        };

        options.input = function (opts) {
            /// <summary> Creates a Csw.input on this element</summary>
            /// <param name="inputOpts" type="Object">Options to define the input.</param>
            /// <returns type="Object">A Csw.input</returns> 
            opts = internal.prepControl(opts, 'input');
            return Csw.controls.input(opts);
        };

        options.button = function (opts) {
            /// <summary> Creates a Csw.button on this element</summary>
            /// <param name="buttonOpts" type="Object">Options to define the button.</param>
            /// <returns type="Object">A Csw.button</returns> 
            opts = internal.prepControl(opts, 'button');
            return Csw.controls.button(opts);
        };

        options.link = function (opts) {
            /// <summary> Creates a Csw.link on this element</summary>
            /// <param name="buttonOpts" type="Object">Options to define the link.</param>
            /// <returns type="Object">A Csw.link</returns> 
            opts = internal.prepControl(opts, 'link');
            return Csw.controls.link(opts);
        };

        options.form = function (opts) {
            /// <summary> Creates a Csw.form on this element</summary>
            /// <param name="formOpts" type="Object">Options to define the form.</param>
            /// <returns type="Object">A Csw.form</returns> 
            opts = internal.prepControl(opts, 'form');
            return Csw.controls.form(opts);
        };

        options.img = function (opts) {
            /// <summary> Creates a Csw.form on this element</summary>
            /// <param name="formOpts" type="Object">Options to define the form.</param>
            /// <returns type="Object">A Csw.form</returns> 
            opts = internal.prepControl(opts, 'img');
            return Csw.controls.form(opts);
        };

        options.jquery = function ($jqElement, opts) {
            /// <summary> Extend a jQuery object with Csw methods.</summary>
            /// <param name="$element" type="jQuery">Element to extend.</param>
            /// <returns type="jquery">A Csw.jquery object</returns>
            opts = internal.prepControl(opts, 'jquery');
            return factory($jqElement, opts);
        };

        //#endregion Csw DOM classes

        //#region Csw "jQuery" classes

        options.parent = options.parent || function () {
            /// <summary>Get the parent of this control</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.parent();
            var ret = options.jquery(_$element);
            return ret;
        };

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
            var _options = {
                parent: function () { return options; }
            };
            var ret = options.jquery(_$element, _options);
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

        options.first = function () {
            /// <summary>Find the first child element of this DOM element represented by this object</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.first();
            var _options = {
                parent: function () { return options; }
            };
            var ret = options.jquery(_$element, _options);
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
            var _options = {
                parent: function () { return options; }
            };
            var ret = factory(_$element, _options);
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
    Csw.controls.register('factory', factory);
    Csw.controls.factory = Csw.controls.factory || factory;

} ());


