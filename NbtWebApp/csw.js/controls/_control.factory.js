/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    function factory($element, external) {
        /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
        /// <param name="$element" type="jQuery">An element to bind to.</param>
        /// <param name="options" type="Object">An options collection to extend.</param>
        /// <returns type="Object">The options object with DOM methods attached.</returns> 

        //#region internal

        var internal = {};
        external = external || {};
        if (Csw.isJQuery($element)) {
            external.$ = $element;
            internal.id = Csw.string($element.prop('id'));
            external.isValid = true;
        } else {
            internal.id = '';
            external.$ = {};
        }
        internal.data = {};
        internal.prepControl = function (opts, controlName) {
            opts = opts || {};
            opts.ID = opts.ID || Csw.controls.dom.makeId(internal.id, 'sub', controlName);
            opts.$parent = $element;
            opts.root = external.root;
            opts.parent = function () {
                return external;
            };
            
            return opts;
        };

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


        delete external.ID;
        delete external.$parent;
        delete external.prepControl;

        //#endregion internal

        //#region Csw prop classes

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
            if (arguments.length === 2 || Csw.isPlainObject(name)) {
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
            if (arguments.length === 2 || Csw.isPlainObject(name)) {
                ret = external;
            }
            return ret;
        };

        //#endregion Csw prop classes

        //#region Csw DOM classes

        external.br = function (opts) {
            /// <summary> Creates a Csw.br on this element</summary>
            /// <param name="opts" type="Object">Options to define the br.</param>
            /// <returns type="Object">A Csw.br</returns> 
            opts = internal.prepControl(opts, 'br');
            return Csw.controls.br(opts);
        };

        external.button = function (opts) {
            /// <summary> Creates a Csw.button on this element</summary>
            /// <param name="opts" type="Object">Options to define the button.</param>
            /// <returns type="Object">A Csw.button</returns> 
            opts = internal.prepControl(opts, 'button');
            return Csw.controls.button(opts);
        };

        external.checkBoxArray = function (opts) {
            /// <summary> Creates a Csw.checkBoxArray on this element</summary>
            /// <param name="opts" type="Object">Options to define the checkBoxArray.</param>
            /// <returns type="Object">A Csw.checkBoxArray</returns>
            opts = internal.prepControl(opts, 'checkBoxArray');
            return Csw.controls.checkBoxArray(opts);
        };

        external.comboBox = function (opts) {
            /// <summary> Creates a Csw.comboBox on this element</summary>
            /// <param name="opts" type="Object">Options to define the comboBox.</param>
            /// <returns type="Object">A Csw.comboBox</returns>
            opts = internal.prepControl(opts, 'comboBox');
            return Csw.controls.comboBox(opts);
        };

        external.dateTimePicker = function (opts) {
            /// <summary> Creates a Csw.dateTimePicker on this element</summary>
            /// <param name="opts" type="Object">Options to define the dateTimePicker.</param>
            /// <returns type="Object">A Csw.dateTimePicker</returns>
            opts = internal.prepControl(opts, 'dateTimePicker');
            return Csw.controls.dateTimePicker(opts);
        };

        external.div = function (opts) {
            /// <summary> Creates a Csw.div on this element</summary>
            /// <param name="opts" type="Object">Options to define the div.</param>
            /// <returns type="Object">A Csw.div</returns> 
            opts = internal.prepControl(opts, 'div');
            return Csw.controls.div(opts);
        };

        external.grid = function (opts) {
            /// <summary> Creates a Csw.grid on this element</summary>
            /// <param name="opts" type="Object">Options to define the grid.</param>
            /// <returns type="Object">A Csw.grid</returns>
            opts = internal.prepControl(opts, 'grid');
            return Csw.controls.grid(opts);
        };

        external.form = function (opts) {
            /// <summary> Creates a Csw.form on this element</summary>
            /// <param name="opts" type="Object">Options to define the form.</param>
            /// <returns type="Object">A Csw.form</returns> 
            opts = internal.prepControl(opts, 'form');
            return Csw.controls.form(opts);
        };

        external.imageButton = function (opts) {
            /// <summary> Creates a Csw.imageButton on this element</summary>
            /// <param name="opts" type="Object">Options to define the imageButton.</param>
            /// <returns type="Object">A Csw.imageButton</returns>
            opts = internal.prepControl(opts, 'imageButton');
            return Csw.controls.imageButton(opts);
        };

        external.img = function (opts) {
            /// <summary> Creates a Csw.img on this element</summary>
            /// <param name="opts" type="Object">Options to define the img.</param>
            /// <returns type="Object">A Csw.img</returns>
            opts = internal.prepControl(opts, 'img');
            return Csw.controls.img(opts);
        };

        external.jquery = function ($jqElement, opts) {
            /// <summary> Extend a jQuery object with Csw methods.</summary>
            /// <param name="$element" type="jQuery">Element to extend.</param>
            /// <returns type="jquery">A Csw.jquery object</returns>
            opts = internal.prepControl(opts, 'jquery');
            return factory($jqElement, opts);
        };

        external.input = function (opts) {
            /// <summary> Creates a Csw.input on this element</summary>
            /// <param name="opts" type="Object">Options to define the input.</param>
            /// <returns type="Object">A Csw.input</returns> 
            opts = internal.prepControl(opts, 'input');
            return Csw.controls.input(opts);
        };

        external.layoutTable = function (opts) {
            /// <summary> Creates a Csw.layoutTable on this element</summary>
            /// <param name="opts" type="Object">Options to define the table.</param>
            /// <returns type="Object">A Csw.layoutTable</returns> 
            opts = internal.prepControl(opts, 'layoutTable');
            return Csw.controls.layoutTable(opts);
        };

        external.li = function (opts) {
            /// <summary> Creates a Csw.li on this element</summary>
            /// <param name="opts" type="Object">Options to define the li.</param>
            /// <returns type="Object">A Csw.li</returns> 
            opts = internal.prepControl(opts, 'li');
            return Csw.controls.li(opts);
        };

        external.link = function (opts) {
            /// <summary> Creates a Csw.link on this element</summary>
            /// <param name="opts" type="Object">Options to define the link.</param>
            /// <returns type="Object">A Csw.link</returns> 
            opts = internal.prepControl(opts, 'link');
            return Csw.controls.link(opts);
        };

        external.multiSelect = function (opts) {
            /// <summary> Creates a Csw.multiSelect on this element</summary>
            /// <param name="opts" type="Object">Options to define the multiSelect.</param>
            /// <returns type="Object">A Csw.multiSelect</returns>
            opts = internal.prepControl(opts, 'multiSelect');
            return Csw.controls.multiSelect(opts);
        };

        external.numberTextBox = function (opts) {
            /// <summary> Creates a Csw.numberTextBox on this element</summary>
            /// <param name="opts" type="Object">Options to define the numberTextBox.</param>
            /// <returns type="Object">A Csw.numberTextBox</returns>
            opts = internal.prepControl(opts, 'numberTextBox');
            return Csw.controls.numberTextBox(opts);
        };

        external.option = function (opts) {
            /// <summary> Creates a Csw.option on this element</summary>
            /// <param name="opts" type="Object">Options to define the option.</param>
            /// <returns type="Object">A Csw.option</returns>
            opts = internal.prepControl(opts, 'option');
            return Csw.controls.option(opts);
        };

        external.p = function (opts) {
            /// <summary> Creates a Csw.p on this element</summary>
            /// <param name="opts" type="Object">Options to define the p.</param>
            /// <returns type="Object">A Csw.p</returns>
            opts = internal.prepControl(opts, 'p');
            return Csw.controls.p(opts);
        };

        external.select = function (opts) {
            /// <summary> Creates a Csw.select on this element</summary>
            /// <param name="opts" type="Object">Options to define the select.</param>
            /// <returns type="Object">A Csw.select</returns>
            opts = internal.prepControl(opts, 'select');
            return Csw.controls.select(opts);
        };

        external.span = function (opts) {
            /// <summary> Creates a Csw.span on this element</summary>
            /// <param name="opts" type="Object">Options to define the span.</param>
            /// <returns type="Object">A Csw.span</returns> 
            opts = internal.prepControl(opts, 'span');
            return Csw.controls.span(opts);
        };

        external.tabDiv = function (opts) {
            /// <summary> Creates a Csw.tabDiv on this element</summary>
            /// <param name="opts" type="Object">Options to define the div.</param>
            /// <returns type="Object">A Csw.tabDiv</returns> 
            opts = internal.prepControl(opts, 'tabdiv');
            return Csw.controls.tabDiv(opts);
        };

        external.table = function (opts) {
            /// <summary> Creates a Csw.table on this element</summary>
            /// <param name="opts" type="Object">Options to define the table.</param>
            /// <returns type="Object">A Csw.table</returns> 
            opts = internal.prepControl(opts, 'table');
            return Csw.controls.table(opts);
        };

        external.textArea = function (opts) {
            /// <summary> Creates a Csw.textArea on this element</summary>
            /// <param name="opts" type="Object">Options to define the textArea.</param>
            /// <returns type="Object">A Csw.textArea</returns>
            opts = internal.prepControl(opts, 'textArea');
            return Csw.controls.textArea(opts);
        };

        external.thinGrid = function (opts) {
            /// <summary> Creates a Csw.thinGrid on this element</summary>
            /// <param name="opts" type="Object">Options to define the thinGrid.</param>
            /// <returns type="Object">A Csw.thinGrid</returns>
            opts = internal.prepControl(opts, 'thinGrid');
            return Csw.controls.thinGrid(opts);
        };

        external.timeInterval = function (opts) {
            /// <summary> Creates a Csw.timeInterval on this element</summary>
            /// <param name="opts" type="Object">Options to define the timeInterval.</param>
            /// <returns type="Object">A Csw.timeInterval</returns>
            opts = internal.prepControl(opts, 'timeInterval');
            return Csw.controls.timeInterval(opts);
        };

        external.triStateCheckBox = function (opts) {
            /// <summary> Creates a Csw.triStateCheckBox on this element</summary>
            /// <param name="opts" type="Object">Options to define the triStateCheckBox.</param>
            /// <returns type="Object">A Csw.triStateCheckBox</returns>
            opts = internal.prepControl(opts, 'triStateCheckBox');
            return Csw.controls.triStateCheckBox(opts);
        };

        external.ul = function (opts) {
            /// <summary> Creates a Csw.ul on this element</summary>
            /// <param name="opts" type="Object">Options to define the ul.</param>
            /// <returns type="Object">A Csw.ul</returns> 
            opts = internal.prepControl(opts, 'ul');
            return Csw.controls.ul(opts);
        };

        external.valueOf = function () {
            return external;
        };

        //#endregion Csw DOM classes

        //#region Csw "jQuery" classes

        external.addClass = function (name) {
            /// <summary>Add a CSS class to an element.</summary>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.addClass($element, name);
            return external;
        };

        external.append = function (object) {
            /// <summary>Append an object to this element.</summary>
            /// <param name="object" type="Object">Raw HTML, a jQuery object or text.</param>
            /// <returns type="Object">The parent Csw object (for chaining)</returns> 
            try {
                $element.append(object);
            } catch (e) {
                Csw.log('Warning: append() failed, text() was used instead.', true);
                if (Csw.isString(object)) {
                    $element.text(object);
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
                    $element.append($child);
                }
            } catch (e) {
                /* One day we'll implement client-side error handling */
            }
            ret = internal.makeControlForChain($child);
            return ret;
        };

        external.bind = function (eventName, event) {
            /// <summary>Bind an action to a jQuery element's event.</summary>
            /// <param name="eventName" type="String">The name of the event</param>
            /// <param name="event" type="Function">A function to execute when the event fires</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.bind($element, eventName, event);
            return external;
        };

        external.children = function (searchTerm, selector) {
            /// <summary>Find the child elements of this DOM element represented by this object</summary>
            /// <param name="searchTerm" type="String">(Optional) Some search term to limit child results</param>
            /// <param name="selector" type="String">(Optional) A selector</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.children(Csw.string(searchTerm), Csw.string(selector)),
                ret = internal.makeControlForChain(_$element);
            return ret;
        };

        external.clickOnEnter = function (cswControl) {
            /// <summary>Bind an event to the enter key, when pressed in this control.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.clickOnEnter(cswControl.$);
            return external;
        };

        external.css = function (param1, param2) {
            /// <summary>Add css styles to an element.</summary>
            /// <param name="param1" type="Object">Either a single JSON object with CSS to apply, or a single CSS name</param>
            /// <param name="param2" type="string">single CSS value</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            if (arguments.length===1) {
                $element.css(param1);
            } else {
                $element.css(param1,param2);
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
                    ret = _internal || $element.data();
                    break;
                case 1:
                    ret = _internal[prop] || $element.data(prop);
                    break;
                case 2:
                    _internal[prop] = val;
                    $element.data(prop, val);
                    Csw.clientDb.setItem('control_data_' + internal.id, _internal);
                    ret = external;
                    break;
            }
            return ret;

        };

        external.empty = function () {
            /// <summary>Empty the element.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.empty();
            return external;
        };

        external.filter = function (selector) {
            /// <summary>Filter the child elements of this DOM element according to this selector</summary>
            /// <param name="selector" type="String">A filter string.</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.filter(selector),
                ret = internal.makeControlForChain(_$element);
            return ret;
        };

        external.find = function (selector) {
            /// <summary>Find the child elements of this DOM element represented by this object</summary>
            /// <param name="selector" type="String">A selector, id or jQuery object to find.</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.find(Csw.string(selector)),
                ret = internal.makeControlForChain(_$element);
            return ret;
        };

        external.first = function () {
            /// <summary>Find the first child element of this DOM element represented by this object</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.first(),
                ret = internal.makeControlForChain(_$element);
            return ret;
        };

        external.hide = function () {
            /// <summary>Make the element invisible.</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            $element.hide();
            return external;
        };

        external.parent = external.parent || function () {
            /// <summary>Get the parent of this control</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.parent(),
                ret;

            if (false === Csw.isNullOrEmpty(_$element, true)) {
                ret = external.jquery(_$element);
            } else {
                ret = {};
            }
            return ret;
        };

        external.remove = function () {
            /// <summary>Remove the element and delete the object.</summary>
            /// <returns type="null"></returns> 
            $element.remove();
            external = null;
            return external;
        };

        external.removeClass = function (name) {
            /// <summary>Remove a CSS class to an element.</summary>
            /// <param name="value" type="String">The value of the attribute</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.removeClass($element, name);
            return external;
        };

        external.root = external.root || function () {
            /// <summary>Get the root (great, great, great grandparent) of this control</summary>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            var _$element = $element.parent(),
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
            $element.show();
            return external;
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

        external.trigger = function (eventName, eventOpts) {
            /// <summary>Trigger an event bound to a jQuery element.</summary>
            /// <param name="eventName" type="String">The name of the event</param>
            /// <param name="eventOpts" type="Object">Options collection to pass to the event handler.</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.trigger($element, eventName, eventOpts);
            return external;
        };

        external.unbind = function (eventName) {
            /// <summary>Unbind an action from a jQuery element's event.</summary>
            /// <param name="eventName" type="String">The name of the event</param>
            /// <returns type="Object">The Csw object (for chaining)</returns> 
            Csw.controls.dom.unbind($element, eventName);
            return external;
        };

        external.val = external.val || function (value) {
            /// <summary>Get the value of the element.</summary>
            /// <returns type="String">If get(), the value. If set(val), the Csw object (for chaining).</returns> 
            if (arguments.length === 1 && false === Csw.isNullOrUndefined(value)) {
                $element.val(value);
                return external;
            } else {
                return Csw.string($element.val());
            }
        };

        //#endregion Csw "jQuery" classes

        return external;
    }
    Csw.controls.register('factory', factory);
    Csw.controls.factory = Csw.controls.factory || factory;

} ());


