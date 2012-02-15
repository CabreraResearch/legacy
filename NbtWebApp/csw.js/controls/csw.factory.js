/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    function factory($element, external) {
        /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
        /// <param name="$element" type="jQuery">An element to bind to.</param>
        /// <param name="options" type="Object">An options collection to extend.</param>
        /// <returns type="Object">The options object with DOM methods attached.</returns> 
        var internal = {};
        external = external || {};
        external.$ = $element;
        internal.id = Csw.string($element.prop('id'));
        internal.data = {};
        internal.prepControl = function (opts, controlName) {
            opts = opts || {};
            opts.ID = opts.ID || Csw.controls.dom.makeId(internal.id, 'sub', controlName);
            opts.$parent = $element;
            opts.parent = function () {
                return external;
            };
            return opts;
        };

        //#region Csw DOM classes

        external.getId = function () {
            /// <summary>Get the DOM Element ID of this object.</summary>
            /// <returns type="String">Element ID.</returns> 
            return internal.id;
        };

        external.length = function () {
            /// <summary>Get the length of this element.</summary>
            /// <returns type="Number">Number of elements at the current level of the tree.</returns> 
            return Csw.number($element.length);
        };

        external.propDom = function (name, value) {
            /// <summary>Gets or sets a DOM property</summary>
            /// <param name="name" type="String">The name of the attribute</param>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
            var ret = Csw.controls.dom.propDom($element, name, value);
            if (arguments.length === 2) {
                ret = external;
            }
            return ret;
        };
        external.propNonDom = function (name, value) {
            /// <summary> Gets or sets an Non-Dom attribute</summary>
            /// <param name="name" type="String">The name of the attribute</param>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 
            var ret = Csw.controls.dom.propNonDom($element, name, value);
            if (arguments.length === 2) {
                ret = external;
            }
            return ret;
        };

        external.table = function (opts) {
            /// <summary> Creates a Csw.table on this element</summary>
            /// <param name="tableOpts" type="Object">Options to define the table.</param>
            /// <returns type="Object">A Csw.table</returns> 
            opts = internal.prepControl(opts, 'table');
            return Csw.controls.table(opts);
        };

        external.layoutTable = function (opts) {
            /// <summary> Creates a Csw.layoutTable on this element</summary>
            /// <param name="tableOpts" type="Object">Options to define the table.</param>
            /// <returns type="Object">A Csw.layoutTable</returns> 
            opts = internal.prepControl(opts, 'layoutTable');
            return Csw.controls.layoutTable(opts);
        };

        external.div = function (opts) {
            /// <summary> Creates a Csw.div on this element</summary>
            /// <param name="opts" type="Object">Options to define the div.</param>
            /// <returns type="Object">A Csw.div</returns> 
            opts = internal.prepControl(opts, 'div');
            return Csw.controls.div(opts);
        };

        external.tabDiv = function (opts) {
            /// <summary> Creates a Csw.tabDiv on this element</summary>
            /// <param name="opts" type="Object">Options to define the div.</param>
            /// <returns type="Object">A Csw.tabDiv</returns> 
            opts = internal.prepControl(opts, 'tabdiv');
            return Csw.controls.tabDiv(opts);
        };

        external.br = function (opts) {
            /// <summary> Creates a Csw.br on this element</summary>
            /// <param name="opts" type="Object">Options to define the br.</param>
            /// <returns type="Object">A Csw.br</returns> 
            opts = internal.prepControl(opts, 'br');
            return Csw.controls.br(opts);
        };

        external.ul = function (opts) {
            /// <summary> Creates a Csw.ul on this element</summary>
            /// <param name="options" type="Object">Options to define the ul.</param>
            /// <returns type="Object">A Csw.ul</returns> 
            opts = internal.prepControl(opts, 'ul');
            return Csw.controls.ul(opts);
        };

        external.li = function (opts) {
            /// <summary> Creates a Csw.li on this element</summary>
            /// <param name="options" type="Object">Options to define the li.</param>
            /// <returns type="Object">A Csw.li</returns> 
            opts = internal.prepControl(opts, 'li');
            return Csw.controls.li(opts);
        };

        external.span = function (opts) {
            /// <summary> Creates a Csw.span on this element</summary>
            /// <param name="spanOpts" type="Object">Options to define the span.</param>
            /// <returns type="Object">A Csw.span</returns> 
            opts = internal.prepControl(opts, 'span');
            return Csw.controls.span(opts);
        };

        external.input = function (opts) {
            /// <summary> Creates a Csw.input on this element</summary>
            /// <param name="inputOpts" type="Object">Options to define the input.</param>
            /// <returns type="Object">A Csw.input</returns> 
            opts = internal.prepControl(opts, 'input');
            return Csw.controls.input(opts);
        };

        external.textArea = function (opts) {
            /// <summary> Creates a Csw.textArea on this element</summary>
            /// <param name="inputOpts" type="Object">Options to define the textArea.</param>
            /// <returns type="Object">A Csw.textArea</returns>
            opts = internal.prepControl(opts, 'textArea');
            return Csw.controls.textArea(opts);
        };

        external.timeInterval = function (opts) {
            /// <summary> Creates a Csw.timeInterval on this element</summary>
            /// <param name="inputOpts" type="Object">Options to define the timeInterval.</param>
            /// <returns type="Object">A Csw.timeInterval</returns>
            opts = internal.prepControl(opts, 'timeInterval');
            return Csw.controls.timeInterval(opts);
        };

        external.button = function (opts) {
            /// <summary> Creates a Csw.button on this element</summary>
            /// <param name="buttonOpts" type="Object">Options to define the button.</param>
            /// <returns type="Object">A Csw.button</returns> 
            opts = internal.prepControl(opts, 'button');
            return Csw.controls.button(opts);
        };

        external.link = function (opts) {
            /// <summary> Creates a Csw.link on this element</summary>
            /// <param name="buttonOpts" type="Object">Options to define the link.</param>
            /// <returns type="Object">A Csw.link</returns> 
            opts = internal.prepControl(opts, 'link');
            return Csw.controls.link(opts);
        };

        external.form = function (opts) {
            /// <summary> Creates a Csw.form on this element</summary>
            /// <param name="formOpts" type="Object">Options to define the form.</param>
            /// <returns type="Object">A Csw.form</returns> 
            opts = internal.prepControl(opts, 'form');
            return Csw.controls.form(opts);
        };

        external.img = function (opts) {
            /// <summary> Creates a Csw.img on this element</summary>
            /// <param name="formOpts" type="Object">Options to define the img.</param>
            /// <returns type="Object">A Csw.img</returns>
            opts = internal.prepControl(opts, 'img');
            return Csw.controls.img(opts);
        };

        external.select = function (opts) {
            /// <summary> Creates a Csw.select on this element</summary>
            /// <param name="formOpts" type="Object">Options to define the select.</param>
            /// <returns type="Object">A Csw.select</returns>
            opts = internal.prepControl(opts, 'select');
            return Csw.controls.select(opts);
        };

        external.option = function (opts) {
            /// <summary> Creates a Csw.option on this element</summary>
            /// <param name="formOpts" type="Object">Options to define the option.</param>
            /// <returns type="Object">A Csw.option</returns>
            opts = internal.prepControl(opts, 'option');
            return Csw.controls.option(opts);
        };

        external.jquery = function ($jqElement, opts) {
            /// <summary> Extend a jQuery object with Csw methods.</summary>
            /// <param name="$element" type="jQuery">Element to extend.</param>
            /// <returns type="jquery">A Csw.jquery object</returns>
            opts = internal.prepControl(opts, 'jquery');
            return factory($jqElement, opts);
        };

        external.valueOf = function () {
            return external;
        };

        //#endregion Csw DOM classes

        //#region Csw "jQuery" classes

        external.parent = external.parent || function () {
            /// <summary>Get the parent of this control</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.parent();
            var ret = external.jquery(_$element);
            return ret;
        };

        external.addClass = function (name) {
            /// <summary>Add a CSS class to an element.</summary>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.addClass($element, name);
            return external;
        };

        external.removeClass = function (name) {
            /// <summary>Remove a CSS class to an element.</summary>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.removeClass($element, name);
            return external;
        };

        external.css = function (values) {
            /// <summary>Add css styles to an element.</summary>
            /// <param name="values" type="Object">Styles to apply</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.css(values);
            return external;
        };

        external.bind = function (eventName, event) {
            /// <summary>Bind an action to a jQuery element's event.</summary>
            /// <param name="eventName" type="String">The name of the event</param>
            /// <param name="event" type="Function">A function to execute when the event fires</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.bind($element, eventName, event);
            return external;
        };
        external.trigger = function (eventName, eventOpts) {
            /// <summary>Trigger an event bound to a jQuery element.</summary>
            /// <param name="eventName" type="String">The name of the event</param>
            /// <param name="eventOpts" type="Object">Options collection to pass to the event handler.</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.trigger($element, eventName, eventOpts);
            return external;
        };

        external.children = function (searchTerm, selector) {
            /// <summary>Find the child elements of this DOM element represented by this object</summary>
            /// <param name="searchTerm" type="String">(Optional) Some search term to limit child results</param>
            /// <param name="selector" type="String">(Optional) A selector</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.children(Csw.string(searchTerm), Csw.string(selector));
            var _options = {
                parent: function () { return external; }
            };
            var ret = external.jquery(_$element, _options);
            return ret;
        };

        external.find = function (selector) {
            /// <summary>Find the child elements of this DOM element represented by this object</summary>
            /// <param name="selector" type="String">A selector, id or jQuery object to find.</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.find(Csw.string(selector));
            var ret = external.jquery(_$element);
            return ret;
        };

        external.filter = function (selector) {
            /// <summary>Filter the child elements of this DOM element according to this selector</summary>
            /// <param name="selector" type="String">A filter string.</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.filter(selector);
            var ret = external.jquery(_$element);
            return ret;
        };

        external.first = function () {
            /// <summary>Find the first child element of this DOM element represented by this object</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.first();
            var _options = {
                parent: function () { return external; }
            };
            var ret = external.jquery(_$element, _options);
            return ret;
        };

        external.append = function (object) {
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
                parent: function () { return external; }
            };
            var ret = factory(_$element, _options);
            return ret;
        };

        external.val = function (value) {
            /// <summary>Get the value of the element.</summary>
            /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
            if (arguments.length === 1 && false === Csw.isNullOrUndefined(value)) {
                $element.val(value);
                return external;
            } else {
                return Csw.string($element.val());
            }
        };

        external.text = function (text) {
            /// <summary>Get the value of the element.</summary>
            /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
            if (arguments.length === 1 && false === Csw.isNullOrUndefined(text)) {
                $element.text(text);
                return external;
            } else {
                return Csw.string($element.text());
            }
        };

        external.show = function () {
            /// <summary>Make the element visible.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.show();
            return external;
        };

        external.hide = function () {
            /// <summary>Make the element invisible.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.hide();
            return external;
        };

        external.empty = function () {
            /// <summary>Empty the element.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.empty();
            return external;
        };

        external.clickOnEnter = function (cswControl) {
            /// <summary>Bind an event to the enter key, when pressed in this control.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.clickOnEnter(cswControl.$);
            return external;
        };

        external.data = function (prop, val) {
            /// <summary>Store property data on the control.</summary>
            /// <returns type="Object">All properties, a single property, or the control if defining a property (for chaining).</returns> 
            var ret = '',
                _internal = Csw.clientDb.getItem('control_data_' + internal.id) || {};
            switch (arguments.length) {
                case 0:
                    ret = _internal;
                    break;
                case 1:
                    ret = _internal[prop];
                    break;
                case 2:
                    _internal[prop] = val;
                    Csw.clientDb.setItem('control_data_' + internal.id, _internal);
                    ret = external;
                    break;
            }
            return ret;

        };

        //#endregion Csw "jQuery" classes

        return external;
    }
    Csw.controls.register('factory', factory);
    Csw.controls.factory = Csw.controls.factory || factory;

} ());


